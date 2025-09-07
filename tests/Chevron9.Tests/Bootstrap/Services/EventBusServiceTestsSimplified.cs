using System.Collections.Concurrent;
using Chevron9.Bootstrap.Interfaces.EventBus;
using Chevron9.Bootstrap.Services;
using NUnit.Framework;

namespace Chevron9.Tests.Bootstrap.Services;

/// <summary>
///     Core tests for EventBusService functionality
/// </summary>
[TestFixture]
public sealed class EventBusServiceTestsSimplified
{
    private EventBusService _eventBus = null!;
    private TestEvent _testEvent = null!;

    [SetUp]
    public void SetUp()
    {
        _eventBus = new EventBusService();
        _testEvent = new TestEvent("Test Message", 42);
    }

    [TearDown]
    public void TearDown()
    {
        _eventBus?.Dispose();
    }

    [Test]
    public async Task Subscribe_WithListener_ShouldReceiveEvent()
    {
        // Arrange
        var listener = new TestEventListener();

        // Act
        _eventBus.Subscribe<TestEvent>(listener);
        await _eventBus.PublishAsync(_testEvent);
        await _eventBus.WaitForCompletionAsync();

        // Assert
        Assert.That(listener.ReceivedEvents, Has.Count.EqualTo(1));
        Assert.That(listener.ReceivedEvents[0].Message, Is.EqualTo("Test Message"));
    }

    [Test]
    public async Task Subscribe_WithFunctionHandler_ShouldReceiveEvent()
    {
        // Arrange
        var receivedEvents = new List<TestEvent>();

        // Act
        _eventBus.Subscribe<TestEvent>(evt =>
        {
            receivedEvents.Add(evt);
            return Task.CompletedTask;
        });

        await _eventBus.PublishAsync(_testEvent);
        await _eventBus.WaitForCompletionAsync();

        // Assert
        Assert.That(receivedEvents, Has.Count.EqualTo(1));
        Assert.That(receivedEvents[0].Message, Is.EqualTo("Test Message"));
    }

    [Test]
    public async Task Unsubscribe_WithListener_ShouldStopReceivingEvents()
    {
        // Arrange
        var listener = new TestEventListener();
        _eventBus.Subscribe<TestEvent>(listener);

        // Act
        _eventBus.Unsubscribe<TestEvent>(listener);
        await _eventBus.PublishAsync(_testEvent);
        await _eventBus.WaitForCompletionAsync();

        // Assert
        Assert.That(listener.ReceivedEvents, Is.Empty);
        Assert.That(_eventBus.GetListenerCount<TestEvent>(), Is.EqualTo(0));
    }

    [Test]
    public void GetListenerCount_ShouldReturnCorrectCount()
    {
        // Arrange
        var listener1 = new TestEventListener();
        var listener2 = new TestEventListener();

        // Act
        _eventBus.Subscribe<TestEvent>(listener1);
        _eventBus.Subscribe<TestEvent>(listener2);

        // Assert
        Assert.That(_eventBus.GetListenerCount<TestEvent>(), Is.EqualTo(2));
        Assert.That(_eventBus.GetListenerCount(), Is.EqualTo(2));
    }

    [Test]
    public void Dispose_ShouldCleanupResources()
    {
        // Arrange
        var listener = new TestEventListener();
        _eventBus.Subscribe<TestEvent>(listener);

        // Act & Assert
        Assert.DoesNotThrow(() => _eventBus.Dispose());
        Assert.DoesNotThrow(() => _eventBus.Dispose()); // Multiple dispose calls should be safe
    }

    #region Test Helper Classes

    private sealed record TestEvent(string Message, int Value);

    private sealed class TestEventListener : IEventBusListener<TestEvent>
    {
        public List<TestEvent> ReceivedEvents { get; } = new();

        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken = default)
        {
            ReceivedEvents.Add(@event);
            return Task.CompletedTask;
        }
    }

    #endregion
}
