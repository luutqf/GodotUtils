namespace GodotUtils;

using Godot;

public static class ExtensionsControl
{
    /// <summary>
    /// 完全覆盖其父节点的矩形区域
    /// </summary>
    /// <param name="control"></param>
    public static void CoverEntireRect(this Control control) =>
        control.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

    /// <summary>
    /// 将控件锚定点和偏移量设置为使控件在其父节点中居中
    /// </summary>
    /// <param name="control"></param>
    public static void CenterToScreen(this Control control) =>
        control.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.Center);
}
