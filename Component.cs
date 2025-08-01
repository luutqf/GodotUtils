using Godot;
using System;

namespace GodotUtils;

public partial class Component : Node
{
    public ComponentManager ComponentManager { get; private set; }

    public override void _EnterTree()
    {
        ComponentManager = GetComponentManager();
        ComponentManager.RegisterReady(this);
    }

    public virtual void Ready() { }
    public virtual void Process(double delta) { }
    public virtual void PhysicsProcess(double delta) { }
    public virtual void ProcessInput(InputEvent @event) { } // Named ProcessInput because conflicts with Input.(...)
    public virtual void UnhandledInput(InputEvent @event) { }

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
