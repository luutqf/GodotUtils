using Godot;
using System.Collections.Generic;

namespace GodotUtils.Debugging;

/// <summary>
/// A static class providing simple profiling utilities for measuring elapsed time of code blocks.
/// </summary>
public static class Profiler
{
    private static Dictionary<string, ProfilerEntry> _entries = [];

    /// <summary>
    /// Starts a one-shot profiling measurement for the given key.
    /// This is intended for profiling a single code block execution.
    /// </summary>
    public static void Start(string key)
    {
        if (!_entries.TryGetValue(key, out ProfilerEntry entry))
        {
            entry = new ProfilerEntry();
            _entries[key] = entry;
        }

        entry.Start();
    }

    /// <summary>
    /// Stops a one-shot profiling measurement for the given key.
    /// Calculates and prints the elapsed time in milliseconds to the console.
    /// </summary>
    public static void Stop(string key)
    {
        ProfilerEntry entry = _entries[key];

        // Measure in microseconds, convert to milliseconds
        ulong elapsedUsec = Time.GetTicksUsec() - entry.StartTimeUsec;
        ulong elapsedMs = elapsedUsec / 1000UL;

        GD.Print($"{key} {elapsedMs} ms");
        entry.Reset();
    }

    /// <summary>
    /// Starts a frame-based profiling measurement for the given key.
    /// This is intended for continuously tracking time across multiple frames.
    /// The key is displayed in the MetricsOverlay.
    /// </summary>
    public static void StartFrame(string key)
    {
        if (!_entries.TryGetValue(key, out ProfilerEntry entry))
        {
            entry = new ProfilerEntry();
            _entries[key] = entry;

            MetricsOverlay.StartTracking(key, () => $"{_entries[key].GetAverageMs():F2} ms");
        }

        entry.Start();
    }

    /// <summary>
    /// Stops a frame-based profiling measurement for the given key.
    /// </summary>
    public static void StopFrame(string key)
    {
        _entries[key].Stop();
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
