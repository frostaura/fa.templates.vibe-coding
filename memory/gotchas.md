---
name: ai-toolkit-gaia-gotchas
description: "MSBuildEnableWorkloadResolver=false mandatory on this volume; MCP session mode silently governs whether header credentials resolve; upstream returns HTTP 200 for every kind of failure; plugin.json pair invariant has no CI check; unpinned playwright MCP"
type: gotcha
last_verified: 2026-08-21
---

# Gotchas

_All lines below re-verified 2026-08-21 except the playwright pin, which was inspected but not exercised._

- ~~The `dotnet test` gate is vacuous~~ — **fixed in 13.0.0.** `src/Gaia.Mcp.Tests` is a real xUnit project in `Gaia.slnx` and 12 tests run. Kept here because **every note written before 13.0.0 that treats a green `dotnet test` as meaningful is wrong**, and CI ran that empty gate for months.
- **`MSBuildEnableWorkloadResolver=false` is mandatory on this volume.** The workload resolver crashes under the iCloud-synced path; nothing here uses workloads. Export it before any `dotnet build` / `test` / `run`, or read a stack trace instead of a build.
- **The MCP session mode silently governs whether header credentials work at all.** The integration tools read credentials from the inbound request headers via `IHttpContextAccessor`. `fa.integrations` ran the server stateless partly for this reason; Gaia runs `StatefulForInitializeClients` so the sampling demo keeps working. Under the hybrid mode a tool call *does* still execute inside the caller's HTTP request — **verified 2026-08-21**, not reasoned about. If the mode changes, re-verify: send only `X-Woolworths-Username` and check that only the password is reported missing. The failure mode looks like a caller-side configuration problem.
- **Woolworths returns HTTP 200 for every kind of failure** — bad credentials, unavailable products, and a logout that did not happen. Always inspect the body (`errorMessage`, `formexceptions`, `loggedInStatus`). Two more traps in the same family: cart adds must be **one product per request** (a batch containing one unavailable item adds *nothing* and still returns a success-shaped 200), and the search `filters[visibility]` parameter must be sent **twice** (`all` and `web and app`) or food results come back empty while non-food does not — which reads convincingly as a location-gated catalogue and is not. Full contract: `docs/integrations/woolworths.md`.
- **The MCP SDK replaces arbitrary exception text with a generic message.** Anything a caller needs to act on must be thrown as `McpException`. Only the *missing*-credential path was wrapped when this code arrived; a *wrong* password produced "an error occurred", which is at least as common. Fixed in 13.0.0 — and it was found by probing the running server, not by reading the code.
- **The plugin.json byte-identical-pair invariant has no CI check** — the `personal` pair had silently drifted before that plugin was removed (2026-08-21). The three remaining pairs were verified identical the same day; without a check, they will drift again.
- **`npx @playwright/mcp@latest` is pulled unpinned** on every engineering-plugin invocation — a live supply-chain and reproducibility exposure.
- ~~Dockerfile root/no-HEALTHCHECK/no-VOLUME~~ and ~~ServerInfo.Version 1.0.0~~ — both fixed in the 2026-08-21 server upgrade (non-root USER, initialize-probe HEALTHCHECK, VOLUME /app/data, version 11.0.0). Note for healthchecks/probes: **`ping` was removed in MCP spec 2026-07-28 and SDK 2.2.0 rejects it with a 400** — probe with a legacy `initialize` POST instead.
- **MCP sampling / roots / logging are spec-deprecated as of 2026-07-28** (12-month window, SEP-2577). `sampling_summarize` exists to demonstrate sampling anyway — MCP9005 is suppressed file-wide in `SamplingTools.cs` only, deliberately.
