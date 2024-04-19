namespace GodotUtils;

using Godot;

public static class ExtensionsInputEventMouseButton
{
    public static bool IsWheelUp(this InputEventMouseButton @event) =>
        IsZoomIn(@event);

    public static bool IsWheelDown(this InputEventMouseButton @event) =>
        IsZoomOut(@event);

    /// <summary>
    /// Returns true if a mouse WheelUp event was detected
    /// IsZoomIn 方法用于检测鼠标滚轮是否向上滚动（即放大操作），而 IsWheelUp 方法则直接调用 IsZoomIn 方法作为别名，两者功能相同。
    /// 如果检测到向上滚动的事件，返回 true。
    /// </summary>
    public static bool IsZoomIn(this InputEventMouseButton @event) =>
        @event.IsPressed(MouseButton.WheelUp);

    /// <summary>
    /// IsZoomOut 方法用于检测鼠标滚轮是否向下滚动（即缩小操作），而 IsWheelDown 方法则直接调用 IsZoomOut 方法作为别名，两者功能相同。
    /// 如果检测到向下滚动的事件，返回 true。
    /// Returns true if a mouse WheelDown event was detected
    /// </summary>
    public static bool IsZoomOut(this InputEventMouseButton @event) =>
        @event.IsPressed(MouseButton.WheelDown);

    /// <summary>
    /// 这两个方法分别用于检测鼠标左键是否被按下和释放。如果检测到相应的事件，返回 true。
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public static bool IsLeftClickPressed(this InputEventMouseButton @event) =>
        @event.IsPressed(MouseButton.Left);

    public static bool IsLeftClickReleased(this InputEventMouseButton @event) =>
        @event.IsReleased(MouseButton.Left);

    /// <summary>
    /// 这两个方法分别用于检测鼠标右键是否被按下和释放。如果检测到相应的事件，返回 true。
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public static bool IsRightClickPressed(this InputEventMouseButton @event) =>
        @event.IsPressed(MouseButton.Right);

    public static bool IsRightClickReleased(this InputEventMouseButton @event) =>
        @event.IsReleased(MouseButton.Right);

    // Helper Functions
    //IsPressed 和 IsReleased 两个辅助性的私有方法用于简化按键状态的检测逻辑。
    //它们检测事件中的 ButtonIndex 是否和特定的 MouseButton 枚举值相匹配，并且根据 Pressed 属性的值判断按钮是被按下还是被释放。
    static bool IsPressed(this InputEventMouseButton @event, MouseButton button) =>
        @event.ButtonIndex == button && @event.Pressed;

    static bool IsReleased(this InputEventMouseButton @event, MouseButton button) =>
        @event.ButtonIndex == button && !@event.Pressed;
}
