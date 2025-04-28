using System;
using System.Diagnostics;

namespace GodotUtils;

// The whole reason I made this was to try and skip the first two frames of the stack trace but because Godot is doing something
// weird, it does not work and this code is useless but I will leave it here in case anyone else figures it out.
internal static class Debug
{
    [StackTraceHidden]
    internal static void Assert(bool condition, string message)
    {
#if DEBUG
        if (condition)
            return;

        throw new AssertException(message);
#endif
    }

    private class AssertException(string message) : Exception(message)
    {
        public override string StackTrace => new StackTrace(skipFrames: 2, true).ToString();
    }
}
