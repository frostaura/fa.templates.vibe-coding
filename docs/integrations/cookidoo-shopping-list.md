# Cookidoo shopping list to Woolworths cart

## Goal

A user picks recipes in the Cookidoo (Thermomix) app, shares the resulting
shopping list, and gets a Woolworths South Africa cart already filled with a
product for every ingredient — ready to review and check out by hand.

## Actors

- **Primary:** the account holder, through any of the three surfaces — an AI
  agent calling `woolworths_add_shopping_list_to_cart` over MCP, a script or
  Siri Shortcut calling `POST /api/woolworths/shopping-list/cart`, or a human
  with `curl`.
- **Secondary:** Cookidoo, which produces the shared text and is never called by
  this service; Constructor.io, which serves the Woolworths catalogue search
  anonymously; Woolworths South Africa, which owns the account and the cart.

## Preconditions

- The caller holds the account holder's Woolworths email and password and sends
  them as `X-Woolworths-Username` and `X-Woolworths-Password`. The service holds
  no credentials of its own — see [the system overview](architecture.md).
- The connection is HTTPS. The password is in a header and TLS is the only thing
  protecting it.
- The Woolworths account has already confirmed a delivery address. The first
  food added to a cart on a fresh account triggers an address-confirmation step
  that this flow does not implement.

The preview half of this use case has none of these preconditions: resolving a
list without touching a cart needs no account at all.

## Main flow

1. **Share.** The user shares the shopping list from Cookidoo as plain text. The
   format is indentation-based: ALL-CAPS section headings
   (`VEGETABLES & FRESH HERBS`) followed by indented ingredient lines that may or
   may not carry a leading quantity and unit.

2. **Parse.** `ShoppingListParser.Parse` splits the text into
   `ShoppingListItem`s. Headings are recognised by being all-caps and not
   starting with a digit, and are recorded as the category of the lines beneath
   them rather than becoming ingredients. A leading quantity and unit
   (`g`, `kg`, `ml`, `l`, `tsp`, `tbsp`, `cup`, `pinch`, `clove`…) are stripped
   into their own fields, with fractional values such as `0.33333` parsed under
   the invariant culture. Lines with no quantity — `grated Parmesan cheese` — are
   kept as-is.

   The parsed `Quantity` is the amount the *recipe* needs, not the number of
   retail units to buy. Nothing in this flow converts one to the other; every
   product is added with quantity 1.

3. **Build search phrases.** `ShoppingListParser.ToQueries` turns each ingredient
   into an ordered list of search phrases, most specific first. Parentheticals
   are stripped (`pouring (whipping) cream` → `pouring cream`), the vocabulary
   map is applied (below), then preparation words are progressively removed —
   `fresh`, `dried`, `grated`, `skinless`, `chopped`, `baby`, `leaves` and
   similar — and as a last resort the trailing word pair and then the head noun
   alone. `fresh baby spinach leaves` yields the full phrase first and
   `spinach` at the end. Duplicates are never emitted twice.

4. **Resolve.** `WoolworthsSearchClient.ResolveIngredientAsync` tries each phrase
   in order against the Constructor.io catalogue and stops at the first that
   returns anything, keeping up to five candidates and reporting which phrase
   worked. Search is anonymous; no sign-in has happened yet.

5. **Sign in.** `WoolworthsCartService.AddShoppingListAsync` creates one
   `WoolworthsClient` for the whole run and signs in with the caller's
   credentials.

6. **Add, one ingredient at a time.** For each ingredient, the top candidate is
   POSTed to the cart on its own. Batching is prohibited: a batch containing one
   unavailable product silently adds nothing while returning HTTP 200. See
   [the Woolworths contract](woolworths.md).

7. **Fall back on unavailability.** If a candidate is refused, the next candidate
   is tried, and so on down the list of up to five. The candidate that succeeded
   becomes the line's product and the rest are reported as alternatives, so the
   caller can offer a swap.

8. **Read the cart, then sign out.** The final item count and order total come
   from `GET /server/cartDetails` rather than from the running tally, and the
   session is signed out in a `finally`.

9. **Open the cart.** The response carries `cartUrl`
   (`https://www.woolworths.co.za/check-out/cart`), a per-ingredient breakdown,
   and the added count. The user opens the cart, reviews substitutions and
   quantities, and checks out themselves.

### The vocabulary problem

Cookidoo emits Australian and European product vocabulary, and the South African
catalogue does not stock those words. Without translation the search does not
return a poor match — it returns nothing at all, which reads as "Woolworths
doesn't sell parsley".

`ShoppingListParser.Synonyms` maps them before searching. The two that matter
most, both covered by tests:

- **capsicum → pepper** (and `red capsicum` → `red pepper`, and the green and
  yellow variants).
