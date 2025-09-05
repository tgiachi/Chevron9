# Chevron9 Game Engine - Claude Code Documentation

## Project Overview
**Chevron9** is a flexible 2D game engine designed to support multiple rendering backends (Terminal, MonoGame, OpenGL). Built with .NET 9.0 and modern C# features, it provides a solid foundation for 2D games and applications with performance-optimized event loops and a command-based rendering system.

## Architecture Overview
The engine follows a layered architecture with separation of concerns:

- **Chevron9.Core**: Core engine functionality (event loops, scene management, rendering commands, input handling)
- **Chevron9.Shared**: Common data structures, primitives, and utilities shared across components  
- **Chevron9.Bootstrap**: Application bootstrapping and initialization
- **Chevron9.Tests**: Comprehensive NUnit 4 test suite with 100% pure Assert.That() syntax

### Key Design Patterns
1. **Command Pattern**: Pure command pattern for rendering with `RenderCommand` hierarchy
2. **Scene Stack Management**: Stack-based scene management for overlays and transitions
3. **Fixed Timestep Loop**: High-performance timing using `Stopwatch.GetTimestamp()`
4. **Interface Segregation**: Clean interfaces for input, rendering, and scene management
5. **Single File Per Type**: Strict adherence to "1 class/struct/record = 1 file" principle

## Build System
- **Target Framework**: .NET 9.0 with preview language features
- **Quality Settings**: Treat warnings as errors, full .NET analyzers enabled
- **Build Commands**:
  - `dotnet build` - Build all projects
  - `dotnet test` - Run comprehensive test suite
  - Solution file: `Chevron9.sln`

## Git Hooks
- **Pre-commit Hook**: Automatically runs `dotnet test` before each commit
- **Purpose**: Ensures all tests pass before code is committed
- **Behavior**: If tests fail, commit is aborted with error message
- **Setup**: Run `./setup-hooks.sh` to install the hook
- **Location**: `.git/hooks/pre-commit` (automatically enabled)
- **Customization**: Edit the hook file to modify test execution parameters

## Testing Standards
- **Framework**: NUnit 4 exclusively - **NO FluentAssertions**
- **Syntax**: Pure `Assert.That(actual, Is.Expected)` format only
- **Coverage**: Comprehensive tests for all utilities, algorithms, and core functionality
- **Structure**: Mirrors source structure in tests/ directory

## Performance Optimizations
- **Timing**: `Stopwatch.GetTimestamp()` for high-precision, low-allocation timing
- **Memory**: `yield return` for enumeration, avoid intermediate collections
- **Algorithms**: Cryptographically secure random generation, efficient hashing

## Code Style Guidelines
1. **No Comments**: Code should be self-documenting unless explicitly requested
2. **XML Documentation**: Full XML docs on all public APIs
3. **Immutability**: Prefer records and readonly structs
4. **Extension Methods**: Utility methods as extensions where appropriate
5. **Nullable Reference Types**: Enabled throughout

## Key Components

### Event Loops
- `FixedEventLoop`: High-performance fixed timestep with interpolation alpha
- `VariableEventLoop`: Variable timestep for different use cases
- Configurations: `Terminal30()` (30 FPS), `Default60()` (60 FPS)

### Rendering System
- **Commands**: `DrawRectangleCommand`, `DrawCircleCommand`, `DrawTextCommand`, etc.
- **Layers**: Support for composite modes and clearing
- **Collector Pattern**: `IRenderCommandCollector` for command aggregation

### Input Handling
- **InputDevice**: Abstract input with modifier key support (`InputKeyModifierType`)
- **Mouse**: `MouseButtonType`, `MouseAction` enums
- **Keyboard**: `InputKey` with predefined keys in `InputKeys`

### Scene Management
- **Stack-based**: Push/pop/replace operations
- **Lifecycle**: Proper Enter/Exit/Update/Render methods
- **Dependency Injection**: Scoped services support

### Data Structures
- **Primitives**: `Position`, `RectI`, `RectF`, `Cell`
- **Graphics**: `Color` with common color constants
- **Extensions**: Utility methods for rectangles, colors, positions

