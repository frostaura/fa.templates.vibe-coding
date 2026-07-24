# ai.toolkit.gaia — Gaia

## Research question
**Answered, and the program knows it.** The original question — can a spec-driven, role-decomposed agent workflow with an enforced completion contract produce more reliable software delivery than ad-hoc prompting — has been settled affirmatively in practice: Gaia is the workflow FrostAura actually builds with. What remains is distribution and durability, not research. Treat any new "research question" framing here with suspicion; the honest read is that this graduated and the paperwork lagged.

## What this program is
FrostAura's spec-driven AI development toolkit, shipped as **three installable plugins** — `foundation`, `engineering`, `product` — for **both** Claude Code and GitHub Copilot, backed by a small .NET 10 MCP server hosted at `https://gaia.frostaura.net/mcp`. The plugins carry the skills and agent definitions that encode FrostAura's delivery method (intake → architecture → planning → engineering → testing → release). The server exposes tools across tasks, memory and evolution, and enforces a **completion contract**: a task cannot be marked done with unresolved blockers, missing proof arguments, or unsatisfied gates. Gaia is simultaneously the tool FrostAura uses and a public developer product.

## Stack & repo
`github.com/frostaura/ai.toolkit.gaia` (public). **.NET 10 only.** There is **no EF Core, no PostgreSQL, no React, and no frontend of any kind** — persistence is flat JSON files on disk via `ThreadSafeJsonStore<T>`. This is a deliberate deviation from the mandated stack, justified by the size and shape of the service, and it is worth naming plainly: this repo ships the `fa-default-tech-stack` skill telling every other FrostAura project to use EF Core + PostgreSQL + React + Tailwind + shadcn, while using none of them itself. That irony is acceptable; leaving it undocumented is not.

Layout: `plugins/{foundation,engineering,product}/` hold the shipped skill and agent definitions; `src/` holds the MCP server; `.claude-plugin/` and `.github/plugin/` hold the two marketplace manifests.

## Owner & key people
Dean Martin (founder). Gaia is the delivery substrate for every other FrostAura program, which makes its blast radius larger than its LOC suggests.

## Milestones
1. **Two-ecosystem distribution** — installable and working in both Claude Code and GitHub Copilot from a published marketplace.
2. **Hosted MCP reliability** — the remote server is the default persistence backend for foundation-plugin users and must behave like a service, not a prototype.
3. **A real test suite** — see kill criteria; a delivery-discipline product with no tests of its own is a credibility problem, not just a quality one.
4. **Placement resolved** — moved to `Technologies/` or explicitly retained in Labs with a reason.

## Kill criteria
- The workflow stops being what FrostAura actually builds with. Gaia's whole justification is that it is dogfooded; the day it is not, it is dead weight.
- The hosted MCP cannot be operated to service standards and no one is willing to move it to a division that can.
- Maintaining plugin parity across two ecosystems costs more than the second ecosystem returns.

## Graduation pathway
`Technologies/`, and overdue. This is a versioned, publicly distributed, multi-ecosystem developer product on v10 with a live hosted service, CI/CD and a support address. It has no research character left, and it is the only Labs program with a granted stealth exception — which is precisely the signature of something that already graduated. Keep the OSS posture; move the home. Placement is parent-controlled: recommend, escalate, do not move it from inside Labs.

## Conventions & invariants
- **The plugins are the product; `src/` is supporting infrastructure.** A change to a skill or agent definition ships to every installed user on the next resolve — treat plugin content edits with production seriousness.
- **Plugin sources are pinned to `ref: main`, not to a tag.** Installs therefore float with `main`. Any breaking change to a skill or agent contract lands on users immediately. Either respect that or move to tags deliberately.
- **The completion contract is the point.** Tasks cannot complete with unresolved blockers, missing proof, or unsatisfied gates. Do not add an escape hatch to make a workflow feel smoother — the friction is the feature.
- **The two marketplace manifests must agree.** `.claude-plugin/marketplace.json` and `.github/plugin/marketplace.json` describe the same product to two ecosystems, and every plugin additionally has a byte-identical duplicate `plugin.json` at two paths. Nothing in CI checks any of this; check it by hand on every change.
- **Hosted-service disclosure is a commitment.** The README states the remote MCP stores evolution suggestions, task plans and project memory segregated by project name, and does **not** store user PII, code, specs or docs. Do not add persistence that breaks that statement without changing the statement first.
- **Shared reference docs are duplicated by design of the plugin format**, not by accident — the same file exists in several plugin trees. When editing one, edit all copies or the plugins disagree with each other.

## Build / test / run
| Task | Command |
| --- | --- |
| Build | `dotnet build Gaia.slnx` |
| Test | `dotnet test` — the test gate asserts nothing until real test projects exist, so **green is not coverage**; check `MEMORY.md` for what actually runs |
| Run (HTTP) | `dotnet run --project src/...` — the web host is the default |
| Run (stdio) | requires `MCP_TRANSPORT=stdio`; without it `Program.cs` starts the HTTP web host on stdio and misbehaves |

Container image publishing goes to Docker Hub under a namespace derived from a CI secret, so the published image name is not discoverable from the repo alone.

## Where docs live
The README is the entry point. `AGENTS.md` is nominally the workflow contract. Both are known to drift from the shipped reality — read `MEMORY.md` before trusting either.

## Never do this here
- Never change a skill or agent definition without checking every duplicated copy across plugin trees.
- Never let the two marketplace manifests or the duplicated `plugin.json` files drift.
- Never treat a green `dotnet test` as evidence of anything until real test projects exist.
- Never weaken the completion contract to unblock a stuck workflow.
- Never add persistence of user code, specs or docs to the hosted service without amending the README's disclosure.

## Upkeep
This `CLAUDE.md`, [`MEMORY.md`](MEMORY.md) and any local `.claude/skills/` here are kept current as changes land. `MEMORY.md` is updated at the end of any session that shipped, decided, discovered or abandoned something, and the program's row in [`../MEMORY.md`](../MEMORY.md) is updated in the same pass. A skill that no longer matches how work is actually done is fixed or deleted, not left to rot. The standard: [`/docs/operating/context-cascade.md`](../../../docs/operating/context-cascade.md).
