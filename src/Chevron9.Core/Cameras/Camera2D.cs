using System;
using Chevron9.Core.Interfaces;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Cameras;

/// <summary>
///     High-performance 2D camera implementation with position, zoom, rotation support
///     Provides world-to-screen coordinate transformations and view bounds calculation
/// </summary>
public sealed class Camera2D : ICamera2D
{
    private Position _position = new(0, 0);
    private float _zoom = 1.0f;
    private float _rotation;
    private RectF _viewport = new(0, 0, 800, 600);
    private RectF? _cachedViewBounds;
    private bool _transformDirty = true;

    /// <summary>
    ///     Gets or sets the camera position in world coordinates
    /// </summary>
    public Position Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                InvalidateTransform();
            }
        }
    }

    /// <summary>
    ///     Gets or sets the camera zoom level (1.0 = normal, >1.0 = zoomed in, <1.0 = zoomed out)
    /// </summary>
    public float Zoom
    {
        get => _zoom;
        set
        {
            var clampedZoom = Math.Max(0.001f, value);
            if (Math.Abs(_zoom - clampedZoom) > 0.0001f)
            {
                _zoom = clampedZoom;
                InvalidateTransform();
            }
        }
    }

    /// <summary>
    ///     Gets or sets the camera rotation angle in radians
    /// </summary>
    public float Rotation
    {
        get => _rotation;
        set
        {
            if (Math.Abs(_rotation - value) > 0.0001f)
            {
                _rotation = value;
                InvalidateTransform();
            }
        }
    }

    /// <summary>
    ///     Gets or sets the viewport dimensions in screen coordinates
    /// </summary>
    public RectF Viewport
    {
        get => _viewport;
        set
        {
            if (_viewport != value)
            {
                _viewport = value;
                InvalidateTransform();
            }
        }
    }

    /// <summary>
    ///     Gets the world rectangle currently visible by this camera
    ///     Useful for frustum culling objects outside the view
    /// </summary>
    public RectF ViewBounds
    {
        get
        {
            if (_transformDirty || !_cachedViewBounds.HasValue)
            {
                CalculateViewBounds();
            }
            return _cachedViewBounds!.Value;
        }
    }

    /// <summary>
    ///     Transforms world coordinates to screen coordinates using camera transformation
    /// </summary>
    /// <param name="worldPosition">World position to transform</param>
    /// <returns>Screen position after camera transformation</returns>
    public Position WorldToScreen(Position worldPosition)
    {
        var relativeX = worldPosition.X - _position.X;
        var relativeY = worldPosition.Y - _position.Y;

        if (Math.Abs(_rotation) > 0.0001f)
        {
            var cos = (float)Math.Cos(_rotation);
            var sin = (float)Math.Sin(_rotation);
            var rotatedX = relativeX * cos - relativeY * sin;
            var rotatedY = relativeX * sin + relativeY * cos;
            relativeX = rotatedX;
            relativeY = rotatedY;
        }

        var screenX = relativeX * _zoom + _viewport.Width * 0.5f;
        var screenY = relativeY * _zoom + _viewport.Height * 0.5f;

        return new Position(screenX, screenY);
    }

    /// <summary>
    ///     Transforms screen coordinates to world coordinates using inverse camera transformation
    /// </summary>
    /// <param name="screenPosition">Screen position to transform</param>
    /// <returns>World position after inverse camera transformation</returns>
    public Position ScreenToWorld(Position screenPosition)
    {
        var relativeX = (screenPosition.X - _viewport.Width * 0.5f) / _zoom;
        var relativeY = (screenPosition.Y - _viewport.Height * 0.5f) / _zoom;

        if (Math.Abs(_rotation) > 0.0001f)
        {
            var cos = (float)Math.Cos(-_rotation);
            var sin = (float)Math.Sin(-_rotation);
            var rotatedX = relativeX * cos - relativeY * sin;
            var rotatedY = relativeX * sin + relativeY * cos;
            relativeX = rotatedX;
            relativeY = rotatedY;
        }

        var worldX = relativeX + _position.X;
        var worldY = relativeY + _position.Y;

        return new Position(worldX, worldY);
    }

    /// <summary>
    ///     Moves the camera by the specified offset in world coordinates
    /// </summary>
    /// <param name="offset">Offset to move the camera by</param>
    public void Move(Position offset)
    {
        Position = new Position(_position.X + offset.X, _position.Y + offset.Y);
    }

    /// <summary>
    ///     Sets the camera to look at the specified world position
    /// </summary>
    /// <param name="worldPosition">World position to center the camera on</param>
    public void LookAt(Position worldPosition)
    {
        Position = worldPosition;
    }

    /// <summary>
    ///     Checks if a world position is visible by this camera
    /// </summary>
    /// <param name="worldPosition">World position to check</param>
    /// <returns>True if the position is within the camera's view bounds</returns>
    public bool IsVisible(Position worldPosition)
    {
        var bounds = ViewBounds;
        return worldPosition.X >= bounds.X &&
               worldPosition.X <= bounds.X + bounds.Width &&
               worldPosition.Y >= bounds.Y &&
               worldPosition.Y <= bounds.Y + bounds.Height;
    }

    /// <summary>
    ///     Checks if a world rectangle intersects with the camera's view bounds
    /// </summary>
    /// <param name="worldRect">World rectangle to check</param>
    /// <returns>True if the rectangle intersects with the view bounds</returns>
    public bool IsVisible(RectF worldRect)
    {
        var bounds = ViewBounds;
        return !(worldRect.X + worldRect.Width < bounds.X ||
                 worldRect.X > bounds.X + bounds.Width ||
                 worldRect.Y + worldRect.Height < bounds.Y ||
                 worldRect.Y > bounds.Y + bounds.Height);
    }

    private void InvalidateTransform()
    {
        _transformDirty = true;
        _cachedViewBounds = null;
    }

    private void CalculateViewBounds()
    {
        var halfWidth = _viewport.Width * 0.5f / _zoom;
        var halfHeight = _viewport.Height * 0.5f / _zoom;

        if (Math.Abs(_rotation) > 0.0001f)
        {
            var cos = Math.Abs(Math.Cos(_rotation));
            var sin = Math.Abs(Math.Sin(_rotation));
            var rotatedHalfWidth = (float)(halfWidth * cos + halfHeight * sin);
            var rotatedHalfHeight = (float)(halfWidth * sin + halfHeight * cos);
            halfWidth = rotatedHalfWidth;
            halfHeight = rotatedHalfHeight;
        }

        _cachedViewBounds = new RectF(
            _position.X - halfWidth,
            _position.Y - halfHeight,
            halfWidth * 2,
            halfHeight * 2
        );

        _transformDirty = false;
    }
}
