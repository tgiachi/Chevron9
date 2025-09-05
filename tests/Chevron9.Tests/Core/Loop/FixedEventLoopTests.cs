using Chevron9.Core.Data.Configs;
using Chevron9.Core.Loop;

namespace Chevron9.Tests.Core.Loop;

[TestFixture]
public class FixedEventLoopTests
{
    [Test]
    public void Constructor_ShouldInitializeWithConfig()
    {
        var config = FixedEventLoopConfig.Default60();
        var eventLoop = new FixedEventLoop(config);

        Assert.That(eventLoop.Config, Is.EqualTo(config));
        Assert.That(eventLoop.Total, Is.EqualTo(0.0));
        Assert.That(eventLoop.Delta, Is.EqualTo(0.0));
        Assert.That(eventLoop.Alpha, Is.EqualTo(0.0f));
    }

    [Test]
    public void Tick_ShouldUpdateTimingValues()
    {
        var config = FixedEventLoopConfig.Default60();
        var eventLoop = new FixedEventLoop(config);

        Thread.Sleep(10); // Wait a small amount
        eventLoop.Tick();

        Assert.That(eventLoop.Delta, Is.GreaterThan(0.0));
        Assert.That(eventLoop.Total, Is.GreaterThan(0.0));
        Assert.That(eventLoop.Total, Is.EqualTo(eventLoop.Delta));
    }

    [Test]
    public void Tick_WithMultipleCalls_ShouldAccumulateTotal()
    {
        var config = FixedEventLoopConfig.Default60();
        var eventLoop = new FixedEventLoop(config);

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
    public void ShouldUpdate_WithoutTick_ShouldReturnFalse()
    {
        var config = FixedEventLoopConfig.Default60();
        var eventLoop = new FixedEventLoop(config);

        Assert.That(eventLoop.ShouldUpdate(), Is.False);
    }

    [Test]
    public void ShouldUpdate_AfterSufficientTime_ShouldReturnTrue()
    {
        var config = new FixedEventLoopConfig(10, 1.0); // 10 FPS = 0.1s per frame
        var eventLoop = new FixedEventLoop(config);

        Thread.Sleep(120); // Sleep longer than fixed step
        eventLoop.Tick();

        Assert.That(eventLoop.ShouldUpdate(), Is.True);
    }

    [Test]
    public void ShouldUpdate_ConsumesSingleFixedStep()
    {
        var config = new FixedEventLoopConfig(10, 1.0); // 10 FPS = 0.1s per frame  
        var eventLoop = new FixedEventLoop(config);

        Thread.Sleep(120); // Sleep longer than fixed step
        eventLoop.Tick();

        var firstUpdate = eventLoop.ShouldUpdate();
        var secondUpdate = eventLoop.ShouldUpdate();

        Assert.That(firstUpdate, Is.True);
        Assert.That(secondUpdate, Is.False);
    }

    [Test]
    public void ShouldUpdate_WithLongFrame_ShouldAllowMultipleUpdates()
    {
        var config = new FixedEventLoopConfig(10, 1.0); // 10 FPS = 0.1s per frame
        var eventLoop = new FixedEventLoop(config);

        Thread.Sleep(250); // Sleep for ~2.5 fixed steps
        eventLoop.Tick();

        var updateCount = 0;
        while (eventLoop.ShouldUpdate())
        {
            updateCount++;
            if (updateCount > 10) break; // Safety break
        }

        Assert.That(updateCount, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public void Alpha_ShouldBeZeroAfterUpdate()
    {
        var config = new FixedEventLoopConfig(10, 1.0); // 10 FPS = 0.1s per frame
        var eventLoop = new FixedEventLoop(config);

        Thread.Sleep(120);
        eventLoop.Tick();
        eventLoop.ShouldUpdate(); // Consume the update

        Assert.That(eventLoop.Alpha, Is.LessThan(0.5f));
    }

    [Test]
    public void Alpha_ShouldIncreaseBetweenUpdates()
    {
        var config = new FixedEventLoopConfig(10, 1.0); // 10 FPS = 0.1s per frame
        var eventLoop = new FixedEventLoop(config);

        Thread.Sleep(50); // Half a fixed step
        eventLoop.Tick();

        Assert.That(eventLoop.Alpha, Is.GreaterThan(0.0f));
        Assert.That(eventLoop.Alpha, Is.LessThan(1.0f));
    }

    [Test]
    public void MaxFrameTime_ShouldClampExcessiveFrameTime()
    {
        var config = new FixedEventLoopConfig(60, 0.1); // Max 0.1s frame time
        var eventLoop = new FixedEventLoop(config);

        Thread.Sleep(200); // Sleep longer than max frame time
        eventLoop.Tick();

        Assert.That(eventLoop.Delta, Is.LessThanOrEqualTo(config.MaxFrameTime));
    }

    [Test]
    public void FixedStep_ShouldMatchFramesPerSecond()
    {
        var config30 = FixedEventLoopConfig.Terminal30();
        var config60 = FixedEventLoopConfig.Default60();

        Assert.That(config30.FixedStep, Is.EqualTo(1.0 / 30).Within(0.0001));
        Assert.That(config60.FixedStep, Is.EqualTo(1.0 / 60).Within(0.0001));
    }

    [Test]
    public void Terminal30Config_ShouldHaveCorrectValues()
    {
        var config = FixedEventLoopConfig.Terminal30();

        Assert.That(config.FramesPerSecond, Is.EqualTo(30));
        Assert.That(config.MaxFrameTime, Is.EqualTo(0.1));
        Assert.That(config.FixedStep, Is.EqualTo(1.0 / 30).Within(0.0001));
    }

    [Test]
    public void Default60Config_ShouldHaveCorrectValues()
    {
        var config = FixedEventLoopConfig.Default60();

        Assert.That(config.FramesPerSecond, Is.EqualTo(60));
        Assert.That(config.MaxFrameTime, Is.EqualTo(0.25));
        Assert.That(config.FixedStep, Is.EqualTo(1.0 / 60).Within(0.0001));
    }

    [Test]
    public void EventLoop_ShouldMaintainConsistentTiming()
    {
        var config = new FixedEventLoopConfig(30, 1.0);
        var eventLoop = new FixedEventLoop(config);

        var totalUpdates = 0;
        var startTime = DateTime.UtcNow;

        // Run for approximately 200ms
        while ((DateTime.UtcNow - startTime).TotalMilliseconds < 200)
        {
            Thread.Sleep(10);
            eventLoop.Tick();
            
            while (eventLoop.ShouldUpdate())
            {
                totalUpdates++;
                if (totalUpdates > 20) break; // Safety
            }
        }

        // Should have processed some updates, but not too many due to timing
        Assert.That(totalUpdates, Is.GreaterThan(0));
        Assert.That(totalUpdates, Is.LessThan(15)); // Reasonable upper bound
        Assert.That(eventLoop.Total, Is.GreaterThan(0.1)); // Should have accumulated time
    }
}