#if NETCODE_ENABLED
using ENet;
using Godot;
using GodotUtils.Netcode.Server;
using System;

namespace GodotUtils.Netcode.Sandbox.Topdown;

public partial class GameServer : GodotServer
{
    protected override void OnDisconnected(Event netEvent)
    {
        
    }

    protected override void OnEmit()
    {
        
    }

    protected override void OnStopped()
    {
        
    }
}
#endif
