using Godot;

namespace RedotUtils;

public partial class RLabel : Label
{
    public RLabel(string text = "", int fontSize = 16)
    {
        Text = text;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        SetFontSize(fontSize);
    }

    public RLabel SetTransparent()
    {
        SelfModulate = new Color(1, 1, 1, 0);
        return this;
    }

    public RLabel SetFontSize(int v)
    {
        AddThemeFontSizeOverride("font_size", v);
        return this;
    }
}

