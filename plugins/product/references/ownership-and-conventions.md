# Product Plugin — Ownership and Conventions

Use this reference when deciding which product skill owns a change, which agent
owns a stage, or which naming convention to apply. Scope is the Gaia product
plugin: the money-gated discovery lifecycle for consumer one-off /
in-app-purchase products.

## Skill ownership (14 skills)

| Skill | Primary ownership |
|---|---|
| `fa-product-process` | The 10-stage lifecycle, coordination pattern, handoff contract, and loop-back edges |
| `fa-product-discovery` | Stage 1: trend scanning, demand-signal kill gate, teardowns, personas/JTBD, net-of-fee sizing |
| `fa-product-ideation` | Stage 2: divergence, monetization-seeded backlog, scoring, one-concept selection |
| `fa-product-monetization-design` | Stage 3: primary model, price ladder, IAP catalogue, economy balance, net viability |
| `fa-product-validation` | Stage 4: IP clearance, real-spend interviews, deflated WTP, fake-door/CPI tests |
| `fa-product-vertical-slice` | Stage 5: spec stack, fidelity ladder, billing spikes, rails-first slice |
| `fa-product-production-hardening` | Stage 6: content scale-up, localization, device/perf matrix, anti-cheat, QA |
| `fa-product-compliance` | Stage 7: ratings, submission, loot-box/kids/privacy law, tax, legal docs |
| `fa-product-soft-launch` | Stage 8: telemetry taxonomy, geo soft-launch reads, scale gate, iterate-or-kill |
| `fa-product-gtm-launch` | Stage 9: ASO, featuring, paid UA, creative testing, CAC:LTV scale loop |
| `fa-product-liveops` | Stage 10: live-ops calendar, experiments, CRM, segmentation, whale program |
| `fa-product-portfolio-retrospective` | Stage 10 closure: end-of-life decision, humane sunset, priors harvested to Stage 1 |
| `fa-product-money-gate` | Gate discipline at every stage boundary: net-of-fee math, thresholds, routing |
| `fa-product-unit-economics-model` | The single shared, versioned model and its schema |

## Stage ownership (11 agents)

| Agent | Owns |
|---|---|
| `fa-product-coordinator` | The whole lifecycle; the only coordinator; gates and loop-back routing |
| `fa-product-market-researcher` | Stage 1 |
| `fa-product-ideation-strategist` | Stage 2 |
| `fa-product-monetization-economist` | Stage 3 + stewardship of the unit-economics model |
| `fa-product-validation-researcher` | Stage 4 |
| `fa-product-build-architect` | Stages 5–6 |
| `fa-product-compliance-officer` | Stage 7 |
| `fa-product-growth-marketer` | Stages 8–9 |
| `fa-product-liveops-manager` | Stage 10 |
| `fa-product-reviewer` | Adversarial review of any stage artifact (≥2 independent instances) |
| `fa-product-synthesizer` | Merging specialist outputs + reviewer corrections into one artifact |

Team architecture, the fan-out → adversarial review → synthesis pattern, and
the shared-model contract: [`product-discovery-team.md`](product-discovery-team.md).

## Conventions

- Skill and agent names are `fa-product-<lowercase-hyphenated-role>`; the two
  plugin-wide exceptions are `fa-product-unit-economics-model` (the shared model) and
  the gate/process pair, which keep their established names.
- Specialists are leaf workers: they return artifacts to the coordinator and
  never call peer specialists.
- Every revenue figure anywhere in the plugin is quoted NET of the platform fee
  the actual channel pays, refunds, chargebacks, fraud, and tax.
- Auto-renewing subscriptions are red-lined unless an explicit in-scope
  decision is recorded; B2B seat-based SaaS is out of scope entirely.
- Shared reference docs live in `references/` at the plugin root; skills link
  to them with plain relative paths that never escape the plugin directory.
