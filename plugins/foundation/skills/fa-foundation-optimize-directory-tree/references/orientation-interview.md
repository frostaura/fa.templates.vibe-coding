# The Orientation Interview

One exchange, before any partition is frozen and before any subagent is dispatched. It asks only what the filesystem cannot answer.

**The governing rule: anything the filesystem answers is not a question — bring the answer, ask for confirmation.** "I see three top-level projects, one with no commits since last year — is that one dormant?" costs the owner a glance. "What projects are in this tree?" costs them the work you were installed to do, and it is how an owner learns that the run is not actually reading their tree. Orient shallowly first, then interview, in that order and once.

## When to skip it entirely

Skip the interview and take every filesystem default under **exactly two testable conditions**:

1. **The request carries an explicit autonomy instruction** — "unattended", "don't ask", "proceed without approval". This is the same precedent `fa-foundation-create-plan` uses, and it is deliberately narrower than "this did not come from a human turn": a skill-chained call is indistinguishable from a normal turn from inside one, so that broader reading would silently skip the interview on ordinary runs.
2. **This session cannot reach a human** — it is itself a subagent, or the ask-user tool is unavailable.

**Anything ambiguous asks.** Asking wrongly costs one exchange; skipping wrongly writes a stranger's tree from guesses.

**The condition is tested *before* asking, never after.** "Ask, and if no answer arrives, skip" is unevaluable — the blocking call has no timeout, so a run that adopts that rule either hangs forever or was never going to ask at all.

Skipping the interview does **not** waive the step-4 write plan. The two gates are decoupled: the write plan is stated in every run, and only the blocking go-ahead is waived here. One misfire of a single predicate would otherwise produce an unannounced multi-hundred-file write.

## The five questions

| # | Asks | Changes | Unattended default |
|---|---|---|---|
| 1 | What is this tree, in one sentence, and who works in it? | Root identity; whether groupings are mandated or just folders | Infer from the top-level layout; mark as inference |
| 2 | Which of these are live, dormant, vendored/off-limits? *(checklist)* | Where the analysis budget goes. Highest-value question by far | All live. Where a level has more entries than fit an exchange, group by parent and ask only for exceptions |
| 3 | `CLAUDE.md` or `AGENTS.md`, and is there a standard to match? | Which filename the whole cascade uses; wrong = tooling never loads it | Whichever already exists; if both, `CLAUDE.md` with `AGENTS.md` demoted to interop pointer; if neither, `CLAUDE.md` |
| 4 | Where does design documentation live, and does a convention govern it? | House templates vs extending what exists | Whatever `docs/` already contains |
| 5 | What must never be touched, **may I run your build and test commands**, and is anything to be committed? | The write boundary, and whether step 5 may execute anything | Nothing committed. Writes limited to context files, `docs/`, and one `.gitignore` entry for `.tmp/`. **Builds: no.** `npm install`, `dotnet build`, `make`, `pytest` execute third-party code, mutate lockfiles and reach the network — running them unasked in a stranger's tree, N times in parallel, violates the boundary the same question just declared. Without consent, the documented command is recorded as unverified inference plus a `question` topic. **No branch runs a dependency-install or network-fetching step regardless of consent.** |

## The conditional sixth

Ask **only if depth is genuinely ambiguous** — otherwise it is a question the filesystem already answered: *how deep should the pair go?* Default: project root only. Depth is earned, never mechanical; a directory gets a pair when it has working rules an agent would otherwise get wrong.

## What the interview did not settle

Every assumption becomes a `question` topic rather than a silent default, so the owner can overturn it in one edit instead of discovering it three months later inside a file that reads as settled fact.

Where no human resolves a question, **the owner field is written literally as `unassigned` and the hook says so — never synthesize a name.** An invented owner is worse than an empty one: it routes the question to somebody who never agreed to answer it, and nothing in the tree records that it was a guess.
