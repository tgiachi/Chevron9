using Chevron9.Core.Render;
using NUnit.Framework;

namespace Chevron9.Tests.Core.Render;

[TestFixture]
public class RenderPerformanceMetricsTests
{
    [Test]
    public void BeginFrame_EndFrame_CalculatesRenderTime()
    {
        var metrics = new RenderPerformanceMetrics();

        metrics.BeginFrame();
        // Simulate some work
        System.Threading.Thread.Sleep(10);
        metrics.EndFrame();

        Assert.That(metrics.LastFrameRenderTime.TotalMilliseconds, Is.GreaterThan(5));
        Assert.That(metrics.LastFrameRenderTime.TotalMilliseconds, Is.LessThan(50));
    }

    [Test]
    public void RecordCommand_IncrementsCommandCount()
    {
        var metrics = new RenderPerformanceMetrics();

        metrics.RecordCommand();
        metrics.RecordCommand();
        metrics.RecordCommand();

        Assert.That(metrics.CommandsPerFrame, Is.EqualTo(3));
    }

    [Test]
    public void RecordLayer_IncrementsLayerCount()
    {
        var metrics = new RenderPerformanceMetrics();

        metrics.RecordLayer();
        metrics.RecordLayer();

        Assert.That(metrics.LayersRendered, Is.EqualTo(2));
    }

    [Test]
    public void Reset_ClearsAllMetrics()
    {
        var metrics = new RenderPerformanceMetrics();

        metrics.RecordCommand();
        metrics.RecordLayer();
        metrics.Reset();

        Assert.That(metrics.CommandsPerFrame, Is.EqualTo(0));
        Assert.That(metrics.LayersRendered, Is.EqualTo(0));
        Assert.That(metrics.LastFrameRenderTime, Is.EqualTo(System.TimeSpan.Zero));
        Assert.That(metrics.AverageRenderTime, Is.EqualTo(System.TimeSpan.Zero));
    }

    [Test]
    public void ToString_ReturnsFormattedPerformanceSummary()
    {
        var metrics = new RenderPerformanceMetrics();

        metrics.RecordCommand();
        metrics.RecordLayer();

        var summary = metrics.ToString();

        Assert.That(summary, Does.Contain("FPS"));
        Assert.That(summary, Does.Contain("Commands: 1"));
        Assert.That(summary, Does.Contain("Layers: 1"));
    }
}
