using Chevron9.Core.Interfaces;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Extensions.Cameras;

/// <summary>
///     Extension methods for ICamera2D to provide common transformation operations
/// </summary>
public static class Camera2DExtensions
{
    /// <summary>
    ///     Transforms a world position to screen coordinates using the camera
    /// </summary>
    /// <param name="camera">Camera to use for transformation</param>
    /// <param name="worldPosition">Position in world coordinates</param>
    /// <returns>Position in screen coordinates</returns>
    public static Position TransformPosition(this ICamera2D camera, Position worldPosition)
    {
        // Use camera's built-in transformation
        return camera.WorldToScreen(worldPosition);
    }

    /// <summary>
    ///     Transforms world bounds to screen coordinates using the camera
    /// </summary>
    /// <param name="camera">Camera to use for transformation</param>
    /// <param name="worldBounds">Bounds in world coordinates</param>
    /// <returns>Bounds in screen coordinates</returns>
    public static RectF TransformBounds(this ICamera2D camera, RectF worldBounds)
    {
        // Transform position and scale size by zoom
        var screenPosition = camera.WorldToScreen(new Position(worldBounds.X, worldBounds.Y));
        var screenWidth = worldBounds.Width * camera.Zoom;
        var screenHeight = worldBounds.Height * camera.Zoom;

        return new RectF(screenPosition.X, screenPosition.Y, screenWidth, screenHeight);
    }

    /// <summary>
    ///     Transforms screen bounds back to world coordinates
    /// </summary>
    /// <param name="camera">Camera to use for transformation</param>
    /// <param name="screenBounds">Bounds in screen coordinates</param>
    /// <returns>Bounds in world coordinates</returns>
    public static RectF ScreenToBounds(this ICamera2D camera, RectF screenBounds)
    {
        // Transform position and scale size by inverse zoom
        var worldPosition = camera.ScreenToWorld(new Position(screenBounds.X, screenBounds.Y));
        var worldWidth = screenBounds.Width / camera.Zoom;
        var worldHeight = screenBounds.Height / camera.Zoom;

        return new RectF(worldPosition.X, worldPosition.Y, worldWidth, worldHeight);
    }

    /// <summary>
    ///     Checks if a world position is visible within the camera viewport
    /// </summary>
    /// <param name="camera">Camera to check visibility against</param>
    /// <param name="worldPosition">Position in world coordinates</param>
    /// <param name="viewportWidth">Viewport width in screen coordinates</param>
    /// <param name="viewportHeight">Viewport height in screen coordinates</param>
    /// <returns>True if the position is visible</returns>
    public static bool IsVisible(this ICamera2D camera, Position worldPosition,
        float viewportWidth, float viewportHeight)
    {
        var screenPos = camera.TransformPosition(worldPosition);

        return screenPos.X >= 0 && screenPos.X <= viewportWidth &&
               screenPos.Y >= 0 && screenPos.Y <= viewportHeight;
    }

    /// <summary>
    ///     Checks if world bounds are visible within the camera viewport (including partial visibility)
    /// </summary>
    /// <param name="camera">Camera to check visibility against</param>
    /// <param name="worldBounds">Bounds in world coordinates</param>
    /// <param name="viewportWidth">Viewport width in screen coordinates</param>
    /// <param name="viewportHeight">Viewport height in screen coordinates</param>
    /// <returns>True if any part of the bounds is visible</returns>
    public static bool IsVisible(this ICamera2D camera, RectF worldBounds,
        float viewportWidth, float viewportHeight)
    {
        var screenBounds = camera.TransformBounds(worldBounds);

        // Check if rectangles intersect (any part is visible)
        return screenBounds.X < viewportWidth &&
               screenBounds.X + screenBounds.Width > 0 &&
               screenBounds.Y < viewportHeight &&
               screenBounds.Y + screenBounds.Height > 0;
    }

    /// <summary>
    ///     Gets the world bounds that are currently visible by the camera
    /// </summary>
    /// <param name="camera">Camera to get viewport for</param>
    /// <param name="viewportWidth">Viewport width in screen coordinates</param>
    /// <param name="viewportHeight">Viewport height in screen coordinates</param>
    /// <returns>Bounds in world coordinates representing the visible area</returns>
    public static RectF GetWorldViewport(this ICamera2D camera, float viewportWidth, float viewportHeight)
    {
        // Convert screen viewport to world coordinates
        var topLeft = camera.ScreenToWorld(new Position(0, 0));
        var bottomRight = camera.ScreenToWorld(new Position(viewportWidth, viewportHeight));

        return new RectF(
            topLeft.X,
            topLeft.Y,
            bottomRight.X - topLeft.X,
            bottomRight.Y - topLeft.Y
        );
    }
}
