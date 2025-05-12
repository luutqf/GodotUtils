using Godot;
using System;
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

    // Return type of void was used here intentionally
    public static async void TryRun(this Func<Task> task)
    {
        if (task != null)
        {
            try
            {
                await task();
            }
            catch (Exception e)
            {
                GD.PrintErr($"Error: {e}");
            }
        }
    }
}
