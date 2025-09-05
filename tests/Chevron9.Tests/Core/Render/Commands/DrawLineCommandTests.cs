using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Core.Render.Commands;

[TestFixture]
public class DrawLineCommandTests
{
    [Test]
    public void Constructor_ShouldInitializeAllProperties()
    {
        var start = new Position(10, 20);
        var end = new Position(100, 200);
        var thickness = 3.5f;
        var color = Color.Blue;

        var command = new DrawLineCommand(start, end, color, thickness);

        Assert.That(command.Start, Is.EqualTo(start));
        Assert.That(command.End, Is.EqualTo(end));
        Assert.That(command.Thickness, Is.EqualTo(thickness));
        Assert.That(command.Color, Is.EqualTo(color));
    }

    [Test]
    public void Constructor_WithSameStartAndEnd_ShouldAllowSamePosition()
    {
        var position = new Position(50, 75);
        var thickness = 1.0f;
        var color = Color.Red;

        var command = new DrawLineCommand(position, position, color, thickness);

        Assert.That(command.Start, Is.EqualTo(position));
        Assert.That(command.End, Is.EqualTo(position));
    }

    [Test]
    public void Constructor_WithZeroThickness_ShouldAllowZeroThickness()
    {
        var start = new Position(0, 0);
        var end = new Position(10, 10);
        var thickness = 0.0f;
        var color = Color.Green;

        var command = new DrawLineCommand(start, end, color, thickness);

        Assert.That(command.Thickness, Is.EqualTo(0.0f));
    }

    [Test]
    public void Constructor_WithNegativeThickness_ShouldAllowNegativeThickness()
    {
        var start = new Position(5, 10);
        var end = new Position(15, 20);
        var thickness = -1.5f;
        var color = Color.Yellow;

        var command = new DrawLineCommand(start, end, color, thickness);

        Assert.That(command.Thickness, Is.EqualTo(-1.5f));
    }

    [Test]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        var start = new Position(10, 20);
        var end = new Position(30, 40);
        var thickness = 2.0f;
        var color = Color.Purple;

        var command1 = new DrawLineCommand(start, end, color, thickness);
        var command2 = new DrawLineCommand(start, end, color, thickness);

        Assert.That(command1, Is.EqualTo(command2));
        Assert.That(command1.GetHashCode(), Is.EqualTo(command2.GetHashCode()));
    }

    [Test]
    public void Equality_WithDifferentStart_ShouldNotBeEqual()
    {
        var end = new Position(30, 40);
        var thickness = 2.0f;
        var color = Color.Red;

        var command1 = new DrawLineCommand(new Position(10, 20), end, color, thickness);
        var command2 = new DrawLineCommand(new Position(11, 20), end, color, thickness);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentEnd_ShouldNotBeEqual()
    {
        var start = new Position(10, 20);
        var thickness = 2.0f;
        var color = Color.Red;

        var command1 = new DrawLineCommand(start, new Position(30, 40), color, thickness);
        var command2 = new DrawLineCommand(start, new Position(31, 40), color, thickness);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentThickness_ShouldNotBeEqual()
    {
        var start = new Position(10, 20);
        var end = new Position(30, 40);
        var color = Color.Red;

        var command1 = new DrawLineCommand(start, end, color, 2.0f);
        var command2 = new DrawLineCommand(start, end, color, 3.0f);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentColor_ShouldNotBeEqual()
    {
        var start = new Position(10, 20);
        var end = new Position(30, 40);
        var thickness = 2.0f;

        var command1 = new DrawLineCommand(start, end, Color.Red, thickness);
        var command2 = new DrawLineCommand(start, end, Color.Blue, thickness);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void ToString_ShouldIncludeMainProperties()
    {
        var command = new DrawLineCommand(new Position(5, 10), new Position(15, 25), Color.Orange, 1.5f);

        var result = command.ToString();

        Assert.That(result, Does.Contain("DrawLineCommand"));
        Assert.That(result, Does.Contain("5"));
        Assert.That(result, Does.Contain("10"));
        Assert.That(result, Does.Contain("15"));
        Assert.That(result, Does.Contain("25"));
        Assert.That(result, Does.Contain("1.5"));
    }

    [Test]
    public void RecordBehavior_ShouldSupportWithExpressions()
    {
        var original = new DrawLineCommand(new Position(5, 10), new Position(15, 20), Color.Red, 2.0f);

        var modified = original with { Thickness = 4.0f };

        Assert.That(modified.Start, Is.EqualTo(original.Start));
        Assert.That(modified.End, Is.EqualTo(original.End));
        Assert.That(modified.Thickness, Is.EqualTo(4.0f));
        Assert.That(modified.Color, Is.EqualTo(original.Color));
    }

    [Test]
    public void RecordBehavior_WithMultipleChanges_ShouldUpdateAllSpecifiedProperties()
    {
        var original = new DrawLineCommand(new Position(5, 10), new Position(15, 20), Color.Red, 2.0f);

        var modified = original with { End = new Position(25, 30), Color = Color.Blue };

        Assert.That(modified.Start, Is.EqualTo(original.Start));
        Assert.That(modified.End, Is.EqualTo(new Position(25, 30)));
        Assert.That(modified.Thickness, Is.EqualTo(original.Thickness));
        Assert.That(modified.Color, Is.EqualTo(Color.Blue));
    }
}
