using System.Diagnostics;

namespace Chevron9.Core.Render;

/// <summary>
///     Performance metrics for rendering operations
///     Tracks frame timing, command counts, and rendering statistics
/// </summary>
public sealed class RenderPerformanceMetrics
{
    private readonly Stopwatch _frameTimer = new();
    private int _commandsThisFrame;
    private int _layersThisFrame;

    /// <summary>
    ///     Gets the number of render commands submitted this frame
    /// </summary>
    public int CommandsPerFrame => _commandsThisFrame;

    /// <summary>
    ///     Gets the number of layers rendered this frame
    /// </summary>
    public int LayersRendered => _layersThisFrame;

    /// <summary>
    ///     Gets the time spent rendering the last frame
    /// </summary>
    public TimeSpan LastFrameRenderTime { get; private set; }

    /// <summary>
    ///     Gets the average render time over the last 60 frames
    /// </summary>
    public TimeSpan AverageRenderTime { get; private set; }

    /// <summary>
    ///     Gets the maximum render time in the last 60 frames
    /// </summary>
    public TimeSpan MaxRenderTime { get; private set; }

    /// <summary>
    ///     Gets the minimum render time in the last 60 frames
    /// </summary>
    public TimeSpan MinRenderTime { get; private set; }

    /// <summary>
    ///     Gets the frames per second based on average render time
    /// </summary>
    public double FramesPerSecond =>
        AverageRenderTime.TotalSeconds > 0 ? 1.0 / AverageRenderTime.TotalSeconds : 0;

    // Rolling average calculation
    private readonly Queue<TimeSpan> _frameTimes = new();
    private const int MaxSamples = 60;

    /// <summary>
    ///     Starts timing a render frame
    /// </summary>
    public void BeginFrame()
    {
        _frameTimer.Restart();
        _commandsThisFrame = 0;
        _layersThisFrame = 0;
    }

    /// <summary>
    ///     Ends timing a render frame and updates metrics
    /// </summary>
    public void EndFrame()
    {
        _frameTimer.Stop();
        LastFrameRenderTime = _frameTimer.Elapsed;

        // Update rolling average
        _frameTimes.Enqueue(LastFrameRenderTime);
        if (_frameTimes.Count > MaxSamples)
        {
            _frameTimes.Dequeue();
        }

        // Calculate statistics
        if (_frameTimes.Count > 0)
        {
            AverageRenderTime = TimeSpan.FromTicks((long)_frameTimes.Average(t => t.Ticks));
            MaxRenderTime = _frameTimes.Max();
            MinRenderTime = _frameTimes.Min();
        }
    }

    /// <summary>
    ///     Records that a render command was submitted
    /// </summary>
    public void RecordCommand() => _commandsThisFrame++;

    /// <summary>
    ///     Records that a layer was rendered
    /// </summary>
    public void RecordLayer() => _layersThisFrame++;

    /// <summary>
    ///     Resets all metrics to initial state
    /// </summary>
    public void Reset()
    {
        _frameTimes.Clear();
        _commandsThisFrame = 0;
        _layersThisFrame = 0;
        LastFrameRenderTime = TimeSpan.Zero;
        AverageRenderTime = TimeSpan.Zero;
        MaxRenderTime = TimeSpan.Zero;
        MinRenderTime = TimeSpan.Zero;
    }

    /// <summary>
    ///     Gets a formatted string with current performance statistics
    /// </summary>
    /// <returns>Human-readable performance summary</returns>
    public override string ToString()
    {
        return $"FPS: {FramesPerSecond:F1} | Commands: {_commandsThisFrame} | Layers: {_layersThisFrame} | " +
               $"Avg: {AverageRenderTime.TotalMilliseconds:F2}ms | Max: {MaxRenderTime.TotalMilliseconds:F2}ms";
    }
}
