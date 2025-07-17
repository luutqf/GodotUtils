using Godot;

namespace GodotUtils;

public partial class GLinkButton
{
    public LinkButton Internal { get; } = new();

    public GLinkButton(Node parent, string text, int fontSize = 16)
    {
        Internal.Text = text;
        Internal.Uri = text;
        Internal.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
        SetFontSize(fontSize);

        parent.AddChild(Internal);
    }

    public void SetFontSize(int v)
    {
        Internal.AddThemeFontSizeOverride("font_size", v);
    }
}

