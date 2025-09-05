namespace Chevron9.Core.Render;

public readonly record struct RenderItem(
    int LayerZ,
    int MaterialKey,
    int SortKey,
    RenderCommand Command
);
