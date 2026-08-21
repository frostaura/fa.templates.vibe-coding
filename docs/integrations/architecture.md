# Integrations — system overview

> Migrated from the retired `fa.integrations` repository (`ARCH-001`) and
> reconciled against Gaia's actual structure on 2026-08-21. Where this document
> and the code disagree, the code is right and this document is a defect — fix
> it in the same change.

## Context

Several third-party consumer services worth automating have no public API, no
API keys and no OAuth. The only way to act on a user's account is to behave like
their browser: sign in with their own credentials and drive the same endpoints
the site's own single-page application calls. That capability is wanted from
more than one place — a Siri Shortcut, a shell script, and an AI agent all want
to fill the same grocery cart — and writing it once per caller is how the
implementation drifts.

This is why the capability is implemented once and exposed twice: as a REST API
for anything that speaks HTTP, and as MCP tools for anything that speaks Model
Context Protocol.

## Decision

A stateless host with two transport surfaces over one shared service layer, and
a credential model in which the service itself holds nothing.

## Structure

Everything lives in the single `Gaia.Mcp.Server` project. The split that matters
is by namespace, not by assembly:

| Location | Owns |
| --- | --- |
| `Integrations/` | The contracts every integration implements — `IIntegration`, `CredentialField`, `IntegrationCredentials`, the two credential exceptions — plus the provider-neutral shopping-list domain (`ShoppingListItem`, `Product`, `CartLineResult`, `CartBuildResult`, `ShoppingListParser`) and `CredentialAccessor`. |
| `Integrations/Woolworths/` | The Woolworths integration: `WoolworthsIntegration` (the declaration), `WoolworthsClient` (an authenticated session), `WoolworthsSearchClient` (anonymous catalogue search), `WoolworthsCartService` (orchestration). |
| `Tools/WoolworthsTools.cs` | The `[McpServerToolType]` — thin wrappers over the service layer. |
| `Program.cs` | The host: REST endpoints under `/api`, MCP under `/mcp`, DI registration, and the exception-to-status-code mapping. |
| `src/Gaia.Mcp.Tests` | xUnit. `Program` is declared `public partial` specifically so the host can be started in-memory. |

The source repository kept these as four separate projects. They were folded
into one on migration because the server is small and the seam that actually
earns its keep is `IIntegration`, which is a type, not an assembly. The
provider-neutral half still depends on nothing Woolworths-specific, and that is
what lets a second integration be added without touching Woolworths code — the
shopping-list domain and the parser are retailer-neutral, and only
`WoolworthsSearchClient` knows how a Woolworths product is found.

### Two surfaces, one service layer

`Program.cs` maps REST endpoints under `/api` and calls `app.MapMcp("/mcp")`.
`WoolworthsTools` is a set of thin wrappers over the exact same
`WoolworthsSearchClient` and `WoolworthsCartService` methods the minimal-API
endpoints call. Nothing of substance lives in either surface.

| Capability | REST | MCP tool | Credentials |
| --- | --- | --- | --- |
| Discovery | `GET /api/integrations` | — | none |
| Health | `GET /api/health` | — | none |
| Catalogue search | `GET /api/woolworths/search` | `woolworths_search_products` | none |
| Resolve a list, dry run | `POST /api/woolworths/shopping-list/preview` | `woolworths_preview_shopping_list` | none |
| Resolve a list into the cart | `POST /api/woolworths/shopping-list/cart` | `woolworths_add_shopping_list_to_cart` | required |
| Add one product | `POST /api/woolworths/cart/items` | `woolworths_add_product_to_cart` | required |

The two anonymous capabilities are deliberate and load-bearing. Search and
preview run entirely against the third-party search provider (see
[`woolworths.md`](woolworths.md)) and never sign in, so an AI agent that holds no
credentials at all can still resolve a shopping list and show its matches.
Requiring an account for those would remove the only safe entry point the
service has.

### Session mode — the one thing that changed on migration

The source repository registered the MCP server as **stateless**, correctly
reasoning that every tool call carries its own credentials and completes in one
round trip. Gaia's host instead uses
`HttpServerSessionMode.StatefulForInitializeClients`, because its sampling demo
needs a session for legacy clients.

