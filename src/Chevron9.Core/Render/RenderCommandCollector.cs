using Chevron9.Core.Interfaces;
using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Render;

/// <summary>
///     High-performance render command collector with sorting for optimal batching
///     Commands are collected and sorted by layer Z, material key, then custom sort key
///     Includes performance monitoring and validation
/// </summary>
public sealed class RenderCommandCollector : IRenderCommandCollector
{
    private readonly List<RenderItem> _items = [];
    private readonly RenderCommandPool? _commandPool;
    private readonly RenderPerformanceMetrics _metrics = new();

    /// <summary>
    ///     Gets the performance metrics for this collector
    /// </summary>
    public RenderPerformanceMetrics Metrics => _metrics;

    /// <summary>
    ///     Initializes a new RenderCommandCollector without pooling
    /// </summary>
    public RenderCommandCollector() : this(null) { }

    /// <summary>
    ///     Initializes a new RenderCommandCollector with optional command pooling
    /// </summary>
    /// <param name="commandPool">Optional pool for reusing command objects</param>
    internal RenderCommandCollector(RenderCommandPool? commandPool)
    {
        _commandPool = commandPool;
    }

    /// <summary>
    ///     Submits a render command with full sorting control
    /// </summary>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="materialKey">Material/texture key for batching similar draws</param>
    /// <param name="sortKey">Custom sort key for fine-grained ordering</param>
    /// <param name="cmd">Render command to submit</param>
    /// <exception cref="ArgumentNullException">Thrown when cmd is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when layerZ, materialKey, or sortKey are negative</exception>
    /// <exception cref="InvalidOperationException">Thrown when maximum commands per frame exceeded</exception>
    public void Submit(int layerZ, int materialKey, int sortKey, RenderCommand cmd)
    {
        ArgumentNullException.ThrowIfNull(cmd);
        Extensions.ValidationHelper.ValidateLayerZ(layerZ);
        Extensions.ValidationHelper.ValidateKeys(materialKey, sortKey);

        if (_items.Count >= RenderDefaults.MaxCommandsPerFrame)
        {
            throw new InvalidOperationException(
                $"Maximum commands per frame ({RenderDefaults.MaxCommandsPerFrame}) exceeded");
        }

        _items.Add(new RenderItem(layerZ, materialKey, sortKey, cmd));
        _metrics.RecordCommand();
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
    ///     Submits a pooled rectangle command for optimal memory usage
    /// </summary>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="bounds">Rectangle boundaries</param>
    /// <param name="color">Fill color</param>
    public void SubmitRectangle(int layerZ, RectF bounds, Color color)
    {
        if (_commandPool is null)
        {
            Submit(layerZ, new DrawRectangleCommand(bounds, color));
            return;
        }

        var pooledCmd = _commandPool.GetRectangleCommand();
        pooledCmd.Bounds = bounds;
        pooledCmd.Color = color;
        Submit(layerZ, pooledCmd.ToCommand());
        _commandPool.ReturnRectangleCommand(pooledCmd);
    }

    /// <summary>
    ///     Submits a pooled text command for optimal memory usage
    /// </summary>
    /// <param name="layerZ">Layer Z-index for depth sorting</param>
    /// <param name="text">Text content</param>
    /// <param name="position">Text position</param>
    /// <param name="color">Text color</param>
    /// <param name="fontSize">Font size</param>
    public void SubmitText(int layerZ, string text, Position position, Color color, float fontSize = 12.0f)
    {
        if (_commandPool is null)
        {
            Submit(layerZ, new DrawTextCommand(text, position, color, fontSize));
            return;
        }

        var pooledCmd = _commandPool.GetTextCommand();
        pooledCmd.Text = text;
        pooledCmd.Position = position;
        pooledCmd.Color = color;
        pooledCmd.FontSize = fontSize;
        Submit(layerZ, pooledCmd.ToCommand());
        _commandPool.ReturnTextCommand(pooledCmd);
    }

    /// <summary>
    ///     Flushes all queued commands, returning them sorted for optimal rendering
    ///     Commands are sorted by layer Z, then material, then sort key
    ///     Updates performance metrics for the frame
    /// </summary>
    /// <returns>Sorted list of render items ready for rendering</returns>
    public IReadOnlyList<RenderItem> FlushSorted()
    {
        _metrics.BeginFrame();

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

        _metrics.EndFrame();
        return _items;
    }

    /// <summary>
    ///     Clears all queued commands, typically called after rendering a frame
    ///     Resets command counter for the next frame
    /// </summary>
    public void Clear()
    {
        _items.Clear();
        // Note: Metrics are preserved across frames for performance tracking
    }
}
