using Godot;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System;
using GodotUtils.RegEx;

namespace GodotUtils.UI.Console;

[SceneTree]
public partial class GameConsole : Component
{
    private const int MaxTextFeed = 1000;

    private static GameConsole _instance;
    private ConsoleHistory     _history = new();
    private PopupPanel         _settingsPopup;
    private CheckBox           _settingsAutoScroll;
    private TextEdit           _feed;
    private LineEdit           _input;
    private Button             _settingsBtn;
    private bool               _autoScroll = true;
    private PanelContainer     _mainContainer;

    public List<ConsoleCommandInfo> Commands { get; private set; } = [];

    public override void Ready()
    {
        if (_instance != null)
            throw new InvalidOperationException($"{nameof(GameConsole)} was initialized already");

        _instance = this;

        RegisterPhysicsProcess();
        LoadCommands();

        _feed          = Output;
        _input         = CmdsInput;
        _settingsBtn   = Settings;
        _mainContainer = MainContainer;
        _settingsPopup = PopupPanel;

        _settingsAutoScroll = PopupAutoScroll;
        _settingsAutoScroll.ButtonPressed = _autoScroll;

        _input.TextSubmitted += OnConsoleInputEntered;
        _settingsBtn.Pressed += OnSettingsBtnPressed;
        _settingsAutoScroll.Toggled += OnAutoScrollToggeled;

        _mainContainer.Hide();
    }

