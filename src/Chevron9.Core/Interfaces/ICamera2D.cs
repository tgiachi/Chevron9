using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Interfaces;

/// <summary>
///     Interface for 2D camera providing world-to-screen coordinate transformation
///     Supports position, zoom, rotation, and viewport management for 2D rendering
/// </summary>
public interface ICamera2D
{
    /// <summary>
    ///     Gets or sets the camera position in world coordinates
    /// </summary>
    Position Position { get; set; }

    /// <summary>
    ///     Gets or sets the camera zoom level (1.0 = normal, >1.0 = zoomed in, <1.0 = zoomed out)
    /// </summary>
    float Zoom { get; set; }

    /// <summary>
    ///     Gets or sets the camera rotation angle in radians
    /// </summary>
    float Rotation { get; set; }

    /// <summary>
    ///     Gets or sets the viewport dimensions in screen coordinates
    /// </summary>
    RectF Viewport { get; set; }

    /// <summary>
    ///     Gets the world rectangle currently visible by this camera
    ///     Useful for frustum culling objects outside the view
    /// </summary>
    RectF ViewBounds { get; }

    /// <summary>
    ///     Transforms world coordinates to screen coordinates using camera transformation
    /// </summary>
    /// <param name="worldPosition">World position to transform</param>
    /// <returns>Screen position after camera transformation</returns>
    Position WorldToScreen(Position worldPosition);

    /// <summary>
    ///     Transforms screen coordinates to world coordinates using inverse camera transformation
    /// </summary>
    /// <param name="screenPosition">Screen position to transform</param>
    /// <returns>World position after inverse camera transformation</returns>
    Position ScreenToWorld(Position screenPosition);

    /// <summary>
    ///     Moves the camera by the specified offset in world coordinates
    /// </summary>
    /// <param name="offset">Offset to move the camera by</param>
    void Move(Position offset);

    /// <summary>
    ///     Sets the camera to look at the specified world position
    /// </summary>
    /// <param name="worldPosition">World position to center the camera on</param>
    void LookAt(Position worldPosition);

    /// <summary>
    ///     Checks if a world position is visible by this camera
    /// </summary>
    /// <param name="worldPosition">World position to check</param>
    /// <returns>True if the position is within the camera's view bounds</returns>
    bool IsVisible(Position worldPosition);

    /// <summary>
    ///     Checks if a world rectangle intersects with the camera's view bounds
    /// </summary>
    /// <param name="worldRect">World rectangle to check</param>
    /// <returns>True if the rectangle intersects with the view bounds</returns>
    bool IsVisible(RectF worldRect);
}
