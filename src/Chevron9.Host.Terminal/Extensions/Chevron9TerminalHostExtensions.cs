using Chevron9.Host.Terminal.Data.Configs;
using Chevron9.Bootstrap.Types;

namespace Chevron9.Host.Terminal.Extensions;

/// <summary>
///     Extension methods for Chevron9TerminalHost to provide convenient builder patterns
/// </summary>
public static class Chevron9TerminalHostExtensions
{
    /// <summary>
    ///     Creates a new terminal host with default configuration
    /// </summary>
    /// <returns>New terminal host instance</returns>
    public static Chevron9TerminalHost CreateDefaultHost()
    {
        var config = new Chevron9TerminalConfig();
        return new Chevron9TerminalHost(config);
    }

    /// <summary>
    ///     Creates a new terminal host with custom configuration
    /// </summary>
    /// <param name="configureAction">Configuration action</param>
    /// <returns>New terminal host instance</returns>
    public static Chevron9TerminalHost CreateHost(Action<Chevron9TerminalConfig> configureAction)
    {
        var config = new Chevron9TerminalConfig();
        configureAction(config);
        return new Chevron9TerminalHost(config);
    }

    /// <summary>
    ///     Creates a new terminal host with minimal logging for production use
    /// </summary>
    /// <param name="rootDirectory">Root directory for the application</param>
    /// <returns>New terminal host instance</returns>
    public static Chevron9TerminalHost CreateProductionHost(string rootDirectory)
    {
        var config = new Chevron9TerminalConfig
        {
            RootDirectory = rootDirectory,
            LogLevel = LogLevelType.Warning,
            LogToConsole = false,
            LogToFile = true,
            ShowCursor = false,
            UseAlternativeScreenBuffer = true
        };
        return new Chevron9TerminalHost(config);
    }

    /// <summary>
    ///     Creates a new terminal host with debug logging for development use
    /// </summary>
    /// <param name="rootDirectory">Root directory for the application</param>
    /// <returns>New terminal host instance</returns>
    public static Chevron9TerminalHost CreateDevelopmentHost(string rootDirectory)
    {
        var config = new Chevron9TerminalConfig
        {
            RootDirectory = rootDirectory,
            LogLevel = LogLevelType.Debug,
            LogToConsole = false, // Still disabled to avoid interfering with terminal UI
            LogToFile = true,
            ShowCursor = true,
            UseAlternativeScreenBuffer = true,
            RefreshRateMs = 33 // ~30 FPS for development
        };
        return new Chevron9TerminalHost(config);
    }

    /// <summary>
    ///     Configures the terminal host for headless operation (no input/output)
    /// </summary>
    /// <param name="host">Terminal host to configure</param>
    /// <returns>The same host instance for chaining</returns>
    public static Chevron9TerminalHost WithHeadlessMode(this Chevron9TerminalHost host)
    {
        host.Configuration.EnableInput = false;
        host.Configuration.EnableOutput = false;
        return host;
    }

    /// <summary>
    ///     Configures the terminal host for input-only operation (no rendering)
    /// </summary>
    /// <param name="host">Terminal host to configure</param>
    /// <returns>The same host instance for chaining</returns>
    public static Chevron9TerminalHost WithInputOnly(this Chevron9TerminalHost host)
    {
        host.Configuration.EnableOutput = false;
        return host;
    }

    /// <summary>
    ///     Configures the terminal host for output-only operation (no input)
    /// </summary>
    /// <param name="host">Terminal host to configure</param>
    /// <returns>The same host instance for chaining</returns>
    public static Chevron9TerminalHost WithOutputOnly(this Chevron9TerminalHost host)
    {
        host.Configuration.EnableInput = false;
        return host;
    }

    /// <summary>
    ///     Sets custom terminal dimensions
    /// </summary>
    /// <param name="host">Terminal host to configure</param>
    /// <param name="width">Buffer width</param>
    /// <param name="height">Buffer height</param>
    /// <returns>The same host instance for chaining</returns>
    public static Chevron9TerminalHost WithDimensions(this Chevron9TerminalHost host, int width, int height)
    {
        host.Configuration.BufferWidth = width;
        host.Configuration.BufferHeight = height;
        return host;
    }

    /// <summary>
    ///     Sets the refresh rate for the terminal
    /// </summary>
    /// <param name="host">Terminal host to configure</param>
    /// <param name="refreshRateMs">Refresh rate in milliseconds</param>
    /// <returns>The same host instance for chaining</returns>
    public static Chevron9TerminalHost WithRefreshRate(this Chevron9TerminalHost host, int refreshRateMs)
    {
        host.Configuration.RefreshRateMs = refreshRateMs;
        return host;
    }

    /// <summary>
    ///     Enables or disables mouse support
    /// </summary>
    /// <param name="host">Terminal host to configure</param>
    /// <param name="enableMouse">Whether to enable mouse support</param>
    /// <returns>The same host instance for chaining</returns>
    public static Chevron9TerminalHost WithMouse(this Chevron9TerminalHost host, bool enableMouse = true)
    {
        host.Configuration.EnableMouse = enableMouse;
        return host;
    }

    /// <summary>
    ///     Enables or disables cursor visibility
    /// </summary>
    /// <param name="host">Terminal host to configure</param>
    /// <param name="showCursor">Whether to show the cursor</param>
    /// <returns>The same host instance for chaining</returns>
    public static Chevron9TerminalHost WithCursor(this Chevron9TerminalHost host, bool showCursor = true)
    {
        host.Configuration.ShowCursor = showCursor;
        return host;
    }
}
