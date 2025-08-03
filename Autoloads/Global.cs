using Godot;
using System.Threading.Tasks;
using System;
using GodotUtils.UI;
using GodotUtils.UI.Console;
using GodotUtils.Debugging.Visualize;
using GodotUtils.Debugging;

namespace GodotUtils;

// todo: Separate [GeneratedRegex] into their own regex class

// Autoload
public partial class Global : Node
{
    public event Func<Task> PreQuit;

    public static Global Instance { get; private set; }

    public AudioManager   AudioManager   { get; } = new();
    public Logger         Logger         { get; } = new();
    public OptionsManager OptionsManager { get; } = new();
    public Services       Services       { get; } = new();
    public MetricsOverlay MetricsOverlay { get; } = new();
    public GameConsole    GameConsole    { get; private set; }
    public SceneManager   SceneManager   { get; private set; }

    private VisualizeAutoload _visualizeAutoload = new();

    public override void _EnterTree()
    {
        Instance = this;
        GameConsole = GetNode<GameConsole>("%Console");
        SceneManager = GetNode<SceneManager>("%SceneManager");
        Services.Init(GetTree(), SceneManager);
    }

    public override void _Ready()
    {
        CommandLineArgs.Init();

        _visualizeAutoload.Init(GetTree());

        AudioManager.Init(this);
        OptionsManager.Init(this);
        MetricsOverlay.Init();
        Logger.Init(GameConsole);
    }

    public override void _Process(double delta)
    {
        _visualizeAutoload.Update();

        OptionsManager.Update();
        MetricsOverlay.Update();
        Logger.Update();
    }

    public override async void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            await QuitAndCleanup();
        }
    }

    public async Task QuitAndCleanup()
    {
        GetTree().AutoAcceptQuit = false;

        // Wait for cleanup
        if (PreQuit != null)
        {
            await PreQuit?.Invoke();
        }

        // This must be here because buttons call Global::Quit()
        GetTree().Quit();
    }
}
