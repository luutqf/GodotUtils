#if NETCODE_ENABLED
using Godot;
using GodotUtils.Netcode.Client;
using GodotUtils.Netcode.Server;
using System;

namespace GodotUtils.Netcode;

public abstract partial class NetControlPanelLow<TGameClient, TGameServer> : Control
    where TGameClient : GodotClient, new()
    where TGameServer : GodotServer, new()
{
    public Net Net { get; private set; }

    private const int    DefaultMaxClients = 100;
    private const string DefaultLocalIp = "127.0.0.1";
    private const ushort DefaultPort = 25565;

    private string _username = "";
    private ushort _port = DefaultPort;
    private string _ip = DefaultLocalIp;

    private Button _startServerBtn;
    private Button _stopServerBtn;

    public override void _Ready()
    {
        ServerFactory serverFactory = new(() => new TGameServer());
        ClientFactory clientFactory = new(() => new TGameClient());

        Net = new Net(clientFactory, serverFactory);

        SetupButtons();
        SetupInputFields();
        SetupClientEvents();
    }

    public override void _Process(double delta)
    {
        Net.Client?.HandlePackets();
    }

    protected abstract ENetOptions Options();

    private void SetupButtons()
    {
        _startServerBtn = GetNode<Button>("%Start Server");
        _stopServerBtn = GetNode<Button>("%Stop Server");

        _startServerBtn.Pressed += () => Net.StartServer(_port, DefaultMaxClients, Options());
        _stopServerBtn.Pressed += Net.StopServer;
        GetNode<Button>("%Start Client").Pressed += OnStartClientBtnPressed;
        GetNode<Button>("%Stop Client").Pressed += Net.StopClient;
    }

    private void OnStartClientBtnPressed()
    {
        Net.StartClient(_ip, _port);
    }

    private void SetupInputFields()
    {
        GetNode<LineEdit>("%Ip").TextChanged += OnIpChanged;
        GetNode<LineEdit>("%Username").TextChanged += OnUsernameChanged;
    }

    private void OnIpChanged(string text)
    {
        _ip = FetchIpFromString(text, ref _port);
    }

    private void OnUsernameChanged(string text)
    {
        _username = text.IsAlphaNumeric() ? text : _username;
    }

    private void SetupClientEvents()
    {
        Net.ClientCreated += OnClientCreated;
    }

    private void OnClientCreated(GodotClient client)
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

    private static string FetchIpFromString(string ipString, ref ushort port)
    {
        string[] parts = ipString.Split(":");
        string ip = parts[0];

        if (parts.Length > 1 && ushort.TryParse(parts[1], out ushort foundPort))
        {
            port = foundPort;
        }

        return ip;
    }

    private record ClientFactory(Func<GodotClient> Creator) : IGameClientFactory
    {
        public GodotClient CreateClient() => Creator();
    }

    private record ServerFactory(Func<GodotServer> Creator) : IGameServerFactory
    {
        public GodotServer CreateServer() => Creator();
    }
}
#endif
