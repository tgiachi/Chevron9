using System.Diagnostics.CodeAnalysis;

namespace Chevron9.Backends.Terminal.Utils;

/// <summary>
///     ANSI escape codes for terminal control and styling
///     Provides constants for common ANSI escape sequences
/// </summary>
public static class AnsiEscapeCodes
{
    // Control sequences
    public const string Escape = "\e[";
    public const string Reset = "\e[0m";
    public const string ClearScreen = "\e[2J";
    public const string ClearLine = "\e[2K";
    public const string CursorHome = "\e[H";
    public const string CursorHide = "\e[?25l";
    public const string CursorShow = "\e[?25h";
    public const string SaveCursor = "\e[s";
    public const string RestoreCursor = "\e[u";

    // Text styles
    public const string Bold = "\e[1m";
    public const string Dim = "\e[2m";
    public const string Underline = "\e[4m";
    public const string Blink = "\e[5m";
    public const string Reverse = "\e[7m";
    public const string Hidden = "\e[8m";
    public const string Strikethrough = "\e[9m";

    // Foreground colors (standard 16 colors)
    public const string Black = "\e[30m";
    public const string Red = "\e[31m";
    public const string Green = "\e[32m";
    public const string Yellow = "\e[33m";
    public const string Blue = "\e[34m";
    public const string Magenta = "\e[35m";
    public const string Cyan = "\e[36m";
    public const string White = "\e[37m";
    public const string Default = "\e[39m";

    // Bright foreground colors
    public const string BrightBlack = "\e[90m";
    public const string BrightRed = "\e[91m";
    public const string BrightGreen = "\e[92m";
    public const string BrightYellow = "\e[93m";
    public const string BrightBlue = "\e[94m";
    public const string BrightMagenta = "\e[95m";
    public const string BrightCyan = "\e[96m";
    public const string BrightWhite = "\e[97m";

    // Background colors (standard 16 colors)
    public const string BgBlack = "\e[40m";
    public const string BgRed = "\e[41m";
    public const string BgGreen = "\e[42m";
    public const string BgYellow = "\e[43m";
    public const string BgBlue = "\e[44m";
    public const string BgMagenta = "\e[45m";
    public const string BgCyan = "\e[46m";
    public const string BgWhite = "\e[47m";
    public const string BgDefault = "\e[49m";

    // Bright background colors
    public const string BgBrightBlack = "\e[100m";
    public const string BgBrightRed = "\e[101m";
    public const string BgBrightGreen = "\e[102m";
    public const string BgBrightYellow = "\e[103m";
    public const string BgBrightBlue = "\e[104m";
    public const string BgBrightMagenta = "\e[105m";
    public const string BgBrightCyan = "\e[106m";
    public const string BgBrightWhite = "\e[107m";

    // Cursor movement
    public static string CursorUp(int n = 1) => $"\e[{n}A";
    public static string CursorDown(int n = 1) => $"\e[{n}B";
    public static string CursorForward(int n = 1) => $"\e[{n}C";
    public static string CursorBack(int n = 1) => $"\e[{n}D";
    public static string CursorPosition(int row, int col) => $"\e[{row};{col}H";

    // Screen and line clearing
    public const string ClearScreenFromCursor = "\e[0J";
    public const string ClearScreenToCursor = "\e[1J";
    public const string ClearLineFromCursor = "\e[0K";
    public const string ClearLineToCursor = "\e[1K";

    // 256-color support
    public static string FgColor256(int color) => $"\e[38;5;{color}m";
    public static string BgColor256(int color) => $"\e[48;5;{color}m";

    // True color (24-bit) support
    public static string FgColorRgb(int r, int g, int b) => $"\e[38;2;{r};{g};{b}m";
    public static string BgColorRgb(int r, int g, int b) => $"\e[48;2;{r};{g};{b}m";

    // Alternative screen buffer
    public const string EnterAlternateScreen = "\e[?1049h";
    public const string ExitAlternateScreen = "\e[?1049l";

    // Mouse support
    public const string EnableMouseTracking = "\e[?1000h\e[?1002h\e[?1015h\e[?1006h";
    public const string DisableMouseTracking = "\e[?1006l\e[?1015l\e[?1002l\e[?1000l";

    // Window manipulation
    public const string GetWindowSize = "\e[14t";
    public const string GetScreenSize = "\e[15t";
    public static string SetWindowTitle(string title) => $"\e]0;{title}\a";
    public static string SetIconName(string name) => $"\e]1;{name}\a";

