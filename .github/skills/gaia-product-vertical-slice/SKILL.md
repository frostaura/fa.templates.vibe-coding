---
name: gaia-product-vertical-slice
description: Provides Stage 5 vertical-slice / MVP guidance for consumer one-off / in-app-purchase products - the spec stack (one-pager -> PRD/GDD -> monetization design doc as a binding instrumentation contract), MLP vs MVG with a designed purchase moment, a prototyping fidelity ladder, tech/billing spikes (StoreKit/Play Billing/Steamworks sandbox), server-authority and anti-cheat decisions, building analytics and IAP/entitlement rails first, and a shippable slice with one of every SKU type. Use to ship a lovable core loop with a live sandbox purchase and a clean reconciled funnel.
license: MIT
---

# Gaia Product Vertical Slice

## Scope and when to use

Use this skill to build the lovable core loop with the purchase moment designed
in. Scope is one-off + IAP/IAG monetization; B2B seat-based SaaS is out of scope
and an auto-renew model is red-lined unless an in-scope decision is recorded.

Use when:

- a validated bet needs a shippable-fidelity vertical slice
- billing, entitlement, and analytics rails must exist before content
- purchase intent must be testable against pre-committed thresholds

Do not use when:

- the bet has no money-on-the-line signal yet (run validation first)
- the slice is built and only needs production hardening

## Required inputs

- the validated money model, confirmed price points, and concept brief
- the current unit-economics model version
- platform billing and analytics constraints

## Owned outputs

- the spec stack: one-pager, PRD/GDD, and the binding monetization design doc
- a shippable-fidelity slice with one of every SKU type
- a sandbox purchase granting an entitlement and a clean reconciled funnel

## Core workflow

1. Write the spec stack: one-pager -> PRD/GDD -> monetization design doc as a binding instrumentation contract.
2. Target an MLP (lovable) over a bare MVG; design the purchase moment INTO the loop and map the first-meaningful-purchase path.
3. Climb the prototyping fidelity ladder (paper -> clickable -> playable), killing cheap, high-uncertainty risk first and testing purchase intent even with fake money.
4. Run tech/billing spikes against StoreKit / Play Billing / Steamworks sandbox, covering restore and receipt/entitlement.
5. Decide server-authority vs client-trust plus anti-cheat/fraud posture.
6. Choose the engine / commerce / analytics stack.
7. Build analytics and IAP/entitlement rails FIRST: sandbox purchase, funnel events, entitlement that survives reinstall/restore, receipt validation, and ATT/consent.
8. Build the slice at shippable fidelity including one of every SKU type, against pre-committed success thresholds.

## Rails-first gate

- The slice is not "done" without a sandbox purchase that grants an entitlement.
- The funnel must reconcile cleanly end-to-end before the gate is judged.
- Entitlements must survive reinstall and restore, or the purchase is not real.

## Anti-patterns

- do not bolt the purchase moment on after the loop is built
- do not defer analytics or entitlement rails behind content work
- do not ship a slice missing one of the SKU types it claims to support
- do not skip restore/receipt validation in the billing spike
- do not call the slice done on an unreconciled funnel

## Handoff and downstream impact

- hand production hardening the lovable slice and the instrumentation contract
- hand the money gate the sandbox purchase proof and the reconciled funnel
- name the loop-back: a failed purchase-intent test returns to the loop design

## Completion checklist

- the spec stack exists and the monetization doc is a binding contract
- the slice is shippable fidelity with one of every SKU type
- a sandbox purchase grants an entitlement that survives reinstall/restore
- the funnel reconciles cleanly against pre-committed thresholds

## References

- [Unit economics model](../gaia-unit-economics-model/SKILL.md)
- [Money gate](../gaia-product-money-gate/SKILL.md)
- [Product process](../gaia-product-process/SKILL.md)
- [Product discovery team architecture](../../../docs/architecture/product-discovery-team.md)
