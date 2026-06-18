# Setup scripts

## `link-claude-dirs` — share agents & skills between `.claude` and `.github`

The agents and skills live once, under `.github/`. The `.claude/` directory
links to them so both Copilot (`.github`) and Claude Code (`.claude`) read the
**same** files — add/edit/remove in either place and it reflects in both.

```
.claude/agents  ->  ../.github/agents
.claude/skills  ->  ../.github/skills
```

These are committed as real symlinks. They work as-is on **macOS/Linux**, and on
**Windows** if Developer Mode is on (or git was cloned with `core.symlinks=true`).
On a Windows checkout where the links came through as plain text files, run the
setup script once to repair them — it creates a junction, which needs no admin
rights.

### Run it

| Platform     | Command                                |
| ------------ | -------------------------------------- |
| macOS/Linux  | `bash scripts/link-claude-dirs.sh`     |
| Windows      | `scripts\link-claude-dirs.cmd`         |

Both scripts are idempotent — safe to run any number of times. They only touch
`.claude/agents` and `.claude/skills`.
