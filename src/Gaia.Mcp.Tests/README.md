# Gaia.Mcp.Tests

The server's xUnit suite. Run it with `dotnet test` from the repository root.

Coverage today is the `ShoppingListParser` contract — quantity/unit separation,
section headings, and the Australian→South African vocabulary map that decides
whether an ingredient matches the retailer catalogue at all. It is the one part
of the integration pipeline that can be exercised without touching a live
upstream, which is exactly why it carries the behavioural contract.

The Woolworths HTTP clients are deliberately not unit-tested against mocks: the
upstream's failure modes (HTTP 200 for bad credentials, for unavailable products
and for a logout that did not happen) are the whole risk, and a mock encodes the
behaviour we assumed rather than the behaviour the site has. Those paths are
verified by hand against the live site — see `docs/integrations/`.
