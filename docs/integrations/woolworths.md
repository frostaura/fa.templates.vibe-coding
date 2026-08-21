# Woolworths South Africa — integration contract

## Context

Woolworths South Africa publishes no API. There is no developer portal, no
documentation, no versioning, no API key and no OAuth. Every endpoint described
here was established by capturing traffic from the live site while signed in as
a real account, and re-verified from plain `curl` where noted.

That has a direct consequence for how this document should be read: it is the
authoritative description of upstream behaviour for this project, because no
vendor document exists to defer to — and it is simultaneously a snapshot of
observed behaviour that the vendor never promised and can change without notice.

The endpoints were verified to work from a plain HTTP client with a cookie jar.
There is no JavaScript challenge and no bot wall on the paths used here, which
is what makes a server-side implementation viable at all.

## Decision

Drive the storefront's own endpoints with a cookie-bearing HTTP client that
imitates the site's SPA, treat every HTTP status code as uninformative, and add
cart items strictly one at a time.

## Structure

`WoolworthsClient` is one authenticated session: its own `CookieContainer`,
`AllowAutoRedirect = false`, a 30-second timeout, and `IDisposable`.
`WoolworthsSearchClient` is separate and anonymous, because search is not served
by Woolworths at all. `WoolworthsCartService` orchestrates the two.
`WoolworthsIntegration` declares the credential fields.

### Required headers

Every call the session makes carries these, and they are what distinguishes an
app call from a scrape. Omitting them is the difference between a JSON reply and
an HTML error page.

```
x-requested-by: Woolworths Online
x-source-chanel: web
accept: application/json, text/plain, */*
user-agent: <a real browser UA>
```

`x-source-chanel` is spelled that way upstream — "chanel", not "channel". It is
not a typo in this codebase and must not be "corrected".

Authentication is entirely cookie-based. There is no bearer token, no
`Authorization` header, and no CSRF token on any of these calls.

### Cookie bootstrap — required before login

```
GET /
```

The homepage sets `__cf_bm`, `TS01a6902f`, `TS01576f1e`,
`f5avraaaaaaaaaaaaaaaa_session_`, `dtCookie` and `by`. These are Cloudflare and
load-balancer cookies, and they gate everything that follows.

**Posting to `/server/login` without them returns a 301 to `/500//login`.** With
them, the identical request succeeds. This is the single biggest trap in the
integration, and it fails in the least helpful possible way — a redirect to an
error path rather than any message about session state. `WoolworthsClient`
therefore treats a 301 or 302 on the login response as a missing-bootstrap
condition and raises `ProviderAuthenticationException` saying so, rather than
letting the caller guess at bad credentials.

### Login — base64 password

```
POST /server/login
{"login":"you@example.com","password":"<base64 of the plaintext password>"}
```

The password is **base64-encoded, not hashed**. `aW52YWxpZA==` decodes to
`invalid`. This is obfuscation and nothing more; TLS is what actually protects
the credential. The endpoint rejects a plaintext password, so the encoding must
be matched, but no one should read it as a security measure — least of all as a
reason to treat the encoded form as safe to log.

Success returns `{corporateNumber, profileId, successMessage}`. **Failure
returns HTTP 200** with `{"statusId":"200","errorMessage":"Login failed..."}`.
`SignInAsync` therefore checks the body for `errorMessage` first, then requires a
non-empty `profileId`, and only then marks the session signed in.

### Search — Constructor.io, third party, no auth

The storefront delegates search to Constructor.io using a public client key
baked into the page bundle.

```
GET https://wpkmgeuco-zone.cnstrc.com/v1/search/{query}
    ?key=key_tw9hKe0fkfgEf36D
    &filters[visibility]=all
    &filters[visibility]=web and app
    &us=default
    &num_results_per_page={n}&page=1&s=1&i={any uuid}
```

This needs no cookies and no login, which is what makes anonymous search and
preview possible in this service.

