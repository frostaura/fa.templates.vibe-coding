<p align="center">
  <img src="https://github.com/frostaura/ai.toolkit.gaia/blob/main/README.icon.png?raw=true" alt="Gaia" width="300" />
</p>

<h1 align="center"><b>Gaia</b></h1>
<h3 align="center">full-stack apps. enterprise-grade. maintainable. customizable.</h3>
<p align="center"><i>A team of AI agents, available as a plugin for GitHub Copilot and Claude Code.</i></p>

---

[![Version 9](https://img.shields.io/badge/Version-9-purple.svg)]()
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![GitHub Copilot](https://img.shields.io/badge/GitHub-Copilot-blue.svg)]()
[![Claude Code](https://img.shields.io/badge/Claude-Code-orange.svg)]()

---

## What is Gaia?

Gaia is a **team of AI agents** that builds and evolves software using **spec-driven development**. You describe your goal; Gaia coordinates architecture, planning, implementation, testing, and release.

The workflow contract lives in [`AGENTS.md`](./AGENTS.md).

Beyond software delivery, Gaia includes a **consumer product-discovery team** — a `gaia-product-coordinator` that takes a one-off or in-app/in-game-purchase product from idea to a money-validated bet across discovery, monetization design, validation, launch, and live-ops. See [`docs/architecture/product-discovery-team.md`](./docs/architecture/product-discovery-team.md).

---

## Install

### GitHub Copilot

```bash
copilot plugin marketplace add frostaura/ai.toolkit.gaia

copilot plugin install gaia-foundation@frostaura
```

### Claude Code

```bash
/plugin marketplace add frostaura/ai.toolkit.gaia

/plugin install gaia-foundation@frostaura
```

---

## Use it

Open any project and prompt your assistant:

> **"Create a REST API for a blog with posts and comments."**

Gaia will refine the request, draft architecture into `docs/`, plan the work, implement it, test it, and validate release gates — automatically.

### Headless (Copilot CLI)

```bash
copilot -p "Create a REST API for a blog with posts and comments" --yolo
```

---

## Product discovery (one-off / in-app purchases)

Gaia also runs a money-gated **product-discovery pipeline** for consumer products that monetize via one-off purchases or in-app / in-game purchases (not SaaS). Kick it off with the slash command (Claude Code) or prompt file (GitHub Copilot):

```bash
/gaia-product-coordinator premium one-off puzzle game for iOS + Steam, ~$200k net in year one
```

…or just describe the goal in natural language:

> **"Use gaia-product-coordinator to find a premium one-off cozy roguelite for Steam I could ship solo and clear ~$150k net in year one."**

The `gaia-product-coordinator` frames the brief, then runs the 10 money-gated stages — each as fan-out → adversarial review → synthesis against one shared unit-economics model — and returns an Opportunity Thesis with a go / loop-back / kill decision. See [`docs/architecture/product-discovery-team.md`](./docs/architecture/product-discovery-team.md).

---

## Disclaimers

Gaia uses a **remote MCP server** by default so plans, memories, and evolution lessons persist across machines and sessions (including GitHub Copilot's web coding agent).

**Stored:** evolution suggestions, task plans, project memory — all segregated by project name.
**Not stored:** user PII, project code, specs, or documentation.

Prefer fully local? Point the MCP config at a local STDIO server instead.

---

## Local development

Working on Gaia itself? Install the plugin from a local clone so changes are picked up immediately.

### GitHub Copilot

Point the install at the plugin directory (where `plugin.json` lives), not the repo root:

```bash
copilot plugin install /absolute/path/to/ai.toolkit.gaia/plugins/foundation
```

### Claude Code

Claude Code clones the marketplace source, so the published `.claude-plugin/marketplace.json` points the `gaia-foundation` plugin at the remote repo. For local dev you must temporarily point the source at the in-repo plugin directory so your local edits are picked up:

```jsonc
// .claude-plugin/marketplace.json
"source": "./plugins/foundation"
```

> ⚠️ This change is for local development only — **do not commit it.** Revert to the published remote source before pushing.

Then install from your local clone:

```bash
/plugin marketplace add /absolute/path/to/ai.toolkit.gaia

/plugin install gaia-foundation@frostaura
```

---

<p align="center">
  <i>"In Greek mythology, Gaia is the personification of Earth and the ancestral mother of all life."</i>
</p>
