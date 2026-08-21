---
name: fa-foundation-create-skill
description: Provides the house rules for a SKILL.md that actually fires — naming, the frontmatter contract, a description written as a trigger rather than a summary of the body, the standard section shape, references kept one level deep, and the prior decision of whether a new skill is warranted at all. Use it by reading the skills specification, then drafting or repairing the file against it — directory name equal to the frontmatter name, description measured against the 1024-character cap, and the sibling skill this one must not displace named in a closing negative. Use it when adding a skill, when a skill never triggers or triggers on the wrong work, when a trap-bearing procedure has been repeated often enough to deserve one, or when consolidating skills that have drifted into each other. It never authors an agent definition — that is `fa-foundation-create-agent`.
license: MIT
---

# Gaia Skills

## Scope and when to use

Use this skill to keep Gaia's procedural layer reusable, concise, and aligned to
the contract and architecture.

Use this skill when:

- adding or revising a skill
- auditing duplicated or drifting skill guidance
- improving skill descriptions, section templates, or references
- deciding whether repeated work deserves a new skill or a stronger existing one

Do not use this skill when:

- the change is really a role-definition issue
- the missing step is still an architecture or contract decision
- the procedural guidance belongs in a reference file rather than `SKILL.md`

## Required inputs

- the current skill roster and shared references
- the procedural gap or maintenance problem being addressed
- naming, metadata, and specification constraints
- any repeated patterns, overlaps, or references worth consolidating

## Owned outputs

- valid skill definitions with strong descriptions and clear triggers
- procedural guidance that tells roles how to do recurring work
- trimmed duplication between shared references and local skills
- documented reasoning for adding, merging, or rejecting a skill

## Decision tree

- If the change is global workflow policy, update `AGENTS.md` or a shared reference.
- If a reference can hold detailed material better than the main skill body, move it out of `SKILL.md`.
- If an existing skill can absorb the new behavior cleanly, revise it instead of adding a new skill.
- If a new skill is justified, define its scope, outputs, and anti-patterns before finalizing the metadata.
- If the skill would sit at the widest scope, confirm it is worth the cost to **every** session there: a skill at the top of a cascade is loaded by all of them forever, so one that only a single project needs is a tax on the rest.
- If a parent scope already carries the skill, reference it from the child — never mirror it downward; and never park a narrow, one-project procedure at the widest scope. Place each skill at the narrowest scope that needs it.

## Core workflow

1. Read the skills specification and the current skill roster, then read the skill you are changing end to end — frontmatter included — **and its neighbours in the same directory end to end**. A scope's procedural layer is coherent or incoherent only as a whole; search locates a skill, it never assesses one.
2. Audit the skill against the code and docs it describes, not against its own prose: if it names a file, a command, a threshold or a config key, verify each one still exists as named.
3. Decide whether the change belongs in a skill, a reference, or the contract layer.
4. Write or revise the skill with clear use-when, decision-tree, procedure, and recovery guidance.
5. If the skill describes a multi-step workflow, state which steps parallelize (independent scopes, no shared state — fan them out concurrently) and which are barriers that gate later steps; a workflow written as an implicit serial relay hides parallelism the executor could use.
6. Keep descriptions specific enough to help discovery while staying within spec limits, and keep frontmatter within the portable spec fields (name, description, license, compatibility, metadata, allowed-tools) unless a Claude-Code-only extension is deliberately needed; give every skill a `license`.
7. Keep references focused and use them to avoid bloating the main skill body; in a plugin, keep every link inside the plugin directory.

## The bar for a new skill

All four, or do not create it. Sprawl is this layer's failure mode, not scarcity:

1. **Repeated.** It has happened more than twice, or will obviously recur.
2. **Procedural.** It is a *how*, with steps and an order. A fact belongs in `MEMORY.md`; a rule belongs in the scope's instruction file; a procedure is a skill.
3. **Trap-bearing.** There is a non-obvious way to get it wrong that has already cost real work. A skill whose content is "do the obvious thing carefully" earns nothing and taxes every session that loads it.
4. **Uncovered.** No skill at this scope or above already fires for it. Extend the existing one first.

Only once all four clear, decide what the skill owns as an artifact and what detail belongs in a reference file rather than the body.

## Failure recovery

| Failure mode            | Recovery                                             | Owner      | Escalation                                   |
| ----------------------- | ---------------------------------------------------- | ---------- | -------------------------------------------- |
| duplicated guidance     | centralize it in shared references or the contract   | maintainer | re-audit neighboring skills                  |
| weak description        | rewrite it for invocation quality within spec limits | maintainer | compare with neighboring skills              |
| procedural gap          | add or expand the right skill                        | maintainer | reject if the pattern is too narrow to reuse |
| skill-contract mismatch | update the skill after the contract is current       | maintainer | involve architecture if the workflow changed |
| stale skill             | delete it without ceremony — a stale skill is worse than a missing one because it fires with authority — and record the supersession in the surviving skill so the knowledge is not simply lost | maintainer | re-audit the roster for other skills describing work nobody does |

## Anti-patterns

- do not move global workflow rules into every skill body
- do not create a new skill for a one-off edge case
- do not leave references so deep that the main skill becomes unusable
- do not write a `description` that summarizes the body; it is a **trigger**, phrased in the vocabulary a caller would actually use — a skill that never fires is dead weight however good its contents
- do not inline policy that belongs in a canonical reference; the inlined copy becomes a second source of truth and drifts against the original without announcing it
- do not end a maintenance pass with every skill still present and slightly longer; deleting and consolidating are expected outcomes, and neither is visible from a search result
- do not edit a vendored skill set — one copied in from an upstream plugin — as though it were locally authored; fix it upstream, or record the divergence deliberately

## Handoff and downstream impact

- tell maintainers whether the change belongs in the contract, a skill, or a shared reference
- tell agent maintainers when a role file must change to invoke the revised skill correctly
- tell architecture when skill maintenance implies a workflow-model change
- tell engineering when the skill rewrite needs direct file edits in the repo

## Examples

- **Good fit:** rewrite Gaia's seven skills into a common procedural template after the contract and role layers are clarified.
- **Good fit:** decide whether new line-count and description standards belong in each skill or in a shared rule.
- **Not a fit:** decide who owns a delivery branch; that is role and planning work, not skill maintenance.

## Completion checklist

- the skill has a clear scope, procedure, and recovery path
- multi-step workflows name their parallel steps and their barriers explicitly instead of implying a serial relay
- descriptions stay within spec and still help discovery
- duplicated guidance has been centralized where appropriate
- references support the skill without making the main file too thin

## References

- [Skills specification](references/skills-specification.md)
