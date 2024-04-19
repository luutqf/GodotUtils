namespace GodotUtils;

using Godot;

public static class ExtensionsMath
{
    /// <summary>
    /// Lerp 方法用于在两种颜色之间进行线性插值。这是通过对两个 Color 对象进行加权相加来实现的，其中 t 是插值参数，其范围通常在 0 到 1 之间。
    /// </summary>
    /// <param name="color1"></param>
    /// <param name="color2"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Color Lerp(this Color color1, Color color2, float t) =>
        color1 * (1 - t) + color2 * t;

    /// <summary>
    /// Remap 方法用来重新映射一个浮点数 value 从一个范围 (from1 到 to1) 到另一个范围 (from2 到 to2)。
    /// 这对于比如将速度、音量和其他可以量化的属性映射到不同的操作很有用。
    /// </summary>
    /// <param name="value"></param>
    /// <param name="from1"></param>
    /// <param name="to1"></param>
    /// <param name="from2"></param>
    /// <param name="to2"></param>
    /// <returns></returns>
    public static float Remap(this float value, float from1, float to1, float from2, float to2) =>
        (value - from1) / (to1 - from1) * (to2 - from2) + from2;

    /// <summary>
    /// LerpRotationToTarget 方法对 Sprite2D 对象的旋转属性进行插值，使其朝向一个目标位置 target。这通常用于使物体面向移动方向或指向特定目标。
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="target"></param>
    /// <param name="t"></param>
    public static void LerpRotationToTarget(this Sprite2D sprite, Vector2 target, float t = 0.1f) =>
        sprite.Rotation = Mathf.LerpAngle(sprite.Rotation, (target - sprite.GlobalPosition).Angle(), t);

    /// <summary>
    /// 这两个方法将角度与弧度之间相互转换。ToRadians 将度数转换为弧度，而 ToDegrees 将弧度转换为度数。
    /// </summary>
    /// <param name="degrees"></param>
    /// <returns></returns>
    public static float ToRadians(this float degrees) => degrees * (Mathf.Pi / 180);
    public static float ToDegrees(this float radians) => radians * (180 / Mathf.Pi);

    /// <summary>
    /// 有针对整型 int 和浮点型 float 两种重载版本的 Clamp 方法，它们限制一个值的大小在指定的最小值 min 和最大值 max 之间。
    /// </summary>
    /// <param name="v"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Clamp(this int v, int min, int max) => Mathf.Clamp(v, min, max);
    public static float Clamp(this float v, float min, float max) => Mathf.Clamp(v, min, max);
    
    /// <summary>
    /// 与 Color 的 Lerp 方法类似，float 类型的 Lerp 方法用于在两个浮点数之间进行线性插值。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float Lerp(this float a, float b, float t) => Mathf.Lerp(a, b, t);

    /// <summary>
    /// Pulse 方法根据时间 time 及指定的频率 frequency 生成一个周期性脉冲值，该值在 0 和 1 之间循环波动。
    /// 这样的函数对于创建闪烁效果或周期性变化的动画特别有用。
    /// Pulses a value from 0 to 1 to 0 to 1 over time
    /// </summary>
    public static float Pulse(this float time, float frequency) =>
        0.5f * (1 + Mathf.Sin(2 * Mathf.Pi * frequency * time));
}
