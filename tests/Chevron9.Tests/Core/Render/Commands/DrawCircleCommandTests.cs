using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Core.Render.Commands;

[TestFixture]
public class DrawCircleCommandTests
{
    [Test]
    public void Constructor_ShouldInitializeAllProperties()
    {
        var center = new Position(50, 100);
        var radius = 25.5f;
        var color = Color.Red;

        var command = new DrawCircleCommand(center, radius, color);

        Assert.That(command.Center, Is.EqualTo(center));
        Assert.That(command.Radius, Is.EqualTo(radius));
        Assert.That(command.Color, Is.EqualTo(color));
    }

    [Test]
    public void Constructor_WithZeroRadius_ShouldAllowZeroRadius()
    {
        var center = new Position(10, 20);
        var radius = 0.0f;
        var color = Color.Blue;

        var command = new DrawCircleCommand(center, radius, color);

        Assert.That(command.Center, Is.EqualTo(center));
        Assert.That(command.Radius, Is.EqualTo(0.0f));
        Assert.That(command.Color, Is.EqualTo(color));
    }

    [Test]
    public void Constructor_WithNegativeRadius_ShouldAllowNegativeRadius()
    {
        var center = new Position(0, 0);
        var radius = -5.0f;
        var color = Color.Green;

        var command = new DrawCircleCommand(center, radius, color);

        Assert.That(command.Radius, Is.EqualTo(-5.0f));
    }

    [Test]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        var center = new Position(10, 20);
        var radius = 15.0f;
        var color = Color.Yellow;

        var command1 = new DrawCircleCommand(center, radius, color);
        var command2 = new DrawCircleCommand(center, radius, color);

        Assert.That(command1, Is.EqualTo(command2));
        Assert.That(command1.GetHashCode(), Is.EqualTo(command2.GetHashCode()));
    }

    [Test]
    public void Equality_WithDifferentCenter_ShouldNotBeEqual()
    {
        var command1 = new DrawCircleCommand(new Position(10, 20), 15.0f, Color.Red);
        var command2 = new DrawCircleCommand(new Position(10, 21), 15.0f, Color.Red);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentRadius_ShouldNotBeEqual()
    {
        var center = new Position(10, 20);
        var command1 = new DrawCircleCommand(center, 15.0f, Color.Red);
        var command2 = new DrawCircleCommand(center, 16.0f, Color.Red);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentColor_ShouldNotBeEqual()
    {
        var center = new Position(10, 20);
        var radius = 15.0f;
        var command1 = new DrawCircleCommand(center, radius, Color.Red);
        var command2 = new DrawCircleCommand(center, radius, Color.Blue);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void ToString_ShouldIncludeMainProperties()
    {
        var command = new DrawCircleCommand(new Position(5, 10), 7.5f, Color.Purple);

        var result = command.ToString();

        Assert.That(result, Does.Contain("DrawCircleCommand"));
        Assert.That(result, Does.Contain("5"));
        Assert.That(result, Does.Contain("10"));
        Assert.That(result, Does.Contain("7.5"));
    }

    [Test]
    public void RecordBehavior_ShouldSupportWithExpressions()
    {
        var original = new DrawCircleCommand(new Position(5, 10), 15.0f, Color.Red);

        var modified = original with { Radius = 20.0f };

        Assert.That(modified.Center, Is.EqualTo(original.Center));
        Assert.That(modified.Radius, Is.EqualTo(20.0f));
        Assert.That(modified.Color, Is.EqualTo(original.Color));
    }

    [Test]
    public void RecordBehavior_WithMultipleChanges_ShouldUpdateAllSpecifiedProperties()
    {
        var original = new DrawCircleCommand(new Position(5, 10), 15.0f, Color.Red);

        var modified = original with { Center = new Position(20, 30), Color = Color.Blue };

        Assert.That(modified.Center, Is.EqualTo(new Position(20, 30)));
        Assert.That(modified.Radius, Is.EqualTo(original.Radius));
        Assert.That(modified.Color, Is.EqualTo(Color.Blue));
    }
}
