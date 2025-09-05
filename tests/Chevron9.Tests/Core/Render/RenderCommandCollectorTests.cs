using Chevron9.Core.Render;
using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Core.Render;

[TestFixture]
public class RenderCommandCollectorTests
{
    private RenderCommandCollector _collector = null!;

    [SetUp]
    public void SetUp()
    {
        _collector = new RenderCommandCollector();
    }

    [Test]
    public void FlushSorted_WithEmptyCollector_ShouldReturnEmptyList()
    {
        var result = _collector.FlushSorted();

        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public void Submit_WithSingleCommand_ShouldReturnSingleItem()
    {
        var cmd = new DrawRectangleCommand(new RectF(0, 0, 10, 10), Color.Red);

        _collector.Submit(100, 0, 0, cmd);
        var result = _collector.FlushSorted();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].LayerZ, Is.EqualTo(100));
        Assert.That(result[0].MaterialKey, Is.EqualTo(0));
        Assert.That(result[0].SortKey, Is.EqualTo(0));
        Assert.That(result[0].Command, Is.EqualTo(cmd));
    }

    [Test]
    public void Submit_WithDefaultOverload_ShouldUseZeroKeys()
    {
        var cmd = new DrawRectangleCommand(new RectF(0, 0, 10, 10), Color.Red);

        _collector.Submit(200, cmd);
        var result = _collector.FlushSorted();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].LayerZ, Is.EqualTo(200));
        Assert.That(result[0].MaterialKey, Is.EqualTo(0));
        Assert.That(result[0].SortKey, Is.EqualTo(0));
    }

    [Test]
    public void FlushSorted_WithMultipleCommands_ShouldSortByLayerZ()
    {
        var cmd1 = new DrawRectangleCommand(new RectF(0, 0, 10, 10), Color.Red);
        var cmd2 = new DrawRectangleCommand(new RectF(10, 10, 20, 20), Color.Blue);
        var cmd3 = new DrawRectangleCommand(new RectF(20, 20, 30, 30), Color.Green);

        _collector.Submit(300, 0, 0, cmd1);
        _collector.Submit(100, 0, 0, cmd2);
        _collector.Submit(200, 0, 0, cmd3);

        var result = _collector.FlushSorted();

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result[0].LayerZ, Is.EqualTo(100));
        Assert.That(result[0].Command, Is.EqualTo(cmd2));
        Assert.That(result[1].LayerZ, Is.EqualTo(200));
        Assert.That(result[1].Command, Is.EqualTo(cmd3));
        Assert.That(result[2].LayerZ, Is.EqualTo(300));
        Assert.That(result[2].Command, Is.EqualTo(cmd1));
    }

    [Test]
    public void FlushSorted_WithSameLayerZ_ShouldSortByMaterialKey()
    {
        var cmd1 = new DrawRectangleCommand(new RectF(0, 0, 10, 10), Color.Red);
        var cmd2 = new DrawRectangleCommand(new RectF(10, 10, 20, 20), Color.Blue);
        var cmd3 = new DrawRectangleCommand(new RectF(20, 20, 30, 30), Color.Green);

        _collector.Submit(100, 30, 0, cmd1);
        _collector.Submit(100, 10, 0, cmd2);
        _collector.Submit(100, 20, 0, cmd3);

        var result = _collector.FlushSorted();

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result[0].MaterialKey, Is.EqualTo(10));
        Assert.That(result[0].Command, Is.EqualTo(cmd2));
        Assert.That(result[1].MaterialKey, Is.EqualTo(20));
        Assert.That(result[1].Command, Is.EqualTo(cmd3));
        Assert.That(result[2].MaterialKey, Is.EqualTo(30));
        Assert.That(result[2].Command, Is.EqualTo(cmd1));
    }

    [Test]
    public void FlushSorted_WithSameLayerZAndMaterial_ShouldSortBySortKey()
    {
        var cmd1 = new DrawRectangleCommand(new RectF(0, 0, 10, 10), Color.Red);
        var cmd2 = new DrawRectangleCommand(new RectF(10, 10, 20, 20), Color.Blue);
        var cmd3 = new DrawRectangleCommand(new RectF(20, 20, 30, 30), Color.Green);

        _collector.Submit(100, 0, 300, cmd1);
        _collector.Submit(100, 0, 100, cmd2);
        _collector.Submit(100, 0, 200, cmd3);

        var result = _collector.FlushSorted();

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result[0].SortKey, Is.EqualTo(100));
        Assert.That(result[0].Command, Is.EqualTo(cmd2));
        Assert.That(result[1].SortKey, Is.EqualTo(200));
        Assert.That(result[1].Command, Is.EqualTo(cmd3));
        Assert.That(result[2].SortKey, Is.EqualTo(300));
        Assert.That(result[2].Command, Is.EqualTo(cmd1));
    }

    [Test]
    public void FlushSorted_WithComplexSorting_ShouldSortCorrectly()
    {
        var cmd1 = new DrawRectangleCommand(new RectF(0, 0, 10, 10), Color.Red);
        var cmd2 = new DrawRectangleCommand(new RectF(10, 10, 20, 20), Color.Blue);
        var cmd3 = new DrawRectangleCommand(new RectF(20, 20, 30, 30), Color.Green);
        var cmd4 = new DrawRectangleCommand(new RectF(30, 30, 40, 40), Color.Yellow);

        _collector.Submit(200, 10, 300, cmd1);
        _collector.Submit(100, 20, 100, cmd2);
        _collector.Submit(200, 10, 100, cmd3);
        _collector.Submit(100, 10, 200, cmd4);

        var result = _collector.FlushSorted();

        Assert.That(result.Count, Is.EqualTo(4));
        
        // Layer 100, Material 10, Sort 200 (cmd4)
        Assert.That(result[0].LayerZ, Is.EqualTo(100));
        Assert.That(result[0].MaterialKey, Is.EqualTo(10));
        Assert.That(result[0].SortKey, Is.EqualTo(200));
        Assert.That(result[0].Command, Is.EqualTo(cmd4));
        
        // Layer 100, Material 20, Sort 100 (cmd2)
        Assert.That(result[1].LayerZ, Is.EqualTo(100));
        Assert.That(result[1].MaterialKey, Is.EqualTo(20));
        Assert.That(result[1].SortKey, Is.EqualTo(100));
        Assert.That(result[1].Command, Is.EqualTo(cmd2));
        
        // Layer 200, Material 10, Sort 100 (cmd3)
        Assert.That(result[2].LayerZ, Is.EqualTo(200));
        Assert.That(result[2].MaterialKey, Is.EqualTo(10));
        Assert.That(result[2].SortKey, Is.EqualTo(100));
        Assert.That(result[2].Command, Is.EqualTo(cmd3));
        
        // Layer 200, Material 10, Sort 300 (cmd1)
        Assert.That(result[3].LayerZ, Is.EqualTo(200));
        Assert.That(result[3].MaterialKey, Is.EqualTo(10));
        Assert.That(result[3].SortKey, Is.EqualTo(300));
        Assert.That(result[3].Command, Is.EqualTo(cmd1));
    }

    [Test]
    public void Clear_ShouldRemoveAllCommands()
    {
        var cmd1 = new DrawRectangleCommand(new RectF(0, 0, 10, 10), Color.Red);
        var cmd2 = new DrawRectangleCommand(new RectF(10, 10, 20, 20), Color.Blue);

        _collector.Submit(100, 0, 0, cmd1);
        _collector.Submit(200, 0, 0, cmd2);
        _collector.Clear();

        var result = _collector.FlushSorted();
        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public void FlushSorted_CalledMultipleTimes_ShouldReturnSameSortedOrder()
    {
        var cmd1 = new DrawRectangleCommand(new RectF(0, 0, 10, 10), Color.Red);
        var cmd2 = new DrawRectangleCommand(new RectF(10, 10, 20, 20), Color.Blue);

        _collector.Submit(200, 0, 0, cmd1);
        _collector.Submit(100, 0, 0, cmd2);

        var result1 = _collector.FlushSorted();
        var result2 = _collector.FlushSorted();

        Assert.That(result1.Count, Is.EqualTo(2));
        Assert.That(result2.Count, Is.EqualTo(2));
        Assert.That(result1[0].LayerZ, Is.EqualTo(result2[0].LayerZ));
        Assert.That(result1[1].LayerZ, Is.EqualTo(result2[1].LayerZ));
    }

    [Test]
    public void Submit_AfterFlushSorted_ShouldAccumulateCommands()
    {
        var cmd1 = new DrawRectangleCommand(new RectF(0, 0, 10, 10), Color.Red);
        var cmd2 = new DrawRectangleCommand(new RectF(10, 10, 20, 20), Color.Blue);

        _collector.Submit(100, 0, 0, cmd1);
        var firstResult = _collector.FlushSorted();
        
        _collector.Submit(200, 0, 0, cmd2);
        var secondResult = _collector.FlushSorted();

        Assert.That(firstResult.Count, Is.EqualTo(1));
        Assert.That(secondResult.Count, Is.EqualTo(2));
        Assert.That(secondResult[0].Command, Is.EqualTo(cmd1));
        Assert.That(secondResult[1].Command, Is.EqualTo(cmd2));
    }

    [Test]
    public void FlushSorted_ShouldNotClearCommands()
    {
        var cmd = new DrawRectangleCommand(new RectF(0, 0, 10, 10), Color.Red);

        _collector.Submit(100, 0, 0, cmd);
        var firstFlush = _collector.FlushSorted();
        var secondFlush = _collector.FlushSorted();

        Assert.That(firstFlush.Count, Is.EqualTo(1));
        Assert.That(secondFlush.Count, Is.EqualTo(1));
        Assert.That(firstFlush[0].Command, Is.EqualTo(cmd));
        Assert.That(secondFlush[0].Command, Is.EqualTo(cmd));
    }
}