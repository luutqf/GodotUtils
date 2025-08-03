using Godot;
using ImGuiNET;
using System;
using System.Collections.Generic;
using Monitor = Godot.Performance.Monitor;
using Vector2 = System.Numerics.Vector2;

namespace GodotUtils.Debugging;

public class MetricsOverlay
{
    private const int BytesInMegabyte     = 1048576;
    private const int BytesInKilobyte     = 1024;
    private const int MaxFpsBuffer        = 100;
    private const int WindowWidth         = 220;
    private const int FpsGraphWidthMargin = 15;
    private const int FpsGraphHeight      = 30;

    private const string ImGuiWindowName = "Metrics Overlay";
    private const string LabelMetrics    = "Metrics";
    private const string LabelVariables  = "Variables";
    private const string LabelFpsGraph   = "##FPSGraph"; // The ## hides the text

    private float[] _fpsBuffer = new float[MaxFpsBuffer];
    private float   _cachedFps;
    private int     _fpsIndex;

    private bool _visible;

    private static Dictionary<string, Func<object>> _trackingVariables = []; // Should this really be static?
    private Dictionary<string, Func<string>> _currentMetrics = [];

    public void Init()
    {
        Dictionary<string, (bool Enabled, Func<string> ValueProvider)> metrics = new()
        {
            { "FPS",                    (true,  () => $"{_cachedFps}") },
            { "Process",                (false, () => $"{Retrieve(Monitor.TimeProcess) * 1000:0.00} ms") },
            { "Physics Process",        (false, () => $"{Retrieve(Monitor.TimePhysicsProcess) * 1000:0.00} ms") },
            { "Navigation Process",     (false, () => $"{Retrieve(Monitor.TimeNavigationProcess) * 1000:0.00} ms") },
            { "Static Memory",          (true,  () => $"{Retrieve(Monitor.MemoryStatic) / BytesInMegabyte:0.0} MiB") },
            { "Static Memory Max",      (false, () => $"{Retrieve(Monitor.MemoryStaticMax) / BytesInMegabyte:0.0} MiB") },
            { "Video Memory",           (true,  () => $"{Retrieve(Monitor.RenderVideoMemUsed) / BytesInMegabyte:0.0} MiB") },
            { "Texture Memory",         (false, () => $"{Retrieve(Monitor.RenderTextureMemUsed) / BytesInMegabyte:0.0} MiB") },
            { "Buffer Memory",          (false, () => $"{Retrieve(Monitor.RenderBufferMemUsed) / BytesInMegabyte:0.0} MiB") },
            { "Message Buffer Max",     (false, () => $"{Retrieve(Monitor.MemoryMessageBufferMax) / BytesInKilobyte:0.0} KiB") },
            { "Resource Count",         (false, () => $"{Retrieve(Monitor.ObjectResourceCount)}") },
            { "Node Count",             (true,  () => $"{Retrieve(Monitor.ObjectNodeCount)}") },
            { "Orphan Node Count",      (true,  () => $"{Retrieve(Monitor.ObjectOrphanNodeCount)}") },
            { "Object Count",           (true,  () => $"{Retrieve(Monitor.ObjectCount)}") },
            { "Total Objects Drawn",    (false, () => $"{Retrieve(Monitor.RenderTotalObjectsInFrame)}") },
            { "Total Primitives Drawn", (false, () => $"{Retrieve(Monitor.RenderTotalPrimitivesInFrame)}") },
            { "Total Draw Calls",       (false, () => $"{Retrieve(Monitor.RenderTotalDrawCallsInFrame)}") },
        };

        // Perfect example of where var is useful...
        foreach (var metric in metrics)
        {
            if (metric.Value.Enabled)
            {
                _currentMetrics.Add(metric.Key, metric.Value.ValueProvider);
            }
        }
    }

    public void Update()
    {
        if (Input.IsActionJustPressed(InputActions.DebugOverlay))
        {
            _visible = !_visible;
        }

        if (_visible)
        {
            RenderPerformanceMetrics(_currentMetrics, _fpsBuffer, ref _fpsIndex, ref _cachedFps);
        }
    }

    public static void StartTracking(string key, Func<object> function)
    {
        _trackingVariables.Add(key, function);
    }

    public static void StopTracking(string key)
    {
        _trackingVariables.Remove(key);
    }

    private static void RenderPerformanceMetrics(Dictionary<string, Func<string>> metrics, float[] fpsBuffer, ref int fpsIndex, ref float cachedFps)
    {
        UpdateFpsBuffer(ref cachedFps, fpsBuffer, ref fpsIndex);

        SetupImGuiWindow();

        RenderMetrics(metrics, fpsBuffer, fpsIndex);
        RenderUserDefinedVariables();

        ImGui.End();
    }

    private static void SetupImGuiWindow()
    {
        // Print the window scale
        Godot.Vector2 size = DisplayServer.WindowGetSize();
        Vector2 screenSize = new(size.X, size.Y);
        Vector2 windowSize = new(WindowWidth, 0);
        Vector2 topRight = new Vector2(screenSize.X, 0) - new Vector2(windowSize.X, -ImGui.GetFrameHeight());

        ImGui.SetNextWindowPos(topRight, ImGuiCond.Always);
        ImGui.SetNextWindowSize(windowSize, ImGuiCond.Always);

        ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar;

        ImGui.Begin(ImGuiWindowName, flags);
    }

    private static void RenderMetrics(Dictionary<string, Func<string>> metrics, float[] fpsBuffer, int fpsIndex)
    {
        if (!ImGui.CollapsingHeader(LabelMetrics, ImGuiTreeNodeFlags.DefaultOpen))
            return;

        foreach ((string key, Func<string> valueProvider) in metrics)
        {
            ImGui.Text($"{key}: {valueProvider()}");

            if (key == "FPS")
            {
                RenderFpsGraph(fpsBuffer, fpsIndex);
            }
        }
    }

    private static void RenderUserDefinedVariables()
    {
        if (_trackingVariables.Count == 0 || !ImGui.CollapsingHeader(LabelVariables, ImGuiTreeNodeFlags.DefaultOpen))
            return;

        foreach (KeyValuePair<string, Func<object>> kvp in _trackingVariables)
        {
            string name = kvp.Key;
            string value = kvp.Value().ToString();

            ImGui.Text($"{name}: {value}");
        }
    }

    private static void UpdateFpsBuffer(ref float cachedFps, float[] fpsBuffer, ref int fpsIndex)
    {
        cachedFps = (float)Retrieve(Monitor.TimeFps);
        fpsBuffer[fpsIndex] = cachedFps;
        fpsIndex = (fpsIndex + 1) % fpsBuffer.Length;
    }

    private static void RenderFpsGraph(float[] fpsBuffer, int fpsIndex)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);

        ImGui.PlotLines(LabelFpsGraph, ref fpsBuffer[0], fpsBuffer.Length, fpsIndex,
            overlay_text: null,
            scale_min: 0,
            scale_max: DisplayServer.ScreenGetRefreshRate(),
            graph_size: new Vector2(WindowWidth - FpsGraphWidthMargin, FpsGraphHeight));

        ImGui.PopStyleVar();
    }

    private static double Retrieve(Monitor monitor) => Performance.GetMonitor(monitor);
}
