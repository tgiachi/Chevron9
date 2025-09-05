namespace Chevron9.Shared.Graphics;

/// <summary>
///     Contains predefined common colors as static readonly properties.
/// </summary>
public readonly partial struct Color
{
    #region Basic Colors

    /// <summary>Gets a color with RGBA values (0, 0, 0, 255) - Black.</summary>
    public static Color Black => new(0, 0, 0);

    /// <summary>Gets a color with RGBA values (255, 255, 255, 255) - White.</summary>
    public static Color White => new(255, 255, 255);

    /// <summary>Gets a color with RGBA values (255, 0, 0, 255) - Red.</summary>
    public static Color Red => new(255, 0, 0);

    /// <summary>Gets a color with RGBA values (0, 255, 0, 255) - Green.</summary>
    public static Color Green => new(0, 255, 0);

    /// <summary>Gets a color with RGBA values (0, 0, 255, 255) - Blue.</summary>
    public static Color Blue => new(0, 0, 255);

    /// <summary>Gets a color with RGBA values (255, 255, 0, 255) - Yellow.</summary>
    public static Color Yellow => new(255, 255, 0);

    /// <summary>Gets a color with RGBA values (255, 0, 255, 255) - Magenta.</summary>
    public static Color Magenta => new(255, 0, 255);

    /// <summary>Gets a color with RGBA values (0, 255, 255, 255) - Cyan.</summary>
    public static Color Cyan => new(0, 255, 255);

    /// <summary>Gets a color with RGBA values (0, 0, 0, 0) - Transparent.</summary>
    public static Color Transparent => new(0, 0, 0, 0);

    #endregion

    #region Gray Scale

    /// <summary>Gets a color with RGBA values (128, 128, 128, 255) - Gray.</summary>
    public static Color Gray => new(128, 128, 128);

    /// <summary>Gets a color with RGBA values (192, 192, 192, 255) - Light Gray.</summary>
    public static Color LightGray => new(192, 192, 192);

    /// <summary>Gets a color with RGBA values (64, 64, 64, 255) - Dark Gray.</summary>
    public static Color DarkGray => new(64, 64, 64);

    /// <summary>Gets a color with RGBA values (220, 220, 220, 255) - Gainsboro.</summary>
    public static Color Gainsboro => new(220, 220, 220);

    /// <summary>Gets a color with RGBA values (211, 211, 211, 255) - Light Gray (web).</summary>
    public static Color LightGrey => new(211, 211, 211);

    /// <summary>Gets a color with RGBA values (105, 105, 105, 255) - Dim Gray.</summary>
    public static Color DimGray => new(105, 105, 105);

    #endregion

    #region Web Colors

    /// <summary>Gets a color with RGBA values (139, 0, 0, 255) - Dark Red.</summary>
    public static Color DarkRed => new(139, 0, 0);

    /// <summary>Gets a color with RGBA values (0, 100, 0, 255) - Dark Green.</summary>
    public static Color DarkGreen => new(0, 100, 0);

    /// <summary>Gets a color with RGBA values (0, 0, 139, 255) - Dark Blue.</summary>
    public static Color DarkBlue => new(0, 0, 139);

    /// <summary>Gets a color with RGBA values (255, 20, 147, 255) - Deep Pink.</summary>
    public static Color DeepPink => new(255, 20, 147);

    /// <summary>Gets a color with RGBA values (255, 192, 203, 255) - Pink.</summary>
    public static Color Pink => new(255, 192, 203);

    /// <summary>Gets a color with RGBA values (255, 165, 0, 255) - Orange.</summary>
    public static Color Orange => new(255, 165, 0);

    /// <summary>Gets a color with RGBA values (128, 0, 128, 255) - Purple.</summary>
    public static Color Purple => new(128, 0, 128);

    /// <summary>Gets a color with RGBA values (165, 42, 42, 255) - Brown.</summary>
    public static Color Brown => new(165, 42, 42);

    /// <summary>Gets a color with RGBA values (0, 128, 0, 255) - Web Green.</summary>
    public static Color WebGreen => new(0, 128, 0);

    /// <summary>Gets a color with RGBA values (128, 128, 0, 255) - Olive.</summary>
    public static Color Olive => new(128, 128, 0);

    /// <summary>Gets a color with RGBA values (0, 128, 128, 255) - Teal.</summary>
    public static Color Teal => new(0, 128, 128);

    /// <summary>Gets a color with RGBA values (128, 0, 0, 255) - Maroon.</summary>
    public static Color Maroon => new(128, 0, 0);

    /// <summary>Gets a color with RGBA values (0, 0, 128, 255) - Navy.</summary>
    public static Color Navy => new(0, 0, 128);

    #endregion

    #region Nature Colors

    /// <summary>Gets a color with RGBA values (124, 252, 0, 255) - Lawn Green.</summary>
    public static Color LawnGreen => new(124, 252, 0);

    /// <summary>Gets a color with RGBA values (50, 205, 50, 255) - Lime Green.</summary>
    public static Color LimeGreen => new(50, 205, 50);

    /// <summary>Gets a color with RGBA values (34, 139, 34, 255) - Forest Green.</summary>
    public static Color ForestGreen => new(34, 139, 34);

    /// <summary>Gets a color with RGBA values (143, 188, 143, 255) - Dark Sea Green.</summary>
    public static Color DarkSeaGreen => new(143, 188, 143);

    /// <summary>Gets a color with RGBA values (135, 206, 235, 255) - Sky Blue.</summary>
    public static Color SkyBlue => new(135, 206, 235);

    /// <summary>Gets a color with RGBA values (70, 130, 180, 255) - Steel Blue.</summary>
    public static Color SteelBlue => new(70, 130, 180);

    /// <summary>Gets a color with RGBA values (65, 105, 225, 255) - Royal Blue.</summary>
    public static Color RoyalBlue => new(65, 105, 225);

    /// <summary>Gets a color with RGBA values (240, 248, 255, 255) - Alice Blue.</summary>
    public static Color AliceBlue => new(240, 248, 255);

    #endregion

    #region Warm Colors

    /// <summary>Gets a color with RGBA values (255, 69, 0, 255) - Orange Red.</summary>
    public static Color OrangeRed => new(255, 69, 0);

    /// <summary>Gets a color with RGBA values (255, 140, 0, 255) - Dark Orange.</summary>
    public static Color DarkOrange => new(255, 140, 0);

    /// <summary>Gets a color with RGBA values (255, 215, 0, 255) - Gold.</summary>
    public static Color Gold => new(255, 215, 0);

    /// <summary>Gets a color with RGBA values (255, 218, 185, 255) - Peach Puff.</summary>
    public static Color PeachPuff => new(255, 218, 185);

    /// <summary>Gets a color with RGBA values (240, 128, 128, 255) - Light Coral.</summary>
    public static Color LightCoral => new(240, 128, 128);

    /// <summary>Gets a color with RGBA values (250, 128, 114, 255) - Salmon.</summary>
    public static Color Salmon => new(250, 128, 114);

    /// <summary>Gets a color with RGBA values (255, 160, 122, 255) - Light Salmon.</summary>
    public static Color LightSalmon => new(255, 160, 122);

    #endregion

    #region Cool Colors

    /// <summary>Gets a color with RGBA values (173, 216, 230, 255) - Light Blue.</summary>
    public static Color LightBlue => new(173, 216, 230);

    /// <summary>Gets a color with RGBA values (176, 196, 222, 255) - Light Steel Blue.</summary>
    public static Color LightSteelBlue => new(176, 196, 222);

    /// <summary>Gets a color with RGBA values (175, 238, 238, 255) - Pale Turquoise.</summary>
    public static Color PaleTurquoise => new(175, 238, 238);

    /// <summary>Gets a color with RGBA values (102, 205, 170, 255) - Medium Aquamarine.</summary>
    public static Color MediumAquamarine => new(102, 205, 170);

    /// <summary>Gets a color with RGBA values (64, 224, 208, 255) - Turquoise.</summary>
    public static Color Turquoise => new(64, 224, 208);

    /// <summary>Gets a color with RGBA values (0, 206, 209, 255) - Dark Turquoise.</summary>
    public static Color DarkTurquoise => new(0, 206, 209);

    /// <summary>Gets a color with RGBA values (95, 158, 160, 255) - Cadet Blue.</summary>
    public static Color CadetBlue => new(95, 158, 160);

    #endregion

    #region Neutral/Earthy Colors

    /// <summary>Gets a color with RGBA values (245, 245, 220, 255) - Beige.</summary>
    public static Color Beige => new(245, 245, 220);

    /// <summary>Gets a color with RGBA values (222, 184, 135, 255) - Burlywood.</summary>
    public static Color Burlywood => new(222, 184, 135);

    /// <summary>Gets a color with RGBA values (210, 180, 140, 255) - Tan.</summary>
    public static Color Tan => new(210, 180, 140);

    /// <summary>Gets a color with RGBA values (244, 164, 96, 255) - Sandy Brown.</summary>
    public static Color SandyBrown => new(244, 164, 96);

    /// <summary>Gets a color with RGBA values (160, 82, 45, 255) - Saddle Brown.</summary>
    public static Color SaddleBrown => new(160, 82, 45);

    /// <summary>Gets a color with RGBA values (205, 133, 63, 255) - Peru.</summary>
    public static Color Peru => new(205, 133, 63);

    /// <summary>Gets a color with RGBA values (139, 69, 19, 255) - Saddle Brown Dark.</summary>
    public static Color SaddleBrownDark => new(139, 69, 19);

    #endregion

    #region Violet/Purple Spectrum

    /// <summary>Gets a color with RGBA values (138, 43, 226, 255) - Blue Violet.</summary>
    public static Color BlueViolet => new(138, 43, 226);

    /// <summary>Gets a color with RGBA values (147, 112, 219, 255) - Medium Purple.</summary>
    public static Color MediumPurple => new(147, 112, 219);

    /// <summary>Gets a color with RGBA values (221, 160, 221, 255) - Plum.</summary>
    public static Color Plum => new(221, 160, 221);

    /// <summary>Gets a color with RGBA values (218, 112, 214, 255) - Orchid.</summary>
    public static Color Orchid => new(218, 112, 214);

    /// <summary>Gets a color with RGBA values (186, 85, 211, 255) - Medium Orchid.</summary>
    public static Color MediumOrchid => new(186, 85, 211);

    /// <summary>Gets a color with RGBA values (153, 50, 204, 255) - Dark Orchid.</summary>
    public static Color DarkOrchid => new(153, 50, 204);

    /// <summary>Gets a color with RGBA values (148, 0, 211, 255) - Dark Violet.</summary>
    public static Color DarkViolet => new(148, 0, 211);

    /// <summary>Gets a color with RGBA values (75, 0, 130, 255) - Indigo.</summary>
    public static Color Indigo => new(75, 0, 130);

    #endregion
}