### Utilities
- **Random**: `BuiltInRng` with cryptographic random seeding
- **Hashing**: `HashUtils` for secure hash generation
- **String**: `StringUtils` and `StringMethodExtension`
- **File System**: `DirectoriesConfig` and `DirectoriesExtension`
- **Environment**: `EnvExtensions` for environment variable handling

## Common Operations

### Building and Testing
```bash
dotnet build                    # Build entire solution
dotnet test                     # Run all tests
dotnet build --configuration Release    # Release build
```

### Adding New Components
1. Follow "1 type = 1 file" principle
2. Add corresponding test file in tests/ directory
3. Use NUnit 4 with `Assert.That()` syntax only
4. Add XML documentation for public APIs

### Performance Considerations
- Use `Stopwatch.GetTimestamp()` for timing operations
- Prefer `yield return` for enumeration
- Use readonly structs and records for immutability
- Avoid allocations in tight loops

## Project Structure
```
Chevron9/
├── src/
│   ├── Chevron9.Core/          # Core engine functionality
│   ├── Chevron9.Shared/        # Common data structures
│   └── Chevron9.Bootstrap/     # Application bootstrapping
├── tests/
│   └── Chevron9.Tests/         # NUnit 4 test suite
├── Directory.Build.props       # Common build properties
├── Chevron9.sln               # Solution file
└── CLAUDE.md                  # This documentation
```

## Development Notes
- **Internal Types**: Use `Internal` namespace for implementation details (e.g., `SceneEntry`)
- **Analyzer Suppressions**: Common suppressions in Directory.Build.props for development phase
- **Source Link**: Enabled for debugging support
- **Deterministic Builds**: Configured for reproducible builds

## Important Reminders for Claude Code
1. **NEVER use FluentAssertions** - User explicitly rejected this multiple times
2. **Use pure NUnit 4 syntax** - `Assert.That(actual, Is.Expected)` format
3. **Follow 1 file = 1 type rule** - Each class/struct/record in separate file
4. **Performance first** - Use `GetTimestamp()`, avoid allocations
5. **No comments unless requested** - Code should be self-documenting
6. **XML documentation required** - All public APIs must have XML docs

## Core Boundaries

### Chevron9.Shared: Primitives Only
- **Graphics Primitives**: `Color` (RGBA), `Position`, `RectF`, `RectI`, `Cell<TVisual>`
- **Lightweight DTOs**: `SpriteRef`, `GlyphRef` (future)
- **No Dependencies**: Pure data structures with no external references

### Chevron9.Core: Pure Engine Contracts & Core Services
**Event Loops**: `IEventLoop`, `FixedEventLoop`, `VariableEventLoop` with configs
**Rendering**: `RenderCommand` hierarchy, `IRenderer` interface, `IRenderQueue` & implementation
**Scenes**: `IScene`, `ILayer`, `ISceneManager`, layer flags/modes, `ICamera2D`, `SceneManager`
**Input**: `IInput`, `InputKey`, `MouseButtonType`

**Strict Policy**: No platform APIs, no console calls, no MonoGame references, no DI references

## Backend Architecture

### Chevron9.Backends.Terminal
- Implements `IRenderer`, `IInput`
- Double buffer of `Cell<int>` (codepoint + fg/bg colors)
- `DrawSprite` → maps to glyphs or skips (configurable)
- `DrawText` ignores `FontId` (optionally maps to SGR styles)
- Composition: `TransparentIfEmpty` = don't overwrite empty cells

### Chevron9.Backends.MonoGame
- Implements `IRenderer`, `IInput`  
- Uses `SpriteBatch` grouped by `materialKey` (texture/atlas/font)
- Pixel-perfect UI layer (point sampling), world layer scrolls/zooms

## Bootstrap & Hosts

### Chevron9.Bootstrap: Builder/Facade
- Choose loop (fixed/variable), backend (terminal/monogame), scene factory
- Extra registrations, exposes `IGameApp` with Start/Stop

