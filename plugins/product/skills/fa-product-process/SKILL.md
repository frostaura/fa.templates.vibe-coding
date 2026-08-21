---
name: fa-product-process
description: Provides the money-gated lifecycle and coordination playbook that takes a consumer one-off / in-app-purchase product from an idea to a validated bet and on through launch and live-ops. Use it by classifying the product request, running each of the 10 stages as fan-out -> adversarial review -> synthesis against one shared unit-economics model, and applying net-of-fee money gates with explicit loop-back edges. Use it when a consumer-product goal needs framing, discovery, monetization design, demand validation, soft-launch or live-ops coordination, when a stage needs review and a gate before it may advance, or when a gate has failed and the work must loop back to the right upstream stage. It never covers B2B seat-based SaaS, and it never runs software construction, testing or release gating - a validated bet is handed to the engineering plugin.
license: MIT
---

# Gaia Product Process

## Scope and when to use

Use this skill to route a consumer-product bet through Gaia's product-discovery
team. Scope is one-off + IAP/IAG monetization; B2B seat-based SaaS is out of
scope and auto-renewing passes are red-lined unless an explicit in-scope decision
is recorded.

Use when:

- a consumer-product goal needs framing, discovery, or monetization design
- a stage needs fan-out, review, and a money gate before advancing
- a gate fails and the work must loop back to the right upstream stage

Do not use when:

- the product is B2B seat-based SaaS
- the bet is already validated and only needs software construction (use the delivery roles)

## Required inputs

- product goal, constraints, monetization lane, and success bar
- the current unit-economics model version (or authority to initialize v0)
- prior-title priors when this is a sequel/follow-on (payer base, CRM, tooling)

## Owned outputs

- the run brief and per-gate pre-registered thresholds
- per-stage synthesized artifacts with reviewer corrections retained
- the Opportunity Thesis and the go / loop-back / kill trail

## The money-gated lifecycle

Each stage exits only on a willingness-to-PAY gate (see `fa-product-money-gate`).

1. **Discovery** — one buildable, monetizable bet; thesis + model v0.
2. **Ideation** — one concept + committed monetization hypothesis; others killed/parked.
3. **Monetization design** — economy, SKU ladder, ad stack; net LTV>CAC ≥ 3:1 + payback + cashflow.
4. **Validation** — money-on-the-line; deflated WTP + ≥1 paid signal; measured CPI replaces placeholder.
5. **Vertical slice / MVP** — lovable core loop + live sandbox IAP + instrumented funnel.
6. **Production & hardening** — content, device/perf QA, anti-cheat, localization.
7. **Compliance & certification** — ratings, loot-box/gacha law, privacy/ATT, tax, EULA.
8. **Soft-launch** — real money + retention in cheap geos; multi-condition scale gate.
9. **Go-to-market & UA** — repeatable creative+channel engine; positive blended CAC:LTV.
10. **Live-ops & portfolio** — the revenue engine; sunset/maintenance/sequel decision compounding to stage 1.

## Coordination pattern

- The coordinator is the only coordinator; specialists never call each other.
- Per stage: fan out the owning specialist(s) in parallel → run ≥2 independent reviewers → synthesize.
- Fan-out is literal concurrency: independent specialists — and the ≥2 reviewers — launch as multiple Task invocations in a single message. A run that serializes parallel-safe work is a defect.
- Await only what gates the next decision; long stage work (scans, experiments, soft-launch reads) runs as background subagents while independent branches progress.
- Reviewers emit traceable `CORRECTION from review:` notes that stay in the artifact.
- One shared, versioned unit-economics model is the passed state; specialists update it, never re-derive it.

## Parallel execution shape

- Within a stage, artifacts with no data dependency are produced concurrently (e.g. S1 trend scan, teardowns, and persona/JTBD work; S4 interview, fake-door, and landing-page tracks). Model reads are free; model WRITES serialize through its steward.
- The money gate is a hard barrier: no work downstream of a gate starts before it passes. Across bets, stages may progress concurrently when they do not contend on the same model.
- Concurrent artifact edits require disjoint file scopes per agent; overlapping scopes need isolated git worktrees merged deliberately. Two agents in one scope overwrite each other.
- Register parallel branches as sibling MCP tasks (`tasks_create`); gates, blockers, and proof are per-task, so branches complete independently without softening the completion contract.

## Gate and loop-back rules

- Pre-register success/kill thresholds BEFORE a stage's evidence is gathered.
- Quote all revenue net-of-fee (commission + refunds + fraud + tax); require LTV:CAC ≥ 3:1, payback, and a cashflow check.
- PASS → auto-advance. FAIL → route to the loop-back target. Uneconomic → kill.
- Route failures to the true upstream owner (the loop-back edge), not to the most recent owner by habit.

## Anti-patterns

- do not advance on gross or optimistic economics
- do not skip review or strip reviewer corrections
- do not run stages in parallel across a causal gate
- do not serialize independent branches; name what is parallel and what truly depends
- do not move thresholds after data lands
- do not relabel a subscription as a repeated one-off

## Handoff and downstream impact

- give each specialist the brief, the current model version, and the stage thresholds
- give the synthesizer the specialist outputs plus reviewer corrections
- hand a validated, built bet to the Gaia delivery roles for construction

## Completion checklist

- the stage artifact, gate result, and next owner are explicit
- the model version and thresholds used are attached
- loop-back targets are named when a gate fails

## References

- [Product discovery team architecture](../../references/product-discovery-team.md)
- [Unit economics model](../fa-product-unit-economics-model/SKILL.md)
- [Money gate](../fa-product-money-gate/SKILL.md)
- [Product delivery policy](../../references/delivery-policy.md)
- [Product ownership and conventions](../../references/ownership-and-conventions.md)
