namespace Chevron9.Core.Types;

/// <summary>
///     Defines how this layer's output is combined with layers below it.
/// </summary>
public enum LayerCompositeMode
{
    /// <summary>
    ///     For terminal/grid backends: only draw non-empty cells,
    ///     leaving underlying content visible in empty spots.
    /// </summary>
    TransparentIfEmpty,

    /// <summary>
    ///     Standard: this layer overwrites whatever is below it.
    /// </summary>
    Overwrite,

    /// <summary>
    ///     Alpha blending: mix this layer with layers below using alpha values.
    /// </summary>
    AlphaBlend
}
