namespace Chevron9.Backends.Terminal.Utils;

/// <summary>
///     ANSI escape codes for terminal control and styling
///     Provides constants for common ANSI escape sequences
/// </summary>
public static class AnsiEscapeCodes
{
    // Control sequences
    public const string Escape = "\u001b[";
    public const string Reset = "\u001b[0m";
    public const string ClearScreen = "\u001b[2J";
    public const string ClearLine = "\u001b[2K";
    public const string CursorHome = "\u001b[H";
    public const string CursorHide = "\u001b[?25l";
    public const string CursorShow = "\u001b[?25h";
    public const string SaveCursor = "\u001b[s";
    public const string RestoreCursor = "\u001b[u";

    // Text styles
    public const string Bold = "\u001b[1m";
    public const string Dim = "\u001b[2m";
    public const string Underline = "\u001b[4m";
    public const string Blink = "\u001b[5m";
    public const string Reverse = "\u001b[7m";
    public const string Hidden = "\u001b[8m";
    public const string Strikethrough = "\u001b[9m";

    // Foreground colors (standard 16 colors)
    public const string Black = "\u001b[30m";
    public const string Red = "\u001b[31m";
    public const string Green = "\u001b[32m";
    public const string Yellow = "\u001b[33m";
    public const string Blue = "\u001b[34m";
    public const string Magenta = "\u001b[35m";
    public const string Cyan = "\u001b[36m";
    public const string White = "\u001b[37m";
    public const string Default = "\u001b[39m";

    // Bright foreground colors
    public const string BrightBlack = "\u001b[90m";
    public const string BrightRed = "\u001b[91m";
    public const string BrightGreen = "\u001b[92m";
    public const string BrightYellow = "\u001b[93m";
    public const string BrightBlue = "\u001b[94m";
    public const string BrightMagenta = "\u001b[95m";
    public const string BrightCyan = "\u001b[96m";
    public const string BrightWhite = "\u001b[97m";

    // Background colors (standard 16 colors)
    public const string BgBlack = "\u001b[40m";
    public const string BgRed = "\u001b[41m";
    public const string BgGreen = "\u001b[42m";
    public const string BgYellow = "\u001b[43m";
    public const string BgBlue = "\u001b[44m";
    public const string BgMagenta = "\u001b[45m";
    public const string BgCyan = "\u001b[46m";
    public const string BgWhite = "\u001b[47m";
    public const string BgDefault = "\u001b[49m";

    // Bright background colors
    public const string BgBrightBlack = "\u001b[100m";
    public const string BgBrightRed = "\u001b[101m";
    public const string BgBrightGreen = "\u001b[102m";
    public const string BgBrightYellow = "\u001b[103m";
    public const string BgBrightBlue = "\u001b[104m";
    public const string BgBrightMagenta = "\u001b[105m";
    public const string BgBrightCyan = "\u001b[106m";
    public const string BgBrightWhite = "\u001b[107m";

    // Cursor movement
    public static string CursorUp(int n = 1) => $"\u001b[{n}A";
    public static string CursorDown(int n = 1) => $"\u001b[{n}B";
    public static string CursorForward(int n = 1) => $"\u001b[{n}C";
    public static string CursorBack(int n = 1) => $"\u001b[{n}D";
    public static string CursorPosition(int row, int col) => $"\u001b[{row};{col}H";

    // Screen and line clearing
    public static string ClearScreenFromCursor = "\u001b[0J";
    public static string ClearScreenToCursor = "\u001b[1J";
    public static string ClearLineFromCursor = "\u001b[0K";
    public static string ClearLineToCursor = "\u001b[1K";

    // 256-color support
    public static string FgColor256(int color) => $"\u001b[38;5;{color}m";
    public static string BgColor256(int color) => $"\u001b[48;5;{color}m";

    // True color (24-bit) support
    public static string FgColorRgb(int r, int g, int b) => $"\u001b[38;2;{r};{g};{b}m";
    public static string BgColorRgb(int r, int g, int b) => $"\u001b[48;2;{r};{g};{b}m";

    // Alternative screen buffer
    public const string EnterAlternateScreen = "\u001b[?1049h";
    public const string ExitAlternateScreen = "\u001b[?1049l";

    // Mouse support
    public const string EnableMouseTracking = "\u001b[?1000h\u001b[?1002h\u001b[?1015h\u001b[?1006h";
    public const string DisableMouseTracking = "\u001b[?1006l\u001b[?1015l\u001b[?1002l\u001b[?1000l";

    // Window manipulation
    public const string GetWindowSize = "\u001b[14t";
    public const string GetScreenSize = "\u001b[15t";
    public static string SetWindowTitle(string title) => $"\u001b]0;{title}\u0007";
    public static string SetIconName(string name) => $"\u001b]1;{name}\u0007";

    // Bracketed paste mode
    public const string EnableBracketedPaste = "\u001b[?2004h";
    public const string DisableBracketedPaste = "\u001b[?2004l";

    // Focus reporting
    public const string EnableFocusReporting = "\u001b[?1004h";
    public const string DisableFocusReporting = "\u001b[?1004l";
}