#!/usr/bin/env bash
# Repair the .claude/{agents,skills} -> ../.github/{agents,skills} symlinks.
#
# The repo commits these as real symlinks (mode 120000), which work natively on
# macOS/Linux and on Windows with Developer Mode enabled. On Windows checkouts
# where git wrote them as plain text files, this script (or its .cmd sibling)
# recreates them as proper links so .claude and .github share one source of truth.
#
# Idempotent: safe to run any number of times.
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

link() {
  local name="$1"               # e.g. agents
  local target="../.github/$name"
  local linkpath=".claude/$name"

  if [ ! -d ".github/$name" ]; then
    echo "ERROR: source .github/$name does not exist" >&2
    return 1
  fi

  # Already a correct symlink? Nothing to do.
  if [ -L "$linkpath" ] && [ "$(readlink "$linkpath")" = "$target" ]; then
    echo "ok: $linkpath -> $target"
    return 0
  fi

  # Remove a stale link, a git-materialised text file, or a wrong-target link.
  rm -rf "$linkpath"
  ln -s "$target" "$linkpath"
  echo "linked: $linkpath -> $target"
}

link agents
link skills
echo "Done. .claude now mirrors .github/{agents,skills}."
