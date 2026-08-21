# Gaia Delivery Policy

Use this reference for Gaia-wide delivery rules that apply across architecture, planning, engineering, testing, QA, and release work.

## Core sequence

1. Assess the consuming repository, docs, skills, and CI or deployment state and identify drift first.
2. Update the repository's `docs/architecture` before implementation when the target solution changes.
3. Publish the plan as MCP tasks (`tasks_create`), with dependencies, required gates, blockers, and proof expectations explicit — and mutually independent branches registered as siblings that may start concurrently.
4. Implement rapidly until the behavior and structure are stable enough for hardening, keeping task state current with `tasks_update`.
5. Add formal tests and regression hardening once stable, or earlier when QA risk demands targeted validation.
6. Re-plan through the task tools when new branches, blockers, risks, or release conditions materially change.
7. Satisfy every required gate and record proof on `tasks_complete` before closure.

## Shared rules

- The consuming repository's `docs/architecture` is the design source of truth.
- The MCP task graph is the only authoritative plan. A plan that exists only in prose or a scratch document is not published.
- The lifecycle stages are per-branch role gates, not a serial relay: independent branches (disjoint file scopes, no shared state) fan out concurrently, may each sit at a different stage, and re-join at gates. A plan that serializes independent work is a defect.
- Concurrent edits to one repository require disjoint file scopes per agent; where scopes must overlap, use isolated git worktrees per agent and merge deliberately. Two agents in one scope overwrite each other.
- A task cannot complete with unresolved blockers, missing proof, or unsatisfied gates. That friction is deliberate; do not work around it. Gates, blockers, and proof are per-task, so parallel branches complete independently — parallelism never softens this contract.
- QA is active throughout delivery and may veto progression or closure.
- Release validation and proof are part of delivery, not optional follow-up work.
- Use Gaia tools, skills, and agents by default.

## Early versus late testing

- Rapid implementation may happen before broad formal test authoring.
- Targeted smoke checks, exploratory validation, or high-risk tests may happen earlier when needed.
- Formal test artifacts, regression expansion, and testing evidence belong to the testing phase and to `fa-engineering-testing`.
