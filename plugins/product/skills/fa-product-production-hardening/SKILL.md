---
name: fa-product-production-hardening
description: Provides the production and hardening playbook (Stage 6) for consumer one-off / in-app-purchase products - scaling content to v1 with tooling and post-launch runway, full art / audio / VFX to the lovable bar with licensing cleared, localization at scale with localized price tiers reconciled to net-by-geo, a device and OS matrix with performance budgets, backend and economy services with anti-cheat plus IAP-fraud and refund-abuse defenses and load testing, an accessibility baseline, and multi-layer QA reconciling production IAP end to end. Use it by closing the gap from a proven slice to feature/content-complete with a production-grade IAP and entitlement path. Use it when a passing vertical slice must become a shippable v1, when production billing must be reconciled end to end, or when device, performance, localization and QA matrices must be cleared before certification. It never clears ratings or store submission - that is `fa-product-compliance`.
license: MIT
---

# Gaia Product Production Hardening

## Scope and when to use

Use this skill to take a proven slice to feature/content-complete and
production-grade. Scope is one-off + IAP/IAG monetization; B2B seat-based SaaS is
out of scope and an auto-renew model is red-lined unless an in-scope decision is
recorded.

Use when:

- a passing vertical slice must scale to a shippable v1
- production IAP/entitlement must be hardened and reconciled end-to-end
- device, performance, localization, and QA matrices must be cleared

Do not use when:

- no vertical slice has passed its gate yet (run the slice first)
- the title is shipping and only needs soft-launch/live-ops

## Required inputs

- the proven vertical slice and the binding monetization instrumentation contract
- the current unit-economics model version and net-by-geo price tiers
- target device/OS matrix, projected DAU, and licensing status

## Owned outputs

- content scaled to v1 with tooling and post-launch runway
- localized, device-tested, accessible, load-tested production build
- a production-grade IAP/entitlement path reconciled end-to-end

## Core workflow

1. Scale content to v1 via tooling/pipeline; validate each batch against economy/difficulty and build post-launch runway.
2. Bring art/audio/VFX to the lovable bar with all licensing cleared.
3. Localize at scale (culturally adapted) and reconcile localized price tiers to net-by-geo, with locale QA.
4. Clear the device/OS matrix and performance targets: low-end Android, thermals/memory/load, a crash-free-users target, and fragmented-Android QA.
5. Harden backend/economy services + save/cloud-sync + anti-cheat + IAP-fraud/refund-abuse defenses; load test at projected DAU and confirm remote-config economy.
6. Meet an accessibility baseline as a featuring criterion.
7. Run multi-layer QA (functional, soak/stability, compatibility, load) and reconcile production IAP end-to-end.

## End-to-end IAP reconciliation

- Production IAP must reconcile from purchase through entitlement to ledger.
- Localized price tiers must tie back to the net-by-geo model, not just display.
- Refund-abuse and IAP-fraud defenses must hold under projected DAU load.

## Anti-patterns

- do not scale content without validating each batch against the economy
- do not ship localized prices that drift from the net-by-geo model
- do not skip low-end / fragmented-Android device QA
- do not load test below projected DAU
- do not call IAP done without an end-to-end reconciliation

## Handoff and downstream impact

- hand soft-launch a feature/content-complete, production-grade build
- hand the money gate the reconciled production IAP path and net-by-geo tiers
- name the loop-back: a failed QA or reconciliation returns to hardening

## Completion checklist

- the build is feature/content-complete with post-launch runway
- localization, device, performance, and accessibility matrices pass
- backend/economy/anti-cheat are load-tested at projected DAU
- production IAP/entitlement reconciles end-to-end

## References

- [Unit economics model](../fa-product-unit-economics-model/SKILL.md)
- [Money gate](../fa-product-money-gate/SKILL.md)
- [Product process](../fa-product-process/SKILL.md)
- [Product discovery team architecture](../../references/product-discovery-team.md)
