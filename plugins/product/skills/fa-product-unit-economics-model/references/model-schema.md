# Unit Economics Model — Schema

A single versioned record. Initialize in Discovery (v0), update at every gate.

## Identity

- `version` — v0, v1, ... (increment when a measured input replaces an assumption)
- `updated_utc`, `changelog[]` — what changed and which stage measured it

## Acquisition

- `cpi_by_source[]` — {source, geo, cpi, status: assumed|measured}
- `organic_share` — fraction of installs from organic / featuring / viral
- `blended_cac` — total acquisition spend / all new users (fraud-filtered)

## Conversion

- `install_to_payer_rate` — typically 1–5% for IAP
- `repeat_purchase_rate`, `time_to_first_purchase_days`
- `segment_mix` — {non_payer, minnow, dolphin, whale} shares

## Value

- `arppu_lifetime` — cumulative per-cohort, NOT monthly
- `ad_arpdau` — net of network rev-share (hybrid only)
- `iap_price_ladder[]` — {sku, gross_price, net_by_channel[], segment}

## Gross-to-net haircuts

- `store_commission_by_channel[]` — {channel, rate}; 0.15 (small business) / 0.30 (standard) are the Apple/Play base tiers — model the fee each actual channel pays net of its applicable tier (US external-purchase links, EU DMA alternative terms, and Play external offers / User Choice Billing all differ)
- `refund_rate`, `chargeback_rate`, `fraud_rate`, `indirect_tax_rate`

## Timing & retention

- `retention` — {d1, d7, d30, d90}
- `paying_lifetime_days`, `payout_lag_days` (typically 30–45)

## Outputs (computed)

- `net_ltv_per_install`, `ltv_cac_ratio` (target ≥ 3.0)
- `payback_window_days` (genre-appropriate; front-loaded for premium)
- `cash_gap_to_breakeven`, `runway_required`
- `cases` — {conservative, base, optimistic}

## Rules

- All revenue NET of commission + refunds + chargebacks + fraud + tax.
- Stated WTP enters only after a quantified deflation haircut and a money test.
- pLTV is provisional until calibrated against realized D90/D180/D360 cohorts.
