---
name: fa-product-portfolio-retrospective
description: Provides the closure and portfolio-compounding playbook (Stage 10) for consumer one-off / in-app-purchase products - end-of-life triggers and review cadence, a three-way continue / maintenance / sunset evaluation plus sequel and DLC options, a humane sunset that honors recent one-off purchases, a retrospective that harvests reusable capabilities and calibrates the unit-economics model, documenting realized priors, and carrying the audience, CRM and payer base forward into a new Discovery cycle. Use it by deciding the end-state on net LTV against operating cost and on realized-versus-predicted calibration, then compounding the learnings back to Stage 1. Use it when a live title trips an end-of-life trigger or its review cadence comes due, when a continue / maintenance / sunset call must be made, or when a title is being shut down and its payer base and tooling must survive it. It never runs day-to-day operations of a healthy title - that is `fa-product-liveops`.
license: MIT
---

# Gaia Product Portfolio Retrospective

## Scope and when to use

Stage 10 (closure) of the money-gated lifecycle: decide a live title's end-state
and compound its learnings back to Stage 1. Scope is one-off + IAP/IAG; auto-renew
passes are red-lined unless an explicit in-scope decision is recorded. Decisions
run on NET-LTV vs operating cost and realized-vs-predicted model calibration.

Use when:

- a live title trips an end-of-life trigger or its review cadence comes due
- a continue / maintenance / sunset (or sequel/DLC) decision must be made
- a sunset must be run humanely and the payer base/tooling harvested forward

Do not use when:

- the title is healthy and only needs day-to-day operations (use `fa-product-liveops`)
- a new bet is being framed from scratch with no prior priors (use `fa-product-discovery`, S1)

## Required inputs

- the title's NET-LTV-vs-operating-cost trend and content-runway economics
- the realized cohort data (retention, ARPPU, whale, churn-cause) and model version
- platform-obsolescence signals and the opportunity-cost picture across the portfolio

## Owned outputs

- the continue / maintenance / sunset (or sequel/DLC) decision with rationale
- the humane sunset plan (stop-selling, honor/refund, archive) when chosen
- harvested capabilities + documented priors + a go/no-go on a follow-on cycle

## Core workflow

1. Watch end-of-life triggers and review cadence (standing NET-LTV-vs-operating-cost trend, content-runway economics, platform obsolescence, opportunity cost).
2. Run the three-way evaluation: continue/invest, maintenance-mode, or sunset, plus a sequel/DLC option.
3. On the sequel/DLC path, scope the follow-on, design migration/loyalty rewards, and build a pre-launch CRM bridge that converts the base into day-one buyers.
4. On maintenance-mode, templatize evergreen events and harvest the long tail at minimal cost.
5. On sunset, run it humanely: stop selling IAP ahead of shutdown; honor/refund recent one-off purchases per platform + consumer law; communicate timelines; archive IP/assets.
6. Run the portfolio retrospective + capability harvest (event templates, live-ops tooling, economy models, CRM playbooks, ASO/creative database, the versioned unit-economics model + realized-vs-predicted calibration) and document realized LTV/retention/whale/churn-cause PRIORS.
7. Transfer audience/CRM/payer-base forward, run the go/no-go on a follow-on, and if GO hand priors + payer base + tooling into a NEW Discovery cycle (Stage 1) — the portfolio compounding loop.

## Stage focus — close the loop, never just close the title

The point of closure is compounding: a sunset still produces calibrated priors, a
harvested toolchain, and a transferable payer base. Decisions are made on NET-LTV
vs operating cost, and the realized-vs-predicted calibration is fed back so the
next cycle's unit-economics model starts smarter than this one did.

## Anti-patterns

- do not keep a title live past the point where NET-LTV covers operating cost
- do not sunset while still selling IAP or without honoring/refunding recent one-off purchases
- do not discard event templates, CRM playbooks, or the calibrated model on shutdown
- do not start a sequel without bridging the existing payer base into day-one buyers
- do not skip documenting realized priors before handing off to Stage 1

## Handoff and downstream impact

- return the end-state decision, sunset plan, and harvested priors to the coordinator
- on a GO follow-on, hand priors + payer base + tooling to `fa-product-market-researcher` (S1)
- feed the realized-vs-predicted calibration into the next unit-economics model version

## Completion checklist

- the continue/maintenance/sunset (or sequel/DLC) decision is explicit and NET-justified
- any sunset honored/refunded recent one-off purchases and archived IP/assets
- realized priors documented and payer base/tooling transferred into the next cycle

## References

- [Unit economics model](../fa-product-unit-economics-model/SKILL.md)
- [Money gate](../fa-product-money-gate/SKILL.md)
- [Product process](../fa-product-process/SKILL.md)
- [Product discovery team architecture](../../references/product-discovery-team.md)
