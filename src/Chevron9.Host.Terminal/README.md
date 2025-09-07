# Chevron9.Host.Terminal

Complete host for terminal applications that combines Chevron9.Bootstrap with terminal backend services (Chevron9.Backends.Terminal).

## Features

- 🔧 **Complete Integration**: Combines Bootstrap and terminal backend into a single host
- ⚡ **Service Management**: Automatic loading and startup of services with priority
- 🎨 **Rendering**: Real-time rendering support with double buffering
- 🖱️ **Input**: Keyboard and mouse input handling with asynchronous events
- 🔄 **Lifecycle**: Complete application lifecycle management
- ⚙️ **Configurable**: Flexible configuration for different needs
- 📝 **Logging**: Serilog integration for structured logging

## Quick Start

### Basic Application

```csharp
using Chevron9.Host.Terminal.Extensions;

// Create and start a terminal application
using var host = Chevron9TerminalHostExtensions.CreateDefaultHost();

// Register your services
host.OnRegisterServices += container =>
{
    container.AddService<MyAppService>(priority: 50);
};

// Graceful Ctrl+C handling
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

// Start the application
await host.RunAsync(cts.Token);
```

### Custom Configuration

```csharp
using var host = Chevron9TerminalHostExtensions.CreateHost(config =>
{
    config.RootDirectory = "./my-app";
    config.LogLevel = LogLevelType.Information;
    config.RefreshRateMs = 33; // 30 FPS
    config.EnableMouse = true;
    config.ShowCursor = false;
});
```

### Fluent Configuration

```csharp
using var host = Chevron9TerminalHostExtensions.CreateDefaultHost()
    .WithRefreshRate(16)     // 60 FPS
    .WithMouse(true)         // Enable mouse
    .WithCursor(false)       // Hide cursor
    .WithDimensions(80, 24); // Fixed dimensions
```

## Pre-configured Builders

### Development Host
```csharp
var host = Chevron9TerminalHostExtensions.CreateDevelopmentHost("./dev-app");
// - LogLevel: Debug
// - LogToFile: true
// - ShowCursor: true
// - RefreshRate: 30 FPS
```

### Production Host
```csharp
var host = Chevron9TerminalHostExtensions.CreateProductionHost("./prod-app");
// - LogLevel: Warning
// - LogToFile: true
// - ShowCursor: false
// - RefreshRate: 60 FPS
```

## Manual Control

```csharp
using var host = Chevron9TerminalHostExtensions.CreateDefaultHost();

host.OnRegisterServices += container =>
{
    container.AddService<MyService>(priority: 50);
};

// Manual lifecycle control
await host.InitializeAsync();
await host.StartAsync();

// Your application logic here
await DoApplicationWork();

// Shutdown
await host.StopAsync();
```

## Terminal Services Access

```csharp
using var host = Chevron9TerminalHostExtensions.CreateDefaultHost();
await host.InitializeAsync();

// Access terminal services (available after InitializeAsync)
var renderService = host.RenderService;
var inputService = host.InputService;

// Access renderer for draw commands
var renderer = renderService?.Renderer;
if (renderer != null)
{
    renderer.BeginFrame();
    // ... draw commands
    renderer.EndFrame();
}
```

## Creating Custom Services

```csharp
using Chevron9.Bootstrap.Interfaces.Base;

public class MyAppService : IChevronAutostartService
{
    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        // Service initialization
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        // Service startup (autostart)
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        // Service shutdown
        return Task.CompletedTask;
    }

    public Task UnloadAsync(CancellationToken cancellationToken = default)
    {
        // Service cleanup
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // Dispose resources
    }
}
```

## Configuration

### Chevron9TerminalConfig

- **EnableInput/EnableOutput**: Enable/disable input and output
- **BufferWidth/BufferHeight**: Buffer dimensions (0 = auto-detect)
- **RefreshRateMs**: Refresh rate in milliseconds
- **EnableMouse**: Mouse support
- **ShowCursor**: Cursor visibility
- **UseAlternativeScreenBuffer**: Alternative screen buffer
- Inherits all `Chevron9Config` options (logging, directories, etc.)

### Specialized Modes

```csharp
// Output only (no input)
host.WithOutputOnly();

// Input only (no rendering)
host.WithInputOnly();

// Headless mode (no I/O)
host.WithHeadlessMode();
```

## Architecture

```
Chevron9TerminalHost
├── Chevron9Bootstrap (service lifecycle management)
├── TerminalRenderService (ConsoleRender wrapper)
├── TerminalInputService (ConsoleInputDevice wrapper)
└── Your Application Services
```

### Service Priorities

- TerminalRenderService: priority 100
- TerminalInputService: priority 90
- Your services: custom priority
- EventDispatcherService: priority 0 (built-in)

## Thread Safety

- ✅ Services lifecycle is thread-safe
- ✅ Event dispatching is thread-safe
- ⚠️ ConsoleRender requires synchronization for concurrent calls
- ⚠️ ConsoleInputDevice uses internal thread-safe polling

## Performance

- **Double Buffering**: Efficient rendering without flickering
- **1D Arrays**: Memory-optimized buffers
- **Configurable Refresh Rate**: Balance performance/responsiveness
- **Frame-based Input**: Optimized polling for real-time applications

## Logging

The logging system is automatically configured:
- Log files saved in `{RootDirectory}/Logs/`
- Console logging disabled by default to avoid UI interference
- Levels: Trace, Debug, Information, Warning, Error, Critical

## Complete Examples

See the `Examples/` folder for complete implementations of:
- Interactive console applications
- Game loops with real-time rendering
- Custom event handlers
- Integration with external services