# MEMORY — ai.toolkit.gaia

- [⚠ Read this before touching the repo or pushing](memory/do-not-push.md) — RED: the unpushed range deletes 21 tracked files (MCP server, CI workflow, .mcp), unmentioned; src/ is genuinely gone from disk; recovery = git checkout origin/main -- src …
- [Current state](memory/state.md) — Shipped plugin product — 4 plugins v10.0.1 in 2 ecosystems; the server half is absent from the local tree; ahead-only but the unpushed range is destructive; remote MCP last verified live 2026-07-19
- [Live decisions](memory/decisions.md) — Flat-JSON over EF+Postgres deliberately; public distribution is the division's one standing stealth exception; plugins pinned to ref:main not tags
- [Gotchas](memory/gotchas.md) — dotnet test gate vacuous (zero test projects, exits 0); personal plugin.json pair already drifted; unpinned playwright MCP pull; root Dockerfile without HEALTHCHECK/VOLUME
- [Open questions](memory/questions.md) — Placement (reads as Technologies); should the personal plugin exist separately; move plugin refs from main to tags
- [Watch list](memory/watch.md) — 12 shipped skills link to a nonexistent doc; .gitattributes describes a dead symlink architecture; marketplace manifests disagree on owner; junk committed at root
