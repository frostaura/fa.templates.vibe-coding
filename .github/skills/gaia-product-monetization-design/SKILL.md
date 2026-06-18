---
name: gaia-product-monetization-design
description: Provides Stage 3 monetization-design guidance for consumer one-off / in-app-purchase products - selecting a primary model (premium / freemium-IAP / free-ads / hybrid) against retention and platform, a price ladder with psychology, an IAP catalogue, ad-revenue economics, a faucet/sink economy balanced via cohort simulation, a conversion funnel with lifetime ARPPU, validation of net-of-fee unit economics with a cashflow check, and translation into product requirements. Use to design the money model and prove net LTV:CAC >= 3:1 with payback before requirements ship.
license: MIT
---

# Gaia Product Monetization Design

## Scope and when to use

Use this skill to design the money model for the selected concept and prove it
nets out. Scope is one-off + IAP/IAG monetization; B2B seat-based SaaS is out of
scope and an auto-renew pass is red-lined unless an in-scope decision is recorded.

Use when:

- a selected concept needs a primary model, price ladder, and economy
- unit economics must be validated net-of-fee with a cashflow check
- the money model must become concrete product requirements

Do not use when:

- no concept is selected yet (run ideation first)
- the model is validated and only needs money-on-the-line testing

## Required inputs

- the selected concept brief and its monetization hypothesis
- the current unit-economics model version and segment mix
- platform billing constraints and target-geo price/eCPM data

## Owned outputs

- the chosen primary model, price ladder, IAP catalogue, and ad stack
- a cohort-simulated economy with faucet:sink balance and gate placement
- a net-of-fee economics validation and the monetization product requirements

## Core workflow

1. Select the primary model (premium / freemium-IAP / free-ads / hybrid) justified against retention, audience, and platform; honor red lines (no exploitative loot boxes targeting minors, no loop-breaking pay-to-win, NO auto-renew pass unless recorded).
2. Build the price ladder with psychology: charm pricing, a 4-6 rung IAP ladder, decoy/best-value anchoring, a first-time starter pack, and localized NET-by-geo pricing.
3. Define the IAP catalogue (consumables, non-consumables, currencies, bundles; soft vs hard currency; a season pass treated as a one-off).
4. Model ad-revenue economics: mediation, eCPM by geo/format, ad ARPDAU, and rewarded feeds back into IAP.
5. Balance the economy faucets and sinks; simulate a cohort at 30/60/90d, set the faucet:sink ratio, place the monetization gate, and guard against inflation.
6. Define the conversion funnel and metrics (install->activated->retained->payer->repeat->whale) using LIFETIME ARPPU (not monthly), ARPDAU, and segments.
7. Validate unit economics: net-of-fee at 15/30 plus refunds/chargebacks/fraud/tax, run a CASHFLOW/working-capital check, require LTV:CAC >= 3:1 with payback, and stress whale fragility.
8. Translate to product requirements: shop surface, contextual offers, pass system, ads integration, remote-config economy, A/B plan, and analytics spec.

## Economy balancing

- Simulate a real cohort over 30/60/90 days; a static spreadsheet hides inflation.
- Tune faucet:sink so the soft economy stays scarce enough for the gate to matter.
- Place the monetization gate where it relieves friction, never where it breaks the loop.

## Anti-patterns

- do not pick a model that fights the genre's retention curve
- do not price in a single currency or quote gross IAP take
- do not report monthly ARPPU or MRR framing (that is subscription math)
- do not balance the economy without a multi-day cohort simulation
- do not pass economics without the cashflow/working-capital check

## Handoff and downstream impact

- hand validation the falsifiable money model and the price ladder to test
- hand the money gate the net-of-fee LTV:CAC, payback, and cash gap
- name the loop-back: a failed validation re-tunes the economy here

## Completion checklist

- the primary model is justified and red lines are honored
- the price ladder and IAP catalogue are localized and net-by-geo
- the economy is cohort-simulated with faucet:sink and gate placement set
- net-of-fee LTV:CAC >= 3:1, payback, and cashflow all pass
- the monetization product requirements are explicit and instrumentable

## References

- [Unit economics model](../gaia-unit-economics-model/SKILL.md)
- [Money gate](../gaia-product-money-gate/SKILL.md)
- [Product process](../gaia-product-process/SKILL.md)
- [Product discovery team architecture](../../../docs/architecture/product-discovery-team.md)
