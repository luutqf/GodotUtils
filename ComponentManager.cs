using Godot;
using System.Collections.Generic;

namespace GodotUtils;

/// <summary>
/// Using any kind of Godot functions from C# is expensive, so we try to minimize this with centralized logic.
/// See <see href="https://www.reddit.com/r/godot/comments/1me7669/a_follow_up_to_my_first_c_stress_test/">stress test results</see>.
/// </summary>
[GlobalClass]
public partial class ComponentManager : Node
{
    private List<Component> _ready          = [];
    private List<Component> _process        = [];
    private List<Component> _physicsProcess = [];
    private List<Component> _unhandledInput = [];
    private List<Component> _input          = [];

    // Disable overrides on startup
    public override void _EnterTree()
    {
        SetProcess(false);
        SetPhysicsProcess(false);
        SetProcessInput(false);
        SetProcessUnhandledInput(false);
    }

    // Handle Godot overrides
    public override void _Ready()
    {
        foreach (Component component in _ready)
        {
            component.Ready();
        }
    }

    public override void _Process(double delta)
    {
        foreach (Component component in _process)
        {
            component.Process(delta);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach (Component component in _physicsProcess)
        {
            component.PhysicsProcess(delta);
        }
    }

    public override void _Input(InputEvent @event)
    {
        foreach (Component component in _input)
        {
            component.ProcessInput(@event);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        foreach (Component component in _unhandledInput)
        {
            component.UnhandledInput(@event);
        }
    }

    // Exposed register functions
    public void RegisterReady(Component component)
    {
        _ready.Add(component);
    }

    public void RegisterProcess(Component component)
    {
        _process.Add(component);
        SetProcess(true);
    }

    public void RegisterPhysicsProcess(Component component)
    {
        _physicsProcess.Add(component);
        SetPhysicsProcess(true);
    }

    public void RegisterInput(Component component)
    {
        _input.Add(component);
        SetProcessInput(true);
    }

    public void RegisterUnhandledInput(Component component)
    {
        _unhandledInput.Add(component);
        SetProcessUnhandledInput(true);
    }
}
