---
name: fa-product-validation
description: Provides the money-on-the-line validation playbook (Stage 4) for consumer one-off / in-app-purchase products - falsifiable money hypotheses under a cost ceiling, IP / trademark / name / domain clearance, problem and solution interviews that probe real past spend, concept tests with a clickable paywall, deflated willingness-to-pay (Van Westendorp, conjoint / MaxDiff), fake-door and painted-price tests, landing-page commitment ladders, ad-creative CPI tests that replace the placeholder CAC, and community pre-validation. Use it by buying at least one behavioural money signal cheaply and re-running the gate on measured inputs. Use it when a designed money model must be falsified before anything is built, when "they said they would buy it" needs testing against behaviour, or when a measured CPI must replace an assumed CAC. It never designs the price ladder - that is `fa-product-monetization-design`.
license: MIT
---

# Gaia Product Validation

## Scope and when to use

Use this skill to put money on the line before committing to a build. Scope is
one-off + IAP/IAG monetization; B2B seat-based SaaS is out of scope and an
auto-renew model is red-lined unless an in-scope decision is recorded.

Use when:

- a designed money model needs falsification before any slice is built
- willingness-to-pay must be confirmed by behavior, not by stated intent
- the placeholder CAC must be replaced by a measured CPI

Do not use when:

- the money model is not designed yet (run monetization design first)
- a paid signal already passed and the slice can start

## Required inputs

- the designed money model, price ladder, and falsifiable hypotheses
- the current unit-economics model version and its placeholder CAC
- a stage cost ceiling and access to test channels

## Owned outputs

- a documented go/no-go with at least one money-on-the-line signal
- deflated WTP, fake-door/painted-price results, and a measured CPI
- the re-run net LTV:CAC + payback gate on real CPI

## Core workflow

1. Frame falsifiable MONEY hypotheses and set a stage cost ceiling before spending.
2. Clear IP / trademark / name / domain before investing in the identity.
3. Run problem and solution interviews (12-20 persona-matched) that probe REAL past spend; treat "I'd download" as not equal to "I'd pay".
4. Run concept tests with a clickable prototype that INCLUDES the paywall/IAP screen.
5. Research WTP (Van Westendorp n>=100-200; conjoint/MaxDiff for bundles) then DEFLATE it with a quantified haircut.
6. Run fake-door / painted-price tests and measure buy-taps against a pre-registered threshold.
7. Build a landing page with a commitment ladder (email -> pre-order -> deposit).
8. Run ad-creative CPI testing that REPLACES the placeholder CAC, then RE-RUN the net LTV:CAC + payback gate.
9. Pre-validate with community (Discord/subreddit, founder pack) and document the go/no-go.

The tracks are independent until the gate: run interviews, WTP research, fake-door/painted-price tests, the landing-page ladder, and CPI creative tests as concurrent lanes (parallel subagents where delegated) — they share no state. Only the gate re-run (step 8's LTV:CAC on measured CPI) serializes, because it needs the lanes' outputs.

## Money-on-the-line evidence

- A stated WTP number is a hypothesis; a buy-tap or deposit is evidence.
- Always deflate survey/conjoint WTP before it touches a build decision.
- The measured CPI, not the placeholder, decides the net LTV:CAC at this gate.

## Anti-patterns

- do not accept "I'd download it" as a willingness-to-pay signal
- do not feed un-deflated stated WTP into the model
- do not skip the paywall screen in the concept test
- do not exceed the stage cost ceiling chasing certainty
- do not pass the gate without re-running it on measured CPI

## Handoff and downstream impact

- hand the vertical slice the validated money model and confirmed price points
- hand the money gate the deflated WTP, paid signal, and measured CPI
- name the loop-back: failed WTP returns to ideation or monetization design

## Completion checklist

- at least one money-on-the-line signal is documented
- WTP is deflated and IP/name/domain are cleared
- fake-door buy-taps were measured against a pre-registered threshold
- the net LTV:CAC + payback gate was re-run on measured CPI

## References

- [Unit economics model](../fa-product-unit-economics-model/SKILL.md)
- [Money gate](../fa-product-money-gate/SKILL.md)
- [Product process](../fa-product-process/SKILL.md)
- [Product discovery team architecture](../../references/product-discovery-team.md)
