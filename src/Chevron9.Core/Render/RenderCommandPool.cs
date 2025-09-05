using Chevron9.Core.Render.Commands;
using Chevron9.Core.Utils;

namespace Chevron9.Core.Render;

/// <summary>
///     Object pool manager for render commands to reduce allocations
/// </summary>
internal sealed class RenderCommandPool
{
    private readonly ObjectPool<PooledDrawRectangleCommand> _rectanglePool;
    private readonly ObjectPool<PooledDrawTextCommand> _textPool;

    /// <summary>
    ///     Initializes a new RenderCommandPool
    /// </summary>
    public RenderCommandPool()
    {
        _rectanglePool = new ObjectPool<PooledDrawRectangleCommand>(
            () => new PooledDrawRectangleCommand(),
            cmd =>
            {
                cmd.Bounds = default;
                cmd.Color = default;
            });

        _textPool = new ObjectPool<PooledDrawTextCommand>(
            () => new PooledDrawTextCommand(),
            cmd =>
            {
                cmd.Text = string.Empty;
                cmd.Position = default;
                cmd.Color = default;
                cmd.FontSize = 12.0f;
            });
    }

    /// <summary>
    ///     Gets a pooled DrawRectangleCommand
    /// </summary>
    /// <returns>Pooled rectangle command instance</returns>
    internal PooledDrawRectangleCommand GetRectangleCommand() => _rectanglePool.Get();

    /// <summary>
    ///     Returns a DrawRectangleCommand to the pool
    /// </summary>
    /// <param name="command">Command to return</param>
    internal void ReturnRectangleCommand(PooledDrawRectangleCommand command) => _rectanglePool.Return(command);

    /// <summary>
    ///     Gets a pooled DrawTextCommand
    /// </summary>
    /// <returns>Pooled text command instance</returns>
    internal PooledDrawTextCommand GetTextCommand() => _textPool.Get();

    /// <summary>
    ///     Returns a DrawTextCommand to the pool
    /// </summary>
    /// <param name="command">Command to return</param>
    internal void ReturnTextCommand(PooledDrawTextCommand command) => _textPool.Return(command);
}
