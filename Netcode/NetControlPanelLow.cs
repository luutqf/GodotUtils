using Godot;
using GodotUtils.Netcode.Client;

namespace GodotUtils.Netcode;

public abstract partial class NetControlPanelLow : Control
{
    public Net Net { get; private set; }

    private const string LocalIp = "127.0.0.1";
    private const ushort DefaultPort = 25565;

    private string _ip = LocalIp;
    private ushort _port = DefaultPort;
    private string _username = "";

    public override void _Ready()
    {
        Net = new Net(this, GameServerFactory(), GameClientFactory());

        SetupButtons();
        SetupInputFields();
        SetupClientEvents();
    }

    public override void _PhysicsProcess(double delta)
    {
        Net.Client?.HandlePackets();
    }

    public abstract IGameServerFactory GameServerFactory();
    public abstract IGameClientFactory GameClientFactory();
    public abstract void StartClientButtonPressed(string username);

    private void SetupButtons()
    {
        GetNode<Button>("%Start Server").Pressed += Net.StartServer;
        GetNode<Button>("%Stop Server").Pressed += Net.StopServer;
        GetNode<Button>("%Start Client").Pressed += OnStartClientBtnPressed;
        GetNode<Button>("%Stop Client").Pressed += Net.StopClient;
    }

    private void OnStartClientBtnPressed()
    {
        StartClientButtonPressed(_username);
        Net.StartClient(_ip, _port);
    }

    private void SetupInputFields()
    {
        GetNode<LineEdit>("%IP").TextChanged += OnIpChanged;
        GetNode<LineEdit>("%Username").TextChanged += OnUsernameChanged;
    }

    private void OnIpChanged(string text)
    {
        string[] parts = text.Split(":");

        _ip = parts[0];

        if (parts.Length > 1 && ushort.TryParse(parts[1], out ushort port))
        {
            _port = port;
        }
    }

    private void OnUsernameChanged(string text)
    {
        _username = text.IsAlphaNumeric() ? text : _username;
    }

    private void SetupClientEvents()
    {
        Net.ClientCreated += OnClientCreated;
    }

    private void OnClientCreated(ENetClient client)
    {
        client.Connected += OnClientConnected;
        client.Disconnected += OnClientDisconnected;
    }

    private void OnClientConnected()
    {
        if (!Net.Server.IsRunning)
        {
            GetNode<Button>("%Start Server").Disabled = true;
            GetNode<Button>("%Stop Server").Disabled = true;
        }

        GetTree().UnfocusCurrentControl();
    }

    private void OnClientDisconnected(DisconnectOpcode opcode)
    {
        GetNode<Button>("%Start Server").Disabled = false;
        GetNode<Button>("%Stop Server").Disabled = false;
    }
}
