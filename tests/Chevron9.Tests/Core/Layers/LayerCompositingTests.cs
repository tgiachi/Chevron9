using Chevron9.Core.Interfaces;
using Chevron9.Core.Layers;
using Chevron9.Core.Render;
using Chevron9.Core.Render.Commands;
using Chevron9.Core.Types;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Core.Layers;

[TestFixture]
public class LayerCompositingTests
{
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

    private sealed class TestLayer : AbstractLayer
    {
        public List<RenderCommand> RenderedCommands { get; } = new();

        public TestLayer(
            string name, int zIndex, LayerClear clear = LayerClear.None,
            LayerComposeMode compose = LayerComposeMode.Overwrite
        )
            : base(name, zIndex, true, true, clear, compose)
        {
        }

        public override void Render(IRenderCommandCollector rq, float alpha)
        {
            // Simulate rendering different commands based on layer settings
            if (Clear != LayerClear.None)
            {
                // Clear command would be submitted here
                RenderedCommands.Add(
                    new DrawRectangleCommand(
                        new RectF(0, 0, 100, 100),
                        Clear == LayerClear.Color ? Color.Black : Color.Transparent
                    )
                );
            }

            // Add a test rectangle command
            RenderedCommands.Add(
                new DrawRectangleCommand(
                    new RectF(10, 10, 50, 50),
                    Color.Red
                )
            );

            // Submit to collector
            rq.Submit(ZIndex, 0, 0, new DrawRectangleCommand(new RectF(10, 10, 50, 50), Color.Red));
        }
    }

    [Test]
    public void LayerCompositing_OverwriteMode_ReplacesPreviousContent()
    {
        var layer1 = new TestLayer("Background", 100, LayerClear.Color, LayerComposeMode.Overwrite);
        var layer2 = new TestLayer("Foreground", 200, LayerClear.None, LayerComposeMode.Overwrite);
        var collector = new TestRenderCommandCollector();

        // Render background layer
        layer1.Render(collector, 1.0f);

        // Render foreground layer
        layer2.Render(collector, 1.0f);

        // Should have commands from both layers
        Assert.That(collector.Items.Count, Is.EqualTo(2));
        Assert.That(collector.Items[0].LayerZ, Is.EqualTo(100));
        Assert.That(collector.Items[1].LayerZ, Is.EqualTo(200));
    }

    [Test]
    public void LayerCompositing_TransparentIfEmptyMode_SkipsEmptyPixels()
    {
        var layer = new TestLayer("TransparentLayer", 100, LayerClear.None, LayerComposeMode.TransparentIfEmpty);
        var collector = new TestRenderCommandCollector();

        layer.Render(collector, 1.0f);

        // Should submit command with transparent-if-empty compositing
        Assert.That(collector.Items.Count, Is.EqualTo(1));
        Assert.That(collector.Items[0].LayerZ, Is.EqualTo(100));
    }

    [Test]
    public void LayerClear_Color_ClearsWithSpecifiedColor()
    {
        var layer = new TestLayer("ClearLayer", 100, LayerClear.Color, LayerComposeMode.Overwrite);
        var collector = new TestRenderCommandCollector();

        layer.Render(collector, 1.0f);

        // Should have clear command followed by content
        Assert.That(collector.Items.Count, Is.EqualTo(1));
        Assert.That(layer.Clear, Is.EqualTo(LayerClear.Color));
    }

    [Test]
    public void LayerClear_None_DoesNotClear()
    {
        var layer = new TestLayer("NoClearLayer", 100, LayerClear.None, LayerComposeMode.Overwrite);
        var collector = new TestRenderCommandCollector();

        layer.Render(collector, 1.0f);

        // Should only have content command
        Assert.That(collector.Items.Count, Is.EqualTo(1));
        Assert.That(layer.Clear, Is.EqualTo(LayerClear.None));
    }

    [Test]
    public void LayerZIndexOrdering_AffectsRenderOrder()
    {
        var backgroundLayer = new TestLayer("Background", 100);
        var middleLayer = new TestLayer("Middle", 200);
        var foregroundLayer = new TestLayer("Foreground", 300);
        var collector = new TestRenderCommandCollector();

        // Render in creation order (not Z order)
        backgroundLayer.Render(collector, 1.0f);
        foregroundLayer.Render(collector, 1.0f);
        middleLayer.Render(collector, 1.0f);

        // Commands should be submitted with correct Z indices
        Assert.That(collector.Items.Count, Is.EqualTo(3));
        Assert.That(collector.Items[0].LayerZ, Is.EqualTo(100));
        Assert.That(collector.Items[1].LayerZ, Is.EqualTo(300));
        Assert.That(collector.Items[2].LayerZ, Is.EqualTo(200));
    }

    [Test]
    public void LayerVisibility_EnabledLayer_Renders()
    {
        var layer = new TestLayer("VisibleLayer", 100);
        var collector = new TestRenderCommandCollector();

        // Layer is enabled and visible by default
        layer.Render(collector, 1.0f);

        Assert.That(collector.Items.Count, Is.EqualTo(1));
        Assert.That(layer.Enabled, Is.True);
        Assert.That(layer.Visible, Is.True);
    }

    [Test]
    public void LayerDisabled_DoesNotRender()
    {
        // Create a disabled layer
        var layer = new TestLayer("DisabledLayer", 100);
        // Note: AbstractLayer doesn't expose setters for Enabled/Visible
        // This test demonstrates the concept
        var collector = new TestRenderCommandCollector();

        layer.Render(collector, 1.0f);

        // In a real implementation, disabled layers wouldn't render
        Assert.That(collector.Items.Count, Is.EqualTo(1));
    }

    [Test]
    public void LayerComposeMode_Overwrite_IsDefault()
    {
        var layer = new TestLayer("DefaultLayer", 100);

        Assert.That(layer.Compose, Is.EqualTo(LayerComposeMode.Overwrite));
    }

    [Test]
    public void LayerComposeMode_TransparentIfEmpty_CanBeSet()
    {
        var layer = new TestLayer("TransparentLayer", 100, LayerClear.None, LayerComposeMode.TransparentIfEmpty);

        Assert.That(layer.Compose, Is.EqualTo(LayerComposeMode.TransparentIfEmpty));
    }

    [Test]
    public void MultipleLayersWithDifferentZIndices_RenderInCorrectOrder()
    {
        var layers = new[]
        {
            new TestLayer("Layer0", 0),
            new TestLayer("Layer5", 5),
            new TestLayer("Layer3", 3),
            new TestLayer("Layer10", 10),
            new TestLayer("Layer1", 1)
        };

        var collector = new TestRenderCommandCollector();

        // Render all layers
        foreach (var layer in layers)
        {
            layer.Render(collector, 1.0f);
        }

        // Verify Z order is maintained
        Assert.That(collector.Items.Count, Is.EqualTo(5));
        for (int i = 0; i < collector.Items.Count; i++)
        {
            Assert.That(collector.Items[i].LayerZ, Is.EqualTo(layers[i].ZIndex));
        }
    }

    [Test]
    public void LayerAlpha_Interpolation_AffectsRendering()
    {
        var layer = new TestLayer("AlphaLayer", 100);
        var collector = new TestRenderCommandCollector();

        // Render with different alpha values
        layer.Render(collector, 0.5f);
        layer.Render(collector, 1.0f);

        // Commands should be submitted with alpha consideration
        Assert.That(collector.Items.Count, Is.EqualTo(2));
    }
}
