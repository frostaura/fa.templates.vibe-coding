# MEMORY — ai.toolkit.gaia

_Last verified: 2026-07-24 by the Labs tree-audit pass (git history, `Gaia.slnx`, both marketplace manifests, every `plugin.json`, `Dockerfile`, `.gitattributes` and the plugin skill links inspected directly)._

## ⚠ Read this before touching the repo or pushing

**The local tip commit deletes the entire MCP server, the CI workflow and the `.mcp` file, and it is unpushed.** Commit `b9d2ee3` ("context + durability pass 2026-07-24: commit working-tree state…") removed 19 files under `src/` — all of `src/Gaia.Mcp.Server` (~800 LOC: `Program.cs`, the four tools, `ThreadSafeJsonStore`, `CompletionValidator`, the models), `src/Gaia.Mcp.Tests/README.md`, `src/README.md`, `src/schemas/` — plus `.github/workflows/build-gaia-mcp.yml` and `.mcp`. Nothing in the commit message mentions a removal; the same commit *added* the `personal` plugin, and it left `Gaia.slnx`, `Dockerfile`, `.gitattributes` and two README links still pointing at `src/`. Read as an accident: an agent ran a catch-all commit over a working tree whose `src/` was absent (iCloud eviction is the likely cause — this volume's standing trap).

Consequences as the repo stands locally: `dotnet build Gaia.slnx` fails (the one project it references does not exist), `docker build` fails, there is **no CI at all**, and the README's `./src` and `./src/Gaia.Mcp.Server` links are dead. **The two "broken README links" the mechanical audit reports are a symptom of this deletion — do not "fix" them by removing the links.**

Nothing is lost: `origin/main` is at `277888e` and still carries `src/`, the workflow and `.mcp`. **Do not push `b9d2ee3` as-is** — pushing it would strip the server and CI from the public repo that every FrostAura project installs from. Recovery, from the repo root:

```
git checkout origin/main -- src .github/workflows/build-gaia-mcp.yml .mcp
git commit -m "restore MCP server, CI workflow and .mcp lost in b9d2ee3"
```

Founder call, because it is product code, not context.

## Current state
**Shipped as a plugin product; the server half is currently broken locally (see above).** Branch `main`, working tree clean, one commit (`b9d2ee3`, 2026-07-24) ahead of `origin/main`. Last product work before the audit passes was `006cad3` (2026-06-26). **Four** plugins now ship — `foundation`, `engineering`, `product` and `personal` (added 2026-07-24) — all at version `10.0.1`, marketplace metadata `1.1.0`, in two ecosystems. `personal` is a near-copy of `foundation` (`fa-create-agent`, `fa-create-plan`, `fa-create-skill`). The remote MCP at `https://gaia.frostaura.net/mcp` was last verified live 2026-07-19; not re-checked this pass.

## Live decisions
- **Flat-JSON persistence over EF Core + Postgres** is deliberate for a service of this size. Recorded here because it is recorded nowhere else, and because this repo ships the skill mandating the opposite to every other project.
- **Public distribution is the one recorded stealth exception in Labs.** Do not relitigate.
- **Plugins are pinned to `ref: main`, not a tag** — a conscious choice whose consequence is that installs float with `main`.

## Gotchas
- **The `dotnet test` CI gate is vacuous — re-confirmed against `origin/main` this pass.** `Gaia.slnx` references exactly one project (`src/Gaia.Mcp.Server`). `src/Gaia.Mcp.Tests/` holds a single `README.md`, no `.csproj` and no `.cs`, and that README points at `tests/Gaia.Mcp.Server.Tests/` and `tests/Gaia.Mcp.Server.IntegrationTests/` — **`tests/` has never existed in this repo's history**. `dotnet test Gaia.slnx` discovers zero test projects and exits 0. The empty `Gaia.Mcp.Tests` directory is worse than nothing: it makes the repo look tested at a glance. The product that ships `fa-testing` has no tests of its own.
- **`plugins/personal/plugin.json` and `plugins/personal/.claude-plugin/plugin.json` have already drifted** — the duplicate lists `"foundation"` where the primary lists `"personal"`. The repo's own invariant says these must stay byte-identical; nothing in CI checks it. The other three plugins' pairs are identical.
- **`npx @playwright/mcp@latest` is pulled unpinned** on every engineering-plugin invocation — a live supply-chain and reproducibility exposure.
- Dockerfile runs as **root**, installs curl but declares no `HEALTHCHECK`, and declares no `VOLUME` for `/app/data` — **container data is ephemeral unless the host mounts it.**
- MCP `ServerInfo.Version` was hardcoded `"1.0.0"` against a v10.0.1 product (in the deleted `Program.cs`; re-check on restore).

## Open questions
- **Placement.** Everything here reads as `Technologies/`: a versioned, publicly distributed, multi-ecosystem developer product with a hosted service and a support address. Parent-controlled; founder owns the call.
- **Whether `personal` should exist as a separate plugin** or be a profile of `foundation`. Three of its four files are duplicates, and duplication is already this repo's dominant defect.
- **Whether to move plugin refs from `main` to tags.** Floating installs are tolerable for a founder-only user base and dangerous for a public one.

## Watch list
- **12 product-plugin skills link to `../../../docs/architecture/product-discovery-team.md`, which resolves to `plugins/docs/architecture/…` and does not exist** — nor does any `docs/` directory at the repo root. Confirmed this pass across `fa-product-{discovery,ideation,monetization-design,validation,vertical-slice,production-hardening,compliance,soft-launch,gtm-launch,liveops,portfolio-retrospective,process}`. Shipped to every installed user. Upstream product content: fix it here, at the source, not in the repos that vendor it.
- **`.gitattributes` describes an architecture that no longer exists** — `.claude` → `.github` symlinks repaired by "setup scripts in `/scripts`". There are zero symlinks in the index, zero on disk, no `/scripts`, and no `.github/agents/` or `.github/skills/`.
- **Live shipped guidance points at nothing.** `fa-create-agent` and `fa-create-skill` still instruct mirroring changes across both `.claude/` and `.github/` trees — a layout the repo abandoned.
- **The two marketplace manifests disagree on owner:** `.claude-plugin/` says "FrostAura **Labs**", `.github/plugin/` says "FrostAura **Technologies**". Nothing compares them.
- `plugins/product/plugin.json` keywords, tags and `category` are still copy-pasted from foundation — they literally read `"foundation"` and `"foundational-ai-contribution-toolset"`.
- `AGENTS.md` is a ~500-byte stub with no build commands and no conventions, while the README calls it the workflow contract.
- Junk: a **2 MB `README.icon.png` committed at root** and referenced by absolute GitHub raw URL anyway; `fa-ownership-and-conventions.md` in 7 copies, `fa-delivery-policy.md` in 5.
