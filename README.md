<p align="center">
  <img src="https://github.com/frostaura/ai.toolkit.gaia/blob/main/README.icon.png?raw=true" alt="Gaia" width="300" />
</p>

<h1 align="center"><b>Gaia</b></h1>
<h3 align="center">full-stack apps. enterprise-grade. maintainable. customizable.</h3>
<p align="center"><i>A team of AI agents, available as a plugin for GitHub Copilot and Claude Code.</i></p>

---

<p align="center">
  <a href="https://github.com/frostaura/ai.toolkit.gaia/releases"><img src="https://img.shields.io/badge/Version-10-purple.svg" alt="Version 10" /></a>
  <a href="https://opensource.org/licenses/MIT"><img src="https://img.shields.io/badge/License-MIT-yellow.svg" alt="License: MIT" /></a>
  <a href="https://github.com/features/copilot"><img src="https://img.shields.io/badge/GitHub-Copilot-blue.svg" alt="GitHub Copilot" /></a>
  <a href="https://www.claude.com/product/claude-code"><img src="https://img.shields.io/badge/Claude-Code-orange.svg" alt="Claude Code" /></a>
  <a href="https://gaia.frostaura.net"><img src="https://img.shields.io/badge/MCP-remote-green.svg" alt="Remote MCP" /></a>
  <a href="https://dotnet.microsoft.com/"><img src="https://img.shields.io/badge/.NET-MCP%20server-512BD4.svg" alt=".NET MCP server" /></a>
</p>

---

## What is Gaia?

Gaia is a **team of AI agents** that builds and evolves software using **spec-driven development**. You describe your goal; Gaia coordinates intake, architecture, planning, implementation, testing, and release — enforcing QA and release gates along the way.

The workflow contract lives in [`AGENTS.md`](./AGENTS.md). Gaia ships as three composable plugins, backed by a custom **.NET MCP server** ([`src/`](./src)) that persists tasks, memory, and self-evolution lessons across machines and sessions.

---

## Catalog

### Plugins

| Plugin            | When to use                                                                                                      |
| ----------------- | ---------------------------------------------------------------------------------------------------------------- |
| **`foundation`**  | Always — base layer for any solution. Authoring new agents/skills/plans and wiring the remote MCP server.        |
| **`engineering`** | Building or evolving software end-to-end: intake → architecture → planning → implementation → testing → release. |
| **`product`**     | Validating a consumer one-off / in-app-purchase product idea against real net-of-fee economics before you build. |

### Engineering agents

| Agent                           | When to use                                                                                    |
| ------------------------------- | ---------------------------------------------------------------------------------------------- |
| **`fa-intake-coordinator`**     | Entry point — frames a raw request, clarifies scope, and routes work to the right specialists. |
| **`fa-solutions-architect`**    | Designing the system: drafting/updating architecture specs in `docs/` before code.             |
| **`fa-implementation-planner`** | Breaking approved architecture into a sequenced, gated task plan.                              |
| **`fa-software-engineer`**      | Implementing planned tasks and writing the code.                                               |
| **`fa-tester`**                 | Writing tests and validating behavior against QA gates.                                        |
| **`fa-release-engineer`**       | Verifying release gates and shipping the change.                                               |

### Product agents

| Agent                                   | When to use                                                                          |
| --------------------------------------- | ------------------------------------------------------------------------------------ |
| **`fa-product-coordinator`**            | Entry point — frames the brief and orchestrates the 10 money-gated discovery stages. |
| **`fa-product-ideation-strategist`**    | Generating and shaping product concepts.                                             |
| **`fa-product-market-researcher`**      | Sizing demand, competition, and market opportunity.                                  |
| **`fa-product-validation-researcher`**  | Testing demand signals and de-risking assumptions with evidence.                     |
| **`fa-product-monetization-economist`** | Designing pricing and the unit-economics model.                                      |
| **`fa-product-build-architect`**        | Scoping the vertical slice / MVP and production hardening.                           |
| **`fa-product-liveops-manager`**        | Planning post-launch live-ops and retention.                                         |
| **`fa-product-growth-marketer`**        | Designing go-to-market, soft launch, and acquisition.                                |
| **`fa-product-compliance-officer`**     | Checking platform, legal, and store-policy compliance.                               |
| **`fa-product-reviewer`**               | Adversarially reviewing each stage's output.                                         |
| **`fa-product-synthesizer`**            | Reconciling fan-out + review into a single decision against the economics model.     |

### Skills

