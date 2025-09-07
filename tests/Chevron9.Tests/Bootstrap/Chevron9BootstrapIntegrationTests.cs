using Chevron9.Bootstrap;
using Chevron9.Bootstrap.Data.Configs;
using Chevron9.Bootstrap.Types;
using Chevron9.Bootstrap.Extensions.Services;
using Chevron9.Bootstrap.Interfaces.Base;
using DryIoc;
using NUnit.Framework;

namespace Chevron9.Tests.Bootstrap;

/// <summary>
///     Integration tests for Chevron9Bootstrap with ServiceLifecycleManager
/// </summary>
[TestFixture]
public sealed class Chevron9BootstrapIntegrationTests
{
    private Chevron9Bootstrap _bootstrap = null!;

    [SetUp]
    public void SetUp()
    {
        var config = new Chevron9Config
        {
            RootDirectory = "./test_temp",
            LogLevel = LogLevelType.Warning, // Reduce noise in tests
            LogToConsole = false,
            LogToFile = false
        };

        _bootstrap = new Chevron9Bootstrap(config);
    }

    [TearDown]
    public void TearDown()
    {
        _bootstrap?.Dispose();

        // Cleanup test directory
        if (Directory.Exists("./test_temp"))
        {
            Directory.Delete("./test_temp", recursive: true);
        }
    }

    [Test]
    public async Task Bootstrap_WithRegisteredServices_ShouldLoadAndStartServices()
    {
        // Arrange
        _bootstrap.OnRegisterServices += container =>
        {
            container.AddService<TestBootstrapService>(priority: 10);
            container.AddService<TestBootstrapAutostartService>(priority: 20);
        };

        // Act
        await _bootstrap.InitializeAsync();
        await _bootstrap.StartAsync();

        // Assert
        Assert.That(_bootstrap.GetLoadedServiceCount(), Is.EqualTo(3)); // 2 test services + EventDispatcherService
        Assert.That(_bootstrap.GetAutostartServiceCount(), Is.EqualTo(2)); // 1 test autostart + EventDispatcherService
    }

    [Test]
    public async Task Bootstrap_StartAndStop_ShouldCompleteSuccessfully()
    {
        // Arrange
        _bootstrap.OnRegisterServices += container =>
        {
            container.AddService<TestBootstrapService>(priority: 5);
        };

        await _bootstrap.InitializeAsync();
        await _bootstrap.StartAsync();

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _bootstrap.StopAsync());

        // After stop, services should still be counted (they're unloaded but tracked)
        Assert.That(_bootstrap.GetLoadedServiceCount(), Is.EqualTo(0));
        Assert.That(_bootstrap.GetAutostartServiceCount(), Is.EqualTo(0));
    }

    [Test]
    public async Task Bootstrap_WithoutInitialization_ShouldThrowOnStart()
    {
        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _bootstrap.StartAsync());
    }

    [Test]
    public void Bootstrap_Dispose_ShouldCleanupGracefully()
    {
        // Act & Assert
        Assert.DoesNotThrow(() => _bootstrap.Dispose());
        Assert.DoesNotThrow(() => _bootstrap.Dispose()); // Multiple dispose should be safe
    }

    #region Test Helper Services

    private sealed class TestBootstrapService : IChevronService
    {
        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task UnloadAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }

    private sealed class TestBootstrapAutostartService : IChevronAutostartService
    {
        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
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
        }
    }

    #endregion
}
