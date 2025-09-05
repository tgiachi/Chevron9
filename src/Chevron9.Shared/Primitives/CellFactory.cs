using Chevron9.Shared.Graphics;

namespace Chevron9.Shared.Primitives;

/// <summary>
///     Factory class for creating Cell instances
///     Provides utility methods for common cell creation patterns
/// </summary>
public static class CellFactory
{
    /// <summary>
    ///     Creates an "empty" cell with a given visual payload
    ///     Typically used for initializing buffers
    /// </summary>
    /// <typeparam name="TVisual">The visual payload type</typeparam>
    /// <param name="visual">The visual payload for the empty cell</param>
    /// <returns>A new empty cell with white foreground and black background</returns>
    public static Cell<TVisual> CreateEmpty<TVisual>(TVisual visual)
    {
        return new Cell<TVisual>(visual, new Color(255, 255, 255), new Color(0, 0, 0));
    }
}
