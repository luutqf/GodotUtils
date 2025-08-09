using ENet;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace GodotUtils.Netcode.Server;

public abstract class GodotServer : ENetServer
{
    /// <summary>
    /// <para>
    /// A thread safe way to start the server. Max clients could be 100 and port could
    /// be set to something like 25565.
    /// </para>
    /// 
    /// <para>
    /// Options contains settings for enabling certain logging features and ignored 
    /// packets are packets that do not get logged to the console.
    /// </para>
    /// </summary>
    public async void Start(ushort port, int maxClients, ENetOptions options, params Type[] ignoredPackets)
    {
        if (_running == 1)
        {
            Log("Server is running already");
            return;
        }

        Options = options;

        InitIgnoredPackets(ignoredPackets);

        EmitLoop = SystemTimerFactory.Create(100, Emit, false);
        EmitLoop.Start();

        _running = 1;

        CTS = new CancellationTokenSource();

        Starting();

        using Task task = Task.Run(() => WorkerThread(port, maxClients), CTS.Token);

        try
        {
            await task;
        }
        catch (Exception e)
        {
            Logger.LogErr(e, "Server");
        }
    }

    /// <summary>
    /// Ban someone by their ID. Thread safe.
    /// </summary>
    public void Ban(uint id)
    {
        Kick(id, DisconnectOpcode.Banned);
    }

    /// <summary>
    /// Ban everyone on the server. Thread safe.
    /// </summary>
    public void BanAll()
    {
        KickAll(DisconnectOpcode.Banned);
    }

    /// <summary>
    /// Kick someone by their ID with a specified opcode. Thread safe.
    /// </summary>
    public void Kick(uint id, DisconnectOpcode opcode)
    {
        ENetCmds.Enqueue(new Cmd<ENetServerOpcode>(ENetServerOpcode.Kick, id, opcode));
    }

    /// <summary>
    /// Stop the server. Thread safe.
    /// </summary>
    public override void Stop()
    {
        if (_running == 0)
        {
            Log("Server has stopped already");
            return;
        }

        EmitLoop.Stop();
        EmitLoop.Dispose();
        ENetCmds.Enqueue(new Cmd<ENetServerOpcode>(ENetServerOpcode.Stop));
    }

    /// <summary>
    /// Send a packet to a client. Thread safe.
    /// </summary>
    public void Send(ServerPacket packet, Peer peer)
    {
        packet.Write();

        Type type = packet.GetType();

        if (!IgnoredPackets.Contains(type) && Options.PrintPacketSent)
        {
            Log($"Sending packet {type.Name} {FormatByteSize(packet.GetSize())}to client {peer.ID}" +
                $"{(Options.PrintPacketData ? $"\n{packet.ToFormattedString()}" : "")}");
        }

        packet.SetSendType(SendType.Peer);
        packet.SetPeer(peer);

        EnqueuePacket(packet);
    }

    /// <summary>
    /// If no clients are specified, then the packet will be sent to everyone. If
    /// one client is specified then that client will be excluded from the broadcast.
    /// If more than one client is specified then the packet will only be sent to
    /// those clients. This function is thread safe.
    /// </summary>
    public void Broadcast(ServerPacket packet, params Peer[] clients)
    {
        packet.Write();

        Type type = packet.GetType();

        if (!IgnoredPackets.Contains(type) && Options.PrintPacketSent)
        {
            // This is messy but I don't know how I will clean it up right
            // now so I'm leaving it as is for now..
            string byteSize = Options.PrintPacketByteSize ?
                $"({packet.GetSize()} bytes)" : "";

            string peerArr = clients.Select(x => x.ID).ToFormattedString();

            string message = $"Broadcasting packet {type.Name} {byteSize}" + (clients.Length == 0 ?
                 "to everyone" : clients.Length == 1 ?
                $"to everyone except peer {peerArr}" :
                $"to peers {peerArr}") + (Options.PrintPacketData ?
                $"\n{packet.ToFormattedString()}" : "");

            Log(message);
        }

        packet.SetSendType(SendType.Broadcast);
        packet.SetPeers(clients);

        EnqueuePacket(packet);
    }
}
