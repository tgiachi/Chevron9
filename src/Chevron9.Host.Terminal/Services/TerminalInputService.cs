using Chevron9.Backends.Terminal.Input;
using Chevron9.Bootstrap.Interfaces.Base;
using Chevron9.Host.Terminal.Data.Configs;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Chevron9.Host.Terminal.Services;

/// <summary>
///     Service that manages terminal input processing
///     Integrates ConsoleInputDevice with the Chevron service lifecycle
/// </summary>
public sealed class TerminalInputService : IChevronAutostartService
{
    private readonly ILogger _logger = Log.ForContext<TerminalInputService>();
    private readonly Chevron9TerminalConfig _config;
    private ConsoleInputDevice? _inputDevice;
    private Task? _inputTask;
    private CancellationTokenSource? _inputCancellationSource;
    private bool _disposed;

    /// <summary>
    ///     Gets the console input device instance
    /// </summary>
    public ConsoleInputDevice? InputDevice => _inputDevice;

    /// <summary>
    ///     Creates a new terminal input service
    /// </summary>
    /// <param name="config">Terminal configuration</param>
    public TerminalInputService(Chevron9TerminalConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    ///     Loads the input service and initializes the console input device
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!_config.EnableInput)
        {
            _logger.Information("Terminal input is disabled, skipping input device initialization");
            return Task.CompletedTask;
        }

        _logger.Information("Loading terminal input service");

        try
        {
            _inputDevice = new ConsoleInputDevice();
            _logger.Information("Terminal input device initialized");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to initialize terminal input device");
            throw;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Starts the input service and begins input processing
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!_config.EnableInput || _inputDevice == null)
        {
            _logger.Information("Terminal input device not available, skipping start");
            return Task.CompletedTask;
        }

        _logger.Information("Starting terminal input service");

        try
        {
            // Start input processing task
            _inputCancellationSource = new CancellationTokenSource();
            _inputTask = Task.Run(() =>
            {
                InputProcessingLoop(_inputCancellationSource.Token);
            }, _inputCancellationSource.Token);

            _logger.Debug("Terminal input service started successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to start terminal input service");
            throw;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Stops the input service and cleanup
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            return;

        _logger.Information("Stopping terminal input service");

        try
        {
            // Cancel input processing
            _inputCancellationSource?.Cancel();

            // Wait for input task to complete
            if (_inputTask != null)
            {
                try
                {
                    await _inputTask;
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancelling
                }
            }

            // Cleanup
            _inputCancellationSource?.Dispose();
            _inputCancellationSource = null;
            _inputTask = null;

            // Input device cleanup is handled in Dispose

            _logger.Debug("Terminal input service stopped successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error stopping terminal input service");
        }
    }

    /// <summary>
    ///     Unloads the input service
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task UnloadAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            return Task.CompletedTask;

        _logger.Information("Unloading terminal input service");

        try
        {
            _inputDevice?.Dispose();
            _inputDevice = null;

            _logger.Debug("Terminal input service unloaded successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error unloading terminal input service");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Main input processing loop
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    private void InputProcessingLoop(CancellationToken cancellationToken)
    {
        _logger.Debug("Starting input processing loop");

        try
        {
            while (!cancellationToken.IsCancellationRequested && _inputDevice != null)
            {
                // Poll input device for current frame
                _inputDevice.Poll();

                // TODO: Process input events and emit to event bus
                // For now, just do a small sleep to prevent tight loop
                Thread.Sleep(16); // ~60 FPS polling
            }
        }
        catch (OperationCanceledException)
        {
            _logger.Debug("Input processing loop cancelled");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error in input processing loop");
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
            _logger.Error(ex, "Error during terminal input service disposal");
        }
        finally
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