    // Bracketed paste mode
    public const string EnableBracketedPaste = "\e[?2004h";
    public const string DisableBracketedPaste = "\e[?2004l";

    // Focus reporting
    public const string EnableFocusReporting = "\e[?1004h";
    public const string DisableFocusReporting = "\e[?1004l";

    // Additional text styles
    public const string Italic = "\e[3m";
    public const string DoubleUnderline = "\e[21m";
    public const string Overline = "\e[53m";
    public const string Superscript = "\e[73m";
    public const string Subscript = "\e[74m";

    // Common RGB colors (convenience constants)
    public static readonly (int R, int G, int B) ColorOrange = (255, 165, 0);
    public static readonly (int R, int G, int B) ColorPurple = (128, 0, 128);
    public static readonly (int R, int G, int B) ColorPink = (255, 192, 203);
    public static readonly (int R, int G, int B) ColorGray = (128, 128, 128);
    public static readonly (int R, int G, int B) ColorLightGray = (211, 211, 211);
    public static readonly (int R, int G, int B) ColorDarkGray = (64, 64, 64);

    // Terminal control characters
    public const string Bell = "\a";
    public const string Backspace = "\b";
    public const string Tab = "\t";
    public const string LineFeed = "\n";
    public const string CarriageReturn = "\r";
    public const string FormFeed = "\f";
    public const string VerticalTab = "\v";

    // Screen manipulation
    public const string ScrollUp = "\e[M";
    public const string ScrollDown = "\e[L";
    public static string ScrollRegion(int top, int bottom) => $"\e[{top};{bottom}r";
    public const string ResetScrollRegion = "\e[r";

    // Cursor styles (some terminals)
    public const string CursorBlock = "\e[2 q";
    public const string CursorUnderline = "\e[4 q";
    public const string CursorBar = "\e[6 q";

    // Hyperlink support (some terminals like iTerm2, GNOME Terminal)
    public static string Hyperlink(string url, string text = "") => $"\e]8;;{url}\e\\{text}\e]8;;\e\\";

    /// <summary>
    /// Combines multiple ANSI sequences into a single sequence
    /// </summary>
    public static string Combine(params string[] sequences)
    {
        return string.Concat(sequences);
    }

    /// <summary>
    /// Creates a style sequence combining foreground color, background color, and text styles
    /// </summary>
    public static string Style(string fgColor = "", string bgColor = "", params string[] styles)
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(fgColor)) parts.Add(fgColor);
        if (!string.IsNullOrEmpty(bgColor)) parts.Add(bgColor);
        parts.AddRange(styles.Where(s => !string.IsNullOrEmpty(s)));

        return string.Concat(parts);
    }

    /// <summary>
    /// Creates a colored text sequence with automatic reset
    /// </summary>
    public static string ColoredText(string text, string fgColor = "", string bgColor = "", params string[] styles)
    {
        if (string.IsNullOrEmpty(fgColor) && string.IsNullOrEmpty(bgColor) && styles.Length == 0)
            return text;

        var styleSequence = Style(fgColor, bgColor, styles);
        return $"{styleSequence}{text}{Reset}";
    }

    /// <summary>
    /// Creates a gradient text effect using 256-color mode
    /// </summary>
    public static string GradientText(string text, int startColor, int endColor)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= 1)
            return ColoredText(text, FgColor256(startColor));

        var result = new System.Text.StringBuilder();
        var colorStep = (endColor - startColor) / (text.Length - 1);

        for (int i = 0; i < text.Length; i++)
        {
            var color = startColor + (int)(colorStep * i);
            color = Math.Clamp(color, 0, 255);
            result.Append($"{FgColor256(color)}{text[i]}");
        }

        result.Append(Reset);
        return result.ToString();
    }

    /// <summary>
    /// Saves the current cursor position and screen content
    /// </summary>
    public const string SaveScreen = "\e[?47h";

    /// <summary>
    /// Restores the previously saved cursor position and screen content
    /// </summary>
    public const string RestoreScreen = "\e[?47l";

    /// <summary>
    /// Enables synchronized output mode (prevents flickering during updates)
    /// </summary>
    public const string EnableSyncOutput = "\e[?2026h";

    /// <summary>
    /// Disables synchronized output mode
    /// </summary>
    public const string DisableSyncOutput = "\e[?2026l";
}
