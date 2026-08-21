---
name: fa-engineering-containerization
description: Provides compose-first containerization guidance for services that expose an HTTP surface, covering the root compose file, the `.env.example` contract, the canonical `up`/`down`/`test`/`lint`/`build` task set, helper services for email and gateway flows, and runbook alignment. Use it by standing the whole stack up from one command with no undocumented environment, exposing a single canonical QA target, and updating the repository's local QA runbook in the same change. Use it when a repository exposes an HTTP API without a runnable compose stack and behaviour work is planned, since compose is blocking for behaviour changes. It stands the stack up locally and never publishes or redeploys an image — that is `fa-engineering-deploy-chain`.
license: MIT
---

# Gaia Containerization

## Scope and when to use

Use this skill to make a service reproducible before anyone validates it. A
change to behaviour cannot be trusted against a runtime that only exists on one
machine, so a runnable compose stack is **blocking** for behaviour work — not a
follow-up chore.

Use this skill when:

- the repository exposes an HTTP surface and its compose stack is missing or incomplete
- behaviour work is planned: new, changed, or removed use cases
- a QA gate enforces a container-first runtime before validation
- the stack runs but only after undocumented environment variables or manual steps

Do not use this skill when:

- the stack already comes up cleanly and the open question is what to test — that is `fa-engineering-testing`, the general testing authority
- the choice of language, framework, or database is still open — settle it with `fa-engineering-default-tech-stack` first
- **the gap is the language baseline rather than the runtime** — no enforced formatter, no runnable test command, ambiguous package management. Use that language's stack skill (`fa-engineering-stack-*`) first; containerizing a project whose own build is not yet trustworthy just moves the problem inside an image
- the work is executing checks against a stack that already runs — use `fa-engineering-manual-regression`

## Required inputs

- the detected service stack and any existing container definitions
- runtime dependencies: databases, caches, queues, object stores, mail
- the environment variables and secrets the service genuinely needs to boot
- the repository's existing task runner and its local QA runbook

## Owned outputs

- a `docker-compose.yml` at the repository root that starts the whole stack
- a root `.env.example` carrying every variable the stack reads
- the canonical task set: `up`, `down`, `build`, `test`, `lint`
- a local QA runbook updated in the same change, naming the canonical QA target

## Decision tree

- If the repository already has container conventions, extend them rather than replacing them.
- If a dependency is only needed in production, leave it out; keep the stack to what the service needs to run and be tested.
- If a required secret has no safe default, place a `CHANGE_ME` placeholder in `.env.example`, raise a blocker, and keep completion gated.
- If flows send email during validation, add a mail-catcher service — otherwise verification and reset paths cannot be proven locally at all.
- If several services are fronted by a gateway, that gateway is the single QA target; do not advertise a second one.

## Core workflow

1. Identify the service, its dependencies, its ports, and its health and migration steps.
2. Add or fix the service's container build: deterministic base images, logs to stdout and stderr, a healthcheck where feasible.
3. Write the root compose file with the service, only the dependencies it needs, and a gateway or helper services where validation requires them.
4. Write `.env.example` with safe non-secret defaults, `CHANGE_ME` placeholders for secrets, and connection strings that use compose service names.
5. Create or extend the task set so `up`, `down`, `build`, `test`, and `lint` all work from a clean checkout.
6. Bring the stack up, confirm the service answers on the canonical QA target, and confirm dependencies are reachable.
7. Update the local QA runbook and any skill or convention this change invalidates; documentation drift here is blocking.

## The compose contract

- **The task names are an interface, not a convenience wrapper.** Downstream skills, CI jobs, and every later agent call `up`, `down`, `build`, `test`, and `lint` by name. Renaming one silently breaks callers that never read this repository.
- **The task runner exports environment the checkout genuinely needs.** A bare toolchain invocation can fail where the task succeeds. Never conclude a repository is broken from a raw build or test call that bypassed the task set.
- **Runnable by default means runnable by a stranger.** If `up` works only because the author's shell already holds three variables, the stack is not runnable, and every future validation session pays that cost again.
- **`.env.example` must list every variable the stack reads, including the ones with defaults.** A variable that exists only in the compose file is invisible to the person trying to run it, and its absence surfaces as an unrelated crash at boot.
- **Never commit real secrets, and never inline them in compose.** Placeholders plus a blocker beat a stack that runs today and leaks tomorrow.
- **One canonical QA target.** When a gateway fronts the stack, publishing raw service ports alongside it invites sign-off against a surface that is not the one under test — a different bundle, a different proxy path, a different config.
- **Development-only helper switches default to on in compose and must never reach production.** Stub modes, time-travel modes, and QA shortcuts belong to the local stack; state that plainly in the runbook so nobody promotes the compose defaults.

## Failure recovery

| Failure mode                    | Recovery                                                     | Owner    | Escalation             |
| ------------------------------- | ------------------------------------------------------------ | -------- | ---------------------- |
| stack will not start            | fix compose or the container build before any validation      | engineer | planner if scope grows |
| required secret unavailable     | placeholder in `.env.example`, blocker raised, work continues | engineer | the human owner        |
| dependency unreachable in stack | correct service names, networks, and healthcheck ordering     | engineer | architect              |
| runbook contradicts the stack   | update the runbook in the same change; drift is blocking      | engineer | architect              |

## Anti-patterns

- do not hardcode secrets, hostnames, or ports where an environment variable belongs
- do not add services the stack does not need to run and be tested
- do not ship a compose file whose only working invocation lives in someone's shell history
- do not leave the QA runbook describing a runtime that no longer exists
- do not treat "it runs locally without compose" as satisfying the prerequisite

## Handoff and downstream impact

- give `fa-engineering-manual-regression` a reachable canonical QA target and the credentials story
- give `fa-engineering-e2e-testing` a stack the browser lane can depend on, and a base URL it can read from the environment
- give release the exact commands that reproduce the runtime
- give architecture any dependency the stack revealed that the design never documented

## Examples

- **Good fit:** stand up compose, `.env.example`, and the five tasks for an HTTP API before its first use-case change lands.
- **Good fit:** add a mail-catcher and a gateway so account-verification flows can be validated against one canonical URL.
- **Not a fit:** decide which validation layers the resulting change needs; that belongs to `fa-engineering-testing`.

## Completion checklist

- a clean checkout runs `up` and reaches the service with no undocumented setup
- `.env.example` covers every variable the stack reads, with secrets as placeholders
- `up`, `down`, `build`, `test`, and `lint` all exist and all work
- the canonical QA target is documented and is the only one advertised
- the local QA runbook matches the stack as it now is

## References

- [Gaia delivery policy](../../references/delivery-policy.md)
- [Gaia ownership and conventions](../../references/ownership-and-conventions.md)
