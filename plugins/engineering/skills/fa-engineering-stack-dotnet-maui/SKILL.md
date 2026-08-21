---
name: fa-engineering-stack-dotnet-maui
description: Provides the .NET MAUI cross-platform client-app baseline - enforced analyzers, unit coverage of the non-UI logic that MAUI's UI automation cannot reach, per-target-framework build validation with the platform limits of CI stated honestly, a command surface CI calls verbatim, and a repeatable path for building and deploying to Android emulators, iOS simulators and physical devices. Use it by auditing a MAUI repository against the baseline table, closing only its real gaps, recording which target frameworks CI can and cannot build rather than implying full coverage, and following the device reference before installing or launching a build on any device. Use it when a MAUI repository has no enforced analyzers, no runnable unit project, or a CI build that quietly covers one platform only, and before UI feature work or on-device QA that needs a gate. It never chooses between MAUI and another client framework; that call belongs to `fa-engineering-default-tech-stack`.
license: MIT
---

# Stack Baseline: .NET MAUI

## Scope and when to use

Use this skill to give a MAUI client repository a foundation that is honest about
what CI can build and what its tests actually cover.

Use this skill when:

- a MAUI repository lacks enforced analyzers, a unit project, or CI build validation
- a new MAUI app is being bootstrapped and needs a defensible foundation
- UI feature work is about to start with nothing gating it
- an existing MAUI repository's CI coverage is unclear or overstated
- emulator, simulator or on-device validation is about to run with no repeatable deploy path

Do not use this skill when:

- the stack itself is still undecided - choosing between stacks belongs to `fa-engineering-default-tech-stack`
- the repository is a .NET service, JS/TS, Flutter or Python; use the sibling stack skill for it
- the work is store submission, release distribution or signing-certificate administration

## Required inputs

- the project layout, the target frameworks declared, and any existing analyzer configuration
- which MAUI workloads and platform SDKs the build requires, and which emulators, simulators or physical devices QA will run against
- the CI runner images available and which platforms they can legitimately build
- the existing test projects, if any, and how much logic sits outside the views

## Owned outputs

- an enforced analyzer baseline that fails the build rather than warning
- a build target that succeeds under real CI platform constraints, with those constraints written down
- a unit lane covering the non-UI logic, however small it starts
- run-and-test instructions, including workload prerequisites and a repeatable device deploy path, in the repository's own QA runbook

## Decision tree

- If the repository already has an analyzer set or test convention, adopt it and close only genuine gaps.
- If the full multi-target build cannot run on available CI runners, build the shared and core projects and state the limit plainly.
- If no physical device is available, validate on an emulator and record on-device behaviour as unverified rather than letting the pass imply it.
- If a platform target is dropped from CI, record it as a known coverage gap rather than letting the green badge imply otherwise.

## Core workflow

1. Inventory target frameworks, workloads, analyzer config, test projects and CI jobs before proposing anything.
2. Establish the analyzer baseline so one command fails on a violation.
3. Establish the build target, building the pure shared or core project first because it compiles in seconds and catches almost everything, and pinning the target framework where the runner cannot build them all.
4. Establish the unit lane over non-UI logic, adding a minimal project if none exists.
5. Wire CI to the same command surface and write down, in one or two factual lines, what it does not cover.
6. Update the repository's QA runbook with workload prerequisites, commands and the device deploy path from the device reference, then hand off the gate list.

## Baseline contract and its traps

| Outcome | Done means | Silent failure if skipped |
|---|---|---|
| Analyzers enforced | verification exits nonzero on a violation | warnings accumulate until nobody reads the build log |
| Build validated in CI | the targets CI can build are built, every run | a platform breaks and is discovered at release |
| Platform limits stated | the README or workflow names what CI cannot build | a green badge is read as full-platform coverage |
| Unit lane over logic | one command runs the non-UI suites and fails loudly | a green test command asserts nothing at all |
| Device path repeatable | one written sequence installs and launches the build just made | QA describes a build that was never installed |
| Docs current | the QA runbook names workloads and commands | a fresh clone fails to build for unexplained reasons |

