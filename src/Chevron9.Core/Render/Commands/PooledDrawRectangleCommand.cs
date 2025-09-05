using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Render.Commands;

/// <summary>
///     Pooled version of DrawRectangleCommand for reducing allocations
/// </summary>
internal sealed class PooledDrawRectangleCommand : IPooledRenderCommand
{
    /// <summary>
    ///     Rectangle boundaries
    /// </summary>
    internal RectF Bounds;

    /// <summary>
    ///     Fill color
    /// </summary>
    internal Color Color;

    /// <summary>
    ///     Gets the render command data as an immutable record
    /// </summary>
    /// <returns>Immutable DrawRectangleCommand</returns>
    public RenderCommand ToCommand() => new DrawRectangleCommand(Bounds, Color);
}
