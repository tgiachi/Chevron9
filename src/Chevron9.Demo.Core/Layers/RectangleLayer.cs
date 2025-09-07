using Chevron9.Core.Interfaces;
using Chevron9.Core.Layers;
using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Demo.Core.Layers;

/// <summary>
///     Layer for rendering a rectangle demo
/// </summary>
public class RectangleLayer : AbstractLayer
{
    private readonly RectF _rectangleBounds;
    private readonly Color _rectangleColor;
    private float _rotationAngle;

    /// <summary>
    ///     Initializes a new instance of the RectangleLayer class
    /// </summary>
    /// <param name="bounds">Bounds of the rectangle</param>
    /// <param name="color">Color of the rectangle</param>
    public RectangleLayer(RectF bounds, Color color)
        : base("RectangleLayer", 200) // World layer Z-index
    {
        _rectangleBounds = bounds;
        _rectangleColor = color;
        _rotationAngle = 0.0f;
    }

    /// <summary>
    ///     Updates the rectangle animation
    /// </summary>
    /// <param name="fixedDt">Fixed delta time in seconds</param>
    /// <param name="input">Input device for user interaction</param>
    public override void Update(double fixedDt, IInputDevice input)
    {
        // Rotate the rectangle slowly
        _rotationAngle += (float)(fixedDt * 45.0); // 45 degrees per second
        if (_rotationAngle >= 360.0f)
        {
            _rotationAngle -= 360.0f;
        }
    }

    /// <summary>
    ///     Renders the rectangle to the render command collector
    /// </summary>
    /// <param name="rq">Render command collector to submit commands to</param>
    /// <param name="alpha">Interpolation alpha for smooth rendering</param>
    public override void Render(IRenderCommandCollector rq, float alpha)
    {
        // Render the main rectangle
        rq.Submit(ZIndex, new DrawRectangleCommand(_rectangleBounds, _rectangleColor));

        // Render a rotating outline
        var outlineColor = new Color(255, 255, 255, 128); // Semi-transparent white
        rq.Submit(ZIndex, new DrawRectangleOutlineCommand(_rectangleBounds, outlineColor, 2.0f));

        // Render center point
        var centerX = _rectangleBounds.X + _rectangleBounds.Width / 2;
        var centerY = _rectangleBounds.Y + _rectangleBounds.Height / 2;
        var centerPoint = new Position(centerX, centerY);
        rq.Submit(ZIndex, new DrawRectangleCommand(new RectF(centerX - 5, centerY - 5, 10, 10), Color.Red));

        // Render info text
        var infoText = $"Rectangle Demo - Rotation: {_rotationAngle:F1}°";
        rq.Submit(ZIndex, new DrawTextCommand(infoText, new Position(20, 20), Color.White, 14.0f));

        // Render bounds info
        var boundsText = $"Bounds: ({_rectangleBounds.X:F0}, {_rectangleBounds.Y:F0}) Size: {_rectangleBounds.Width:F0}x{_rectangleBounds.Height:F0}";
        rq.Submit(ZIndex, new DrawTextCommand(boundsText, new Position(20, 50), Color.Gray, 12.0f));
    }
}
