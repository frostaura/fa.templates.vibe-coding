---
name: ai-toolkit-gaia-questions
description: "BLOCKING BEFORE PUSH: the migration moves the Woolworths reverse-engineering from a private repo into this public one; one repo now ships two unrelated products; placement still reads as Technologies; main→tags"
type: question
last_verified: 2026-08-21
---

# Open questions

## ⚠ Blocking, before v13.0.0 is pushed: private → public

`fa.integrations` was **private**. This repo is **public**. The migration therefore does
more than move code — it publishes work only its author has seen:

- **`WoolworthsSearchClient` carries a Constructor.io key lifted from the Woolworths
  storefront bundle.** Not a secret leak — the key is served to every browser. What changes
  is attribution and convenience: a public repo under FrostAura's name hands anyone a
  working, documented proxy for a search contract **Woolworths pays for and we do not**.
  Abuse lands on their bill, which makes it worse rather than better.
- **`docs/integrations/woolworths.md` is a 300-line reverse-engineered contract** for
  undocumented endpoints. As a private note it is engineering documentation; published under
  a company name it reads as an invitation.
- **No agreement, no terms, no notice channel with Woolworths**, and there never was.
  Tolerable for a private tool a founder ran against his own account; a different
  proposition attached to a public MIT-licensed repository.

This is not an argument that the migration was wrong. It is the argument that **publishing
is a separate decision from migrating, and it has not been made.** v13.0.0 is uncommitted
and unpushed, so the choice is free: state the repo's stance explicitly (personal-use tool,
no affiliation, no warranty); split the server into a private repo; approach Woolworths; or
publish deliberately. **Founder's call — do not let a push make it by default.**

## Inherited from `fa.integrations` — these did not die with the repo

- **Does a headless-filled cart appear in the Woolworths mobile app?** Account-bound and it
  survived sign-out in testing, so it should, but never confirmed on a phone. The Siri
  Shortcut flow ends with "open the cart".
- **Should the write path require explicit confirmation?**
  `woolworths_add_shopping_list_to_cart` adds ~15 items in one call with no confirmation.
  It stops short of checkout, but an agent calling it silently mutates a real cart — more
  pressing now the tools are reachable from a hosted server.
- **Will Woolworths tolerate this traffic pattern?** Unknowable without asking. Treat the
  integration as something that can break without warning.

## Standing

- **This repo now ships two unrelated products** — a developer-delivery plugin suite and a
  grocery integration gateway — sharing a repo, a CHANGELOG and a hostname, and since
  `foundation` stopped wiring the server, nothing else. Leave it and say so; split the
  server out; or reframe Gaia deliberately.
- **Placement** still reads as `Technologies/`, and a live credential-handling gateway is an
  operations concern, not a Labs one. Parent-controlled.
- **`main` vs tags for plugin refs.** Floating installs are dangerous for a public user base,
  and 13.0.0 is exactly the breaking change that lands the moment it is pushed.
- **Should the integration be wired into any plugin at all?** If so it wants its own opt-in
  plugin, not the context layer.

_(Resolved 2026-08-21: the `b9d2ee3` deletion incident — restored, committed, pushed. The
vacuous test gate — `src/Gaia.Mcp.Tests` is real, 12 assertions, gated in CI.)_
