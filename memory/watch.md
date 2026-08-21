---
name: ai-toolkit-gaia-watch
description: "BLOCKING: pushing 13.0.0 publishes the Woolworths reverse-engineering from a formerly private repo — see questions.md; 13.0.0 uncommitted and the hosted service still serves the old tool set; the v13 commit deliberately deletes 14 files so a durability check will flag it; the frostaura marketplace was uninstalled from this machine; context-audit.py still cannot see any plugin skill; playwright MCP unpinned"
type: watch
last_verified: 2026-08-21
---

# Watch list

- **⚠ Pushing v13.0.0 publishes work that has only ever been private.** The Woolworths
  integration and its reverse-engineered endpoint contract came from a **private** repo into
  this **public** one. That is a decision nobody has made yet, and a push makes it silently.
  Full reasoning and the options in [`questions.md`](questions.md). **Read that before the
  first push, not after.**

- **13.0.0 sits uncommitted, and `gaia.frostaura.net` still serves the 12.1.0-era build.**
  Until the image is rebuilt and the stack redeployed, the hosted MCP still exposes
  `tasks_*`, `memory_*` and `evolve_*` while this tree says they are gone. Anyone reading
  the tree without reading this store will misreport what is actually running.
- **The 13.0.0 commit deliberately deletes 14 files.** `git diff origin/main...HEAD
  --diff-filter=D` will be non-empty for the first time since the `b9d2ee3` incident.
  **Expected, enumerated in `state.md`, not a recurrence** — do not let a durability check
  turn it into a restore.
- **The `frostaura` marketplace was uninstalled from this machine on 2026-08-21** (founder
  request): removed from `~/.claude/settings.json`, from
  `~/.claude/plugins/known_marketplaces.json`, and both `marketplaces/frostaura` and
  `frostaura.bak` clones deleted. **No `@frostaura` plugin was actually enabled at the
  time** — the marketplace was registered but the plugins were switched off, which means
  every FrostAura `CLAUDE.md` instruction to "reach for `fa-foundation-*`" has been firing
  against nothing. Dogfooding is a stated kill criterion for this program; treat the gap
  as a real signal, not a config detail.
- **`context-audit.py` still cannot validate a single shipped plugin skill.**
  `check_skill_indexes` gates on `dirpath.parent.name in (".claude", ".github")`, and
  plugin skills live at `plugins/<name>/skills/` — so the frontmatter↔directory check, the
  one invariant that silently produces an uninvokable skill, never runs on any of them.
  The substitute is the by-hand loop in `docs/development.md`; run it on every skill change.
- **`--group-dir` still defaults to `projects`.** A caller who does not pass
  `--group-dir packages` on a workspace monorepo still gets CLEAN over directories the
  script never entered. Silent-pass-by-default.
- **`fa-foundation-create-agent` (lines 43, 99) and `fa-foundation-create-skill` (line 43)
  still route global policy into `AGENTS.md`**, against the interop-pointer doctrine this
  repo applies to its own `AGENTS.md`.
- **This repo is public, and its own context layer is not written as if it were.**
  `CLAUDE.md` names the owner; `memory/decisions.md` names the parent organization's
  internal structure and now names a killed internal project. Unlike in 12.1.0 this is no
  longer a free choice — the earlier layers are committed and pushed. Decide deliberately
  whether to genericize going forward.
- **2 MB `README.icon.png` committed at root**, referenced by absolute GitHub raw URL anyway.
- **`npx @playwright/mcp@latest` remains unpinned** in the engineering plugin's MCP config.
- **The Claude Desktop / claude.ai / Cowork install path in the README is documented by
  link, not by click-path** — unconfirmed in the live product.

_Cleared items are deleted here, not struck through — a watch list carrying resolved rows
stops being read. (Resolved 2026-08-21 and removed: the 12.0.0/12.1.0-uncommitted row —
both are committed and pushed; the stale remote task store — the tools that owned it are
gone; the `.vscode/` configs pointing at a deleted `src/`.)_
