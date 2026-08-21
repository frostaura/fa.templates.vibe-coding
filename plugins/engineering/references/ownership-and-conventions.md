# Gaia Ownership and Conventions

Use this reference when deciding which skill or agent should own a change, which repository artifact should be updated, or which naming convention to apply.

## Skill ownership (engineering plugin)

| Skill | Primary ownership |
|---|---|
| `fa-engineering-process` | Complexity classification, execution path, coordination, handoffs, and closure expectations |
| `fa-engineering-architecture` | The consuming repository's `docs/architecture/` tree, architecture drift resolution, and README sync after architecture changes |
| `fa-engineering-planning` | The MCP task plan (`tasks_create` / `tasks_update` / `tasks_complete`): branch structure, dependencies, QA checkpoints, release gates, and proof expectations |
| `fa-engineering-implementation` | Planned implementation work, rapid iteration, implementation stabilization, and engineering standards during delivery |
| `fa-engineering-ui` | React UI implementation, design-system enforcement, token-only styling, responsive layout discipline, and cleanup of legacy arbitrary values |
| `fa-engineering-testing` | Formal test strategy, test artifacts, regression coverage, and testing evidence |
| `fa-engineering-default-tech-stack` | Default frontend and backend stack selection, design-system standardization, the MCP-ready API baseline, and the declared-deviation rule. **This is the routing layer**: it chooses *between* stacks, then defers to the per-language baseline below |
| `fa-engineering-deploy-chain` | Activating a repository's already-authored build-and-publish chain — the credential set, the deployment target that must exist before its identifier has a value, and the image-namespace match |
| `fa-engineering-containerization` | The root compose stack and the `.env.example` contract for a service that already has its language baseline in place |
| `fa-engineering-e2e-testing` | End-to-end browser coverage and its traceable test identifiers |
| `fa-engineering-manual-regression` | The manual API and web regression lanes, and their low-context evidence capture |

### Per-language stack baselines

Each owns one language's lint, format, test and command surface, and nothing else. Choosing *between* them is `fa-engineering-default-tech-stack`; none of them ever argues for its own language.

| Skill | Primary ownership |
|---|---|
| `fa-engineering-stack-web-ts` | The TypeScript web app baseline |
| `fa-engineering-stack-dotnet-api` | The .NET HTTP API baseline |
| `fa-engineering-stack-dotnet-maui` | The .NET MAUI client baseline, plus device build and deploy to emulators, simulators and physical hardware |
| `fa-engineering-stack-flutter` | The Flutter and Dart client baseline |
| `fa-engineering-stack-python` | The Python repository baseline |

Skill-definition and agent-definition maintenance is owned by the **foundation plugin**'s `fa-foundation-create-skill` and `fa-foundation-create-agent` skills. This plugin routes definition-file work to them; it does not carry its own copies.

## Agent roster (engineering plugin)

| Agent | Role |
|---|---|
| `fa-engineering-intake-coordinator` | Request framing, complexity classification, drift detection, and first routing |
| `fa-engineering-solutions-architect` | Target-solution ownership and architecture documentation in the consuming repository |
| `fa-engineering-implementation-planner` | Turning approved architecture into the MCP task plan with gates and proof expectations |
| `fa-engineering-software-engineer` | Planned implementation delivery and branch stabilization |
| `fa-engineering-tester` | Formal validation, evidence, and pass-fail-blocked authority |
| `fa-engineering-release-engineer` | Final gate evaluation, proof recording, and ready-or-not-ready decisions |

## Repository artifacts

These paths refer to the **consuming repository** — the project Gaia is operating on, not this plugin.

- `docs/architecture/**` -> `fa-engineering-architecture`
- `README.md` when architecture messaging changes -> `fa-engineering-architecture`
- The MCP task graph -> `fa-engineering-planning`
- Repository code and implementation changes -> `fa-engineering-implementation`
- React UI implementation and design-system conformance -> `fa-engineering-ui`
- Formal test files and testing evidence -> `fa-engineering-testing`
- Skill definitions (`SKILL.md` files) -> foundation plugin's `fa-foundation-create-skill`
- Agent definitions -> foundation plugin's `fa-foundation-create-agent`

## Naming conventions

- Skills and agents carry the `fa-` prefix followed by a lowercase-hyphenated role or domain name: `fa-engineering-planning`, `fa-engineering-solutions-architect`, `fa-engineering-default-tech-stack`.
- Prefer role or domain names over implementation details; the name should survive a refactor of how the job is done.

These are Gaia conventions layered on top of any lower-level format constraints required by the underlying skill or agent specification.
