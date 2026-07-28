#!/bin/bash
if ! command -v jq >/dev/null 2>&1; then
  echo "dotnet-format-hook.sh: jq not found on PATH, skipping format check" >&2
  exit 1
fi

INPUT=$(cat)
FILE_PATH=$(echo "$INPUT" | jq -r '.tool_input.file_path // empty')

if [[ "$FILE_PATH" == *.cs ]]; then
  SCRIPT_DIR=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
  SOLUTION_DIR="$SCRIPT_DIR/../FreelancerApp"
  FILE_PATH_POSIX=$(cygpath -u "$FILE_PATH")
  RELATIVE_FILE_PATH=$(realpath --relative-to="$SOLUTION_DIR" "$FILE_PATH_POSIX")
  (cd "$SOLUTION_DIR" && dotnet format --include "$RELATIVE_FILE_PATH") >&2
fi
exit 0
