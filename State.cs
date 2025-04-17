using System;

namespace GodotUtils;

public class State(string name = "")
{
    public Action Enter { get; set; } = () => { };
    public Action<double> Update { get; set; } = delta => { };
    public Action Exit { get; set; } = () => { };

    private string _name = name;

    public override string ToString()
    {
        return _name.ToLower();
    }
}

public interface IStateMachine
{
    void SwitchState(State newState);
}
