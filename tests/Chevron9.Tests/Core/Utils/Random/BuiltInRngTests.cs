using Chevron9.Core.Utils.Random;

namespace Chevron9.Tests.Core.Utils.Random;

[TestFixture]
public class BuiltInRngTests
{
    [SetUp]
    public void SetUp()
    {
        BuiltInRng.Reset();
    }

    [Test]
    public void Generator_ShouldNotBeNull()
    {
        Assert.That(BuiltInRng.Generator, Is.Not.Null);
    }

    [Test]
    public void Reset_ShouldCreateNewGenerator()
    {
        var originalGenerator = BuiltInRng.Generator;
        BuiltInRng.Reset();
        var newGenerator = BuiltInRng.Generator;

        Assert.That(newGenerator, Is.Not.SameAs(originalGenerator));
    }

    [Test]
    public void Next_ShouldReturnNonNegativeInteger()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = BuiltInRng.Next();
            Assert.That(result, Is.GreaterThanOrEqualTo(0));
        }
    }

    [Test]
    public void Next_WithMaxValue_ShouldReturnValueInRange()
    {
        const int maxValue = 100;
        for (var i = 0; i < 100; i++)
        {
            var result = BuiltInRng.Next(maxValue);
            Assert.That(result, Is.InRange(0, maxValue - 1));
        }
    }

    [Test]
    public void Next_WithMinValueAndCount_ShouldReturnValueInRange()
    {
        const int minValue = 50;
        const int count = 30;

        for (var i = 0; i < 100; i++)
        {
            var result = BuiltInRng.Next(minValue, count);
            Assert.That(result, Is.InRange(minValue, minValue + count - 1));
        }
    }

    [Test]
    public void Next_WithLongMaxValue_ShouldReturnValueInRange()
    {
        const long maxValue = 1000L;
        for (var i = 0; i < 100; i++)
        {
            var result = BuiltInRng.Next(maxValue);
            Assert.That(result, Is.InRange(0L, maxValue - 1));
        }
    }

    [Test]
    public void NextDouble_ShouldReturnValueBetween0And1()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = BuiltInRng.NextDouble();
            Assert.That(result, Is.InRange(0.0, 1.0));
        }
    }

    [Test]
    public void NextBytes_ShouldFillBuffer()
    {
        var buffer = new byte[20];

        BuiltInRng.NextBytes(buffer);

        Assert.That(buffer.Length, Is.EqualTo(20));
    }

    [Test]
    public void NextBytes_WithEmptyBuffer_ShouldNotThrow()
    {
        var buffer = Array.Empty<byte>();

        Assert.DoesNotThrow(() => BuiltInRng.NextBytes(buffer));
    }
}
