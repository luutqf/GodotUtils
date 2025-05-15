using Godot;

namespace GodotUtils;

public partial class GLinkButton : LinkButton
{
    public GLinkButton(string text, int fontSize = 16)
    {
        Text = text;
        Uri = text;
        SizeFlagsVertical = SizeFlags.ShrinkCenter;
        SetFontSize(fontSize);
    }

    public void SetFontSize(int v)
    {
        AddThemeFontSizeOverride("font_size", v);
    }
}

