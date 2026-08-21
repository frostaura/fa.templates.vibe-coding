# The Context Cascade — instruction file + `MEMORY.md`

Every directory in a consuming repository that carries meaning carries **two** context files: an **instruction file** (`CLAUDE.md` for Claude Code, `AGENTS.md` for tools that look for that name) and a `MEMORY.md`. A **scope** is any directory that carries this pair. Both files cascade downward: the repository root applies everywhere, a scope narrows it, a scope nested inside that one narrows it further. An agent entering at any depth reads its own pair plus every pair above it.

## The split

| | instruction file | `MEMORY.md` |
| --- | --- | --- |
| **Answers** | What is this, and how do I work here? | What is actually true here right now? |
| **Nature** | Normative — rules, mandate, conventions, standards | Observational — state, dates, decisions, gotchas |
| **Changes** | Rarely. A change is a policy change. | Often. Every meaningful session should touch it. |
| **Tense** | Timeless present ("projects use lowercase names") | Dated past/present ("as of 2026-07-19, dormant since 2026-05-02") |
| **If wrong** | The rule was wrong | The world moved |

The failure this split exists to prevent: instructions and reality decay at completely different rates, and mixing them into one file means **the whole file rots at the speed of the fastest-moving line**. Keeping state out of the instruction file is what lets the instruction file stay stable and trustworthy.

**Rule of thumb:** if a sentence contains a date, a status, a version, a "currently", or a "we decided" — it belongs in `MEMORY.md`. If it would still be true after a year of work, it belongs in the instruction file.

**The one refinement worth stating precisely.** A *structural inventory* is not state. "This scope owns these projects", "the API exposes these pillars", "the stack is .NET + EF Core + PostgreSQL" are facts about the shape of the thing, and they belong in the instruction file because routing and navigation depend on them. What does not belong is anything about *condition*: patch-level version pins, progress, phase, dormancy, dates, counts that move on the next commit, or credentials. The test is not "could this ever change" — everything can. It is "does an agent need this to find its way around, or to know how things are going right now?" **Navigation is instruction; condition is memory.**

## The third artifact — skills

A scope's `skills/` directory answers a third question: **how is a recurring job done here?** It cascades exactly as the pair does — root skills fire for any session inside the repository, a nested scope's own skills fire only when a session is rooted at or inside that directory.

The division of labour is clean once stated. A *fact* is `MEMORY.md`. A *rule* is the instruction file. A *procedure with an order and traps in it* is a skill. "The tuned strategy returns 18.1%/yr" is memory. "Strategies live in `user_data/strategies/`" is instruction. "Run the campaign loop, and without `--analyze-per-epoch` the optimizer silently fake-optimizes" is a skill.

Two rules keep the layer healthy. **Never mirror a root skill downward** — one root skill referenced everywhere, not one copy per scope drifting apart. **Never park a scope-specific procedure at root**, where it taxes every session in the repository forever.

**A stale skill is worse than a missing one.** It fires with authority and misleads. Delete without ceremony.

## Agents — the workers, not a fourth context artifact

Agent definitions are pre-written subagent roles. They are not a fourth cascading artifact — they carry no rules of their own that the instruction file does not already carry. They exist so that the *method* for a recurring fan-out lives in one editable place instead of being re-typed from memory into every brief, which is how method drifts. When how a sweep is run changes, **edit the agent definition**; a brief is not a place to keep policy.

**A session report is not a context artifact either, and must never become a fourth one.** There are three, and a report is none of them: nothing cascades it and the audit does not check it, so a finding parked in one is invisible to every mechanism this document describes. **Promote the findings into the memory store of the scope they concern, then delete the report** — in that order. A subagent's report is returned to its caller and never written into the tree; the pass's working files live in a `.tmp/` at that scope and go with it.

## Where the pair is required

A tree carrying none of this yet acquires the cascade in one pass rather than a file at a time: `fa-foundation-optimize-directory-tree` partitions the tree, writes the pair at every scope that earns one, and reconciles the root last.

- **The repository root** — required.
- **Every scope passed to the audit as a `--scope` argument** — required.
- **Every directory that groups scopes** (a `projects/` or `packages/` directory) — required. The instruction file holds the schema each child follows; `MEMORY.md` holds the live roster with per-child status.
- **Every scope inside it** — required.

**A directory with an instruction file and no `MEMORY.md` is a defect, not a shortcut.** It states the rules and leaves reality unstated, so the next agent infers status instead of reading it — precisely the failure the split exists to prevent. **Both files or neither.**

- **Reference (`docs/`) directories** — instruction file only, and only where the reference layer itself needs rules. Doctrine, not state.
- **`skills/` directories** — a `README.md` index, wherever one exists at any scope.
- **Deeper than a scope root** — at your discretion. Add a pair to a subdirectory only when it has its own working rules an agent would otherwise get wrong (a Python sidecar with a purity contract, a research folder with a one-shot holdout). Do not add them mechanically; an unnecessary context file is noise that costs every future agent tokens.

## Memory layout — the topic store

`MEMORY.md` is a **pure index**; the content lives in a `memory/` directory beside it, one topic file per concern. This mirrors how agent memory systems themselves work: list the index, skim each topic's frontmatter, deep-read only what the task needs.

**The index** contains nothing but the scope heading and one line per topic:

