# Product Discovery Team — Architecture

> Design source of truth for Gaia's consumer-product-discovery team: a coordinated
> set of agents and skills that take a consumer-software goal from idea to a
> money-validated bet, through launch, live-ops, and portfolio compounding.

This team lives inside the `gaia-foundation` plugin and reuses Gaia's existing
coordinator → fan-out → adversarial-review → gate/proof machinery. It is a new
_domain_ (product management) layered on the same operating model as the
software-delivery roles, not a replacement for them.

## Scope

In scope — monetization through:

- one-off purchases (premium paid download, paid unlock / "go pro")
- consumable IAP (currency, lives, boosters, refills)
- durable IAP (remove-ads, level/character/content packs)
- cosmetics / in-app goods (IAG) and bundles
- per-cycle passes purchased fresh each season

Out of scope:

- B2B seat-based / per-tenant SaaS
- **Red line:** auto-renewing passes and "VIP/club monthly" offers are NOT
  one-off purchases. They are consumer subscriptions with a retention/churn LTV
  curve and may only be modeled as such after a deliberate, recorded in-scope
  decision — never smuggled in as a "repeated one-off."

## Cross-cutting principles

These hold across every stage and are enforced by the coordinator and the
`gaia-product-money-gate` skill:

1. **Money is the gate, not interest.** Every macro-gate is a willingness-to-PAY
   decision. Free interest ("cool, I'd download that") is discounted; only
   money-on-the-line or attention-with-cost signals validate.
2. **One shared, versioned unit-economics model.** A single canonical model is
   initialized in Discovery and re-run/updated at every gate. No stage re-derives
   economics from scratch. (`gaia-unit-economics-model`)
3. **Every gate is net-of-fee with an explicit LTV:CAC ratio and payback window.**
   Revenue is quoted net of 15/30% store commission, refunds, chargebacks, fraud,
   and tax. Gates require LTV:CAC ≥ 3:1, a financeable payback window, and a
   cashflow/working-capital check (store payouts lag CAC by 30–45 days).
4. **Stated WTP is deflated; pLTV is calibrated to realized cohorts.** Survey WTP
   is a ceiling with a quantified deflation haircut, confirmed by a money test.
5. **Parallel within a stage, strict causal gating between stages.**
6. **Kill cheap, kill early, pre-register thresholds.** Success/kill numbers are
   written down BEFORE data lands so goalposts cannot move.
7. **Loop-backs are explicit edges, not vague "refine."** (See loop-back table.)
8. **Design monetization into the loop; instrument it first.** Analytics + IAP /
   entitlement / restore rails are built and sandbox-proven before content.

## The lifecycle (10 money-gated stages)

Each stage exits only on a real-consumer-money / willingness-to-pay gate.

- **S1 — Discovery & Market Research** — narrow the market to one buildable,
  monetizable bet. Exit: evidence-backed Opportunity Thesis + unit-economics
  model v0 + GO/NO-GO/PIVOT.
- **S2 — Ideation & Concept Generation** — diverge then converge to ONE concept
  with a committed monetization hypothesis. Exit: concept brief + net-of-fee
  viability + kill/park for all others.
- **S3 — Business-Model & Monetization Design** — design the economy, SKU ladder,
  ad stack. Exit: net blended LTV > CAC ≥ 3:1 + payback + cashflow check.
- **S4 — Validation & Demand-Testing** — money-on-the-line experiments. Exit:
  deflated WTP + ≥1 paid signal + IP clearance; measured CPI replaces placeholder.
- **S5 — Definition, Prototyping & MVP / Vertical-Slice** — smallest lovable build
  with live sandbox IAP + instrumented funnel. Exit: slice hits pre-set thresholds.
- **S6 — Production Build, Content & Hardening** — scale content, device/perf QA,
  anti-cheat, localization. Exit: feature/content-complete, QA matrix passed.
- **S7 — Compliance, Ratings, Legal & Certification** — age ratings, loot-box /
  gacha law, COPPA/GDPR-K, ATT, tax, EULA. Exit: signed compliance certificate.
