using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Render;

/// <summary>
///     Default values and constants used throughout the rendering system
///     Centralizes configuration to avoid hardcoded values scattered across the codebase
/// </summary>
public static class RenderDefaults
{
    // Layer Z-Index defaults
    public const int BackgroundLayerZ = 100;
    public const int WorldLayerZ = 200;
    public const int EntityLayerZ = 300;
    public const int FxLayerZ = 800;
    public const int UiLayerZ = 1000;
    public const int DebugLayerZ = 9000;

    // Font and text defaults
    public const float DefaultFontSize = 12.0f;
    public const float TitleFontSize = 18.0f;
    public const float SubtitleFontSize = 14.0f;
    public const float SmallFontSize = 10.0f;

    // Material and sorting defaults
    public const int DefaultMaterialKey = 0;
    public const int DefaultSortKey = 0;
    public const int FontMaterialKey = 1;
    public const int TextureMaterialKey = 2;

    // Color defaults
    public static readonly Color DefaultTextColor = Color.White;
    public static readonly Color DefaultBackgroundColor = Color.Black;
    public static readonly Color DebugTextColor = Color.Yellow;
    public static readonly Color ErrorTextColor = Color.Red;
    public static readonly Color SuccessTextColor = Color.Green;

    // Geometry defaults
    public const float DefaultLineThickness = 1.0f;
    public const float DefaultOutlineThickness = 2.0f;
    public const float ThickLineThickness = 3.0f;

    // Screen dimensions (fallback when actual screen size unavailable)
    public const float DefaultScreenWidth = 800.0f;
    public const float DefaultScreenHeight = 600.0f;

    // Performance defaults
    public const int MaxCommandsPerFrame = 10000;
    public const int MaxLayersPerScene = 100;

    /// <summary>
    ///     Gets a standard background rectangle covering the default screen area
    /// </summary>
    /// <param name="color">Background color</param>
    /// <returns>Rectangle covering the default screen area</returns>
    public static RectF GetDefaultScreenRect(Color color = default)
    {
        return new RectF(0, 0, DefaultScreenWidth, DefaultScreenHeight);
    }

    /// <summary>
    ///     Gets standard padding values for UI elements
    /// </summary>
    public static class Padding
    {
        public const float Small = 5.0f;
        public const float Medium = 10.0f;
        public const float Large = 20.0f;
        public const float ExtraLarge = 40.0f;
    }

    /// <summary>
    ///     Gets standard margin values for layout
    /// </summary>
    public static class Margin
    {
        public const float Small = 5.0f;
        public const float Medium = 10.0f;
        public const float Large = 15.0f;
        public const float ExtraLarge = 25.0f;
    }
}
