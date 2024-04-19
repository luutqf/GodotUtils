namespace GodotUtils.World2D.TopDown;

using Godot;

/*
 * Attach this script to a child node of the Camera node
 * 将此脚本附加到Camera节点的子节点
 */
public partial class CameraController : Node
{
    // Inspector
    [Export]
    float speed = 100;

    //默认的缩放增量
    [ExportGroup("Zoom")]
    [Export(PropertyHint.Range, "0.02, 0.16")]
    float zoomIncrementDefault = 0.02f;

    [Export(PropertyHint.Range, "0.01, 10")]
    float minZoom = 0.01f;

    //摄像机能够进行的最小和最大缩放值。
    [Export(PropertyHint.Range, "0.1, 10")]
    float maxZoom = 1.0f;

    //缩放当中的线性插值的平滑系数。
    [Export(PropertyHint.Range, "0.01, 1")]
    float smoothFactor = 0.25f;

    //用于调节在每一帧中摄像机的缩放比例。
    float zoomIncrement = 0.02f;
    
    //目标缩放值。
    float targetZoom;

    // Panning 平移的初始位置。
    Vector2 initialPanPosition;
    
    //是否正在进行平移的标志。
    bool panning;
    
    //存储Camera2D类型的节点引用。
    Camera2D camera;

    public override void _Ready()
    {
        camera = GetParent<Camera2D>();

        // Make sure the camera zoom does not go past MinZoom
        // Note that a higher MinZoom value means the camera can zoom out more
        float maxZoom = Mathf.Max(camera.Zoom.X, minZoom);
        camera.Zoom = Vector2.One * maxZoom;

        // Set the initial target zoom value on game start
        targetZoom = camera.Zoom.X;
    }

    //在主循环的每一帧中执行。负责处理WASD/箭头键位的摄像机移动和处理平移。
    public override void _Process(double delta)
    {
        // Not sure if the below code should be in _PhysicsProcess or _Process

        // Arrow keys and WASD move camera around
        var dir = Vector2.Zero;

        if (GInput.IsMovingLeft())
            dir.X -= 1;

        if (GInput.IsMovingRight())
            dir.X += 1;

        if (GInput.IsMovingUp())
            dir.Y -= 1;

        if (GInput.IsMovingDown())
            dir.Y += 1;

        if (panning)
            camera.Position = initialPanPosition - (GetViewport().GetMousePosition() / camera.Zoom.X);

        // Arrow keys and WASD movement are added onto the panning position changes
        camera.Position += dir.Normalized() * speed;
    }

    //在物理帧中执行。控制缩放的增量（防止缩放速度过快）和执行平滑的缩放插值。
    public override void _PhysicsProcess(double delta)
    {
        // Prevent zoom from becoming too fast when zooming out
        zoomIncrement = zoomIncrementDefault * camera.Zoom.X;

        // Lerp to the target zoom for a smooth effect
        camera.Zoom = camera.Zoom.Lerp(new Vector2(targetZoom, targetZoom), smoothFactor);
    }

    // Not sure if this should be done in _Input or _UnhandledInput
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
            InputEventMouseButton(mouseButton);

        @event.Dispose(); // Object count was increasing a lot when this function was executed
    }

    void InputEventMouseButton(InputEventMouseButton @event)
    {
        HandlePan(@event);
        HandleZoom(@event);
    }

    //处理平移逻辑，当左键点击时开始平移，当释放时结束平移。
    void HandlePan(InputEventMouseButton @event)
    {
        // Left click to start panning the camera
        if (@event.ButtonIndex != MouseButton.Left)
            return;

        // Is this the start of a left click or is this releasing a left click?
        if (@event.IsPressed())
        {
            // Save the intial position
            initialPanPosition = camera.Position + (GetViewport().GetMousePosition() / camera.Zoom.X);
            panning = true;
        }
        else
            // Only stop panning once left click has been released
            panning = false;
    }

    //处理缩放逻辑，使用鼠标滚轮进行缩放，并且保证缩放值在指定的最小和最大范围内。
    void HandleZoom(InputEventMouseButton @event)
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