- **flat-leaf parsley → Italian parsley**, which is what the product is called
  locally.

Others in the map: `aubergine` → `brinjal`, `courgette` and `zucchini` → `baby
marrow`, `thickened cream` → `whipping cream`, `caster sugar` → `castor sugar`,
`plain flour` → `cake flour`, `chicken stock paste` → `chicken stock`.
Multi-word keys are matched against the whole ingredient; single-word keys are
substituted word by word, so `red capsicum strips` still becomes `red pepper
strips`.

This map is load-bearing, and it is also incomplete by nature — it covers the
vocabulary encountered so far, not the whole AU/EU–SA delta. A new ingredient
that silently returns no results is the first thing to check here.

### Preview

`POST /api/woolworths/shopping-list/preview` and
`woolworths_preview_shopping_list` run steps 1 through 4 and stop. Every line
comes back with status `Resolved`, the cart is never touched, and no credentials
are required. This is the intended first call for an AI agent: it lets the
matches be shown and corrected before anything is committed, and it is the only
part of the flow safe to run on behalf of someone whose credentials the caller
does not hold.

## Alternate and failure flows

| Situation | Line status | What the caller sees |
| --- | --- | --- |
| No search phrase returned anything | `NoMatch` | The ingredient, no product, "No matching product found." |
| Candidates exist but every one was refused | `Unavailable` | The top candidate as context, the alternatives, and the upstream refusal message from the last attempt |
| Top candidate refused, a later one added | `Added` | The product that actually went in, with the others listed as alternatives |
| Preview run | `Resolved` | The match, deliberately not added |

Whole-run failures:

- **Missing credential headers** — `MissingCredentialsException`, surfaced as
  HTTP 401 naming the exact headers, or as an `McpException` with the same text
  plus a note to configure them on the MCP server connection.
- **Wrong credentials** — Woolworths answers HTTP 200 with an `errorMessage`
  body; this becomes `ProviderAuthenticationException` and HTTP 401.
- **Missing session cookies** — a 301 on the login POST, reported as a session
  bootstrap failure rather than as bad credentials.
- **Upstream timeout or transport failure** — HTTP 504 and 502 respectively.
- **Sign-out did not take** — logged as a warning and never allowed to fail the
  run or mask its result.

A partially filled cart is a normal outcome, not an error. The response always
reports per-ingredient status, so "13 of 15 added" is visible rather than
inferred.

## What this deliberately does not do

- **No checkout, no payment.** The flow fills a cart and stops. A human reviews
  and completes the order.
- **No quantity arithmetic.** Recipe quantities are parsed and reported but never
  translated into pack counts; every product is added as one unit.
- **No credential storage.** Nothing is remembered between requests.
- **No batching**, for the correctness reason above.

## Acceptance criteria

- [ ] A shared Cookidoo list parses into one item per ingredient, with section
      headings excluded and quantities separated from names.
- [ ] `capsicum` and `flat-leaf parsley` produce South African search phrases
      before any Woolworths call is made.
- [ ] Preview resolves a list and returns per-line matches with no credentials
      supplied and no cart mutation.
- [ ] Every cart addition is a separate HTTP request.
- [ ] An ingredient whose best match is unavailable is retried against the next
      candidate before being reported as failed.
- [ ] The response distinguishes `Added`, `Unavailable` and `NoMatch` per
      ingredient, and reports a cart total read from the cart itself.
- [ ] The session is signed out even when the run fails part-way.

The parser criteria are covered by `ShoppingListParserTests`. The rest can only
be confirmed against a live Woolworths account.

## Verified behaviour

Run against the fifteen-ingredient sample at
`docs/fixtures/cookidoo-share-sample.txt`, signed in to a real account:

- **15 of 15 ingredients resolved** to sensible products.
- **14 were added** to a real cart, totalling **R780.46**.
- The one that needed the fallback was the parmesan: `parmesan cheese grated`
  resolved first to an unavailable SKU, and the next candidate — Parmigiano
  Reggiano — added cleanly. This is the candidate-walk behaviour working exactly
  as intended, and it is why the fallback exists.

Note that resolution and addition are different measurements: 15/15 is the
search-and-match result, 14 is what was in the cart. That gap is the point of
reporting both.

This is a single observed run, not a regression test. It cannot be re-run
automatically — it needs live credentials and a real cart to inspect — so it
should be treated as evidence that the pipeline works, not as a guarantee that
it still does.

## References

- [System overview](architecture.md)
- [Woolworths integration](woolworths.md)
- `docs/fixtures/cookidoo-share-sample.txt` — the verbatim share used above and
  by `ShoppingListParserTests`
- `src/Gaia.Mcp.Server/Integrations/ShoppingListParser.cs`
- `src/Gaia.Mcp.Server/Integrations/Woolworths/WoolworthsCartService.cs`
