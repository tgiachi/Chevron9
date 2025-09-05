using Chevron9.Core.Cameras;
using Chevron9.Core.Interfaces;
using Chevron9.Core.Types;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Layers;

/// <summary>
///     Abstract base class for rendering layers within a scene
///     Provides common layer functionality and default implementations
/// </summary>
public abstract class AbstractLayer : ILayer
{
    private readonly Camera2D _camera;

    /// <summary>
    ///     Initializes a new instance of the Layer class with default settings
    /// </summary>
    /// <param name="name">Layer name for identification</param>
    /// <param name="zIndex">Z-index for depth sorting</param>
    protected AbstractLayer(string name, int zIndex)
    {
        Name = name;
        ZIndex = zIndex;
        Enabled = true;
        Visible = true;
        Clear = LayerClear.None;
        Compose = LayerComposeMode.Overwrite;

        _camera = new Camera2D();
    }

    /// <summary>
    ///     Initializes a new instance of the Layer class with custom settings
    /// </summary>
    /// <param name="name">Layer name for identification</param>
    /// <param name="zIndex">Z-index for depth sorting</param>
    /// <param name="enabled">Whether layer processes updates and input</param>
    /// <param name="visible">Whether layer is rendered</param>
    /// <param name="clear">Clear flags for layer</param>
    /// <param name="compose">Compositing mode for layer</param>
    protected AbstractLayer(string name, int zIndex, bool enabled, bool visible, LayerClear clear, LayerComposeMode compose)
    {
        Name = name;
        ZIndex = zIndex;
        Enabled = enabled;
        Visible = visible;
        Clear = clear;
        Compose = compose;

        _camera = new Camera2D();
    }

    /// <summary>
    ///     Gets the layer name for identification and debugging
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the Z-index for depth sorting (higher values render on top)
    /// </summary>
    public int ZIndex { get; }

    /// <summary>
    ///     Gets whether this layer processes updates and input
    /// </summary>
    public bool Enabled { get; protected set; }

    /// <summary>
    ///     Gets whether this layer is rendered to screen
    /// </summary>
    public bool Visible { get; protected set; }

    /// <summary>
    ///     Gets the camera for world-to-screen coordinate transformation
    /// </summary>
    public ICamera2D Camera => _camera;

    /// <summary>
    ///     Gets the clear flags determining what to clear before rendering this layer
    /// </summary>
    public LayerClear Clear { get; protected set; }

    /// <summary>
    ///     Gets the compositing mode for blending this layer with previous layers
    /// </summary>
    public LayerComposeMode Compose { get; protected set; }

    /// <summary>
    ///     Updates layer logic with fixed timestep
    /// </summary>
    /// <param name="fixedDt">Fixed delta time in seconds</param>
    /// <param name="input">Input device for user interaction</param>
    public virtual void Update(double fixedDt, IInputDevice input)
    {
        // Default implementation does nothing
        // Override in derived classes to implement layer-specific update logic
    }

    /// <summary>
    ///     Renders layer content to the render command collector using layer camera
    /// </summary>
    /// <param name="rq">Render command collector to submit commands to</param>
    /// <param name="alpha">Interpolation alpha for smooth rendering</param>
    public abstract void Render(IRenderCommandCollector rq, float alpha);

    /// <summary>
    ///     Processes input for this layer
    /// </summary>
    /// <param name="input">Input device to process</param>
    /// <returns>True if input was consumed by this layer</returns>
    public virtual bool HandleInput(IInputDevice input)
    {
        // Default implementation consumes no input
        // Override in derived classes to handle layer-specific input
        return false;
    }

    /// <summary>
    ///     Enables or disables the layer
    /// </summary>
    /// <param name="enabled">Whether to enable the layer</param>
    protected void SetEnabled(bool enabled)
    {
        Enabled = enabled;
    }

    /// <summary>
    ///     Shows or hides the layer
    /// </summary>
    /// <param name="visible">Whether to show the layer</param>
    protected void SetVisible(bool visible)
    {
        Visible = visible;
    }

    /// <summary>
    ///     Sets the clear flags for this layer
    /// </summary>
    /// <param name="clear">Clear flags to set</param>
    protected void SetClear(LayerClear clear)
    {
        Clear = clear;
    }

    /// <summary>
    ///     Sets the compositing mode for this layer
    /// </summary>
    /// <param name="compose">Compositing mode to set</param>
    protected void SetCompose(LayerComposeMode compose)
    {
        Compose = compose;
    }
}
