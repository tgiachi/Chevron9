using Chevron9.Bootstrap.Extensions.DryIoc;
using DryIoc;
using NUnit.Framework;

namespace Chevron9.Tests.Bootstrap.Extensions.DryIoc;

/// <summary>
///     Tests for ContainerTypedListExtension to verify typed list management in DryIoc
/// </summary>
[TestFixture]
public sealed class ContainerTypedListExtensionTests
{
    private Container _container = null!;

    [SetUp]
    public void SetUp()
    {
        _container = new Container();
    }

    [TearDown]
    public void TearDown()
    {
        _container?.Dispose();
    }

    [Test]
    public void AddToRegisterTypedList_WithNewList_ShouldCreateListWithItem()
    {
        // Arrange
        var item = "test item";

        // Act
        _container.AddToRegisterTypedList(item);

        // Assert
        var list = _container.ResolveTypedList<string>();
        Assert.That(list, Has.Count.EqualTo(1));
        Assert.That(list[0], Is.EqualTo("test item"));
    }

    [Test]
    public void AddToRegisterTypedList_WithMultipleItems_ShouldAccumulateInList()
    {
        // Act
        _container.AddToRegisterTypedList("item1");
        _container.AddToRegisterTypedList("item2");
        _container.AddToRegisterTypedList("item3");

        // Assert
        var list = _container.ResolveTypedList<string>();
        Assert.That(list, Has.Count.EqualTo(3));
        Assert.That(list, Contains.Item("item1"));
        Assert.That(list, Contains.Item("item2"));
        Assert.That(list, Contains.Item("item3"));
    }

    [Test]
    public void AddToRegisterTypedList_WithNullContainer_ShouldThrowArgumentNullException()
    {
        // Arrange
        Container nullContainer = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            nullContainer.AddToRegisterTypedList("test"));
    }

    [Test]
    public void AddToRegisterTypedList_WithNullItem_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _container.AddToRegisterTypedList<string>(null!));
    }

    [Test]
    public void ResolveTypedList_WithNoRegistrations_ShouldReturnEmptyList()
    {
        // Act
        var list = _container.ResolveTypedList<string>();

        // Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void ResolveTypedList_WithNullContainer_ShouldThrowArgumentNullException()
    {
        // Arrange
        Container nullContainer = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            nullContainer.ResolveTypedList<string>());
    }

    [Test]
    public void IsTypedListRegistered_WithNoRegistrations_ShouldReturnFalse()
    {
        // Act
        var isRegistered = _container.IsTypedListRegistered<string>();

        // Assert
        Assert.That(isRegistered, Is.False);
    }

    [Test]
    public void IsTypedListRegistered_WithRegisteredList_ShouldReturnTrue()
    {
        // Arrange
        _container.AddToRegisterTypedList("test item");

        // Act
        var isRegistered = _container.IsTypedListRegistered<string>();

        // Assert
        Assert.That(isRegistered, Is.True);
    }

    [Test]
    public void GetTypedListCount_WithEmptyList_ShouldReturnZero()
    {
        // Act
        var count = _container.GetTypedListCount<string>();

        // Assert
        Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public void GetTypedListCount_WithMultipleItems_ShouldReturnCorrectCount()
    {
        // Arrange
        _container.AddToRegisterTypedList("item1");
        _container.AddToRegisterTypedList("item2");
        _container.AddToRegisterTypedList("item3");

        // Act
        var count = _container.GetTypedListCount<string>();

        // Assert
        Assert.That(count, Is.EqualTo(3));
    }

    [Test]
    public void TypedListExtensions_WithDifferentTypes_ShouldMaintainSeparateLists()
    {
        // Arrange & Act
        _container.AddToRegisterTypedList("string item");
        _container.AddToRegisterTypedList(42);
        _container.AddToRegisterTypedList("another string");

        // Assert
        var stringList = _container.ResolveTypedList<string>();
        var intList = _container.ResolveTypedList<int>();

        Assert.That(stringList, Has.Count.EqualTo(2));
        Assert.That(intList, Has.Count.EqualTo(1));
        Assert.That(stringList, Contains.Item("string item"));
        Assert.That(stringList, Contains.Item("another string"));
        Assert.That(intList, Contains.Item(42));
    }

    [Test]
    public void AddToRegisterTypedList_FluentChaining_ShouldReturnContainer()
    {
        // Act
        var result = _container.AddToRegisterTypedList("test");

        // Assert
        Assert.That(result, Is.EqualTo(_container));
    }

    [Test]
    public void AddToRegisterTypedList_WithComplexObjects_ShouldWorkCorrectly()
    {
        // Arrange
        var obj1 = new TestObject("Object1", 1);
        var obj2 = new TestObject("Object2", 2);

        // Act
        _container.AddToRegisterTypedList(obj1);
        _container.AddToRegisterTypedList(obj2);

        // Assert
        var list = _container.ResolveTypedList<TestObject>();
        Assert.That(list, Has.Count.EqualTo(2));
        Assert.That(list[0].Name, Is.EqualTo("Object1"));
        Assert.That(list[1].Name, Is.EqualTo("Object2"));
    }

    #region Test Helper Classes

    private sealed record TestObject(string Name, int Value);

    #endregion
}
