# Product Discovery Team — Architecture

The Gaia product plugin ships an 11-agent team that takes a CONSUMER software
product (one-off purchase or in-app / in-game purchase — never B2B seat-based
SaaS, never auto-renew without an explicit recorded in-scope decision) from idea
to a money-validated bet, through launch, and into live-ops. The team runs a
**10-stage, money-gated lifecycle** in which every stage exits only on a
willingness-to-PAY gate.

## The 10-stage money-gated lifecycle

| # | Stage | Exit condition (gate) |
|---|---|---|
| 1 | Discovery | Opportunity Thesis + unit-economics model v0 + GO / NO-GO / PIVOT |
| 2 | Ideation | ONE concept with a committed monetization hypothesis; the rest killed or parked |
| 3 | Monetization design | Net LTV:CAC ≥ 3:1 with payback + cashflow check |
| 4 | Validation | Deflated WTP + ≥1 money-on-the-line signal; measured CPI replaces placeholder CAC |
| 5 | Vertical slice / MVP | Lovable core loop + live sandbox IAP granting an entitlement + clean funnel event |
| 6 | Production & hardening | Feature/content-complete with a production-grade IAP/entitlement path |
| 7 | Compliance & certification | Signed compliance certificate (ratings, loot-box law, privacy, tax, legal docs) |
| 8 | Soft-launch | Real retention AND real spend in cheap geos; multi-condition scale gate |
| 9 | Go-to-market & UA | Repeatable acquisition engine; positive defensible blended CAC:LTV |
| 10 | Live-ops & portfolio | Steady-state NET revenue engine; sunset / maintenance / sequel decision compounding back to Stage 1 |

Gates are pre-registered before evidence lands, scored NET of the platform fee
each channel actually pays plus refunds, chargebacks, fraud, and tax, and route
failures backward along explicit loop-back edges — or kill the bet. The gate
discipline is `fa-product-money-gate`; the lifecycle contract is
`fa-product-process`.

## The 11-agent team

One coordinator, eight stage specialists, one adversarial reviewer (run as ≥2
independent instances), one synthesizer. Specialists are leaf workers: they
never call each other; all routing goes through the coordinator.

| Agent | Role | Stage(s) |
|---|---|---|
| `fa-product-coordinator` | The only coordinator: frames the brief, guards the shared model, fans out, gates, routes loop-backs | all |
| `fa-product-market-researcher` | Trend scan, demand-signal kill gate, teardowns, personas/JTBD, net-of-fee sizing | 1 |
| `fa-product-ideation-strategist` | Divergence, monetization-seeded backlog, three-lens filter, one concept | 2 |
| `fa-product-monetization-economist` | Money model, price ladder, IAP catalogue, economy balance; steward of the unit-economics model | 3 |
| `fa-product-validation-researcher` | IP clearance, real-spend interviews, deflated WTP, fake-door/CPI tests | 4 |
| `fa-product-build-architect` | Spec stack, fidelity ladder, billing/entitlement rails, hardening | 5–6 |
| `fa-product-compliance-officer` | Ratings, store/console submission, loot-box/kids/privacy law, tax, legal docs | 7 |
| `fa-product-growth-marketer` | Soft-launch reads, scale gate, ASO, paid UA, CAC:LTV control loop | 8–9 |
| `fa-product-liveops-manager` | Live-ops calendar, experiments, CRM, whale program, end-of-life + retrospective | 10 |
| `fa-product-reviewer` | Adversarial stress-test of one stage artifact; ≥2 independent instances per stage | every stage |
| `fa-product-synthesizer` | Merges specialist outputs + reviewer corrections into one artifact; adds no new analysis | every stage |

## Fan-out → adversarial review → synthesis

Every stage runs the same pattern:

1. **Fan-out** — the coordinator launches the stage's owning specialist(s) as
   concurrent Task invocations in a single message, each with the brief, the
   current model version, and the pre-registered thresholds. Long-running work
   (scans, experiments) runs as background subagents; the coordinator awaits
   only the results that gate the stage decision.
2. **Adversarial review** — ≥2 independent `fa-product-reviewer` instances,
   launched in parallel (never sequentially, never shown each other's output),
   attack the artifact for gross-instead-of-net economics, un-deflated WTP,
   moved goalposts, smuggled subscriptions, and unsupported claims, emitting
   traceable `CORRECTION from review:` notes. Reviewers never coordinate with
   each other and never rewrite the artifact.
3. **Synthesis** — `fa-product-synthesizer` merges outputs and corrections into
   ONE coherent artifact, retaining every correction visibly.
4. **Gate** — the coordinator applies `fa-product-money-gate`: PASS →
   auto-advance; FAIL on a fixable lever → loop-back to the true upstream
   owner; uneconomic after honest net math → kill.

Work is parallel-safe within a stage, never across a causal gate — the money
gate is a hard barrier. Within a stage, artifacts with no data dependency are
produced concurrently (Stage 1's trend scan, teardowns, and persona work; Stage
4's interview, fake-door, and landing-page tracks); only true dependencies
serialize, and a plan that serializes independent work is a defect. Concurrent
artifact edits get disjoint file scopes per agent — two agents in one scope
overwrite each other; overlapping scopes require isolated git worktrees merged
deliberately. Parallel branches are registered as sibling MCP tasks with
per-task gates, blockers, and proof, so they complete independently.

## The shared unit-economics model

One versioned model (`fa-product-unit-economics-model`) is the passed state of the whole
lifecycle: initialized as v0 in Discovery, advanced whenever a measured input
(CPI, WTP, ARPPU, retention) replaces an assumption, and re-run at every gate.
The monetization economist is its steward; every specialist reads and updates
it; nobody re-derives it privately. All revenue in it is NET of store
commission (at the tier each actual channel pays), refunds, chargebacks, fraud,
and tax. The schema lives at
[`../skills/fa-product-unit-economics-model/references/model-schema.md`](../skills/fa-product-unit-economics-model/references/model-schema.md).

## Handoff out of the team

A validated, built, certified bet leaves this team for software construction
via the Gaia delivery roles (`fa-engineering-solutions-architect` and the engineering
plugin's chain). This team decides WHAT earns a build and proves it makes
money; it does not run final code delivery or release gating.
