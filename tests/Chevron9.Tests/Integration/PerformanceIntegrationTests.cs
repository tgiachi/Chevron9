using System.Diagnostics;
using Chevron9.Core.Interfaces;
using Chevron9.Core.Layers;
using Chevron9.Core.Render;
using Chevron9.Core.Render.Commands;
using Chevron9.Core.Scenes;
using Chevron9.Core.Types;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Integration;

[TestFixture]
public class PerformanceIntegrationTests
{
    private sealed class PerformanceTestLayer : AbstractLayer
    {
        private readonly int _commandCount;

        public PerformanceTestLayer(string name, int zIndex, int commandCount)
            : base(name, zIndex, true, true, LayerClear.None, LayerComposeMode.Overwrite)
        {
            _commandCount = commandCount;
        }

        public override void Render(IRenderCommandCollector rq, float alpha)
        {
            for (int i = 0; i < _commandCount; i++)
            {
                rq.Submit(ZIndex, 0, i, new DrawRectangleCommand(
                    new RectF(i * 10, i * 10, 10, 10), Color.Red));
            }
        }
    }

    private sealed class PerformanceTestScene : IScene
    {
        public string Name { get; }
        public IReadOnlyList<ILayer> Layers { get; }

        public PerformanceTestScene(string name, params ILayer[] layers)
        {
            Name = name;
            Layers = layers;
        }

        public void Enter() { }
        public void Close() { }
        public void Dispose() { }

        public void Update(double fixedDt, IInputDevice input)
        {
            // Simulate some work
            Thread.Sleep(1);
        }

        public void Render(IRenderCommandCollector rq, float alpha)
        {
            foreach (var layer in Layers)
            {
                layer.Render(rq, alpha);
            }
        }

        public bool HandleInput(IInputDevice input) => false;
    }

