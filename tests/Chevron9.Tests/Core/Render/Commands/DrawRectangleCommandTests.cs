using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Core.Render.Commands;

[TestFixture]
public class DrawRectangleCommandTests
{
    [Test]
    public void Constructor_ShouldInitializeAllProperties()
    {
        var rectangle = new RectF(10, 20, 100, 200);
        var color = Color.Magenta;

        var command = new DrawRectangleCommand(rectangle, color);

        Assert.That(command.Bounds, Is.EqualTo(rectangle));
        Assert.That(command.Color, Is.EqualTo(color));
    }

    [Test]
    public void Constructor_WithZeroSizeRectangle_ShouldAllowZeroSize()
    {
        var rectangle = new RectF(50, 75, 0, 0);
        var color = Color.Cyan;

        var command = new DrawRectangleCommand(rectangle, color);

        Assert.That(command.Bounds.Width, Is.EqualTo(0));
        Assert.That(command.Bounds.Height, Is.EqualTo(0));
    }

    [Test]
    public void Constructor_WithNegativeSizeRectangle_ShouldAllowNegativeSize()
    {
        var rectangle = new RectF(10, 20, -5, -10);
        var color = Color.Gray;

        var command = new DrawRectangleCommand(rectangle, color);

        Assert.That(command.Bounds.Width, Is.EqualTo(-5));
        Assert.That(command.Bounds.Height, Is.EqualTo(-10));
    }

    [Test]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        var rectangle = new RectF(10, 20, 30, 40);
        var color = Color.Brown;

        var command1 = new DrawRectangleCommand(rectangle, color);
        var command2 = new DrawRectangleCommand(rectangle, color);

        Assert.That(command1, Is.EqualTo(command2));
        Assert.That(command1.GetHashCode(), Is.EqualTo(command2.GetHashCode()));
    }

    [Test]
    public void Equality_WithDifferentRectangle_ShouldNotBeEqual()
    {
        var color = Color.Red;
        var command1 = new DrawRectangleCommand(new RectF(10, 20, 30, 40), color);
        var command2 = new DrawRectangleCommand(new RectF(11, 20, 30, 40), color);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentColor_ShouldNotBeEqual()
    {
        var rectangle = new RectF(10, 20, 30, 40);
        var command1 = new DrawRectangleCommand(rectangle, Color.Red);
        var command2 = new DrawRectangleCommand(rectangle, Color.Blue);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void ToString_ShouldIncludeMainProperties()
    {
        var command = new DrawRectangleCommand(new RectF(5, 10, 15, 25), Color.Pink);

        var result = command.ToString();

        Assert.That(result, Does.Contain("DrawRectangleCommand"));
        Assert.That(result, Does.Contain("5"));
        Assert.That(result, Does.Contain("10"));
        Assert.That(result, Does.Contain("15"));
        Assert.That(result, Does.Contain("25"));
    }

    [Test]
    public void RecordBehavior_ShouldSupportWithExpressions()
    {
        var original = new DrawRectangleCommand(new RectF(5, 10, 15, 20), Color.Red);

        var modified = original with { Color = Color.Green };

        Assert.That(modified.Bounds, Is.EqualTo(original.Bounds));
        Assert.That(modified.Color, Is.EqualTo(Color.Green));
    }

    [Test]
    public void RecordBehavior_WithRectangleChange_ShouldUpdateRectangle()
    {
        var original = new DrawRectangleCommand(new RectF(5, 10, 15, 20), Color.Red);
        var newRectangle = new RectF(25, 30, 35, 40);

        var modified = original with { Bounds = newRectangle };

        Assert.That(modified.Bounds, Is.EqualTo(newRectangle));
        Assert.That(modified.Color, Is.EqualTo(original.Color));
    }

    [Test]
    public void RecordBehavior_WithMultipleChanges_ShouldUpdateAllSpecifiedProperties()
    {
        var original = new DrawRectangleCommand(new RectF(5, 10, 15, 20), Color.Red);
        var newRectangle = new RectF(25, 30, 35, 40);

        var modified = original with { Bounds = newRectangle, Color = Color.Blue };

        Assert.That(modified.Bounds, Is.EqualTo(newRectangle));
        Assert.That(modified.Color, Is.EqualTo(Color.Blue));
    }

    [Test]
    public void Constructor_WithLargeRectangle_ShouldHandleLargeValues()
    {
        var rectangle = new RectF(1000000, 2000000, 5000000, 10000000);
        var color = Color.White;

        var command = new DrawRectangleCommand(rectangle, color);

        Assert.That(command.Bounds.X, Is.EqualTo(1000000));
        Assert.That(command.Bounds.Y, Is.EqualTo(2000000));
        Assert.That(command.Bounds.Width, Is.EqualTo(5000000));
        Assert.That(command.Bounds.Height, Is.EqualTo(10000000));
    }

    [Test]
    public void Constructor_WithFloatingPointPrecision_ShouldMaintainPrecision()
    {
        var rectangle = new RectF(10.5f, 20.25f, 30.125f, 40.0625f);
        var color = Color.Black;

        var command = new DrawRectangleCommand(rectangle, color);

        Assert.That(command.Bounds.X, Is.EqualTo(10.5f));
        Assert.That(command.Bounds.Y, Is.EqualTo(20.25f));
        Assert.That(command.Bounds.Width, Is.EqualTo(30.125f));
        Assert.That(command.Bounds.Height, Is.EqualTo(40.0625f));
    }
}