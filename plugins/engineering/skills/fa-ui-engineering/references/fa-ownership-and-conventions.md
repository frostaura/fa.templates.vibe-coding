# Gaia Ownership and Conventions

Use this reference when deciding which skill should own a change, which repository artifact should be updated, or which naming convention to apply.

## Skill ownership

| Skill | Primary ownership |
|---|---|
| `fa-process` | Complexity classification, execution path, coordination, handoffs, and closure expectations |
| `fa-architecture` | `docs/architecture`, architecture drift resolution, and README sync after architecture changes |
| `fa-planning` | The repository's `gaia_plan.md`, branching plan structure, dependencies, QA checkpoints, release gates, and proof expectations |
| `fa-engineering` | Planned implementation work, rapid iteration, implementation stabilization, and engineering standards during delivery |
| `fa-ui-engineering` | React UI implementation, design-system enforcement, token-only styling, responsive layout discipline, and cleanup of legacy arbitrary values |
| `fa-testing` | Formal test strategy, test artifacts, regression coverage, and testing evidence |
| `fa-default-tech-stack` | Default frontend and backend stack selection, design-system standardization, and MCP-ready API baseline when the request or repo does not specify another stack |
| `fa-skills` | Skill naming, skill structure, `SKILL.md` maintenance, and repository skill conventions |
| `fa-agents` | Agent naming, agent definitions, and repository custom agent conventions |

## Repository artifacts

- `docs/architecture/**` -> `fa-architecture`
- `README.md` when architecture messaging changes -> `fa-architecture`
- `gaia_plan.md` -> `fa-planning`
- Repository code and implementation changes -> `fa-engineering`
- React UI implementation and design-system conformance -> `fa-ui-engineering`
- Formal test files and testing evidence -> `fa-testing`
- `skills/**`, `.github/skills/**`, or `.claude/skills/**` -> `fa-skills`
- `agents/**`, `.github/agents/**`, or `.claude/agents/**` -> `fa-agents`

## Naming conventions

- Skill names: `<lowercase-hyphenated-repo-name>-<lowercase-hyphenated-skill-name>`
- Agent names: `<lowercase-hyphenated-repo-name>-<lowercase-hyphenated-agent-name>`

These are Gaia repository conventions layered on top of any lower-level format constraints required by the underlying skill or agent specification.
