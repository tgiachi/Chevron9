#!/usr/bin/env bash
set -euo pipefail

# Simple build script using Task
# Usage: ./build.sh [task] [variables]

# Check if Task is installed
if ! command -v task >/dev/null 2>&1; then
    echo "Installing Task..."
    # Install Task (Go task runner)
    if command -v go >/dev/null 2>&1; then
        go install github.com/go-task/task/v3/cmd/task@latest
    else
        echo "Go not found. Please install Task manually: https://taskfile.dev/installation/"
        echo "Or install Go first: https://golang.org/dl/"
        exit 1
    fi
fi

# Run Task with all arguments
exec task "$@"