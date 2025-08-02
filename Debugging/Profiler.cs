using Godot;
using System;
using System.Collections.Generic;

namespace GodotUtils;

public static class Profiler
{
    private static Dictionary<string, ProfilerEntry> _entries = [];

    public static void Start(string key)
    {
        if (!_entries.TryGetValue(key, out ProfilerEntry entry))
        {
            entry = new ProfilerEntry();
            _entries[key] = entry;
        }

        entry.Start();
    }

    public static void Stop(string key)
    {
        ProfilerEntry entry = _entries[key];

        // Measure in microseconds, convert to milliseconds
        ulong elapsedUsec = Time.GetTicksUsec() - entry.StartTimeUsec;
        ulong elapsedMs = elapsedUsec / 1000UL;

        GD.Print($"{key} {elapsedMs} ms");
        entry.Reset();
    }

    public static void OneShot(string key, Action code)
    {
        Start(key);
        code();
        Stop(key);
    }

    public static void AverageMs(string key, Action code)
    {
        if (!_entries.TryGetValue(key, out ProfilerEntry entry))
        {
            entry = new ProfilerEntry();
            _entries[key] = entry;

            MetricsOverlay.StartTracking(key, () => $"{_entries[key].GetAverageMs():F2} ms");
        }

        entry.Start();
        code();
        entry.Stop();
    }
}

public class ProfilerEntry
{
    public ulong StartTimeUsec { get; private set; }
    public ulong AccumulatedTimeUsec { get; private set; }
    public int FrameCount { get; private set; }

    public void Start()
    {
        StartTimeUsec = Time.GetTicksUsec();
    }

    public void Stop()
    {
        AccumulatedTimeUsec += Time.GetTicksUsec() - StartTimeUsec;
        FrameCount++;
    }

    public void Reset()
    {
        AccumulatedTimeUsec = 0UL;
        FrameCount = 0;
    }

    public double GetAverageMs()
    {
        return (double)AccumulatedTimeUsec / FrameCount / 1000.0;
    }
}
