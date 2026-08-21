---
name: ai-toolkit-gaia-questions
description: "Woolworths reverse-engineering is now PUBLISHED (founder-authorised 2026-08-21) and the terms/agreement question is live rather than hypothetical; one repo ships two unrelated products; placement reads as Technologies; main→tags"
type: question
last_verified: 2026-08-21
---

# Open questions

## Published, 2026-08-21 — the exposure is now live, not hypothetical

**Decided:** the founder authorised the push with this stated, so v13.0.0 is on
`origin/main`. `fa.integrations` was **private**; this repo is **public**; the Woolworths
work is now published. What that means, kept here because it stopped being a question about
whether and became a question about what to do next:

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

**Still open, and now time-sensitive rather than preventable:** the README says nothing
about the integration's stance. Worth adding deliberately — personal-use tool, no
affiliation with or endorsement by Woolworths, no warranty — and worth deciding whether to
approach them. Unpublishing is not a remedy once something is on a public repo; the
remaining levers are framing, a vendor conversation, and being ready to remove the
integration if asked.

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
