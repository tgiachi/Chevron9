using System.Text;
using Chevron9.Backends.Terminal.Utils;
using Chevron9.Core.Args;
using Chevron9.Core.Interfaces;
using Chevron9.Core.Render;
using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Backends.Terminal.Renderer;

/// <summary>
///     Terminal-based renderer implementation using ANSI escape sequences with double buffering
///     Uses Cell<int> system for efficient character-based rendering with diff-based updates
/// </summary>
public class ConsoleRender : IRenderer
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public event EventHandler<ResizeEventArgs>? Resize;

    private readonly Stream _outputStream;
    private readonly AnsiBuilder _builder;
    private readonly List<RenderCommand> _batchedCommands;

    // Double buffering with Cell<int> (int = Unicode codepoint) - 1D arrays for performance
    private Cell<int>[] _frontBuffer;
    private Cell<int>[] _backBuffer;
    private readonly Cell<int> _emptyCellTemplate;

    // Viewport clipping
    private int _viewportX, _viewportY, _viewportWidth, _viewportHeight;
    private bool _hasViewport;

    // Frame state
    private bool _inFrame;
    private bool _inBatch;

    public ConsoleRender()
    {
        Width = Console.WindowWidth;
        Height = Console.WindowHeight;

        _outputStream = Console.OpenStandardOutput();
        _builder = new AnsiBuilder(8192);
        _batchedCommands = new List<RenderCommand>(256);

        // Initialize double buffers
        _emptyCellTemplate = new Cell<int>(' ', Color.White, Color.Black);
        InitializeBuffers();

        // Initialize viewport to full console
        _viewportWidth = Width;
        _viewportHeight = Height;

        // Initialize terminal
        WriteToTerminal(TerminalInit.SetupHighPerformance());

        // Hide cursor for cleaner rendering
        WriteToTerminal(AnsiEscapeCodes.CursorHide);
    }

    private void InitializeBuffers()
    {
        int totalCells = Height * Width;
        _frontBuffer = new Cell<int>[totalCells];
        _backBuffer = new Cell<int>[totalCells];

        // Initialize with empty cells
        Array.Fill(_frontBuffer, _emptyCellTemplate);
        Array.Fill(_backBuffer, _emptyCellTemplate);
    }

    public void SetViewport(int x, int y, int width, int height)
    {
        _viewportX = Math.Max(0, Math.Min(x, Width));
        _viewportY = Math.Max(0, Math.Min(y, Height));
        _viewportWidth = Math.Max(0, Math.Min(width, Width - _viewportX));
        _viewportHeight = Math.Max(0, Math.Min(height, Height - _viewportY));
        _hasViewport = true;
    }

    public void Clear(Color color)
    {
        var clearCell = new Cell<int>(' ', Color.White, color);

        // Clear back buffer with specified background color
        Array.Fill(_backBuffer, clearCell);
    }

    public void BeginFrame()
    {
        if (_inFrame)
        {
            throw new InvalidOperationException("Already in frame - call EndFrame() first");
        }

        _inFrame = true;

        // Check for console resize
        int newWidth = Console.WindowWidth;
        int newHeight = Console.WindowHeight;
        if (newWidth != Width || newHeight != Height)
        {
            ResizeBuffers(newWidth, newHeight);
        }

        // Clear back buffer to default state
        Clear(Color.Black);
    }

    public void BeginBatch()
    {
        if (!_inFrame)
        {
            throw new InvalidOperationException("Must call BeginFrame() before BeginBatch()");
        }

        _inBatch = true;
        _batchedCommands.Clear();
    }

    public void Submit(RenderCommand cmd)
    {
        if (!_inFrame)
        {
            throw new InvalidOperationException("Must call BeginFrame() before submitting commands");
        }

        if (_inBatch)
        {
            _batchedCommands.Add(cmd);
        }
        else
        {
            ProcessCommand(cmd);
        }
    }

    public void EndBatch()
    {
        if (!_inBatch)
        {
            return;
        }

        _inBatch = false;

        // Process all batched commands
        foreach (var cmd in _batchedCommands)
        {
            ProcessCommand(cmd);
        }

        _batchedCommands.Clear();
    }

    public void EndFrame()
    {
        if (!_inFrame)
        {
            return;
        }

        // End any active batch
        if (_inBatch)
        {
            EndBatch();
        }

        // Perform diff and update only changed cells
        RenderDiff();

        // Swap buffers
        SwapBuffers();

        _inFrame = false;
    }

    private void ProcessCommand(RenderCommand cmd)
    {
        switch (cmd)
        {
            case DrawTextCommand textCmd:
                RenderText(textCmd);
                break;

            case DrawRectangleCommand rectCmd:
                RenderRectangle(rectCmd);
                break;

            case DrawRectangleOutlineCommand outlineCmd:
                RenderRectangleOutline(outlineCmd);
                break;

            case DrawLineCommand lineCmd:
                RenderLine(lineCmd);
                break;

            case DrawGlyphCommand glyphCmd:
                RenderGlyph(glyphCmd);
                break;

            default:
                // Ignore unknown commands gracefully
                break;
        }
    }

    private void RenderText(DrawTextCommand cmd)
    {
        int startX = (int)cmd.Position.X;
        int startY = (int)cmd.Position.Y;

        for (int i = 0; i < cmd.Text.Length; i++)
        {
            int x = startX + i;
            int y = startY;

            if (!IsInBounds(x, y) || !IsInViewport(x, y))
            {
                continue;
            }

            int index = y * Width + x;
            var cell = new Cell<int>(cmd.Text[i], cmd.Color, _backBuffer[index].Bg);
            _backBuffer[index] = cell;
        }
    }

    private void RenderRectangle(DrawRectangleCommand cmd)
    {
        int startX = (int)cmd.Bounds.X;
        int startY = (int)cmd.Bounds.Y;
        int endX = startX + (int)cmd.Bounds.Width;
        int endY = startY + (int)cmd.Bounds.Height;

        var fillCell = new Cell<int>('█', cmd.Color, cmd.Color); // Full block with matching bg

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                if (!IsInBounds(x, y) || !IsInViewport(x, y))
                {
                    continue;
                }

                int index = y * Width + x;
                _backBuffer[index] = fillCell;
            }
        }
    }

    private void RenderRectangleOutline(DrawRectangleOutlineCommand cmd)
    {
        int startX = (int)cmd.Bounds.X;
        int startY = (int)cmd.Bounds.Y;
        int width = (int)cmd.Bounds.Width;
        int height = (int)cmd.Bounds.Height;
        int endX = startX + width - 1;
        int endY = startY + height - 1;

        // Box drawing characters
        const int topLeft = '┌', topRight = '┐', bottomLeft = '└', bottomRight = '┘';
        const int horizontal = '─', vertical = '│';

        // Top and bottom edges
        for (int x = startX; x <= endX; x++)
        {
            if (IsInBounds(x, startY) && IsInViewport(x, startY))
            {
                int ch = (x == startX) ? topLeft :
                    (x == endX) ? topRight : horizontal;
                int topIndex = startY * Width + x;
                _backBuffer[topIndex] = new Cell<int>(ch, cmd.Color, _backBuffer[topIndex].Bg);
            }

            if (height > 1 && IsInBounds(x, endY) && IsInViewport(x, endY))
            {
                int ch = (x == startX) ? bottomLeft :
                    (x == endX) ? bottomRight : horizontal;
                int bottomIndex = endY * Width + x;
                _backBuffer[bottomIndex] = new Cell<int>(ch, cmd.Color, _backBuffer[bottomIndex].Bg);
            }
        }

        // Left and right edges
        for (int y = startY + 1; y < endY; y++)
        {
            if (IsInBounds(startX, y) && IsInViewport(startX, y))
            {
                int leftIndex = y * Width + startX;
                _backBuffer[leftIndex] = new Cell<int>(vertical, cmd.Color, _backBuffer[leftIndex].Bg);
            }

            if (width > 1 && IsInBounds(endX, y) && IsInViewport(endX, y))
            {
                int rightIndex = y * Width + endX;
                _backBuffer[rightIndex] = new Cell<int>(vertical, cmd.Color, _backBuffer[rightIndex].Bg);
            }
        }
    }

    private void RenderLine(DrawLineCommand lineCmd)
    {
        var points = BresenhamLine(
            (int)lineCmd.Start.X,
            (int)lineCmd.Start.Y,
            (int)lineCmd.End.X,
            (int)lineCmd.End.Y
        );

        var lineCell = new Cell<int>('█', lineCmd.Color, Color.Black);

        foreach (var (x, y) in points)
        {
            if (!IsInBounds(x, y) || !IsInViewport(x, y))
            {
                continue;
            }

            int index = y * Width + x;
            _backBuffer[index] = lineCell;
        }
    }

    private void RenderGlyph(DrawGlyphCommand cmd)
    {
        int x = (int)cmd.Position.X;
        int y = (int)cmd.Position.Y;

        if (!IsInBounds(x, y) || !IsInViewport(x, y))
        {
            return;
        }

        int index = y * Width + x;
        var cell = new Cell<int>(cmd.Codepoint, cmd.Foreground, _backBuffer[index].Bg);
        _backBuffer[index] = cell;
    }

    private void RenderDiff()
    {
        _builder.Clear();

        int index = 0;
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var frontCell = _frontBuffer[index];
                var backCell = _backBuffer[index];

                // Only update if cell changed
                if (!frontCell.Equals(backCell))
                {
                    _builder.CursorPosition(y + 1, x + 1) // ANSI is 1-based
                        .FgColor(backCell.Fg)
                        .BgColor(backCell.Bg)
                        .Append((char)backCell.Visual)
                        .Reset();
                }

                index++;
            }
        }

        // Flush the diff
        if (_builder.Length > 0)
        {
            WriteToTerminal(_builder.Build());
            _builder.Clear();
        }
    }

    private void SwapBuffers()
    {
        // Copy back buffer to front buffer
        _backBuffer.CopyTo(_frontBuffer, 0);
    }

    private void ResizeBuffers(int newWidth, int newHeight)
    {
        Width = newWidth;
        Height = newHeight;

        var oldFront = _frontBuffer;
        var oldBack = _backBuffer;
        int oldWidth = Width;

        InitializeBuffers();

        // Copy old content (clipped to new size)
        int copyWidth = Math.Min(oldWidth, Width);
        int copyHeight = Math.Min(oldFront.Length / oldWidth, Height);

        for (int y = 0; y < copyHeight; y++)
        {
            for (int x = 0; x < copyWidth; x++)
            {
                int oldIndex = y * oldWidth + x;
                int newIndex = y * Width + x;
                _frontBuffer[newIndex] = oldFront[oldIndex];
                _backBuffer[newIndex] = oldBack[oldIndex];
            }
        }

        Resize?.Invoke(this, new ResizeEventArgs(Width, Height));
    }

    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    private bool IsInViewport(int x, int y)
    {
        if (!_hasViewport) return true;
        return x >= _viewportX && x < _viewportX + _viewportWidth &&
               y >= _viewportY && y < _viewportY + _viewportHeight;
    }

    private static List<(int x, int y)> BresenhamLine(int x0, int y0, int x1, int y1)
    {
        var points = new List<(int x, int y)>();
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        int x = x0, y = y0;

        while (true)
        {
            points.Add((x, y));

            if (x == x1 && y == y1)
            {
                break;
            }

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }

        return points;
    }

    private void WriteToTerminal(string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        _outputStream.Write(bytes, 0, bytes.Length);
        _outputStream.Flush();
    }

    public void Dispose()
    {
        // Show cursor before cleanup
        WriteToTerminal(AnsiEscapeCodes.CursorShow);
        WriteToTerminal(TerminalInit.Cleanup());

        _builder.Dispose();
        _outputStream.Dispose();
        GC.SuppressFinalize(this);
    }
}
