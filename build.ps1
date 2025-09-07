[CmdletBinding()]
param(
    [Parameter(Position = 0, Mandatory = $false, ValueFromRemainingArguments = $true)]
    [System.Collections.Generic.List[string]]$Arguments
)

# Simple build script using Task
# Usage: .\build.ps1 [task] [variables]

# Check if Task is installed
if (!(Get-Command task -ErrorAction SilentlyContinue)) {
    Write-Host "Installing Task..." -ForegroundColor Yellow

    # Try to install Task using Go if available
    if (Get-Command go -ErrorAction SilentlyContinue) {
        go install github.com/go-task/task/v3/cmd/task@latest
    } else {
        Write-Host "Go not found. Please install Task manually: https://taskfile.dev/installation/" -ForegroundColor Red
        Write-Host "Or install Go first: https://golang.org/dl/" -ForegroundColor Red
        exit 1
    }
}

# Run Task with all arguments
& task @Arguments