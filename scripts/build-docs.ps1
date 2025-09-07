# PowerShell script to build documentation with DocFX
# Requires DocFX to be installed: dotnet tool install -g docfx

param(
    [switch]$Serve,
    [switch]$Clean
)

$ErrorActionPreference = "Stop"

# Check if DocFX is installed
try {
    $docfxVersion = & docfx --version 2>$null
    Write-Host "DocFX version: $docfxVersion" -ForegroundColor Green
} catch {
    Write-Host "DocFX is not installed. Installing..." -ForegroundColor Yellow
    dotnet tool install -g docfx
}

# Clean previous build if requested
if ($Clean) {
    Write-Host "Cleaning previous documentation build..." -ForegroundColor Yellow
    if (Test-Path "_site") {
        Remove-Item "_site" -Recurse -Force
    }
    if (Test-Path "api") {
        Remove-Item "api" -Recurse -Force
    }
    Write-Host "Clean completed." -ForegroundColor Green
}

# Build the documentation
Write-Host "Building documentation..." -ForegroundColor Cyan
& docfx build

if ($LASTEXITCODE -ne 0) {
    Write-Error "DocFX build failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Documentation build completed successfully!" -ForegroundColor Green
Write-Host "Generated files are in the '_site' directory." -ForegroundColor Green

# Serve the documentation if requested
if ($Serve) {
    Write-Host "Starting documentation server..." -ForegroundColor Cyan
    Write-Host "Documentation will be available at: http://localhost:8080" -ForegroundColor Green
    Write-Host "Press Ctrl+C to stop the server." -ForegroundColor Yellow

    & docfx serve _site
}