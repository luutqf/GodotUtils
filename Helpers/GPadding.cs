using Godot;

namespace GodotUtils;

public partial class GPadding
{
    public Control Internal { get; } = new();

    public GPadding(Node parent, int paddingX = 0, int paddingY = 0)
    {
        Internal.CustomMinimumSize = new Vector2(paddingX, paddingY);

        parent.AddChild(Internal);
    }
}
