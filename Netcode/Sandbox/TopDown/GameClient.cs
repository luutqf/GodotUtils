using ENet;
using Godot;
using GodotUtils.Netcode.Client;
using GodotUtils.Netcode.Server;
using System;

namespace GodotUtils.Netcode.Sandbox.Topdown;

public partial class GameClient : GodotClient
{
    protected override void Connect(Event netEvent)
    {
        base.Connect(netEvent);
        Net.Client.Send(new CPacketPlayerInfo { Username = "Valky", Position = new Vector2(100, 100) });
    }
}

public class CPacketPlayerInfo : ClientPacket
{
    // The NetSend parameter indicates the order of what gets sent first
    [NetSend(1)]
    public string Username { get; set; }

    [NetSend(2)]
    public Vector2 Position { get; set; }

    public override void Handle(ENetServer server, Peer client)
    {
        Logger.Log(Username);
        Logger.Log(Position);
    }
}
