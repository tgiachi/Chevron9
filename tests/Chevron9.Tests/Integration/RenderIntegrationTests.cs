using Chevron9.Core.Extensions;
using Chevron9.Core.Render;
using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;
using NUnit.Framework;

namespace Chevron9.Tests.Integration;

[TestFixture]
public class RenderIntegrationTests
{
    [Test]
    public void RenderCommandCollector_WithExtensions_ValidatesAndSubmitsCommands()
    {
        var collector = new RenderCommandCollector();

        // Test valid submissions
        Assert.DoesNotThrow(() =>
        {
            collector.SubmitText(100, "Hello World", 10, 20, Color.White);
            collector.SubmitRectangle(200, 50, 50, 100, 100, Color.Red);
            collector.SubmitCircle(300, 150, 150, 25, Color.Blue);
            collector.SubmitLine(400, 0, 0, 100, 100, Color.Green);
        });

        var commands = collector.FlushSorted();
        Assert.That(commands.Count, Is.EqualTo(4));
    }

    [Test]
    public void RenderCommandCollector_WithValidInput_ProcessesSuccessfully()
    {
        var collector = new RenderCommandCollector();

        // Test valid submissions work correctly
        Assert.DoesNotThrow(() => collector.SubmitText(100, "Valid text", new Position(10, 20), Color.White));
        Assert.DoesNotThrow(() => collector.SubmitRectangle(100, new RectF(10, 20, 50, 30), Color.Red));
        Assert.DoesNotThrow(() => collector.SubmitCircle(100, new Position(150, 150), 25, Color.Blue));

        var commands = collector.FlushSorted();
        Assert.That(commands.Count, Is.EqualTo(3));
    }

    [Test]
    public void RenderCommandCollector_WithPerformanceMetrics_TracksFrameData()
    {
        var collector = new RenderCommandCollector();

        // Submit some commands
        collector.SubmitText(100, "Test", 10, 20, Color.White);
        collector.SubmitRectangle(200, 50, 50, 100, 100, Color.Red);

        // Flush and check metrics
        var commands = collector.FlushSorted();

        Assert.That(commands.Count, Is.EqualTo(2));
        Assert.That(collector.Metrics.LastFrameRenderTime, Is.GreaterThan(TimeSpan.Zero));
        // Note: CommandsPerFrame is reset after FlushSorted, so we check the count instead
    }

    [Test]
    public void RenderDefaults_ProvidesConsistentValues()
    {
        // Test that defaults are reasonable
        Assert.That(RenderDefaults.DefaultFontSize, Is.GreaterThan(0));
        Assert.That(RenderDefaults.BackgroundLayerZ, Is.GreaterThan(0));
        Assert.That(RenderDefaults.UiLayerZ, Is.GreaterThan(RenderDefaults.BackgroundLayerZ));
        Assert.That(RenderDefaults.DebugLayerZ, Is.GreaterThan(RenderDefaults.UiLayerZ));

        // Test color defaults
        Assert.That(RenderDefaults.DefaultTextColor, Is.Not.EqualTo(default(Color)));
        Assert.That(RenderDefaults.DefaultBackgroundColor, Is.Not.EqualTo(default(Color)));
    }

    [Test]
    public void RenderCommandCollector_CommandLimit_Enforced()
    {
        var collector = new RenderCommandCollector();

        // Submit maximum allowed commands
        for (int i = 0; i < RenderDefaults.MaxCommandsPerFrame; i++)
        {
            collector.SubmitText(100, $"Command{i}", 10, 20 + i, Color.White);
        }

        // Next command should throw
        Assert.Throws<InvalidOperationException>(() =>
            collector.SubmitText(100, "Overflow", 10, 20, Color.White));
    }

    [Test]
    public void RenderPerformanceMetrics_CalculatesRollingAverage()
    {
        var metrics = new RenderPerformanceMetrics();

        // Simulate multiple frames
        for (int i = 0; i < 10; i++)
        {
            metrics.BeginFrame();
            System.Threading.Thread.Sleep(1); // Small delay
            metrics.EndFrame();
        }

        Assert.That(metrics.AverageRenderTime, Is.GreaterThan(TimeSpan.Zero));
        Assert.That(metrics.FramesPerSecond, Is.GreaterThan(0));
    }
}
