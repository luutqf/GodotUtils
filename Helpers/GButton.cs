using Godot;

namespace GodotUtils;

public partial class GButton
{
    public Button Internal { get; } = new();

    public GButton(Node parent, string text)
    {
        Internal.Text = text;

        parent.AddChild(Internal);
    }
}
