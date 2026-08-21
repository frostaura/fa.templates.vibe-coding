---
name: fa-engineering-e2e-testing
description: Provides browser end-to-end specification guidance for user-visible behaviour changes, covering suite location, traceable test IDs carried in test titles, stable selectors, environment-driven base URLs, and CI lane placement. Use it by adding or extending browser specs whose titles carry the requirement ID, wiring them to the repository's canonical test task, and recording spec paths as proof instead of pasted run output. Use it when a change alters what a user can see or do on a web surface, when a drift fix changes a documented flow, or when a QA gate requires browser coverage before closure. It writes and wires the browser specs themselves; deciding which layers a task's gates require is `fa-engineering-testing`, and hand-executed checks are `fa-engineering-manual-regression`.
license: MIT
---

# Gaia End-to-End Testing

## Scope and when to use

Use this skill to author and maintain the browser end-to-end suite that proves a
web surface still does what its requirements say it does. Browser coverage is
**required** for user-visible behaviour changes; it is not traded away for unit
coverage.

Use this skill when:

- a task is classified as a user-visible behaviour change and the repository has a web surface
- a drift fix ("code wins") changes a documented flow a browser can exercise
- a QA gate requires browser coverage before a branch can close
- an existing spec suite needs new cases, restructuring, or CI lane placement

Do not use this skill when:

- the question is which validation layers a change needs, or what the overall pass, fail, or blocked verdict is — that stays with `fa-engineering-testing`, which remains the general testing authority
- the validation is a one-off interactive walkthrough rather than a durable spec — use `fa-engineering-manual-regression`
- the stack cannot be brought up reproducibly yet — `fa-engineering-containerization` is a blocking prerequisite

## Required inputs

- the requirement or use-case documents the change touches, and their IDs
- the repository's existing e2e framework, folder convention, and task-runner entrypoints
- a runnable environment: composed services, base URL, seed data, and auth fixtures
- the CI lane structure (pull-request smoke, browser matrix, stack-backed lane) and each lane's trigger

## Owned outputs

- browser specs added or updated, with requirement IDs inside the test titles
- one headless command that runs the suite, wired to the repository's canonical test task
- lane placement for every new spec, or an explicit blocker when CI cannot run it
- spec paths recorded on `tests_added[]` and config changes on `changed_files[]`

## Decision tree

- If the repository already has an e2e framework and folder, use them; never stand a second convention up beside the first.
- If no framework exists, standardize on Playwright under `tests/e2e/`.
- If the flow needs credentials that are unavailable, raise a blocker, keep completion gated, and cover the reachable cases.
- If a spec needs a running stack, place it in the stack-backed lane, never the default pull-request shard.
- If a spec fails because the flow genuinely changed, correct the spec and the requirement document together; drift is blocking.

## Core workflow

1. Locate the existing suite and confirm how it is invoked, preferring the repository's own test task over a raw runner call.
2. Install and configure a framework only if none exists, with an environment-driven base URL and a single headless command.
3. Translate each affected requirement into specs: the happy path, plus at least one critical validation, auth, or edge case.
4. Carry the requirement ID in the test title — `test('<ID> — <title>', ...)`; filenames may repeat it, titles must hold it.
5. Make data and session state deterministic through seeded fixtures or stored auth state rather than whatever the environment happens to contain.
6. Wire each spec into the correct CI lane and confirm that lane's trigger actually executes it.
7. Record spec and config paths as proof; do not paste run output, transcripts, or screenshots.

## Traceability, selectors, and lane placement

- The ID belongs in the test title because runners print titles: a CI failure then names the requirement with no lookup table. An ID that lives only in a filename is lost the first time that file is split or renamed, and nobody notices, because the suite still passes.
- Prefer role and visible-text selectors. A CSS-class selector fails on a purely cosmetic refactor and passes after a redesign that broke the flow — both are false signals, and the second one ships.
- Resolve the base URL from the environment. A hardcoded host makes the suite pass on the author's machine and silently target the wrong environment everywhere else.
- Keep stack-dependent specs out of the default shard jobs. They fail on every pull request that has no stack running, and the standing red trains reviewers to ignore the lane that matters.
- Put a reduced or extended browser matrix behind an explicit flag or schedule, not in the pull-request path.
- When CI genuinely cannot run the suite, record the blocker and validate through `fa-engineering-manual-regression`. A suite nothing runs is not coverage; it is a file.

## Failure recovery

| Failure mode                 | Recovery                                                | Owner  | Escalation                              |
| ---------------------------- | ------------------------------------------------------- | ------ | --------------------------------------- |
| no suite or framework exists | scaffold under the repository's convention              | tester | planner if the task set is missing too  |
| brittle or flaky selector    | replace with a role or text selector and re-run          | tester | engineer if no accessible handle exists |
| missing credentials or seed  | raise a blocker and keep completion gated                | tester | the human owner                         |
| CI cannot execute the lane   | record the blocker, fall back to manual regression      | tester | release engineer                        |

## Anti-patterns

- do not introduce a second e2e convention beside one that already works
- do not assert DOM presence in place of the observable user outcome
- do not hardcode hostnames, ports, or user identifiers into specs
- do not paste run output, logs, or screenshots into the task record
- do not close a user-visible behaviour change on unit tests alone

## Handoff and downstream impact

- give `fa-engineering-testing` the spec paths and lane placement it needs to reach a verdict
- give release the exact command and lane that reproduce the result
- give implementation precise reproduction detail: spec ID, step, expected, observed
- give architecture the mismatch whenever a spec proves the documented flow is wrong

## Examples

- **Good fit:** add specs for a new sign-up flow, IDs in the titles, wired into the pull-request smoke lane.
- **Good fit:** move compose-dependent specs out of the default shard jobs into the stack-backed lane after they turn the pull-request run permanently red.
- **Not a fit:** decide whether this change needs browser coverage at all; that call belongs to `fa-engineering-testing`.

## Completion checklist

- every affected requirement has at least a happy path and one critical edge case
- every test title carries its requirement ID
- the suite runs headless from one command against an environment-driven base URL
- each spec sits in a lane whose trigger actually runs it, or has a recorded blocker
- proof is spec paths and changed-file paths, never pasted output

## References

- [Gaia delivery policy](../../references/delivery-policy.md)
- [Gaia ownership and conventions](../../references/ownership-and-conventions.md)
