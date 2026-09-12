#!/usr/bin/env bash

set -euo pipefail

CODEX_HOME="${CODEX_HOME:-$HOME/.codex}"
DRY_RUN=0

usage() {
    cat <<'EOF'
Usage: ./install-onnxify-skills.sh [options]

Options:
  --codex-home <path>  Override the Codex home directory.
                       Default: $CODEX_HOME or $HOME/.codex
  -n, --dry-run        Print the actions without executing them.
  -h, --help           Show this help message.
EOF
}

fail() {
    printf '%s\n' "$1" >&2
    exit 1
}

run_command() {
    if [[ "$DRY_RUN" -eq 1 ]]; then
        printf '+'
        printf ' %q' "$@"
        printf '\n'
        return
    fi

    "$@"
}

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
SOURCE_ROOT="$SCRIPT_DIR/.agents/skills"
TARGET_ROOT="$CODEX_HOME/skills"
SKILL_NAMES=(
    "panlingo"
)

while [[ $# -gt 0 ]]; do
    case "$1" in
        --codex-home)
            [[ $# -ge 2 ]] || fail "Missing value for $1"
            CODEX_HOME="$2"
            TARGET_ROOT="$CODEX_HOME/skills"
            shift 2
            ;;
        -n|--dry-run)
            DRY_RUN=1
            shift
            ;;
        -h|--help)
            usage
            exit 0
            ;;
        *)
            fail "Unknown argument: $1"
            ;;
    esac
done

install_or_update_skill() {
    local skill_name="$1"
    local source_path="$SOURCE_ROOT/$skill_name"
    local target_path="$TARGET_ROOT/$skill_name"

    [[ -d "$source_path" ]] || fail "Skill source was not found: $source_path"

    if [[ -e "$target_path" ]]; then
        run_command rm -rf "$target_path"
    fi

    run_command cp -R "$source_path" "$target_path"

    if [[ "$DRY_RUN" -eq 0 ]]; then
        printf "Updated skill '%s' -> %s\n" "$skill_name" "$target_path"
    fi
}

if [[ ! -d "$TARGET_ROOT" ]]; then
    run_command mkdir -p "$TARGET_ROOT"
fi

for skill_name in "${SKILL_NAMES[@]}"; do
    install_or_update_skill "$skill_name"
done

printf '\nDone. Restart Codex if it is already running so it picks up the refreshed skills.\n'
