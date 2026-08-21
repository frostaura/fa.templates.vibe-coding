---
name: ai-toolkit-gaia-do-not-push
description: "AMBER (was RED): b9d2ee3's 21-file deletion is now countered by a STAGED, uncommitted restore of src/, CI and .mcp (founder-ordered 2026-08-21, build+run verified); push only AFTER that restore is committed — the committed range alone is still destructive"
type: alert
last_verified: 2026-08-21
---

# ⚠ Read this before touching the repo or pushing

**Status 2026-08-21 (evening): the founder called restore, and it is done — but only in the index.** All 21 files deleted by `b9d2ee3` — the entire `src/Gaia.Mcp.Server` (~800 LOC), `src/README.md`, `src/schemas/`, `src/Gaia.Mcp.Tests/README.md`, `.github/workflows/build-gaia-mcp.yml`, `.mcp` — were restored byte-identical from `origin/main` via `git checkout origin/main -- src .github/workflows/build-gaia-mcp.yml .mcp` and are **staged but uncommitted** (standing founder instruction this session: commit nothing). Verified after restore: `dotnet build Gaia.slnx` green (with `MSBuildEnableWorkloadResolver=false`, this volume's trap), and the server answers an MCP `initialize` on `localhost:5059`.

**Why this is amber, not closed:** the *committed* range (`origin/main..a5b434b`, 5 commits) still deletes those 21 files — `b9d2ee3` is buried mid-range, and no single-commit revert reaches it. A push of the commits **without first committing the staged restore** would still strip the server and CI from the public repo every FrostAura project installs from. The safe shape is now simple:

1. Commit the working tree (staged restore + the uncommitted v11.0.0 rework) — founder review first, per instruction.
2. Then push. Net effect vs `origin/main` at that point: zero deletions; `git diff origin/main...HEAD --diff-filter=D` must confirm empty before the push. Run the repo-durability check regardless.

Anyone unstaging, committing selectively around `src/`, or "cleaning" the index re-arms the original RED condition. History of the incident (accidental catch-all commit over an iCloud-evicted tree, deletion unmentioned in any message) is preserved in this file's git history and in the report at the session artifact.
