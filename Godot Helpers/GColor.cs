namespace GodotUtils;

using Godot;

public class GColor
{
    /// <summary>
    /// <para>Create a color from HSV (Hue, Saturation, Value)</para>
    /// <para>'hue' - values range from 0 to 359</para>
    /// <para>'saturation' - values range from 0 to 100</para>
    /// <para>'value' - values range from 0 to 100</para>
    /// <para>'alpha' - values range from 0 to 255</para>
    /// 这个方法允许你使用 HSV 值来创建一个 Color 对象。其中，
    /// hue 参数的值范围从 0 到 359，表示颜色的色相。
    /// saturation 参数的值范围从 0 到 100，表示颜色的饱和度，如果不提供，默认值为 100。
    /// value 参数的值范围从 0 到 100，表示颜色的明度，如果不提供，默认值为 100。
    /// alpha 参数的值范围从 0 到 255，表示颜色的透明度，如果不提供，默认值为 255（不透明）。
    /// 该方法将 HSV 值转换为浮点数表示（例如，将 hue 除以 359），然后使用 Godot 中的 Color.FromHsv 方法来创建并返回一个 Color 对象。
    /// </summary>
    public static Color FromHSV(int hue, int saturation = 100, int value = 100, int alpha = 255)
    {
        return Color.FromHsv(hue / 359f, saturation / 100f, value / 100f, alpha / 255f);
    }

    /// <summary>
    /// Generate a random color
    /// 随机RGB
    /// </summary>
    public static Color Random(int alpha = 255)
    {
        float r = GU.RandRange(0.0, 1.0);
        float g = GU.RandRange(0.0, 1.0);
        float b = GU.RandRange(0.0, 1.0);

        return new Color(r, g, b, alpha / 255f);
    }
}