**The `filters[visibility]` parameter must be sent twice** — once as `all` and
once as `web and app` — exactly as the storefront does. Sending only
`web and app` returns **zero food results** while continuing to return non-food:
"baby spinach", "parmesan cheese" and "paprika" all come back empty, while
"garlic" returns garlic *presses* from homeware. Omitting the filter entirely
also works. This behaviour is indistinguishable from a location-gated catalogue
and cost real debugging time to identify; the doubled parameter in
`WoolworthsSearchClient` is deliberate and must not be deduplicated by a
tidy-up pass or by a query-string builder that collapses repeated keys.

Each result gives `value` (the product name), `data.id`, `data.p10` (price) and
`data.url`. Price parsing uses `CultureInfo.InvariantCulture` on purpose — a
comma-decimal host locale would otherwise silently drop every price — and
tolerates the value arriving as either a JSON number or a string.

### `data.id` is `catalogRefId`

The identifier a search result returns as `data.id` is the same identifier the
cart endpoint expects as `catalogRefId` (and as `productId`). That equivalence
is the join that makes the whole pipeline work: search output feeds add-to-cart
directly, with no lookup step in between. Do not "fix" this by introducing one.

It is also an assumption with no contract behind it. If Woolworths ever splits
the two identifier spaces, the failure will present as every product being
unavailable rather than as anything resembling an id error.

### Add to cart — one item per request

```
POST /server/cartAddItems
{
  "deliveryType": "Standard",
  "fromDeliverySelectionPopup": "true",
  "items": [
    {"productId":"6009226280856","catalogRefId":"6009226280856",
     "quantity":1,"itemListName":"Product Description Page"}
  ]
}
```

Success returns `{message, productCountMap, groupSubTotal}`, from which
`productCountMap.totalProductCount` and `groupSubTotal.orderTotal` are read.

**The `items` array accepts multiple products, and it must never be used that
way.** A two-item batch worked. A thirteen-item batch containing **one**
unavailable product added **nothing at all** — silently, with HTTP 200 and a
response that did not identify which item was at fault. The whole batch was
voided by a single bad id.

This is why `WoolworthsClient.AddToCartAsync` sends exactly one item per call
and `WoolworthsCartService` loops. Batching here is not an optimisation that was
skipped for simplicity; it is a correctness hazard whose failure mode is the
worst available one — the caller is told the operation succeeded and the cart is
empty. The one-at-a-time rule costs one HTTP round trip per ingredient and is
the difference between thirteen items in the cart and a success message over
nothing.

### The `formexceptions` failure shape

An unavailable product returns **HTTP 200** with:

```json
{"links":[],"formexceptions":[
  {"id":"Product Description Page",
   "message":"Sorry DEAN, we don't have this product available.",
   "status":false}]}
```

`AddToCartAsync` checks for a non-empty `formexceptions` array before anything
else and returns an unsuccessful `AddItemOutcome` carrying the upstream message.
Note that the message is personalised with the account holder's name and is
surfaced to the caller as-is; it is the most useful description available of why
the item was refused, and it originates from the caller's own account.

Absence of `formexceptions` is not by itself success. If `productCountMap` is
also missing, the response is unrecognised and is treated as a failure rather
than optimistically as an add.

### Cart contents

```
GET /server/cartDetails?page=cart
```

Read for `productCountMap.totalProductCount` and `groupSubTotal.orderTotal`.
`WoolworthsCartService` calls this at the end of a run and prefers its numbers
over the running tally accumulated from individual adds — the cart itself is the
authority on what is in the cart.

### Sign out — POST only

```
POST /server/logout
```

**A `GET` to the same path returns 200, serves HTML, and does not end the
session** — `currentUser` still reports `loggedInStatus: 4` afterwards. Only a
POST works, after which `loggedInStatus` drops to `0`.

Because the endpoint gives a plausible-looking response either way, sign-out is
verified rather than assumed: `SignOutAsync` follows the POST with a
`GET /server/currentUser` and only reports success when `loggedInStatus` is `0`,
logging a warning otherwise. It never throws — a failed sign-out must not mask
the result of the work that preceded it.

### Delivery address — `address.placeId` is REQUIRED on every add

`cartAddItems` refuses every item with HTTP 200 and
`formexceptions: [{message: "Place Id is missing."}]` unless the payload carries
an `address.placeId` (a Google place id). A browser session doesn't hit this
because the site's address-confirmation modal has already run `confirmPlace`;
**a fresh headless session has no confirmed place, so the field is mandatory
there**. This was discovered when the first end-to-end write through this
service failed — the original traffic capture happened in a browser session
where the modal had already fired, which hid the requirement.

