#if DEBUG
using Godot;

namespace GodotUtils.Debugging.Visualize;

public partial class ExampleScene : Node
{
    [Export] private int _cameraSpeed = 5;

    private Camera2D _camera;

    public override void _Ready()
    {
        _camera = GetNode<Camera2D>("Camera2D");

        VisualizeExampleSprite sprite = VisualizeExampleSprite.Instantiate();

        // As you can see the visualize info is created at the moment of node creation
        _ = new GTween(this)
            .Delay(0.1)
            .Callback(() =>
            {
                AddChild(sprite);
            });
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 dir = Input.GetVector(InputActions.MoveLeft, InputActions.MoveRight, InputActions.MoveUp, InputActions.MoveDown);
        
        _camera.Position += dir * _cameraSpeed;
    }
}
#endif
