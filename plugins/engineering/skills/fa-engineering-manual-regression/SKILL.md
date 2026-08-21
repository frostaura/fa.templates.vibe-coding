---
name: fa-engineering-manual-regression
description: Provides the interactive regression procedure for behaviour changes in two lanes — an API lane driven by curl-style HTTP checks and a web lane driven by browser automation — both executed against the composed stack rather than a development server. Use it by writing a compact three-to-eight check list per affected requirement, executing it against the canonical QA target, and recording low-context evidence as proof labels and file paths instead of pasted bodies or transcripts. Use it when a behaviour change needs direct observation, when automated coverage is not yet runnable, or when a QA gate demands manual sign-off. It is the hand-executed lane and never replaces automated browser coverage, which is `fa-engineering-e2e-testing`.
license: MIT
---

# Gaia Manual Regression

## Scope and when to use

Use this skill to observe a behaviour change directly, at the boundary a user or
a caller actually touches, and to leave behind evidence a reviewer can act on
without reading a transcript. Manual regression is **required** for behaviour
changes on either surface.

Use this skill when:

- a task is classified as a behaviour change affecting an HTTP API or a web UI
- a drift fix ("code wins") alters documented behaviour and needs confirmation
- automated coverage cannot run yet but validation is still required before closure
- a QA gate demands manual sign-off in addition to automated lanes

Do not use this skill when:

- the change needs durable repeatable browser coverage — author it with `fa-engineering-e2e-testing`; a manual pass is never a substitute for the spec
- the stack cannot be brought up from one command — fix that first with `fa-engineering-containerization`
- the open question is which layers to run or what the overall pass, fail, or blocked verdict is — that stays with `fa-engineering-testing`, the general testing authority

## Required inputs

- the requirement documents the change touches, and their acceptance criteria
- a running composed stack and its canonical QA target
- credentials, tokens, and any seed data the flows require
- for the web lane, working browser-automation tool access

## Owned outputs

- a compact per-requirement checklist, executed and accounted for
- proof labels recorded on `manual_regression[]`: `curl` for the API lane, `playwright-mcp` for the web lane
- any spec created while investigating a failure, recorded on `tests_added[]`
- config or documentation paths recorded on `changed_files[]`

## Decision tree

- If the stack is unreachable, stop; fixing the runtime is blocking and comes first.
- If credentials, ports, or seed data are missing, raise "needs input", keep completion gated, and continue other work in parallel.
- If a check exposes behaviour no document describes, treat it as drift and block until the document and the code agree.
- If a failure is reproducible, promote it to an automated spec where feasible; otherwise open a blocker naming the step, the expectation, and the observation.
- If the browser session itself is untrustworthy, fall back to a headed run of the repository's own live lane against the same target rather than declaring a pass.

## Core workflow

1. Bring the stack up and confirm the canonical QA target answers, along with every dependency the flow needs.
2. Write a checklist of three to eight checks per affected requirement: happy path, one validation or auth failure, and any state change the requirement implies.
3. Execute the checks in the lane that matches the surface, keeping the interaction tight and repeating only what a confirmation genuinely needs.
4. On any failure, capture step, expectation, and observation precisely, then route it — spec, blocker, or drift.
5. Record proof as labels and paths only; never paste response bodies, logs, or tool transcripts.
6. Promote reusable local-setup knowledge into the repository's QA runbook, and leave transient detail out of it.

## The two lanes

### API lane — curl-style HTTP checks

- Run against the composed stack. A check against a hand-started process proves the developer's machine, not the system that ships.
- Assert status codes and the presence of key fields. A full payload diff fails on every unrelated field addition, and a check people learn to ignore is worse than no check.
- Use the real headers and real auth. A check that passes because authentication was skipped has validated nothing the caller will experience.
- Where a flow sends email, verify it through the stack's mail-catcher; verification and reset paths cannot be proven any other way locally.

### Web lane — browser automation

- Target the canonical QA target, never a development port. The development server serves a different bundle through a different proxy path with different configuration, so a green walkthrough there is evidence about a system nobody ships.
- Validate observable outcomes tied to acceptance criteria — rendered state, navigation, key content — not the presence of an element.
- Keep local QA shortcut flags off the path being validated. If a shortcut is what made the check pass, the check passed on the shortcut.
- Move directly through the flow. Exploratory wandering burns context and produces evidence nobody can reproduce.

**Both lanes:** evidence is a label plus a path plus a runnable command. Pasted
output is not reproducible, and it costs the context the next agent needs.

## Failure recovery

| Failure mode                     | Recovery                                                   | Owner  | Escalation                       |
| -------------------------------- | ----------------------------------------------------------- | ------ | -------------------------------- |
| stack unreachable                | fix runtime or compose before any checks                    | tester | engineer                         |
| missing credentials or seed data | raise "needs input", gate completion, continue elsewhere     | tester | the human owner                  |
| undocumented behaviour found     | stop and reconcile document and code; drift is blocking      | tester | architect                        |
| browser session untrustworthy    | rerun headed against the same target before judging          | tester | engineer if the session stays bad |

## Anti-patterns

- do not sign off against a development port when a canonical QA target exists
- do not paste response bodies, logs, or tool transcripts into the task record
- do not stretch a checklist past eight checks; a skimmed list is indistinguishable from a pass
- do not note undocumented behaviour and move on
- do not let a manual pass stand in for automated coverage the change actually needs

## Handoff and downstream impact

- give `fa-engineering-testing` the labels and paths it needs for the verdict
- give `fa-engineering-e2e-testing` every failure worth turning into a durable spec
- give implementation exact reproduction steps for defects, never a vague symptom
- give architecture the reconciliation work whenever observed and documented behaviour diverge

## Examples

- **Good fit:** confirm a new registration flow end to end through the gateway, checking the mail-catcher for the verification message.
- **Good fit:** verify an API's authorization change with six curl-style checks covering happy path, wrong token, and the persisted state change.
- **Not a fit:** decide whether this change needs manual regression at all; that call belongs to `fa-engineering-testing`.

## Completion checklist

- every affected requirement has an executed checklist of at most eight checks
- all checks ran against the composed stack's canonical QA target
- the correct proof label is recorded: `curl`, `playwright-mcp`, or both
- failures are routed as specs, blockers, or drift — never left as prose
- no bodies, logs, or transcripts were stored anywhere

## References

- [Gaia delivery policy](../../references/delivery-policy.md)
- [Gaia ownership and conventions](../../references/ownership-and-conventions.md)
