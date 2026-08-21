# Gaia Delivery Policy

Use this reference for Gaia-wide delivery rules that apply across architecture, planning, engineering, testing, QA, and release work.

## Core sequence

1. Assess the consuming repository, docs, skills, and CI or deployment state and identify drift first.
2. Update the repository's `docs/architecture` before implementation when the target solution changes.
3. Publish the plan as the session's **native todo list**, with dependencies, required gates, blockers, and proof expectations explicit — and mutually independent branches registered as siblings that may start concurrently.
4. Implement rapidly until the behavior and structure are stable enough for hardening, keeping the todo list current as work progresses.
5. Add formal tests and regression hardening once stable, or earlier when QA risk demands targeted validation.
6. Re-plan by rewriting the todo list when new branches, blockers, risks, or release conditions materially change.
7. Satisfy every required gate and record proof — in the branch's memory entry or its PR description — before closing an item.

## Where the plan and the record live

- **The plan is the session's native todo list.** In Claude Code that is `TodoWrite`; in other harnesses it is whatever equivalent the agent exposes. It is the only authoritative plan: a plan that exists only in prose or a scratch document is not published.
- **Durable facts go to the repository's own memory store** — `MEMORY.md` and its `memory/` topic files, maintained through `fa-foundation-memory-maintenance` and swept at the end of a session by `fa-foundation-session-close`. What survives the session lives there, not in the todo list.
- **Improvements, lessons and process upgrades are logged as memories** in that same store, with the reasoning that produced them. A lesson without its why gets reversed by the next agent who sees only its cost.
- **Neither is a hosted service.** Both are files in the consuming repository, so they version with the code, review in the same pull request, and survive a machine change without anything to sync.

## Shared rules

- The consuming repository's `docs/architecture` is the design source of truth.
- The lifecycle stages are per-branch role gates, not a serial relay: independent branches (disjoint file scopes, no shared state) fan out concurrently, may each sit at a different stage, and re-join at gates. A plan that serializes independent work is a defect.
- Concurrent edits to one repository require disjoint file scopes per agent; where scopes must overlap, use isolated git worktrees per agent and merge deliberately. Two agents in one scope overwrite each other.
- **A task cannot be closed with unresolved blockers, missing proof, or unsatisfied gates.** That friction is deliberate; do not work around it. Gates, blockers, and proof are per-task, so parallel branches complete independently — parallelism never softens this contract.
- **The completion contract is now enforced by the roles, not by a server.** It used to be refused mechanically by the MCP task tools; those are gone, so the discipline rests on the agents that apply this policy — principally QA's veto. Weaker enforcement is a reason to apply it more literally, not less.
- QA is active throughout delivery and may veto progression or closure.
- Release validation and proof are part of delivery, not optional follow-up work.
- Use Gaia skills and agents by default.

## Early versus late testing

- Rapid implementation may happen before broad formal test authoring.
- Targeted smoke checks, exploratory validation, or high-risk tests may happen earlier when needed.
- Formal test artifacts, regression expansion, and testing evidence belong to the testing phase and to `fa-engineering-testing`.
