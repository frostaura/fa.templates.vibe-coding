---
name: fa-engineering-stack-python
description: Provides the Python repository baseline - a single-tool `ruff` lint and format gate that fails rather than merely reports, `pytest` wired to a command CI calls verbatim, and an explicit decision about which packaging manager already owns the project. Use it by auditing a Python repository against the baseline table and closing only its real gaps, deferring to the interpreter pin, dependency manager and test layout the repository already established. Use it when a Python repository formats on save but never fails a build, has a test runner nobody can invoke the same way twice, or has two dependency managers half-installed, and before feature work that needs a gate to mean something. It never migrates a project between packaging tools, and never chooses Python over another language for a service; that call belongs to `fa-engineering-default-tech-stack`.
license: MIT
---

# Stack Baseline: Python

## Scope and when to use

Use this skill to give a Python repository a foundation where formatting drift, lint findings
and test failures each stop a build, and where one documented command runs each of them.

Use when:

- a Python repository has no enforced lint or format gate, or has one that only warns
- tests exist but there is no single canonical way to run them
- packaging is ambiguous — two managers' lockfiles present, or none
- a new Python service or library is being bootstrapped before feature work
- CI passes while the same checks fail locally, or the reverse

Do not use when:

- the choice *between* Python and another stack is still open — that is
  `fa-engineering-default-tech-stack`, which routes here once Python is settled
- the real problem is test *strategy* rather than a runnable test command — that is
  `fa-engineering-testing`

## Required inputs

- the dependency manifest and any lockfiles present
- the interpreter version targeted, and where it is pinned
- the existing command surface (`Makefile`, `justfile`, `tox.ini`, manifest scripts)
- the CI workflow, and whether the project ships a service, a library, or both

## Owned outputs

- a lint and format configuration that fails on drift rather than reporting it
- one runnable command per gate, identical locally and in CI
- a test entry point that works from a clean checkout
- a recorded packaging decision — which manager owns the project, and why

## Decision tree

- If a working manager already owns the project (`uv`, `poetry`, `pdm`, or `pip` +
  `requirements.txt`), **keep it** — migrating packaging is a project of its own, never a
  side effect of adding a gate.
- If two managers are half-present, resolve that first; every other gate is unreliable until
  one owns dependency resolution.
- If lint and format are separate tools and both work, leave them; adopt the single-tool
  baseline only where the slot is empty or broken.
- If the project exposes an HTTP surface, containerization is `fa-engineering-containerization`.
- If it is a library, skip every service-shaped step rather than inventing one.

## Core workflow

1. Read the manifest, lockfiles and CI workflow first, and write down which manager and
   interpreter version own this project today.
2. Resolve packaging ambiguity if any exists. One manager, one lockfile, committed.
3. Establish the lint and format gate — prefer one tool covering both (`ruff check`,
   `ruff format --check`), configured in the project's own manifest rather than a new file.
4. Establish the test gate: `pytest` runnable from a clean checkout. Where no tests exist,
   add a scaffold that imports the package and asserts something real.
5. Expose each gate as one command, and have CI call **that command**, not a re-typed equivalent.
6. Record the gates and the environment setup where a newcomer will find them.

## Baseline contract and its traps

| Slot | Baseline | The trap |
| --- | --- | --- |
| Format | `ruff format --check` in CI | Formatting on save makes drift invisible locally while CI is the only thing that ever objects. The check must run in the gate, not just the editor. |
| Lint | `ruff check` with rules selected explicitly | An empty or default rule set produces a clean report that means nothing. A linter with nothing enabled is worse than none — it reads as passing. |
| Tests | `pytest` from a clean checkout | Tests that pass only because the developer's shell has the project on `PYTHONPATH` fail for everyone else. Install the package (editable) rather than relying on ambient path. |
| Packaging | Exactly one manager, lockfile committed | Two half-installed managers give two different dependency graphs, and which one you get depends on who ran what last. This is the single most common cause of "works on my machine" here. |
| Interpreter | Pinned, and the same pin CI uses | A floating interpreter turns an upstream Python release into a surprise build break with no change on your side. |

**The failure this baseline exists to prevent:** every slot above can be filled in a way that
produces green output while checking nothing — a formatter that only runs in the editor, a
linter with no rules enabled, tests passing on ambient path. From the CI summary all three
look exactly like a healthy repository. Make each gate fail on purpose once and confirm the
build goes red. **A gate nobody has ever seen fail is not known to work.**

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
| --- | --- | --- | --- |
| Lint gate reports nothing | Enable an explicit rule set; re-run against a known violation | maintainer | if the rule set is contested, settle it before enabling |
| Tests pass locally, fail in CI | Install the package rather than relying on `PYTHONPATH`; compare interpreter pins | maintainer | escalate if CI's interpreter cannot match the project's |
| Two dependency managers present | Pick the one the lockfile and CI already use; remove the other's artefacts | maintainer | human owner if the choice is genuinely open |
| Format check fails everywhere at once | Run the formatter as one isolated commit, then enable the gate — never mix a formatting sweep with a behaviour change | maintainer | — |
| A gate cannot be made to fail on purpose | Treat it as not installed; it is not wired to the command CI runs | maintainer | re-read the CI workflow first |

## Anti-patterns

- Do not migrate packaging managers as a side effect of adding a gate.
- Do not commit a virtualenv, or a lockfile from a manager the project does not use.
- Do not let CI re-type a command instead of calling the project's own entry point — the two
  drift, and the local one is the copy nobody notices is wrong.
- Do not add a test scaffold that asserts `True`. It makes the gate permanently green.

## Handoff and downstream impact

- Tell planning which gates now exist, so branch plans can require them by name.
- Tell the service owner if containerization is still open — `fa-engineering-containerization`.
- Tell the testing owner what the scaffold does and does not cover; a runnable gate is not a
  test strategy.
- A stack differing from the organisation's default must be declared in the repository's own
  instruction file with its reason.

## Examples

- **Good fit:** CI is green while `ruff` has no rules enabled and the only tests import
  nothing — close all three slots and prove each can fail.
- **Good fit:** both `poetry.lock` and `requirements.txt` present, resolution differing by
  contributor — settle on one manager before touching anything else.
- **Not a fit:** deciding whether a service should be Python at all. That is
  `fa-engineering-default-tech-stack`; this skill starts once the language is settled.

## Completion checklist

- one dependency manager owns the project, with its lockfile committed
- lint and format each run in CI and have each been observed failing on purpose
- `pytest` runs from a clean checkout without ambient path assumptions
- the interpreter pin is explicit and matches CI
- every gate is one command, CI calls that command, and both are written down where a
  newcomer will find them

## References

- [Ownership and conventions](../../references/ownership-and-conventions.md)
- [Delivery policy](../../references/delivery-policy.md)
