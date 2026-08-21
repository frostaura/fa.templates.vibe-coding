---
name: ai-toolkit-gaia-state
description: "v13.0.0 UNCOMMITTED: tasks/memory/evolve tools removed, the fa.integrations Woolworths capability migrated in whole (MCP + REST), plugins retargeted onto native todos and the file-based memory store, first real test suite. Origin serves 12.1.0"
type: state
last_verified: 2026-08-21
---

# Current state

**v13.0.0 landed in the working tree 2026-08-21, uncommitted. Breaking.** Everything below
it is committed and pushed — `main` was `8cb1a31`, level with `origin/main` and clean,
before this change began.

## What v13.0.0 does

- **Removed all 15 `tasks_*` / `memory_*` / `evolve_*` tools** and everything behind them:
  `ThreadSafeJsonStore<T>`, `JsonTaskStore`, `CompletionValidator`, five models, the `GAIA_*`
  error codes, `src/schemas/`. **14 files deleted** (6 models, 2 stores, 3 tools, the
  validator, 2 schema docs) — **15 in the push range**, the extra being this store's own
  resolved `do-not-push.md`. No data directory; the container's `/app/data` volume is gone.
- **Removed `foundation`'s `mcpServers` block** — it wired `fa-gaia-remote` into every
  install to persist tasks/memory/evolution, and there is nothing left to persist to. **No
  plugin wires the server now.**
- **Migrated the whole Woolworths capability in from `fa.integrations`** — 4 MCP tools plus
  the REST surface, folded from four projects into `src/Gaia.Mcp.Server/Integrations/`
  (+ `Woolworths/`, `Tools/WoolworthsTools.cs`); docs to `docs/integrations/`. That repo is
  killed; its record is in `Technologies/projects/memory/kill-records.md`.
- **Retargeted 9 plugin content files** off the removed tools: plan → native todo list,
  durable state → the repository's own `MEMORY.md` + `memory/` store, improvements → logged
  as memories. Two skill `description` fields changed, so routing changed with them.
- **First real test suite** — `src/Gaia.Mcp.Tests`, 12 migrated parser tests. Milestone 3 met.

All 12 JSON version sites read `13.0.0`; `metadata.version` stays `1.3.0` per the documented
catalog-version rule. The three `plugin.json` pairs verified byte-identical with `cmp`; both
marketplace manifests agree ignoring `source`; zero broken relative links repo-wide.

## Verified, not assumed (2026-08-21)

- `dotnet build` green, 0 warnings; `dotnet test` **12/12**. Both need
  `MSBuildEnableWorkloadResolver=false` on this volume.
- HTTP `:5199`: `initialize` → `gaia-mcp 13.0.0`; `tools/list` → **7 tools**, **zero**
  `tasks_`/`memory_`/`evolve_`. Search returned live catalogue data; preview resolved 2/2
  through the localisation map (`red capsicum` → "Single Red Pepper"; `flat-leaf parsley` →
  "Fresh Italian Parsley"). stdio likewise, reading the derived `GAIA_WOOLWORTHS_*` variables.
- **Header credentials still resolve under the hybrid session mode** — the one real
  migration risk, since `fa.integrations` ran stateless. Proved by sending only
  `X-Woolworths-Username` and seeing only the password reported missing. **Re-run if the
  session mode ever changes.**
- REST: `/api/health` 200; `/api/integrations` lists both credential fields with header
  **and** environment-variable names.

## Not verified

- **Nothing is deployed.** `gaia.frostaura.net/mcp` still serves the 12.1.0-era build, so
  the hosted service still exposes tools this tree says are gone.
- No test covers the HTTP clients or either transport surface.

## Before the next durability check

**v13.0.0 legitimately deletes 15 files** — the 14 above plus the resolved `do-not-push.md`
alert. `git diff origin/main...HEAD --diff-filter=D` therefore reports deletions for the
first time since the `b9d2ee3` incident. **Expected, enumerated above, not a recurrence** —
do not let a check that flags it trigger a restore.
