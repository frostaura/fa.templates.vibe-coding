---
name: fa-engineering-default-tech-stack
description: Provides Gaia's default full-stack baseline with React, TypeScript, Redux Toolkit, Tailwind CSS, and shadcn/ui on the frontend plus .NET, EF Core, PostgreSQL, and MCP exposure on the backend. Use it by adopting the documented baseline (versions, scaffolds, references) for any new app or unspecified-stack request, and by declaring the stack explicitly before planning when the request leaves it implicit. Use it when the request or repo leaves stack choice open, when bootstrapping a new application's stack from scratch, or when standardizing an existing codebase onto Gaia's preferred platform. It decides which stack a repository uses and never implements that stack's lint, test and command baseline, which belongs to the matching `fa-engineering-stack-*` skill.
license: MIT
compatibility: Requires a project where React/TypeScript, .NET, PostgreSQL, and MCP exposure are acceptable defaults.
---

# Gaia Default Tech Stack

## Scope and when to use

Use this skill when Gaia needs a concrete default application stack instead of
leaving frontend, backend, UI-system, and API-surface choices implicit.

Use this skill when:

- the user did not specify a tech stack for a new application
- an existing repo needs to be standardized onto Gaia's preferred stack
- frontend UI work needs a consistent design-system baseline before feature work
- backend API work needs a default persistence and MCP exposure model

Do not use this skill when:

- the user or repo already mandates a different stack
- the task only needs local implementation details within an existing standard
- architecture still needs to decide whether Gaia's default stack is appropriate

## Required inputs

- the user request and any explicit platform constraints
- current repo signals about frontend, backend, database, and API topology
- any migration constraints for existing codebases
- confirmation of whether Gaia defaults are allowed or overridden

## Owned outputs

- an explicit default-stack decision when the request leaves it open
- a frontend baseline centered on React, TypeScript, Redux Toolkit, Tailwind, and shadcn/ui
- a backend baseline centered on .NET, EF Core, PostgreSQL, and MCP exposure
- a phase-based modernization path for UI-system standardization

## Decision tree

- If the user specified a stack, respect it unless they explicitly ask to standardize on Gaia defaults.
- If the repo already has an approved stack, use this skill only to judge migration or gap-filling work.
- If the request lacks a stack choice, declare Gaia's default stack explicitly before planning or implementation.
- If the frontend lacks a coherent design system, start with the foundation phase before component migration.
- If the backend exposes APIs without MCP coverage, add or plan an MCP server surface instead of treating it as optional.
- If the repo deviates from the declared baseline, require the deviation to be written into the repo's own instruction file **together with its reason**. That is the whole enforcement mechanism: an undeclared deviation is a finding regardless of how good the technical argument behind it is, because the argument is not on file for the next person to read. A *declared* deviation — on-device work that needs a native UI toolkit, a sidecar in another language for a library that only exists there — is legitimate and closes the finding.

## Core workflow

1. Confirm that Gaia's default stack is actually needed and not overridden.
2. Declare the default frontend and backend baselines explicitly in the plan or architecture artifact.
3. Apply the frontend baseline in phases: foundation first, component migration second, anti-pattern cleanup third.
4. Use standardized shadcn/ui primitives and semantic Tailwind tokens instead of bespoke UI patterns.
5. Keep backend delivery on the latest stable .NET, EF Core, and PostgreSQL baseline unless the repo already says otherwise.
6. Ensure API capabilities are reachable through an MCP server surface, typically an `/mcp` endpoint.
7. Hand off implementation or validation with the baseline and any approved deviations stated plainly.

## Default baseline

- Frontend: latest stable React + TypeScript, Redux Toolkit, Tailwind CSS, shadcn/ui, and a centralized semantic design system
- Backend: latest stable .NET API, EF Core, PostgreSQL, and MCP server exposure for API capabilities
- Delivery rule: standardize incrementally, page by page or component by component, instead of rewriting the full app in one pass

## Failure recovery

| Failure mode                  | Recovery                                              | Owner                 | Escalation                                 |
| ----------------------------- | ----------------------------------------------------- | --------------------- | ------------------------------------------ |
| stack already fixed elsewhere | treat Gaia defaults as non-applicable                 | architect or engineer | document the override                      |
| design-system sprawl          | return to the foundation phase and token model        | engineer              | re-plan migration order                    |
| backend lacks MCP exposure    | add the MCP branch or block completeness claims       | engineer or planner   | escalate if platform constraints forbid it |
| migration scope explodes      | phase the work more narrowly instead of broad rewrite | planner               | route back to process if ownership changed |

## Anti-patterns

- do not treat Gaia's default stack as a reason to ignore explicit user or repo constraints
- do not migrate the entire UI surface in one unscoped rewrite
- do not keep arbitrary design tokens, spacing, and component patterns after standardization begins
- do not ship HTTP APIs without an MCP exposure plan when Gaia defaults are in effect
- do not silently normalize a deviation by building around it; write it down with its reason, or bring the repo back to the baseline
- do not default to whatever stack spins up fastest in the moment — fragmentation across a portfolio costs more, for longer, than stack discipline does up front

## Handoff and downstream impact

**This skill is the routing layer.** It chooses *between* stacks; it does not implement any
one of them. Once the language is settled, hand the repository to the baseline that owns it —
`fa-engineering-stack-web-ts`, `fa-engineering-stack-dotnet-api`,
`fa-engineering-stack-dotnet-maui`, `fa-engineering-stack-flutter` or
`fa-engineering-stack-python` — each of which owns that language's lint, format, test and
command surface and nothing else.

- The container runtime is separate from every language baseline: `fa-engineering-containerization`.
- Publishing a built image through an existing pipeline is `fa-engineering-deploy-chain`.
- Tell planning which gates now exist so branch plans can require them by name.
- Where a repository deviates from the baseline, the deviation is written into that
  repository's own instruction file with its reason — an undeclared deviation is a finding
  regardless of how good the technical argument is.

## Examples

- **Good fit:** a user asks for a new full-stack app without naming frameworks, so Gaia needs a concrete default baseline.
- **Good fit:** an existing React app needs a structured migration to shadcn/ui and semantic Tailwind tokens.
- **Not a fit:** a repo already standardizes on another approved stack and only needs feature work.

## Completion checklist

- the baseline in effect is stated explicitly rather than left implicit in the code
- every deviation from that baseline is declared in the repo's own instruction file, with its reason, before the work is called done
- frontend work uses shadcn/ui primitives and semantic tokens rather than bespoke patterns
- API capabilities are reachable through an MCP surface, or the gap is planned and named
- migration work is phased rather than delivered as one unscoped rewrite

## References

- [Default stack baseline](references/default-stack-baseline.md)
- [Gaia ownership and conventions](../../references/ownership-and-conventions.md)
