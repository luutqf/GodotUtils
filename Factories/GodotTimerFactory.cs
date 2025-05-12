using Godot;

namespace GodotUtils;

public class GodotTimerFactory
{
    public static Timer Create(double milliseconds, bool looping)
    {
        Timer timer = new()
        {
            ProcessCallback = Timer.TimerProcessCallback.Physics,
            WaitTime = milliseconds * 0.001, // Convert from milliseconds to seconds
            OneShot = !looping
        };
        
        return timer;
    }
}
