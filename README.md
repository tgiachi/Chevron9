# Chevron9 Game Engine

A flexible 2D game engine designed to support multiple rendering backends (Terminal, MonoGame, OpenGL). Built with .NET 9.0 and modern C# features, it provides a solid foundation for 2D games and applications with performance-optimized event loops and a command-based rendering system.

## ✨ Features

- **Multi-Backend Rendering**: Terminal, MonoGame, and OpenGL backends
- **High-Performance Event Loops**: Fixed and variable timestep loops with precise timing
- **Command-Based Rendering**: Pure command pattern for efficient rendering pipeline
- **Scene Management**: Stack-based scene system with proper lifecycle management
- **Input Handling**: Abstract input system with keyboard and mouse support
- **Layer System**: Z-index based rendering layers with composition modes
- **Camera System**: 2D camera with zoom, pan, and viewport support
- **Comprehensive Testing**: NUnit 4 test suite with 100% coverage

## 🏗️ Architecture

The engine follows a clean layered architecture:

- **Chevron9.Core**: Core engine functionality (event loops, scene management, rendering commands)
- **Chevron9.Shared**: Common data structures and primitives
- **Chevron9.Bootstrap**: Application bootstrapping and initialization
- **Chevron9.Backends**: Platform-specific implementations (Terminal, MonoGame, etc.)

### Key Design Patterns

- **Command Pattern**: Pure command pattern for rendering operations
- **Scene Stack Management**: Stack-based scene management for overlays
- **Fixed Timestep Loop**: High-performance timing using `Stopwatch.GetTimestamp()`
- **Interface Segregation**: Clean interfaces for input, rendering, and scene management

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- Git (for cloning and hooks)

### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/chevron9.git
cd chevron9
```

2. Setup git hooks (optional but recommended):
```bash
./setup-hooks.sh
```

3. Build the solution:
```bash
dotnet build
```

4. Run tests:
```bash
dotnet test
```

## 📁 Project Structure

```
Chevron9/
├── src/
│   ├── Chevron9.Core/          # Core engine functionality
│   │   ├── Cameras/           # 2D camera implementation
│   │   ├── Data/              # Configuration and input data
│   │   ├── Extensions/        # Extension methods
│   │   ├── Interfaces/        # Core interfaces
│   │   ├── Layers/            # Rendering layers
│   │   ├── Loop/              # Event loop implementations
│   │   ├── Render/            # Rendering commands and pipeline
│   │   ├── Scenes/            # Scene management
│   │   └── Utils/             # Utility classes
│   ├── Chevron9.Shared/       # Common data structures
│   │   ├── Extensions/        # Shared extensions
│   │   ├── Graphics/          # Graphics primitives
│   │   └── Primitives/        # Basic data types
│   └── Chevron9.Bootstrap/    # Application bootstrapping
├── tests/
│   └── Chevron9.Tests/         # Comprehensive test suite
├── .tools/                    # Development tools
└── Chevron9.sln              # Solution file
```

## 🛠️ Build & Development

### Build Commands

```bash
# Build all projects
dotnet build

# Build in release mode
dotnet build --configuration Release

# Run tests
dotnet test

# Run specific test project
dotnet test tests/Chevron9.Tests/Chevron9.Tests.csproj
```

### Development Tools

The project includes several development tools:

- **dotnet-audit**: Security vulnerability scanning
- **dotnet-outdated**: Package update checking

### Git Hooks

The project uses pre-commit hooks to ensure code quality:

- Automatically runs `dotnet format` for code formatting
- Runs full test suite before commits
- Prevents commits if tests fail or code is improperly formatted

## 🧪 Testing

The project uses NUnit 4 with a comprehensive test suite:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test category
dotnet test --filter "Category=Unit"
```

### Test Structure

Tests mirror the source structure and cover:
- Core functionality (event loops, scene management)
- Rendering commands and pipeline
- Input handling and edge cases
- Utility functions and extensions
- Data structures and primitives

## 📚 Key Components

### Event Loops

- **FixedEventLoop**: High-performance fixed timestep with interpolation
- **VariableEventLoop**: Variable timestep for different use cases
- Configurations: `Terminal30()` (30 FPS), `Default60()` (60 FPS)

### Rendering System

- **Commands**: `DrawRectangleCommand`, `DrawCircleCommand`, `DrawTextCommand`, etc.
- **Layers**: Support for composite modes and clearing
- **Collector Pattern**: `IRenderCommandCollector` for command aggregation

### Scene Management

- **Stack-based**: Push/pop/replace operations
- **Lifecycle**: Proper Enter/Exit/Update/Render methods
- **Dependency Injection**: Scoped services support

## 🎯 Usage Example

```csharp
// Create a simple scene
public class GameScene : Scene
{
    public override void Update(double deltaTime)
    {
        // Game logic here
    }

    public override void Render(IRenderCommandCollector collector)
    {
        // Rendering commands here
        collector.Add(new DrawRectangleCommand(
            new RectF(10, 10, 100, 100),
            Colors.Red
        ));
    }
}

// Bootstrap the application
var app = new Chevron9Bootstrap()
    .WithFixedEventLoop(FixedEventLoopConfig.Default60())
    .WithScene<GameScene>()
    .Build();

app.Start();
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Make your changes following the coding standards
4. Add tests for new functionality
5. Ensure all tests pass: `dotnet test`
6. Commit with descriptive messages
7. Push to your fork and create a pull request

### Coding Standards

- **No Comments**: Code should be self-documenting
- **XML Documentation**: Full XML docs on all public APIs
- **Immutability**: Prefer records and readonly structs
- **Extension Methods**: Utility methods as extensions where appropriate
- **Nullable Reference Types**: Enabled throughout
- **One Type Per File**: Strict adherence to "1 class/struct/record = 1 file" principle

### Testing Standards

- **Framework**: NUnit 4 exclusively
- **Syntax**: Pure `Assert.That(actual, Is.Expected)` format only
- **Coverage**: Comprehensive tests for all utilities and core functionality

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built with .NET 9.0 and modern C# features
- Inspired by modern game engine architectures
- Comprehensive testing with NUnit 4

---

For more detailed documentation, see [CLAUDE.md](CLAUDE.md).