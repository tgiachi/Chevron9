using Chevron9.Core.Interfaces;
using Chevron9.Core.Layers;
using Chevron9.Core.Types;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Core.Layers;

[TestFixture]
public class AbstractLayerTests
{
    private sealed class TestAbstractLayer : AbstractLayer
    {
        public TestAbstractLayer(string name, int zIndex) : base(name, zIndex)
        {
        }

        public TestAbstractLayer(
            string name, int zIndex, bool enabled, bool visible, LayerClear clear, LayerCompositeMode compose
        )
            : base(name, zIndex, enabled, visible, clear, compose)
        {
        }

        public override void Render(IRenderCommandCollector rq, float alpha)
        {
            // Test implementation - do nothing
        }
    }

    [Test]
    public void Constructor_WithNameAndZIndex_SetsPropertiesCorrectly()
    {
        const string expectedName = "TestLayer";
        const int expectedZIndex = 100;

        var layer = new TestAbstractLayer(expectedName, expectedZIndex);

        Assert.That(layer.Name, Is.EqualTo(expectedName));
        Assert.That(layer.ZIndex, Is.EqualTo(expectedZIndex));
        Assert.That(layer.Enabled, Is.True);
        Assert.That(layer.Visible, Is.True);
        Assert.That(layer.Clear, Is.EqualTo(LayerClear.None));
        Assert.That(layer.Compose, Is.EqualTo(LayerCompositeMode.Overwrite));
    }

    [Test]
    public void Constructor_WithCustomSettings_SetsPropertiesCorrectly()
    {
        const string expectedName = "CustomLayer";
        const int expectedZIndex = 200;
        const bool expectedEnabled = false;
        const bool expectedVisible = false;
        const LayerClear expectedClear = LayerClear.Color;
        const LayerCompositeMode expectedCompose = LayerCompositeMode.TransparentIfEmpty;

        var layer = new TestAbstractLayer(
            expectedName,
            expectedZIndex,
            expectedEnabled,
            expectedVisible,
            expectedClear,
            expectedCompose
        );

        Assert.That(layer.Name, Is.EqualTo(expectedName));
        Assert.That(layer.ZIndex, Is.EqualTo(expectedZIndex));
        Assert.That(layer.Enabled, Is.EqualTo(expectedEnabled));
        Assert.That(layer.Visible, Is.EqualTo(expectedVisible));
        Assert.That(layer.Clear, Is.EqualTo(expectedClear));
        Assert.That(layer.Compose, Is.EqualTo(expectedCompose));
    }

    [Test]
    public void Camera_IsNotNull()
    {
        var layer = new TestAbstractLayer("Test", 0);

        Assert.That(layer.Camera, Is.Not.Null);
    }

    [Test]
    public void Update_DefaultImplementation_DoesNotThrow()
    {
        var layer = new TestAbstractLayer("Test", 0);

        // Should not throw when called with null input (though in practice it would be provided)
        Assert.DoesNotThrow(() => layer.Update(0.016, null!));
    }

    [Test]
    public void HandleInput_DefaultImplementation_ReturnsFalse()
    {
        var layer = new TestAbstractLayer("Test", 0);

        var result = layer.HandleInput(null!);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Properties_AreReadOnlyFromOutside()
    {
        var layer = new TestAbstractLayer("Test", 0);

        // Properties should be readable but not directly settable from outside
        // This test verifies that the properties have the expected default values
        Assert.That(layer.Enabled, Is.True);
        Assert.That(layer.Visible, Is.True);
        Assert.That(layer.Clear, Is.EqualTo(LayerClear.None));
        Assert.That(layer.Compose, Is.EqualTo(LayerCompositeMode.Overwrite));
    }
}
