using Chevron9.Bootstrap.Interfaces.Services;
using Chevron9.Core.Extensions.Strings;
using Serilog;

namespace Chevron9.Bootstrap.Services;

public class EventDispatcherService : IEventDispatcherService
{
    private readonly Dictionary<string, List<Action<object?>>> _eventHandlers = new();

    private readonly ILogger _logger = Log.ForContext<EventDispatcherService>();

    public EventDispatcherService(IEventBusService eventBusService)
    {
        eventBusService.AllEventsObservable.Subscribe(OnEvent);
    }

    private void OnEvent(object obj)
    {
        DispatchEvent(obj.GetType().Name.ToSnakeCase().Replace("_event", ""), obj);
    }


    private void DispatchEvent(string eventName, object? eventData = null)
    {
        _logger.Debug("Dispatching event {EventName}", eventName);
        if (!_eventHandlers.TryGetValue(eventName, out var eventHandler))
        {
            return;
        }

        foreach (var handler in eventHandler)
        {
            handler(eventData);
        }
    }

    public void SubscribeToEvent(string eventName, Action<object?> eventHandler)
    {
        if (!_eventHandlers.TryGetValue(eventName, out var eventHandlers))
        {
            eventHandlers = new List<Action<object?>>();
            _eventHandlers.Add(eventName, eventHandlers);
        }

        eventHandlers.Add(eventHandler);
    }

    public void UnsubscribeFromEvent(string eventName, Action<object?> eventHandler)
    {
        if (!_eventHandlers.TryGetValue(eventName, out var eventHandlers))
        {
            return;
        }

        eventHandlers.Remove(eventHandler);
    }

    public void Dispose()
    {
        _eventHandlers.Clear();
        GC.SuppressFinalize(this);
    }

    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        _logger.Information("EventDispatcherService loaded");
        return Task.CompletedTask;
    }

    public Task UnloadAsync(CancellationToken cancellationToken = default)
    {
        _logger.Information("EventDispatcherService unloading");
        _eventHandlers.Clear();
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.Information("EventDispatcherService started");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.Information("EventDispatcherService stopping");
        return Task.CompletedTask;
    }
}
