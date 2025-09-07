using Chevron9.Backends.Terminal.Renderer;
using Chevron9.Bootstrap.Interfaces.Base;
using Chevron9.Host.Terminal.Data.Configs;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Chevron9.Host.Terminal.Services;

/// <summary>
///     Service that manages terminal rendering and display updates
///     Integrates ConsoleRender with the Chevron service lifecycle
/// </summary>
public sealed class TerminalRenderService : IChevronAutostartService
{
    private readonly ILogger _logger = Log.ForContext<TerminalRenderService>();
    private readonly Chevron9TerminalConfig _config;
    private ConsoleRender? _renderer;
    private Timer? _refreshTimer;
    private bool _disposed;

    /// <summary>
    ///     Gets the console renderer instance
    /// </summary>
    public ConsoleRender? Renderer => _renderer;

    /// <summary>
    ///     Creates a new terminal render service
    /// </summary>
    /// <param name="config">Terminal configuration</param>
    public TerminalRenderService(Chevron9TerminalConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    ///     Loads the render service and initializes the console renderer
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!_config.EnableOutput)
        {
            _logger.Information("Terminal output is disabled, skipping renderer initialization");
            return Task.CompletedTask;
        }

        _logger.Information("Loading terminal render service");

        try
        {
            _renderer = new ConsoleRender();

            _logger.Information("Terminal renderer initialized with dimensions {Width}x{Height}", _renderer.Width, _renderer.Height);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to initialize terminal renderer");
            throw;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Starts the render service and begins the refresh loop
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!_config.EnableOutput || _renderer == null)
        {
            _logger.Information("Terminal renderer not available, skipping start");
            return Task.CompletedTask;
        }

        _logger.Information("Starting terminal render service with {RefreshRate}ms refresh rate", _config.RefreshRateMs);

        try
        {
            // Start refresh timer
            _refreshTimer = new Timer(RefreshCallback, null, 0, _config.RefreshRateMs);

            _logger.Debug("Terminal render service started successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to start terminal render service");
            throw;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Stops the render service and cleanup
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            return;

        _logger.Information("Stopping terminal render service");

        try
        {
            // Stop refresh timer
            if (_refreshTimer != null)
            {
                await _refreshTimer.DisposeAsync();
                _refreshTimer = null;
            }

            // Terminal cleanup is handled in renderer's Dispose

            _logger.Debug("Terminal render service stopped successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error stopping terminal render service");
        }
    }

    /// <summary>
    ///     Unloads the render service
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task UnloadAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            return Task.CompletedTask;

        _logger.Information("Unloading terminal render service");

        try
        {
            _renderer?.Dispose();
            _renderer = null;

            _logger.Debug("Terminal render service unloaded successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error unloading terminal render service");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Timer callback for rendering updates
    /// </summary>
    private void RefreshCallback(object? state)
    {
        try
        {
            if (_renderer != null && !_disposed)
            {
                _renderer.BeginFrame();
                _renderer.EndFrame();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during terminal refresh");
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        try
        {
            StopAsync().GetAwaiter().GetResult();
            UnloadAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during terminal render service disposal");
        }
        finally
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
