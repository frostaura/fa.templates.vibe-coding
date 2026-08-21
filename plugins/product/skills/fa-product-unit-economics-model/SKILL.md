---
name: fa-product-unit-economics-model
description: Provides the single shared, versioned unit-economics model for consumer one-off / in-app-purchase products, created in Discovery and re-run at every gate - inputs (CPI, install-to-payer conversion, ARPPU, repeat purchase, ad ARPDAU), the gross-to-net stack (store commission, refunds, chargebacks, fraud, tax), LTV by segment, the payback window, and a cashflow / working-capital check. Use it by keeping ONE canonical model that every stage reads and updates, so no stage re-derives economics from scratch. Use it when Discovery must initialize model v0 with every input flagged as an assumption, when a specialist returns measured CPI, willingness-to-pay, ARPPU or retention to fold in, or when a gate needs the current net-of-fee LTV:CAC, payback and cashflow read. It never makes the advance-or-kill call itself - that is `fa-product-money-gate` - and its math does not fit B2B seat-based SaaS.
license: MIT
---

# Gaia Unit Economics Model

## Scope and when to use

Use to maintain ONE canonical, versioned model that every stage reads and updates.
No stage re-derives economics from scratch. Premium LTV is realized at install
(finite, no tail); IAP LTV is back-loaded and whale-driven.

Use when:

- Discovery must initialize model v0 (all inputs flagged as assumptions)
- a specialist returns measured inputs (CPI, WTP, ARPPU, retention) to fold in
- a gate needs the current net-of-fee LTV:CAC, payback, and cashflow read

Do not use when:

- the product is B2B SaaS (different LTV/churn math)

## Required inputs

- acquisition: CPI per source/geo, organic share, blended CAC
- conversion: install→payer rate, repeat-purchase rate, time-to-first-purchase
- value: ARPPU (lifetime/per-cohort, NOT monthly), ad ARPDAU, segment mix
- haircuts: store commission (channel-dependent; 15/30% base tiers), refund/chargeback %, fraud %, indirect tax %
- timing: retention curve (D1/D7/D30+), paying lifetime, payout lag

## Owned outputs

- net blended LTV per install (IAP net-of-fee + ad net-of-rev-share)
- LTV:CAC ratio and payback window
- cashflow gap to break-even and required runway
- a version stamp (v0..vN) and a changelog of which inputs became measured

## Core workflow

1. Initialize v0 with assumptions; label every input assumption-vs-measured.
2. Compute gross revenue per payer and per install from conversion × ARPPU.
3. Apply the gross-to-net stack: commission, refunds, chargebacks, fraud, tax.
4. Add the ad component (eCPM × fill × sessions, net of network rev-share) for hybrid titles.
5. Compute net LTV per install, LTV:CAC, and payback window by source.
6. Run conservative/base/optimistic cases and sensitivity on install volume and conversion × ARPPU.
7. Run the cashflow check: payout lag vs CAC timing; compute the cash gap to break-even.
8. Re-version whenever a measured input replaces an assumption; keep the changelog.

## Segments

- non-payers (monetize via ads), minnows, dolphins, whales (top decile drives most IAP).
- treat whale concentration as a fragility risk, not a static fact.

## Anti-patterns

- do not quote gross revenue at a gate
- do not use monthly ARPPU or MRR framing (that is subscription math)
- do not ignore the payout-lag cash gap
- do not freeze assumptions as if measured

## Handoff and downstream impact

- give the money gate the net-of-fee LTV:CAC, payback, and cash gap
- give specialists the current version and which inputs still need measuring

## Completion checklist

- every input is labeled assumption or measured
- net-of-fee LTV:CAC, payback, and cash gap are current
- the version and changelog are updated

## References

- [Model schema](references/model-schema.md)
- [Money gate](../fa-product-money-gate/SKILL.md)
