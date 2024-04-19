namespace GodotUtils;

using Godot;

/// <summary>
/// 这段代码为 Godot 游戏引擎中的 InputEventKey 类型扩展了三个方法，使得处理键盘输入事件时更加方便。
/// 在 Godot 中，InputEventKey 代表一个键盘事件，这些扩展方法增加了检测特定键是否刚被按下或释放，以及将键盘输入转换为易于阅读的字符串的功能。
/// </summary>
public static class ExtensionsInputEventKey
{
    /// <summary>
    /// 检查特定的键是否刚刚被按下。它检查 InputEventKey 对象的 Keycode 是否等于指定的 key，
    /// Pressed 属性是否为 true，并且确保事件不是重复的按键事件（即 Echo 为 false）。
    /// 如果这些条件都满足，方法返回 true。
    /// </summary>
    /// <param name="v"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool IsJustPressed(this InputEventKey v, Key key) =>
        v.Keycode == key && v.Pressed && !v.Echo;

    /// <summary>
    /// 检查特定的键是否刚刚被释放。它检查 InputEventKey 对象的 Keycode 是否等于指定的 key，
    /// Pressed 属性是否为 false，并确认不是重复的按键事件（即 Echo 为 false）。
    /// 如果这些条件都满足，方法返回 true。
    /// </summary>
    /// <param name="v"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool IsJustReleased(this InputEventKey v, Key key) =>
        v.Keycode == key && !v.Pressed && !v.Echo;

    /// <summary>
    /// 将 InputEventKey 对象转换成人类可读的键盘按键字符串，例如 "Ctrl + Shift + E"。
    /// 如果 Keycode 属性没有设置（即为 Key.None），方法会使用 GetPhysicalKeycodeWithModifiers() 来获取物理按键码及其修饰键；
    /// 否则，使用 GetKeycodeWithModifiers() 获取按键码和修饰键。最后，使用 OS.GetKeycodeString 方法将按键码转换为字符串，
    /// 并把其中的 "+" 替换成 " + " 以提高可读性。
    /// <para>Convert to a human readable key</para>
    /// <para>For example 'Ctrl + Shift + E'</para>
    /// </summary>
    public static string Readable(this InputEventKey v)
    {
        // If Keycode is not set than use PhysicalKeycode
        //如果没有设置Keycode，则使用PhysicalKeycode
        Key keyWithModifiers =
            v.Keycode == Key.None ? v.GetPhysicalKeycodeWithModifiers() : v.GetKeycodeWithModifiers();

        return OS.GetKeycodeString(keyWithModifiers).Replace("+", " + ");
    }
}
