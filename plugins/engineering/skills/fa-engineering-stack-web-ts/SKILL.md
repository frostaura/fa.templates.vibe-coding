---
name: fa-engineering-stack-web-ts
description: Provides the JavaScript/TypeScript web-repo baseline - ESLint and Prettier enforcement, one runnable unit lane on the repo's own runner (Vitest, Jest or equivalent), Playwright browser end-to-end coverage, and a command surface that CI calls verbatim. Use it by auditing a Node or TypeScript repository against the baseline table, closing only the gaps it actually has, and deferring to whatever runner, formatter and config the repository already established. Use it when a React, Next, Vite, Svelte, Astro or Node/TypeScript repository has no enforced lint, no single test command, or no browser E2E harness, and before feature work lands in such a repository. It never decides which language or platform a project should be built on; that call belongs to `fa-engineering-default-tech-stack`.
license: MIT
---

# Stack Baseline: Web (JavaScript/TypeScript)

## Scope and when to use

Use this skill to bring a JS/TS web repository up to a foundation that lint,
test and CI can stand on, without rewriting choices it already made.

Use this skill when:

- a JS/TS web repository lacks enforced lint, a single test command, or browser E2E
- a new web application is being bootstrapped from an empty or near-empty tree
- feature work is about to start and there is nothing to gate it with
- an existing web repository is being standardized before a larger change

Do not use this skill when:

- the stack itself is still undecided - choosing between stacks belongs to `fa-engineering-default-tech-stack`
- the repository is .NET, Flutter or Python; use the sibling stack skill for that language
- the gap is React component or design-system quality, which belongs to `fa-engineering-ui`

## Required inputs

- the repository tree, its package manifest, and any existing lint/test config
- the framework in use (React, Next, Vite, Svelte, Astro, plain Node) and its conventions
- the current CI workflow and what it runs today
- whether the repository ships an HTTP surface that needs a compose stack

## Owned outputs

- an enforced lint and format configuration the build fails on
- one canonical command surface, mirrored by CI rather than duplicated by it
- a Playwright E2E harness that owns the lifecycle of the app it tests
- run-and-test instructions updated in the repository's own QA runbook

## Decision tree

- If the repository already uses a different linter, runner or formatter, adopt it and close only the real gaps.
- If no unit harness exists, add the minimal one the framework expects rather than a second parallel runner.
- If no browser E2E exists and use-case behavior is changing, add Playwright before the feature work.
- If CI is split into separate frontend, unit and E2E jobs, keep each job's command identical in effect to the canonical one.

## Core workflow

1. Inventory what already exists - manifest scripts, lint config, runner, CI jobs - before proposing anything.
2. Establish the lint and format baseline so one command fails on a violation.
3. Establish the unit lane on the repository's own runner, adding a minimal harness only if none exists.
4. Add or repair the Playwright E2E harness, headless, with the app lifecycle owned by the config.
5. Wire CI to the same command surface, adding dependency caching only once the lanes are reliable.
6. Update the repository's QA runbook with install, commands and E2E invocation, then hand off the gate list.

## Baseline contract and its traps

| Outcome | Done means | Silent failure if skipped |
|---|---|---|
| Lint and format enforced | one command exits nonzero on a violation | every PR carries unrelated reformat noise |
| One unit command | the repo's runner runs headlessly and fails loudly | contributors run three commands and CI guards none |
| Browser E2E present | Playwright drives the build it produced, headless | UI regressions ship because only unit tests gate merge |
| CI mirrors local | CI job commands match the canonical surface in effect | green CI proves nothing about what a contributor runs |
| Docs current | the QA runbook names install, commands, E2E invocation | onboarding rediscovers setup by trial and error |

### Required files

- a canonical command surface at the repository root - a `Makefile`, or manifest scripts where that is the convention
- a CI workflow that invokes it (`.github/workflows/ci.yml` or the repository's equivalent)
- lint and format configuration, plus a pinned runtime version (`.nvmrc`, `engines`, or a CI matrix)
- a Playwright config and an E2E directory - `tests/e2e/` only when the repository has no convention of its own
- the repository's own QA runbook, wherever its docs tree keeps run and test instructions

### Standard targets

- required: `lint`, `build`, `test`, and `up` / `down` where a compose stack exists
- optional: `format`, and `test-e2e` where the repository separates unit from end-to-end

### Traps

- **Prefer existing repository conventions over every default named here.** A second linter or a second test runner added beside a working one does not raise the floor; it produces two disagreeing verdicts and a config nobody dares delete.
- **Let the E2E config own the app lifecycle.** A Playwright suite pointed at a server someone started by hand passes against a stale bundle and reports it as a clean regression run.
- **Install browsers explicitly in CI.** Playwright without its browser download step fails on the runner in a way that reads as a test failure, and the first instinct is to disable the lane.
- **Do not invent a second traceability scheme.** Keep durable identifiers in test titles only where the repository already does that; two schemes make both unsearchable.
- **A split CI lane is allowed only while it stays equivalent.** Direct package-manager commands in a dedicated job are fine, and drift the moment the canonical target changes without them.

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
|---|---|---|---|
| repository uses a different toolchain | adopt it; close only genuine gaps | engineer | record the deviation in the plan |
| E2E passes locally, flakes in CI | pin browsers, own the server lifecycle, then quarantine | engineer | route to `fa-engineering-testing` if behavioral |
| lint green locally, red in CI | make both run the identical command | engineer | fix the command surface, not the CI file |
| no unit harness exists at all | add the minimal framework-native runner | engineer | escalate if the framework forbids one |

## Anti-patterns

- do not migrate a working runner or formatter to a preferred one as part of a baseline pass
- do not add dependency caching before the lanes are reliable; reliability first, speed second
- do not let CI become the only place the real commands live

## Handoff and downstream impact

- give planning the gates this baseline earns: `lint`, `build` and `ci` for setup, plus `unit` once tests exist and `e2e` once Playwright does
- give implementation the canonical command surface so diffs are verified the same way CI will verify them
- give testing a working harness and the E2E location, not an intention to add one

## Examples

- **Good fit:** a Vite + React repository with tests in three shapes, no lint gate, and a CI file that runs `npm run build` alone.
- **Good fit:** bootstrapping a new Next application that needs a defensible foundation before the first use case.
- **Not a fit:** deciding whether the product should be a web app at all, or which frontend framework to adopt.

## Completion checklist

- one command each for lint, build and test, and CI calls them rather than reimplementing them
- Playwright runs headless and starts the application it tests
- the repository's own conventions survived the pass; only real gaps were filled
- the QA runbook matches the commands that now exist, and planning has the gate list

## References

- [Gaia delivery policy](../../references/delivery-policy.md)
- [Gaia ownership and conventions](../../references/ownership-and-conventions.md)
- [Default tech stack routing](../fa-engineering-default-tech-stack/SKILL.md)
