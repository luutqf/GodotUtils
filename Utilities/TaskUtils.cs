using Godot;
using System.Threading.Tasks;

namespace GodotUtils;

public static class TaskUtils
{
    public static void FireAndForget(this Task task)
    {
        task.ContinueWith(t =>
        {
            GD.PrintErr($"Error: {t.Exception}");
        }, TaskContinuationOptions.OnlyOnFaulted);
    }
}
