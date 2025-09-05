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
}
