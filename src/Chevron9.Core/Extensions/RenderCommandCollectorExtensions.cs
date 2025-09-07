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
    public static void SubmitText(
        this IRenderCommandCollector collector,
        int layerZ,
        string text,
        Position position,
        Color color,
        float fontSize = 12.0f)
    {
        ArgumentNullException.ThrowIfNull(collector);
        ArgumentNullException.ThrowIfNull(text);

        collector.Submit(layerZ, new DrawTextCommand(text, position, color, fontSize));
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
    public static void SubmitRectangle(
        this IRenderCommandCollector collector,
        int layerZ,
        RectF bounds,
        Color color)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, new DrawRectangleCommand(bounds, color));
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
    ///     Submits a circle rendering command with default material and sort keys
    /// </summary>
    /// <param name="collector">The render command collector</param>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="center">Circle center position</param>
    /// <param name="radius">Circle radius</param>
    /// <param name="color">Fill color</param>
    public static void SubmitCircle(
        this IRenderCommandCollector collector,
        int layerZ,
        Position center,
        float radius,
        Color color)
    {
        ArgumentNullException.ThrowIfNull(collector);

        collector.Submit(layerZ, new DrawCircleCommand(center, radius, color));
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