```markdown
# MEMORY — <scope name>

- [Current state](memory/state.md) — one-line hook that carries the actual signal
- [Live decisions](memory/decisions.md) — the decisions an agent must not relitigate, lead phrases inline
```

The hook is not a label — it carries the state signal itself ("deploy decided, unpushed range still local, kill clock void once live"), so a reader who stops at the index still leaves informed. Zero other content: a sentence in the index is index drift, and the audit flags it.

**Topic files** open with exactly this frontmatter — six lines, so `head -7` of any topic file returns the complete relevance signal plus the first body line:

```markdown
---
name: <scope>-<topic>
description: "one line — enough to judge relevance without opening the body"
type: state
last_verified: YYYY-MM-DD
---

# <Topic heading>

body…
```

`name` is globally unique kebab-case. `type` is one of exactly ten values, always singular — the *file* may be `decisions.md`, the *type* is `decision`, and a plural defeats any tooling that filters on type: `state` (the scope's current state), `decision` (live decisions with their *why*), `gotcha`, `question` (open questions, naming the owner), `watch` (known drift and unpaid debts), `kill-record`, `alert` (red do-not-do-this items — indexed near the top), `log` (dated session logs), `evidence` (assembled facts for a pending human-owner call), `reference`.

**Read protocol.** Entering a scope: read its index and every index above it — indexes cascade exactly as the instruction file does. Then `head -7` any topic whose hook looks relevant; deep-read only those that are. `alert` topics are read always, at every level passed through.

**Write protocol.** New concern → new topic file plus one index line. Changed concern → edit the topic file, restamp its `last_verified`, and re-cut the index hook if the signal moved. Dead concern → delete the file and its index line. Relative links inside a topic file resolve from `memory/`, one level below the scope — prefix `../`.

Keep a topic file under ~60 lines; past that it is probably two topics. A small scope may carry just `state.md` and one or two others — never pad to a canonical set. Delete anything that stops being true: memory is not a changelog and never grows without bound.

**Do not record volatile integers in prose.** Ahead-counts, behind-counts, dirty-path counts and commit totals go stale *inside the session that writes them* — the context commit that closes the session moves them, so the recording agent invalidates its own sentence before it finishes, sometimes with arithmetic that no longer closes. The audit re-measures every one of these in seconds and is the single source; a number copied into a topic file is a second source that starts disagreeing with it immediately.

**Record the topology, not the integer.** "Safe to push — ahead-only, no deletions in the range" · "diverged fork; a plain push is rejected and `--force` destroys the remote" · "no remote at all, exists on one machine" · "a large uncommitted body that has never been `git add`ed, so `git clean` deletes it with no reflog" — these stay true across sessions and carry the signal a reader actually needs. A number is worth writing down only when it is **itself the hazard and does not move**: "the unpushed range deletes 21 named tracked files" is a fact about what a commit *does*; "3 ahead" is a fact about where a branch pointer *currently sits*. If a count is worth quoting at all, say when it was measured and that it must be re-measured before it is acted on.

## Maintenance duties

1. **Read up the chain before acting.** Root → grouping directory → scope. Later files narrow earlier ones; they never silently contradict them. A real contradiction is a bug — fix it, don't route around it.
2. **Write only the delta.** Never restate a parent's content. Link to it. Duplication is how a tree drifts against itself.
3. **Update the memory store at the end of any session that changed reality** — shipped, decided, discovered, abandoned. Edit the topic files, re-cut the index hooks that moved, and restamp `last_verified` only where you actually checked, not merely edited.
4. **Prune.** Resolved questions move into `decision` topics or disappear. Cleared watch items disappear, with their index lines. Delete topic files whose concern died.
5. **Promote up, don't duplicate sideways.** A fact that matters across a whole grouping belongs in that level's topic file, referenced by the scopes below — not copy-pasted into each.
6. **Facts beat inference.** Establish state by inspection (`git log`, manifests, tests) before recording it. Never record a status you did not verify. Git claims specifically — committed, pushed, backed up, deploy-ready — decay faster than anything else here and are never repeated from a context file without re-running the command behind them.
7. **Maintain the skills alongside the pair.** If a session changed *how* the work is done, the local skills are as stale as an unupdated `MEMORY.md` would be. Fix or delete them in the same session.
8. **Assess by reading, never by searching.** Read every context file in a scope end to end before changing any of it — the pair, every `memory/` topic, every local `SKILL.md`. Duties 4 and 5 are the ones a grep cannot serve at all: nothing in a search result tells you that a topic file's concern has died, or that the same fact now sits in three scopes and belongs one level up. Deletion and consolidation are normal outcomes here, not exceptional ones.
9. **Never describe a directory's file inventory from inside a file you are adding to that directory.** One pass wrote "this directory contains exactly one file" into four `MEMORY.md` files it was in the act of adding to those directories — false before the write completed. Describe *dormancy and last real activity*, not file counts and mtimes, and warn explicitly when a context file's own mtime should not be read as scope activity.

## Automation

Everything here that does not require judgement is enforced mechanically by `${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py` — run it rather than checking by hand. Its finding codes, what each means, and which ones need a human read: [`context-audit-findings.md`](context-audit-findings.md).

## Precedence

The instruction file outranks `MEMORY.md` on *how to work*. `MEMORY.md` outranks the instruction file on *what is currently true*. If an instruction file asserts a state at all, that is a defect — move it.
