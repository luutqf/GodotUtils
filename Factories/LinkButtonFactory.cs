using Godot;

namespace GodotUtils;

public partial class LinkButtonFactory
{
    public static LinkButton Create(string text, int fontSize = 16)
    {
        LinkButton button = new()
        {
            SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
            Text = text,
            Uri = text
        };

        button.SetFontSize(fontSize);

        return button;
    }
}
