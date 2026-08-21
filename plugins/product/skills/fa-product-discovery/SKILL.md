---
name: fa-product-discovery
description: Provides the market-discovery playbook (Stage 1) for consumer one-off / in-app-purchase products - trend scanning across market-intel tools (Sensor Tower, AppMagic, Appfigures, AppTweak), store charts, Steam tags/wishlists and social; demand-signal harvesting as the cheapest kill gate; competitor teardowns; personas, jobs-to-be-done and willingness-to-pay by segment; and bottoms-up TAM/SAM/SOM sized net of platform fee. Use it by gathering evidence for one buildable, monetizable bet and exiting with an Opportunity Thesis, unit-economics model v0 and a GO / NO-GO / PIVOT call. Use it when a consumer-product goal needs trend, demand and competitor evidence before any concept work, when an opportunity must be sized net-of-fee, or when someone asks what consumer product to build and whether anyone would actually pay for it. It never picks the final concept - that is `fa-product-ideation` - and it does not cover B2B seat-based SaaS.
license: MIT
---

# Gaia Product Discovery

## Scope and when to use

Use this skill to scan the market and surface ONE buildable, monetizable
consumer bet. Scope is one-off + IAP/IAG monetization; B2B seat-based SaaS is out
of scope and auto-renewing passes are red-lined unless an explicit in-scope
decision is recorded.

Use when:

- a consumer-product goal needs trend, demand, and competitor evidence
- an opportunity must be sized net-of-fee before any concept work begins
- the coordinator needs a GO / NO-GO / PIVOT call backed by money signals

Do not use when:

- a concept is already selected and only needs shaping (use ideation)
- the product is B2B seat-based SaaS

## Required inputs

- product goal, constraints, monetization lane, and success bar
- access to market-intel tools and store/charts data (or named proxies)
- prior-title priors when this is a sequel/follow-on (payer base, CRM, tooling)

## Owned outputs

- an Opportunity Thesis: who, what job, why now, why pay, why us
- unit-economics model v0 (all inputs flagged assumption-vs-measured)
- the GO / NO-GO / PIVOT call with pre-registered demand thresholds

## Core workflow

1. Scan trends across market-intel (Sensor Tower, AppMagic, Appfigures, AppTweak, SimilarWeb), store charts, Steam tags/wishlists, and TikTok/Reddit/Product Hunt; tag each as durable vs fad vs over-served and note the launch-window/seasonality.
2. Source opportunities via review mining, frustration archaeology, monetization-gap, genre recombination, platform-capability arbitrage, and re-segmentation.
3. Harvest demand signals as the CHEAPEST kill gate: keyword volume, wishlist velocity, fake-door taps, and social purchase-intent, scored against a pre-registered pass/fail BEFORE the data lands.
4. Mine top-charts and RPD to confirm the segment actually spends money, not just attention.
5. Tear down 3-6 direct competitors plus adjacents: map paywall placement and LLM-cluster 1-3 star reviews into addressable gaps.
6. Build personas + JTBD (functional/emotional/social), anti-personas, and willingness-to-pay by segment.
7. Size the market bottoms-up (TAM/SAM/SOM) NET of store fee; name the commission case per channel explicitly (15%/30% base tiers; US external-purchase links, EU DMA terms, and Play alternative billing carry different tiers — model the fee your actual channel pays).
8. Initialize unit-economics model v0 and issue the GO / NO-GO / PIVOT call.

Steps 1-6 are independent scan modes with no shared state — run trend scanning, opportunity sourcing, demand-signal harvesting, RPD mining, teardowns, and persona/JTBD work concurrently (parallel subagents where delegated); only sizing (7) and model v0 + the call (8) serialize on their outputs.

## Demand-signal kill gate

- The cheapest kill is a demand test, not a build; spend cents before dollars.
- Pre-register the pass/fail threshold first, then gather the signal; never move the goalpost after taps land.
- A frustrated review crowd with no spend behind it is interest, not demand.

## Anti-patterns

- do not size TAM/SAM/SOM on gross revenue or top-down hand-waving
- do not treat attention (views, upvotes) as willingness to pay
- do not skip the demand kill gate because the idea "feels obvious"
- do not advance a fad or over-served trend as if it were durable
- do not relabel a subscription opportunity as a repeated one-off

## Handoff and downstream impact

- hand ideation the Opportunity Thesis, the personas/JTBD, and WTP-by-segment
- hand the money gate model v0 with every input labeled assumption-vs-measured
- name the loop-back: a NO-GO returns to trend scanning, a PIVOT re-sources

## Completion checklist

- the Opportunity Thesis names who, what job, why now, why pay, why us
- demand thresholds were pre-registered and the result is recorded
- TAM/SAM/SOM is bottoms-up and net-of-fee with the per-channel fee case named
- model v0 exists and the GO / NO-GO / PIVOT call is explicit

## References

- [Unit economics model](../fa-product-unit-economics-model/SKILL.md)
- [Money gate](../fa-product-money-gate/SKILL.md)
- [Product process](../fa-product-process/SKILL.md)
- [Product discovery team architecture](../../references/product-discovery-team.md)
