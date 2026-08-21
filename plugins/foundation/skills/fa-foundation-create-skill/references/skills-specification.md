# Agent Skills specification

The format specification for Agent Skills (the open standard at https://agentskills.io, which Claude Code follows), plus the Claude-Code-specific extensions. Canonical Claude Code doc: https://code.claude.com/docs/en/skills

## Directory structure

A skill is a directory containing, at minimum, a `SKILL.md` file:

```
skill-name/
├── SKILL.md          # Required: metadata + instructions
├── scripts/          # Optional: executable code
├── references/       # Optional: documentation
├── assets/           # Optional: templates, resources
└── ...               # Any additional files or directories
```

## `SKILL.md` format

The `SKILL.md` file must contain YAML frontmatter followed by Markdown content.

### Frontmatter (portable spec fields)

| Field           | Required | Constraints                                                                                                       |
| --------------- | -------- | ----------------------------------------------------------------------------------------------------------------- |
| `name`          | Yes      | Max 64 characters. Lowercase letters, numbers, and hyphens only. Must not start or end with a hyphen.             |
| `description`   | Yes      | Max 1024 characters. Non-empty. Describes what the skill does and when to use it.                                 |
| `license`       | No       | License name or reference to a bundled license file.                                                              |
| `compatibility` | No       | Max 500 characters. Indicates environment requirements (intended product, system packages, network access, etc.). |
| `metadata`      | No       | Arbitrary key-value mapping for additional metadata.                                                              |
| `allowed-tools` | No       | Space-delimited list of pre-approved tools the skill may use. (Experimental)                                      |

Minimal example:

```yaml
---
name: skill-name
description: A description of what this skill does and when to use it.
---
```

Example with optional fields:

```yaml
---
name: pdf-processing
description: Extract PDF text, fill forms, merge files. Use when handling PDFs.
license: Apache-2.0
metadata:
  author: example-org
---
```

#### `name` field

The required `name` field:

* Must be 1-64 characters
* May only contain unicode lowercase alphanumeric characters (`a-z`) and hyphens (`-`)
* Must not start or end with a hyphen (`-`)
* Must not contain consecutive hyphens (`--`)
* Must match the parent directory name

Valid: `pdf-processing`, `data-analysis`, `code-review`. Invalid: `PDF-Processing` (uppercase), `-pdf` (leading hyphen), `pdf--processing` (consecutive hyphens).

#### `description` field

The required `description` field:

* Must be 1-1024 characters
* Should describe both what the skill does and when to use it
* Should include specific keywords that help agents identify relevant tasks

Good: `Extracts text and tables from PDF files, fills PDF forms, and merges multiple PDFs. Use when working with PDF documents or when the user mentions PDFs, forms, or document extraction.`

Poor: `Helps with PDFs.`

#### `license` field

Specifies the license applied to the skill. Keep it short — either the name of a license (`MIT`) or the name of a bundled license file (`Proprietary. LICENSE.txt has complete terms`).

#### `compatibility` field

* Must be 1-500 characters if provided
* Should only be included if the skill has specific environment requirements
* Can indicate intended product, required system packages, network access needs, etc. (e.g. `Requires git, docker, jq, and access to the internet`)

Most skills do not need the `compatibility` field.

#### `metadata` field

* A map from string keys to string values
* Clients can use this to store additional properties not defined by the Agent Skills spec
* Make key names reasonably unique to avoid accidental conflicts

#### `allowed-tools` field

* A space-delimited list of tools that are pre-approved to run
* Experimental; support varies between agent implementations. In Claude Code the grant applies for the turn that invokes the skill and clears on the next user message.

### Claude Code extensions

Claude Code accepts additional frontmatter fields on **skills** beyond the portable six — these are valid in Claude Code but rejected by strict spec validators (claude.ai skill uploads, the Skills API), so use them only in skills that ship exclusively to Claude Code:

| Field | Purpose |
| --- | --- |
| `argument-hint` | Autocomplete hint for expected arguments, e.g. `[issue-number]`. |
| `disable-model-invocation` | `true` = only the user can invoke (via `/name`); Claude never auto-loads it. For side-effectful workflows like deploys or commits. |
| `user-invocable` | `false` = only Claude can invoke; hidden from the `/` menu. For background knowledge that is not a meaningful user action. |
| `context: fork` | Runs the skill in its own forked subagent context instead of inline; `agent` selects the subagent type and `model` the model. |
| `disallowed-tools` | Tools removed from the pool while the skill is active. |

These are **skill** fields — do not confuse them with sub-agent frontmatter, which has a different schema (see the agents specification in `fa-foundation-create-agent`). Staying within the portable six keeps a skill loadable everywhere; Claude Code accepts all six unchanged.

### Body content

The Markdown body after the frontmatter contains the skill instructions. There are no format restrictions. Write whatever helps agents perform the task effectively.

Recommended sections:

* Step-by-step instructions
* Examples of inputs and outputs
* Common edge cases

Note that the agent will load this entire file once it's decided to activate a skill. Consider splitting longer `SKILL.md` content into referenced files.

## Optional directories

### `scripts/`

Contains executable code that agents can run. Scripts should:

* Be self-contained or clearly document dependencies
* Include helpful error messages
* Handle edge cases gracefully

Supported languages depend on the agent implementation. Common options include Python, Bash, and JavaScript.

### `references/`

Contains additional documentation that agents can read when needed — detailed technical references, form templates, or domain-specific files (`finance.md`, `legal.md`, etc.).

Keep individual reference files focused. Agents load these on demand, so smaller files mean less use of context.

### `assets/`

Contains static resources:

* Templates (document templates, configuration templates)
* Images (diagrams, examples)
* Data files (lookup tables, schemas)

## Progressive disclosure

Skills should be structured for efficient use of context:

1. **Metadata** (~100 tokens): The `name` and `description` fields are loaded at startup for all skills
2. **Instructions** (< 5000 tokens recommended): The full `SKILL.md` body is loaded when the skill is activated
3. **Resources** (as needed): Files (e.g. those in `scripts/`, `references/`, or `assets/`) are loaded only when required

Keep your main `SKILL.md` under 500 lines. Move detailed reference material to separate files.

## File references

When referencing other files in a skill, use relative paths from the skill root, e.g. a link written as `[the reference guide](references/my-reference.md)` from `SKILL.md`, or `scripts/extract.py` for a script the skill runs.

Keep file references one level deep from `SKILL.md`. Avoid deeply nested reference chains. In a Claude Code plugin, links must stay inside the plugin directory — a path escaping the plugin root can never resolve after installation.

## Validation

Use the [skills-ref](https://github.com/agentskills/agentskills/tree/main/skills-ref) reference library to validate a skill against the portable spec:

```bash
skills-ref validate ./my-skill
```

For Claude Code plugins, `claude plugin validate <plugin-dir>` checks skill frontmatter as part of plugin validation.
