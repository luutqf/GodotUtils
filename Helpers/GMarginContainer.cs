using Godot;

namespace GodotUtils;

public partial class GMarginContainer
{
    public MarginContainer Internal { get; } = new();

    public GMarginContainer(Node parent, int padding = 5) : this(parent, padding, padding, padding, padding) { }

    public GMarginContainer(Node parent, int left, int right, int top, int bottom)
    {
        SetMarginLeft(left);
        SetMarginRight(right);
        SetMarginTop(top);
        SetMarginBottom(bottom);

        parent.AddChild(Internal);
    }

    public void SetMarginLeft(int padding)
    {
        Internal.AddThemeConstantOverride("margin_left", padding);
    }

    public void SetMarginRight(int padding)
    {
        Internal.AddThemeConstantOverride("margin_right", padding);
    }

    public void SetMarginTop(int padding)
    {
        Internal.AddThemeConstantOverride("margin_top", padding);
    }

    public void SetMarginBottom(int padding)
    {
        Internal.AddThemeConstantOverride("margin_bottom", padding);
    }
}
