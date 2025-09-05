#!/bin/bash

# Setup script for Git hooks
# This script copies the pre-commit hook to the local .git/hooks directory

echo "Setting up Git hooks..."

# Check if we're in a git repository
if [ ! -d ".git" ]; then
    echo "❌ Error: Not in a git repository"
    exit 1
fi

# Create hooks directory if it doesn't exist
mkdir -p .git/hooks

# Copy the pre-commit hook
if [ -f ".git/hooks/pre-commit" ]; then
    echo "⚠️  Pre-commit hook already exists. Backing up..."
    cp .git/hooks/pre-commit .git/hooks/pre-commit.backup
fi

# Copy our hook from the repository
cp setup-hooks.sh .git/hooks/pre-commit

# Make sure it's executable
chmod +x .git/hooks/pre-commit

echo "✅ Git hooks setup complete!"
echo "The pre-commit hook will now run 'dotnet test' before each commit."