using Godot;

namespace RedotUtils;

public partial class RLinkButton : LinkButton
{
    public RLinkButton(string text, int fontSize = 16)
    {
        Text = text;
        Uri = text;
        SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
        SetFontSize(fontSize);
    }

    public void SetFontSize(int v)
    {
        AddThemeFontSizeOverride("font_size", v);
    }
}

