using Chevron9.Core.Utils.Random;

namespace Chevron9.Tests.Core.Utils.Random;

[TestFixture]
public class RandomUtilsTests
{
    [SetUp]
    public void SetUp()
    {
        BuiltInRng.Reset();
    }

    [Test]
    public void Random_WithFromAndCount_ShouldReturnValueInRange()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = RandomUtils.Random(10, 5);
            Assert.That(result, Is.InRange(10, 14));
        }
    }

    [Test]
    public void Random_WithPositiveCount_ShouldReturnValueInRange()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = RandomUtils.Random(5);
            Assert.That(result, Is.InRange(0, 4));
        }
    }

    [Test]
    public void Random_WithNegativeCount_ShouldReturnNegativeValueInRange()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = RandomUtils.Random(-5);
            Assert.That(result, Is.InRange(-4, 0));
        }
    }

    [Test]
    public void Random_WithLongFromAndCount_ShouldReturnValueInRange()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = RandomUtils.Random(100L, 50L);
            Assert.That(result, Is.InRange(100L, 149L));
        }
    }

    [Test]
    public void RandomBytes_ShouldFillBuffer()
    {
        var buffer = new byte[10];
        RandomUtils.RandomBytes(buffer);

        Assert.That(buffer.Length, Is.EqualTo(10));
    }

    [Test]
    public void RandomDouble_ShouldReturnValueBetween0And1()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = RandomUtils.RandomDouble();
            Assert.That(result, Is.InRange(0.0, 1.0));
        }
    }

    [Test]
    public void CoinFlips_WithAmount_ShouldReturnValidCount()
    {
        for (var amount = 1; amount <= 10; amount++)
        {
            var result = RandomUtils.CoinFlips(amount);
            Assert.That(result, Is.InRange(0, amount));
        }
    }

    [Test]
    public void CoinFlips_WithAmountAndMaximum_ShouldNotExceedMaximum()
    {
        const int maximum = 5;
        for (var i = 0; i < 100; i++)
        {
            var result = RandomUtils.CoinFlips(10, maximum);
            Assert.That(result, Is.InRange(0, maximum));
        }
    }

    [Test]
    public void RandomList_WithValidArray_ShouldReturnOneOfItems()
    {
        var items = new[] { "apple", "banana", "cherry" };

        for (var i = 0; i < 100; i++)
        {
            var result = RandomUtils.RandomList(items);
            Assert.That(items, Contains.Item(result));
        }
    }

    [Test]
    public void RandomList_WithEmptyArray_ShouldReturnDefault()
    {
        var result = RandomUtils.RandomList<string>();
        Assert.That(result, Is.Null);
    }

    [Test]
    public void RandomList_WithSingleItem_ShouldReturnThatItem()
    {
        var result = RandomUtils.RandomList("only");
        Assert.That(result, Is.EqualTo("only"));
    }

    [Test]
    public void Dice_WithValidParameters_ShouldReturnCorrectRange()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = RandomUtils.Dice(2, 6, 0);
            Assert.That(result, Is.InRange(2, 12));
        }
    }

    [Test]
    public void Dice_WithBonus_ShouldAddBonusToResult()
    {
        const int bonus = 5;
        for (var i = 0; i < 100; i++)
        {
            var result = RandomUtils.Dice(1, 6, bonus);
            Assert.That(result, Is.InRange(1 + bonus, 6 + bonus));
        }
    }

    [Test]
    public void Dice_WithZeroAmount_ShouldReturnZero()
    {
        var result = RandomUtils.Dice(0, 6, 0);
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void Dice_WithNegativeAmount_ShouldReturnZero()
    {
        var result = RandomUtils.Dice(-1, 6, 0);
        Assert.That(result, Is.EqualTo(0));
    }
}
