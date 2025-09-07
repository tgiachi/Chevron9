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
public class SceneManagerIntegrationTests
{
    private sealed class TestLayer : AbstractLayer
    {
        public int RenderCallCount { get; private set; }
        public int UpdateCallCount { get; private set; }
        public List<RenderCommand> RenderedCommands { get; } = new();

        public TestLayer(string name, int zIndex)
            : base(name, zIndex, true, true, LayerClear.None, LayerCompositeMode.Overwrite)
        {
        }

        public override void Render(IRenderCommandCollector rq, float alpha)
        {
            RenderCallCount++;
            rq.Submit(ZIndex, 0, 0, new DrawRectangleCommand(
                new RectF(0, 0, 100, 100), Color.Red));
        }

        public void Update(double dt, IInputDevice input)
        {
            UpdateCallCount++;
        }
    }

    private sealed class TestScene : IScene
    {
        public string Name { get; }
        public IReadOnlyList<ILayer> Layers { get; }
        public int EnterCallCount { get; private set; }
        public int CloseCallCount { get; private set; }
        public int UpdateCallCount { get; private set; }
        public int RenderCallCount { get; private set; }
        public int HandleInputCallCount { get; private set; }

        public TestScene(string name, params ILayer[] layers)
        {
            Name = name;
            Layers = layers;
        }

        public void Enter() => EnterCallCount++;
        public void Close() => CloseCallCount++;
        public void Dispose() { }

        public void Update(double fixedDt, IInputDevice input)
        {
            UpdateCallCount++;
            foreach (var layer in Layers)
            {
                if (layer is TestLayer testLayer)
                {
                    testLayer.Update(fixedDt, input);
                }
            }
        }

        public void Render(IRenderCommandCollector rq, float alpha)
        {
            RenderCallCount++;
            foreach (var layer in Layers)
            {
                layer.Render(rq, alpha);
            }
        }

        public bool HandleInput(IInputDevice input)
        {
            HandleInputCallCount++;
            return false;
        }
    }

    [Test]
    public void SceneManager_SceneWithLayers_EndToEndLifecycle()
    {
        // Arrange
        using var sceneManager = new SceneManager();
        var layer1 = new TestLayer("Background", 100);
        var layer2 = new TestLayer("Foreground", 200);
        var scene = new TestScene("TestScene", layer1, layer2);

        // Act - Push scene
        sceneManager.Push(scene);

        // Assert - Scene entered
        Assert.That(scene.EnterCallCount, Is.EqualTo(1));
        Assert.That(sceneManager.Current, Is.EqualTo(scene));

        // Act - Update scene
        sceneManager.Update(0.016, null!);

        // Assert - Scene and layers updated
        Assert.That(scene.UpdateCallCount, Is.EqualTo(1));
        Assert.That(layer1.UpdateCallCount, Is.EqualTo(1));
        Assert.That(layer2.UpdateCallCount, Is.EqualTo(1));

        // Act - Render scene
        var renderCollector = new TestRenderCommandCollector();
        sceneManager.Render(renderCollector, 1.0f);

        // Assert - Scene and layers rendered
        Assert.That(scene.RenderCallCount, Is.EqualTo(1));
        Assert.That(layer1.RenderCallCount, Is.EqualTo(1));
        Assert.That(layer2.RenderCallCount, Is.EqualTo(1));
        Assert.That(renderCollector.Items.Count, Is.EqualTo(2)); // One command per layer

        // Act - Handle input
        var result = sceneManager.HandleInput(null!);

        // Assert - Input handled (called once in Update, once explicitly)
        Assert.That(scene.HandleInputCallCount, Is.EqualTo(2));
        Assert.That(result, Is.False);

        // Act - Pop scene
        sceneManager.Pop();

        // Assert - Scene closed
        Assert.That(scene.CloseCallCount, Is.EqualTo(1));
        Assert.That(sceneManager.Current, Is.Null);
    }

    [Test]
    public void SceneManager_MultipleScenesWithLayers_ProperStackManagement()
    {
        // Arrange
        using var sceneManager = new SceneManager();
        var scene1Layer = new TestLayer("Scene1Layer", 100);
        var scene1 = new TestScene("Scene1", scene1Layer);

        var scene2Layer = new TestLayer("Scene2Layer", 200);
        var scene2 = new TestScene("Scene2", scene2Layer);

        // Act - Push first scene
        sceneManager.Push(scene1);

        // Assert - First scene active
        Assert.That(sceneManager.Current, Is.EqualTo(scene1));
        Assert.That(scene1.EnterCallCount, Is.EqualTo(1));

        // Act - Push second scene
        sceneManager.Push(scene2);

        // Assert - Second scene active, first still entered
        Assert.That(sceneManager.Current, Is.EqualTo(scene2));
        Assert.That(scene2.EnterCallCount, Is.EqualTo(1));
        Assert.That(scene1.CloseCallCount, Is.EqualTo(0)); // First scene not closed yet

        // Act - Update (should only update current scene)
        sceneManager.Update(0.016, null!);

        // Assert - Only current scene updated
        Assert.That(scene2.UpdateCallCount, Is.EqualTo(1));
        Assert.That(scene1.UpdateCallCount, Is.EqualTo(0));

        // Act - Render (should render all scenes in stack)
        var renderCollector = new TestRenderCommandCollector();
        sceneManager.Render(renderCollector, 1.0f);

        // Assert - Both scenes rendered (bottom to top)
        Assert.That(scene1.RenderCallCount, Is.EqualTo(1));
        Assert.That(scene2.RenderCallCount, Is.EqualTo(1));
        Assert.That(renderCollector.Items.Count, Is.EqualTo(2));

        // Act - Pop current scene
        sceneManager.Pop();

        // Assert - Back to first scene
        Assert.That(sceneManager.Current, Is.EqualTo(scene1));
        Assert.That(scene2.CloseCallCount, Is.EqualTo(1));
        Assert.That(scene1.CloseCallCount, Is.EqualTo(0));
    }

    [Test]
    public void SceneManager_SceneReplacement_ProperLifecycle()
    {
        // Arrange
        using var sceneManager = new SceneManager();
        var oldLayer = new TestLayer("OldLayer", 100);
        var oldScene = new TestScene("OldScene", oldLayer);

        var newLayer = new TestLayer("NewLayer", 200);
        var newScene = new TestScene("NewScene", newLayer);

        // Act - Push old scene
        sceneManager.Push(oldScene);

        // Assert - Old scene active
        Assert.That(sceneManager.Current, Is.EqualTo(oldScene));
        Assert.That(oldScene.EnterCallCount, Is.EqualTo(1));

        // Act - Replace with new scene
        sceneManager.Replace(newScene);

        // Assert - New scene active, old scene cleaned up
        Assert.That(sceneManager.Current, Is.EqualTo(newScene));
        Assert.That(newScene.EnterCallCount, Is.EqualTo(1));
        Assert.That(oldScene.CloseCallCount, Is.EqualTo(1));

        // Act - Update new scene
        sceneManager.Update(0.016, null!);

        // Assert - Only new scene updated
        Assert.That(newScene.UpdateCallCount, Is.EqualTo(1));
        Assert.That(oldScene.UpdateCallCount, Is.EqualTo(0));
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

        public void SubmitRectangle(int layerZ, RectF bounds, Color color)
        {
            Submit(layerZ, 0, 0, new DrawRectangleCommand(bounds, color));
        }

        public void SubmitText(int layerZ, string text, Position position, Color color, float fontSize = 12.0f)
        {
            Submit(layerZ, 0, 0, new DrawTextCommand(text, position, color, fontSize));
        }
    }
}
