---
name: ai-toolkit-gaia-watch
description: "13.0.0 is pushed and live to every installed user, but the hosted MCP still serves the OLD tool set — plugins and server now disagree; the v13 commit deliberately deletes 14 files so a durability check will flag it; the frostaura marketplace was uninstalled from this machine; context-audit.py still cannot see any plugin skill; playwright MCP unpinned"
type: watch
last_verified: 2026-08-21
---

# Watch list

- **⚠ The plugins and the hosted server now disagree, and users have the new plugins.**
  v13.0.0 is on `origin/main`, and because sources float on `ref: main` it reached every
  installed user immediately — while `gaia.frostaura.net/mcp` still serves the build with
  `tasks_*`/`memory_*`/`evolve_*`. Nothing in the shipped plugins calls those any more, so
  the practical risk is low, but the live service contradicts its own repository until it is
  redeployed. **Highest-priority follow-up.**

- **The Woolworths reverse-engineering is now public.** Founder-authorised 2026-08-21 with
  the exposure stated. The remaining work is framing, not prevention: the README carries no
  stance on the integration — no statement that it is a personal-use tool, unaffiliated with
  and unendorsed by Woolworths. See [`questions.md`](questions.md).
- **v13.0.0 deleted 15 files on purpose** (the 14 tasks/memory/evolve sources and schemas,
  plus the resolved `do-not-push.md` alert). Now in origin's history. **Expected, enumerated
  in `state.md`, not a recurrence** — do not turn it into a restore.
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
