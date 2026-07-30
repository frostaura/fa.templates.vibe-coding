---
name: ai-toolkit-gaia-do-not-push
description: "RED: local tip b9d2ee3 deletes the MCP server, CI workflow and .mcp (19 files, unmentioned); local build broken; recovery = git checkout origin/main -- src …"
type: alert
last_verified: 2026-07-24
---

# ⚠ Read this before touching the repo or pushing

**The local tip commit deletes the entire MCP server, the CI workflow and the `.mcp` file, and it is unpushed.** Commit `b9d2ee3` ("context + durability pass 2026-07-24: commit working-tree state…") removed 19 files under `src/` — all of `src/Gaia.Mcp.Server` (~800 LOC: `Program.cs`, the four tools, `ThreadSafeJsonStore`, `CompletionValidator`, the models), `src/Gaia.Mcp.Tests/README.md`, `src/README.md`, `src/schemas/` — plus `.github/workflows/build-gaia-mcp.yml` and `.mcp`. Nothing in the commit message mentions a removal; the same commit *added* the `personal` plugin, and it left `Gaia.slnx`, `Dockerfile`, `.gitattributes` and two README links still pointing at `src/`. Read as an accident: an agent ran a catch-all commit over a working tree whose `src/` was absent (iCloud eviction is the likely cause — this volume's standing trap).

Consequences as the repo stands locally: `dotnet build Gaia.slnx` fails (the one project it references does not exist), `docker build` fails, there is **no CI at all**, and the README's `./src` and `./src/Gaia.Mcp.Server` links are dead. **The two "broken README links" the mechanical audit reports are a symptom of this deletion — do not "fix" them by removing the links.**

Nothing is lost: `origin/main` is at `277888e` and still carries `src/`, the workflow and `.mcp`. **Do not push `b9d2ee3` as-is** — pushing it would strip the server and CI from the public repo that every FrostAura project installs from. Recovery, from the repo root:

```
git checkout origin/main -- src .github/workflows/build-gaia-mcp.yml .mcp
git commit -m "restore MCP server, CI workflow and .mcp lost in b9d2ee3"
```

Founder call, because it is product code, not context.
