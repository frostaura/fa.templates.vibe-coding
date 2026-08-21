---
name: fa-foundation-create-agent
description: Provides the house rules for an agent definition that actually gets invoked and stays in its lane — one role per file, a description carrying what the agent is for and when to reach for it, an explicit anti-mission, an output contract, and a tool scope no wider than the role needs. Use it by reading the agent specification, then drafting or repairing the frontmatter and body against it, and checking the new role against every agent already installed for overlap. Use it when adding an agent, when an agent never fires or fires on the wrong work, when two agents claim the same scope, when a definition's tools no longer match what it does, or when rationalizing a set of agents that has sprawled. It never authors a skill — that is `fa-foundation-create-skill` — and it never audits a whole definition directory, which is the skills auditor's.
license: MIT
---

# Custom Agents

## Scope and when to use

Use this skill to keep Gaia's custom agents clear, distinct, and aligned with
the global contract.

Use this skill when:

- adding or revising a custom agent definition
- auditing role overlap or ambiguous tool scopes
- rewriting agent descriptions or instructions for better invocation quality
- aligning agent files to a new shared contract or architecture update

Do not use this skill when:

- the missing step is still an architecture decision
- an existing role can be extended cleanly without creating a new one
- the change belongs in reusable procedural guidance instead of a role definition

## Required inputs

- the current agent roster and `AGENTS.md`
- the purpose, scope, and intended users of the proposed role change
- any deliberate tool restrictions (`disallowedTools`) or model overrides the role needs
- any overlapping definitions or maintenance pain the change is meant to solve

## Owned outputs

- valid agent definitions with clear names and scopes
- decision-grade descriptions that explain when to invoke or avoid each role
- explicit role boundaries, delegates, and anti-patterns
- overlap decisions that explain why a role was added, merged, or rejected

## Decision tree

- If the gap is global policy, update `AGENTS.md` instead of a role file.
- If an existing role can absorb the behavior cleanly, revise it rather than create a new role.
- If a truly new role is needed, define its mission, adjacent roles, tool posture, and failure boundaries explicitly.
- If tool posture is unclear, decide it before considering the definition complete: the default is to omit `tools` and inherit everything (including MCP tools); a role that must not mutate or execute expresses that with `disallowedTools`.
- If the role is meant to be read-only, **state the limit honestly in the body, in the imperative**. A shell or bash grant can run anything, so a "read-only" agent is read-only *by instruction*, not by tool grant; denying the edit tools does not close that hole, and a role that relies on the grant alone will eventually mutate something.

## Core workflow

1. Read the agent specification and current roster before editing anything.
2. Identify whether the change is global contract work or local role work.
3. Test the proposal against neighboring roles to avoid overlap.
4. Write or revise the agent with clear mission, use-when rules, delegates, and anti-patterns.
5. State the role's concurrency rule in the `description` if it has one — e.g. one instance per scope, because two instances in the same subtree overwrite each other's edits.
6. End the definition with a hard return contract and a line budget for what the role hands back.
7. Confirm that the role improves routing instead of adding organizational noise.

## Design for parallel composition

Agents are routinely launched several at a time — multiple Task-tool invocations
in a single coordinator message — so author every role to fan out safely: keep it
single-responsibility, state explicitly what it must NOT own, and give concurrent
agents disjoint file scopes. Two agents writing in one scope overwrite each other;
where scopes must overlap, isolate each in its own git worktree and merge
deliberately. The full parallel-composition rules are in the agent specification.

## The bar for a new agent

Narrower than the bar for a new skill. Both, or extend an existing role:

- **The fan-out actually recurs.** A role dispatched once is a brief, not an agent.
- **The brief is long enough that re-typing it is where the drift comes from.** That is the definition's entire reason to exist: every branch of a fan-out inherits the same method, traps and prohibitions instead of whatever the coordinator happened to remember.

If one existing agent plus a different scope line covers the case, that is the answer — a per-project variant of a per-scope agent is sprawl, not precision. Only then decide the invocation signal, any deliberate tool restriction, and how overlap will be prevented.

## Failure recovery

| Failure mode               | Recovery                                     | Owner      | Escalation                                          |
| -------------------------- | -------------------------------------------- | ---------- | --------------------------------------------------- |
| overlap with existing role | merge or narrow the proposal                 | maintainer | reject new role if still redundant                  |
| unclear tool scope         | inherit by default; restrict via `disallowedTools` | maintainer | block adoption until explicit                       |
| weak description           | rewrite for invocation quality               | maintainer | compare against neighboring roles                   |
| contract mismatch          | update local role or contract as appropriate | maintainer | involve architecture if the operating model changed |

## Anti-patterns

- do not create a new agent because one current role feels temporarily overloaded
- do not leave delegation rules implied, or restrict tools without stating why
- do not hardcode fragile MCP tool prefixes in `tools` when omitting the field inherits them robustly
- do not store global policy inside every role file
- do not let role descriptions stay too short to drive good invocation choices
- do not author a role so broad that two concurrent instances would collide on the same files or decisions
- do not let a role return a file dump or a raw transcript; an agent that hands back everything it read defeats the point of delegating to it — make it return findings against its contract
- do not patch a changed method into the invocation brief; when how the work is done changes, edit the agent definition, because a brief written from memory drifts from the definition immediately and silently

## Handoff and downstream impact

- tell maintainers whether the change belongs in `AGENTS.md` or only in local role files
- tell engineering when agent maintenance requires direct file edits after design approval
- tell architecture when a role change implies a workflow-model change
- tell skill maintainers when procedural guidance must be updated to match a role rewrite

## Examples

- **Good fit:** rewrite all six Gaia agents to a shared execution template after the contract layer changes.
- **Good fit:** decide whether a proposed analyzer role is actually needed or should be folded into intake and architecture.
- **Not a fit:** define planning procedures or architecture artifacts; those belong to skills and docs.

## Completion checklist

- every changed role has a clear mission and anti-mission
- the role is scoped for safe parallel fan-out: siblings can run concurrently against it without scope collision
- tool posture (inherit-all vs deliberate `disallowedTools` restriction) and delegation rules are explicit
- every changed role parses (valid YAML, `name` + `description` present) and would actually launch — no invalid tool names
- the definition ends with a hard return contract and a line budget, and any read-only posture is stated imperatively in the body rather than assumed from the tool grant
- role overlap has been reduced rather than increased
- local role files align with the global contract

## References

- [Agent specification](references/agents-specification.md)
