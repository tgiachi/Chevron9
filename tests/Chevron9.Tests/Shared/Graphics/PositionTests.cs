using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Shared.Graphics;

[TestFixture]
public class PositionTests
{
    [Test]
    public void Constructor_ShouldSetProperties()
    {
        var position = new Position(10.5f, 20.3f);

        Assert.That(position.X, Is.EqualTo(10.5f));
        Assert.That(position.Y, Is.EqualTo(20.3f));
    }

    [Test]
    public void Zero_ShouldReturnZeroPosition()
    {
        var zero = Position.Zero;

        Assert.That(zero.X, Is.EqualTo(0f));
        Assert.That(zero.Y, Is.EqualTo(0f));
    }

    [Test]
    public void DistanceTo_ShouldCalculateCorrectDistance()
    {
        var pos1 = new Position(0f, 0f);
        var pos2 = new Position(3f, 4f);

        var distance = pos1.DistanceTo(pos2);

        Assert.That(distance, Is.EqualTo(5f));
    }

    [Test]
    public void Length_ShouldCalculateCorrectLength()
    {
        var position = new Position(3f, 4f);

        var length = position.Length();

        Assert.That(length, Is.EqualTo(5f));
    }

    [Test]
    public void Normalized_WithValidVector_ShouldReturnUnitVector()
    {
        var position = new Position(3f, 4f);

        var normalized = position.Normalized();

        Assert.That(normalized.X, Is.EqualTo(0.6f).Within(0.001f));
        Assert.That(normalized.Y, Is.EqualTo(0.8f).Within(0.001f));
        Assert.That(normalized.Length(), Is.EqualTo(1f).Within(0.001f));
    }

    [Test]
    public void Normalized_WithZeroVector_ShouldThrowInvalidOperationException()
    {
        var position = Position.Zero;

        Assert.Throws<InvalidOperationException>(() => position.Normalized());
    }

    [Test]
    public void Lerp_ShouldInterpolateCorrectly()
    {
        var start = new Position(0f, 0f);
        var end = new Position(10f, 20f);

        var midpoint = start.Lerp(end, 0.5f);

        Assert.That(midpoint.X, Is.EqualTo(5f));
        Assert.That(midpoint.Y, Is.EqualTo(10f));
    }

    [Test]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        var pos1 = new Position(3.5f, 4.2f);
        var pos2 = new Position(3.5f, 4.2f);

        Assert.That(pos1.Equals(pos2), Is.True);
        Assert.That(pos1 == pos2, Is.True);
        Assert.That(pos1 != pos2, Is.False);
    }

    [Test]
    public void ToString_ShouldReturnCorrectFormat()
    {
        var position = new Position(3.14f, 2.71f);

        var str = position.ToString();

        Assert.That(str, Is.EqualTo("Position(X:3.14, Y:2.71)"));
    }

    [Test]
    public void AdditionOperator_ShouldAddPositions()
    {
        var pos1 = new Position(3f, 4f);
        var pos2 = new Position(2f, 1f);

        var result = pos1 + pos2;

        Assert.That(result.X, Is.EqualTo(5f));
        Assert.That(result.Y, Is.EqualTo(5f));
    }

    [Test]
    public void SubtractionOperator_ShouldSubtractPositions()
    {
        var pos1 = new Position(5f, 7f);
        var pos2 = new Position(2f, 3f);

        var result = pos1 - pos2;

        Assert.That(result.X, Is.EqualTo(3f));
        Assert.That(result.Y, Is.EqualTo(4f));
    }

    [Test]
    public void MultiplicationByScalar_ShouldScalePosition()
    {
        var position = new Position(3f, 4f);
        var scalar = 2f;

        var result1 = position * scalar;
        var result2 = scalar * position;

        Assert.That(result1.X, Is.EqualTo(6f));
        Assert.That(result1.Y, Is.EqualTo(8f));
        Assert.That(result2, Is.EqualTo(result1));
    }

    [Test]
    public void DivisionByScalar_WithZero_ShouldThrowDivideByZeroException()
    {
        var position = new Position(6f, 8f);

        Assert.Throws<DivideByZeroException>(() =>
            {
                var result = position / 0f;
            }
        );
    }
}
