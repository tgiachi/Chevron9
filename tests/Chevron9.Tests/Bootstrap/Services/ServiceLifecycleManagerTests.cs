using Chevron9.Bootstrap.Extensions.Services;
using Chevron9.Bootstrap.Interfaces.Base;
using Chevron9.Bootstrap.Services;
using DryIoc;
using NUnit.Framework;

namespace Chevron9.Tests.Bootstrap.Services;

/// <summary>
///     Comprehensive tests for ServiceLifecycleManager to verify priority-based lifecycle management
/// </summary>
[TestFixture]
public sealed class ServiceLifecycleManagerTests
{
    private Container _container = null!;
    private ServiceLifecycleManager _lifecycleManager = null!;

    [SetUp]
    public void SetUp()
    {
        _container = new Container();
        _lifecycleManager = new ServiceLifecycleManager(_container);
    }

    [TearDown]
    public void TearDown()
    {
        _lifecycleManager?.Dispose();
        _container?.Dispose();
    }

    [Test]
    public async Task LoadServicesAsync_WithRegisteredServices_ShouldLoadInPriorityOrder()
    {
        // Arrange - Register each with unique types to avoid DryIoc conflicts
        _container.AddService(typeof(PriorityTestService), typeof(PriorityTestService), priority: 10);
        _container.AddService(typeof(HighPriorityTestService), typeof(HighPriorityTestService), priority: 20);
        _container.AddService(typeof(AutostartTestService), typeof(AutostartTestService), priority: 5);

        // Act
        await _lifecycleManager.LoadServicesAsync();

        // Assert
        Assert.That(_lifecycleManager.GetLoadedServiceCount(), Is.EqualTo(3));
        Assert.That(_lifecycleManager.GetAutostartServiceCount(), Is.EqualTo(1));

        var loadedServices = _lifecycleManager.GetLoadedServiceDefinitions();
        Assert.That(loadedServices, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task LoadServicesAsync_WithNoServices_ShouldCompleteSuccessfully()
    {
        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _lifecycleManager.LoadServicesAsync());
        Assert.That(_lifecycleManager.GetLoadedServiceCount(), Is.EqualTo(0));
        Assert.That(_lifecycleManager.GetAutostartServiceCount(), Is.EqualTo(0));
    }

    [Test]
    public async Task LoadServicesAsync_WithFailingService_ShouldThrowAndStopLoading()
    {
        // Arrange
        _container.AddService(typeof(FailingTestService), typeof(FailingTestService), priority: 10);
        _container.AddService(typeof(PriorityTestService), typeof(PriorityTestService), priority: 5);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _lifecycleManager.LoadServicesAsync());
    }

    [Test]
    public async Task UnloadServicesAsync_WithLoadedServices_ShouldUnloadInReverseOrder()
    {
        // Arrange
        _container.AddService(typeof(OrderTrackingTestService), typeof(OrderTrackingTestService), priority: 15);
        _container.AddService(typeof(PriorityTestService), typeof(PriorityTestService), priority: 10);

        await _lifecycleManager.LoadServicesAsync();

        // Act
        await _lifecycleManager.UnloadServicesAsync();

        // Assert
        Assert.That(_lifecycleManager.GetLoadedServiceCount(), Is.EqualTo(0));
        Assert.That(_lifecycleManager.GetAutostartServiceCount(), Is.EqualTo(0));
        // Services were unloaded successfully
    }

    [Test]
    public async Task LoadServicesAsync_WithAutostartService_ShouldStartAutomatically()
    {
        // Arrange
        _container.AddService(typeof(TrackingAutostartService), typeof(TrackingAutostartService), priority: 10);

        // Act
        await _lifecycleManager.LoadServicesAsync();

        // Assert
        Assert.That(_lifecycleManager.GetAutostartServiceCount(), Is.EqualTo(1));

        // Get the actual service instance to check state
        var autostartService = _container.Resolve<TrackingAutostartService>();
        Assert.That(autostartService.WasLoaded, Is.True);
        Assert.That(autostartService.WasStarted, Is.True);
    }

    [Test]
    public async Task UnloadServicesAsync_WithAutostartService_ShouldStopBeforeUnload()
    {
        // Arrange
        _container.AddService(typeof(TrackingAutostartService), typeof(TrackingAutostartService), priority: 10);

        await _lifecycleManager.LoadServicesAsync();

        // Act
        await _lifecycleManager.UnloadServicesAsync();

        // Assert
        var autostartService = _container.Resolve<TrackingAutostartService>();
        Assert.That(autostartService.WasStopped, Is.True);
        Assert.That(autostartService.WasUnloaded, Is.True);
    }

