namespace Chevron9.Bootstrap.Interfaces.Base;

/// <summary>
///     Base interface for Chevron services with lifecycle management
///     Services implementing this interface will be automatically managed by the bootstrap system
/// </summary>
public interface IChevronService : IDisposable
{
    /// <summary>
    ///     Called when the service is loaded during application startup
    ///     Use this method to initialize resources, setup event handlers, etc.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for startup operations</param>
    /// <returns>Task representing the async load operation</returns>
    Task LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Called when the service is unloaded during application shutdown
    ///     Use this method to cleanup resources, dispose connections, etc.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for shutdown operations</param>
    /// <returns>Task representing the async unload operation</returns>
    Task UnloadAsync(CancellationToken cancellationToken = default);
}
