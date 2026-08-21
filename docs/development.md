# Development

Working on Gaia itself. For using the plugins, see the [README](../README.md).

## Releases

Plugin sources track `ref: main` — there are no tagged releases. The `version` field in each plugin's `plugin.json` is the update trigger: bumping it is what makes installed clients re-fetch the plugin. Changes reach users when they are pushed to GitHub, and not before.

Versions are **lockstep across all three plugins**, and every release touches **12 JSON version sites**:

| Sites | Where |
| --- | --- |
| 6 | `plugins/<name>/plugin.json` and `plugins/<name>/.claude-plugin/plugin.json` — three pairs, each pair **byte-identical** (`cmp` them) |
| 3 | plugin entries in [`.claude-plugin/marketplace.json`](../.claude-plugin/marketplace.json) |
| 3 | plugin entries in [`.github/plugin/marketplace.json`](../.github/plugin/marketplace.json) |

The marketplace `metadata.version` is the **catalog** version and moves independently of the plugin version — do not bump it for a plugin change.

Every user-visible change is recorded in [`CHANGELOG.md`](../CHANGELOG.md), and the README badge carries the full semver so a stale badge is visible rather than rounded away.

## The two marketplace manifests

[`.claude-plugin/marketplace.json`](../.claude-plugin/marketplace.json) (Claude Code) and [`.github/plugin/marketplace.json`](../.github/plugin/marketplace.json) (GitHub Copilot) describe the same catalog and must agree on plugins, versions, descriptions and owner. Their one allowed difference is source *form*: the Claude Code manifest uses `git-subdir` sources pinned to `ref: main`, while the Copilot manifest uses relative in-repo paths — both are legal for a git-hosted marketplace.

`engineering` and `product` declare `"dependencies": ["foundation"]` in their `plugin.json`. That field is deliberately **not** mirrored into the marketplace manifests: the manifests are a discovery catalog, not a resolver.

## Install from a local clone

Point the install at a plugin directory (where `plugin.json` lives), not the repo root.

### GitHub Copilot

```bash
copilot plugin install /absolute/path/to/ai.toolkit.gaia/plugins/foundation
```

### Claude Code

Claude Code clones the marketplace source, so the published [`.claude-plugin/marketplace.json`](../.claude-plugin/marketplace.json) points each plugin at the remote repo. For local dev, temporarily point the `source` at the in-repo plugin directory so local edits are picked up:

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

## Authoring skills and agents

Use the plugins on themselves: `fa-foundation-create-skill` for a `SKILL.md`, `fa-foundation-create-agent` for an agent definition. Both carry the format specifications as references.

Three invariants that are either satisfied or silently broken:

- **A skill's directory name must equal its frontmatter `name`.** The loader matches on the directory; a mismatch produces a skill that cannot be invoked and looks fine in a listing.
- **A `description` is a trigger, not a summary of the body.** It is the only layer a router reads. A description written as a summary never fires, and nothing tells you it is dead. The cap is 1024 characters — measure it, never eyeball it.
- **Markdown links never escape their plugin root; scripts are referenced as `${CLAUDE_PLUGIN_ROOT}/scripts/<name>`.** Both rules hold at once.

## Checks before a change ships

Nothing in CI asserts any of this yet — run it by hand.

```bash
# 1. Strict validation of all three plugins
claude plugin validate ./plugins/foundation --strict
claude plugin validate ./plugins/engineering --strict
claude plugin validate ./plugins/product --strict

# 2. The three plugin.json pairs are byte-identical
for p in foundation engineering product; do
  cmp "plugins/$p/plugin.json" "plugins/$p/.claude-plugin/plugin.json" || echo "DRIFT $p"
done

# 3. Every skill's frontmatter name equals its directory name
for d in plugins/*/skills/*/; do
  grep -m1 '^name:' "$d/SKILL.md" | grep -q "$(basename "$d")" || echo "MISMATCH $d"
done

# 4. Measure a description against the 1024-character cap
python3 -c "import re,sys; d=re.search(r'^description:\s*(.*?)\nlicense:', open(sys.argv[1]).read(), re.S|re.M).group(1); d=' '.join(d.split()); print(len(d), len(d.encode()))" plugins/foundation/skills/<name>/SKILL.md

# 5. Mechanical context-layer and link checks over this repo
python3 plugins/foundation/scripts/context-audit.py --root . --all
```

The context-audit script cannot reach plugin skills: its skill-index check gates on a `.claude`/`.github` parent directory, and plugin skills live at `plugins/<name>/skills/`. Check 3 above is the substitute, and the gap is recorded in [`memory/watch.md`](../memory/watch.md).

## MCP server

```bash
dotnet build src/Gaia.Mcp.Server/Gaia.Mcp.Server.csproj
```

Server detail — tools, transports, configuration — lives in [`src/README.md`](../src/README.md).

## Contributing

Contributions are welcome under the [MIT License](../LICENSE). Keep the [README](../README.md) concise: it is an install-and-first-run document, not a catalog. New skills go in [`docs/catalog.md`](catalog.md), and the live roster is always `/plugin` in your client.
