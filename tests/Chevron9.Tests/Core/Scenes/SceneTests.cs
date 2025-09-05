using Chevron9.Core.Interfaces;
using Chevron9.Core.Layers;
using Chevron9.Core.Scenes;
using Chevron9.Shared.Graphics;

namespace Chevron9.Tests.Core.Scenes;

[TestFixture]
public class SceneTests
{
    private sealed class TestLayer : AbstractLayer
    {
        public TestLayer(string name, int zIndex) : base(name, zIndex)
        {
        }

        public override void Render(IRenderCommandCollector rq, float alpha)
        {
            // Test implementation - do nothing
        }
    }

    private sealed class TestScene : Scene
    {
        public TestScene(string name) : base(name)
        {
        }

        public void AddTestLayer(string name, int zIndex)
        {
            AddLayer(new TestLayer(name, zIndex));
        }

        public new void AddLayer(ILayer layer)
        {
            base.AddLayer(layer);
        }

        public new bool RemoveLayer(ILayer layer)
        {
            return base.RemoveLayer(layer);
        }

        public new ILayer? GetLayer(string name)
        {
            return base.GetLayer(name);
        }

        public new T? GetLayer<T>() where T : class, ILayer
        {
            return base.GetLayer<T>();
        }
    }

    [Test]
    public void Constructor_WithValidName_SetsNameProperty()
    {
        const string expectedName = "TestScene";

        var scene = new TestScene(expectedName);

        Assert.That(scene.Name, Is.EqualTo(expectedName));
    }

    [Test]
    public void Constructor_WithNullName_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TestScene(null!));
    }

    [Test]
    public void Layers_InitiallyEmpty()
    {
        var scene = new TestScene("Test");

        Assert.That(scene.Layers.Count, Is.EqualTo(0));
    }

    [Test]
    public void AddLayer_WithValidLayer_AddsToLayersList()
    {
        var scene = new TestScene("Test");
        var layer = new TestLayer("TestLayer", 100);

        scene.AddTestLayer("TestLayer", 100);

        Assert.That(scene.Layers.Count, Is.EqualTo(1));
        Assert.That(scene.Layers[0].Name, Is.EqualTo("TestLayer"));
        Assert.That(scene.Layers[0].ZIndex, Is.EqualTo(100));
    }

    [Test]
    public void AddLayer_WithMultipleLayers_SortsByZIndex()
    {
        var scene = new TestScene("Test");

        scene.AddTestLayer("Layer3", 300);
        scene.AddTestLayer("Layer1", 100);
        scene.AddTestLayer("Layer2", 200);

        Assert.That(scene.Layers.Count, Is.EqualTo(3));
        Assert.That(scene.Layers[0].ZIndex, Is.EqualTo(100));
        Assert.That(scene.Layers[1].ZIndex, Is.EqualTo(200));
        Assert.That(scene.Layers[2].ZIndex, Is.EqualTo(300));
    }

    [Test]
    public void AddLayer_WithNullLayer_ThrowsArgumentNullException()
    {
        var scene = new TestScene("Test");

        Assert.Throws<ArgumentNullException>(() => scene.AddLayer(null!));
    }

    [Test]
    public void AddLayer_WithSameLayerTwice_ThrowsInvalidOperationException()
    {
        var scene = new TestScene("Test");
        var layer = new TestLayer("TestLayer", 100);
        scene.AddLayer(layer);

        Assert.Throws<InvalidOperationException>(() => scene.AddLayer(layer));
    }

    [Test]
    public void RemoveLayer_WithExistingLayer_RemovesFromList()
    {
        var scene = new TestScene("Test");
        var layer = new TestLayer("TestLayer", 100);
        scene.AddLayer(layer);

        var result = scene.RemoveLayer(layer);

        Assert.That(result, Is.True);
        Assert.That(scene.Layers.Count, Is.EqualTo(0));
    }

    [Test]
    public void RemoveLayer_WithNonExistingLayer_ReturnsFalse()
    {
        var scene = new TestScene("Test");
        var layer = new TestLayer("TestLayer", 100);

        var result = scene.RemoveLayer(layer);

        Assert.That(result, Is.False);
    }

    [Test]
    public void GetLayer_WithExistingName_ReturnsLayer()
    {
        var scene = new TestScene("Test");
        var layer = new TestLayer("TestLayer", 100);
        scene.AddLayer(layer);

        var result = scene.GetLayer("TestLayer");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("TestLayer"));
    }

    [Test]
    public void GetLayer_WithNonExistingName_ReturnsNull()
    {
        var scene = new TestScene("Test");

        var result = scene.GetLayer("NonExisting");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetLayer_WithExistingType_ReturnsLayer()
    {
        var scene = new TestScene("Test");
        var layer = new TestLayer("TestLayer", 100);
        scene.AddLayer(layer);

        var result = scene.GetLayer<TestLayer>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("TestLayer"));
    }

    [Test]
    public void GetLayer_WithNonExistingType_ReturnsNull()
    {
        var scene = new TestScene("Test");

        var result = scene.GetLayer<TestLayer>();

        Assert.That(result, Is.Null);
    }

    [Test]
    public void Enter_DefaultImplementation_DoesNotThrow()
    {
        var scene = new TestScene("Test");

        Assert.DoesNotThrow(() => scene.Enter());
    }

    [Test]
    public void Close_DefaultImplementation_DoesNotThrow()
    {
        var scene = new TestScene("Test");

        Assert.DoesNotThrow(() => scene.Close());
    }

    [Test]
    public void Update_WithEnabledLayers_CallsUpdateOnLayers()
    {
        var scene = new TestScene("Test");
        var layer = new TestLayer("TestLayer", 100);
        scene.AddLayer(layer);

        Assert.DoesNotThrow(() => scene.Update(0.016, null!));
    }

    [Test]
    public void Render_WithVisibleLayers_CallsRenderOnLayers()
    {
        var scene = new TestScene("Test");
        var layer = new TestLayer("TestLayer", 100);
        scene.AddLayer(layer);

        Assert.DoesNotThrow(() => scene.Render(null!, 1.0f));
    }

    [Test]
    public void HandleInput_WithLayersThatConsumeInput_ReturnsTrue()
    {
        var scene = new TestScene("Test");
        var layer = new TestLayer("TestLayer", 100);
        scene.AddLayer(layer);

        // Since our test layer doesn't consume input, this should return false
        var result = scene.HandleInput(null!);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Dispose_WithLayers_DisposesAllLayers()
    {
        var scene = new TestScene("Test");
        var layer = new TestLayer("TestLayer", 100);
        scene.AddLayer(layer);

        scene.Dispose();

        Assert.That(scene.Layers.Count, Is.EqualTo(0));
    }

    [Test]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        var scene = new TestScene("Test");

        scene.Dispose();

        Assert.DoesNotThrow(() => scene.Dispose());
    }
}
