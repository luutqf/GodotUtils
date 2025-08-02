using GodotUtils.UI.Console;

namespace GodotUtils.UI;

// Autoload
public partial class LoggerManager : Component
{
    public static LoggerManager Instance { get; private set; }

    public Logger Logger { get; } = new();

    public override void Ready()
    {
        Instance = this;
        Logger.MessageLogged += GetNode<GameConsole>(AutoloadPaths.Console).AddMessage;
    }

    public override void PhysicsProcess(double delta)
    {
        Logger.Update();
    }
}
