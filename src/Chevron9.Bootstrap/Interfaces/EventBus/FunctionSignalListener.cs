namespace Chevron9.Bootstrap.Interfaces.EventBus;

/// <summary>
///     Event bus listener implementation that wraps a function handler
///     Allows registering simple functions as event listeners without creating dedicated classes
/// </summary>
/// <typeparam name="TEvent">Event type to listen for</typeparam>
public class FunctionSignalListener<TEvent> : IEventBusListener<TEvent>, IEquatable<FunctionSignalListener<TEvent>>
    where TEvent : class
{
    private readonly Func<TEvent, Task> _handler;

    /// <summary>
    ///     Creates a new function-based event listener
    /// </summary>
    /// <param name="handler">Function to handle the event</param>
    public FunctionSignalListener(Func<TEvent, Task> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    /// <summary>
    ///     Handles the received event by invoking the wrapped function
    /// </summary>
    /// <param name="event">The event to handle</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        await _handler(@event);
    }

    /// <summary>
    ///     Checks if this listener wraps the same handler function as another
    /// </summary>
    /// <param name="handler">Handler function to compare</param>
    /// <returns>True if this listener wraps the same handler function</returns>
    public bool HasSameHandler(Func<TEvent, Task> handler)
    {
        return ReferenceEquals(_handler, handler);
    }

    /// <summary>
    ///     Gets the wrapped handler function
    /// </summary>
    /// <returns>The handler function</returns>
    public Func<TEvent, Task> GetHandler()
    {
        return _handler;
    }

    public bool Equals(FunctionSignalListener<TEvent>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ReferenceEquals(_handler, other._handler);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((FunctionSignalListener<TEvent>)obj);
    }

    public override int GetHashCode()
    {
        return _handler.GetHashCode();
    }

    public static bool operator ==(FunctionSignalListener<TEvent>? left, FunctionSignalListener<TEvent>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(FunctionSignalListener<TEvent>? left, FunctionSignalListener<TEvent>? right)
    {
        return !Equals(left, right);
    }
}
