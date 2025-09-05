using Chevron9.Core.Interfaces;

namespace Chevron9.Core.Render;

/// <summary>
///     High-performance render command collector with sorting for optimal batching
///     Commands are collected and sorted by layer Z, material key, then custom sort key
/// </summary>
public sealed class RenderCommandCollector : IRenderCommandCollector
{
    private readonly List<RenderItem> _items = [];

    /// <summary>
    ///     Submits a render command with full sorting control
    /// </summary>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching similar draws</param>
    /// <param name="sortKey">Custom sort key for fine-grained ordering</param>
    /// <param name="cmd">Render command to submit</param>
    public void Submit(int layerZ, int materialKey, int sortKey, RenderCommand cmd)
    {
        _items.Add(new RenderItem(layerZ, materialKey, sortKey, cmd));
    }

    /// <summary>
    ///     Submits a render command with default material and sort keys
    /// </summary>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="cmd">Render command to submit</param>
    public void Submit(int layerZ, RenderCommand cmd)
    {
        Submit(layerZ, 0, 0, cmd);
    }

    /// <summary>
    ///     Flushes all queued commands, returning them sorted for optimal rendering
    ///     Commands are sorted by layer Z, then material, then sort key
    /// </summary>
    /// <returns>Sorted list of render items ready for rendering</returns>
    public IReadOnlyList<RenderItem> FlushSorted()
    {
        _items.Sort(static (a, b) =>
        {
            var layerCompare = a.LayerZ.CompareTo(b.LayerZ);
            if (layerCompare != 0)
            {
                return layerCompare;
            }

            var materialCompare = a.MaterialKey.CompareTo(b.MaterialKey);
            if (materialCompare != 0)
            {
                return materialCompare;
            }

            return a.SortKey.CompareTo(b.SortKey);
        });

        return _items.ToArray();
    }

    /// <summary>
    ///     Clears all queued commands, typically called after rendering a frame
    /// </summary>
    public void Clear()
    {
        _items.Clear();
    }
}
