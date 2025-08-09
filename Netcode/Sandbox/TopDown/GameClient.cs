using ENet;
using Godot;
using GodotUtils.Netcode.Client;
using System;

namespace GodotUtils.Netcode.Sandbox.Topdown;

public partial class GameClient : GodotClient
{
    protected override void Connect(Event netEvent)
    {
        base.Connect(netEvent);
        Send(new CPacketPlayerInfo { Username = "Valky", Position = new Vector2(100, 100) });
    }
}
