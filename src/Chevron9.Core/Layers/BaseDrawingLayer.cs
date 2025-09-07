using Chevron9.Core.Extensions.Cameras;
using Chevron9.Core.Interfaces;
using Chevron9.Core.Types;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Layers;

/// <summary>
///     Base layer class that provides convenient drawing methods using pooled render commands
///     for optimal memory usage. Extends AbstractLayer with helper methods for common drawing operations.
/// </summary>
public abstract class BaseDrawingLayer : AbstractLayer
{
    /// <summary>
    ///     Initializes a new BaseDrawingLayer with default settings
    /// </summary>
    /// <param name="name">Layer name for identification</param>
    /// <param name="zIndex">Z-index for depth sorting</param>
    protected BaseDrawingLayer(string name, int zIndex) : base(name, zIndex)
    {
    }

    /// <summary>
    ///     Initializes a new BaseDrawingLayer with custom settings
    /// </summary>
    /// <param name="name">Layer name for identification</param>
    /// <param name="zIndex">Z-index for depth sorting</param>
    /// <param name="enabled">Whether layer processes updates and input</param>
    /// <param name="visible">Whether layer is rendered</param>
    /// <param name="clear">Clear flags for layer</param>
    /// <param name="compose">Compositing mode for layer</param>
    protected BaseDrawingLayer(string name, int zIndex, bool enabled, bool visible,
        LayerClear clear, LayerCompositeMode compose)
        : base(name, zIndex, enabled, visible, clear, compose)
    {
    }

    /// <summary>
    ///     Draws a filled rectangle using pooled commands for optimal memory usage
    /// </summary>
    /// <param name="rq">Render command collector</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="width">Rectangle width</param>
    /// <param name="height">Rectangle height</param>
    /// <param name="color">Fill color</param>
    protected void DrawRectangle(IRenderCommandCollector rq, float x, float y, float width, float height, Color color)
    {
        var bounds = new RectF(x, y, width, height);
        DrawRectangle(rq, bounds, color);
    }

    /// <summary>
    ///     Draws a filled rectangle using pooled commands for optimal memory usage
    /// </summary>
    /// <param name="rq">Render command collector</param>
    /// <param name="bounds">Rectangle boundaries</param>
    /// <param name="color">Fill color</param>
    protected void DrawRectangle(IRenderCommandCollector rq, RectF bounds, Color color)
    {
        // Apply camera transformation to bounds
        var transformedBounds = Camera.TransformBounds(bounds);
        rq.SubmitRectangle(ZIndex, transformedBounds, color);
    }

    /// <summary>
    ///     Draws a filled rectangle at screen coordinates (no camera transformation)
    /// </summary>
    /// <param name="rq">Render command collector</param>
    /// <param name="bounds">Rectangle boundaries in screen space</param>
    /// <param name="color">Fill color</param>
    protected void DrawRectangleScreen(IRenderCommandCollector rq, RectF bounds, Color color)
    {
        rq.SubmitRectangle(ZIndex, bounds, color);
    }

    /// <summary>
    ///     Draws text using pooled commands for optimal memory usage
    /// </summary>
    /// <param name="rq">Render command collector</param>
    /// <param name="text">Text to draw</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="color">Text color</param>
    /// <param name="fontSize">Font size</param>
    protected void DrawText(IRenderCommandCollector rq, string text, float x, float y,
        Color color, float fontSize = 12.0f)
    {
        var position = new Position(x, y);
        DrawText(rq, text, position, color, fontSize);
    }

    /// <summary>
    ///     Draws text using pooled commands for optimal memory usage
    /// </summary>
    /// <param name="rq">Render command collector</param>
    /// <param name="text">Text to draw</param>
    /// <param name="position">Text position</param>
    /// <param name="color">Text color</param>
    /// <param name="fontSize">Font size</param>
    protected void DrawText(IRenderCommandCollector rq, string text, Position position,
        Color color, float fontSize = 12.0f)
    {
        // Apply camera transformation to position
        var transformedPosition = Camera.TransformPosition(position);
        rq.SubmitText(ZIndex, text, transformedPosition, color, fontSize);
    }

    /// <summary>
    ///     Draws text at screen coordinates (no camera transformation)
    /// </summary>
    /// <param name="rq">Render command collector</param>
    /// <param name="text">Text to draw</param>
    /// <param name="position">Text position in screen space</param>
    /// <param name="color">Text color</param>
    /// <param name="fontSize">Font size</param>
    protected void DrawTextScreen(IRenderCommandCollector rq, string text, Position position,
        Color color, float fontSize = 12.0f)
    {
        rq.SubmitText(ZIndex, text, position, color, fontSize);
    }

    /// <summary>
    ///     Draws a simple UI panel with background
    /// </summary>
    /// <param name="rq">Render command collector</param>
    /// <param name="bounds">Panel boundaries</param>
    /// <param name="backgroundColor">Background color</param>
    protected void DrawPanel(IRenderCommandCollector rq, RectF bounds, Color backgroundColor)
    {
        DrawRectangleScreen(rq, bounds, backgroundColor);
    }

