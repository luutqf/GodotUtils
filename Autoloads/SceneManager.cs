using Godot;
using System;

namespace GodotUtils;

// About Scene Switching: https://docs.godotengine.org/en/latest/tutorials/scripting/singletons_autoload.html
public partial class SceneManager : Component
{
    [Export] private PackedScene _sceneMainMenu;
    [Export] private PackedScene _sceneModLoader;
    [Export] private PackedScene _sceneOptions;
    [Export] private PackedScene _sceneCredits;
    [Export] private PackedScene _sceneGame;

    /// <summary>
    /// The event is invoked right before the scene is changed
    /// </summary>
    public event Action<string> PreSceneChanged;

    private static SceneManager _instance;
    private SceneTree _tree;
    private Node _currentScene;

    public SceneManager(Global global, Scenes scenes)
    {
        if (_instance != null)
            throw new InvalidOperationException($"{nameof(SceneManager)} was initialized already");

        _instance = this;
        _tree = GetTree();
        Window root = _tree.Root;
        _currentScene = root.GetChild(root.GetChildCount() - 1);

        // Gradually fade out all SFX whenever the scene is changed
        PreSceneChanged += OnPreSceneChanged;
    }

    public override void _ExitTree()
    {
        PreSceneChanged -= OnPreSceneChanged;

        _instance = null;
    }

    private void OnPreSceneChanged(string scene) => AudioManager.FadeOutSFX();

    public static Node GetCurrentScene()
    {
        return _instance._currentScene;
    }

    public static void SwitchScene(Scene scene, TransType transType = TransType.None)
    {
        string scenePath = scene switch
        {
            Scene.MainMenu => _instance._sceneMainMenu.ResourcePath,
            Scene.ModLoader => _instance._sceneModLoader.ResourcePath,
            Scene.Options => _instance._sceneOptions.ResourcePath,
            Scene.Credits => _instance._sceneCredits.ResourcePath,
            Scene.Game => _instance._sceneGame.ResourcePath,
            _ => throw new ArgumentOutOfRangeException(nameof(scene), scene, "Tried to switch to unknown scene")
        };

        _instance.PreSceneChanged?.Invoke(scenePath);

        switch (transType)
        {
            case TransType.None:
                _instance.ChangeScene(scenePath, transType);
                break;
            case TransType.Fade:
                _instance.FadeTo(TransColor.Black, 2, () => _instance.ChangeScene(scenePath, transType));
                break;
        }
    }

    /// <summary>
    /// Resets the currently active scene.
    /// </summary>
    public void ResetCurrentScene()
    {
        string sceneFilePath = _tree.CurrentScene.SceneFilePath;

        string[] words = sceneFilePath.Split("/");
        string sceneName = words[words.Length - 1].Replace(".tscn", "");

        PreSceneChanged?.Invoke(sceneName);

        // Wait for engine to be ready before switching scenes
        _instance.CallDeferred(nameof(DeferredSwitchScene), sceneFilePath, Variant.From(TransType.None));
    }

    private void ChangeScene(string scenePath, TransType transType)
    {
        // Wait for engine to be ready before switching scenes
        _instance.CallDeferred(nameof(DeferredSwitchScene), scenePath, Variant.From(transType));
    }

    private void DeferredSwitchScene(string rawName, Variant transTypeVariant)
    {
        // Safe to remove scene now
        _currentScene.Free();

        // Load a new scene.
        PackedScene nextScene = (PackedScene)GD.Load(rawName);

        // Internal the new scene.
        _currentScene = nextScene.Instantiate();

        // Add it to the active scene, as child of root.
        _tree.Root.AddChild(_currentScene);

        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        _tree.CurrentScene = _currentScene;

        TransType transType = transTypeVariant.As<TransType>();

        switch (transType)
        {
            case TransType.None:
                break;
            case TransType.Fade:
                FadeTo(TransColor.Transparent, 1);
                break;
        }
    }

    private void FadeTo(TransColor transColor, double duration, Action finished = null)
    {
        // Add canvas layer to scene
        CanvasLayer canvasLayer = new()
        {
            Layer = 10 // render on top of everything else
        };

        _currentScene.AddChild(canvasLayer);

        // Setup color rect
        ColorRect colorRect = new()
        {
            Color = new Color(0, 0, 0, transColor == TransColor.Black ? 0 : 1),
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        // Make the color rect cover the entire screen
        colorRect.SetLayout(Control.LayoutPreset.FullRect);
        canvasLayer.AddChild(colorRect);

        // Animate color rect
        new GTween(colorRect)
            .Animate(ColorRect.PropertyName.Color, new Color(0, 0, 0, transColor == TransColor.Black ? 1 : 0), duration)
            .Callback(() =>
            {
                canvasLayer.QueueFree();
                finished?.Invoke();
            });
    }

    public enum TransType
    {
        None,
        Fade
    }

    private enum TransColor
    {
        Black,
        Transparent
    }
}

public enum Scene
{
    MainMenu,
    ModLoader,
    Options,
    Credits,
    Game
}
