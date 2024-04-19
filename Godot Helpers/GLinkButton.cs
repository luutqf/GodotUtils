namespace GodotUtils;

using Godot;

public partial class GLinkButton : LinkButton
{
    public GLinkButton(string text, int fontSize = 16)
    {
        Text = text;
        Uri = text;
        //按钮在垂直方向上缩小并居中。
        SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
        SetFontSize(fontSize);
    }

    public void SetFontSize(int v) => AddThemeFontSizeOverride("font_size", v);
}
