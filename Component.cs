using Godot;
using System;

namespace GodotUtils;

public partial class Component : Node
{
    private ComponentManager _componentManager;

    public override void _EnterTree()
    {
        _componentManager = GetComponentManager();
        _componentManager.RegisterReady(this);
    }

    public virtual void Ready() { }
    public virtual void Process(double delta) { }
    public virtual void PhysicsProcess(double delta) { }
    public virtual void ProcessInput(InputEvent @event) { } // Named ProcessInput because conflicts with Input.(...)
    public virtual void UnhandledInput(InputEvent @event) { }

    public void RegisterReady()            => _componentManager.RegisterReady(this);
    public void RegisterProcess()          => _componentManager.RegisterProcess(this);
    public void RegisterPhysicsProcess()   => _componentManager.RegisterPhysicsProcess(this);
    public void RegisterInput()            => _componentManager.RegisterInput(this);
    public void RegisterUnhandledInput()   => _componentManager.RegisterUnhandledInput(this);
    public void UnregisterProcess()        => _componentManager.UnregisterProcess(this);
    public void UnregisterPhysicsProcess() => _componentManager.UnregisterPhysicsProcess(this);
    public void UnregisterInput()          => _componentManager.UnregisterInput(this);
    public void UnregisterUnhandledInput() => _componentManager.UnregisterUnhandledInput(this);

    private ComponentManager GetComponentManager()
    {
        Node current = GetParent();

        while (current != null)
        {
            if (current is ComponentManager manager)
            {
                return manager;
            }

            current = current.GetParent();
        }

        throw new InvalidOperationException($"ComponentManager not found for {Name}.");
    }
}
