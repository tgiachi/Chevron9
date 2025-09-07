# Chevron9 Build System

This project uses [Task](https://taskfile.dev) as its build automation tool, providing a simple and efficient alternative to complex build systems.

## Quick Start

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Task](https://taskfile.dev/installation/) (installed automatically by build scripts)

### Basic Usage

```bash
# Show available tasks
./build.sh help

# Development build (clean, restore, analyze, build, test)
./build.sh dev

# Full CI pipeline
./build.sh ci

# Quick build without tests
./build.sh quick

# Clean everything
./build.sh clean
```

### Windows (PowerShell)
```powershell
.\build.ps1 dev
```

### Windows (Command Prompt)
```cmd
build.cmd dev
```

## Available Tasks

| Task | Description |
|------|-------------|
| `help` | Show available tasks and usage |
| `clean` | Clean build artifacts |
| `restore` | Restore NuGet packages |
| `build` | Build the solution |
| `test` | Run tests |
| `pack` | Create NuGet packages |
| `docs` | Generate documentation |
| `format` | Format code |
| `lint` | Run code analysis |
| `audit` | Security audit |
| `outdated` | Check for outdated packages |
| `analyze` | Run all analysis tasks |
| `ci` | Full CI pipeline |
| `dev` | Development build |
| `all` | Build everything |
| `setup` | Setup development environment |
| `version` | Show version information |
| `check` | Quick health check |

## Variables

You can override default variables:

```bash
# Build in Debug configuration
./build.sh build CONFIGURATION=Debug

# Build for specific runtime
./build.sh build RUNTIME=linux-x64

# Build for specific framework
./build.sh build FRAMEWORK=net9.0
```

## Multi-Platform Builds

```bash
# Windows
./build.sh build-win

# Linux
./build.sh build-linux

# macOS
./build.sh build-osx
```

## Development Workflow

### Daily Development
```bash
# Quick health check
./build.sh check

# Development build with all checks
./build.sh dev

# Run tests only
./build.sh test
```

### Before Commit
```bash
# Full analysis
./build.sh analyze

# Format code
./build.sh format
```

### Release Process
```bash
# Full CI pipeline
./build.sh ci

# Create packages
./build.sh pack
```

## CI/CD Integration

The build system is integrated with GitHub Actions:

- **Build Workflow**: Runs on every push/PR with full analysis and testing
- **Docs Workflow**: Generates documentation on changes
- **Multi-Platform**: Tests across Windows, Linux, and macOS

## File Structure

```
├── Taskfile.yml          # Main build configuration
├── build.sh             # Unix/Linux build script
├── build.ps1            # Windows PowerShell build script
├── build.cmd            # Windows Command Prompt build script
├── artifacts/           # Build outputs (auto-generated)
├── _site/              # Documentation (auto-generated)
└── .github/workflows/  # CI/CD workflows
```

## Customization

### Adding New Tasks

Edit `Taskfile.yml` to add new tasks:

```yaml
my-task:
  desc: My custom task
  deps: [build]
  cmds:
    - echo "Running my task..."
    - dotnet run --project MyProject
```

### Modifying Variables

Update the `vars` section in `Taskfile.yml`:

```yaml
vars:
  CONFIGURATION: Debug  # Change default to Debug
  RUNTIME: win-x64      # Set default runtime
```

## Troubleshooting

### Task Not Found
```bash
# Install Task manually
curl -sL https://taskfile.dev/install.sh | sh

# Or using Go
go install github.com/go-task/task/v3/cmd/task@latest
```

### .NET SDK Issues
```bash
# Check .NET version
dotnet --version

# List available SDKs
dotnet --list-sdks
```

### Clean Build
```bash
# Clean everything and start fresh
./build.sh clean
./build.sh restore
./build.sh build
```

## Migration from Nuke

This build system replaces the previous Nuke-based setup with:

- **Simpler syntax**: YAML instead of C#
- **Faster execution**: Direct shell commands
- **Better readability**: Clear task definitions
- **Easier maintenance**: No compilation required
- **Same functionality**: All previous features preserved

Previous Nuke commands:
```bash
nuke --target DevBuild
nuke --target Ci
```

New Task commands:
```bash
./build.sh dev
./build.sh ci
```