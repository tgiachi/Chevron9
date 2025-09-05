using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Core.Render.Commands;

[TestFixture]
public class DrawRectangleOutlineCommandTests
{
    [Test]
    public void Constructor_ShouldInitializeAllProperties()
    {
        var rectangle = new RectF(15, 25, 120, 180);
        var thickness = 3.0f;
        var color = Color.Teal;

        var command = new DrawRectangleOutlineCommand(rectangle, color, thickness);

        Assert.That(command.Bounds, Is.EqualTo(rectangle));
        Assert.That(command.Thickness, Is.EqualTo(thickness));
        Assert.That(command.Color, Is.EqualTo(color));
    }

    [Test]
    public void Constructor_WithZeroThickness_ShouldAllowZeroThickness()
    {
        var rectangle = new RectF(10, 20, 30, 40);
        var thickness = 0.0f;
        var color = Color.LightGray;

        var command = new DrawRectangleOutlineCommand(rectangle, color, thickness);

        Assert.That(command.Thickness, Is.EqualTo(0.0f));
    }

    [Test]
    public void Constructor_WithNegativeThickness_ShouldAllowNegativeThickness()
    {
        var rectangle = new RectF(5, 10, 50, 60);
        var thickness = -2.5f;
        var color = Color.Gold;

        var command = new DrawRectangleOutlineCommand(rectangle, color, thickness);

        Assert.That(command.Thickness, Is.EqualTo(-2.5f));
    }

    [Test]
    public void Constructor_WithZeroSizeRectangle_ShouldAllowZeroSize()
    {
        var rectangle = new RectF(50, 75, 0, 0);
        var thickness = 1.0f;
        var color = Color.Navy;

        var command = new DrawRectangleOutlineCommand(rectangle, color, thickness);

        Assert.That(command.Bounds.Width, Is.EqualTo(0));
        Assert.That(command.Bounds.Height, Is.EqualTo(0));
    }

    [Test]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        var rectangle = new RectF(10, 20, 30, 40);
        var thickness = 2.5f;
        var color = Color.LimeGreen;

        var command1 = new DrawRectangleOutlineCommand(rectangle, color, thickness);
        var command2 = new DrawRectangleOutlineCommand(rectangle, color, thickness);

        Assert.That(command1, Is.EqualTo(command2));
        Assert.That(command1.GetHashCode(), Is.EqualTo(command2.GetHashCode()));
    }

    [Test]
    public void Equality_WithDifferentRectangle_ShouldNotBeEqual()
    {
        var thickness = 2.0f;
        var color = Color.Red;

        var command1 = new DrawRectangleOutlineCommand(new RectF(10, 20, 30, 40), Color.Red, thickness);
        var command2 = new DrawRectangleOutlineCommand(new RectF(11, 20, 30, 40), Color.Red, thickness);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentThickness_ShouldNotBeEqual()
    {
        var rectangle = new RectF(10, 20, 30, 40);
        var color = Color.Red;

        var command1 = new DrawRectangleOutlineCommand(rectangle, color, 2.0f);
        var command2 = new DrawRectangleOutlineCommand(rectangle, color, 3.0f);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentColor_ShouldNotBeEqual()
    {
        var rectangle = new RectF(10, 20, 30, 40);
        var thickness = 2.0f;

        var command1 = new DrawRectangleOutlineCommand(rectangle, Color.Red, thickness);
        var command2 = new DrawRectangleOutlineCommand(rectangle, Color.Blue, thickness);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void ToString_ShouldIncludeMainProperties()
    {
        var command = new DrawRectangleOutlineCommand(new RectF(5, 10, 15, 25), Color.DarkViolet, 1.5f);

        var result = command.ToString();

        Assert.That(result, Does.Contain("DrawRectangleOutlineCommand"));
        Assert.That(result, Does.Contain("5"));
        Assert.That(result, Does.Contain("10"));
        Assert.That(result, Does.Contain("15"));
        Assert.That(result, Does.Contain("25"));
        Assert.That(result, Does.Contain("1.5"));
    }

    [Test]
    public void RecordBehavior_ShouldSupportWithExpressions()
    {
        var original = new DrawRectangleOutlineCommand(new RectF(5, 10, 15, 20), Color.Red, 2.0f);

        var modified = original with { Thickness = 4.0f };

        Assert.That(modified.Bounds, Is.EqualTo(original.Bounds));
        Assert.That(modified.Thickness, Is.EqualTo(4.0f));
        Assert.That(modified.Color, Is.EqualTo(original.Color));
    }

    [Test]
    public void RecordBehavior_WithMultipleChanges_ShouldUpdateAllSpecifiedProperties()
    {
        var original = new DrawRectangleOutlineCommand(new RectF(5, 10, 15, 20), Color.Red, 2.0f);
        var newRectangle = new RectF(25, 30, 35, 40);

        var modified = original with { Bounds = newRectangle, Color = Color.Green };

        Assert.That(modified.Bounds, Is.EqualTo(newRectangle));
        Assert.That(modified.Thickness, Is.EqualTo(original.Thickness));
        Assert.That(modified.Color, Is.EqualTo(Color.Green));
    }

    [Test]
    public void Constructor_WithFloatingPointPrecision_ShouldMaintainPrecision()
    {
        var rectangle = new RectF(10.5f, 20.25f, 30.125f, 40.0625f);
        var thickness = 1.75f;
        var color = Color.Maroon;

        var command = new DrawRectangleOutlineCommand(rectangle, color, thickness);

        Assert.That(command.Bounds.X, Is.EqualTo(10.5f));
        Assert.That(command.Bounds.Y, Is.EqualTo(20.25f));
        Assert.That(command.Bounds.Width, Is.EqualTo(30.125f));
        Assert.That(command.Bounds.Height, Is.EqualTo(40.0625f));
        Assert.That(command.Thickness, Is.EqualTo(1.75f));
    }

    [Test]
    public void Constructor_WithLargeRectangle_ShouldHandleLargeValues()
    {
        var rectangle = new RectF(1000000, 2000000, 5000000, 10000000);
        var thickness = 100.0f;
        var color = Color.DarkBlue;

        var command = new DrawRectangleOutlineCommand(rectangle, color, thickness);

        Assert.That(command.Bounds.X, Is.EqualTo(1000000));
        Assert.That(command.Bounds.Y, Is.EqualTo(2000000));
        Assert.That(command.Bounds.Width, Is.EqualTo(5000000));
        Assert.That(command.Bounds.Height, Is.EqualTo(10000000));
        Assert.That(command.Thickness, Is.EqualTo(100.0f));
    }
}
