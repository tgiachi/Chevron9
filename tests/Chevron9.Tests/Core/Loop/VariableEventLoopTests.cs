using Chevron9.Core.Data.Configs;
using Chevron9.Core.Loop;

namespace Chevron9.Tests.Core.Loop;

[TestFixture]
public class VariableEventLoopTests
{
    [Test]
    public void Constructor_ShouldInitializeWithConfig()
    {
        var config = new VariableEventLoopConfig(0.1);
        var eventLoop = new VariableEventLoop(config);

        Assert.That(eventLoop.Config, Is.EqualTo(config));
        Assert.That(eventLoop.Total, Is.EqualTo(0.0));
        Assert.That(eventLoop.Delta, Is.EqualTo(0.0));
    }

    [Test]
    public void Tick_ShouldUpdateTimingValues()
    {
        var config = new VariableEventLoopConfig(0.1);
        var eventLoop = new VariableEventLoop(config);

        Thread.Sleep(10); // Wait a small amount
        eventLoop.Tick();

        Assert.That(eventLoop.Delta, Is.GreaterThan(0.0));
        Assert.That(eventLoop.Total, Is.GreaterThan(0.0));
        Assert.That(eventLoop.Total, Is.EqualTo(eventLoop.Delta));
    }

    [Test]
    public void Tick_WithMultipleCalls_ShouldAccumulateTotal()
    {
        var config = new VariableEventLoopConfig(0.1);
        var eventLoop = new VariableEventLoop(config);

        Thread.Sleep(5);
        eventLoop.Tick();
        var firstDelta = eventLoop.Delta;
        var firstTotal = eventLoop.Total;

        Thread.Sleep(5);
        eventLoop.Tick();
        var secondDelta = eventLoop.Delta;
        var secondTotal = eventLoop.Total;

        Assert.That(secondTotal, Is.GreaterThan(firstTotal));
        Assert.That(secondTotal, Is.EqualTo(firstDelta + secondDelta).Within(0.001));
    }

    [Test]
    public void Tick_ShouldProvideVariableDeltaTimes()
    {
        var config = new VariableEventLoopConfig(0.1);
        var eventLoop = new VariableEventLoop(config);

        // Take different length sleeps
        Thread.Sleep(5);
        eventLoop.Tick();
        var shortDelta = eventLoop.Delta;

        Thread.Sleep(15);
        eventLoop.Tick();
        var longDelta = eventLoop.Delta;

        // Variable timestep should show different deltas
        Assert.That(longDelta, Is.GreaterThan(shortDelta));
        Assert.That(shortDelta, Is.GreaterThan(0.0));
        Assert.That(longDelta, Is.GreaterThan(0.0));
    }

    [Test]
    public void MaxFrameTime_ShouldClampExcessiveFrameTime()
    {
        var config = new VariableEventLoopConfig(0.05); // 50ms max frame time
        var eventLoop = new VariableEventLoop(config);

        Thread.Sleep(100); // Sleep longer than max frame time
        eventLoop.Tick();

        Assert.That(eventLoop.Delta, Is.LessThanOrEqualTo(config.MaxFrameTime));
        Assert.That(eventLoop.Delta, Is.EqualTo(config.MaxFrameTime));
    }

    [Test]
    public void Timing_ShouldBeConsistentAcrossFrames()
    {
        var config = new VariableEventLoopConfig(1.0);
        var eventLoop = new VariableEventLoop(config);

        var deltas = new List<double>();
        
        // Collect several frame deltas with consistent timing
        for (int i = 0; i < 5; i++)
        {
            Thread.Sleep(10); // Consistent 10ms sleep
            eventLoop.Tick();
            deltas.Add(eventLoop.Delta);
        }

        // All deltas should be reasonably similar (within 50% of each other)
        var minDelta = deltas.Min();
        var maxDelta = deltas.Max();
        var deltaRange = maxDelta - minDelta;
        var averageDelta = deltas.Average();

        Assert.That(deltaRange, Is.LessThan(averageDelta * 0.5)); // Range within 50% of average
        Assert.That(eventLoop.Total, Is.EqualTo(deltas.Sum()).Within(0.001));
    }

    [Test]
    public void EventLoop_ShouldHandleZeroInitialDelta()
    {
        var config = new VariableEventLoopConfig(0.1);
        var eventLoop = new VariableEventLoop(config);

        // Immediately call Tick without any wait
        eventLoop.Tick();

        // Delta should be very small but non-negative
        Assert.That(eventLoop.Delta, Is.GreaterThanOrEqualTo(0.0));
        Assert.That(eventLoop.Delta, Is.LessThan(0.001)); // Should be very small
        Assert.That(eventLoop.Total, Is.EqualTo(eventLoop.Delta));
    }

    [Test]
    public void Config_ShouldRetainOriginalValues()
    {
        var expectedMaxFrameTime = 0.123;
        var config = new VariableEventLoopConfig(expectedMaxFrameTime);
        var eventLoop = new VariableEventLoop(config);

        Assert.That(eventLoop.Config.MaxFrameTime, Is.EqualTo(expectedMaxFrameTime));
        
        // Config should remain unchanged after ticks
        eventLoop.Tick();
        Thread.Sleep(5);
        eventLoop.Tick();
        
        Assert.That(eventLoop.Config.MaxFrameTime, Is.EqualTo(expectedMaxFrameTime));
    }
}