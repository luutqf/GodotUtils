namespace GodotUtils;

using Godot;

public partial class GPadding : Control
{
    /// <summary>
    /// 自定义最小尺寸的 X 和 Y 
    /// </summary>
    /// <param name="paddingX"></param>
    /// <param name="paddingY"></param>
    public GPadding(int paddingX = 0, int paddingY = 0)
    {
        CustomMinimumSize = new Vector2(paddingX, paddingY);
    }
}
