#!/bin/bash

# Bash script to build documentation with DocFX
# Requires DocFX to be installed: dotnet tool install -g docfx

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Check if DocFX is installed
if ! command -v docfx &> /dev/null; then
    echo -e "${YELLOW}DocFX is not installed. Installing...${NC}"
    dotnet tool install -g docfx
fi

# Display DocFX version
echo -e "${GREEN}DocFX version: $(docfx --version)${NC}"

# Parse command line arguments
SERVE=false
CLEAN=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --serve)
            SERVE=true
            shift
            ;;
        --clean)
            CLEAN=true
            shift
            ;;
        *)
            echo "Unknown option: $1"
            echo "Usage: $0 [--serve] [--clean]"
            exit 1
            ;;
    esac
done

# Clean previous build if requested
if [ "$CLEAN" = true ]; then
    echo -e "${YELLOW}Cleaning previous documentation build...${NC}"
    rm -rf _site
    rm -rf api
    echo -e "${GREEN}Clean completed.${NC}"
fi

# Build the documentation
echo -e "${CYAN}Building documentation...${NC}"
docfx build

if [ $? -ne 0 ]; then
    echo -e "${RED}DocFX build failed!${NC}"
    exit 1
fi

echo -e "${GREEN}Documentation build completed successfully!${NC}"
echo -e "${GREEN}Generated files are in the '_site' directory.${NC}"

# Serve the documentation if requested
if [ "$SERVE" = true ]; then
    echo -e "${CYAN}Starting documentation server...${NC}"
    echo -e "${GREEN}Documentation will be available at: http://localhost:8080${NC}"
    echo -e "${YELLOW}Press Ctrl+C to stop the server.${NC}"
    docfx serve _site
fi