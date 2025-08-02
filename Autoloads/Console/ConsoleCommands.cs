using GodotUtils;
using System.Collections.Generic;
using System.Linq;

namespace GodotUtils.UI.Console;

public partial class GameConsole
{
    [ConsoleCommand("help")]
    private void Help()
    {
        IEnumerable<string> cmds = Commands.Select(x => x.Name);

        Logger.Instance.Log(cmds.ToFormattedString());
    }

    [ConsoleCommand("quit", "exit")]
    private async void Quit()
    {
        await GetNode<Global>(AutoloadPaths.Global).QuitAndCleanup();
    }

    [ConsoleCommand("debug")]
    private void Debug(int x)
    {
        Logger.Instance.Log(x);
    }
}
