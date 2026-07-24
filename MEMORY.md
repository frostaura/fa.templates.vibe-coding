# MEMORY — ai.toolkit.gaia

_Last verified: 2026-07-20 by Labs audit (git, solution file, CI workflow and marketplace manifests inspected directly)._

## Current state
**Shipped as a product; prototype-grade as a service; dormant since 2026-06-26.** 123 commits spanning 2025-12-11 → 2026-06-26 on branch `main`, remote set. Version **10.0.1** across all three plugins, publicly installable in two ecosystems. The remote MCP at `https://gaia.frostaura.net/mcp` was verified live on 2026-07-19. Working tree is dirty: `.vscode/settings.json` staged, plus untracked `CLAUDE.md`, `MEMORY.md` and a `.claude/` directory holding ~25 agent and skill files that duplicate `plugins/product/`.

**The tip commit is unpushed.** `006cad3` "chore(mcp): giving the mcp side some basic love" (2026-06-26) exists only locally — `origin/main` is one commit behind. Public GitHub is therefore not showing the current state of the repo, which for a publicly installed product pinned to `ref: main` means installed users are on the *older* code than the working copy suggests.

## Live decisions
- **Flat-JSON persistence over EF Core + Postgres** is deliberate for a service of this size. Documented here because it is documented nowhere else, and because the repo ships the skill mandating the opposite to everyone else.
- **Public distribution is the one recorded stealth exception in Labs.** Do not relitigate it.
- **Plugins are pinned to `ref: main`** rather than a tag — a conscious choice with the consequence that installs float.

## Gotchas
- **The `dotnet test` CI gate is vacuous and passes green — re-confirmed 2026-07-20.** `Gaia.slnx` references exactly one project, `src/Gaia.Mcp.Server`. `src/Gaia.Mcp.Tests/` contains a single `README.md` and no `.csproj` or `.cs` at all, pointing at `tests/Gaia.Mcp.Server.Tests/` — and **`tests/` does not exist**. `dotnet test Gaia.slnx` therefore discovers zero test projects and exits 0. The empty `Gaia.Mcp.Tests` directory is worse than nothing: it makes the repo look tested at a glance. The `.csproj` also carries `InternalsVisibleTo` for two test projects that do not exist. The repo that ships `fa-testing` (enforcing unit → integration → e2e gates) has no tests of its own.
- **`npx @playwright/mcp@latest` is pulled unpinned on every engineering-plugin invocation** — a live supply-chain and reproducibility exposure.
- The `.mcp` file is **extensionless**, so Claude Code will not auto-discover it. It sets `GAIA_DATA_DIR: "docs"` — and `docs/` does not exist. Its stdio entry omits `MCP_TRANSPORT=stdio`, which `Program.cs` requires; as written it would start the HTTP web host on stdio and likely break.
- Dockerfile runs as **root**, installs curl but declares no `HEALTHCHECK`, and declares no `VOLUME` for `/app/data` — **container data is ephemeral unless the host mounts it.**
- MCP `ServerInfo.Version` is hardcoded `"1.0.0"` while the product is v10.0.1.

## Open questions
- **Placement.** Everything about this reads as `Technologies/`. Parent-controlled; founder owns the call.
- **Whether the untracked `.claude/` directory is intentional workspace state or an accidental duplicate** of `plugins/product/`. Resolve before it drifts against the shipped copy.
- **Whether to move plugin refs from `main` to tags.** Floating installs are fine for a founder-only user base and dangerous for a public one.

## Watch list
- **No `CLAUDE.md` existed until 2026-07-19**, and `AGENTS.md` is a ~500-byte stub — three headings, no build commands, no conventions — despite the README claiming "the workflow contract lives in `AGENTS.md`".
- **`.gitattributes` describes an architecture that no longer exists.** It says to keep `.claude` → `.github` symlinks as real symlinks in the git index and that "the setup scripts in `/scripts` repair them". There are **zero symlinks in the index, zero on disk, no `/scripts` directory, and no `.github/agents/` or `.github/skills/`**. The layout migrated to `plugins/` and this file was never cleaned up.
- **Live shipped guidance points at nothing.** The `foundation:fa-create-agent` and `fa-create-skill` skill descriptions still instruct mirroring changes across both `.claude/agents/` and `.github/agents/` trees — a layout that no longer exists. Every user of the foundation plugin reads this.
- **The two marketplace manifests have already drifted:** `.claude-plugin/marketplace.json` says owner "FrostAura **Labs**"; `.github/plugin/marketplace.json` says "FrostAura **Technologies**". Nothing in CI compares them.
- `plugins/product/plugin.json` keywords and tags are copy-pasted from foundation — they literally say `"foundation"`.
- `src/README.md` documents tools as `tasks.mark_done` / `tasks.update` / `tasks.flag_needs_input`; the code registers **`tasks_complete` / `tasks_update` / `tasks_request_input`**.
- README references architecture specs in `docs/` — there is no `docs/` folder.
- Junk: a **2 MB `README.icon.png` committed at root** and referenced by absolute GitHub raw URL anyway; dead `Tools/ExampleTools.cs` echo scaffolding that is never registered; the placeholder `src/Gaia.Mcp.Tests/` README; `fa-ownership-and-conventions.md` in 7 copies and `fa-delivery-policy.md` in 5.
