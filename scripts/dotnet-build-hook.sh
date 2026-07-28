#!/bin/bash
if ! command -v jq >/dev/null 2>&1; then
  echo "dotnet-build-hook.sh: jq not found on PATH, skipping build check" >&2
  exit 1
fi

INPUT=$(cat)
FILE_PATH=$(echo "$INPUT" | jq -r '.tool_input.file_path // empty')

if [[ "$FILE_PATH" == *.cs ]]; then
  SCRIPT_DIR=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
  SOLUTION_DIR="$SCRIPT_DIR/../FreelancerApp"
  BUILD_OUTPUT=$(cd "$SOLUTION_DIR" && dotnet build 2>&1)
  if [ $? -ne 0 ]; then
    echo "$BUILD_OUTPUT" >&2
    exit 2
  fi
fi
exit 0
