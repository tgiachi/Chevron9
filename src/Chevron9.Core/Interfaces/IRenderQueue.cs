using Chevron9.Core.Render;

namespace Chevron9.Core.Interfaces;

/// <summary>
/// Interface for collecting and sorting render commands for optimal batching
/// Commands are sorted by layer, material, and custom sort keys for performance
/// </summary>
public interface IRenderQueue
{
    /// <summary>
    /// Submits a render command with full sorting control
    /// </summary>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching similar draws</param>
    /// <param name="sortKey">Custom sort key for fine-grained ordering</param>
    /// <param name="cmd">Render command to submit</param>
    void Submit(int layerZ, int materialKey, int sortKey, RenderCommand cmd);

    /// <summary>
    /// Submits a render command with default material and sort keys
    /// </summary>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="cmd">Render command to submit</param>
    void Submit(int layerZ, RenderCommand cmd) => Submit(layerZ, 0, 0, cmd);

    /// <summary>
    /// Flushes all queued commands, returning them sorted for optimal rendering
    /// Commands are sorted by layer Z, then material, then sort key
    /// </summary>
    /// <returns>Sorted list of render items ready for rendering</returns>
    IReadOnlyList<RenderItem> FlushSorted();
    
    /// <summary>
    /// Clears all queued commands, typically called after rendering a frame
    /// </summary>
    void Clear();
}