### Hosts (Chevron9.Hosts.*): Composition Roots
- Own the DryIoc container
- Wire main loop (Terminal: while-loop; MonoGame: Game.Run)

## Scenes & Layers

**Scene**: Lifecycle + orchestration (Enter/Exit, Update, Render, HandleInput)
**Layer**: Render/update unit with own `ICamera2D` and Z-index

### Recommended Z-Index Presets
- **100**: Background
- **200**: World (map/tiles) 
- **300**: Entities
- **800**: FX/Overlays
- **1000**: UI
- **9000**: Debug

### Compose Modes
- **TransparentIfEmpty**: Only non-empty cells overwrite (terminal grid)
- **Overwrite**: Standard painter's order

## Rendering Pipeline

1. Layers submit `RenderCommands` to `IRenderQueue` with (layerZ, materialKey, sortKey)
2. Queue sorts by layerZ ASC, materialKey ASC, sortKey ASC  
3. Backend renderer consumes in order
4. Terminal: materialKey=0, MonoGame: use texture/font ID as materialKey

### Material Key Guidance
- **DrawSprite**: atlas/texture ID
- **DrawText**: font/atlas ID  
- **DrawGlyph/DrawQuad**: 0 (or stable key if batching helps)

## Timing & Loop Standards

- **Unit**: seconds (double precision)
- **Fixed Loop Default**: 60 Hz (`FixedEventLoopConfig(60, 0.25)`)
- **Variable Loop**: Only for tools/UI/prototypes
- **Clamp Policy**: Always clamp large frame deltas (`MaxFrameTime`)

## Input Policy

- Call `HandleInput` on top scene only
- If returns `true`, input is consumed  
- UI scenes typically consume; gameplay scenes may bubble unhandled keys

## Text & Fonts

**Terminal**: Monospace only, `FontId` ignored or mapped to SGR (bold/italic)
**MonoGame/Skia**: `FontId` maps to font asset, layout handled by backend
**Preference**: Bitmap/SDF fonts for crisp scaling, maintain fallback chain

## Dependency Injection (DryIoc)

**Scope**: Only in hosts and bootstrap
**Singleton**: `IRenderer`, `IInput`, `IEventLoop`, `IRenderQueue`, `ISceneManager`  
**Scoped**: Scenes/layers (per scene), scene changes = open/close scope

## Coding Standards

- **.NET**: C# 12, nullable enabled
- **Primitives**: Readonly structs for math types
- **Precision**: World math (float), Time (double)
- **Performance**: No LINQ in hot paths, avoid per-frame allocations, consider pooling
- **Naming**: `Chevron9.*` namespaces, files mirror folders
- **Isolation**: No platform calls outside backends

## Testing Strategy

- **Core**: Unit tests for `RenderQueue` ordering, `FixedEventLoop` behavior, `SceneManager` stack logic
- **Terminal**: Snapshot tests on `Cell<int>[,]` diff
- **Samples**: Shared scenes running on both backends to verify parity

## Performance Notes

**Terminal**: Coalesce runs on same row, prefer truecolor ANSI, cap ~30 FPS if output bound
**MonoGame**: Batch by material, prefer atlas, `PointClamp` for pixel art
**General**: Avoid per-frame string concat, reuse buffers

## Security & Safety

- No untrusted file execution, content loading read-only by default
- Telnet backend (future): sandbox input, never eval remote code  
- Logging: avoid leaking sensitive paths in public builds

## PR / Commit Etiquette

- Small, focused PRs per concern (loop, queue, terminal diff...)
- Update docs when touching contracts in Core or Shared
- Add/maintain tests for new behavior
- Commit format: `area: change` (e.g., `core/loop: add VariableEventLoop`)

## Non-Goals (Current Scope)

- 3D rendering, complex physics, network replication
- Heavy text shaping (Harfbuzz) — planned for later
- Non-standard terminal graphics (Sixel/Kitty) — optional future feature flags

This engine is designed for high-performance 2D applications with clean architecture and comprehensive testing. The command-based rendering system allows for easy backend swapping while maintaining consistent API.