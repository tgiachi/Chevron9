namespace Chevron9.Core.Render;

/// <summary>
///     Interface for pooled render commands that can be reused to reduce allocations
/// </summary>
public interface IPooledRenderCommand
{
    /// <summary>
    ///     Gets the render command data as an immutable record
    /// </summary>
    /// <returns>Immutable render command</returns>
    RenderCommand ToCommand();
}