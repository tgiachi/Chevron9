using Chevron9.Bootstrap;
using Chevron9.Bootstrap.Extensions.Services;
using Chevron9.Bootstrap.Interfaces.Base;
using Chevron9.Host.Terminal.Data.Configs;
using Chevron9.Host.Terminal.Services;
using DryIoc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Chevron9.Host.Terminal;

/// <summary>
///     Terminal host that combines Chevron9Bootstrap with terminal-specific services
///     Provides a complete terminal application hosting environment
/// </summary>
public sealed class Chevron9TerminalHost : IDisposable
{
    private readonly ILogger _logger = Log.ForContext<Chevron9TerminalHost>();
    private readonly Chevron9Bootstrap _bootstrap;
    private readonly Chevron9TerminalConfig _config;
    private IContainer? _container;
    private bool _disposed;
    private bool _initialized;
    private bool _started;

    /// <summary>
    ///     Event fired when additional services need to be registered
    ///     Use this to register your application-specific services
    /// </summary>
    public event Action<IContainer>? OnRegisterServices;

    /// <summary>
    ///     Gets the terminal render service instance (available after initialization)
    /// </summary>
    public TerminalRenderService? RenderService { get; private set; }

    /// <summary>
    ///     Gets the terminal input service instance (available after initialization)
    /// </summary>
    public TerminalInputService? InputService { get; private set; }

    /// <summary>
    ///     Gets the underlying bootstrap instance
    /// </summary>
    public Chevron9Bootstrap Bootstrap => _bootstrap;

    /// <summary>
    ///     Gets the current configuration
    /// </summary>
    public Chevron9TerminalConfig Configuration => _config;

    /// <summary>
    ///     Gets the loaded service count
    /// </summary>
    public int GetLoadedServiceCount() => _bootstrap.GetLoadedServiceCount();

    /// <summary>
    ///     Gets the autostart service count
    /// </summary>
    public int GetAutostartServiceCount() => _bootstrap.GetAutostartServiceCount();

    /// <summary>
    ///     Creates a new terminal host
    /// </summary>
    /// <param name="config">Terminal host configuration</param>
    public Chevron9TerminalHost(Chevron9TerminalConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _bootstrap = new Chevron9Bootstrap(config);

        // Wire up bootstrap events
        _bootstrap.OnRegisterServices += RegisterTerminalServices;
    }

    /// <summary>
    ///     Initializes the terminal host and all services
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_initialized)
            throw new InvalidOperationException("Terminal host is already initialized");

        _logger.Information("Initializing Chevron9 Terminal Host");

        try
        {
            await _bootstrap.InitializeAsync();
            _initialized = true;

            // Resolve terminal services after initialization
            if (_container != null)
            {
                RenderService = _container.Resolve<TerminalRenderService>();
                InputService = _container.Resolve<TerminalInputService>();
            }

            _logger.Information("Chevron9 Terminal Host initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to initialize terminal host");
            throw;
        }
    }

    /// <summary>
    ///     Starts the terminal host and all autostart services
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!_initialized)
            throw new InvalidOperationException("Terminal host must be initialized before starting");

        if (_started)
            throw new InvalidOperationException("Terminal host is already started");

        _logger.Information("Starting Chevron9 Terminal Host");

        try
        {
            await _bootstrap.StartAsync();
            _started = true;

            _logger.Information("Chevron9 Terminal Host started successfully");
            _logger.Information("Loaded {LoadedCount} services, {AutostartCount} autostarted",
                GetLoadedServiceCount(), GetAutostartServiceCount());
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to start terminal host");
            throw;
        }
    }

    /// <summary>
    ///     Stops the terminal host and all running services
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed || !_started)
            return;

        _logger.Information("Stopping Chevron9 Terminal Host");

        try
        {
            await _bootstrap.StopAsync();
            _started = false;

            _logger.Information("Chevron9 Terminal Host stopped successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error stopping terminal host");
        }
    }

    /// <summary>
    ///     Runs the terminal host application until cancellation is requested
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _logger.Information("Running Chevron9 Terminal Host");

        try
        {
            if (!_initialized)
            {
                await InitializeAsync(cancellationToken);
            }

            if (!_started)
            {
                await StartAsync(cancellationToken);
            }

            // Wait for cancellation
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.Information("Terminal host run cancelled");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error running terminal host");
            throw;
        }
        finally
        {
            await StopAsync(CancellationToken.None);
        }
    }

    /// <summary>
    ///     Registers terminal-specific services with the container
    /// </summary>
    /// <param name="container">DryIoc container</param>
    private void RegisterTerminalServices(IContainer container)
    {
        _logger.Debug("Registering terminal services");

        try
        {
            // Store container reference for later use
            _container = container;

            // Register configuration as singleton
            container.RegisterInstance(_config);

            // Register terminal services with appropriate priorities
            // Higher priority for render service as other services may depend on it
            container.AddService<TerminalRenderService>(priority: 100);
            container.AddService<TerminalInputService>(priority: 90);

            // Allow application to register additional services
            OnRegisterServices?.Invoke(container);

            _logger.Debug("Terminal services registered successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to register terminal services");
            throw;
        }
    }

    /// <summary>
    ///     Disposes the terminal host and releases all resources
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        try
        {
            StopAsync().GetAwaiter().GetResult();
            _bootstrap.Dispose();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during terminal host disposal");
        }
        finally
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
