---
name: fa-product-soft-launch
description: Provides the Stage-8 build-iteration and soft-launch playbook for consumer one-off / in-app-purchase products - always-green trunk with feature-flags, a versioned telemetry/event taxonomy wired before external testers, closed/open beta hardening, soft-launch in cheap representative geos with attribution, reading D1/D7/D30 + conversion + ARPPU/ARPDAU by cohort/source, modeling pLTV vs CPI, one-variable A/B tuning, iterate-or-kill discipline, and clearing the multi-condition scale gate. Use to prove real-money retention before paid scale. Money-gated and net-of-fee aware (require net LTV > CPI + payback).
license: MIT
---

# Gaia Product Soft-Launch

## Scope and when to use

Stage 8 of the money-gated lifecycle: prove, with real money and retention in
cheap representative geos, that the title can scale. Scope is one-off + IAP/IAG;
auto-renew passes are red-lined unless an explicit in-scope decision is recorded.
The exit gate is a multi-condition scale gate on net-of-fee economics.

Use when:

- a certified build (S7) needs telemetry, beta hardening, and a controlled soft-launch
- D1/D7/D30, conversion, ARPPU/ARPDAU must be read by cohort/source against pre-set thresholds
- iterate-or-kill discipline or the scale gate must be applied before paid UA

Do not use when:

- the build is not certified (return to `fa-product-compliance-officer`, S7)
- the title is already scale-ready and only needs the GTM engine (use `fa-product-gtm-launch`)

## Required inputs

- the certified build with the full purchase pipeline (buy/restore/refund)
- pre-registered scale-gate thresholds and the current unit-economics model version
- soft-launch geo list, UA budget, and attribution (MMP) configuration

## Owned outputs

- the versioned, CI-validated event taxonomy and reconciled telemetry
- cohort/source KPI reads (D1/D7/D30, conversion, ARPPU, ARPDAU) and pLTV-vs-CPI model
- a scale-ready package or an iterate/kill decision with the loop-back target

## Core workflow

1. Run an agile cadence on an always-green trunk with feature-flags/remote-config so any cohort can be toggled safely.
2. Ship the versioned event taxonomy BEFORE external testers (snake_case object_action; funnel + currency ledger + monetization events; schema validation in CI; reconcile client events vs server receipts; wire an MMP for attribution).
3. Run closed beta for crash-hardening, FTUE legibility, and the full purchase pipeline incl. restore/refund.
4. Run open/wider beta for FTUE funnel significance; treat tutorial completion <75% as a pivot trigger.
5. Soft-launch in cheap representative geos with controlled, attributed UA; read D1/D7/D30 + conversion + lifetime ARPPU + ARPDAU by cohort/source and model pLTV vs CPI — require net LTV > CPI + payback.
6. Tune economy + onboarding, then prove each change with ONE-VARIABLE A/B; loop validated prices back to `fa-product-monetization-economist` (S3).
7. Apply iterate-or-kill discipline (bright-line triggers, bounded cycles, diagnose retention vs monetization vs UA), then clear the multi-condition SCALE GATE (reproducible across cohorts + ≥2 acquisition sources) → scale-ready package.

## Stage focus — instrument first, then trust the read

Telemetry precedes testers so no cohort is wasted on an un-measured build.
Reconcile client events against server receipts before believing any conversion
or ARPPU number, and read every metric net-of-fee. A KPI that is not reproducible
across cohorts and ≥2 sources is not a pass — it is noise.

## Anti-patterns

- do not let external testers touch a build before the event taxonomy and reconciliation exist
- do not multi-variable a tuning change and claim the lift
- do not read ARPPU/LTV gross or from a single lucky cohort
- do not extend iterate cycles past the pre-registered bound to avoid a kill
- do not pass the scale gate on one acquisition source

## Handoff and downstream impact

- hand the scale-ready package, cohort KPI reads, and calibrated pLTV to the coordinator
- loop validated prices/economy back to `fa-product-monetization-economist` (S3)
- on CPI > net LTV, route to S3 + unit-economics model update; on high-retention/zero-conversion, route to S2

## Completion checklist

- event taxonomy versioned, CI-validated, and reconciled vs receipts
- net LTV > CPI + payback proven across cohorts and ≥2 sources
- scale gate result and any loop-back/kill target are explicit

## References

- [Unit economics model](../fa-unit-economics-model/SKILL.md)
- [Money gate](../fa-product-money-gate/SKILL.md)
- [Product process](../fa-product-process/SKILL.md)
- [Product discovery team architecture](../../../docs/architecture/product-discovery-team.md)
