using Godot;
using System.Threading.Tasks;
using System;
using GodotUtils.UI;
using GodotUtils.UI.Console;
using GodotUtils.Debugging.Visualize;
using GodotUtils.Debugging;

namespace GodotUtils;

// Autoload
public partial class Global : Node
{
    public event Func<Task> PreQuit;

    public static Global Instance { get; private set; }

    public AudioManager   AudioManager   { get; private set; } = new();
    public Logger         Logger         { get; private set; } = new();
    public OptionsManager OptionsManager { get; private set; } = new();
    public Services       Services       { get; private set; } = new();
    public MetricsOverlay MetricsOverlay { get; private set; } = new();
    public GameConsole    GameConsole    { get; private set; }
    public SceneManager   SceneManager   { get; private set; }

#if DEBUG
    private VisualizeAutoload _visualizeAutoload = new();
#endif

    public override void _EnterTree()
    {
        if (Instance != null)
            throw new InvalidOperationException("Global has been initialized already");

        Instance = this;
        GameConsole = GetNode<GameConsole>("%Console");
        SceneManager = GetNode<SceneManager>("%SceneManager");
        Services.Init(SceneManager);
    }

    public override void _Ready()
    {
        CommandLineArgs.Init();

        OptionsManager.Init(this);
        AudioManager.Init(this);
        MetricsOverlay.Init();
        Logger.Init(GameConsole);

#if DEBUG
        _visualizeAutoload.Init();
#endif
    }

    public override void _Process(double delta)
    {
        OptionsManager.Update();
        MetricsOverlay.Update();
        Logger.Update();

#if DEBUG
        _visualizeAutoload.Update();
#endif
    }

    public override async void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            await QuitAndCleanup();
        }
    }

    public override void _ExitTree()
    {
        AudioManager.Dispose();
        Logger.Dispose();
        OptionsManager.Dispose();
        Services.Dispose();
        MetricsOverlay.Dispose();

#if DEBUG
        _visualizeAutoload.Dispose();
#endif

        Profiler.Dispose();

        Instance = null;
        PreQuit = null;
    }

    public async Task QuitAndCleanup()
    {
        GetTree().AutoAcceptQuit = false;

        // Wait for cleanup
        if (PreQuit != null)
            await PreQuit?.Invoke();

        // This must be here because buttons call Global::Quit()
        GetTree().Quit();
    }
}
