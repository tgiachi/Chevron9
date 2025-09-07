using System.Collections.Concurrent;
using System.Reactive.Subjects;
using System.Threading.Channels;
using Chevron9.Bootstrap.Data.Events;
using Chevron9.Bootstrap.Interfaces.EventBus;
using Chevron9.Bootstrap.Interfaces.Services;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Chevron9.Bootstrap.Services;

public class EventBusService : IEventBusService, IDisposable
{
    private readonly ILogger _logger = Log.ForContext<EventBusService>();
    private readonly ConcurrentDictionary<Type, ConcurrentBag<object>> _listeners = new();
    private readonly Channel<EventDispatchJob> _channel;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _processingTask;
    private readonly Subject<object> _allEventsSubject = new();

    /// <summary>
    /// Observable that emits all events
    /// </summary>
    public IObservable<object> AllEventsObservable => _allEventsSubject;

    public EventBusService()
    {
        _channel = Channel.CreateUnbounded<EventDispatchJob>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            }
        );

        _processingTask = Task.Run(ProcessEventsAsync, _cts.Token);

        _logger.Information("EventBusService initialized with Channel");
    }

    /// <summary>
    /// Registers a listener for a specific event type
    /// </summary>
    public void Subscribe<TEvent>(IEventBusListener<TEvent> listener) where TEvent : class
    {
        var eventType = typeof(TEvent);
        var listeners = _listeners.GetOrAdd(
            eventType,
            _ => new ConcurrentBag<object>()
        );

        listeners.Add(listener);

        _logger.Verbose(
            "Registered listener {ListenerType} for event {EventType}",
            listener.GetType().Name,
            eventType.Name
        );
    }

    /// <summary>
    /// Registers a function as a listener for a specific event type
    /// </summary>
    public void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        var listener = new FunctionSignalListener<TEvent>(handler);
        Subscribe(listener);

        _logger.Verbose("Registered function handler for event {EventType}", typeof(TEvent).Name);
    }

    /// <summary>
    /// Unregisters a listener for a specific event type
    /// </summary>
    public void Unsubscribe<TEvent>(IEventBusListener<TEvent> listener) where TEvent : class
    {
        var eventType = typeof(TEvent);

        if (_listeners.TryGetValue(eventType, out var listeners))
        {
            // Thread-safe removal by recreating the bag
            lock (listeners)
            {
                var filteredListeners = listeners.Cast<IEventBusListener<TEvent>>()
                    .Where(l => !ReferenceEquals(l, listener))
                    .Cast<object>()
                    .ToArray();

                var newBag = new ConcurrentBag<object>(filteredListeners);
                _listeners.TryUpdate(eventType, newBag, listeners);
            }

            _logger.Verbose(
                "Unregistered listener {ListenerType} from event {EventType}",
                listener.GetType().Name,
                eventType.Name
            );
        }
    }

    /// <summary>
    /// Unregisters a function handler for a specific event type
    /// </summary>
    public void Unsubscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        var eventType = typeof(TEvent);

        if (_listeners.TryGetValue(eventType, out var listeners))
        {
            // Thread-safe removal by recreating the bag
            lock (listeners)
            {
                var filteredListeners = listeners.Cast<IEventBusListener<TEvent>>()
                    .Where(l => !(l is FunctionSignalListener<TEvent> functionListener) ||
                                !functionListener.HasSameHandler(handler))
                    .Cast<object>()
                    .ToArray();

                var newBag = new ConcurrentBag<object>(filteredListeners);
                _listeners.TryUpdate(eventType, newBag, listeners);
            }

            _logger.Verbose("Unregistered function handler for event {EventType}", eventType.Name);
        }
    }

    /// <summary>
    /// Publishes an event to all registered listeners asynchronously
    /// </summary>
    public async Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        var eventType = typeof(TEvent);

        _allEventsSubject.OnNext(eventData);

        if (!_listeners.TryGetValue(eventType, out var listeners))
        {
            _logger.Verbose("No listeners registered for event {EventType}", eventType.Name);
            return;
        }

        var typedListeners = listeners.Cast<IEventBusListener<TEvent>>().ToArray();

        _logger.Verbose(
            "Publishing event {EventType} to {ListenerCount} listeners",
            eventType.Name,
            typedListeners.Length
        );

        foreach (var listener in typedListeners)
        {
            var job = new EventDispatchJob<TEvent>(listener, eventData);
            await _channel.Writer.WriteAsync(job, cancellationToken);
        }
    }

    /// <summary>
    /// Returns total listener count
    /// </summary>
    public int GetListenerCount()
    {
        int total = 0;

        foreach (var kvp in _listeners)
        {
            total += kvp.Value.Count;
        }

        return total;
    }

    /// <summary>
    /// Returns listener count for a specific event type
    /// </summary>
    public int GetListenerCount<TEvent>() where TEvent : class
    {
        if (_listeners.TryGetValue(typeof(TEvent), out var listeners))
        {
            return listeners.Count;
        }

        return 0;
    }

    /// <summary>
    /// Waits for all pending events to be processed
    /// </summary>
    public async Task WaitForCompletionAsync()
    {
        if (!_channel.Writer.TryComplete())
        {
            _logger.Warning("Channel writer already completed");
        }

        try
        {
            await _processingTask;
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
    }

    /// <summary>
    /// Background processor for event dispatch jobs
    /// </summary>
    private async Task ProcessEventsAsync()
    {
        try
        {
            await foreach (var job in _channel.Reader.ReadAllAsync(_cts.Token))
            {
                try
                {
                    await job.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error while executing job {JobType}", job.GetType().Name);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.Information("Event processing was cancelled");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error in event processing task");
        }
    }

    public void Dispose()
    {
        try
        {
            _cts.Cancel();
            _channel.Writer.TryComplete();

            // Use async disposal pattern to avoid deadlock
            if (_processingTask.IsCompleted)
            {
                _processingTask.Wait(100); // Short timeout
            }

            _allEventsSubject.OnCompleted();
            _allEventsSubject.Dispose();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during EventBusService disposal");
        }
        finally
        {
            _cts.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
