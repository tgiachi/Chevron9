namespace Chevron9.Bootstrap.Interfaces.Base;

/// <summary>
///     Interface for Chevron services that should be automatically started during application initialization
///     Services implementing this interface will be resolved and started automatically based on their priority
/// </summary>
public interface IChevronAutostartService : IChevronService
{
    /// <summary>
    ///     Called after LoadAsync to start the service's main functionality
    ///     This is where the service should begin its primary operations
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for start operations</param>
    /// <returns>Task representing the async start operation</returns>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Called before UnloadAsync to stop the service's main functionality
    ///     This is where the service should gracefully stop its primary operations
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for stop operations</param>
    /// <returns>Task representing the async stop operation</returns>
    Task StopAsync(CancellationToken cancellationToken = default);
}
