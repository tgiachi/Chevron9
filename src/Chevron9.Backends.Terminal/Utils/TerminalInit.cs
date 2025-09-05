using System.Runtime.InteropServices;

namespace Chevron9.Backends.Terminal.Utils;

/// <summary>
///     Terminal initialization and configuration utilities
///     Handles setup and teardown of terminal state for optimal rendering
/// </summary>
public static class TerminalInit
{
    /// <summary>
    ///     Initializes the terminal for optimal game rendering
    ///     Sets up alternate screen buffer, hides cursor, and enables mouse tracking
    /// </summary>
    /// <returns>Initialization sequence string</returns>
    public static string Initialize()
    {
        using var builder = new AnsiBuilder();

        return builder
            .Append(AnsiEscapeCodes.EnterAlternateScreen)    // Use alternate screen buffer
            .Append(AnsiEscapeCodes.CursorHide)               // Hide cursor
            .Append(AnsiEscapeCodes.ClearScreen)              // Clear screen
            .Append(AnsiEscapeCodes.CursorHome)               // Move cursor to home
            .Append(AnsiEscapeCodes.EnableMouseTracking)      // Enable mouse tracking
            .Append(AnsiEscapeCodes.EnableBracketedPaste)     // Enable bracketed paste
            .Append(AnsiEscapeCodes.EnableFocusReporting)     // Enable focus reporting
            .Build();
    }

    /// <summary>
    ///     Restores the terminal to its original state
    ///     Exits alternate screen buffer, shows cursor, and disables special modes
    /// </summary>
    /// <returns>Cleanup sequence string</returns>
    public static string Cleanup()
    {
        using var builder = new AnsiBuilder();

        return builder
            .Append(AnsiEscapeCodes.ExitAlternateScreen)      // Exit alternate screen buffer
            .Append(AnsiEscapeCodes.CursorShow)               // Show cursor
            .Append(AnsiEscapeCodes.DisableMouseTracking)     // Disable mouse tracking
            .Append(AnsiEscapeCodes.DisableBracketedPaste)    // Disable bracketed paste
            .Append(AnsiEscapeCodes.DisableFocusReporting)    // Disable focus reporting
            .Append(AnsiEscapeCodes.Reset)                    // Reset all formatting
            .Build();
    }

    /// <summary>
    ///     Gets the terminal initialization sequence optimized for the current platform
    /// </summary>
    /// <returns>Platform-optimized initialization sequence</returns>
    public static string GetPlatformInitSequence()
    {
        var sequence = Initialize();

        // Add platform-specific optimizations
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows Terminal specific optimizations
            sequence += AnsiEscapeCodes.SetWindowTitle("Chevron9 Game");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Linux terminal specific optimizations
            // Could add support for different terminal emulators
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // macOS Terminal/iTerm2 specific optimizations
            // Could add support for iTerm2 features
        }

        return sequence;
    }

    /// <summary>
    ///     Creates a complete terminal setup sequence including window title
    /// </summary>
    /// <param name="title">Window title to set</param>
    /// <returns>Complete setup sequence</returns>
    public static string SetupTerminal(string title = "Chevron9 Game")
    {
        using var builder = new AnsiBuilder();

        return builder
            .Append(GetPlatformInitSequence())
            .Append(AnsiEscapeCodes.SetWindowTitle(title))
            .Build();
    }

    /// <summary>
    ///     Gets the current terminal size using ANSI escape sequences
    /// </summary>
    /// <returns>Sequence to query terminal size</returns>
    public static string GetTerminalSize()
    {
        return AnsiEscapeCodes.GetScreenSize;
    }

    /// <summary>
    ///     Creates a sequence to set up the terminal for high-performance rendering
    ///     Includes optimizations for 256-color and true color support detection
    /// </summary>
    /// <returns>High-performance setup sequence</returns>
    public static string SetupHighPerformance()
    {
        using var builder = new AnsiBuilder();

        return builder
            .Append(Initialize())
            // Test for 256-color support by setting a 256-color background
            .BgColor256(0)
            .Reset()
            // Test for true color support by setting an RGB background
            .BgColorRgb(0, 0, 0)
            .Reset()
            .Build();
    }

    /// <summary>
    ///     Checks if the current terminal supports ANSI escape sequences
    /// </summary>
    /// <returns>True if ANSI is supported</returns>
    public static bool IsAnsiSupported()
    {
        // Basic check for ANSI support
        var term = Environment.GetEnvironmentVariable("TERM");
        if (string.IsNullOrEmpty(term))
            return false;

        // Common terminals that support ANSI
        return term.Contains("xterm") ||
               term.Contains("screen") ||
               term.Contains("tmux") ||
               term.Contains("linux") ||
               term.Contains("vt100") ||
               term.Contains("ansi") ||
               term == "dumb" == false;
    }

    /// <summary>
    ///     Gets the terminal type from environment variables
    /// </summary>
    /// <returns>Terminal type string</returns>
    public static string GetTerminalType()
    {
        return Environment.GetEnvironmentVariable("TERM") ?? "unknown";
    }

    /// <summary>
    ///     Checks if the terminal supports 256 colors
    /// </summary>
    /// <returns>True if 256-color mode is supported</returns>
    public static bool Supports256Colors()
    {
        var term = GetTerminalType();
        var colorTerm = Environment.GetEnvironmentVariable("COLORTERM");

        return term.Contains("256color") ||
               colorTerm == "truecolor" ||
               colorTerm == "256color";
    }

    /// <summary>
    ///     Checks if the terminal supports true color (24-bit)
    /// </summary>
    /// <returns>True if true color is supported</returns>
    public static bool SupportsTrueColor()
    {
        var colorTerm = Environment.GetEnvironmentVariable("COLORTERM");
        return colorTerm == "truecolor" ||
               colorTerm == "24bit";
    }
}
