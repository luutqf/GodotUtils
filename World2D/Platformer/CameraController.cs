namespace GodotUtils.World2D.Platformer;

using Godot;

public partial class CameraController : Camera2D
{
    //每次滚轮操作改变的缩放量。
    float zoomIncrement = 0.08f;
    
    //定义摄像机可以缩放的最小和最大值。
    float minZoom = 1.5f;
    float maxZoom = 3.0f;
    
    //缩放操作时平滑因子，用于插值。
    float smoothFactor = 0.25f;
    
    //摄像机横向移动的速度。
    float horizontalPanSpeed = 8;

    //目标缩放值，用于在缩放功能中插值到的缩放级别。
    float targetZoom;

    public override void _Ready()
    {
        // Set the initial target zoom value on game start
        //在游戏开始时初始化targetZoom为当前摄像机的缩放水平。
        targetZoom = base.Zoom.X;
    }

    //处理物理帧更新。在这里调用平移、缩放和边界约束方法。
    public override void _PhysicsProcess(double delta)
    {
        float cameraWidth = GetViewportRect().Size.X / base.Zoom.X;
        float camLeftPos = Position.X - (cameraWidth / 2);
        float camRightPos = Position.X + (cameraWidth / 2);

        Panning(camLeftPos, camRightPos);
        Zooming();
        Boundaries(camLeftPos, camRightPos);
    }

    //处理输入事件。当检测到输入时，调用InputZoom方法，处理缩放操作。
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
            InputEventMouseButton(mouseEvent);

        @event.Dispose(); // Object count was increasing a lot when this function was executed 当执行此函数时，对象计数增加了很多
    }

    //根据用户输入，计算摄像机的平移动作，左或右移动。此方法防止相机超过设定的左右界限。
    void Panning(float camLeftPos, float camRightPos)
    {
        if (Input.IsActionPressed("move_left"))
        {
            // Prevent the camera from going too far left
            if (camLeftPos > LimitLeft)
                Position -= new Vector2(horizontalPanSpeed, 0);
        }

        if (Input.IsActionPressed("move_right"))
        {
            // Prevent the camera from going too far right
            if (camRightPos < LimitRight)
                Position += new Vector2(horizontalPanSpeed, 0);
        }
    }

    //使用Lerp方法平滑地插值到targetZoom，以实现平滑的缩放效应。
    void Zooming()
    {
        // Lerp to the target zoom for a smooth effect
        Zoom = Zoom.Lerp(new Vector2(targetZoom, targetZoom), smoothFactor);
    }

    // 控制摄像机，防止它超出左右边界。
    void Boundaries(float camLeftPos, float camRightPos)
    {
        if (camLeftPos < LimitLeft)
        {
            // Travelled this many pixels too far
            float gapDifference = Mathf.Abs(camLeftPos - LimitLeft);

            // Correct position
            Position += new Vector2(gapDifference, 0);
        }

        if (camRightPos > LimitRight)
        {
            // Travelled this many pixels too far
            float gapDifference = Mathf.Abs(camRightPos - LimitRight);

            // Correct position
            Position -= new Vector2(gapDifference, 0);
        }
    }

    //是一个处理鼠标缩放输入的辅助方法。
    void InputEventMouseButton(InputEventMouseButton @event)
    {
        InputZoom(@event);
    }

    //根据鼠标滚轮事件，计算摄像机的目标缩放值，并更新targetZoom。
    void InputZoom(InputEventMouseButton @event)
    {
        // Not sure why or if this is required
        if (!@event.IsPressed())
            return;

        // Zoom in
        if (@event.ButtonIndex == MouseButton.WheelUp)
            targetZoom += zoomIncrement;

        // Zoom out
        if (@event.ButtonIndex == MouseButton.WheelDown)
            targetZoom -= zoomIncrement;

        // Clamp the zoom
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }
}
