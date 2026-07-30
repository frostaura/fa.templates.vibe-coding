---
name: ai-toolkit-gaia-gotchas
description: "dotnet test gate vacuous (zero test projects, exits 0); personal plugin.json pair already drifted; unpinned playwright MCP pull; root Dockerfile without HEALTHCHECK/VOLUME"
type: gotcha
last_verified: 2026-07-24
---

# Gotchas

- **The `dotnet test` CI gate is vacuous — re-confirmed against `origin/main` this pass.** `Gaia.slnx` references exactly one project (`src/Gaia.Mcp.Server`). `src/Gaia.Mcp.Tests/` holds a single `README.md`, no `.csproj` and no `.cs`, and that README points at `tests/Gaia.Mcp.Server.Tests/` and `tests/Gaia.Mcp.Server.IntegrationTests/` — **`tests/` has never existed in this repo's history**. `dotnet test Gaia.slnx` discovers zero test projects and exits 0. The empty `Gaia.Mcp.Tests` directory is worse than nothing: it makes the repo look tested at a glance. The product that ships `fa-testing` has no tests of its own.
- **`plugins/personal/plugin.json` and `plugins/personal/.claude-plugin/plugin.json` have already drifted** — the duplicate lists `"foundation"` where the primary lists `"personal"`. The repo's own invariant says these must stay byte-identical; nothing in CI checks it. The other three plugins' pairs are identical.
- **`npx @playwright/mcp@latest` is pulled unpinned** on every engineering-plugin invocation — a live supply-chain and reproducibility exposure.
- Dockerfile runs as **root**, installs curl but declares no `HEALTHCHECK`, and declares no `VOLUME` for `/app/data` — **container data is ephemeral unless the host mounts it.**
- MCP `ServerInfo.Version` was hardcoded `"1.0.0"` against a v10.0.1 product (in the deleted `Program.cs`; re-check on restore).
