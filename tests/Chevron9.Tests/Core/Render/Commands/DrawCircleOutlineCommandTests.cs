using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Core.Render.Commands;

[TestFixture]
public class DrawCircleOutlineCommandTests
{
    [Test]
    public void Constructor_ShouldInitializeAllProperties()
    {
        var center = new Position(75, 125);
        var radius = 30.5f;
        var thickness = 2.5f;
        var color = Color.Green;

        var command = new DrawCircleOutlineCommand(center, radius, color, thickness);

        Assert.That(command.Center, Is.EqualTo(center));
        Assert.That(command.Radius, Is.EqualTo(radius));
        Assert.That(command.Thickness, Is.EqualTo(thickness));
        Assert.That(command.Color, Is.EqualTo(color));
    }

    [Test]
    public void Constructor_WithZeroThickness_ShouldAllowZeroThickness()
    {
        var center = new Position(10, 20);
        var radius = 15.0f;
        var thickness = 0.0f;
        var color = Color.Blue;

        var command = new DrawCircleOutlineCommand(center, radius, color, thickness);

        Assert.That(command.Thickness, Is.EqualTo(0.0f));
    }

    [Test]
    public void Constructor_WithZeroRadius_ShouldAllowZeroRadius()
    {
        var center = new Position(10, 20);
        var radius = 0.0f;
        var thickness = 1.0f;
        var color = Color.Red;

        var command = new DrawCircleOutlineCommand(center, radius, color, thickness);

        Assert.That(command.Radius, Is.EqualTo(0.0f));
    }

    [Test]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        var center = new Position(10, 20);
        var radius = 15.0f;
        var thickness = 2.0f;
        var color = Color.Yellow;

        var command1 = new DrawCircleOutlineCommand(center, radius, color, thickness);
        var command2 = new DrawCircleOutlineCommand(center, radius, color, thickness);

        Assert.That(command1, Is.EqualTo(command2));
        Assert.That(command1.GetHashCode(), Is.EqualTo(command2.GetHashCode()));
    }

    [Test]
    public void Equality_WithDifferentCenter_ShouldNotBeEqual()
    {
        var command1 = new DrawCircleOutlineCommand(new Position(10, 20), 15.0f, Color.Red, 2.0f);
        var command2 = new DrawCircleOutlineCommand(new Position(11, 20), 15.0f, Color.Red, 2.0f);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentRadius_ShouldNotBeEqual()
    {
        var center = new Position(10, 20);
        var command1 = new DrawCircleOutlineCommand(center, 15.0f, Color.Red, 2.0f);
        var command2 = new DrawCircleOutlineCommand(center, 16.0f, Color.Red, 2.0f);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentThickness_ShouldNotBeEqual()
    {
        var center = new Position(10, 20);
        var command1 = new DrawCircleOutlineCommand(center, 15.0f, Color.Red, 2.0f);
        var command2 = new DrawCircleOutlineCommand(center, 15.0f, Color.Red, 3.0f);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentColor_ShouldNotBeEqual()
    {
        var center = new Position(10, 20);
        var command1 = new DrawCircleOutlineCommand(center, 15.0f, Color.Red, 2.0f);
        var command2 = new DrawCircleOutlineCommand(center, 15.0f, Color.Blue, 2.0f);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void ToString_ShouldIncludeMainProperties()
    {
        var command = new DrawCircleOutlineCommand(new Position(5, 10), 7.5f, Color.Purple, 1.5f);

        var result = command.ToString();

        Assert.That(result, Does.Contain("DrawCircleOutlineCommand"));
        Assert.That(result, Does.Contain("5"));
        Assert.That(result, Does.Contain("10"));
        Assert.That(result, Does.Contain("7.5"));
        Assert.That(result, Does.Contain("1.5"));
    }

    [Test]
    public void RecordBehavior_ShouldSupportWithExpressions()
    {
        var original = new DrawCircleOutlineCommand(new Position(5, 10), 15.0f, Color.Red, 2.0f);

        var modified = original with { Thickness = 3.0f };

        Assert.That(modified.Center, Is.EqualTo(original.Center));
        Assert.That(modified.Radius, Is.EqualTo(original.Radius));
        Assert.That(modified.Thickness, Is.EqualTo(3.0f));
        Assert.That(modified.Color, Is.EqualTo(original.Color));
    }

    [Test]
    public void RecordBehavior_WithMultipleChanges_ShouldUpdateAllSpecifiedProperties()
    {
        var original = new DrawCircleOutlineCommand(new Position(5, 10), 15.0f, Color.Red, 2.0f);

        var modified = original with { Radius = 25.0f, Color = Color.Green };

        Assert.That(modified.Center, Is.EqualTo(original.Center));
        Assert.That(modified.Radius, Is.EqualTo(25.0f));
        Assert.That(modified.Thickness, Is.EqualTo(original.Thickness));
        Assert.That(modified.Color, Is.EqualTo(Color.Green));
    }
}