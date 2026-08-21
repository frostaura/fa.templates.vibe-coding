# Context Audit — Finding Codes

`python3 ${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py` mechanically checks the context layer described in [`context-cascade.md`](context-cascade.md). It sweeps the scopes it is given as `--scope` arguments (never a hard-coded list), takes `--max-age DAYS` for memory freshness and `--no-git` to skip the repository checks, and exits `0` clean / `1` with findings.

**Two rules make it safe to use.**

1. **Findings are advisory, not policy.** The script sees text and file state; it cannot see intent. Every finding is a question to answer, and "this is correct as written" is a legitimate answer — recorded in the memory store so the next sweep does not re-litigate it.
2. **The count is not a score.** Driving the total to zero is not the goal and is sometimes actively destructive. **`POSSIBLE-STATE-IN-INSTRUCTIONS` is the class where clearing findings deletes binding rules** — a locked product rule ("releases are cut from `main`, currently and always") and a status line ("currently on v3, dormant since March") look identical to a regex. An agent that clears this class to zero has probably deleted rules the repository depended on. Read every one; expect to keep some.

Read `GIT-DIVERGED` and `GIT-DESTRUCTIVE-UNPUSHED` first in any run — they are the two that can lose work.

## Structure and pairs

| Code | Means | Resolve | Judgement? |
|---|---|---|---|
| `MISSING-SCOPE` | A scope named to the audit has no directory | Fix the argument, or create the scope | Yes — which is wrong, the name or the disk? |
| `MISSING-PAIR` | A scope has one context file, not both | Author the missing file. Both files or neither | No |
| `UPKEEP-MISSING` | An instruction file has no `## Upkeep` section | Add it; the standing duty must be stated where it applies | No |
| `SYNC-CONFLICT-DUPES` | ` 2.ext` duplicate files in the context layer | Cloud-sync tools create these when two machines write the same file. Diff the pair before deleting either — the copy is usually stale, but not always | Yes |

## Registries

| Code | Means | Resolve | Judgement? |
|---|---|---|---|
| `REGISTRY` | The registry section could not be found at all | Restore the heading the audit anchors on | No |
| `REGISTRY-UNLISTED` | A directory exists on disk but no registry names it | Register it, or delete it — an unregistered scope drifts immediately | Yes |
| `REGISTRY-PHANTOM` | A registry names something with no directory | Remove the entry, or restore what was lost | Yes |
| `REGISTRY-CONTRADICTION` | Listed as retired, yet the directory exists | Resolve which is true; a retired-but-present scope is a half-finished kill | Yes |

## Links and readability

| Code | Means | Resolve | Judgement? |
|---|---|---|---|
| `BROKEN-LINK` | A relative markdown link resolves to nothing | Repair or delete it. Code fences and inline spans are already excluded, so a reported link is genuinely broken | No |
| `UNCLOSED-FENCE` | A code fence opened and never closed | Close it. Until then every line after it is blanked before the link check — the rest of that file is silently unchecked | No |
| `UNREADABLE` | A file could not be read from disk | Open it with the agent's own read tool; cloud-backed or permission-gated files hit this | No |

## Memory store

| Code | Means | Resolve | Judgement? |
|---|---|---|---|
| `MEMORY-UNMIGRATED` | A monolithic `MEMORY.md` with no `memory/` directory | Split it into the topic store | No |
| `NO-VERIFIED-STAMP` | An unmigrated `MEMORY.md` carries no verified date | Migrate it; the stamp moves into topic frontmatter | No |
| `STALE-MEMORY` | A topic's `last_verified` is older than `--max-age` | Re-verify by inspection, then restamp. **Never restamp what you merely edited** | Yes |
| `MEMORY-INDEX-DRIFT` | Non-index content inside the index | Move the prose into a topic file; the index is heading plus hook lines only | No |
| `MEMORY-LINK-BROKEN` | The index links a topic that is not on disk | Restore the topic or drop the index line | No |
| `MEMORY-ORPHAN-TOPIC` | A topic file exists that nothing indexes | Index it or delete it — an unindexed topic is never read | Yes |
| `MEMORY-FRONTMATTER` | The six-line block is malformed | Rewrite it: `---`, `name`, `description`, `type`, `last_verified`, `---` — four keys, in that order, no extras | No |
| `MEMORY-TOPIC-TYPE` | `type:` is outside the ten-value vocabulary | Use the singular form from the closed list; plurals defeat every filter | No |
| `MEMORY-TOPIC-LONG` | A topic file has outgrown ~60 lines | Split it, or prune what stopped being true | Yes |

## Instruction-vs-state split

| Code | Means | Resolve | Judgement? |
|---|---|---|---|
| `POSSIBLE-STATE-IN-INSTRUCTIONS` | A date or a state word inside an instruction file | Move genuine state to the memory store; **keep the line where it is a rule that happens to contain a state word** | **Always** — see rule 2 above |

## Skills

| Code | Means | Resolve | Judgement? |
|---|---|---|---|
| `SKILLS-INDEX-MISSING` | A skills directory with no `README.md` index | Write the index | No |
| `SKILL-MALFORMED` | A skill directory with no `SKILL.md` | Add it, or remove the directory. `references/`, `assets/`, `scripts/` and `templates/` are exempt | No |

## Scratch discipline

| Code | Means | Resolve | Judgement? |
|---|---|---|---|
| `TMP-NOT-EMPTY` | A `.tmp/` still holds files | The session that wrote them is not closed. **Promote anything that still matters, then delete — in that order**; there is no undo above the repositories | Yes |
| `TMP-TRACKED` | `.tmp/` paths are tracked by git | Add `.tmp/` to that repository's `.gitignore` and untrack them; scratch is being pushed instead of emptied | No |
| `STRAY-ARTIFACT` | Tool residue, a loose log/image, or a dated filename in the reference layer | Move it into a `.tmp/`, or promote it and delete the file. A dated `.md` in a reference tree is a session report, and a report is not a memory | Yes |

## Repository durability (skipped by `--no-git`)

| Code | Means | Resolve | Judgement? |
|---|---|---|---|
| `GIT-LOCK-DEBRIS` | A stale `.git/*.lock` | Remove it — it blocks every commit until it goes | No |
| `GIT-DIRTY` | Uncommitted paths in the working tree | Commit them at the scope that owns them. Never `reset`, `clean`, `stash` or `restore` to clear this — the work exists nowhere else | Yes |
| `GIT-NO-REMOTE` | No remote at all | This repository exists on one machine. Escalate to the human owner | Yes |
| `GIT-NO-UPSTREAM` | A remote exists but the branch tracks nothing | Never pushed. Escalate before setting an upstream | Yes |
| `GIT-DIVERGED` | Local and remote each hold commits the other lacks | **Do not run a generic push runbook here.** A plain push is rejected and `--force` destroys the remote. Human-owner decision | **Always** |
| `GIT-UNPUSHED` | Ahead of upstream | Usually safe to push, once `GIT-DESTRUCTIVE-UNPUSHED` is clear. Record the topology, not the number | Yes |
| `GIT-DESTRUCTIVE-UNPUSHED` | Unpushed commits delete many tracked files | Verify the deletions are intended *before* any push. This is the one count worth recording, because it describes what a commit does rather than where a pointer sits | **Always** |
