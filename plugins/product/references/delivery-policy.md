# Product Plugin — Delivery Policy

Use this reference for the delivery rules that apply across the product
lifecycle: how stage work is tracked, gated, and closed. The authoritative
work-tracking mechanism is the Gaia MCP task workflow — there is no plan file.

## Task workflow (Gaia MCP)

1. Create stage work as MCP tasks via `tasks_create`, with `required_gates` and
   any known `blockers` declared up front — for product stages the gate is the
   pre-registered money gate (`fa-product-money-gate`), plus any
   evidence-specific gates (e.g. sandbox-purchase proof, signed compliance
   certificate, store-CVR A/B). Register mutually independent work as sibling
   tasks so the parallel branches are explicit in the task graph — a task set
   that serializes independent work is a defect. Gates, blockers, and proof are
   per-task, so sibling branches complete independently; this composes with
   parallelism without softening the completion contract.
2. Keep tasks current as work progresses via `tasks_update`: record measured
   inputs as they land, add blockers the moment they surface, and never let a
   task silently drift from the stage's real state.
3. Close a task only via `tasks_complete` with the gate satisfied and proof
   recorded. The completion contract is enforced by the server and is the
   point: a task cannot complete with unresolved blockers, missing proof, or
   unsatisfied gates. Do not soften it or route around it.

## Shared rules

- The shared unit-economics model (`fa-product-unit-economics-model`) is the passed
  state of the lifecycle; tasks reference the model version they were gated
  against.
- Pre-register thresholds before evidence is gathered; a threshold moved after
  data lands is a review finding, not a judgment call.
- Adversarial review (≥2 independent reviewers) and synthesis happen before
  the gate decision, and reviewer corrections stay visible in the artifact.
- A failed gate routes backward along the explicit loop-back edge to the true
  upstream owner — recorded on the task — never to the most recent owner by
  habit.
- Sibling tasks with no shared state run concurrently; the money gate is a
  hard barrier between stages. Agents editing artifacts in parallel take
  disjoint file scopes (isolated git worktrees when scopes must overlap) —
  two agents in one scope overwrite each other.
- Construction of a validated, certified bet is handed to the Gaia delivery
  roles (engineering plugin); this plugin's tasks end at that handoff.

## Proof expectations by stage

- Stages 1–4: demand signals, deflated WTP, and measured CPI attached to the
  gate task; model version advanced.
- Stages 5–6: sandbox purchase granting an entitlement + clean funnel event;
  hardening/QA evidence.
- Stage 7: the signed compliance certificate.
- Stages 8–9: soft-launch retention AND spend reads; store-CVR A/B result;
  blended CAC:LTV record.
- Stage 10: NET-of-fee revenue reconciliation and experiment records with
  rollback evidence.
