using System.Numerics;
using Chevron9.Shared.Extensions;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Shared.Extensions;

[TestFixture]
public class PositionExtensionsTests
{
    [Test]
    public void ToCell_ShouldRoundToNearestIntegers()
    {
        var position = new Position(3.7f, 4.3f);

        var (col, row) = position.ToCell();

        Assert.That(col, Is.EqualTo(4));
        Assert.That(row, Is.EqualTo(4));
    }

    [Test]
    public void ToCell_WithNegativeValues_ShouldRoundCorrectly()
    {
        var position = new Position(-2.3f, -1.7f);

        var (col, row) = position.ToCell();

        Assert.That(col, Is.EqualTo(-2));
        Assert.That(row, Is.EqualTo(-2));
    }

    [Test]
    public void ToCell_WithExactHalfValues_ShouldRoundToEven()
    {
        var position = new Position(2.5f, 3.5f);

        var (col, row) = position.ToCell();

        Assert.That(col, Is.EqualTo(2));
        Assert.That(row, Is.EqualTo(4));
    }

    [Test]
    public void ToVector2_ShouldCreateVector2WithSameValues()
    {
        var position = new Position(10.5f, 20.3f);

        var vector = position.ToVector2();

        Assert.That(vector.X, Is.EqualTo(10.5f));
        Assert.That(vector.Y, Is.EqualTo(20.3f));
    }

    [Test]
    public void ToPosition_ShouldCreatePositionWithSameValues()
    {
        var vector = new Vector2(15.7f, 25.1f);

        var position = vector.ToPosition();

        Assert.That(position.X, Is.EqualTo(15.7f));
        Assert.That(position.Y, Is.EqualTo(25.1f));
    }

    [Test]
    public void Vector2RoundTrip_ShouldMaintainValues()
    {
        var originalPosition = new Position(12.34f, 56.78f);

        var vector = originalPosition.ToVector2();
        var convertedBack = vector.ToPosition();

        Assert.That(convertedBack.X, Is.EqualTo(originalPosition.X));
        Assert.That(convertedBack.Y, Is.EqualTo(originalPosition.Y));
    }

    [Test]
    public void ToCell_WithZeroValues_ShouldReturnZero()
    {
        var position = Position.Zero;

        var (col, row) = position.ToCell();

        Assert.That(col, Is.EqualTo(0));
        Assert.That(row, Is.EqualTo(0));
    }

    [Test]
    public void ToVector2_WithZeroPosition_ShouldReturnZeroVector()
    {
        var position = Position.Zero;

        var vector = position.ToVector2();

        Assert.That(vector, Is.EqualTo(Vector2.Zero));
    }

    [Test]
    public void ToPosition_WithZeroVector_ShouldReturnZeroPosition()
    {
        var vector = Vector2.Zero;

        var position = vector.ToPosition();

        Assert.That(position, Is.EqualTo(Position.Zero));
    }
}