    /// <summary>
    ///     Draws a UI panel with background and border
    /// </summary>
    /// <param name="rq">Render command collector</param>
    /// <param name="bounds">Panel boundaries</param>
    /// <param name="backgroundColor">Background color</param>
    /// <param name="borderColor">Border color</param>
    /// <param name="borderWidth">Border thickness</param>
    protected void DrawPanelWithBorder(IRenderCommandCollector rq, RectF bounds,
        Color backgroundColor, Color borderColor, float borderWidth = 1.0f)
    {
        // Draw background
        DrawRectangleScreen(rq, bounds, backgroundColor);

        // Draw border (4 rectangles for top, right, bottom, left)
        // Top border
        DrawRectangleScreen(rq, new RectF(bounds.X, bounds.Y, bounds.Width, borderWidth), borderColor);
        // Right border  
        DrawRectangleScreen(rq, new RectF(bounds.X + bounds.Width - borderWidth, bounds.Y, borderWidth, bounds.Height), borderColor);
        // Bottom border
        DrawRectangleScreen(rq, new RectF(bounds.X, bounds.Y + bounds.Height - borderWidth, bounds.Width, borderWidth), borderColor);
        // Left border
        DrawRectangleScreen(rq, new RectF(bounds.X, bounds.Y, borderWidth, bounds.Height), borderColor);
    }

    /// <summary>
    ///     Draws a progress bar
    /// </summary>
    /// <param name="rq">Render command collector</param>
    /// <param name="bounds">Progress bar boundaries</param>
    /// <param name="progress">Progress value (0.0 to 1.0)</param>
    /// <param name="backgroundColor">Background color</param>
    /// <param name="progressColor">Progress fill color</param>
    /// <param name="borderColor">Border color</param>
    protected void DrawProgressBar(IRenderCommandCollector rq, RectF bounds, float progress,
        Color backgroundColor, Color progressColor, Color? borderColor = null)
    {
        progress = Math.Clamp(progress, 0.0f, 1.0f);

        // Draw background
        DrawRectangleScreen(rq, bounds, backgroundColor);

        // Draw progress fill
        if (progress > 0.0f)
        {
            var progressWidth = bounds.Width * progress;
            var progressBounds = new RectF(bounds.X, bounds.Y, progressWidth, bounds.Height);
            DrawRectangleScreen(rq, progressBounds, progressColor);
        }

        // Draw border if specified
        if (borderColor.HasValue)
        {
            DrawPanelWithBorder(rq, bounds, Color.Transparent, borderColor.Value);
        }
    }

    /// <summary>
    ///     Draws a simple button with text
    /// </summary>
    /// <param name="rq">Render command collector</param>
    /// <param name="bounds">Button boundaries</param>
    /// <param name="text">Button text</param>
    /// <param name="backgroundColor">Button background color</param>
    /// <param name="textColor">Button text color</param>
    /// <param name="borderColor">Button border color</param>
    /// <param name="fontSize">Text font size</param>
    protected void DrawButton(IRenderCommandCollector rq, RectF bounds, string text,
        Color backgroundColor, Color textColor, Color? borderColor = null, float fontSize = 12.0f)
    {
        // Draw button background and border
        if (borderColor.HasValue)
        {
            DrawPanelWithBorder(rq, bounds, backgroundColor, borderColor.Value);
        }
        else
        {
            DrawRectangleScreen(rq, bounds, backgroundColor);
        }

        // Draw centered text (simple center approximation)
        var textX = bounds.X + bounds.Width * 0.5f - text.Length * fontSize * 0.3f; // Rough text width estimation
        var textY = bounds.Y + bounds.Height * 0.5f - fontSize * 0.5f;
        DrawTextScreen(rq, text, new Position(textX, textY), textColor, fontSize);
    }

    /// <summary>
    ///     Draws a health bar with segments
    /// </summary>
    /// <param name="rq">Render command collector</param>
    /// <param name="bounds">Health bar boundaries</param>
    /// <param name="currentHealth">Current health value</param>
    /// <param name="maxHealth">Maximum health value</param>
    /// <param name="segments">Number of segments to divide the health bar into</param>
    /// <param name="fullColor">Color for full health segments</param>
    /// <param name="emptyColor">Color for empty health segments</param>
    protected void DrawHealthBar(IRenderCommandCollector rq, RectF bounds, float currentHealth,
        float maxHealth, int segments, Color fullColor, Color emptyColor)
    {
        if (maxHealth <= 0) return;

        var segmentWidth = bounds.Width / segments;
        var segmentSpacing = 1.0f; // Small gap between segments
        var actualSegmentWidth = segmentWidth - segmentSpacing;

        var healthRatio = currentHealth / maxHealth;
        var fullSegments = (int)(healthRatio * segments);

        for (int i = 0; i < segments; i++)
        {
            var segmentX = bounds.X + i * segmentWidth;
            var segmentBounds = new RectF(segmentX, bounds.Y, actualSegmentWidth, bounds.Height);

            var segmentColor = i < fullSegments ? fullColor : emptyColor;
            DrawRectangleScreen(rq, segmentBounds, segmentColor);
        }
    }
}
