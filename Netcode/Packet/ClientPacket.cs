using System.Collections.Generic;
using System;
using GodotUtils.Netcode.Server;

namespace GodotUtils.Netcode;

/// <summary>
/// A packet sent from the client to the server
/// </summary>
public abstract class ClientPacket : GamePacket
{
    public void Send()
    {
        ENet.Packet enetPacket = CreateENetPacket();
        Peers[0].Send(ChannelId, ref enetPacket);
    }

    public override byte GetOpcode()
    {
        return PacketRegistry.ClientPacketMap[GetType()].Opcode;
    }

    /// <summary>
    /// The packet handled server-side
    /// </summary>
    public abstract void Handle(ENetServer server, ENet.Peer client);
}
