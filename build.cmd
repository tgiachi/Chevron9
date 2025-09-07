@echo off
REM Simple build script using Task
REM Usage: build.cmd [task] [variables]

REM Check if Task is installed
task --version >nul 2>&1
if %errorlevel% neq 0 (
    echo Installing Task...
    REM Try to install Task using Go if available
    go version >nul 2>&1
    if %errorlevel% equ 0 (
        go install github.com/go-task/task/v3/cmd/task@latest
    ) else (
        echo Go not found. Please install Task manually: https://taskfile.dev/installation/
        echo Or install Go first: https://golang.org/dl/
        exit 1
    )
)

REM Run Task with all arguments
task %*