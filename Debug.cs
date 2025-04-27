using System;

namespace GodotUtils;

internal static class Debug
{
    internal static void Assert(bool condition, string message)
    {
#if DEBUG
        if (condition)
            return;

        throw new Exception(message);
#endif
    }
}