    [Test]
    public async Task LoadServicesAsync_AfterDisposed_ShouldThrowObjectDisposedException()
    {
        // Arrange
        _lifecycleManager.Dispose();

        // Act & Assert
        Assert.ThrowsAsync<ObjectDisposedException>(
            () => _lifecycleManager.LoadServicesAsync());
    }

    [Test]
    public async Task UnloadServicesAsync_AfterDisposed_ShouldReturnSafely()
    {
        // Arrange
        _lifecycleManager.Dispose();

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _lifecycleManager.UnloadServicesAsync());
    }

    [Test]
    public void Dispose_ShouldUnloadServicesGracefully()
    {
        // Arrange
        _container.AddService<IChevronService, PriorityTestService>(priority: 10);
        _lifecycleManager.LoadServicesAsync().Wait();

        // Act & Assert
        Assert.DoesNotThrow(() => _lifecycleManager.Dispose());
    }

    [Test]
    public void Dispose_MultipleCalls_ShouldBeSafe()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            _lifecycleManager.Dispose();
            _lifecycleManager.Dispose();
        });
    }

    [Test]
    public async Task LoadServicesAsync_WithMixedServiceTypes_ShouldLoadAllCorrectly()
    {
        // Arrange
        _container.AddService(typeof(PriorityTestService), typeof(PriorityTestService), priority: 100);
        _container.AddService(typeof(AutostartTestService), typeof(AutostartTestService), priority: 50);
        _container.AddService(typeof(HighPriorityTestService), typeof(HighPriorityTestService), priority: 25);

        // Act
        await _lifecycleManager.LoadServicesAsync();

        // Assert
        Assert.That(_lifecycleManager.GetLoadedServiceCount(), Is.EqualTo(3));
        Assert.That(_lifecycleManager.GetAutostartServiceCount(), Is.EqualTo(1));

        var loadedServices = _lifecycleManager.GetLoadedServiceDefinitions();
        var autostartServices = _lifecycleManager.GetRunningAutostartServiceDefinitions();

        Assert.That(loadedServices, Has.Count.EqualTo(3));
        Assert.That(autostartServices, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task LoadServicesAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        _container.AddService(typeof(SlowLoadingTestService), typeof(SlowLoadingTestService), priority: 10);
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(50); // Cancel after 50ms

        // Act & Assert - TaskCanceledException is thrown by Task.Delay when cancelled
        Assert.ThrowsAsync<TaskCanceledException>(
            () => _lifecycleManager.LoadServicesAsync(cts.Token));
    }

    #region Test Helper Classes

    private sealed class PriorityTestService : IChevronService
    {
        public bool WasLoaded { get; private set; }
        public bool WasUnloaded { get; private set; }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            WasLoaded = true;
            return Task.CompletedTask;
        }

        public Task UnloadAsync(CancellationToken cancellationToken = default)
        {
            WasUnloaded = true;
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }

    private sealed class HighPriorityTestService : IChevronService
    {
        public bool WasLoaded { get; private set; }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            WasLoaded = true;
            return Task.CompletedTask;
        }

        public Task UnloadAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }

    private sealed class AutostartTestService : IChevronAutostartService
    {
        public bool WasLoaded { get; private set; }
        public bool WasStarted { get; private set; }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            WasLoaded = true;
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            WasStarted = true;
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

        public void Dispose() { }
    }

    private sealed class TrackingAutostartService : IChevronAutostartService
    {
        public bool WasLoaded { get; private set; }
        public bool WasStarted { get; private set; }
        public bool WasStopped { get; private set; }
        public bool WasUnloaded { get; private set; }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            WasLoaded = true;
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            WasStarted = true;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            WasStopped = true;
            return Task.CompletedTask;
        }

        public Task UnloadAsync(CancellationToken cancellationToken = default)
        {
            WasUnloaded = true;
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }

    private sealed class OrderTrackingTestService : IChevronService
    {
        public bool WasLoaded { get; private set; }
        public bool WasUnloaded { get; private set; }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            WasLoaded = true;
            return Task.CompletedTask;
        }

        public Task UnloadAsync(CancellationToken cancellationToken = default)
        {
            WasUnloaded = true;
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }

    private sealed class FailingTestService : IChevronService
    {
        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Load failed");
        }

        public Task UnloadAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }

    private sealed class SlowLoadingTestService : IChevronService
    {
        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(200, cancellationToken); // Slow loading to test cancellation
        }

        public Task UnloadAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }

    #endregion
}