That is a real risk to header-based credentials: a tool executing on a session's
background context would see no `HttpContext` and no headers. **It was verified
empirically rather than reasoned about.** Under the hybrid mode a tool call still
executes inside the caller's HTTP request, and `CredentialAccessor` resolves the
inbound headers correctly — confirmed by supplying only `X-Woolworths-Username`
and observing that only `X-Woolworths-Password` was reported missing.

If the session mode is ever changed again, re-run that check. It is two minutes
and it is the difference between working credentials and a class of failure that
looks like a configuration problem on the caller's side.

### Error reporting

The one place the surfaces legitimately differ. REST failures go through the
`UseExceptionHandler` block in `Program.cs`, which maps
`MissingCredentialsException` and `ProviderAuthenticationException` to 401,
cancellation to 504, `HttpRequestException` to 502, and everything else to 500 —
and only surfaces the message text for the two credential cases, because those
describe the caller's own request rather than provider internals.

MCP failures **must** be thrown as `McpException`: the SDK replaces arbitrary
exception text with a generic "an error occurred" so servers cannot leak
internals, which would otherwise leave a caller with no idea what is wrong.
`WoolworthsTools` performs that translation in two places, and both are needed:
`RequireCredentials` for a caller who has not configured credentials at all, and
`TranslateAsync` for a caller whose credentials are simply **wrong**. Only the
first existed before migration; the second was found by probing the migrated
server and is at least as common a case.

### The credential model

**The service stores no credentials.** There is no configuration file of
accounts, no database, no cache, no "remember me". Every request that acts on a
user's account carries that user's credentials, and those values live only for
the duration of that request. The security property this buys is worth stating
plainly: compromising the host yields no standing access to anybody's account.

The mechanism is `IIntegration`. An integration declares itself — key, display
name, description — and lists the `CredentialField`s it needs, each binding a
stable short key the integration reads (`"password"`) to the HTTP header a
caller sets (`X-Woolworths-Password`), with a description, a `Required` flag and
a `Secret` flag.

`CredentialAccessor` resolves them per call:

- **Over HTTP** — from the inbound request headers.
- **Over stdio** — from the process environment, because there is no request to
  read. The variable name is *derived from the header* so the two cannot drift:
  strip a leading `X-`, uppercase, dashes to underscores, prefix `GAIA_`. Hence
  `X-Woolworths-Password` → `GAIA_WOOLWORTHS_PASSWORD`.

The environment is consulted **only** when the transport supplied no headers at
all. An HTTP caller who omits a header is told so, rather than being silently
served the host's own environment — which would be a genuine security defect,
not a convenience.

Either way `MissingCredentialsException` names the exact header or variable that
is absent. That message is written for someone wiring up an MCP client who
cannot see the code.

Each integration declaring its own fields is the point. What Woolworths needs is
an account login, because Woolworths offers nothing else; the next integration
may need an API key, a bearer token, a store id, or all three. There is no
shared credential shape to force them into.

Discovery falls out of the same abstraction. `GET /api/integrations` enumerates
every `IIntegration` registered in DI and returns each one's credential fields,
headers, environment variable names and descriptions. Registration in
`AddIntegrationServices` is what makes an integration exist — there is no central
list to keep in sync. Note the two-line registration pattern: the concrete type
is registered as a singleton, then registered *again* as `IIntegration` resolving
to the same instance, so tools can inject the concrete type while discovery still
enumerates it.

### Session lifecycle

An authenticated Woolworths session is a `WoolworthsClient`, which owns its own
`CookieContainer` and is `IDisposable`. Sessions follow one shape and only one:

1. **Sign in.** `WoolworthsCartService` constructs a fresh `WoolworthsClient` for
   the operation and calls `SignInAsync` with the credentials resolved for this
   call.
2. **Do the work.** All cart mutation for this one call.
3. **Sign out**, in a `finally`, so a run that fails part-way still ends its
   session.