- **S8 — Build-Iteration & Soft-Launch** — real money + retention in cheap geos.
  Exit: multi-condition SCALE GATE (retention + ARPDAU + net LTV > CPI + payback).
- **S9 — Go-To-Market, Launch & UA** — repeatable creative + channel engine. Exit:
  positive blended-CAC-to-LTV at scale + support/fraud ops live.
- **S10 — Post-Launch, Live-Ops, Lifecycle & Portfolio** — the revenue engine;
  ends in a sunset / maintenance / sequel decision that compounds back to S1.

## Coordination pattern

Hub-and-spoke, mirroring the proven "8 parallel surveys → 2 adversarial reviews →
synthesis" workflow:

- **The coordinator is the only coordinator.** Specialists are leaf workers; they
  do not call each other. Cross-specialist needs are sequenced by the coordinator.
- **Fan-out:** within a stage, the coordinator launches independent specialist
  work in parallel.
- **Adversarial review:** ≥2 independent `gaia-product-reviewer` instances critique
  each stage output and emit traceable `CORRECTION from review:` notes.
- **Synthesis:** `gaia-product-synthesizer` merges specialist outputs + corrections
  into the stage artifact and the final report.
- **Shared state:** the unit-economics model + Opportunity Thesis are the passed
  artifacts. Specialists read/update the model; they never re-derive it.
- **Auto-advance gates:** the coordinator advances automatically while
  pre-registered, net-of-fee thresholds pass; it stops only on a fail/kill or a
  loop-back trigger.

## Agent roster

| Agent                                 | Stage(s) | Owns                                                                             |
| ------------------------------------- | -------- | -------------------------------------------------------------------------------- |
| `gaia-product-coordinator`            | all      | money-gated DAG, fan-out, gate evaluation, loop-back routing, synthesis dispatch |
| `gaia-product-market-researcher`      | S1       | Opportunity Thesis, demand evidence, sizing, persona/JTBD                        |
| `gaia-product-ideation-strategist`    | S2       | concept generation, scoring, concept brief                                       |
| `gaia-product-monetization-economist` | S3       | monetization model, price ladder, economy — **owns the unit-economics model**    |
| `gaia-product-validation-researcher`  | S4       | interviews, WTP, fake-door, ad-CPI, IP clearance                                 |
| `gaia-product-build-architect`        | S5–S6    | vertical slice, instrument/IAP rails, production hardening                       |
| `gaia-product-compliance-officer`     | S7       | ratings, loot-box/gacha law, privacy/ATT, tax, EULA                              |
| `gaia-product-growth-marketer`        | S8–S9    | soft-launch scale gate, ASO/UA, creative, CAC/LTV loop                           |
| `gaia-product-liveops-manager`        | S10      | live-ops calendar, CRM, whale program, churn, portfolio                          |
| `gaia-product-reviewer`               | cross    | adversarial critique (run as ≥2 instances), corrections                          |
| `gaia-product-synthesizer`            | cross    | merge outputs + corrections into the final artifact                              |

## Skill catalog