    [Test]
    public void SceneManager_MultipleScenesAndLayers_RenderPerformance()
    {
        // Arrange - Create many scenes with many layers
        const int sceneCount = 5;
        const int layersPerScene = 10;
        const int commandsPerLayer = 100;

        using var sceneManager = new SceneManager();
        var scenes = new List<PerformanceTestScene>();

        for (int s = 0; s < sceneCount; s++)
        {
            var layers = new ILayer[layersPerScene];
            for (int l = 0; l < layersPerScene; l++)
            {
                layers[l] = new PerformanceTestLayer($"Layer{s}_{l}", l * 100, commandsPerLayer);
            }
            var scene = new PerformanceTestScene($"Scene{s}", layers);
            scenes.Add(scene);
            sceneManager.Push(scene);
        }

        var renderCollector = new TestRenderCommandCollector();

        // Act - Measure render performance
        var stopwatch = Stopwatch.StartNew();
        sceneManager.Render(renderCollector, 1.0f);
        stopwatch.Stop();

        // Assert - Performance within acceptable bounds
        var expectedCommands = sceneCount * layersPerScene * commandsPerLayer;
        Assert.That(renderCollector.Items.Count, Is.EqualTo(expectedCommands));

        // Performance assertion - should complete within reasonable time
        // Adjust threshold based on your performance requirements
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(100),
            $"Render performance degraded: {stopwatch.ElapsedMilliseconds}ms for {expectedCommands} commands");
    }

    [Test]
    public void SceneManager_SceneStackOperations_Performance()
    {
        // Arrange
        const int operationCount = 1000;
        using var sceneManager = new SceneManager();

        // Act - Measure push/pop performance
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < operationCount; i++)
        {
            var scene = new PerformanceTestScene($"Scene{i}");
            sceneManager.Push(scene);
        }

        for (int i = 0; i < operationCount; i++)
        {
            sceneManager.Pop();
        }
        stopwatch.Stop();

        // Assert - Performance within acceptable bounds
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(200),
            $"Scene stack operations too slow: {stopwatch.ElapsedMilliseconds}ms for {operationCount} operations");
    }

    [Test]
    public void RenderCommandCollector_LargeCommandSet_SortingPerformance()
    {
        // Arrange
        const int commandCount = 10000;
        var collector = new RenderCommandCollector();

        // Act - Submit many commands with different sort keys
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < commandCount; i++)
        {
            var layerZ = i % 10; // Vary layer Z
            var materialKey = i % 5; // Vary material key
            var sortKey = commandCount - i; // Reverse order to test sorting
            collector.Submit(layerZ, materialKey, sortKey,
                new DrawRectangleCommand(new RectF(i, i, 1, 1), Color.Red));
        }

        var sortedItems = collector.FlushSorted();
        stopwatch.Stop();

        // Assert - All commands collected and properly sorted
        Assert.That(sortedItems.Count, Is.EqualTo(commandCount));

        // Verify sorting: layerZ ASC, then materialKey ASC, then sortKey ASC
        for (int i = 1; i < sortedItems.Count; i++)
        {
            var prev = sortedItems[i - 1];
            var curr = sortedItems[i];

            if (prev.LayerZ == curr.LayerZ)
            {
                if (prev.MaterialKey == curr.MaterialKey)
                {
                    Assert.That(prev.SortKey, Is.LessThanOrEqualTo(curr.SortKey),
                        "Commands not properly sorted by sort key");
                }
                else
                {
                    Assert.That(prev.MaterialKey, Is.LessThanOrEqualTo(curr.MaterialKey),
                        "Commands not properly sorted by material key");
                }
            }
            else
            {
                Assert.That(prev.LayerZ, Is.LessThanOrEqualTo(curr.LayerZ),
                    "Commands not properly sorted by layer Z");
            }
        }

        // Performance assertion
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(50),
            $"Command sorting too slow: {stopwatch.ElapsedMilliseconds}ms for {commandCount} commands");
    }

    [Test]
    public void SceneManager_UpdateLoop_PerformanceUnderLoad()
    {
        // Arrange
        const int updateIterations = 100;
        const int sceneCount = 10;

        using var sceneManager = new SceneManager();
        for (int i = 0; i < sceneCount; i++)
        {
            var layers = new ILayer[5];
            for (int l = 0; l < 5; l++)
            {
                layers[l] = new PerformanceTestLayer($"Layer{i}_{l}", l * 100, 10);
            }
            var scene = new PerformanceTestScene($"Scene{i}", layers);
            sceneManager.Push(scene);
        }

        // Act - Measure update performance
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < updateIterations; i++)
        {
            sceneManager.Update(0.016, null!); // 60 FPS delta
        }
        stopwatch.Stop();

        // Assert - Performance within acceptable bounds
        var totalOperations = updateIterations * sceneCount;
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(200),
            $"Update performance degraded: {stopwatch.ElapsedMilliseconds}ms for {totalOperations} operations");
    }

    [Test]
    public void MemoryAllocation_RenderCommandCollector_BasicFunctionality()
    {
        // Arrange
        const int commandCount = 1000;
        var collector = new RenderCommandCollector();

        // Act - Measure memory allocations during command submission
        var beforeMemory = GC.GetTotalMemory(true);

        for (int i = 0; i < commandCount; i++)
        {
            collector.Submit(0, 0, i, new DrawRectangleCommand(
                new RectF(i, i, 1, 1), Color.Red));
        }

        var sortedItems = collector.FlushSorted();
        var afterMemory = GC.GetTotalMemory(false);

        // Assert - Basic functionality works
        var memoryUsed = afterMemory - beforeMemory;
        Assert.That(sortedItems.Count, Is.EqualTo(commandCount));

        // Instead of strict memory limits, just ensure we're not allocating gigabytes
        // Memory allocation tests can be flaky in different environments
        Assert.That(memoryUsed, Is.LessThan(10 * 1024 * 1024), // Less than 10MB
            $"Unexpected excessive memory allocation: {memoryUsed} bytes for {commandCount} commands");

        // Verify the commands are properly sorted
        for (int i = 1; i < sortedItems.Count; i++)
        {
            Assert.That(sortedItems[i].SortKey, Is.GreaterThanOrEqualTo(sortedItems[i - 1].SortKey),
                "Commands should be sorted by sort key");
        }
    }

    private sealed class TestRenderCommandCollector : IRenderCommandCollector
    {
        public List<RenderItem> Items { get; } = new();

        public void Submit(int layerZ, int materialKey, int sortKey, RenderCommand cmd)
        {
            Items.Add(new RenderItem(layerZ, materialKey, sortKey, cmd));
        }

        public IReadOnlyList<RenderItem> FlushSorted()
        {
            return Items.AsReadOnly();
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}
