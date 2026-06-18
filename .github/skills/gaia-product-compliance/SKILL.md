---
name: gaia-product-compliance
description: Provides the Stage-7 compliance, ratings, legal and certification playbook for consumer one-off / in-app-purchase products - IARC age ratings and content descriptors, store/console submission and resubmission loops, loot-box/gacha/gambling-law handling (odds disclosure, disable-where-banned, minor spend limits), kids/privacy/consent (COPPA, GDPR-K, ATT, GDPR/CCPA), indirect tax, IP/asset licensing carry-through, and the EULA/ToS/Privacy-Policy set. Use to clear certification before soft-launch and to loop rejections back to production hardening. Money-gated and net-of-fee aware (store-remitted vs developer-remitted tax changes net revenue).
license: MIT
---

# Gaia Product Compliance

## Scope and when to use

Stage 7 of the money-gated lifecycle: convert a feature/content-complete build
into a legally shippable, certified product. Scope is one-off + IAP/IAG consumer
software; auto-renew passes are red-lined unless an explicit in-scope decision is
recorded. Tax outcomes are net-of-fee inputs the unit-economics model must absorb.

Use when:

- a hardened build (S6) needs ratings, certification, and legal review before soft-launch
- loot-box/gacha odds, minor spend limits, or a banned-geo disable must be decided
- kids/privacy/consent (COPPA, GDPR-K, ATT) or indirect-tax handling must be set

Do not use when:

- the build is not yet content/perf-complete (return to `gaia-product-build-architect`)
- the request is pure software delivery with no legal/cert surface (use the delivery roles)

## Required inputs

- the feature/content-complete build and its full IAP/IAG catalogue
- target store/console list, geos, and minimum supported audience age
- current unit-economics model version (for tax-remittance impact on net revenue)

## Owned outputs

- a signed compliance certificate (ratings, legal, privacy, tax) gating soft-launch
- the per-geo loot-box decision (disclose / disable / age-gate + spend cap)
- the EULA, ToS, Privacy Policy and consent-state wiring proof

## Core workflow

1. Run the IARC questionnaire honestly; map to ESRB/PEGI/USK/CERO and attach descriptors ("in-game purchases", "includes random items") truthfully — under-declaring forces resubmission.
2. Prepare store/console submission packs and budget review/resubmission loops (Apple guidelines incl. 3.x IAP and 4.5.4 odds disclosure, Play, Steamworks, console TRC/TCR/lotcheck).
3. Decide loot-box/gacha posture per geo: disclose probabilities where required (China/Korea/Apple 4.5.4), DISABLE where banned (Belgium/Netherlands), and apply age-gate + minor spend limits.
4. Clear kids/privacy/consent: COPPA, GDPR-K, Apple Kids Category, Google Families; wire ATT/IDFA + GDPR/CCPA consent and prove SDKs respect consent state.
5. Settle indirect tax (VAT/GST) as store-remitted vs developer-remitted and push the resulting net-revenue delta into the unit-economics model.
6. Confirm IP/trademark/asset/font/music licensing carry-through, then publish the EULA, ToS, and Privacy Policy.
7. Issue the signed compliance certificate; on any rejection, loop back to production hardening with the specific defect.

## Stage focus — certification as a gate, not a formality

A rejection is a money event: it delays the soft-launch payback clock and can
force a content change. Treat odds disclosure, minor spend caps, and consent-state
honoring as ship-blocking. The exit gate is the signed certificate; a banned-geo
loot-box that is merely "documented" but still purchasable is a hard fail.

## Anti-patterns

- do not under-declare random-item or IAP descriptors to dodge a higher rating
- do not ship loot boxes enabled in a banned geo or without required odds disclosure
- do not fire monetization/attribution SDKs before consent state is resolved
- do not treat developer-remitted tax as free — it lowers net revenue
- do not relabel an auto-renew pass as a one-off to escape subscription rules

## Handoff and downstream impact

- hand the signed certificate, per-geo loot-box matrix, and tax-remittance note to the coordinator
- push the net-revenue tax delta into the unit-economics model before S8 gating
- route any rejection back to `gaia-product-build-architect` (S6) with the exact defect

## Completion checklist

- ratings + descriptors filed and store/console submissions accepted
- loot-box posture (disclose/disable/age-gate+cap) recorded per geo
- consent wiring proven and EULA/ToS/Privacy Policy published; certificate signed

## References

- [Unit economics model](../gaia-unit-economics-model/SKILL.md)
- [Money gate](../gaia-product-money-gate/SKILL.md)
- [Product process](../gaia-product-process/SKILL.md)
- [Product discovery team architecture](../../../docs/architecture/product-discovery-team.md)
