using __TEMPLATE__.UI;
using Godot;
using GodotUtils.UI;
using GodotUtils.UI.Console;
using GodotUtils.Debugging;
using GodotUtils.Debugging.Visualize;
using System;
using System.Threading.Tasks;

namespace GodotUtils;

// Autoload
public partial class Global : Node
{
    [Export] private Scenes _scenes;

    public event Func<Task> PreQuit;

    public static Global Instance { get; private set; }

    // Game developers should be able to access each individual manager
    public AudioManager   AudioManager   { get; private set; }
    public Logger         Logger         { get; private set; }
    public OptionsManager OptionsManager { get; private set; }
    public Services       Services       { get; private set; }
    public MetricsOverlay MetricsOverlay { get; private set; }
    public SceneManager   SceneManager   { get; private set; }
    public GameConsole    GameConsole    { get; private set; }

#if DEBUG
    private VisualizeAutoload _visualizeAutoload;
#endif

    public override void _EnterTree()
    {
        if (Instance != null)
            throw new InvalidOperationException("Global has been initialized already");

        Instance = this;
        GameConsole = GetNode<GameConsole>("%Console");
        SceneManager = new SceneManager(this, _scenes);
        Services = new Services(SceneManager);
    }

    public override void _Ready()
    {
        CommandLineArgs.Init();
        Commands.RegisterAll();

        OptionsManager = new OptionsManager(this);
        AudioManager = new AudioManager(this);
        MetricsOverlay = new MetricsOverlay();
        Logger = new Logger(GameConsole);

#if DEBUG
        _visualizeAutoload = new VisualizeAutoload();
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
        SceneManager.Dispose();

#if DEBUG
        _visualizeAutoload.Dispose();
#endif

        Profiler.Dispose();

        Instance = null;
        PreQuit = null;
    }

    public void DeferredSwitchSceneProxy(string rawName, Variant transTypeVariant)
    {
        if (SceneManager.Instance == null)
            return;

        SceneManager.Instance.DeferredSwitchScene(rawName, transTypeVariant);
    }

    public async Task QuitAndCleanup()
    {
        GetTree().AutoAcceptQuit = false;

        // Wait for cleanup
        if (PreQuit != null)
            await PreQuit?.Invoke();

        GetTree().Quit();
    }
}
