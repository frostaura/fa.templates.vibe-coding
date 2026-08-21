---
name: fa-engineering-stack-dotnet-api
description: Provides the .NET HTTP API repository baseline - an `.editorconfig` with enforced analyzers and format verification, a `dotnet test` unit lane plus HTTP-boundary integration coverage, a Dockerfile with `docker-compose.yml` and `.env.example` so the service comes up locally, and a command surface CI calls verbatim. Use it by auditing an ASP.NET Core, minimal-API or similar .NET service repository against the baseline table and closing only its real gaps, deferring to analyzer sets and test conventions the repository already established. Use it when a .NET service repository has no failing lint step, no runnable test project, no compose stack that serves a request, or no pinned SDK, and before use-case work that needs a live local API. It never picks the language or framework; that call belongs to `fa-engineering-default-tech-stack`.
license: MIT
---

# Stack Baseline: .NET HTTP API

## Scope and when to use

Use this skill to give a .NET service repository a foundation where lint fails on
violations, tests run, and the API can be brought up and called locally.

Use this skill when:

- a .NET HTTP API repository lacks enforced formatting or a runnable test project
- a new .NET service is being bootstrapped and needs a defensible foundation
- use-case work needs the API reachable locally before validation can mean anything
- an inherited .NET service is being standardized before larger delivery

Do not use this skill when:

- the stack itself is still undecided - choosing between stacks belongs to `fa-engineering-default-tech-stack`
- the repository is JS/TS, MAUI, Flutter or Python; use the sibling stack skill for that language
- **the gap is the container stack itself** — the compose file, the `.env.example` contract, or a service that will not come up locally. That is `fa-engineering-containerization`, which owns the runtime surface for every language. This skill owns the .NET *language* baseline and stops at the project file
- the gap is architectural rather than foundational, which belongs to `fa-engineering-architecture`

## Required inputs

- the solution or project layout, target framework, and any existing analyzer configuration
- the current test projects, if any, and how integration coverage is expressed today
- the CI workflow, whether it pins an SDK, and where the service's required configuration comes from

## Owned outputs

- an enforced format and analyzer baseline that fails the build rather than warning
- a `dotnet test` lane with a real unit project and an HTTP-boundary integration path
- a compose stack that brings the API up locally, proven by a served request
- run-and-test instructions updated in the repository's own QA runbook

## Decision tree

- If the repository already has an analyzer set or test convention, adopt it and close only genuine gaps.
- If no test project exists, add one minimal project shaped to the existing solution rather than a parallel structure.
- If the service exposes HTTP and has no compose stack, add one before any use-case level validation is claimed.
- If CI is split into API, unit and integration jobs, or already has an integration lane, align the canonical target to it rather than adding a second path.

## Core workflow

1. Inventory the solution layout, analyzer config, test projects and CI jobs before proposing anything.
2. Establish the format and analyzer baseline so one command fails on a violation.
3. Establish build and test so a single command each compiles and runs the suites.
4. Add the Dockerfile, compose file and `.env.example`, then prove the API answers a request.
5. Wire CI to the same command surface and pin the SDK so the runner cannot float.
6. Update the repository's QA runbook with commands, compose usage and the local API address, then hand off the gate list.

## Baseline contract and its traps

| Outcome | Done means | Silent failure if skipped |
|---|---|---|
| Format and analyzers enforced | verification exits nonzero on a violation | warnings accumulate until nobody reads the build log |
| Build and test commands | one command each, both failing loudly | contributors verify differently from CI |
| Compose stack serves a request | the API answers on the documented address | "the container is running" is mistaken for "the service works" |
| CI mirrors local | each job's command matches the canonical one in effect | green CI proves nothing about the developer path |
| Docs current | the QA runbook names commands, compose and the address | every new contributor rediscovers the port |

### Required files

- a canonical command surface at the repository root, typically a `Makefile`
- `.editorconfig` carrying the format and analyzer rules
- a CI workflow that invokes the command surface (`.github/workflows/ci.yml` or the repository's equivalent)
- `Dockerfile`, `docker-compose.yml` and `.env.example` whenever the repository serves HTTP
- the repository's own QA runbook, wherever its docs tree keeps run and test instructions

### Standard targets

- required: `lint` (format verification or analyzer run), `build`, `test`, and `up` / `down` for compose
- optional: `test-integration` where the repository separates boundary tests from unit tests

### Traps

- **Prefer existing repository conventions over every default named here.** A second analyzer package layered on a working one yields two disagreeing verdicts and a config nobody dares delete.
- **Verify formatting, do not apply it, in the lint target.** A lint step that rewrites files exits successfully with the repository dirty, so CI reports clean while the branch is not.
- **A running container is not a working service.** Prove `up` with an actual request to a health or root route; a process that crashed after binding still shows as running, and the failure surfaces later as a mystery in someone else's branch.
- **Keep `.env.example` in step with what compose actually requires.** Compose substitutes an unset variable with an empty string rather than failing, so a missing secret becomes a runtime error on first request instead of a startup error.
- **Pin the SDK.** Without a pinned version the runner floats to whatever it has, and a build that passed yesterday fails on a language or analyzer change nobody in the repository made.
- **Integration tests must own their dependencies.** A boundary test pointed at a shared or long-lived database passes on leftover state and fails the first time it runs alone.

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
|---|---|---|---|
| repository uses a different analyzer set | adopt it; close only genuine gaps | engineer | record the deviation in the plan |
| compose comes up but returns errors | check required configuration against `.env.example` | engineer | escalate if a real dependency is unavailable |
| build passes locally, fails in CI | pin the SDK and match the commands | engineer | fix the command surface, not the CI file |
| no test project exists at all | add one minimal project matching solution layout | engineer | route to `fa-engineering-testing` for strategy |

## Anti-patterns

- do not restructure the solution as part of a baseline pass
- do not accept container status as evidence that the API works
- do not let CI become the only place the real commands live

## Handoff and downstream impact

- give planning the gates this baseline earns: `lint`, `build` and `ci` for setup, plus `unit` once tests exist and `integration` once compose-backed checks do
- give implementation the canonical command surface so diffs are verified the way CI will verify them
- give testing a live local API and a documented address rather than an intention to provide one

## Examples

- **Good fit:** an inherited ASP.NET Core service with warnings-as-noise, no test project, and no way to run it locally.
- **Good fit:** bootstrapping a new .NET API that must be callable before its first use case is written.
- **Not a fit:** deciding whether the service should exist, or which persistence technology it should use.

## Completion checklist

- lint verifies rather than rewrites, and fails on a violation
- `up` has been proven by a served request, not by container status
- `.env.example` lists every variable compose consumes
- the QA runbook matches the commands and address that now exist, and planning has the gate list

## References

- [Gaia delivery policy](../../references/delivery-policy.md)
- [Gaia ownership and conventions](../../references/ownership-and-conventions.md)
- [Default tech stack routing](../fa-engineering-default-tech-stack/SKILL.md)
