using System.Reflection;

namespace GodotUtils.UI.Console;

public class ConsoleCommandInfo
{
    public string Name { get; set; }
    public string[] Aliases { get; set; }
    public MethodInfo Method { get; set; }
}
