---
name: fa-product-money-gate
description: Provides the money-gate evaluation discipline for the product-discovery lifecycle - pre-registered thresholds, net-of-fee revenue math (store commission, refunds, chargebacks, fraud, tax), LTV:CAC and payback checks, a cashflow/working-capital check, deflated willingness-to-pay, and pass/fail/kill routing along loop-back edges. Use at every stage gate to decide advance, loop-back, or kill for consumer one-off / in-app-purchase products.
license: MIT
---

# Gaia Product Money Gate

## Scope and when to use

Use at every stage boundary to convert evidence into an advance / loop-back / kill
decision. Every gate is a willingness-to-PAY decision, never a free-interest one.

Use when:

- a stage has produced evidence and the coordinator must decide the gate
- measured inputs (CPI, WTP, ARPPU, retention) arrive and the model must re-run
- a threshold is at risk and a loop-back or kill must be chosen

Do not use when:

- thresholds were not pre-registered (register them first)
- the decision is a pure software-delivery gate (use the delivery roles)

## Required inputs

- the pre-registered success/kill thresholds for this gate
- the current unit-economics model version and its inputs
- the stage's measured evidence (paid signals, retention, conversion, CPI)

## Owned outputs

- a net-of-fee gate verdict: advance / loop-back / kill
- the updated model version with measured inputs replacing placeholders
- the named loop-back target on failure

## Core workflow

1. Confirm thresholds were pre-registered before this stage's evidence existed; if not, register them now and flag the goalpost-bias risk.
2. Recompute revenue NET of: store commission (15% small-business / 30% standard), refunds, chargebacks, IAP/receipt fraud, and indirect tax where not store-remitted.
3. Deflate any stated WTP (survey/conjoint) with a quantified haircut; require a money-on-the-line signal to confirm before it drives a build decision.
4. Compute net blended LTV per install (IAP component net of fee + ad component net of rev-share) and compare to CPI.
5. Require LTV:CAC ≥ 3:1 AND a financeable payback window (genre-appropriate; front-loaded for premium).
6. Run the cashflow / working-capital check: store payouts arrive 30–45 days after CAC is spent; confirm runway to break-even volume.
7. Apply the verdict: PASS → advance; FAIL on a fixable lever → loop-back; uneconomic after honest net math → kill.

## Net-of-fee math (worked)

- A $4.99 IAP nets ~$4.24 at 15% or ~$3.49 at 30%, before refunds / fraud / tax.
- Never score a gate on gross, on a single optimistic case, or on un-paid-back LTV.

## Failure recovery

| Failure mode | Recovery | Route to |
|---|---|---|
| thresholds not pre-registered | register and flag goalpost-bias | stay at gate |
| gross used instead of net | recompute net-of-fee | stay at gate |
| WTP unconfirmed by money | run a fake-door / ad test | `fa-product-validation-researcher` |
| CPI > net LTV | re-tune economy or kill | `fa-product-monetization-economist` |
| LTV>CAC but cash gap | confirm financing or descope | `fa-product-monetization-economist` |

## Anti-patterns

- do not pass on gross or optimistic economics
- do not treat stated WTP as real WTP
- do not ignore the cashflow gap on an LTV-positive title
- do not move a threshold after results land

## Handoff and downstream impact

- return the verdict, the model version, and the loop-back target to the coordinator
- keep the net-of-fee derivation visible so the decision is auditable

## Completion checklist

- the verdict is net-of-fee and threshold-based
- the model version is updated with measured inputs
- the loop-back target (or kill) is explicit

## References

- [Unit economics model](../fa-unit-economics-model/SKILL.md)
- [Product process](../fa-product-process/SKILL.md)
