---
name: ai-toolkit-gaia-gotchas
description: "dotnet test gate vacuous (zero test projects, exits 0); plugin.json pair invariant has no CI check; unpinned playwright MCP pull; root Dockerfile without HEALTHCHECK/VOLUME"
type: gotcha
last_verified: 2026-08-21
---

# Gotchas

_The plugin.json-pair line was verified 2026-08-21; the dotnet-test and Dockerfile lines carry their 2026-07-24 verification and were not re-inspected since._

- **The `dotnet test` CI gate is vacuous — re-confirmed against `origin/main` 2026-07-24.** `Gaia.slnx` references exactly one project (`src/Gaia.Mcp.Server`). `src/Gaia.Mcp.Tests/` holds a single `README.md`, no `.csproj` and no `.cs`, and that README points at `tests/Gaia.Mcp.Server.Tests/` and `tests/Gaia.Mcp.Server.IntegrationTests/` — **`tests/` has never existed in this repo's history**. `dotnet test Gaia.slnx` discovers zero test projects and exits 0. The empty `Gaia.Mcp.Tests` directory is worse than nothing: it makes the repo look tested at a glance. The product that ships `fa-engineering-testing` has no tests of its own.
- **The plugin.json byte-identical-pair invariant has no CI check** — the `personal` pair had silently drifted before that plugin was removed (2026-08-21). The three remaining pairs were verified identical the same day; without a check, they will drift again.
- **`npx @playwright/mcp@latest` is pulled unpinned** on every engineering-plugin invocation — a live supply-chain and reproducibility exposure.
- ~~Dockerfile root/no-HEALTHCHECK/no-VOLUME~~ and ~~ServerInfo.Version 1.0.0~~ — both fixed in the 2026-08-21 server upgrade (non-root USER, initialize-probe HEALTHCHECK, VOLUME /app/data, version 11.0.0). Note for healthchecks/probes: **`ping` was removed in MCP spec 2026-07-28 and SDK 2.2.0 rejects it with a 400** — probe with a legacy `initialize` POST instead.
- **MCP sampling / roots / logging are spec-deprecated as of 2026-07-28** (12-month window, SEP-2577). `sampling_summarize` exists to demonstrate sampling anyway — MCP9005 is suppressed file-wide in `SamplingTools.cs` only, deliberately.
