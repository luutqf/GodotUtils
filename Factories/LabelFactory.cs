using Godot;

namespace GodotUtils;

public class LabelFactory
{
    public static Label Create(string text = "", int fontSize = 16)
    {
        Label label = new()
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        label.SetFontSize(fontSize);

        return label;
    }
}
