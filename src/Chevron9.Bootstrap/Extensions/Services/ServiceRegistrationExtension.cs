using Chevron9.Bootstrap.Data.Services;
using Chevron9.Bootstrap.Extensions.DryIoc;
using DryIoc;

namespace Chevron9.Bootstrap.Extensions.Services;

/// <summary>
///     Extension methods for registering Chevron services with lifecycle management
/// </summary>
public static class ServiceRegistrationExtension
{
    /// <summary>
    ///     Registers a service with its implementation type and priority
    /// </summary>
    /// <param name="container">DryIoc container</param>
    /// <param name="serviceType">Service interface type</param>
    /// <param name="implementationType">Implementation type</param>
    /// <param name="priority">Loading priority (higher values load first)</param>
    /// <returns>Container for fluent chaining</returns>
    public static IContainer AddService(this IContainer container, Type serviceType, Type implementationType, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(implementationType);

        container.Register(serviceType, implementationType, Reuse.Singleton);
        container.AddToRegisterTypedList(new ServiceDefinitionObject(serviceType, implementationType, priority));

        return container;
    }

    /// <summary>
    ///     Registers a service where the type is both the interface and implementation
    /// </summary>
    /// <param name="container">DryIoc container</param>
    /// <param name="serviceType">Service type</param>
    /// <param name="priority">Loading priority (higher values load first)</param>
    /// <returns>Container for fluent chaining</returns>
    public static IContainer AddService(this IContainer container, Type serviceType, int priority = 0)
    {
        return AddService(container, serviceType, serviceType, priority);
    }

    /// <summary>
    ///     Registers a service using generic types
    /// </summary>
    /// <typeparam name="TService">Service interface type</typeparam>
    /// <typeparam name="TImplementation">Implementation type</typeparam>
    /// <param name="container">DryIoc container</param>
    /// <param name="priority">Loading priority (higher values load first)</param>
    /// <returns>Container for fluent chaining</returns>
    public static IContainer AddService<TService, TImplementation>(this IContainer container, int priority = 0)
        where TImplementation : class, TService
    {
        return AddService(container, typeof(TService), typeof(TImplementation), priority);
    }

    /// <summary>
    ///     Registers a service using generic type where type is both interface and implementation
    /// </summary>
    /// <typeparam name="TService">Service type</typeparam>
    /// <param name="container">DryIoc container</param>
    /// <param name="priority">Loading priority (higher values load first)</param>
    /// <returns>Container for fluent chaining</returns>
    public static IContainer AddService<TService>(this IContainer container, int priority = 0)
        where TService : class
    {
        return AddService(container, typeof(TService), priority);
    }

    /// <summary>
    ///     Gets all registered service definitions
    /// </summary>
    /// <param name="container">DryIoc container</param>
    /// <returns>List of service definitions</returns>
    public static List<ServiceDefinitionObject> GetRegisteredServices(this IContainer container)
    {
        return container.ResolveTypedList<ServiceDefinitionObject>();
    }

    /// <summary>
    ///     Gets the count of registered services
    /// </summary>
    /// <param name="container">DryIoc container</param>
    /// <returns>Number of registered services</returns>
    public static int GetRegisteredServiceCount(this IContainer container)
    {
        return container.GetTypedListCount<ServiceDefinitionObject>();
    }
}
