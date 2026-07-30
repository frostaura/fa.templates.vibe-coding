---
name: ai-toolkit-gaia-watch
description: "12 shipped skills link to a nonexistent doc; .gitattributes describes a dead symlink architecture; marketplace manifests disagree on owner; junk committed at root"
type: watch
last_verified: 2026-07-24
---

# Watch list

- **12 product-plugin skills link to `../../../docs/architecture/product-discovery-team.md`, which resolves to `plugins/docs/architecture/…` and does not exist** — nor does any `docs/` directory at the repo root. Confirmed this pass across `fa-product-{discovery,ideation,monetization-design,validation,vertical-slice,production-hardening,compliance,soft-launch,gtm-launch,liveops,portfolio-retrospective,process}`. Shipped to every installed user. Upstream product content: fix it here, at the source, not in the repos that vendor it.
- **`.gitattributes` describes an architecture that no longer exists** — `.claude` → `.github` symlinks repaired by "setup scripts in `/scripts`". There are zero symlinks in the index, zero on disk, no `/scripts`, and no `.github/agents/` or `.github/skills/`.
- **Live shipped guidance points at nothing.** `fa-create-agent` and `fa-create-skill` still instruct mirroring changes across both `.claude/` and `.github/` trees — a layout the repo abandoned.
- **The two marketplace manifests disagree on owner:** `.claude-plugin/` says "FrostAura **Labs**", `.github/plugin/` says "FrostAura **Technologies**". Nothing compares them.
- `plugins/product/plugin.json` keywords, tags and `category` are still copy-pasted from foundation — they literally read `"foundation"` and `"foundational-ai-contribution-toolset"`.
- `AGENTS.md` is a ~500-byte stub with no build commands and no conventions, while the README calls it the workflow contract.
- Junk: a **2 MB `README.icon.png` committed at root** and referenced by absolute GitHub raw URL anyway; `fa-ownership-and-conventions.md` in 7 copies, `fa-delivery-policy.md` in 5.
