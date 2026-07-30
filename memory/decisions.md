---
name: ai-toolkit-gaia-decisions
description: "Flat-JSON over EF+Postgres deliberately; public distribution is the recorded stealth exception; plugins pinned to ref:main not tags"
type: decision
last_verified: 2026-07-24
---

# Live decisions

- **Flat-JSON persistence over EF Core + Postgres** is deliberate for a service of this size. Recorded here because it is recorded nowhere else, and because this repo ships the skill mandating the opposite to every other project.
- **Public distribution is the one recorded stealth exception in Labs.** Do not relitigate.
- **Plugins are pinned to `ref: main`, not a tag** — a conscious choice whose consequence is that installs float with `main`.