The integration resolves the placeId dynamically after sign-in:
`GET /server/savedAddress` → the entry named by `defaultProfileAddressNickName`
→ its `placeId`, falling back to the first saved address with a non-empty
`placeId` (legacy entries can carry an empty one). The resolved value is then
sent on every `cartAddItems` call.

An account with **no** saved address cannot add items at all; the client fails
those adds with an instruction to set a delivery address once at
woolworths.co.za. The related endpoints
`GET /server/validatePlace?addressNickname={nickname}` and
`POST /server/confirmPlace` (the modal's own flow) are deliberately not
implemented — reading the saved address is sufficient and mutates nothing.

## Invariants

- Bootstrap with `GET /` before any login attempt.
- Send both custom headers, including the misspelled `x-source-chanel`.
- Base64-encode the password; never log it in either form.
- Send `filters[visibility]` twice.
- Add exactly one cart item per request.
- Include `address.placeId` on every `cartAddItems` call.
- Check the response body — `errorMessage`, `formexceptions`, `loggedInStatus` —
  never the HTTP status, on every single call.
- POST to log out, then verify.

## Consequences

- Correct behaviour costs one HTTP request per ingredient plus a sign-in and a
  sign-out per operation. A fifteen-item list is roughly seventeen upstream
  calls, more when candidate fallback engages.
- Failures are attributable per ingredient rather than per run, which is what
  makes the per-line reporting in `CartBuildResult` possible.
- Every response has to be parsed defensively. `ReadJsonAsync` returns `null`
  rather than throwing when the body is not JSON, because several of these
  endpoints serve HTML on error.

## Fragility and unverified assumptions

Everything in this document is undocumented and unversioned, and the site can
change any of it without notice. The pieces most likely to break, in rough order
of exposure:

- **The Constructor.io client key** (`key_tw9hKe0fkfgEf36D`). It is a public key
  lifted from the page bundle and can be rotated at any time. Breakage presents
  as every search returning nothing.
- **The doubled `visibility` filter.** If the semantics change, searches will
  return non-food results with no error — the exact symptom that already misled
  once.
- **The `data.id` = `catalogRefId` join.** Breakage presents as every product
  being unavailable.
- **The batch-voiding behaviour** is the reason for the one-at-a-time rule, and
  it was observed on a thirteen-item batch. Whether it applies to every batch
  size and every kind of unavailability was not exhaustively tested and does not
  need to be — the rule is unconditional.
- **Cookie names** (`__cf_bm`, `TS*`, `f5avr*`) are Cloudflare and F5 artefacts
  and are not depended on individually; the client keeps whatever the homepage
  sets. That is deliberately less brittle than naming them.
- **The delivery-address flow is unimplemented and untested**, as noted above.
- **Rate limiting is unknown.** No throttling was observed at the volumes tested,
  and no limit has been established. A large list issuing one request per
  ingredient is the most likely thing to find one.

Because these endpoints fail with HTTP 200 and success-shaped bodies, anything
consuming this integration must fail loudly and visibly. A naive implementation
of this API silently adds nothing and reports success, and that is the outcome
every design choice above is defending against.

## Validation

- **Automated:** none of the upstream behaviour is under test. There is no
  contract to test against and no sandbox account; tests would either hit the
  live site with real credentials or assert against fixtures that cannot detect
  the drift that matters. `ShoppingListParser` — the one piece with no upstream
  dependency — is unit tested.
- **Manual:** the only real validation is a run against a live account with the
  resulting cart inspected in a browser. See
  [the Cookidoo use case](cookidoo-shopping-list.md) for what has
  actually been observed end to end.

## References

- [System overview](architecture.md)
- [Cookidoo shopping list to cart](cookidoo-shopping-list.md)
- `src/Gaia.Mcp.Server/Integrations/Woolworths/WoolworthsClient.cs`
- `src/Gaia.Mcp.Server/Integrations/Woolworths/WoolworthsSearchClient.cs`
- `src/Gaia.Mcp.Server/Integrations/Woolworths/WoolworthsCartService.cs`
