---
name: ai-toolkit-gaia-state
description: "v12.1.0 and v12.0.0 both sit UNCOMMITTED: 12.1.0 adds fa-foundation-optimize-directory-tree (foundation 9→10), genericizes context-audit.py and rewrites the README into docs/; 12.0.0 was the breaking fa-<plugin>-<name> rename. Origin still serves 10.0.1"
type: state
last_verified: 2026-08-21
---

# Current state

**v12.1.0 landed in the working tree 2026-08-21, uncommitted, on top of 12.0.0 — which is itself
uncommitted.** Additive. Plugin counts are now **foundation 10 · engineering 16 · product 14
skills, and 20 agents (3 · 6 · 11)**. What shipped:

- **`fa-foundation-optimize-directory-tree`** — the entry point that was missing: establishing the
  context layer across a whole tree, maintaining it against each project's own source, and
  sweeping it lean (where deletion is the deliverable). Three references under it:
  `context-templates.md`, `analysis-brief.md`, `orientation-interview.md`. No new agent — the
  method is a pasted brief, by founder decision.
- **Routing boundaries** rewritten in five shipped descriptions plus the `fa-foundation-context-auditor`
  agent, because a description is the only layer a router reads and four skills now collide.
- **`context-audit.py` genericized** — `--group-dir`, `--all` honoured for vendored skips, leaf-scope
  registry seeding, widened name patterns at all three sites, `--instruction-file`.
- **`ownership-and-conventions.md`** registers the new skill and the previously unregistered
  `fa-foundation-session-close` in both its lists.
- **README 211 → 143 lines**, with `docs/catalog.md` and `docs/development.md` created; `AGENTS.md`
  reduced to an interop pointer.

All 12 JSON version sites read `12.1.0`, `metadata.version` stays `1.3.0`, the three `plugin.json`
pairs verified byte-identical with `cmp`, and the two marketplace manifests agree ignoring `source`.
**The README's Claude Desktop / claude.ai / Cowork install path is documented by link to Anthropic's
plugin docs, not by a click-path — unconfirmed in the live product as of 2026-08-21.** Record the
confirmation date here when it happens.

**v12.0.0 sits underneath it, also uncommitted** — the breaking rename of every skill and agent to
`fa-<plugin>-<name>` (11 skill directories, 6 agent files, 194 references across 53 files), plus the
generic context-layer skills migrating in (foundation 3→9, engineering 7→16) and the first shipped
`scripts/context-audit.py`. Full detail is in `CHANGELOG.md`; what matters here is that a skill's
directory name must equal its frontmatter `name`, so that change was either complete or silently
broken, and it was verified both directions.

**Beneath that, the v11.0.0 plugin rework and the restored+upgraded MCP server, also uncommitted**
(founder instruction 2026-08-21: commit nothing) — the 21 `b9d2ee3`-deleted files restored from
`origin/main`, then `ModelContextProtocol` 1.4.0→2.2.0 (spec 2026-07-28, hybrid session mode), tool
annotations and structured output, `sampling_summarize`, hardened Dockerfile with an
initialize-probe HEALTHCHECK. **Verified 36/36** by a throwaway SDK-2.2.0 client over Streamable
HTTP. Build needs `MSBuildEnableWorkloadResolver=false` on this volume. Branch `main` is at
`a5b434b`, **5 ahead of `origin/main`, 0 behind**; the committed range alone is still destructive —
see the do-not-push alarm, AMBER. Origin still serves 4 plugins at 10.0.1.

_Git state, working tree, plugin content and manifests re-inspected 2026-08-21._

The remote MCP at `https://gaia.frostaura.net/mcp` verified live 2026-08-21 (`tasks_list` answered). Its task store still carries records from the pre-plugin repo generation (`.github/skills/gaia-*`, symlink scripts, the deleted `docs/architecture/product-discovery-team.md`) and one `todo` release task blocked on `NEEDS_INPUT` about a v9.0.0 bump — stale remote state, but proof the completion contract enforces.

On disk: `plugins/{foundation,engineering,product}/` (each now with a top-level `references/`), `.claude-plugin/marketplace.json`, `.github/plugin/marketplace.json`, `CHANGELOG.md`, `docs/` (new in 12.1.0: `catalog.md`, `development.md`), `Gaia.slnx`, `Dockerfile`, `AGENTS.md`, and **`src/` + `.github/workflows/` + `.mcp` are back** (restored 2026-08-21; `Gaia.slnx`/`Dockerfile`/README `src/` links all valid again). Ship path: founder reviews and commits the working tree, confirms the range deletes nothing net vs origin, pushes — nothing reaches installed users before that.
