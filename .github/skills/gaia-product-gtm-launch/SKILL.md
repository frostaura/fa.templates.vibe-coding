---
name: gaia-product-gtm-launch
description: Provides the Stage-9 go-to-market, launch and user-acquisition playbook for consumer one-off / in-app-purchase products - lock pricing/offer ladder before traffic, ASO/store-listing optimization with a store-page CVR gate, earned/owned demand priming, platform-featuring pitches, creator seeding, launch sequencing, paid UA only after organic CVR validates, a creative-testing engine, virality/referral with fraud detection, support/refund ops, and a blended-CAC-vs-LTV scale-decision control loop. Use to build a repeatable acquisition engine. Money-gated and net-of-fee aware (require positive defensible blended CAC:LTV with payback).
license: MIT
---

# Gaia Product GTM & Launch

## Scope and when to use

Stage 9 of the money-gated lifecycle: turn a scale-ready title into a repeatable,
defensible acquisition engine. Scope is one-off + IAP/IAG; auto-renew passes are
red-lined unless an explicit in-scope decision is recorded. The exit gate is a
positive, fraud-filtered blended CAC:LTV at scale with a financeable payback.

Use when:

- a scale-ready package (S8) needs ASO, featuring, creators, and paid-UA setup
- a store-page CVR gate or blended-CAC-vs-LTV scale decision must be applied
- a creative-testing engine or referral/virality loop must be stood up

Do not use when:

- the title has not cleared the S8 scale gate (return to `gaia-product-growth-marketer`, S8)
- the work is steady-state live-ops revenue, not acquisition (use `gaia-product-liveops`)

## Required inputs

- the scale-ready package with locked launch pricing and offer ladder
- the current unit-economics model version with realized pLTV by source
- store assets, attribution (SKAN/AdAttributionKit) config, and UA budget

## Owned outputs

- an optimized store listing that clears the CVR gate, plus a featuring/creator plan
- a paid-UA + creative-testing engine with a fraud-filtered cohort dashboard
- the blended-CAC-vs-LTV scale decision with loop-backs to creative/positioning

## Core workflow

1. Lock monetization, launch pricing, and the offer ladder BEFORE traffic so CVR and CAC are read against a fixed target.
2. Optimize ASO/store listing (iOS title/subtitle/100-char keywords; Play descriptions; OCR-readable screenshot text; 8–9 screenshots, first two = core loop + monetization fantasy; preview video; ratings/review engine; localization) and run store-page A/B as a CVR GATE that caps downstream UA and loops back to creative/positioning on failure.
3. Prime earned & owned demand (Discord, Steam wishlists + Coming Soon, press/embargo, email list, first-purchase narrative) and pitch platform featuring (Apple 3+ weeks out; Play; Steam algorithmic; never pay for editorial).
4. Seed creators in 3 waves with referral codes paid on in-game purchases, then sequence launch (Next Fest demo, staged playtests, 24–48h velocity spike, launch starter pack, waved global).
5. Stand up paid UA ONLY after organic CVR validates (Meta/TikTok/Google/Apple Search Ads by role; value-based/event bidding toward payers; SKAN/AdAttributionKit correction).
6. Run the creative-testing engine (10–20 concepts/month, first-3-seconds, formats, fatigue cadence, performance DB) and a virality/referral loop (K-factor, double-sided rewards, fraud detection) with support/refund ops + UA-install-fraud filtering.
7. Apply the blended-CAC-vs-LTV scale-decision control loop (fraud-filtered, payback by genre, cohort dashboard, incrementality/geo-holdout) → exit on positive defensible blended CAC:LTV at scale.

## Stage focus — organic CVR gates paid spend

Paid UA is never switched on against an unvalidated store page: the CVR gate caps
spend and routes failures back to creative/positioning. Every CAC read is
fraud-filtered and net-of-fee, and scale is justified by incrementality (geo-holdout),
not last-click vanity installs.

## Anti-patterns

- do not buy paid traffic before organic store-page CVR validates
- do not pay for editorial featuring or buy installs that fail fraud filters
- do not read blended CAC on gross LTV or on last-click attribution alone
- do not change launch pricing after traffic is live
- do not scale a channel without an incrementality / geo-holdout check

## Handoff and downstream impact

- hand the validated acquisition engine, CVR-passing listing, and cohort dashboard to the coordinator
- on store-listing CVR failure, loop back to creative/positioning (S9) and possibly reposition the concept
- hand the scaled, instrumented title to `gaia-product-liveops-manager` (S10)

## Completion checklist

- store page clears the CVR gate; featuring/creator plan executed
- paid UA live only post-CVR with SKAN/AdAttributionKit correction
- positive defensible blended CAC:LTV proven with fraud filtering + incrementality

## References

- [Unit economics model](../gaia-unit-economics-model/SKILL.md)
- [Money gate](../gaia-product-money-gate/SKILL.md)
- [Product process](../gaia-product-process/SKILL.md)
- [Product discovery team architecture](../../../docs/architecture/product-discovery-team.md)
