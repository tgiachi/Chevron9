using Chevron9.Core.Render;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Interfaces;

/// <summary>
///     Interface for collecting and sorting render commands for optimal batching
///     Commands are sorted by layer, material, and custom sort keys for performance
/// </summary>
public interface IRenderCommandCollector
{
    /// <summary>
    ///     Submits a render command with full sorting control
    /// </summary>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching similar draws</param>
    /// <param name="sortKey">Custom sort key for fine-grained ordering</param>
    /// <param name="cmd">Render command to submit</param>
    void Submit(int layerZ, int materialKey, int sortKey, RenderCommand cmd);

    /// <summary>
    ///     Submits a render command with default material and sort keys
    /// </summary>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="cmd">Render command to submit</param>
    void Submit(int layerZ, RenderCommand cmd)
    {
        Submit(layerZ, 0, 0, cmd);
    }

    /// <summary>
    ///     Flushes all queued commands, returning them sorted for optimal rendering
    ///     Commands are sorted by layer Z, then material, then sort key
    /// </summary>
    /// <returns>Sorted list of render items ready for rendering</returns>
    IReadOnlyList<RenderItem> FlushSorted();

    /// <summary>
    ///     Clears all queued commands, typically called after rendering a frame
    /// </summary>
    void Clear();

    /// <summary>
    ///     Submits a pooled rectangle command for optimal memory usage
    /// </summary>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="bounds">Rectangle boundaries</param>
    /// <param name="color">Fill color</param>
    void SubmitRectangle(int layerZ, RectF bounds, Color color);

    /// <summary>
    ///     Submits a pooled text command for optimal memory usage
    /// </summary>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="text">Text content</param>
    /// <param name="position">Text position</param>
    /// <param name="color">Text color</param>
    /// <param name="fontSize">Font size</param>
    void SubmitText(int layerZ, string text, Position position, Color color, float fontSize = 12.0f);
}
