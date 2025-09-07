using System.Collections.Concurrent;

namespace Chevron9.Core.Utils;

/// <summary>
///     Thread-safe object pool for reducing allocations of frequently created objects
///     Uses ConcurrentBag for high-performance concurrent access
///     Includes pool size limits to prevent unbounded memory growth
/// </summary>
/// <typeparam name="T">Type of object to pool</typeparam>
public sealed class ObjectPool<T> where T : class
{
    private readonly ConcurrentBag<T> _pool = new();
    private readonly Func<T> _factory;
    private readonly Action<T>? _resetAction;
    private readonly int _maxPoolSize;

    /// <summary>
    ///     Gets the current number of objects in the pool
    /// </summary>
    public int Count => _pool.Count;

    /// <summary>
    ///     Initializes a new ObjectPool with factory function
    /// </summary>
    /// <param name="factory">Function to create new instances when pool is empty</param>
    /// <param name="resetAction">Optional action to reset object state before returning to pool</param>
    /// <param name="maxPoolSize">Maximum number of objects to keep in pool (default: 100)</param>
    public ObjectPool(Func<T> factory, Action<T>? resetAction = null, int maxPoolSize = 100)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxPoolSize);

        _factory = factory;
        _resetAction = resetAction;
        _maxPoolSize = maxPoolSize;
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
    /// <exception cref="ArgumentNullException">Thrown when item is null</exception>
    public void Return(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        // Reset object state if reset action provided
        _resetAction?.Invoke(item);

        // Only add to pool if under max size to prevent unbounded growth
        if (_pool.Count < _maxPoolSize)
        {
            _pool.Add(item);
        }
        // If pool is full, let GC handle the object
    }

    /// <summary>
    ///     Clears all objects from the pool
    /// </summary>
    public void Clear()
    {
        while (_pool.TryTake(out _)) { }
    }
}

/// <summary>
///     Static helper class for common object pool patterns
/// </summary>
public static class ObjectPool
{
    /// <summary>
    ///     Creates a pool for lists with specified initial capacity
    /// </summary>
    /// <typeparam name="T">Type of list elements</typeparam>
    /// <param name="initialCapacity">Initial capacity for pooled lists</param>
    /// <param name="maxPoolSize">Maximum pool size</param>
    /// <returns>Object pool for lists</returns>
    public static ObjectPool<List<T>> CreateListPool<T>(int initialCapacity = 16, int maxPoolSize = 50)
    {
        return new ObjectPool<List<T>>(
            () => new List<T>(initialCapacity),
            list => list.Clear(),
            maxPoolSize
        );
    }

    /// <summary>
    ///     Creates a pool for dictionaries
    /// </summary>
    /// <typeparam name="TKey">Type of dictionary keys</typeparam>
    /// <typeparam name="TValue">Type of dictionary values</typeparam>
    /// <param name="maxPoolSize">Maximum pool size</param>
    /// <returns>Object pool for dictionaries</returns>
    public static ObjectPool<Dictionary<TKey, TValue>> CreateDictionaryPool<TKey, TValue>(int maxPoolSize = 25)
        where TKey : notnull
    {
        return new ObjectPool<Dictionary<TKey, TValue>>(
            () => new Dictionary<TKey, TValue>(),
            dict => dict.Clear(),
            maxPoolSize
        );
    }
}
