using Chevron9.Bootstrap.Data.Services;
using Chevron9.Bootstrap.Extensions.DryIoc;
using Chevron9.Bootstrap.Interfaces.Base;
using DryIoc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Chevron9.Bootstrap.Services;

/// <summary>
///     Manages the lifecycle of Chevron services with priority-based loading and unloading
///     Handles both regular services (IChevronService) and autostart services (IChevronAutostartService)
/// </summary>
public sealed class ServiceLifecycleManager : IDisposable
{
    private readonly IContainer _container;
    private readonly ILogger _logger = Log.ForContext<ServiceLifecycleManager>();
    private readonly List<IChevronService> _loadedServices = new();
    private readonly List<IChevronAutostartService> _autostartServices = new();
    private bool _disposed;

    /// <summary>
    ///     Creates a new service lifecycle manager
    /// </summary>
    /// <param name="container">DryIoc container with registered services</param>
    public ServiceLifecycleManager(IContainer container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    /// <summary>
    ///     Loads all registered Chevron services in priority order (highest priority first)
    ///     Services implementing IChevronAutostartService will also be started
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Task representing the async operation</returns>
    public async Task LoadServicesAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _logger.Information("Starting service lifecycle management");

        // Get all service definitions and sort by priority (highest first)
        var serviceDefinitions = _container.ResolveTypedList<ServiceDefinitionObject>()
            .Where(svc => typeof(IChevronService).IsAssignableFrom(svc.ServiceType))
            .OrderByDescending(svc => svc.Priority)
            .ToList();

        _logger.Information("Found {ServiceCount} Chevron services to load", serviceDefinitions.Count);

        // Load services in priority order
        foreach (var serviceDefinition in serviceDefinitions)
        {
            try
            {
                _logger.Verbose("Loading service {ServiceType} (Priority: {Priority})",
                    serviceDefinition.ServiceType.Name, serviceDefinition.Priority);

                var service = (IChevronService)_container.Resolve(serviceDefinition.ServiceType);

                await service.LoadAsync(cancellationToken);
                _loadedServices.Add(service);

                // If it's an autostart service, start it
                if (service is IChevronAutostartService autostartService)
                {
                    _logger.Verbose("Starting autostart service {ServiceType}", serviceDefinition.ServiceType.Name);
                    await autostartService.StartAsync(cancellationToken);
                    _autostartServices.Add(autostartService);
                }

                _logger.Debug("Successfully loaded service {ServiceType}", serviceDefinition.ServiceType.Name);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load service {ServiceType}", serviceDefinition.ServiceType.Name);
                throw;
            }
        }

        _logger.Information("Successfully loaded {LoadedCount} services, {AutostartCount} autostarted",
            _loadedServices.Count, _autostartServices.Count);
    }

    /// <summary>
    ///     Unloads all loaded services in reverse priority order (lowest priority first)
    ///     Autostart services will be stopped before being unloaded
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Task representing the async operation</returns>
    public async Task UnloadServicesAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            return;

        _logger.Information("Starting service shutdown");

        // Stop autostart services first (in reverse order)
        var autostartServicesToStop = _autostartServices.AsEnumerable().Reverse().ToList();
        foreach (var autostartService in autostartServicesToStop)
        {
            try
            {
                _logger.Verbose("Stopping autostart service {ServiceType}", autostartService.GetType().Name);
                await autostartService.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error stopping autostart service {ServiceType}", autostartService.GetType().Name);
                // Continue with other services
            }
        }

        // Unload all services in reverse order (lowest priority first)
        var servicesToUnload = _loadedServices.AsEnumerable().Reverse().ToList();
        foreach (var service in servicesToUnload)
        {
            try
            {
                _logger.Verbose("Unloading service {ServiceType}", service.GetType().Name);
                await service.UnloadAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error unloading service {ServiceType}", service.GetType().Name);
                // Continue with other services
            }

            // Dispose the service if it implements IDisposable
            try
            {
                service.Dispose();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error disposing service {ServiceType}", service.GetType().Name);
                // Continue with other services
            }
        }

        _autostartServices.Clear();
        _loadedServices.Clear();

        _logger.Information("Service shutdown completed");
    }

    /// <summary>
    ///     Gets the count of currently loaded services
    /// </summary>
    /// <returns>Number of loaded services</returns>
    public int GetLoadedServiceCount() => _loadedServices.Count;

    /// <summary>
    ///     Gets the count of currently running autostart services
    /// </summary>
    /// <returns>Number of running autostart services</returns>
    public int GetAutostartServiceCount() => _autostartServices.Count;

    /// <summary>
    ///     Gets information about all loaded services
    /// </summary>
    /// <returns>List of service definitions for loaded services</returns>
    public List<ServiceDefinitionObject> GetLoadedServiceDefinitions()
    {
        var serviceDefinitions = _container.ResolveTypedList<ServiceDefinitionObject>()
            .Where(svc => typeof(IChevronService).IsAssignableFrom(svc.ServiceType))
            .ToDictionary(svc => svc.ServiceType, svc => svc);

        return _loadedServices.Select(service =>
        {
            var serviceType = service.GetType();
            serviceDefinitions.TryGetValue(serviceType, out var definition);
            return definition;
        }).ToList();
    }

    /// <summary>
    ///     Gets service definitions for autostart services that are currently running
    /// </summary>
    /// <returns>List of service definitions for running autostart services</returns>
    public List<ServiceDefinitionObject> GetRunningAutostartServiceDefinitions()
    {
        var serviceDefinitions = _container.ResolveTypedList<ServiceDefinitionObject>()
            .Where(svc => typeof(IChevronService).IsAssignableFrom(svc.ServiceType))
            .ToDictionary(svc => svc.ServiceType, svc => svc);

        return _autostartServices.Select(service =>
        {
            var serviceType = service.GetType();
            serviceDefinitions.TryGetValue(serviceType, out var definition);
            return definition;
        }).ToList();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        try
        {
            // Use a reasonable timeout for shutdown
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            UnloadServicesAsync(cts.Token).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during service lifecycle manager disposal");
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}

