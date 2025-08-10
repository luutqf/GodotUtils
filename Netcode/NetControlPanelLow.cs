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

    [Export] protected abstract Button   StartServerBtn   { get; set; }
    [Export] protected abstract Button   StopServerBtn    { get; set; }
    [Export] protected abstract Button   StartClientBtn   { get; set; }
    [Export] protected abstract Button   StopClientBtn    { get; set; }
    [Export] protected abstract LineEdit IpLineEdit       { get; set; }
    [Export] protected abstract LineEdit UsernameLineEdit { get; set; }

    protected abstract ENetOptions Options { get; set; }

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

    private void SetupButtons()
    {
        StartServerBtn.Pressed += () => Net.StartServer(_port, DefaultMaxClients, Options);
        StopServerBtn.Pressed += Net.StopServer;
        StartClientBtn.Pressed += OnStartClientBtnPressed;
        StopClientBtn.Pressed += Net.StopClient;
    }

    private async void OnStartClientBtnPressed()
    {
        await Net.StartClient(_ip, _port);
    }

    private void SetupInputFields()
    {
        IpLineEdit.TextChanged += OnIpChanged;
        UsernameLineEdit.TextChanged += OnUsernameChanged;
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
            StartServerBtn.Disabled = true;
            StopServerBtn.Disabled = true;
        }

        GetTree().UnfocusCurrentControl();
    }

    private void OnClientDisconnected(DisconnectOpcode opcode)
    {
        StartServerBtn.Disabled = false;
        StopServerBtn.Disabled = false;
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