### Required files

- a canonical command surface at the repository root, typically a `Makefile`
- `.editorconfig` carrying the format and analyzer rules
- a CI workflow that invokes the command surface (`.github/workflows/ci.yml` or the repository's equivalent)
- the repository's own QA runbook, wherever its docs tree keeps run and test instructions

### Standard targets

- required: `lint`, `build`, and `test` for the unit lane
- there is deliberately no compose target here; a client app has no local service stack to bring up

### Traps

- **Prefer existing repository conventions over every default named here.** A second analyzer package layered on a working one yields two disagreeing verdicts and a config nobody dares delete.
- **A multi-target build fails on the target the machine cannot serve.** An unqualified build attempts every declared target framework, so a runner missing one platform's SDK reports a failure that looks like broken code. Pin the framework where the runner is limited, and say why.
- **Missing workloads produce misleading errors.** Without the MAUI workloads restored, the build reports missing targets or unknown SDKs rather than a missing prerequisite, and the next reader debugs the wrong thing. Make workload restore part of the documented setup.
- **State the coverage gap in the same place as the green badge.** A CI lane that builds one platform while the app ships three is fine; a CI lane that hides it is a defect, because the release decision is made from that signal.
- **UI automation is constrained, so keep testable logic out of the views, and do not add tests that only prove the framework works.** Logic embedded in code-behind cannot be reached by the unit lane, and a test asserting that a view model constructs is inventory rather than coverage; together they are how a repository ends up with a passing `test` command, zero real assertions, and a gap nobody can see.
- **Installing an artifact the build did not fully produce silently runs the old code.** A default Debug Android build leaves the managed assemblies outside the package, so installing it by hand leaves the device executing an earlier deployment while reporting success. Pick one of the two Android paths in the device reference below and never mix them.
- **A green build is not a working app, and an emulator is not a device.** Gradient alpha, styles shared across shape instances and duplicated route or tab names all compile and fail only at runtime, while sensor, haptic, performance and codesigning behaviour diverge on real hardware. Suppressed toolchain version checks belong in the same category: they unblock a build and defer a real mismatch to runtime.

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
|---|---|---|---|
| CI cannot build a platform target | build what it can, record the gap | engineer | escalate if a runner image is needed |
| build fails on a missing workload | restore workloads and document the prerequisite | engineer | fix the setup docs, not the build file |
| unit lane has nothing to cover | move logic out of views before adding tests | engineer | route to `fa-engineering-architecture` if structural |
| analyzers flood the build with warnings | enforce a scoped rule set first, widen later | engineer | record the deviation in the plan |

## Anti-patterns

- do not present a single-platform CI build as full coverage
- do not write UI tests the harness cannot run reliably, then mark them skipped and forget them
- do not restructure the app as part of a baseline pass
- do not let CI become the only place the real commands live

## Handoff and downstream impact

- give planning the gates this baseline earns: `lint`, `build` and `ci` for setup, plus `unit` once real tests exist
- give implementation the canonical command surface and the pinned target framework CI uses
- give testing the honest coverage boundary, so manual device validation is planned rather than assumed

## Examples

- **Good fit:** a MAUI app whose CI builds only one platform and whose single test project asserts nothing.
- **Good fit:** bootstrapping a new MAUI app that needs a build gate before UI work starts.
- **Not a fit:** deciding whether the product should be MAUI, a web app, or native per platform.

## Completion checklist

- lint fails on a violation and the build runs on the available runners
- what CI cannot build is written down where the build result is read
- the unit lane covers logic that lives outside the views
- the QA runbook names workload prerequisites and commands, and planning has the gate list

## References

- [Gaia delivery policy](../../references/delivery-policy.md)
- [Gaia ownership and conventions](../../references/ownership-and-conventions.md)
- [Default tech stack routing](../fa-engineering-default-tech-stack/SKILL.md)
- [Device build and deploy](references/device-build-and-deploy.md)
