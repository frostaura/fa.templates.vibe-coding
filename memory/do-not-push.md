---
name: ai-toolkit-gaia-do-not-push
description: "RED: the unpushed range deletes 21 tracked files — MCP server, CI workflow and .mcp — unmentioned; src/ is genuinely absent on disk; local build and CI are gone; recovery = git checkout origin/main -- src …"
type: alert
last_verified: 2026-08-21
---

# ⚠ Read this before touching the repo or pushing

**The unpushed range deletes the entire MCP server, the CI workflow and `.mcp`.** `main` is ahead-only of `origin/main`, so a plain `git push` **succeeds**. The oldest commit in the range, `b9d2ee3` ("context + durability pass 2026-07-24: commit working-tree state…"), removes **21 files**: all of `src/Gaia.Mcp.Server` (~800 LOC — `Program.cs`, the four tools, `ThreadSafeJsonStore`, `CompletionValidator`, the six models), `src/README.md`, `src/schemas/`, `src/Gaia.Mcp.Tests/README.md`, plus `.github/workflows/build-gaia-mcp.yml` and `.mcp`. Nothing in the commit message mentions a removal; the same commit *added* the `personal` plugin and left `Gaia.slnx`, `Dockerfile`, `.gitattributes` and two README links still pointing at `src/`. The commits on top of it (`ea88af9`, `6770272`, both 2026-07-30) are context-only and delete nothing — so **the destructive commit is buried in the middle of the range, not at the tip**, and no single-commit fix reaches it.

Read as an accident: a catch-all commit run over a working tree whose `src/` was absent (iCloud eviction is the likely cause — this volume's standing trap).

Consequences as the repo stands locally: `src/` does not exist on disk, `dotnet build Gaia.slnx` fails (the one project it references is gone), `docker build` fails, `.github/workflows/` does not exist so there is **no CI at all**, and the README's `./src` and `./src/Gaia.Mcp.Server` links are dead. **The two "broken README links" the mechanical audit reports are a symptom of this deletion — do not "fix" them by removing the links; that would hide the only visible trace of the problem.**

Nothing is lost: `origin/main` is at `277888e` and still carries `src/`, the workflow and `.mcp`. **Do not push** — it would strip the server and CI from the public repo that every FrostAura project installs from. Recovery, from the repo root:

```
git checkout origin/main -- src .github/workflows/build-gaia-mcp.yml .mcp
git commit -m "restore MCP server, CI workflow and .mcp lost in b9d2ee3"
```

Founder call, because it is product code, not context.
