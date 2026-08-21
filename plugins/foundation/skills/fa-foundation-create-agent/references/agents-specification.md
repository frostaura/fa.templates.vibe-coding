# Claude Code sub-agents specification

Reference for authoring sub-agent definition files. A sub-agent is a Markdown file: YAML frontmatter between `---` markers, followed by the agent's system prompt as the Markdown body. Canonical documentation: https://code.claude.com/docs/en/sub-agents

## File locations and precedence

| Location | Scope | Precedence |
| --- | --- | --- |
| Managed settings | Organization-wide | Highest |
| `--agents` CLI flag (JSON) | Current session only | 2 |
| `.claude/agents/` | Current project | 3 |
| `~/.claude/agents/` | All projects (user) | 4 |
| Plugin `agents/` directory | Wherever the plugin is enabled | Lowest |

Project and user directories are scanned recursively; subfolders do not affect identity there — only the `name` field matters. In a **plugin**, the subfolder path becomes part of the scoped identifier: `agents/review/security.md` in plugin `my-plugin` loads as `my-plugin:review:security`. On duplicate names, the definition closest to the working directory wins.

## Frontmatter

Required:

| Field | Rules |
| --- | --- |
| `name` | Unique identifier: lowercase letters and hyphens. Must not contain `:` (reserved for plugin-scoped IDs) or start with `-`. The filename does not need to match. |
| `description` | When Claude should delegate to this agent. This is the routing signal — Claude reads it to decide whether to invoke the agent, so write it decision-grade: what the agent does, when to use it, when not to. Phrases like "use proactively" or "use after X" encourage delegation. |

A file with no `name` is treated as documentation; a file with `name` but no `description`, or with invalid YAML, is **silently skipped** (reason goes to the debug log only). Always verify both fields parse.

Optional (commonly used):

| Field | Rules |
| --- | --- |
| `tools` | Allowlist of tool names. **Omit it to inherit every tool available to subagents, including all MCP tools — this is the robust default.** An empty list, or a list where no entry resolves to a real tool, makes Claude Code refuse to launch the agent ("Agent would be spawned with zero tools", v2.1.208+). |
| `disallowedTools` | Denylist removed from the inherited set. The right mechanism for deliberately restricted agents (e.g. a reviewer that must not edit or execute). Applied before `tools` when both are set. |
| `model` | `sonnet`, `opus`, `haiku`, a full model ID, or `inherit` (the default). Add it only when the override is deliberate. |

Other optional fields exist (`permissionMode`, `maxTurns`, `skills`, `mcpServers`, `hooks`, `memory`, `background`, `effort`, `isolation`, `color`) — see the canonical doc. Note that `hooks`, `mcpServers`, and `permissionMode` are **ignored in plugin agents**.

Copilot-era keys (`target`, `disable-model-invocation`, `user-invocable`, `infer`, `mcp-servers`, tool aliases like `read`/`edit`/`execute`) are not part of this schema. Unknown keys are ignored at load time, but `claude plugin validate` warns on them — do not carry them.

## Tool naming

- **Built-in tools are PascalCase**: `Read`, `Write`, `Edit`, `NotebookEdit`, `Bash`, `Grep`, `Glob`, `WebFetch`, `WebSearch`, `TodoWrite`, `Skill`, `Task`.
- **MCP tools**: `mcp__<server>__<tool>` (e.g. `mcp__github__list_issues`).
- **MCP server patterns**: `mcp__<server>` or `mcp__<server>__*` matches every tool from that server; `mcp__*` (in `disallowedTools` only) matches all MCP tools.
- Lowercase aliases (`read`, `edit`, `search`, `execute`, `agent`) and `<server>/<tool>` slash-prefixed MCP names are Copilot conventions — in Claude Code they resolve to nothing, and a `tools` list made entirely of them refuses to launch.

Because MCP tool prefixes vary with how a server is registered (`mcp__plugin_<plugin>_<server>__…` for plugin-bundled servers), prefer omitting `tools` entirely over hardcoding fragile MCP prefixes, and express restrictions with `disallowedTools` on built-in names.

## Recipes

Full-capability worker (inherits everything, including MCP):

```markdown
---
name: fa-engineer
description: Implements planned tasks... Use after planning is complete.
---
```

Read-only reviewer (everything except mutation and execution):

```markdown
---
name: fa-reviewer
description: Reviews diffs for correctness... Use before merge.
disallowedTools: [Write, Edit, NotebookEdit, Bash]
---
```

## Body

The Markdown body is the agent's system prompt. State the mission, its boundaries (what the agent must not do), delegation rules, and anti-patterns explicitly. Non-fork subagents start with a fresh context — they do not see the parent conversation — so the body plus the invocation brief must be self-sufficient.

### Design for parallel composition

Sub-agents are routinely launched several at a time — a coordinator issues multiple Task-tool invocations in a single message and awaits only the results that gate its next decision. Author for that:

- **Single responsibility.** Keep the scope narrow enough that multiple instances, or several sibling agents, can fan out concurrently without contending for the same work.
- **Explicit anti-mission.** The body's "must not do" list is what keeps concurrent siblings out of each other's scope — write it as deliberately as the mission.
- **Disjoint file scopes.** Agents that edit the same repository concurrently must each own a disjoint set of files; when scopes must overlap, isolate each agent in its own git worktree and merge deliberately. Two agents writing in one scope overwrite each other.
- **Background-friendly.** Long-running work can run as a background subagent while the coordinator continues other branches; note in the body if the role suits that.

## Validation

`claude plugin validate <dir>` checks agent files (warns on unknown frontmatter keys); `claude --plugin-dir <plugin>` smoke-tests a plugin locally. An agent that lists invalid tools fails at launch, not at load — test that agents actually spawn.
