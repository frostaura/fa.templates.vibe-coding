---
name: fa-engineering-testing
description: Provides Gaia's formal validation guidance across unit, integration, manual regression, automated regression, and evidence review. Use it by running the test layers required by each task's gates (unit → integration → e2e → manual regression), recording proof labels (e.g. 'curl', 'playwright-mcp') when closing a todo item, and vetoing completion when evidence is missing. Use it when a branch is stable enough for hardening, when high-risk behavior needs targeted early validation, or when release readiness depends on a clear QA signal. It decides which layers a task's gates require and never authors the browser specs or runs the hand-executed checks itself — those are `fa-engineering-e2e-testing` and `fa-engineering-manual-regression`.
license: MIT
---

# Gaia Testing

## Scope and when to use

Use this skill to turn validation into explicit evidence and explicit ownership,
not just a vague "tests passed" summary.

Use this skill when:

- a stable branch needs formal validation or regression hardening
- high-risk behavior needs targeted early QA
- UI, integration, or user-visible behavior requires direct observation
- release readiness needs a clear QA outcome with supporting evidence

Do not use this skill when:

- the branch is too unstable for meaningful validation
- acceptance criteria are missing and need planning work first
- the only remaining task is interpreting release gates

## Required inputs

- the branch or deliverable under test
- acceptance criteria, architecture context, and identified risk areas
- relevant environments, commands, fixtures, or browser flows
- existing validation artifacts that should be extended or replaced

## Owned outputs

- validation artifacts and evidence appropriate to the change
- explicit pass, fail, or blocked outcomes
- true-owner routing for defects or blockers
- QA notes that a release reviewer can use directly

## Decision tree

- If the branch is not stable, send it back to engineering.
- If criteria are missing or contradictory, send the work to planning.
- If validation reveals design drift, route to architecture.
- If validation passes, package the evidence for release review.
- If only targeted early validation is warranted, scope it narrowly and say what remains for full hardening.

## Core workflow

1. Choose the validation layers that match the risk and surface area.
2. Add or update the required tests, fixtures, or manual validation notes.
3. Run independent lanes concurrently when they share no mutable state; serialize only lanes that contend for the same environment.
4. Validate user-visible behavior directly when visuals or browser flows matter.
5. Record the single pass, fail, or blocked outcome where the lane results join, with evidence and true-owner routing.
6. Hand successful evidence to release review and unsuccessful evidence to the real failure owner.

## Coverage layers

- use focused unit or structural checks for local logic and small rule changes
- use integration validation for cross-component behavior and boundaries
- use browser or manual regression when user-visible flow or visual quality matters
- lanes that share no mutable state (fixtures, databases, ports, browser profiles) are independent and run in parallel; a lane serializes only against the lanes it contends with
- scale evidence to the risk of the change instead of chasing raw coverage numbers

## Failure recovery

| Failure mode     | Recovery                                      | Owner  | Escalation |
| ---------------- | --------------------------------------------- | ------ | ---------- |
| unstable branch  | stop formal QA and request stabilization      | tester | engineer   |
| missing criteria | request plan clarification                    | tester | planner    |
| design mismatch  | surface architecture drift                    | tester | architect  |
| weak evidence    | strengthen validation before passing the work | tester | stay in QA |

## Anti-patterns

- do not equate DOM presence with user-visible correctness
- do not hide planning or design defects inside generic failing test notes
- do not approve a branch with weak or incomplete evidence
- do not assume automated checks alone are enough for visual or UX-sensitive work
- do not report on-device behaviour that was only observed on an emulator or simulator

## Handoff and downstream impact

- give release the exact evidence needed for a readiness call
- give engineering precise reproduction details for implementation defects
- give planning the missing criteria when validation is blocked by ambiguity
- give architecture the mismatch when documented behavior and observed behavior diverge
- **this skill owns test strategy, not the execution lanes.** Hand browser coverage and its
  traceable identifiers to `fa-engineering-e2e-testing`, and the manual API and web passes to
  `fa-engineering-manual-regression`. Both defer back here for what is worth covering.

## Examples

- **Good fit:** review Gaia's rewritten definitions for coherence, line-count compliance, and supporting evidence.
- **Good fit:** use browser automation or manual regression notes for user-visible Copilot plugin workflows.
- **Not a fit:** decide what the success criteria should be when the plan never defined them.

## Completion checklist

- the outcome is clearly pass, fail, or blocked
- evidence matches the risk and surface area of the branch
- the true failure owner is named when the result is not a pass
- release review can proceed without guessing what QA actually proved

## References

- [Gaia delivery policy](../../references/delivery-policy.md)
- [Gaia ownership and conventions](../../references/ownership-and-conventions.md)
