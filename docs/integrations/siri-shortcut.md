# Siri Shortcut: share a Cookidoo list into the Woolworths cart

The end-user surface for [the Cookidoo use case](cookidoo-shopping-list.md).
The Shortcut is deliberately thin: it collects text, posts it, and opens the
cart. All logic lives in the service.

## Why the Shortcut cannot do this alone

Worth stating, because it is the reason this service exists at all.

Signing in to Woolworths requires a cookie jar: the client must `GET /` to
collect `__cf_bm` and the load-balancer cookies *before* `POST /server/login`
will do anything but redirect. Shortcuts' "Get Contents of URL" action does not
expose response headers, so a Shortcut cannot read `Set-Cookie` and carry it
forward. Even where iOS's shared cookie store happens to carry it implicitly,
that is undocumented behaviour.

Add the per-ingredient loop with availability fallback — roughly fifteen
sequential requests, each needing its response body inspected — and a native
Shortcut becomes an unmaintainable flowchart. So the Shortcut calls one
endpoint and the service does the rest.

## Prerequisites

1. The service is reachable over **HTTPS** at `gaia.frostaura.net`.
   Plain HTTP is not acceptable: the Woolworths password travels in a request
   header and TLS is the only thing protecting it.
2. A Woolworths online account with a delivery address already confirmed. The
   first time an account adds food to a cart, the website requires an address
   confirmation that this service does not perform. Do it once in a browser.

## Build the Shortcut

In the Shortcuts app, create a new shortcut named **Cookidoo → Woolies**.

**1. Accept the shared text**

Open the shortcut's settings (the ⓘ / details panel):
- Enable **Show in Share Sheet**.
- Set **Accept** to **Text** only. Turn the other types off so the shortcut does
  not appear where it cannot work.

**2. Add "Text"** — set its content to the **Shortcut Input** variable.
This normalises whatever the share sheet hands over into plain text.

**3. Add "Get Contents of URL"**

| Field | Value |
| --- | --- |
| URL | `https://gaia.frostaura.net/api/woolworths/shopping-list/cart` |
| Method | `POST` |
| Request Body | `JSON` |

Headers:

| Key | Value |
| --- | --- |
| `Content-Type` | `application/json` |
| `X-Woolworths-Username` | your Woolworths email |
| `X-Woolworths-Password` | your Woolworths password |

JSON body — one field:

| Key | Type | Value |
| --- | --- | --- |
| `text` | Text | the **Text** variable from step 2 |

**4. Add "Get Dictionary Value"** — get `addedCount` from the previous result.
Add a second one for `cartItemCount` if you want both in the notification.

**5. Add "Show Notification"** — body something like
`Added [addedCount] items to your Woolies cart.`
This is the only feedback the user gets, so make it say what actually happened.

**6. Add "Open URLs"** — `https://www.woolworths.co.za/check-out/cart`

## Using it

In Cookidoo, open the shopping list, tap Share, and pick **Cookidoo → Woolies**.
The shortcut runs, notifies, and lands you on the cart to review and check out.

**Checkout is always manual.** The service fills the cart and stops; nothing in
this flow can place an order.

## Storing the password

Typing the password into the header field stores it in plaintext inside the
shortcut. It is on-device and not synced anywhere public, but anyone with the
unlocked phone can read it in the shortcut editor.

If that is not acceptable, the alternatives are, roughly in order of effort:
- Keep the shortcut but store the password in a note the shortcut reads at run
  time — marginal improvement, mostly moves the problem.
- Prompt for the password with an **Ask for Input** action each run — safe,
  annoying, defeats the point of a one-tap shortcut.
- Put a per-user token in front of the service and have it hold the Woolworths
  credential. This would reverse the "service stores no credentials" decision
  recorded in `MEMORY.md`, so it needs a deliberate call, not a quiet change.

## Verify before trusting it

Preview needs no credentials and changes nothing, so check the matching first:

```bash
curl -sX POST https://gaia.frostaura.net/api/woolworths/shopping-list/preview \
  -H 'content-type: application/json' \
  --data '{"text":"150 g dried fusilli pasta\n60 g fresh baby spinach leaves"}'
```

Then run the real thing once from the terminal before wiring the Shortcut, so a
failure is legible rather than a silent notification:

```bash
curl -sX POST https://gaia.frostaura.net/api/woolworths/shopping-list/cart \
  -H 'content-type: application/json' \
  -H "X-Woolworths-Username: $WOOLIES_USER" \
  -H "X-Woolworths-Password: $WOOLIES_PASS" \
  --data '{"text":"150 g dried fusilli pasta"}'
```

Confirm the item actually appears in the cart in a browser. A `200` from this
service means the upstream call was accepted, which — given how Woolworths
reports failure — is not the same as the item being in the cart.
