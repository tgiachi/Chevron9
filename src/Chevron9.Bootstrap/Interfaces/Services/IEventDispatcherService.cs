using Chevron9.Bootstrap.Interfaces.Base;

namespace Chevron9.Bootstrap.Interfaces.Services;

public interface IEventDispatcherService : IChevronAutostartService
{
    void SubscribeToEvent(string eventName, Action<object?> eventHandler);
    void UnsubscribeFromEvent(string eventName, Action<object?> eventHandler);
}
