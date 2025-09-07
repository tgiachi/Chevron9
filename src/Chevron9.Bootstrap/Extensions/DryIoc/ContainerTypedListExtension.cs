using DryIoc;

namespace Chevron9.Bootstrap.Extensions.DryIoc;

/// <summary>
///     Extension methods for DryIoc container to manage typed lists
///     Allows registration of multiple items of the same type that can be resolved as a List&lt;T&gt;
/// </summary>
public static class ContainerTypedListExtension
{
    /// <summary>
    ///     Adds an item to a typed list that can be resolved as List&lt;T&gt;
    ///     If the list doesn't exist, it creates a new one
    /// </summary>
    /// <typeparam name="T">Type of items in the list</typeparam>
    /// <param name="container">DryIoc container instance</param>
    /// <param name="item">Item to add to the list</param>
    /// <returns>The container for fluent chaining</returns>
    public static IContainer AddToRegisterTypedList<T>(this IContainer container, T item)
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(item);

        // Try to resolve existing list, or create new one
        var existingList = container.IsRegistered<List<T>>()
            ? container.Resolve<List<T>>()
            : new List<T>();

        existingList.Add(item);

        // Register the updated list as a singleton
        container.RegisterInstance<List<T>>(existingList, IfAlreadyRegistered.Replace);

        return container;
    }

    /// <summary>
    ///     Resolves a typed list from the container
    ///     Returns empty list if no items were registered
    /// </summary>
    /// <typeparam name="T">Type of items in the list</typeparam>
    /// <param name="container">DryIoc container instance</param>
    /// <returns>List of registered items, or empty list if none found</returns>
    public static List<T> ResolveTypedList<T>(this IContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);

        return container.IsRegistered<List<T>>()
            ? container.Resolve<List<T>>()
            : new List<T>();
    }

    /// <summary>
    ///     Checks if a typed list has been registered in the container
    /// </summary>
    /// <typeparam name="T">Type of items in the list</typeparam>
    /// <param name="container">DryIoc container instance</param>
    /// <returns>True if the list is registered, false otherwise</returns>
    public static bool IsTypedListRegistered<T>(this IContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);
        return container.IsRegistered<List<T>>();
    }

    /// <summary>
    ///     Gets the count of items in a typed list
    /// </summary>
    /// <typeparam name="T">Type of items in the list</typeparam>
    /// <param name="container">DryIoc container instance</param>
    /// <returns>Number of items in the list, or 0 if list doesn't exist</returns>
    public static int GetTypedListCount<T>(this IContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);
        return container.ResolveTypedList<T>().Count;
    }
}
