using System.Collections.Concurrent;

namespace Chevron9.Core.Utils;

/// <summary>
///     Thread-safe object pool for reducing allocations of frequently created objects
///     Uses ConcurrentBag for high-performance concurrent access
/// </summary>
/// <typeparam name="T">Type of object to pool</typeparam>
public sealed class ObjectPool<T> where T : class
{
    private readonly ConcurrentBag<T> _pool = new();
    private readonly Func<T> _factory;
    private readonly Action<T>? _resetAction;

    /// <summary>
    ///     Initializes a new ObjectPool with factory function
    /// </summary>
    /// <param name="factory">Function to create new instances when pool is empty</param>
    /// <param name="resetAction">Optional action to reset object state before returning to pool</param>
    public ObjectPool(Func<T> factory, Action<T>? resetAction = null)
    {
        _factory = factory;
        _resetAction = resetAction;
    }

    /// <summary>
    ///     Gets an object from the pool, creating a new one if necessary
    /// </summary>
    /// <returns>Object instance from pool or newly created</returns>
    public T Get()
    {
        return _pool.TryTake(out var item) ? item : _factory();
    }

    /// <summary>
    ///     Returns an object to the pool for reuse
    /// </summary>
    /// <param name="item">Object to return to pool</param>
    public void Return(T item)
    {
        _resetAction?.Invoke(item);
        _pool.Add(item);
    }
}