**Sessions are never pooled, cached, or shared across requests.** This is not an
efficiency oversight. Authentication upstream is entirely cookie-based — no
bearer token, no `Authorization` header — so a pooled client is a container of
one specific person's cookies. Reusing it across callers would serve one user's
cart to another. The cost is a sign-in round trip per operation; that is the
correct price.

Sign-out is best-effort by design. `SignOutAsync` catches its own exceptions and
returns `false` rather than throwing, so a failed logout can never mask the
result of the work that preceded it. It also verifies rather than assumes: after
the POST it re-reads `/server/currentUser` and only considers the session ended
when `loggedInStatus` is `0`, logging a warning otherwise.

## Invariants

- **Both surfaces call the same service methods.** A behaviour added to one must
  not diverge from the other. If a tool starts doing its own orchestration, that
  logic belongs in the service layer instead.
- **No credential is ever stored, logged, cached or echoed back.** The password
  field is marked `Secret`; discovery returns the flag, never a value.
- **Anonymous endpoints stay anonymous.** Search and preview must never begin
  requiring an account.
- **Every authenticated operation signs out in a `finally`.**
- **The service must never be exposed over plain HTTP.** Credentials travel in
  headers, and TLS is the only thing protecting them.
- **No checkout.** This service fills a cart and stops. Placing a completion or
  payment step behind an automated call is out of scope deliberately, not
  incidentally.

## Alternatives considered

- **A single surface with an adapter.** Rejected: an MCP tool needs its
  descriptions written for a model rather than for a human reader, and the
  error-reporting contracts genuinely differ. Sharing the service layer captures
  all of the benefit; sharing the transport would have captured none of it.
- **Server-side stored accounts.** Rejected. It would remove per-request
  credential plumbing from every caller, and in exchange make the host a
  credential store worth attacking. Adding it later requires an explicit recorded
  decision.
- **A pooled, kept-alive upstream session.** Rejected on correctness, per the
  cookie-container argument above.
- **Dropping the REST surface on migration.** Rejected: the Siri Shortcut use
  case ([`siri-shortcut.md`](siri-shortcut.md)) cannot speak MCP, and the
  endpoints cost nothing because they call the same services the tools do.

## Consequences

- Adding an integration is a matter of implementing `IIntegration`, writing its
  services, registering them, and adding a tool class. Discovery, credential
  resolution and error mapping come for free.
- Every authenticated operation pays a sign-in and a sign-out. Latency is the
  price of the isolation property.
- Callers carry the burden of holding credentials, which is where that burden
  belongs, but it does mean an MCP client must be configured with per-user
  headers rather than being genuinely multi-tenant.

## Fragility and unverified assumptions

The entire Woolworths integration drives undocumented, unversioned endpoints
captured from live browser traffic. Nothing about them is contractual and none
of it can be pinned to a version. [`woolworths.md`](woolworths.md) covers the
specific fragilities. The general posture that follows: **this service must fail
loudly.** A silent partial success — the natural failure mode of these endpoints,
which return HTTP 200 for almost everything — is worse than an error, because
the user believes they have a full cart.

The credential model itself is verified only to the extent that the Woolworths
integration exercises it. `CredentialField.Required = false` (optional fields)
and the `Secret` flag's effect on any future logging are declared but not yet
exercised by a real integration.

## Validation

- **Automated:** `src/Gaia.Mcp.Tests` covers `ShoppingListParser` — the one part
  of the pipeline testable without reaching Woolworths. `Program` is
  `public partial` so the host can be started in-memory, but the surface-level
  tests that would use it are not yet written.
- **Manual:** see [`testing.md`](testing.md). Anything touching a cart requires a
  real account and a real observable cart.

## References

- [Woolworths integration](woolworths.md) — the upstream endpoint contract.
- [Cookidoo shopping list to cart](cookidoo-shopping-list.md) — the primary use case.
- [Siri Shortcut setup](siri-shortcut.md) — the REST consumer.
- [Testing](testing.md) — the end-to-end check across both surfaces.
- `src/Gaia.Mcp.Server/Program.cs`, `Integrations/Credentials.cs`, `Integrations/CredentialAccessor.cs`