| Skill                                  | Stage | Purpose                                                                  |
| -------------------------------------- | ----- | ------------------------------------------------------------------------ |
| `gaia-product-process`                 | all   | lifecycle + coordination playbook (the coordinator's primary skill)      |
| `gaia-unit-economics-model`            | all   | the shared, versioned model: inputs, net-of-fee math, calculator         |
| `gaia-product-money-gate`              | all   | pre-registered thresholds, net-of-fee gate evaluation, loop-back routing |
| `gaia-product-discovery`               | S1    | trend scan, competitor teardown, sizing, persona/JTBD                    |
| `gaia-product-ideation`                | S2    | divergence/convergence, RICE/opportunity scoring, concept brief          |
| `gaia-product-monetization-design`     | S3    | model choice, price ladder, IAP catalogue, economy faucets/sinks         |
| `gaia-product-validation`              | S4    | interviews, WTP deflation, fake-door, landing/ad CPI                     |
| `gaia-product-vertical-slice`          | S5    | MLP/MVG, instrument-first, IAP/entitlement rails, prototyping ladder     |
| `gaia-product-production-hardening`    | S6    | content pipeline, device/perf QA, anti-cheat, localization               |
| `gaia-product-compliance`              | S7    | ratings, loot-box/gacha, privacy/ATT, tax, EULA/ToS                      |
| `gaia-product-soft-launch`             | S8    | soft-launch KPIs, scale gate, iterate-or-kill discipline                 |
| `gaia-product-gtm-launch`              | S9    | ASO, featuring, creators, paid UA, CAC/LTV control loop                  |
| `gaia-product-liveops`                 | S10   | live-ops calendar, CRM, segmentation, whale program, churn               |
| `gaia-product-portfolio-retrospective` | S10   | end-of-life decision, capability + payer-base harvest back to S1         |

## Loop-back edges

The lifecycle is a loop, not a line. The coordinator routes these backward:

| Trigger                             | Detected at | Routes back to                                           |
| ----------------------------------- | ----------- | -------------------------------------------------------- |
| Weak / no paid demand               | S4          | S2 (re-select concept) or kill                           |
| Deflated WTP below ladder economics | S4          | S3 (re-tune price ladder / economy)                      |
| Measured CPI > achievable net LTV   | S4 / S8     | S3 + unit-economics model update                         |
| High-retention / zero-conversion    | S8          | S2 (reopen concept-level monetization verdict)           |
| Store-listing CVR test failure      | S9          | creative/positioning (S9); possibly reposition concept   |
| Churn cause = monetization pressure | S10         | S3 (economy / offer-pressure design)                     |
| Platform SDK / policy deprecation   | S10         | S6 (forced API bump) and/or S7 (re-certification)        |
| Sequel / DLC greenlit               | S10         | S1 (new cycle; carry payer base + CRM + tooling forward) |

## Gate model (auto-advance)

- The coordinator evaluates each stage's exit gate via `gaia-product-money-gate`.
- Thresholds are **pre-registered** before the stage's evidence is gathered.
- All revenue is **net-of-fee** (commission + refunds + fraud + tax) and checked
  against LTV:CAC ≥ 3:1, payback window, and the cashflow/working-capital gap.
- **Pass →** advance to the next stage automatically.
- **Fail →** stop and route to the loop-back target (or kill) — never advance on
  gross, optimistic, or unpaid-back economics.

## Invocation

All entry points reach `gaia-product-coordinator`:

- **Natural language:** describe a consumer-product goal and the coordinator's description auto-routes it (Claude), or pick `gaia-product-coordinator` from the agent picker (Copilot).
- **Direct specialist / skill:** for spot work, invoke a single specialist (e.g. `gaia-product-market-researcher`) or trigger a stage skill (e.g. "score these concepts with RICE"), bypassing the full pipeline.

Example:

```text
/gaia-product-discovery premium one-off cozy roguelite for Steam, solo dev, ~$150k net year one
```

The command frames the brief (lane / platform / success bar), initializes the unit-economics model, and runs the money-gated lifecycle — auto-advancing each gate while net-of-fee thresholds hold, otherwise looping back or killing.

## Packaging & conventions

- Agents: `.github/agents/gaia-product-*.md` (Copilot custom-agent frontmatter).
- Skills: `.github/skills/gaia-product-*/SKILL.md` (+ `references/`).
- `.claude/{agents,skills}` symlink to `.github` (single source of truth).
- Naming: `gaia-product-<role|skill>`; the hub is always a **coordinator**.
- Portability: the same `SKILL.md` and agent frontmatter run on both Claude Code
  and GitHub Copilot (both support the Agent Skills format).
- Distribution: ships with `gaia-foundation` via the `frostaura` marketplace.

## Relationship to the software-delivery roles

The product team decides _what consumer product to build and how it makes money_.
When a bet clears S4/S5 and needs engineering, it hands a built spec to the
existing Gaia delivery roles (`gaia-solutions-architect` → `gaia-implementation-planner`
→ `gaia-software-engineer` → `gaia-tester` → `gaia-release-engineer`). The product
coordinator owns the money-gated discovery loop; the delivery roles own
construction.
