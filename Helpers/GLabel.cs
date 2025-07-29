using Godot;

namespace GodotUtils;

public partial class GLabel
{
    public Label Internal { get; } = new();

    public GLabel(Node parent, string text = "", int fontSize = 16)
    {
        Internal.Text = text;
        Internal.HorizontalAlignment = HorizontalAlignment.Center;
        Internal.VerticalAlignment = VerticalAlignment.Center;
        SetFontSize(fontSize);

        parent.AddChild(Internal);
    }

    public GLabel SetTransparent()
    {
        Internal.SelfModulate = new Color(1, 1, 1, 0);
        return this;
    }

    public GLabel SetFontSize(int v)
    {
        Internal.AddThemeFontSizeOverride("font_size", v);
        return this;
    }
}
