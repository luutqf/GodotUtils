namespace GodotUtils;

using Godot;

public partial class GLabel : Label
{
    /// <summary>
    /// 文本对齐方式设置为水平和垂直居中
    /// </summary>
    /// <param name="text"></param>
    /// <param name="fontSize"></param>
    public GLabel(string text = "", int fontSize = 16)
    {
        Text = text;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        SetFontSize(fontSize);
    }

    /// <summary>
    /// 利用 SelfModulate 属性将标签的颜色设置为完全透明（颜色值为白色，透明度为 0）。这样标签的文本和背景将不再可见。
    /// </summary>
    public void SetTransparent() => SelfModulate = new Color(1, 1, 1, 0);
    
    /// <summary>
    /// 覆盖主题,设置字体大小
    /// </summary>
    /// <param name="v"></param>
    public void SetFontSize(int v) => AddThemeFontSizeOverride("font_size", v);
}