    public override void PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed(InputActions.ToggleConsole))
        {
            ToggleVisibility();
            return;
        }

        InputNavigateHistory();
    }

    public override void _ExitTree()
    {
        _input.TextSubmitted -= OnConsoleInputEntered;
        _settingsBtn.Pressed -= OnSettingsBtnPressed;
        _settingsAutoScroll.Toggled -= OnAutoScrollToggeled;

        _instance = null;
    }

    public void AddMessage(object message)
    {
        double prevScroll = _feed.ScrollVertical;
        
        // Prevent text feed from becoming too large
        if (_feed.Text.Length > MaxTextFeed)
        {
            // If there are say 2353 characters then 2353 - 1000 = 1353 characters
            // which is how many characters we need to remove to get back down to
            // 1000 characters
            _feed.Text = _feed.Text.Remove(0, _feed.Text.Length - MaxTextFeed);
        }

        _feed.Text += $"\n{message}";

        // Removing text from the feed will mess up the scroll, this is why the
        // scroll value was stored previous, we set this to that value now to fix
        // this
        _feed.ScrollVertical = prevScroll;

        // Autoscroll if enabled
        ScrollDown();
    }

    public static bool Visible => _instance._mainContainer.Visible;

    public static void ToggleVisibility()
    {
        _instance._mainContainer.Visible = !_instance._mainContainer.Visible;

        if (_instance._mainContainer.Visible)
        {
            _instance._input.GrabFocus();
            _instance.CallDeferred(nameof(ScrollDown));
        }
    }

    private void ScrollDown()
    {
        if (_autoScroll)
        {
            _feed.ScrollVertical = (int)_feed.GetVScrollBar().MaxValue;
        }
    }

    private bool ProcessCommand(string text)
    {
        ConsoleCommandInfo cmd = TryGetCommand(text.Split()[0].ToLower());

        if (cmd == null)
        {
            Logger.Log($"The command '{text.Split()[0].ToLower()}' does not exist");
            return false;
        }

        MethodInfo method = cmd.Method;

        object instance = GetMethodInstance(cmd.Method);

        // Use a regex to split the command input into parameters,
        // treating quoted strings as single parameters.
        // For example: command "param with spaces" param2
        // will split into: ["command", "param with spaces", "param2"]
        string[] rawCommandSplit = RegexUtils
            .CommandParams()
            .Matches(text)
            .Select(m => m.Value)
            .ToArray();

        object[] parameters = ConvertMethodParams(method, rawCommandSplit);

        method.Invoke(instance, parameters);

        return true;
    }

    private ConsoleCommandInfo TryGetCommand(string text)
    {
        ConsoleCommandInfo cmd = Commands.Find(IsMatchingCommand);

        return cmd;

        bool IsMatchingCommand(ConsoleCommandInfo cmd)
        {
            if (string.Equals(cmd.Name, text, StringComparison.OrdinalIgnoreCase))
                return true;

            return cmd.Aliases.Any(alias => string.Equals(alias, text, StringComparison.OrdinalIgnoreCase));
        }
    }

    private void InputNavigateHistory()
    {
        // If console is not visible or there is no history to navigate do nothing
        if (!_mainContainer.Visible || _history.NoHistory())
            return;

        if (Input.IsActionJustPressed(InputActions.UIUp))
        {
            string historyText = _history.MoveUpOne();

            _input.Text = historyText;

            // if deferred is not used then something else will override these settings
            SetCaretColumn(historyText.Length);
        }

        if (Input.IsActionJustPressed(InputActions.UIDown))
        {
            string historyText = _history.MoveDownOne();

            _input.Text = historyText;

            // if deferred is not used then something else will override these settings
            SetCaretColumn(historyText.Length);
        }
    }

    private void OnSettingsBtnPressed()
    {
        if (!_settingsPopup.Visible)
        {
            _settingsPopup.PopupCentered();
        }
    }

    private void OnAutoScrollToggeled(bool value)
    {
        _autoScroll = value;
    }

    private void OnConsoleInputEntered(string text)
    {
        // case sensitivity and trailing spaces should not factor in here
        string inputToLowerTrimmed = text.Trim().ToLower();
        string[] inputArr = inputToLowerTrimmed.Split(' ');

        // extract command from input
        string cmd = inputArr[0];

        // do not do anything if cmd is just whitespace
        if (string.IsNullOrWhiteSpace(cmd))
            return;

        // keep track of input history
        _history.Add(inputToLowerTrimmed);

        // process the command
        ProcessCommand(text);

        // clear the input after the command is executed
        _input.Clear();

        CallDeferred(nameof(RefocusInput));
    }

    private void RefocusInput()
    {
        // Put focus back on the input and move caret to end so user can type immediately.
        _input.Edit(); // MUST do this otherwise refocus on LineEdit will NOT work
        _input.GrabFocus();
        _input.CaretColumn = _input.Text.Length;
    }

    private static void LoadCommands()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        foreach (Type type in types)
        {
            MethodInfo[] methods = type.GetMethods(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MethodInfo method in methods)
            {
                object[] attributes = method.GetCustomAttributes(typeof(ConsoleCommandAttribute), false);

                foreach (object attribute in attributes)
                {
                    if (attribute is not ConsoleCommandAttribute cmd)
                        continue;

                    TryLoadCommand(cmd, method);
                }
            }
        }
    }

    private static void TryLoadCommand(ConsoleCommandAttribute cmd, MethodInfo method)
    {
        if (_instance.Commands.FirstOrDefault(x => x.Name == cmd.Name) != null)
        {
            throw new Exception($"Duplicate console command: {cmd.Name}");
        }

        _instance.Commands.Add(new ConsoleCommandInfo
        {
            Name = cmd.Name.ToLower(),
            Aliases = cmd.Aliases.Select(x => x.ToLower()).ToArray(),
            Method = method
        });
    }

    #region Helper Functions
    private void SetCaretColumn(int pos)
    {
        _input.CallDeferred(Control.MethodName.GrabFocus);
        _input.CallDeferred(GodotObject.MethodName.Set, LineEdit.PropertyName.CaretColumn, pos);
    }

    private static object[] ConvertMethodParams(MethodInfo method, string[] rawCmdSplit)
    {
        ParameterInfo[] paramInfos = method.GetParameters();
        object[] parameters = new object[paramInfos.Length];
        for (int i = 0; i < paramInfos.Length; i++)
        {
            parameters[i] = rawCmdSplit.Length > i + 1 && rawCmdSplit[i + 1] != null
                ? ConvertStringToType(
                    input: rawCmdSplit[i + 1],
                    targetType: paramInfos[i].ParameterType)
                : null;
        }

        return parameters;
    }

    private object GetMethodInstance(MethodInfo method)
    {
        // Return null if the method is static (no instance needed)
        if (method.IsStatic)
        {
            return null;
        }

        Type type = method.DeclaringType!;

        if (type.IsSubclassOf(typeof(GodotObject)))
        {
            // Try to find an existing Godot node of this type or create a new one
            return FindNodeByType(_mainContainer.GetTree().Root, type) ?? Activator.CreateInstance(type);
        }

        // For non-GodotObject classes, just create a new instance
        return Activator.CreateInstance(type);
    }

    private static object ConvertStringToType(string input, Type targetType)
    {
        if (targetType == typeof(string))
            return input;

        try
        {
            if (targetType == typeof(int))
            {
                return int.Parse(input);
            }
        } catch (FormatException e)
        {
            Logger.Log(e.Message);
            return 0;
        }

        try
        {
            if (targetType == typeof(bool))
            {
                return bool.Parse(input);
            }
        }
        catch (FormatException e)
        {
            Logger.Log(e.Message);
            return false;
        }

        if (targetType == typeof(float))
        {
            // Valk: Not entirely sure what is happening here other than
            // convert the input to a float.
            float value = float.Parse(input.Replace(',', '.'),
                style: NumberStyles.Any,
                provider: CultureInfo.InvariantCulture);

            return value;
        }

        throw new ArgumentException($"Unsupported type: {targetType}");
    }

    // Valk: I have not tested this code to see if it works with 100% no errors.
    private static Node FindNodeByType(Node root, Type targetType)
    {
        if (root.GetType() == targetType)
            return root;

        foreach (Node child in root.GetChildren())
        {
            Node foundNode = FindNodeByType(child, targetType);

            if (foundNode != null)
                return foundNode;
        }

        return null;
    }
    #endregion Utils
}
