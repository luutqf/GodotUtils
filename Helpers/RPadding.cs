using Godot;

namespace RedotUtils;

public partial class RPadding : Control
{
    public RPadding(int paddingX = 0, int paddingY = 0)
    {
        CustomMinimumSize = new Vector2(paddingX, paddingY);
    }
}

