namespace Chevron9.Core.Extensions;

/// <summary>
///     Extension methods for array manipulation and enumeration
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    ///     Reverses array enumeration without allocations using yield return
    ///     More efficient than LINQ Reverse() which creates intermediate collections
    /// </summary>
    /// <typeparam name="T">Array element type</typeparam>
    /// <param name="array">Array to enumerate in reverse</param>
    /// <returns>Enumerable that yields elements from last to first</returns>
    public static IEnumerable<T> ReverseEnumerate<T>(this T[] array)
    {
        for (var i = array.Length - 1; i >= 0; i--)
        {
            yield return array[i];
        }
    }
}
