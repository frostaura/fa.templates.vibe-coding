---
name: fa-engineering-stack-flutter
description: Provides the Flutter and Dart app-repository baseline - `dart format` drift checking that actually fails, a lint rule set wired into `analysis_options.yaml` so the analyzer has something to say, `flutter test` widget and unit coverage, and a command surface CI calls verbatim. Use it by auditing a Flutter repository against the baseline table and closing only its real gaps, deferring to whatever lint package, test layout and SDK pinning the repository already established. Use it when a Flutter repository has a formatter that never fails CI, an analyzer reporting no issues because no rules are enabled, or no runnable test command, and before UI flow work that needs a gate. There is no container stack here; any backend is a separate repository with its own baseline, and this skill never chooses between Flutter and another client framework — that call belongs to `fa-engineering-default-tech-stack`.
license: MIT
---

# Stack Baseline: Flutter

## Scope and when to use

Use this skill to give a Flutter repository a foundation where formatting drift,
analyzer findings and test failures all actually stop a build.

Use this skill when:

- a Flutter repository lacks a failing format check, an enabled lint set, or a test command
- a new Flutter app is being bootstrapped and needs a defensible foundation
- UI flow work is about to start with nothing gating it
- an existing Flutter repository's checks pass suspiciously easily

Do not use this skill when:

- the stack itself is still undecided - choosing between stacks belongs to `fa-engineering-default-tech-stack`
- the repository is .NET, JS/TS or Python; use the sibling stack skill for that language
- the work is on a backend the app talks to, which is its own repository and its own baseline

## Required inputs

- the repository tree, `pubspec.yaml`, and the existing `analysis_options.yaml` if any
- the Flutter and Dart SDK versions the project expects and how they are pinned
- the current CI workflow, the existing test layout, and whether golden tests are in use

## Owned outputs

- a format check that fails on drift instead of silently rewriting files
- an analyzer configured with a real lint rule set, not an empty default
- a `flutter test` lane with at least a working widget or unit scaffold
- run-and-test instructions, including SDK prerequisites, in the repository's own QA runbook

## Decision tree

- If the repository already uses a lint package or test layout, adopt it and close only genuine gaps.
- If the analyzer reports nothing on obviously imperfect code, treat the rule set as missing rather than the code as clean.
- If golden tests exist, decide deliberately whether they belong in the required gate before wiring CI.
- If CI builds distributable artifacts, add a build target; otherwise leave build out rather than faking one.

## Core workflow

1. Inventory `pubspec.yaml`, analysis options, test layout and CI jobs before proposing anything.
2. Establish the format check in verify mode so drift fails rather than being rewritten.
3. Enable a lint rule set in `analysis_options.yaml` and make the analyzer fail on findings.
4. Establish the test lane, adding a minimal widget or unit scaffold only if none exists.
5. Wire CI to the same command surface, pinning the SDK version so the runner cannot float.
6. Update the repository's QA runbook with SDK prerequisites and commands, then hand off the gate list.

## Baseline contract and its traps

| Outcome | Done means | Silent failure if skipped |
|---|---|---|
| Format drift fails | the check reports a nonzero exit on drift | the CI step rewrites files, exits clean, and gates nothing |
| Analyzer has rules | a lint set is enabled and findings fail the run | "No issues found" is read as quality, not as silence |
| Tests run reliably | one command runs the suites and fails loudly | contributors verify by launching the app and looking |
| CI mirrors local | each job's command matches the canonical one in effect | green CI proves nothing about the developer path |
| Docs current | the QA runbook names SDK prerequisites and commands | a fresh clone fails on a version mismatch nobody explains |

### Required files

- a canonical command surface at the repository root, typically a `Makefile`
- `analysis_options.yaml` with an explicit lint rule set included
- a CI workflow that invokes the command surface (`.github/workflows/ci.yml` or the repository's equivalent)
- the repository's own QA runbook, wherever its docs tree keeps run and test instructions

### Standard targets

- required: `lint` (format check plus analyze) and `test`
- optional: `build` where CI produces artifacts, and `run` where a stable launch command is genuinely useful
- there is deliberately no compose target here; a client app has no local service stack to bring up

### Traps

- **Prefer existing repository conventions over every default named here.** A second lint package layered on a working one yields two disagreeing verdicts and a config nobody dares delete.
- **The formatter rewrites by default, which makes it useless as a gate.** Run it in check mode so drift produces a nonzero exit; otherwise the CI step reformats the checkout, passes, and the drift lands in the next contributor's diff.
- **An analyzer with no rule set enabled is not a clean codebase.** Dart lints are opt-in through the analysis options file, so a repository without a lint package included reports no issues on code that would fail everywhere else.
- **Pin the SDK.** Flutter and Dart move together and a floating runner version changes both the analyzer's opinion and the formatter's output, so a build passes one week and fails the next with no code change.
- **Golden tests compare rendered pixels and are host-dependent.** Fonts and rendering differ between a developer machine and a CI image, so goldens generated locally fail on the runner. Keep them out of the required gate unless CI regenerates them on the same image.
- **A widget test that never settles hangs rather than fails.** An unresolved animation or pending timer stalls the lane until the job times out, which reads as infrastructure flakiness and gets the whole suite disabled.

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
|---|---|---|---|
| lint passes on obviously bad code | enable a real lint set, then triage findings | engineer | record the deviation in the plan |
| goldens fail only in CI | regenerate on the CI image or drop from the gate | engineer | route to `fa-engineering-testing` for strategy |
| test lane times out | find the unsettled animation or timer | engineer | escalate before disabling the suite |
| SDK version mismatch | pin it and document the prerequisite | engineer | fix the setup docs, not the test file |

## Anti-patterns

- do not run the formatter in write mode inside a CI check
- do not treat an empty analyzer report as evidence of quality
- do not let CI become the only place the real commands live

## Handoff and downstream impact

- give planning the gates this baseline earns: `lint` and `ci` for setup, plus `unit` once tests exist and `build` where artifacts are produced
- give implementation the canonical command surface so diffs are verified the way CI will verify them
- give testing the working test lane and an explicit decision on whether goldens are in the gate

## Examples

- **Good fit:** a Flutter repository whose CI runs the formatter in write mode and whose analyzer has never reported anything.
- **Good fit:** bootstrapping a new Flutter app that needs a lint and test gate before UI flows are built.
- **Not a fit:** deciding whether the product should be Flutter, native, or a web app.

## Completion checklist

- the format check fails on drift instead of rewriting the checkout
- a lint rule set is enabled and the analyzer's silence is now meaningful
- the test lane runs and the golden-test decision is explicit
- the QA runbook names SDK prerequisites and commands, and planning has the gate list

## References

- [Gaia delivery policy](../../references/delivery-policy.md)
- [Gaia ownership and conventions](../../references/ownership-and-conventions.md)
- [Default tech stack routing](../fa-engineering-default-tech-stack/SKILL.md)
