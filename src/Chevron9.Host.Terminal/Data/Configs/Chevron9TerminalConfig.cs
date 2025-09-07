using Chevron9.Bootstrap.Data.Configs;
using Chevron9.Bootstrap.Types;

namespace Chevron9.Host.Terminal.Data.Configs;

/// <summary>
///     Configuration specific to the Chevron9 Terminal Host
///     Extends base Chevron9Config with terminal-specific settings
/// </summary>
public sealed class Chevron9TerminalConfig : Chevron9Config
{
    /// <summary>
    ///     Enable terminal input processing
    /// </summary>
    public bool EnableInput { get; set; } = true;

    /// <summary>
    ///     Enable terminal output rendering
    /// </summary>
    public bool EnableOutput { get; set; } = true;

    /// <summary>
    ///     Terminal buffer width (0 = auto-detect)
    /// </summary>
    public int BufferWidth { get; set; }

    /// <summary>
    ///     Terminal buffer height (0 = auto-detect)
    /// </summary>
    public int BufferHeight { get; set; }

    /// <summary>
    ///     Enable mouse input support
    /// </summary>
    public bool EnableMouse { get; set; } = true;

    /// <summary>
    ///     Enable alternative screen buffer
    /// </summary>
    public bool UseAlternativeScreenBuffer { get; set; } = true;

    /// <summary>
    ///     Terminal refresh rate in milliseconds
    /// </summary>
    public int RefreshRateMs { get; set; } = 16; // ~60 FPS

    /// <summary>
    ///     Enable cursor visibility
    /// </summary>
    public bool ShowCursor { get; set; }

    /// <summary>
    ///     Creates a new terminal configuration with default values
    /// </summary>
    public Chevron9TerminalConfig()
    {
        // Set appropriate defaults for terminal applications
        LogLevel = LogLevelType.Information;
        LogToConsole = false; // Disable console logging to avoid interference with terminal UI
        LogToFile = true;
    }
}
