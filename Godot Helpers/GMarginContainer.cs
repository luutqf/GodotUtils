namespace GodotUtils;

using Godot;

public partial class GMarginContainer : MarginContainer
{
    /// <summary>
    /// 传入一个 padding 值来为容器的所有四个边（左、右、顶、底）设置同样的边距。默认情况下，padding 的值为 5。这通过调用 SetMarginAll(5) 方法来实现。
    /// </summary>
    /// <param name="padding"></param>
    public GMarginContainer(int padding = 5) => SetMarginAll(5);
    
    /// <summary>
    ///
    /// 分别设置边距
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <param name="top"></param>
    /// <param name="bottom"></param>
    public GMarginContainer(int left, int right, int top, int bottom)
    {
        SetMarginLeft(left);
        SetMarginRight(right);
        SetMarginTop(top);
        SetMarginBottom(bottom);
    }

    /// <summary>
    /// 为容器的所有四个边界（左、右、顶、底）设置相同的边距。此方法通过循环遍历一个包含四个方向的字符串数组，并为每个方向调用 AddThemeConstantOverride 方法来实现边距设置。
    /// </summary>
    /// <param name="padding"></param>
    public void SetMarginAll(int padding)
    {
        foreach (string margin in new string[] { "left", "right", "top", "bottom" })
            AddThemeConstantOverride($"margin_{margin}", padding);
    }

    public void SetMarginLeft(int padding) =>
        AddThemeConstantOverride("margin_left", padding);

    public void SetMarginRight(int padding) =>
        AddThemeConstantOverride("margin_right", padding);

    public void SetMarginTop(int padding) =>
        AddThemeConstantOverride("margin_top", padding);

    public void SetMarginBottom(int padding) =>
        AddThemeConstantOverride("margin_bottom", padding);
}
