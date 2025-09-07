# Migration Guide: Nuke → Task

This document outlines the changes made during the migration from Nuke to Task.

## What Changed

### Removed Files
- `.nuke` - Nuke bootstrap file
- `Build.cs` - Nuke build script (294 lines of C#)
- `build.cmd` - Old Windows batch script
- `build.ps1` - Old PowerShell script
- `build.sh` - Old shell script

### Added Files
- `Taskfile.yml` - New build configuration (YAML)
- `BUILD.md` - Comprehensive build documentation
- `MIGRATION.md` - This migration guide
- New simplified build scripts

## Command Translation

| Old Nuke Command | New Task Command | Description |
|------------------|------------------|-------------|
| `nuke` | `./build.sh` | Default task |
| `nuke --target DevBuild` | `./build.sh dev` | Development build |
| `nuke --target Ci` | `./build.sh ci` | CI pipeline |
| `nuke --target Clean` | `./build.sh clean` | Clean artifacts |
| `nuke --target Test` | `./build.sh test` | Run tests |
| `nuke --target Pack` | `./build.sh pack` | Create packages |
| `nuke --target Analyze` | `./build.sh analyze` | Code analysis |

## Benefits of the Migration

### ✅ Simplicity
- **Before**: 294 lines of C# build script
- **After**: ~150 lines of YAML configuration
- No compilation required
- Direct shell commands

### ✅ Performance
- Faster startup (no C# compilation)
- Direct command execution
- Better caching support

### ✅ Maintainability
- YAML is easier to read and modify
- No .NET dependencies for build system
- Clear task dependencies

### ✅ Developer Experience
- `task --list` shows all available tasks
- Better error messages
- Easier debugging

## Breaking Changes

### Variable Names
- `PreReleaseSuffix` → Not needed (use different package names)
- `Runtime` → `RUNTIME` (environment variable style)
- `Configuration` → `CONFIGURATION`

### Task Names
- `DevBuild` → `dev`
- `QuickBuild` → `quick`
- `All` → `all`
- `Ci` → `ci`

## Examples

### Before (Nuke)
```bash
# Complex parameter passing
nuke --target Compile --configuration Release --runtime win-x64

# Custom target execution
nuke --target CustomTarget --myParameter value
```

### After (Task)
```bash
# Simple variable override
./build.sh build CONFIGURATION=Release RUNTIME=win-x64

# Task with parameters
./build.sh my-task MY_PARAM=value
```

## Getting Started

1. **Install Task** (automatic with build scripts)
2. **Run help**: `./build.sh help`
3. **Development build**: `./build.sh dev`
4. **Read docs**: See `BUILD.md` for detailed usage

## Need Help?

- Run `./build.sh help` for available tasks
- Check `BUILD.md` for detailed documentation
- Task documentation: https://taskfile.dev

## Rollback

If you need to rollback to Nuke:

```bash
# This would require restoring the old files from git history
git checkout <commit-before-migration> -- .nuke Build.cs build.*
```

The migration maintains all functionality while significantly simplifying the build system.