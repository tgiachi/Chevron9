using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Interfaces;

/// <summary>
/// Interface for 2D camera providing world-to-screen coordinate transformation
/// Supports zoom, origin offset, and view rectangle calculation
/// </summary>
public interface ICamera2D
{
    /// <summary>
    /// Transforms world coordinates to screen coordinates using camera transformation
    /// </summary>
    /// <param name="world">World position to transform</param>
    /// <returns>Screen position after camera transformation</returns>
    Position WorldToScreen(Position world);
    
    /// <summary>
    /// Gets the world rectangle currently visible by this camera
    /// Useful for culling objects outside the view
    /// </summary>
    RectF ViewWorldRect { get; }
    
    /// <summary>
    /// Gets the camera zoom level (1.0 = normal, >1.0 = zoomed in, <1.0 = zoomed out)
    /// </summary>
    float Zoom { get; }
    
    /// <summary>
    /// Gets the camera origin/center position in world coordinates
    /// </summary>
    Position Origin { get; }
}
