using Chevron9.Bootstrap.Data.Configs;
using Chevron9.Bootstrap.Data.Services;
using Chevron9.Bootstrap.Extensions.Loggers;
using Chevron9.Bootstrap.Extensions.Services;
using Chevron9.Bootstrap.Interfaces.Services;
using Chevron9.Bootstrap.Services;
using Chevron9.Core.Directories;
using Chevron9.Core.Interfaces;
using Chevron9.Core.Loop;
using Chevron9.Core.Render;
using Chevron9.Core.Scenes;
using DryIoc;
using Serilog;
using Serilog.Formatting.Compact;

namespace Chevron9.Bootstrap;

public class Chevron9Bootstrap : IDisposable
{
    private readonly Container _container;
    private readonly Chevron9Config _config;
    private DirectoriesConfig _directoriesConfig;
    private ServiceLifecycleManager? _serviceLifecycleManager;

    public event EventHandler? OnStarted;
    public event EventHandler? OnStopped;
    public event RegisterServicesHandler OnRegisterServices;

    public delegate void RegisterServicesHandler(Container container);

    public Chevron9Bootstrap(Chevron9Config config)
    {
        _container = new Container();
        _config = config;

        InitDirectoryConfig();

        InitLogging();
        RegisterCoreServices();
    }


    private void InitDirectoryConfig()
    {
        if (!_config.DefaultDirectories.Contains("Logs"))
        {
            var dirs = _config.DefaultDirectories.ToList();
            dirs.Add("Logs");
            _config.DefaultDirectories = dirs.ToArray();
        }

        _directoriesConfig = new DirectoriesConfig(_config.RootDirectory, _config.DefaultDirectories);
    }

    private void InitLogging()
    {
        var loggingConfigurator = new LoggerConfiguration();

        loggingConfigurator.MinimumLevel.Is(_config.LogLevel.ToSerilogLogLevel());

        if (_config.LogToFile)
        {
            loggingConfigurator.WriteTo.File(
                new CompactJsonFormatter(),
                Path.Combine(_directoriesConfig.Root, "Logs", "engine_.log"),
                rollingInterval: RollingInterval.Day
            );
        }

        if (_config.LogToConsole)
        {
            loggingConfigurator.WriteTo.Console();
        }

        Log.Logger = loggingConfigurator.CreateLogger();
    }


    private void RegisterCoreServices()
    {
        _container.RegisterInstance(_config);
        _container.RegisterInstance(_directoriesConfig);

        _container.Register<IEventBusService, EventBusService>(Reuse.Singleton);

        _container.AddService(typeof(IEventDispatcherService), typeof(EventDispatcherService), 0);

        _container.Register<IRenderCommandCollector, RenderCommandCollector>();
        _container.Register<ISceneManager, SceneManager>(Reuse.Singleton);
    }

    public void RegisterFixedEventLoop()
    {
        _container.Register<IEventLoop, FixedEventLoop>();
    }

    public void RegisterVariableEventLoop()
    {
        _container.Register<IEventLoop, VariableEventLoop>();
    }



    public Task InitializeAsync()
    {
        OnRegisterServices?.Invoke(_container);

        // Create service lifecycle manager
        _serviceLifecycleManager = new ServiceLifecycleManager(_container);

        return Task.CompletedTask;
    }


    public async Task StartAsync()
    {
        if (_serviceLifecycleManager == null)
            throw new InvalidOperationException("Bootstrap not initialized. Call InitializeAsync() first.");

        // Load and start all registered Chevron services
        await _serviceLifecycleManager.LoadServicesAsync();

        OnStarted?.Invoke(this, EventArgs.Empty);
    }


    public async Task StopAsync()
    {
        if (_serviceLifecycleManager != null)
        {
            // Stop and unload all services
            await _serviceLifecycleManager.UnloadServicesAsync();
        }

        OnStopped?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        try
        {
            // Ensure services are stopped before disposing
            _serviceLifecycleManager?.Dispose();
        }
        finally
        {
            _container.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    ///     Gets information about loaded services
    /// </summary>
    /// <returns>List of loaded service definitions</returns>
    public List<ServiceDefinitionObject> GetLoadedServices()
    {
        return _serviceLifecycleManager?.GetLoadedServiceDefinitions() ?? new List<ServiceDefinitionObject>();
    }

    /// <summary>
    ///     Gets count of loaded services
    /// </summary>
    /// <returns>Number of loaded services</returns>
    public int GetLoadedServiceCount()
    {
        return _serviceLifecycleManager?.GetLoadedServiceCount() ?? 0;
    }

    /// <summary>
    ///     Gets count of running autostart services
    /// </summary>
    /// <returns>Number of running autostart services</returns>
    public int GetAutostartServiceCount()
    {
        return _serviceLifecycleManager?.GetAutostartServiceCount() ?? 0;
    }
}
