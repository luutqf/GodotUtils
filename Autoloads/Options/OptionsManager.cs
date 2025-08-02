using Godot;
using Godot.Collections;
using GodotUtils.UI;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileAccess = Godot.FileAccess;

namespace GodotUtils.UI;

// Autoload
public partial class OptionsManager : Component
{
    public event Action<WindowMode> WindowModeChanged;

    public Dictionary<StringName, Array<InputEvent>> DefaultHotkeys { get; set; }
    public ResourceHotkeys Hotkeys { get; private set; }
    public ResourceOptions Options { get; private set; }
    public string CurrentOptionsTab { get; set; } = "General";

    private const string PathOptions = "user://options.json";
    private const string PathHotkeys = "user://hotkeys.tres";

    private JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public override void Ready()
    {
        RegisterPhysicsProcess();
        GetNode<Global>(AutoloadPaths.Global).PreQuit += SaveSettingsOnQuit;

        LoadOptions();

        GetDefaultHotkeys();
        LoadHotkeys();

        SetWindowMode();
        SetVSyncMode();
        SetWinSize();
        SetMaxFPS();
        SetLanguage();
        SetAntialiasing();
    }

    public override void PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed(InputActions.Fullscreen))
        {
            ToggleFullscreen();
        }
    }

    public void ToggleFullscreen()
    {
        if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
        {
            SwitchToFullscreen();
        }
        else
        {
            SwitchToWindow();
        }
    }

    public void SaveOptions()
    {
        string json = JsonSerializer.Serialize(Options, _jsonOptions);

        FileAccess file = FileAccess.Open(PathOptions, FileAccess.ModeFlags.Write);

        file.StoreString(json);
        file.Close();
    }

    public void SaveHotkeys()
    {
        Error error = ResourceSaver.Save(Hotkeys, PathHotkeys);

        if (error != Error.Ok)
        {
            GD.Print($"Failed to save hotkeys: {error}");
        }
    }

    public void ResetHotkeys()
    {
        // Deep clone default hotkeys over
        Hotkeys.Actions = [];

        foreach (System.Collections.Generic.KeyValuePair<StringName, Array<InputEvent>> element in DefaultHotkeys)
        {
            Array<InputEvent> arr = [];

            foreach (InputEvent item in DefaultHotkeys[element.Key])
            {
                arr.Add((InputEvent)item.Duplicate());
            }

            Hotkeys.Actions.Add(element.Key, arr);
        }

        // Set input map
        LoadInputMap(DefaultHotkeys);
    }

    private void LoadOptions()
    {
        if (FileAccess.FileExists(PathOptions))
        {
            FileAccess file = FileAccess.Open(PathOptions, FileAccess.ModeFlags.Read);

            Options = JsonSerializer.Deserialize<ResourceOptions>(file.GetAsText());

            file.Close();
        }
        else
        {
            Options = new();
        }
    }

    private static void LoadInputMap(Dictionary<StringName, Array<InputEvent>> hotkeys)
    {
        Array<StringName> actions = InputMap.GetActions();

        foreach (StringName action in actions)
        {
            InputMap.EraseAction(action);
        }

        foreach (StringName action in hotkeys.Keys)
        {
            InputMap.AddAction(action);

            foreach (InputEvent @event in hotkeys[action])
            {
                InputMap.ActionAddEvent(action, @event);
            }
        }
    }

    private void GetDefaultHotkeys()
    {
        // Get all the default actions defined in the input map
        Dictionary<StringName, Array<InputEvent>> actions = [];

        foreach (StringName action in InputMap.GetActions())
        {
            actions.Add(action, []);

            foreach (InputEvent actionEvent in InputMap.ActionGetEvents(action))
            {
                actions[action].Add(actionEvent);
            }
        }

        DefaultHotkeys = actions;
    }

    private void LoadHotkeys()
    {
        if (FileAccess.FileExists(PathHotkeys))
        {
            string localResPath = ProjectSettings.LocalizePath(DirectoryUtils.FindFile("res://", "ResourceHotkeys.cs"));
            ValdiateResourceFile(PathHotkeys, localResPath);
            Hotkeys = GD.Load<ResourceHotkeys>(PathHotkeys);

            // InputMap in project settings has changed so reset all saved hotkeys
            if (!ActionsAreEqual(DefaultHotkeys, Hotkeys.Actions))
            {
                Hotkeys = new();
                ResetHotkeys();
            }

            LoadInputMap(Hotkeys.Actions);
        }
        else
        {
            Hotkeys = new();
            ResetHotkeys();
        }
    }

    // *.tres files store the path to their script in res:// and as a result if that script is moved then the
    // path in *.tres will point to an invalid path and so this function corrects the path again.
    private void ValdiateResourceFile(string localUserPath, string localResPath)
    {
        string userGlobalPath = ProjectSettings.GlobalizePath(localUserPath);
        string content = File.ReadAllText(userGlobalPath);

        // Find current path in the resource file
        Match match = ScriptPathRegex().Match(content);

        if (!match.Success)
        {
            GD.PrintErr($"Script path not found in {localUserPath}");
            return;
        }

        string currentPath = match.Value;

        if (currentPath == localResPath)
            return; // Resource path is correct. No update needed.

        // Path is incorrect, proceed to rewrite.
        string updatedContent = ScriptPathRegex().Replace(content, localResPath);

        File.WriteAllText(userGlobalPath, updatedContent);

        GD.Print($"Updated {Path.GetFileName(userGlobalPath)} script path to: {localResPath}");
    }

    [GeneratedRegex(@"(?<=type=""Script""[^\n]*path="")[^""]+(?="")", RegexOptions.Multiline)]
    private static partial Regex ScriptPathRegex();

    private static bool ActionsAreEqual(Dictionary<StringName, Array<InputEvent>> dict1, Dictionary<StringName, Array<InputEvent>> dict2)
    {
        return dict1.Count == dict2.Count && dict1.All(pair => dict2.ContainsKey(pair.Key));
    }

    private void SetWindowMode()
    {
        switch (Options.WindowMode)
        {
            case WindowMode.Windowed:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                break;
            case WindowMode.Borderless:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                break;
            case WindowMode.Fullscreen:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                break;
        }
    }

    private void SwitchToFullscreen()
    {
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
        Options.WindowMode = WindowMode.Fullscreen;
        WindowModeChanged?.Invoke(WindowMode.Fullscreen);
    }

    private void SwitchToWindow()
    {
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        Options.WindowMode = WindowMode.Windowed;
        WindowModeChanged?.Invoke(WindowMode.Windowed);
    }

    private void SetVSyncMode()
    {
        DisplayServer.WindowSetVsyncMode(Options.VSyncMode);
    }

    private void SetWinSize()
    {
        Vector2I windowSize = new(Options.WindowWidth, Options.WindowHeight);

        if (windowSize != Vector2I.Zero)
        {
            DisplayServer.WindowSetSize(windowSize);

            // center window
            Vector2I screenSize = DisplayServer.ScreenGetSize();
            Vector2I winSize = DisplayServer.WindowGetSize();
            DisplayServer.WindowSetPosition(screenSize / 2 - winSize / 2);
        }
    }

    private void SetMaxFPS()
    {
        if (DisplayServer.WindowGetVsyncMode() == DisplayServer.VSyncMode.Disabled)
        {
            Engine.MaxFps = Options.MaxFPS;
        }
    }

    private void SetLanguage()
    {
        TranslationServer.SetLocale(
        Options.Language.ToString().Substring(0, 2).ToLower());
    }

    private void SetAntialiasing()
    {
        // Set both 2D and 3D settings to the same value
        ProjectSettings.SetSetting("rendering/anti_aliasing/quality/msaa_2d", Options.Antialiasing);
        ProjectSettings.SetSetting("rendering/anti_aliasing/quality/msaa_3d", Options.Antialiasing);
    }

    private Task SaveSettingsOnQuit()
    {
        SaveOptions();
        SaveHotkeys();

        return Task.CompletedTask;
    }
}
