---
name: ai-toolkit-gaia-watch
description: "12.0.0+12.1.0 uncommitted — README/CHANGELOG claim what origin doesn't serve; context-audit.py cannot see any plugin skill, and --group-dir still defaults to projects; two foundation skills still route policy into AGENTS.md; 2MB icon at root; playwright MCP unpinned; stale remote task store"
type: watch
last_verified: 2026-08-21
---

# Watch list

- **12.0.0 AND 12.1.0 sit uncommitted in the working tree, on top of the restored `src`/CI/`.mcp`.** Until the founder commits and pushes, `README.md`/`CHANGELOG.md` on disk describe releases that origin does not serve (origin: 4 plugins @ 10.0.1 incl. `personal`), and the restore exists only in the index. Any session that reads the tree without reading this store will misreport what ships.
- **`context-audit.py` cannot validate a single one of the shipped plugin skills.** `check_skill_indexes` gates on `dirpath.parent.name in (".claude", ".github")`, and plugin skills live at `plugins/<name>/skills/` — so the frontmatter↔directory check, the one invariant that silently produces an uninvokable skill, never runs on any of them. Deliberately **not** fixed in 12.1.0 (it is a scoping question, not a bug in the check). The substitute is the by-hand loop in `docs/development.md`; run it on every skill change.
- **`--group-dir` still defaults to `projects`.** 12.1.0 made the grouping directory configurable, which is what makes the script usable outside one org's shape — but a caller who does not pass `--group-dir packages` on a workspace monorepo still gets CLEAN over directories the script never entered. Silent-pass-by-default; the skill's step 7 passes it, nothing else does.
- **`fa-foundation-create-agent` (lines 43, 99) and `fa-foundation-create-skill` (line 43) still route global policy into `AGENTS.md`**, against the interop-pointer doctrine now shipped in `fa-foundation-optimize-directory-tree/references/context-templates.md` and applied to this repo's own `AGENTS.md` in 12.1.0. The README/`CLAUDE.md` "workflow contract" framing is resolved; these two skills are not.
- **This repo is public, and its own context layer is not written as if it were.** `CLAUDE.md` names the owner and links a path outside the repository; `memory/decisions.md` names the parent organization's internal structure. All of it is uncommitted today, so the decision is still free: genericize before the first commit, or accept it deliberately. Do not let a commit make the choice.
- **2 MB `README.icon.png` committed at root**, referenced by absolute GitHub raw URL anyway.
- **`npx @playwright/mcp@latest` remains unpinned** in the engineering plugin's MCP config (deliberately left — plugin.json edits were scoped; pin on the next manifest touch).
- **The remote MCP task store for this project is stale** — records reference the pre-plugin `.github/skills/gaia-*` generation and a `todo` release task blocked on a v9.0.0 NEEDS_INPUT. Harmless, but don't treat the remote store as current repo state.
- **The Claude Desktop / claude.ai / Cowork install path in the README is documented by link, not by click-path** — nothing in this repo corroborates the navigation string, the plan requirement, or the surface matrix beyond "sub-agents and hooks are Claude Code". Confirm in the live product, then either keep the link or replace it with verified steps and record the confirmation date in `state.md`.

_Cleared items are deleted here, not struck through — a watch list carrying resolved rows stops being read. (Resolved 2026-08-21 and removed: `.vscode/` configs pointing at a deleted `src/`; the README/`CLAUDE.md` "AGENTS.md is the workflow contract" framing.)_
