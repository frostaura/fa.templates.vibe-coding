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

## Disclaimers

Gaia uses a **remote MCP server** by default so plans, memories, and evolution lessons persist across machines and sessions (including GitHub Copilot's web coding agent).

**Stored:** evolution suggestions, task plans, project memory — all segregated by project name.
**Not stored:** user PII, project code, specs, or documentation.

Prefer fully local? Point the MCP config at a local STDIO server instead.

---

## Local development

Working on Gaia itself? Install the plugin from a local clone so changes are picked up immediately.

### GitHub Copilot

```bash
copilot plugin install /absolute/path/to/ai.toolkit.gaia
```

### Claude Code

```bash
/plugin marketplace add /absolute/path/to/ai.toolkit.gaia

/plugin install gaia-foundation@frostaura
```

---

<p align="center">
  <i>"In Greek mythology, Gaia is the personification of Earth and the ancestral mother of all life."</i>
</p>
