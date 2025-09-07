using Chevron9.Core.Interfaces;

namespace Chevron9.Core.Extensions;

/// <summary>
///     Helper class for common validation patterns used throughout the rendering system
/// </summary>
internal static class ValidationHelper
{
    /// <summary>
    ///     Validates layer Z-index is non-negative
    /// </summary>
    /// <param name="layerZ">Layer Z-index to validate</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when layerZ is negative</exception>
    public static void ValidateLayerZ(int layerZ)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(layerZ);
    }

    /// <summary>
    ///     Validates font size is positive
    /// </summary>
    /// <param name="fontSize">Font size to validate</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when fontSize is negative or zero</exception>
    public static void ValidateFontSize(float fontSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fontSize);
    }

    /// <summary>
    ///     Validates rectangle dimensions are non-negative
    /// </summary>
    /// <param name="width">Rectangle width</param>
    /// <param name="height">Rectangle height</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when width or height are negative</exception>
    public static void ValidateRectangleDimensions(float width, float height)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(width);
        ArgumentOutOfRangeException.ThrowIfNegative(height);
    }

    /// <summary>
    ///     Validates circle radius is non-negative
    /// </summary>
    /// <param name="radius">Circle radius to validate</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when radius is negative</exception>
    public static void ValidateRadius(float radius)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(radius);
    }

    /// <summary>
    ///     Validates line thickness is positive
    /// </summary>
    /// <param name="thickness">Line thickness to validate</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when thickness is negative or zero</exception>
    public static void ValidateThickness(float thickness)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(thickness);
    }

    /// <summary>
    ///     Validates material and sort keys are non-negative
    /// </summary>
    /// <param name="materialKey">Material key to validate</param>
    /// <param name="sortKey">Sort key to validate</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when keys are negative</exception>
    public static void ValidateKeys(int materialKey, int sortKey)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(materialKey);
        ArgumentOutOfRangeException.ThrowIfNegative(sortKey);
    }
}
