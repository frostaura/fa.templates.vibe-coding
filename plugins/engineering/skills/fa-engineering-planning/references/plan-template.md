# Gaia Plan Template

> Use this structure when translating approved architecture into the todo plan. The session's native todo list — kept current as work progresses — is the authoritative plan; this template defines what a complete plan must capture before the first item is created. Keep it concise, current, and aligned to the latest architecture decision.

## Plan Header

Capture these once per plan, in the plan summary or the root todo item:

| Field | Value |
|---|---|
| Request | {{request_name}} |
| Complexity | Trivial / Standard / Complex |
| Owner | {{owner_or_team}} |
| Architecture Reference | {{docs/architecture/... or no-op architecture decision}} |
| Related Specs | {{docs/specs/... or N/A}} |

## 1. Objective and Scope

**Objective**
{{What this work will deliver.}}

**In Scope**
- {{deliverable or change}}

**Out of Scope**
- {{explicit non-goal}}

## 2. Architecture Basis

**Target Solution Summary**
{{Restate the approved architecture delta in 2-4 sentences.}}

**Constraints and Assumptions**
- {{constraint or assumption}}

## 3. Task Breakdown

Publish one todo item per branch of work. Each item must carry:

| Item field | Content |
|---|---|
| Title | {{branch name and expected outcome}} |
| Description | {{scope, owning role, and skills to invoke}} |
| Dependencies | {{items that must complete first, or none}} |
| Required gates | {{the QA and release gates this item must satisfy}} |
| Blockers | {{current blockers, recorded on the item, or none}} |
| Acceptance criteria | {{the completion test, stated so QA can verify it}} |

**Parallel Branches and Dependency Edges**

A complete plan partitions the work into explicitly declared parallel branches; a plan that serializes independent work is a defect. Register mutually independent branches as sibling todo items so they can start concurrently — gates, blockers, and proof stay per-item, so siblings complete independently without softening the completion contract.

- Parallel branches: {{branches that are mutually independent — disjoint file scopes, no shared state — and may run concurrently}}
- True dependency edges: {{branch A → branch B, and why the dependency is real}}
- Barrier / merge gates: {{where parallel work re-joins, and the gate that recombines it}}
- Justified serialization: {{any independent work run serially, and the justification — or none}}
- Scope isolation: {{confirm per-branch file scopes are disjoint; where scopes must overlap, name the worktree isolation plan}}

## 4. QA Strategy

| QA Checkpoint | Coverage | Veto Point | Evidence Required |
|---|---|---|---|
| {{checkpoint}} | {{tests, review, validation}} | {{what blocks progress}} | {{proof}} |

## 5. CI / Release Gates

Encode each gate in the owning task's `required_gates` so completion is mechanically blocked until it is satisfied.

| Gate | Requirement | Owner | Evidence |
|---|---|---|---|
| {{ci_or_release_gate}} | {{must-pass condition}} | {{owner}} | {{artifact or result}} |

## 6. Risks, Blockers, and Open Questions

**Risks**
- {{risk and impact}}

**Blockers**
- {{current blocker or none — record active blockers on the affected tasks}}

**Open Questions**
- {{question needing resolution}}

## 7. Definition of Done

- {{implemented outcome}}
- {{qa expectation met}}
- {{ci or deployment gate passed}}
- {{required proof recorded when the item is closed}}

## 8. Proof of Completion

Proof is recorded when an item is closed, per item. A task cannot complete with unresolved blockers, missing proof, or unsatisfied gates. A todo list dies with the session, so proof that must outlive it goes to the repository's memory store or the PR description — the table below is where you name that location.

| Proof Item | Location / Reference |
|---|---|
| {{test results}} | {{path, PR note, artifact, or log}} |
| {{ci run or deployment validation}} | {{path, PR note, artifact, or log}} |
| {{qa sign-off}} | {{path, PR note, artifact, or log}} |

## 9. Re-Plan Triggers

Re-plan by rewriting the todo list — do not patch around the published plan informally:

- when architecture changes
- when QA discovers a blocking issue or new branch of work
- when dependencies, estimates, or release gates materially change
