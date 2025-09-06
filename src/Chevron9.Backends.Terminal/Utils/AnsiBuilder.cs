using System.Text;
using Chevron9.Shared.Graphics;

namespace Chevron9.Backends.Terminal.Utils;

/// <summary>
///     High-performance ANSI escape sequence builder using StringBuilder
///     Minimizes allocations and provides fluent API for building complex sequences
/// </summary>
public sealed class AnsiBuilder : IDisposable
{
    private readonly StringBuilder _builder;
    private bool _hasPendingSequence;

    /// <summary>
    ///     Initializes a new AnsiBuilder with default capacity
    /// </summary>
    public AnsiBuilder()
    {
        _builder = new StringBuilder(1024);
        _hasPendingSequence = false;
    }

    /// <summary>
    ///     Initializes a new AnsiBuilder with specified capacity
    /// </summary>
    /// <param name="capacity">Initial capacity for the StringBuilder</param>
    public AnsiBuilder(int capacity)
    {
        _builder = new StringBuilder(capacity);
        _hasPendingSequence = false;
    }

    /// <summary>
    ///     Clears the builder and resets state
    /// </summary>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder Clear()
    {
        _builder.Clear();
        _hasPendingSequence = false;
        return this;
    }

    /// <summary>
    ///     Appends a raw string to the builder
    /// </summary>
    /// <param name="text">Text to append</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder Append(string text)
    {
        _builder.Append(text);
        return this;
    }

    /// <summary>
    ///     Appends a character to the builder
    /// </summary>
    /// <param name="c">Character to append</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder Append(char c)
    {
        _builder.Append(c);
        return this;
    }

    /// <summary>
    ///     Starts a new ANSI escape sequence
    /// </summary>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder Escape()
    {
        _builder.Append(AnsiEscapeCodes.Escape);
        _hasPendingSequence = true;
        return this;
    }

    /// <summary>
    ///     Adds a parameter to the current escape sequence
    /// </summary>
    /// <param name="param">Parameter value</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder Param(int param)
    {
        if (_hasPendingSequence)
        {
            _builder.Append(param);
        }
        return this;
    }

    /// <summary>
    ///     Adds multiple parameters to the current escape sequence
    /// </summary>
    /// <param name="params">Parameter values</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder Params(params int[] @params)
    {
        if (_hasPendingSequence && @params.Length > 0)
        {
            _builder.Append(@params[0]);
            for (int i = 1; i < @params.Length; i++)
            {
                _builder.Append(';').Append(@params[i]);
            }
        }
        return this;
    }

    /// <summary>
    ///     Ends the current escape sequence
    /// </summary>
    /// <param name="terminator">Sequence terminator character</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder End(char terminator = 'm')
    {
        if (_hasPendingSequence)
        {
            _builder.Append(terminator);
            _hasPendingSequence = false;
        }
        return this;
    }

    /// <summary>
    ///     Sets cursor position
    /// </summary>
    /// <param name="row">Row (1-based)</param>
    /// <param name="col">Column (1-based)</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder CursorPosition(int row, int col)
    {
        _builder.Append(AnsiEscapeCodes.Escape).Append(row).Append(';').Append(col).Append('H');
        return this;
    }

    /// <summary>
    ///     Moves cursor up by specified number of lines
    /// </summary>
    /// <param name="lines">Number of lines to move up</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder CursorUp(int lines = 1)
    {
        _builder.Append(AnsiEscapeCodes.Escape).Append(lines).Append('A');
        return this;
    }

    /// <summary>
    ///     Moves cursor down by specified number of lines
    /// </summary>
    /// <param name="lines">Number of lines to move down</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder CursorDown(int lines = 1)
    {
        _builder.Append(AnsiEscapeCodes.Escape).Append(lines).Append('B');
        return this;
    }

    /// <summary>
    ///     Moves cursor forward by specified number of columns
    /// </summary>
    /// <param name="cols">Number of columns to move forward</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder CursorForward(int cols = 1)
    {
        _builder.Append(AnsiEscapeCodes.Escape).Append(cols).Append('C');
        return this;
    }

    /// <summary>
    ///     Moves cursor back by specified number of columns
    /// </summary>
    /// <param name="cols">Number of columns to move back</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder CursorBack(int cols = 1)
    {
        _builder.Append(AnsiEscapeCodes.Escape).Append(cols).Append('D');
        return this;
    }

    /// <summary>
    ///     Sets text color using 256-color palette
    /// </summary>
    /// <param name="color">Color index (0-255)</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder FgColor256(int color)
    {
        _builder.Append(AnsiEscapeCodes.Escape).Append("38;5;").Append(color).Append('m');
        return this;
    }

