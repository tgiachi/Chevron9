using Chevron9.Core.Interfaces;
using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Extensions;

/// <summary>
///     Extension methods for IRenderCommandCollector to simplify command submission
///     Provides fluent API for common rendering operations
/// </summary>
public static class RenderCommandCollectorExtensions
{
    /// <summary>
    ///     Submits a text rendering command with default material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="text">Text content to render</param>
    /// <param name="position">Text position</param>
    /// <param name="color">Text color</param>
    /// <param name="fontSize">Font size in points</param>
    /// <exception cref="ArgumentNullException">Thrown when collector or text is null</exception>
    /// <exception cref="ArgumentException">Thrown when text is empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when fontSize is negative or zero</exception>
    public static void SubmitText(
        this IRenderCommandCollector collector,
        int layerZ,
        string text,
        Position position,
        Color color,
        float fontSize = 12.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);
        ArgumentException.ThrowIfNullOrEmpty(text);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fontSize);

        collector.Submit(layerZ, new DrawTextCommand(text, position, color, fontSize));
    }

    /// <summary>
    ///     Submits a text rendering command with default material and sort keys using coordinates
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="text">Text content to render</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="color">Text color</param>
    /// <param name="fontSize">Font size in points</param>
    public static void SubmitText(
        this IRenderCommandCollector collector,
        int layerZ,
        string text,
        float x,
        float y,
        Color color,
        float fontSize = 12.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);
        ArgumentNullException.ThrowIfNull(text);

        collector.Submit(layerZ, new DrawTextCommand(text, new Position(x, y), color, fontSize));
    }

    /// <summary>
    ///     Submits a text rendering command with custom material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching</param>
    /// <param name="sortKey">Custom sort key for ordering</param>
    /// <param name="text">Text content to render</param>
    /// <param name="position">Text position</param>
    /// <param name="color">Text color</param>
    /// <param name="fontSize">Font size in points</param>
    public static void SubmitText(
        this IRenderCommandCollector collector,
        int layerZ,
        int materialKey,
        int sortKey,
        string text,
        Position position,
        Color color,
        float fontSize = 12.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);
        ArgumentNullException.ThrowIfNull(text);

        collector.Submit(layerZ, materialKey, sortKey, new DrawTextCommand(text, position, color, fontSize));
    }

    /// <summary>
    ///     Submits a rectangle rendering command with default material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="bounds">Rectangle boundaries</param>
    /// <param name="color">Fill color</param>
    /// <exception cref="ArgumentNullException">Thrown when collector is null</exception>
    /// <exception cref="ArgumentException">Thrown when bounds have negative width or height</exception>
    public static void SubmitRectangle(
        this IRenderCommandCollector collector,
        int layerZ,
        RectF bounds,
        Color color)
    {
        ArgumentNullException.ThrowIfNull(collector);
        ArgumentOutOfRangeException.ThrowIfLessThan(bounds.Width, 0, nameof(bounds.Width));
        ArgumentOutOfRangeException.ThrowIfLessThan(bounds.Height, 0, nameof(bounds.Height));

        collector.Submit(layerZ, new DrawRectangleCommand(bounds, color));
    }

    /// <summary>
    ///     Submits a rectangle rendering command with default material and sort keys using coordinates
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="width">Rectangle width</param>
    /// <param name="height">Rectangle height</param>
    /// <param name="color">Fill color</param>
    /// <exception cref="ArgumentNullException">Thrown when collector is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when width or height are negative</exception>
    public static void SubmitRectangle(
        this IRenderCommandCollector collector,
        int layerZ,
        float x,
        float y,
        float width,
        float height,
        Color color)
    {
        ArgumentNullException.ThrowIfNull(collector);
        ArgumentOutOfRangeException.ThrowIfNegative(width);
        ArgumentOutOfRangeException.ThrowIfNegative(height);

        collector.Submit(layerZ, new DrawRectangleCommand(new RectF(x, y, width, height), color));
    }

    /// <summary>
    ///     Submits a rectangle rendering command with custom material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching</param>
    /// <param name="sortKey">Custom sort key for ordering</param>
    /// <param name="bounds">Rectangle boundaries</param>
    /// <param name="color">Fill color</param>
    public static void SubmitRectangle(
        this IRenderCommandCollector collector,
        int layerZ,
        int materialKey,
        int sortKey,
        RectF bounds,
        Color color)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, materialKey, sortKey, new DrawRectangleCommand(bounds, color));
    }

    /// <summary>
    ///     Submits a rectangle outline rendering command with default material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="bounds">Rectangle boundaries</param>
    /// <param name="color">Outline color</param>
    /// <param name="thickness">Line thickness</param>
    public static void SubmitRectangleOutline(
        this IRenderCommandCollector collector,
        int layerZ,
        RectF bounds,
        Color color,
        float thickness = 1.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, new DrawRectangleOutlineCommand(bounds, color, thickness));
    }

    /// <summary>
    ///     Submits a rectangle outline rendering command with default material and sort keys using coordinates
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="width">Rectangle width</param>
    /// <param name="height">Rectangle height</param>
    /// <param name="color">Outline color</param>
    /// <param name="thickness">Line thickness</param>
    public static void SubmitRectangleOutline(
        this IRenderCommandCollector collector,
        int layerZ,
        float x,
        float y,
        float width,
        float height,
        Color color,
        float thickness = 1.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, new DrawRectangleOutlineCommand(new RectF(x, y, width, height), color, thickness));
    }

    /// <summary>
    ///     Submits a rectangle outline rendering command with custom material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching</param>
    /// <param name="sortKey">Custom sort key for ordering</param>
    /// <param name="bounds">Rectangle boundaries</param>
    /// <param name="color">Outline color</param>
    /// <param name="thickness">Line thickness</param>
    public static void SubmitRectangleOutline(
        this IRenderCommandCollector collector,
        int layerZ,
        int materialKey,
        int sortKey,
        RectF bounds,
        Color color,
        float thickness = 1.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, materialKey, sortKey, new DrawRectangleOutlineCommand(bounds, color, thickness));
    }

    /// <summary>
    ///     Submits a rectangle outline rendering command with custom material and sort keys using coordinates
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching</param>
    /// <param name="sortKey">Custom sort key for ordering</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="width">Rectangle width</param>
    /// <param name="height">Rectangle height</param>
    /// <param name="color">Outline color</param>
    /// <param name="thickness">Line thickness</param>
    public static void SubmitRectangleOutline(
        this IRenderCommandCollector collector,
        int layerZ,
        int materialKey,
        int sortKey,
        float x,
        float y,
        float width,
        float height,
        Color color,
        float thickness = 1.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, materialKey, sortKey, new DrawRectangleOutlineCommand(new RectF(x, y, width, height), color, thickness));
    }

    /// <summary>
    ///     Submits a circle rendering command with default material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="center">Circle center position</param>
    /// <param name="radius">Circle radius</param>
    /// <param name="color">Fill color</param>
    /// <exception cref="ArgumentNullException">Thrown when collector is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when radius is negative</exception>
    public static void SubmitCircle(
        this IRenderCommandCollector collector,
        int layerZ,
        Position center,
        float radius,
        Color color)
    {
        ArgumentNullException.ThrowIfNull(collector);
        ArgumentOutOfRangeException.ThrowIfNegative(radius);

        collector.Submit(layerZ, new DrawCircleCommand(center, radius, color));
    }

    /// <summary>
    ///     Submits a circle rendering command with default material and sort keys using coordinates
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="x">Center X coordinate</param>
    /// <param name="y">Center Y coordinate</param>
    /// <param name="radius">Circle radius</param>
    /// <param name="color">Fill color</param>
    public static void SubmitCircle(
        this IRenderCommandCollector collector,
        int layerZ,
        float x,
        float y,
        float radius,
        Color color)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, new DrawCircleCommand(new Position(x, y), radius, color));
    }

    /// <summary>
    ///     Submits a circle rendering command with custom material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching</param>
    /// <param name="sortKey">Custom sort key for ordering</param>
    /// <param name="center">Circle center position</param>
    /// <param name="radius">Circle radius</param>
    /// <param name="color">Fill color</param>
    public static void SubmitCircle(
        this IRenderCommandCollector collector,
        int layerZ,
        int materialKey,
        int sortKey,
        Position center,
        float radius,
        Color color)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, materialKey, sortKey, new DrawCircleCommand(center, radius, color));
    }

    /// <summary>
    ///     Submits a circle outline rendering command with default material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="center">Circle center position</param>
    /// <param name="radius">Circle radius</param>
    /// <param name="color">Outline color</param>
    /// <param name="thickness">Line thickness</param>
    public static void SubmitCircleOutline(
        this IRenderCommandCollector collector,
        int layerZ,
        Position center,
        float radius,
        Color color,
        float thickness = 1.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, new DrawCircleOutlineCommand(center, radius, color, thickness));
    }

    /// <summary>
    ///     Submits a circle outline rendering command with default material and sort keys using coordinates
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="x">Center X coordinate</param>
    /// <param name="y">Center Y coordinate</param>
    /// <param name="radius">Circle radius</param>
    /// <param name="color">Outline color</param>
    /// <param name="thickness">Line thickness</param>
    public static void SubmitCircleOutline(
        this IRenderCommandCollector collector,
        int layerZ,
        float x,
        float y,
        float radius,
        Color color,
        float thickness = 1.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, new DrawCircleOutlineCommand(new Position(x, y), radius, color, thickness));
    }

    /// <summary>
    ///     Submits a circle outline rendering command with custom material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching</param>
    /// <param name="sortKey">Custom sort key for ordering</param>
    /// <param name="center">Circle center position</param>
    /// <param name="radius">Circle radius</param>
    /// <param name="color">Outline color</param>
    /// <param name="thickness">Line thickness</param>
    public static void SubmitCircleOutline(
        this IRenderCommandCollector collector,
        int layerZ,
        int materialKey,
        int sortKey,
        Position center,
        float radius,
        Color color,
        float thickness = 1.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, materialKey, sortKey, new DrawCircleOutlineCommand(center, radius, color, thickness));
    }

    /// <summary>
    ///     Submits a circle outline rendering command with custom material and sort keys using coordinates
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching</param>
    /// <param name="sortKey">Custom sort key for ordering</param>
    /// <param name="x">Center X coordinate</param>
    /// <param name="y">Center Y coordinate</param>
    /// <param name="radius">Circle radius</param>
    /// <param name="color">Outline color</param>
    /// <param name="thickness">Line thickness</param>
    public static void SubmitCircleOutline(
        this IRenderCommandCollector collector,
        int layerZ,
        int materialKey,
        int sortKey,
        float x,
        float y,
        float radius,
        Color color,
        float thickness = 1.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, materialKey, sortKey, new DrawCircleOutlineCommand(new Position(x, y), radius, color, thickness));
    }

    /// <summary>
    ///     Submits a line rendering command with default material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="start">Line start position</param>
    /// <param name="end">Line end position</param>
    /// <param name="color">Line color</param>
    /// <param name="thickness">Line thickness</param>
    public static void SubmitLine(
        this IRenderCommandCollector collector,
        int layerZ,
        Position start,
        Position end,
        Color color,
        float thickness = 1.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, new DrawLineCommand(start, end, color, thickness));
    }

    /// <summary>
    ///     Submits a line rendering command with default material and sort keys using coordinates
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="startX">Start X coordinate</param>
    /// <param name="startY">Start Y coordinate</param>
    /// <param name="endX">End X coordinate</param>
    /// <param name="endY">End Y coordinate</param>
    /// <param name="color">Line color</param>
    /// <param name="thickness">Line thickness</param>
    public static void SubmitLine(
        this IRenderCommandCollector collector,
        int layerZ,
        float startX,
        float startY,
        float endX,
        float endY,
        Color color,
        float thickness = 1.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, new DrawLineCommand(
            new Position(startX, startY),
            new Position(endX, endY),
            color,
            thickness));
    }

    /// <summary>
    ///     Submits a line rendering command with custom material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching</param>
    /// <param name="sortKey">Custom sort key for ordering</param>
    /// <param name="start">Line start position</param>
    /// <param name="end">Line end position</param>
    /// <param name="color">Line color</param>
    /// <param name="thickness">Line thickness</param>
    public static void SubmitLine(
        this IRenderCommandCollector collector,
        int layerZ,
        int materialKey,
        int sortKey,
        Position start,
        Position end,
        Color color,
        float thickness = 1.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, materialKey, sortKey, new DrawLineCommand(start, end, color, thickness));
    }

    /// <summary>
    ///     Submits a glyph rendering command with default material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="position">Glyph position</param>
    /// <param name="codepoint">Unicode codepoint of the glyph</param>
    /// <param name="foreground">Foreground color</param>
    /// <param name="background">Background color</param>
    public static void SubmitGlyph(
        this IRenderCommandCollector collector,
        int layerZ,
        Position position,
        int codepoint,
        Color foreground,
        Color background)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, new DrawGlyphCommand(position, codepoint, foreground, background));
    }

    /// <summary>
    ///     Submits a glyph rendering command with default material and sort keys using coordinates
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="codepoint">Unicode codepoint of the glyph</param>
    /// <param name="foreground">Foreground color</param>
    /// <param name="background">Background color</param>
    public static void SubmitGlyph(
        this IRenderCommandCollector collector,
        int layerZ,
        float x,
        float y,
        int codepoint,
        Color foreground,
        Color background)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, new DrawGlyphCommand(new Position(x, y), codepoint, foreground, background));
    }

    /// <summary>
    ///     Submits a glyph rendering command with custom material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching</param>
    /// <param name="sortKey">Custom sort key for ordering</param>
    /// <param name="position">Glyph position</param>
    /// <param name="codepoint">Unicode codepoint of the glyph</param>
    /// <param name="foreground">Foreground color</param>
    /// <param name="background">Background color</param>
    public static void SubmitGlyph(
        this IRenderCommandCollector collector,
        int layerZ,
        int materialKey,
        int sortKey,
        Position position,
        int codepoint,
        Color foreground,
        Color background)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, materialKey, sortKey, new DrawGlyphCommand(position, codepoint, foreground, background));
    }

    /// <summary>
    ///     Submits a glyph rendering command from a character with default material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="position">Glyph position</param>
    /// <param name="character">Character to render</param>
    /// <param name="foreground">Foreground color</param>
    /// <param name="background">Background color</param>
    public static void SubmitGlyph(
        this IRenderCommandCollector collector,
        int layerZ,
        Position position,
        char character,
        Color foreground,
        Color background)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, DrawGlyphCommand.FromChar(position, character, foreground, background));
    }

    /// <summary>
    ///     Submits a glyph rendering command from a character with custom material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching</param>
    /// <param name="sortKey">Custom sort key for ordering</param>
    /// <param name="position">Glyph position</param>
    /// <param name="character">Character to render</param>
    /// <param name="foreground">Foreground color</param>
    /// <param name="background">Background color</param>
    public static void SubmitGlyph(
        this IRenderCommandCollector collector,
        int layerZ,
        int materialKey,
        int sortKey,
        Position position,
        char character,
        Color foreground,
        Color background)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, materialKey, sortKey, DrawGlyphCommand.FromChar(position, character, foreground, background));
    }
}
