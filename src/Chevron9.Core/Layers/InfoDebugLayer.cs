using Chevron9.Core.Cameras;
using Chevron9.Core.Extensions;
using Chevron9.Core.Interfaces;
using Chevron9.Core.Render;
using Chevron9.Core.Types;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Layers;

/// <summary>
///     Global debug layer that displays mouse position and FPS in the top-right corner
///     Always visible and rendered on top of all scenes and other global layers
/// </summary>
public sealed class InfoDebugLayer : ILayer
{
    private const int DefaultZIndex = 10000; // Very high Z-index to be on top
    private const int FpsHistorySize = 60; // Keep 60 frames of history for FPS calculation

    private readonly Queue<double> _frameTimes = new();
    private readonly Camera2D _camera;
    private double _fps;
    private Position _mousePosition;
    private bool _enabled = true;
    private bool _visible = true;

    /// <summary>
    ///     Initializes a new instance of the InfoDebugLayer class
    /// </summary>
    public InfoDebugLayer()
    {
        Name = "InfoDebugLayer";
        ZIndex = DefaultZIndex;
        _camera = new Camera2D();
        Clear = LayerClear.None;
        Compose = LayerCompositeMode.Overwrite;
    }

    /// <summary>
    ///     Initializes a new instance of the InfoDebugLayer class with custom Z-index
    /// </summary>
    /// <param name="zIndex">Z-index for the debug layer</param>
    public InfoDebugLayer(int zIndex)
    {
        Name = "InfoDebugLayer";
        ZIndex = zIndex;
        _camera = new Camera2D();
        Clear = LayerClear.None;
        Compose = LayerCompositeMode.Overwrite;
    }

    /// <summary>
    ///     Gets the layer name for identification
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the Z-index for depth sorting
    /// </summary>
    public int ZIndex { get; }

    /// <summary>
    ///     Gets whether this debug layer is enabled
    /// </summary>
    public bool Enabled => _enabled;

    /// <summary>
    ///     Gets whether this debug layer is visible
    /// </summary>
    public bool Visible => _visible;

    /// <summary>
    ///     Gets the camera for world-to-screen coordinate transformation
    /// </summary>
    public ICamera2D Camera => _camera;

    /// <summary>
    ///     Gets the clear flags determining what to clear before rendering this layer
    /// </summary>
    public LayerClear Clear { get; }

    /// <summary>
    ///     Gets the compositing mode for blending this layer with previous layers
    /// </summary>
    public LayerCompositeMode Compose { get; }

    /// <summary>
    ///     Enables or disables the debug layer
    /// </summary>
    /// <param name="enabled">Whether to enable the layer</param>
    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }

    /// <summary>
    ///     Shows or hides the debug layer
    /// </summary>
    /// <param name="visible">Whether to show the layer</param>
    public void SetVisible(bool visible)
    {
        _visible = visible;
    }

    /// <summary>
    ///     Updates the debug layer with current mouse position and FPS calculation
    /// </summary>
    /// <param name="fixedDt">Fixed delta time in seconds</param>
    /// <param name="input">Input device for mouse position</param>
    public void Update(double fixedDt, IInputDevice input)
    {
        // Update mouse position
        _mousePosition = input.MousePosition();

        // Update FPS calculation
        _frameTimes.Enqueue(fixedDt);
        if (_frameTimes.Count > FpsHistorySize)
        {
            _frameTimes.Dequeue();
        }

        // Calculate average FPS
        if (_frameTimes.Count > 0)
        {
            var averageFrameTime = _frameTimes.Average();
            _fps = 1.0 / averageFrameTime;
        }
    }

    /// <summary>
    ///     Renders the debug information in the top-right corner
    /// </summary>
    /// <param name="rq">Render command collector to submit commands to</param>
    /// <param name="alpha">Interpolation alpha for smooth rendering</param>
    public void Render(IRenderCommandCollector rq, float alpha)
    {
        const float lineHeight = 16.0f;
        const float margin = RenderDefaults.Padding.Medium;

        var startX = RenderDefaults.DefaultScreenWidth - margin;
        var startY = margin;

        // Render FPS (legacy calculation)
        var fpsText = $"FPS: {_fps:F1}";
        rq.SubmitText(ZIndex, fpsText, startX - 80, startY, RenderDefaults.DebugTextColor, RenderDefaults.DefaultFontSize);

        // Render mouse position
        var mouseText = $"Mouse: ({_mousePosition.X:F0}, {_mousePosition.Y:F0})";
        rq.SubmitText(ZIndex, mouseText, startX - 150, startY + lineHeight, Color.Cyan, RenderDefaults.DefaultFontSize);

        // Render advanced performance metrics if available
        if (rq is RenderCommandCollector collector)
        {
            var metrics = collector.Metrics;
            var renderTimeText = $"Render: {metrics.LastFrameRenderTime.TotalMilliseconds:F2}ms";
            rq.SubmitText(ZIndex, renderTimeText, startX - 120, startY + lineHeight * 2, RenderDefaults.SuccessTextColor, RenderDefaults.SmallFontSize);

            var commandsText = $"Commands: {metrics.CommandsPerFrame}";
            rq.SubmitText(ZIndex, commandsText, startX - 100, startY + lineHeight * 3, RenderDefaults.DefaultTextColor, RenderDefaults.SmallFontSize);
        }

        // Render background for better readability
        var backgroundWidth = 160.0f;
        var backgroundHeight = lineHeight * 4 + RenderDefaults.Padding.Small;
        var backgroundX = startX - backgroundWidth + margin;
        var backgroundY = startY - RenderDefaults.Padding.Small;

        rq.SubmitRectangle(ZIndex - 1,
            new RectF(backgroundX, backgroundY, backgroundWidth, backgroundHeight),
            new Color(0, 0, 0, 180)); // Semi-transparent black background
    }

    /// <summary>
    ///     Processes input for the debug layer (currently no input handling)
    /// </summary>
    /// <param name="input">Input device to process</param>
    /// <returns>Always returns false as this layer doesn't consume input</returns>
    public bool HandleInput(IInputDevice input)
    {
        // Debug layer doesn't consume input, just displays information
        return false;
    }
}
