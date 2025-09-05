using System.Diagnostics;
using Chevron9.Core.Data.Configs;
using Chevron9.Core.Interfaces;

namespace Chevron9.Core.Loop;

/// <summary>
/// High-performance fixed timestep event loop using GetTimestamp for precise timing
/// Provides fixed timestep updates with variable rendering interpolation
/// </summary>
public sealed class FixedEventLoop : IEventLoop
{
    private static readonly double TicksToSeconds = 1.0 / Stopwatch.Frequency;
    
    private long _lastTimestamp;
    private double _accumulator;

    public double Total { get; private set; }
    public double Delta { get; private set; }

    /// <summary>
    /// Gets the configuration for this event loop
    /// </summary>
    public FixedEventLoopConfig Config { get; }

    /// <summary>
    /// Initializes a new FixedEventLoop with the specified configuration
    /// </summary>
    /// <param name="config">Event loop configuration</param>
    public FixedEventLoop(FixedEventLoopConfig config)
    {
        Config = config;
        _lastTimestamp = Stopwatch.GetTimestamp();
    }

    /// <summary>
    /// Updates timing information for the current frame
    /// Call once per frame before processing updates
    /// </summary>
    public void Tick()
    {
        var currentTimestamp = Stopwatch.GetTimestamp();
        var frameTime = (currentTimestamp - _lastTimestamp) * TicksToSeconds;
        _lastTimestamp = currentTimestamp;

        // Clamp frame time to prevent spiral of death
        if (frameTime > Config.MaxFrameTime)
        {
            frameTime = Config.MaxFrameTime;
        }

        Delta = frameTime;
        Total += frameTime;
        _accumulator += frameTime;
    }

    /// <summary>
    /// Determines if a fixed timestep update should occur
    /// </summary>
    /// <returns>True if an update should be processed</returns>
    public bool ShouldUpdate()
    {
        if (_accumulator >= Config.FixedStep)
        {
            _accumulator -= Config.FixedStep;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the interpolation alpha for smooth rendering between fixed updates
    /// Value between 0.0 and 1.0 representing progress to next update
    /// </summary>
    public float Alpha => (float)(_accumulator / Config.FixedStep);
}