    /// <summary>
    ///     Sets background color using 256-color palette
    /// </summary>
    /// <param name="color">Color index (0-255)</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder BgColor256(int color)
    {
        _builder.Append(AnsiEscapeCodes.Escape).Append("48;5;").Append(color).Append('m');
        return this;
    }

    /// <summary>
    ///     Sets text color using true color (24-bit RGB)
    /// </summary>
    /// <param name="r">Red component (0-255)</param>
    /// <param name="g">Green component (0-255)</param>
    /// <param name="b">Blue component (0-255)</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder FgColorRgb(int r, int g, int b)
    {
        _builder.Append(AnsiEscapeCodes.Escape).Append("38;2;").Append(r).Append(';').Append(g).Append(';').Append(b).Append('m');
        return this;
    }

    /// <summary>
    ///     Sets background color using true color (24-bit RGB)
    /// </summary>
    /// <param name="r">Red component (0-255)</param>
    /// <param name="g">Green component (0-255)</param>
    /// <param name="b">Blue component (0-255)</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder BgColorRgb(int r, int g, int b)
    {
        _builder.Append(AnsiEscapeCodes.Escape).Append("48;2;").Append(r).Append(';').Append(g).Append(';').Append(b).Append('m');
        return this;
    }

    /// <summary>
    ///     Sets text color using Color struct
    /// </summary>
    /// <param name="color">Color to set</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder FgColor(Color color)
    {
        return FgColorRgb(color.R, color.G, color.B);
    }

    /// <summary>
    ///     Sets background color using Color struct
    /// </summary>
    /// <param name="color">Color to set</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder BgColor(Color color)
    {
        return BgColorRgb(color.R, color.G, color.B);
    }

    /// <summary>
    ///     Sets both foreground and background colors using Color structs
    /// </summary>
    /// <param name="fgColor">Foreground color</param>
    /// <param name="bgColor">Background color</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder Colors(Color fgColor, Color bgColor)
    {
        return FgColor(fgColor).BgColor(bgColor);
    }

    /// <summary>
    ///     Applies text style
    /// </summary>
    /// <param name="style">ANSI style code</param>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder Style(string style)
    {
        _builder.Append(style);
        return this;
    }

    /// <summary>
    ///     Resets all text formatting
    /// </summary>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder Reset()
    {
        _builder.Append(AnsiEscapeCodes.Reset);
        return this;
    }

    /// <summary>
    ///     Clears the entire screen
    /// </summary>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder ClearScreen()
    {
        _builder.Append(AnsiEscapeCodes.ClearScreen);
        return this;
    }

    /// <summary>
    ///     Clears the current line
    /// </summary>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder ClearLine()
    {
        _builder.Append(AnsiEscapeCodes.ClearLine);
        return this;
    }

    /// <summary>
    ///     Moves cursor to home position (1,1)
    /// </summary>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder CursorHome()
    {
        _builder.Append(AnsiEscapeCodes.CursorHome);
        return this;
    }

    /// <summary>
    ///     Hides the cursor
    /// </summary>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder HideCursor()
    {
        _builder.Append(AnsiEscapeCodes.CursorHide);
        return this;
    }

    /// <summary>
    ///     Shows the cursor
    /// </summary>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder ShowCursor()
    {
        _builder.Append(AnsiEscapeCodes.CursorShow);
        return this;
    }

    /// <summary>
    ///     Saves current cursor position
    /// </summary>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder SaveCursor()
    {
        _builder.Append(AnsiEscapeCodes.SaveCursor);
        return this;
    }

    /// <summary>
    ///     Restores previously saved cursor position
    /// </summary>
    /// <returns>This AnsiBuilder for method chaining</returns>
    public AnsiBuilder RestoreCursor()
    {
        _builder.Append(AnsiEscapeCodes.RestoreCursor);
        return this;
    }

    /// <summary>
    ///     Builds and returns the complete ANSI sequence string
    /// </summary>
    /// <returns>Complete ANSI sequence string</returns>
    public string Build()
    {
        return _builder.ToString();
    }

    /// <summary>
    ///     Returns the length of the current sequence
    /// </summary>
    public int Length => _builder.Length;

    /// <summary>
    ///     Returns the built string implicitly
    /// </summary>
    /// <param name="builder">AnsiBuilder instance</param>
    public static implicit operator string(AnsiBuilder builder) => builder.Build();

    /// <summary>
    ///     Disposes the AnsiBuilder (no-op since StringBuilder doesn't need disposal)
    /// </summary>
    public void Dispose()
    {
        // StringBuilder doesn't need disposal, but we implement IDisposable for using statements
    }
}
