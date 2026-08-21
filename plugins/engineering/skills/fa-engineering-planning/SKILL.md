---
name: fa-engineering-planning
description: Provides execution planning guidance that turns approved architecture into a branch-aware plan with dependencies, QA checkpoints, release gates, and proof expectations. Use it by translating the current architecture into native todo items sequenced by branch, with explicit required gates and blockers, then keeping the plan current as new work is discovered. Use it after architecture is current, when work needs explicit sequencing instead of informal next steps, or when new branches, blockers, or gate definitions require re-planning. It sequences work and never decides the target solution — that is `fa-engineering-architecture` — nor implements the branches it declares.
license: MIT
---

# Gaia Planning

## Scope and when to use

Use this skill to publish the execution tree — as the session's native todo
list — that tells Gaia what can start, what must wait, and how the work will be
validated and closed. The todo list is the authoritative plan; there is no
separate plan file and no hosted task service. Anything that must outlive the
session is written to the repository's memory store instead (see the delivery
policy).

Use this skill when:

- architecture is current and the work needs sequencing
- delivery spans multiple branches, dependencies, or owners
- QA, release, or proof expectations need to be explicit before implementation
- execution reveals new work streams that materially change the plan

Do not use this skill when:

- design is still missing or stale
- the branch only needs local implementation against an already current plan
- the only remaining work is release-ready interpretation

## Required inputs

- the approved architecture basis
- the request summary, constraints, and non-goals
- current repo, CI, deployment, and testing constraints
- the existing todo list, blockers, and re-plan triggers

## Owned outputs

- a current execution plan, published as native todos with branch boundaries and dependencies
- explicit acceptance criteria, QA checkpoints, and release gates
- explicit parallel-branch declarations, disjoint-scope boundaries, and merge-gate decisions
- proof expectations and re-plan triggers

## Decision tree

- If architecture is stale, send the work back before planning.
- If the work is simple but still risky, keep the plan concise but make QA and proof explicit.
- If branches are independent, mark them as parallel-safe and define the merge gate.
- If criteria or gate ownership are missing, block plan publication until they exist.
- If execution exposes new branches or blocker types, re-publish the plan by rewriting the todo list instead of patching around it informally.

## Core workflow

1. Restate the target solution from architecture in a short, testable summary.
2. Break the work into delivery branches by capability, dependency, or ownership, partitioning for maximum safe parallelism — branches with disjoint file scopes and no shared state are independent by default.
3. For each branch, create a todo item with outcome, dependencies, owner roles, skills, and acceptance criteria; register mutually independent branches as siblings so they can start concurrently.
4. Declare which branches are mutually independent, name every true dependency edge and barrier point, and define the gate that recombines parallel work.
5. Attach QA checkpoints, release gates, and proof expectations directly to the todo items.
6. Record blockers, assumptions, open questions, and re-plan triggers on the items, keep them current as work progresses, and promote the ones that must survive the session to the repository's memory store.

## Parallelism and merge rules

- independence test: disjoint file scopes and no shared mutable state; branches that pass it are parallel by default
- a plan that serializes independent work is a planning defect — any serialization of branches that pass the independence test must be justified in the plan
- parallelize only when branch outputs do not compete for the same artifact or decision; overlapping file scopes require either a true dependency edge or isolated worktrees per agent
- mark every parallel branch with the condition that allows it to start
- define the merge gate that recombines parallel work into one validated branch
- gates, blockers, and proof are per-task, so parallel sibling branches complete independently — parallelism never softens the completion contract
- re-plan immediately when a supposedly independent branch gains a new dependency

## Failure recovery

| Failure mode                | Recovery                              | Owner   | Escalation                               |
| --------------------------- | ------------------------------------- | ------- | ---------------------------------------- |
| stale architecture          | stop and request design clarification | planner | send to architect                        |
| missing acceptance criteria | write or request testable outcomes    | planner | block downstream work                    |
| dependency loop             | split or re-sequence the work         | planner | escalate if no clean branch model exists |
| gate ambiguity              | assign gate ownership and evidence    | planner | involve release if needed                |

## Anti-patterns

- do not use a linear checklist when the work has real dependencies
- do not hide QA under a generic future test task
- do not delegate implementation from an undocumented target solution
- do not treat proof recording as a nice-to-have

## Handoff and downstream impact

- give engineering branch-level acceptance criteria and dependency context
- give testing the QA checkpoints and evidence model before validation starts
- give release the gates, proof expectations, and ready-state definition
- give intake clear re-plan triggers for future workflow resets

## Examples

- **Good fit:** split a Gaia definition overhaul into architecture baseline, contract rewrite, agent rewrite, skill rewrite, and validation branches.
- **Good fit:** re-plan after validation shows the current acceptance criteria are incomplete.
- **Not a fit:** decide what the architecture should be in the first place.

## Completion checklist

- each branch has an owner, outcome, and acceptance criteria
- independent branches are declared parallel, true dependency edges and barriers are named, and any serialization of independent work is justified
- QA, release, and proof work are embedded rather than implied
- blockers and re-plan triggers are visible to downstream roles

## References

- [Gaia delivery policy](../../references/delivery-policy.md)
- [Gaia ownership and conventions](../../references/ownership-and-conventions.md)
- [Plan template](references/plan-template.md)
