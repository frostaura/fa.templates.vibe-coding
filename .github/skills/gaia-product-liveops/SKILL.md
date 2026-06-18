---
name: gaia-product-liveops
description: Provides the Stage-10 live-ops operations playbook for consumer one-off / in-app-purchase products - a live-ops data/economy-health/experimentation foundation, a recurring live-ops calendar with safe rollback, lifecycle CRM/push/in-app, continuous one-variable monetization and store/price experiments, spend-spectrum segmentation and personalized offers, a guardrailed whale/high-value program, community/crisis ops, churn analysis and resurrection, and platform-policy/SDK monitoring. Use to run the steady-state revenue engine. Money-gated and net-of-fee aware (reconcile NET-of-fee revenue, guard retention/refunds, no auto-renew offers).
license: MIT
---

# Gaia Product Live-Ops

## Scope and when to use

Stage 10 (operations) of the money-gated lifecycle: run the live title as a
steady-state revenue engine. Scope is one-off + IAP/IAG with explicit scarcity;
auto-renew/club offers are red-lined unless an explicit in-scope decision is
recorded. All revenue is reconciled net-of-fee against the MMP.

Use when:

- a launched title (S9) needs a live-ops calendar, CRM, and ongoing experiments
- segmentation, a whale program, or churn/resurrection work must be run
- a monetization-pressure or platform-policy signal must be diagnosed and looped back

Do not use when:

- the title is not yet launched/scaled (use `gaia-product-gtm-launch`, S9)
- the decision is end-of-life/sunset or portfolio harvest (use `gaia-product-portfolio-retrospective`)

## Required inputs

- the live title with reliable telemetry and MMP reconciliation
- the current unit-economics model version and canonical KPI tree
- segment definitions, remote-config buckets, and guardrail/alert thresholds

## Owned outputs

- the recurring live-ops calendar with staged rollout + kill-switch proof
- lifecycle CRM, segmentation, and personalized-offer programs (NET revenue-positive, retention-safe)
- churn/resurrection programs and platform-policy loop-backs to production/compliance

## Core workflow

1. Stand up the live-ops data/economy-health/experimentation foundation (event taxonomy fires reliably; NET-of-fee revenue reconciled vs MMP; economy as a closed system with "days of runway" per currency; canonical KPI tree; deterministic bucketing; cohorting; guardrail alerting).
2. Run a recurring live-ops calendar (overlapping layers: daily login, weekly rotating archetypes, monthly tentpoles, tactical sales/LTOs; weekend-heavy; templatized events; a sale/LTO ladder with explicit scarcity and NO auto-renew/club offers; trigger-based offers; change-safety/rollback staged % + kill-switch; weekly retro vs holdout) on a content-cadence + runway dashboard.
3. Run lifecycle CRM/push/in-app (behavior-triggered, segment-aware, permission-prime after a win, churn-probability/next-best-action, A/B vs holdout).
4. Run continuous monetization + store/price experiments (one-variable, NET revenue-per-impression, guard retention/refunds).
5. Segment the spend spectrum and personalize offers (non-payer → minnow → dolphin → whale; ML micro-cohorts; fairness/ethics review).
6. Run a whale/high-value program with FRAGILITY guardrails (VIP, white-glove, protect experience, spend-harm monitoring, diversification).
7. Run community/ratings/reviews + crisis PR and churn analysis + resurrection (predict; segment by cause; pre-churn intervention; win-back by lapse window), looping monetization-pressure causes back to `gaia-product-monetization-economist` (S3) and platform SDK/policy changes back to production/compliance.

## Stage focus — every live change is reversible and net-measured

The calendar is run like deployments: staged %, kill-switch, weekly retro vs a
holdout. Monetization and price tests move one variable and are scored on NET
revenue-per-impression while guarding retention and refunds. Whale revenue is
treated as fragile — protect the experience and monitor spend-harm.

## Anti-patterns

- do not ship a calendar event or price test without staged rollout + kill-switch
- do not read live revenue gross or unreconciled against the MMP
- do not introduce an auto-renew/club offer under the "one-off" banner
- do not over-pressure whales or skip the spend-harm / fairness review
- do not treat a churn spike from monetization pressure as a CRM problem

## Handoff and downstream impact

- report calendar performance, NET revenue, and economy runway to the coordinator
- loop monetization-pressure churn causes back to `gaia-product-monetization-economist` (S3)
- loop platform SDK/policy deprecations to `gaia-product-build-architect` (S6) / `gaia-product-compliance-officer` (S7)

## Completion checklist

- calendar events run with staged rollout, kill-switch, and holdout retros
- monetization/price experiments are one-variable, NET-scored, retention-guarded
- churn/whale/platform loop-backs are routed to their true owners

## References

- [Unit economics model](../gaia-unit-economics-model/SKILL.md)
- [Money gate](../gaia-product-money-gate/SKILL.md)
- [Product process](../gaia-product-process/SKILL.md)
- [Product discovery team architecture](../../../docs/architecture/product-discovery-team.md)
