using Godot;
using GodotUtils.Netcode.Client;
using GodotUtils.Netcode.Server;

namespace GodotUtils.Netcode;

public abstract partial class NetControlPanelLow<TGameClient, TGameServer> : Control
    where TGameClient : ENetClient, new()
    where TGameServer : ENetServer, new()
{
    public Net Net { get; private set; }

    private const string LocalIp = "127.0.0.1";
    private const ushort DefaultPort = 25565;

    private string _ip = LocalIp;
    private ushort _port = DefaultPort;
    private string _username = "";

    private Button _startServerBtn;
    private Button _stopServerBtn;

    public override void _Ready()
    {
        Net = new Net(new TGameServer(), new TGameClient());

        SetupButtons();
        SetupInputFields();
        SetupClientEvents();
    }

    public override void _PhysicsProcess(double delta)
    {
        Net.Client?.HandlePackets();
    }

    public abstract void StartClientButtonPressed(string username);

    private void SetupButtons()
    {
        _startServerBtn = GetNode<Button>("%Start Server");
        _stopServerBtn = GetNode<Button>("%Stop Server");

        _startServerBtn.Pressed += Net.StartServer;
        _stopServerBtn.Pressed += Net.StopServer;
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
        GetNode<LineEdit>("%Ip").TextChanged += OnIpChanged;
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
            _startServerBtn.Disabled = true;
            _stopServerBtn.Disabled = true;
        }

        GetTree().UnfocusCurrentControl();
    }

    private void OnClientDisconnected(DisconnectOpcode opcode)
    {
        _startServerBtn.Disabled = false;
        _stopServerBtn.Disabled = false;
    }
}
