using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Render.Commands;

/// <summary>
///     Pooled version of DrawTextCommand for reducing allocations
/// </summary>
internal sealed class PooledDrawTextCommand : IPooledRenderCommand
{
    /// <summary>
    ///     Text content
    /// </summary>
    internal string Text = string.Empty;

    /// <summary>
    ///     Text position
    /// </summary>
    internal Position Position;

    /// <summary>
    ///     Text color
    /// </summary>
    internal Color Color;

    /// <summary>
    ///     Font size
    /// </summary>
    internal float FontSize = 12.0f;

    /// <summary>
    ///     Gets the render command data as an immutable record
    /// </summary>
    /// <returns>Immutable DrawTextCommand</returns>
    public RenderCommand ToCommand() => new DrawTextCommand(Text, Position, Color, FontSize);
}