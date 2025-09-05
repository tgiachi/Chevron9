using Chevron9.Core.Types;

namespace Chevron9.Core.Interfaces;

/// <summary>
/// Interface for a rendering layer within a scene
/// Layers provide depth sorting, camera transforms, and compositing control
/// </summary>
public interface ILayer
{
    /// <summary>
    /// Gets the layer name for identification and debugging
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the Z-index for depth sorting (higher values render on top)
    /// </summary>
    int ZIndex { get; }
    
    /// <summary>
    /// Gets whether this layer processes updates and input
    /// </summary>
    bool Enabled { get; }
    
    /// <summary>
    /// Gets whether this layer is rendered to screen
    /// </summary>
    bool Visible { get; }
    
    /// <summary>
    /// Gets the camera for world-to-screen coordinate transformation
    /// </summary>
    ICamera2D Camera { get; }

    /// <summary>
    /// Gets the clear flags determining what to clear before rendering this layer
    /// </summary>
    LayerClearFlags Clear { get; }
    
    /// <summary>
    /// Gets the compositing mode for blending this layer with previous layers
    /// </summary>
    LayerComposeMode Compose { get; }

    /// <summary>
    /// Updates layer logic with fixed timestep
    /// </summary>
    /// <param name="fixedDt">Fixed delta time in seconds</param>
    /// <param name="input">Input device for user interaction</param>
    void Update(double fixedDt, IInputDevice input);
    
    /// <summary>
    /// Renders layer content to the render queue using layer camera
    /// </summary>
    /// <param name="rq">Render queue to submit commands to</param>
    /// <param name="alpha">Interpolation alpha for smooth rendering</param>
    void Render(IRenderQueue rq, float alpha);
    
    /// <summary>
    /// Processes input for this layer
    /// </summary>
    /// <param name="input">Input device to process</param>
    /// <returns>True if input was consumed by this layer</returns>
    bool HandleInput(IInputDevice input);
}