| Skill                                    | Plugin      | When to use                                                   |
| ---------------------------------------- | ----------- | ------------------------------------------------------------- |
| **`fa-create-agent`**                    | foundation  | Authoring or revising a custom agent definition.              |
| **`fa-create-skill`**                    | foundation  | Authoring or revising a `SKILL.md`.                           |
| **`fa-create-plan`**                     | foundation  | Researching and outlining a multi-step plan.                  |
| **`fa-process`**                         | engineering | Orchestrating the end-to-end software workflow and gates.     |
| **`fa-architecture`**                    | engineering | Keeping architecture docs aligned before structural changes.  |
| **`fa-default-tech-stack`**              | engineering | Choosing the baseline stack when none is specified.           |
| **`fa-planning`**                        | engineering | Turning architecture into a branch-aware, gated task plan.    |
| **`fa-engineering`**                     | engineering | Delivering planned implementation against repo conventions.   |
| **`fa-ui-engineering`**                  | engineering | Building React UI with the design system and semantic tokens. |
| **`fa-testing`**                         | engineering | Running formal validation layers and recording proof.         |
| **`fa-product-process`**                 | product     | Orchestrating the money-gated product lifecycle.              |
| **`fa-product-discovery`**               | product     | Researching market and opportunity.                           |
| **`fa-product-ideation`**                | product     | Generating and shaping concepts.                              |
| **`fa-product-monetization-design`**     | product     | Designing pricing and monetization.                           |
| **`fa-product-validation`**              | product     | Validating demand with evidence.                              |
| **`fa-product-vertical-slice`**          | product     | Scoping the MVP / vertical slice.                             |
| **`fa-product-production-hardening`**    | product     | Hardening the build for production.                           |
| **`fa-product-compliance`**              | product     | Checking compliance and store policy.                         |
| **`fa-product-soft-launch`**             | product     | Planning and running a soft launch.                           |
| **`fa-product-gtm-launch`**              | product     | Planning the full go-to-market launch.                        |
| **`fa-product-liveops`**                 | product     | Planning post-launch live-ops.                                |
| **`fa-product-money-gate`**              | product     | Enforcing the money gate at each stage.                       |
| **`fa-unit-economics-model`**            | product     | Building/maintaining the shared net-of-fee economics model.   |
| **`fa-product-portfolio-retrospective`** | product     | Reviewing outcomes across the portfolio.                      |

---

## Install

### GitHub Copilot

```bash
copilot plugin marketplace add frostaura/ai.toolkit.gaia

copilot plugin install foundation@frostaura
copilot plugin install engineering@frostaura
copilot plugin install product@frostaura
```

### Claude Code

```bash
/plugin marketplace add frostaura/ai.toolkit.gaia

/plugin install foundation@frostaura
/plugin install engineering@frostaura
/plugin install product@frostaura
```

> Install only what you need — `foundation` is the base, `engineering` adds software delivery, `product` adds product discovery.

---

## Use it

Open any project and prompt your assistant:

> **"Create a REST API for a blog with posts and comments."**

Gaia refines the request, drafts architecture into `docs/`, plans the work, implements it, tests it, and validates release gates — automatically.

### Headless (Copilot CLI)

```bash
copilot -p "Create a REST API for a blog with posts and comments" --yolo
```

### Product discovery (one-off / in-app purchases)

The `product` plugin runs a money-gated pipeline for consumer products that monetize via one-off or in-app / in-game purchases (not SaaS):

```bash
/fa-product-coordinator premium one-off puzzle game for iOS + Steam, ~$200k net in year one
```

…or describe the goal in natural language. The coordinator frames the brief, then runs 10 money-gated stages — each as fan-out → adversarial review → synthesis against one shared unit-economics model — and returns an Opportunity Thesis with a go / loop-back / kill decision.

---

## Disclaimers

Gaia uses a **remote MCP server** by default so plans, memories, and evolution lessons persist across machines and sessions (including GitHub Copilot's web coding agent).

- **Stored:** evolution suggestions, task plans, project memory — all segregated by project name.
- **Not stored:** user PII, project code, specs, or documentation.

Prefer fully local? Point the MCP config at a local STDIO server instead. The server source lives in [`src/Gaia.Mcp.Server`](./src/Gaia.Mcp.Server).

---

## Local development

Working on Gaia itself? Install from a local clone so changes are picked up immediately. Point the install at a plugin directory (where `plugin.json` lives), not the repo root.

### GitHub Copilot

```bash
copilot plugin install /absolute/path/to/ai.toolkit.gaia/plugins/foundation
```

### Claude Code

Claude Code clones the marketplace source, so the published [`.claude-plugin/marketplace.json`](./.claude-plugin/marketplace.json) points each plugin at the remote repo. For local dev, temporarily point the `source` at the in-repo plugin directory so local edits are picked up:

```jsonc
// .claude-plugin/marketplace.json
"source": "./plugins/foundation"
```

> ⚠️ This change is for local development only — **do not commit it.** Revert to the published remote source before pushing.

Then install from your local clone:

```bash
/plugin marketplace add /absolute/path/to/ai.toolkit.gaia
/plugin install foundation@frostaura
```

### MCP server

```bash
dotnet build src/Gaia.Mcp.Server/Gaia.Mcp.Server.csproj
```

---

## Contributing

See [`AGENTS.md`](./AGENTS.md). Keep this README concise and current with any new features or changes. Contributions are welcome under the [MIT License](./LICENSE).

---

<p align="center">
  <i>"In Greek mythology, Gaia is the personification of Earth and the ancestral mother of all life."</i>
</p>
