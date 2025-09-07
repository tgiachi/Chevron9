using Chevron9.Bootstrap.Extensions.Services;
using Chevron9.Bootstrap.Interfaces.Base;
using Chevron9.Core.Interfaces;
using Chevron9.Demo.Core.Scenes;
using Chevron9.Demo.Terminal;
using Chevron9.Host.Terminal.Extensions;

await TerminalDemoApp.RunAsync();

namespace Chevron9.Demo.Terminal
{
    /// <summary>
    ///     Terminal demo application showcasing Chevron9 engine features
    /// </summary>
    public static class TerminalDemoApp
    {
        /// <summary>
        ///     Runs the terminal demo application
        /// </summary>
        public static async Task RunAsync()
        {
            using var host = Chevron9TerminalHostExtensions.CreateDefaultHost()
                .WithRefreshRate(16) // 60 FPS
                .WithMouse(true)
                .WithCursor(false);

            host.OnRegisterServices += container =>
            {
                container.AddService<DemoSceneService>(priority: 50);
            };

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                await host.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Demo terminated by user");
            }
        }
    }

    /// <summary>
    ///     Service that manages demo scenes and handles scene transitions
    /// </summary>
    internal sealed class DemoSceneService : IChevronAutostartService
    {
        private readonly ISceneManager _sceneManager;
        private MenuScene? _menuScene;
        private RectangleScene? _rectangleScene;

        public DemoSceneService(ISceneManager sceneManager)
        {
            _sceneManager = sceneManager ?? throw new ArgumentNullException(nameof(sceneManager));
        }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            InitializeScenes();
            ShowMenu();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task UnloadAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _menuScene?.Dispose();
            _rectangleScene?.Dispose();
        }

        private void InitializeScenes()
        {
            _menuScene = new MenuScene(OnMenuItemSelected);
            _rectangleScene = new RectangleScene(ShowMenu);
        }

        private void ShowMenu()
        {
            if (_menuScene != null)
            {
                _sceneManager.ReplaceScene(_menuScene);
            }
        }

        private void OnMenuItemSelected(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0: // Rectangle Demo
                    if (_rectangleScene != null)
                    {
                        _sceneManager.ReplaceScene(_rectangleScene);
                    }
                    break;
                case 1: // Exit
                    Environment.Exit(0);
                    break;
            }
        }
    }

    /// <summary>
    ///     Example terminal application showing how to use Chevron9TerminalHost
    /// </summary>
    public static class ExampleTerminalApp
    {
        /// <summary>
        ///     Example of a simple terminal application
        /// </summary>
        public static async Task RunSimpleAppAsync()
        {
            // Create host with default settings
            using var host = Chevron9TerminalHostExtensions.CreateDefaultHost()
                .WithRefreshRate(16) // 60 FPS
                .WithMouse(true)
                .WithCursor(false);



            // Register custom services
            host.OnRegisterServices += container => { container.AddService<ExampleAppService>(priority: 50); };



            // Run the application
            using var cts = new CancellationTokenSource();

            // Handle Ctrl+C gracefully
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                await host.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Application terminated by user");
            }
        }

        /// <summary>
        ///     Example of a production terminal application
        /// </summary>
        public static async Task RunProductionAppAsync(string rootDirectory)
        {
            using var host = Chevron9TerminalHostExtensions.CreateProductionHost(rootDirectory);

            host.OnRegisterServices += container =>
            {
                // Register your application services here
                container.AddService<ExampleAppService>(priority: 50);
            };

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            await host.RunAsync(cts.Token);
        }

        /// <summary>
        ///     Example of manual initialization and control
        /// </summary>
        public static async Task RunManualControlAppAsync()
        {
            using var host = Chevron9TerminalHostExtensions.CreateDevelopmentHost("./app");

            host.OnRegisterServices += container => { container.AddService<ExampleAppService>(priority: 50); };

            // Manual initialization
            await host.InitializeAsync();
            await host.StartAsync();

            // Your application logic here
            await Task.Delay(5000); // Simulate work

            // Manual shutdown
            await host.StopAsync();
        }
    }

    /// <summary>
    ///     Example service that demonstrates integration with the terminal host
    /// </summary>
    internal sealed class ExampleAppService : IChevronAutostartService
    {
        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("ExampleAppService loaded");
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("ExampleAppService started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("ExampleAppService stopped");
            return Task.CompletedTask;
        }

        public Task UnloadAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("ExampleAppService unloaded");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Cleanup resources
        }
    }
}
