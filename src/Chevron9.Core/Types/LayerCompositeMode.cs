namespace Chevron9.Core.Types;

/// <summary>
/// Defines how this layer's output is combined with layers below it.
/// </summary>
public enum LayerComposeMode
{
    /// <summary>
    /// For terminal/grid backends: only draw non-empty cells,
    /// leaving underlying content visible in empty spots.
    /// </summary>
    TransparentIfEmpty,

    /// <summary>
    /// Standard: this layer overwrites whatever is below it.
    /// </summary>
    Overwrite
}
