---
name: ai-toolkit-gaia-state
description: "Shipped plugin product — 4 plugins v10.0.1 in 2 ecosystems; the server half does not exist in the local tree; ahead-only, but the unpushed range is destructive; remote MCP last verified live 2026-07-19"
type: state
last_verified: 2026-08-21
---

# Current state

_Git state, working tree, remote refs and the local file layout re-inspected 2026-08-21. Plugin versions, manifest contents and the remote MCP service were **not** re-checked this pass — those lines carry their 2026-07-24 / 2026-07-19 dates._

**Shipped as a plugin product; the server half does not exist in the local tree at all** (see the alarm above). Branch `main` at `6770272` (2026-07-30), **ahead-only of `origin/main`** — so a plain push succeeds, and the unpushed range deletes `src/`, the CI workflow and `.mcp`. Last actual product work was `006cad3` (2026-06-26); everything since is context and plugin edits.

On disk: `plugins/{foundation,engineering,product,personal}/`, `.claude-plugin/marketplace.json`, `.github/plugin/marketplace.json`, `Gaia.slnx`, `Dockerfile`, `AGENTS.md`. **No `src/`, no `.github/workflows/`.**

As of 2026-07-24: **four** plugins ship — `foundation`, `engineering`, `product` and `personal` (added 2026-07-24) — all at version `10.0.1`, marketplace metadata `1.1.0`, in two ecosystems. `personal` is a near-copy of `foundation` (`fa-create-agent`, `fa-create-plan`, `fa-create-skill`). The remote MCP at `https://gaia.frostaura.net/mcp` was last verified live 2026-07-19 and has not been re-checked since.
