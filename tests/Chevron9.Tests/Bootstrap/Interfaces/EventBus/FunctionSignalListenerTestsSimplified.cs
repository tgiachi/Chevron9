using Chevron9.Bootstrap.Interfaces.EventBus;
using NUnit.Framework;

namespace Chevron9.Tests.Bootstrap.Interfaces.EventBus;

/// <summary>
///     Core tests for FunctionSignalListener functionality
/// </summary>
[TestFixture]
public sealed class FunctionSignalListenerTestsSimplified
{
    [Test]
    public void Constructor_WithValidHandler_ShouldCreateListener()
    {
        // Arrange
        Func<TestEvent, Task> handler = evt => Task.CompletedTask;

        // Act
        var listener = new FunctionSignalListener<TestEvent>(handler);

        // Assert
        Assert.That(listener, Is.Not.Null);
        Assert.That(listener.GetHandler(), Is.EqualTo(handler));
    }

    [Test]
    public void Constructor_WithNullHandler_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new FunctionSignalListener<TestEvent>(null!));
    }

    [Test]
    public async Task HandleAsync_ShouldInvokeWrappedFunction()
    {
        // Arrange
        var receivedEvents = new List<TestEvent>();
        Func<TestEvent, Task> handler = evt =>
        {
            receivedEvents.Add(evt);
            return Task.CompletedTask;
        };

        var listener = new FunctionSignalListener<TestEvent>(handler);
        var testEvent = new TestEvent("Test Message", 42);

        // Act
        await listener.HandleAsync(testEvent);

        // Assert
        Assert.That(receivedEvents, Has.Count.EqualTo(1));
        Assert.That(receivedEvents[0], Is.EqualTo(testEvent));
    }

    [Test]
    public void HasSameHandler_WithSameHandler_ShouldReturnTrue()
    {
        // Arrange
        Func<TestEvent, Task> handler = evt => Task.CompletedTask;
        var listener = new FunctionSignalListener<TestEvent>(handler);

        // Act
        var result = listener.HasSameHandler(handler);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void HasSameHandler_WithDifferentHandler_ShouldReturnFalse()
    {
        // Arrange
        Func<TestEvent, Task> handler1 = evt => Task.CompletedTask;
        Func<TestEvent, Task> handler2 = evt => Task.CompletedTask;
        var listener = new FunctionSignalListener<TestEvent>(handler1);

        // Act
        var result = listener.HasSameHandler(handler2);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Equals_WithSameHandler_ShouldReturnTrue()
    {
        // Arrange
        Func<TestEvent, Task> handler = evt => Task.CompletedTask;
        var listener1 = new FunctionSignalListener<TestEvent>(handler);
        var listener2 = new FunctionSignalListener<TestEvent>(handler);

        // Act & Assert
        Assert.That(listener1.Equals(listener2), Is.True);
        Assert.That(listener1 == listener2, Is.True);
        Assert.That(listener1 != listener2, Is.False);
    }

    [Test]
    public void GetHashCode_WithSameHandler_ShouldReturnSameHashCode()
    {
        // Arrange
        Func<TestEvent, Task> handler = evt => Task.CompletedTask;
        var listener1 = new FunctionSignalListener<TestEvent>(handler);
        var listener2 = new FunctionSignalListener<TestEvent>(handler);

        // Act
        var hash1 = listener1.GetHashCode();
        var hash2 = listener2.GetHashCode();

        // Assert
        Assert.That(hash1, Is.EqualTo(hash2));
    }

    #region Test Helper Classes

    private sealed record TestEvent(string Message, int Value);

    #endregion
}
