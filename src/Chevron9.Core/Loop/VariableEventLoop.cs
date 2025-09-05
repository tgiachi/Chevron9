using System.Diagnostics;
using Chevron9.Core.Data.Configs;
using Chevron9.Core.Interfaces;

namespace Chevron9.Core.Loop;

/// <summary>
/// High-performance variable timestep event loop using GetTimestamp for precise timing
/// Provides variable delta time updates without fixed timestep constraints
/// </summary>
public sealed class VariableEventLoop : IEventLoop
{
    private static readonly double TicksToSeconds = 1.0 / Stopwatch.Frequency;
    
    private long _lastTimestamp;

    public double Total { get; private set; }
    public double Delta { get; private set; }

    /// <summary>
    /// Gets the configuration for this event loop
    /// </summary>
    public VariableEventLoopConfig Config { get; }

    /// <summary>
    /// Initializes a new VariableEventLoop with the specified configuration
    /// </summary>
    /// <param name="config">Event loop configuration</param>
    public VariableEventLoop(VariableEventLoopConfig config)
    {
        Config = config;
        _lastTimestamp = Stopwatch.GetTimestamp();
    }

    /// <summary>
    /// Updates timing information for the current frame
    /// Delta time varies based on actual frame duration
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
    }
}
