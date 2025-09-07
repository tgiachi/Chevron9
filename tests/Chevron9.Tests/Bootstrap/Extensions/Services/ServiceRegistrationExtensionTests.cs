using Chevron9.Bootstrap.Data.Services;
using Chevron9.Bootstrap.Extensions.Services;
using Chevron9.Bootstrap.Interfaces.Base;
using DryIoc;
using NUnit.Framework;

namespace Chevron9.Tests.Bootstrap.Extensions.Services;

/// <summary>
///     Tests for ServiceRegistrationExtension to verify service registration with priorities
/// </summary>
[TestFixture]
public sealed class ServiceRegistrationExtensionTests
{
    private Container _container = null!;

    [SetUp]
    public void SetUp()
    {
        _container = new Container();
    }

    [TearDown]
    public void TearDown()
    {
        _container?.Dispose();
    }

    [Test]
    public void AddService_WithInterfaceAndImplementation_ShouldRegisterService()
    {
        // Act
        _container.AddService<ITestService, TestServiceImplementation>(priority: 10);

        // Assert
        var service = _container.Resolve<ITestService>();
        Assert.That(service, Is.Not.Null);
        Assert.That(service, Is.InstanceOf<TestServiceImplementation>());

        var registeredServices = _container.GetRegisteredServices();
        Assert.That(registeredServices, Has.Count.EqualTo(1));
        Assert.That(registeredServices[0].Priority, Is.EqualTo(10));
    }

    [Test]
    public void AddService_WithConcreteType_ShouldRegisterService()
    {
        // Act
        _container.AddService<ConcreteTestService>(priority: 20);

        // Assert
        var service = _container.Resolve<ConcreteTestService>();
        Assert.That(service, Is.Not.Null);

        var registeredServices = _container.GetRegisteredServices();
        Assert.That(registeredServices, Has.Count.EqualTo(1));
        Assert.That(registeredServices[0].ServiceType, Is.EqualTo(typeof(ConcreteTestService)));
        Assert.That(registeredServices[0].Priority, Is.EqualTo(20));
    }

    [Test]
    public void AddService_WithTypeParameters_ShouldRegisterService()
    {
        // Act
        _container.AddService(typeof(ITestService), typeof(TestServiceImplementation), priority: 15);

        // Assert
        var service = _container.Resolve<ITestService>();
        Assert.That(service, Is.Not.Null);
        Assert.That(service, Is.InstanceOf<TestServiceImplementation>());

        var registeredServices = _container.GetRegisteredServices();
        Assert.That(registeredServices[0].Priority, Is.EqualTo(15));
    }

    [Test]
    public void AddService_WithDefaultPriority_ShouldUseZeroPriority()
    {
        // Act
        _container.AddService<ITestService, TestServiceImplementation>();

        // Assert
        var registeredServices = _container.GetRegisteredServices();
        Assert.That(registeredServices[0].Priority, Is.EqualTo(0));
    }

    [Test]
    public void AddService_WithMultipleServices_ShouldRegisterAllWithCorrectPriorities()
    {
        // Act
        _container.AddService<ITestService, TestServiceImplementation>(priority: 100);
        _container.AddService<ConcreteTestService>(priority: 50);
        _container.AddService<IAnotherTestService, AnotherTestServiceImplementation>(priority: 25);

        // Assert
        var registeredServices = _container.GetRegisteredServices();
        Assert.That(registeredServices, Has.Count.EqualTo(3));

        var orderedServices = registeredServices.OrderByDescending(s => s.Priority).ToList();
        Assert.That(orderedServices[0].Priority, Is.EqualTo(100));
        Assert.That(orderedServices[1].Priority, Is.EqualTo(50));
        Assert.That(orderedServices[2].Priority, Is.EqualTo(25));
    }

    [Test]
    public void AddService_WithNullContainer_ShouldThrowArgumentNullException()
    {
        // Arrange
        IContainer nullContainer = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            nullContainer.AddService<ITestService, TestServiceImplementation>());
    }

    [Test]
    public void AddService_WithNullServiceType_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _container.AddService(null!, typeof(TestServiceImplementation)));
    }

    [Test]
    public void AddService_WithNullImplementationType_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _container.AddService(typeof(ITestService), null!));
    }

    [Test]
    public void GetRegisteredServices_WithNoServices_ShouldReturnEmptyList()
    {
        // Act
        var registeredServices = _container.GetRegisteredServices();

        // Assert
        Assert.That(registeredServices, Is.Not.Null);
        Assert.That(registeredServices, Is.Empty);
    }

    [Test]
    public void GetRegisteredServiceCount_WithMultipleServices_ShouldReturnCorrectCount()
    {
        // Arrange
        _container.AddService<ITestService, TestServiceImplementation>();
        _container.AddService<ConcreteTestService>();

        // Act
        var count = _container.GetRegisteredServiceCount();

        // Assert
        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public void AddService_FluentChaining_ShouldReturnContainer()
    {
        // Act
        var result = _container.AddService<ITestService, TestServiceImplementation>();

        // Assert
        Assert.That(result, Is.EqualTo(_container));
    }

    [Test]
    public void AddService_WithChevronServices_ShouldBeCompatibleWithLifecycleManager()
    {
        // Act
        _container.AddService<IChevronService, ChevronTestService>(priority: 10);
        _container.AddService<IChevronAutostartService, ChevronAutostartTestService>(priority: 20);

        // Assert
        var chevronService = _container.Resolve<IChevronService>();
        var autostartService = _container.Resolve<IChevronAutostartService>();

        Assert.That(chevronService, Is.Not.Null);
        Assert.That(autostartService, Is.Not.Null);

        var registeredServices = _container.GetRegisteredServices();
        var chevronServices = registeredServices.Where(s => typeof(IChevronService).IsAssignableFrom(s.ServiceType)).ToList();
        Assert.That(chevronServices, Has.Count.EqualTo(2));
    }

    #region Test Helper Classes

    private interface ITestService { }
    private interface IAnotherTestService { }

    private sealed class TestServiceImplementation : ITestService { }
    private sealed class AnotherTestServiceImplementation : IAnotherTestService { }
    private sealed class ConcreteTestService { }

    private sealed class ChevronTestService : IChevronService
    {
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UnloadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Dispose() { }
    }

    private sealed class ChevronAutostartTestService : IChevronAutostartService
    {
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UnloadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Dispose() { }
    }

    #endregion
}
