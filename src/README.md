# Gaia MCP Server

A small .NET 10 MCP server that exposes third-party consumer services as agent
tools. It is **stateless**: no database, no credential store, nothing persisted
between calls.

This is a separate concern from the three Gaia plugins. The plugins hold the
delivery method and need no server; this server holds integrations and needs no
plugin.

## Tools

| Tool | Credentials | What it does |
| --- | --- | --- |
| `woolworths_search_products` | none | Search the Woolworths South Africa grocery catalogue. Returns id, name and ZAR price. |
| `woolworths_preview_shopping_list` | none | Resolve a shared shopping list (e.g. the text the Cookidoo/Thermomix app puts on the share sheet) to products, without signing in or touching a cart. |
| `woolworths_add_shopping_list_to_cart` | required | Resolve the list and add every match to the signed-in customer's cart. |
| `woolworths_add_product_to_cart` | required | Add one product by id, as returned by search. |
| `example_echo`, `example_echo_by_letter` | none | Systems-test tools: echo, and echo with per-character progress notifications. |
| `sampling_summarize` | none | Demonstrates MCP sampling — the server borrows the *client's* LLM; it has none of its own. |

The anonymous tools are anonymous on purpose: they are what makes the server
safe to expose to an agent that holds no credentials, and they must stay that
way.

## REST surface

The same services are mapped under `/api`, so a Siri Shortcut or a curl script
reaches the identical behaviour without speaking MCP. Both surfaces call the
same code — a behaviour added to one must not diverge from the other.

| Endpoint | Credentials | |
| --- | --- | --- |
| `GET /api/health` | none | Liveness. |
| `GET /api/integrations` | none | Discovery: what this server can drive, and exactly which header or environment variable each integration needs. |
| `GET /api/woolworths/search?q=&limit=` | none | Product search. |
| `POST /api/woolworths/shopping-list/preview` | none | Resolve a list without touching a cart. |
| `POST /api/woolworths/shopping-list/cart` | required | Resolve and fill the cart. |
| `POST /api/woolworths/cart/items` | required | Add one product by id. |

## Credentials

Every integration declares the credentials it needs; nothing is configured
centrally. `GET /api/integrations` reports the exact names.

- **Over HTTP** — request headers, e.g. `X-Woolworths-Username` /
  `X-Woolworths-Password`. An MCP client sets exactly the headers a curl caller
  would, so there is one auth story per integration, not two.
- **Over stdio** — the process environment, derived from the header so the two
  cannot drift: `X-Woolworths-Password` → `GAIA_WOOLWORTHS_PASSWORD`.

Credentials are read per call and discarded with the call. They are never
stored, cached, logged, or echoed back. **Never expose this server over plain
HTTP** — TLS is the only thing protecting them in transit.

## Transports

| Transport | How |
| --- | --- |
| Streamable HTTP (default) | `dotnet run --project src/Gaia.Mcp.Server` — serves `/mcp` and `/api`. |
| stdio | set `MCP_TRANSPORT=stdio`; without it the web host starts on stdio and misbehaves. |

HTTP uses a hybrid session mode — stateless for 2026-07-28 discovery clients, a
stateful session for legacy `initialize` clients so server-initiated sampling
still works. Tool calls execute inside the caller's HTTP request in either mode,
which is what lets header credentials work at all; verified, not assumed.

## How it works

[`../docs/integrations/architecture.md`](../docs/integrations/architecture.md) covers
the credential model, the session lifecycle, and why the anonymous tools must stay
anonymous. [`woolworths.md`](../docs/integrations/woolworths.md) is the captured
upstream contract — endpoints, the traps, and what no vendor documentation exists for.

## Tests

`dotnet test` from the repository root. See
[`Gaia.Mcp.Tests/README.md`](./Gaia.Mcp.Tests/README.md) for what is
covered and what is deliberately verified by hand instead.
