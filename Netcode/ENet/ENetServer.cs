using ENet;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;

namespace GodotUtils.Netcode.Server;

// ENet API Reference: https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md
public abstract class ENetServer : ENetLow
{
    /// <summary>
    /// This Dictionary is NOT thread safe and should only be accessed on the ENet Thread
    /// </summary>
    public Dictionary<uint, Peer> Peers { get; } = [];
    protected System.Timers.Timer EmitLoop { get; set; }

    private readonly ConcurrentQueue<(Packet, Peer)> incoming = new();
    private readonly ConcurrentQueue<ServerPacket> outgoing = new();
    protected readonly ConcurrentQueue<Cmd<ENetServerOpcode>> enetCmds = new();

    /// <summary>
    /// Log a message as the server. This function is thread safe.
    /// </summary>
    public override void Log(object message, BBColor color = BBColor.Green)
    {
        Logger.Log($"[Server] {message}", color);
    }

    /// <summary>
    /// Kick everyone on the server with a specified opcode. Thread safe.
    /// </summary>
    public void KickAll(DisconnectOpcode opcode)
    {
        enetCmds.Enqueue(new Cmd<ENetServerOpcode>(ENetServerOpcode.KickAll, opcode));
    }

    protected abstract void Emit();

    protected void EnqueuePacket(ServerPacket packet)
    {
        outgoing.Enqueue(packet);
    }

    protected override void ConcurrentQueues()
    {
        // ENet Cmds
        while (enetCmds.TryDequeue(out Cmd<ENetServerOpcode> cmd))
        {
            if (cmd.Opcode == ENetServerOpcode.Stop)
            {
                KickAll(DisconnectOpcode.Stopping);

                if (CTS.IsCancellationRequested)
                {
                    Log("Server is in the middle of stopping");
                    break;
                }

                CTS.Cancel();
            }
            else if (cmd.Opcode == ENetServerOpcode.Kick)
            {
                uint id = (uint)cmd.Data[0];
                DisconnectOpcode opcode = (DisconnectOpcode)cmd.Data[1];

                if (!Peers.ContainsKey(id))
                {
                    Log($"Tried to kick peer with id '{id}' but this peer does not exist");
                    break;
                }

                if (opcode == DisconnectOpcode.Banned)
                {
                    /* 
                     * TODO: Save the peer ip to banned.json and
                     * check banned.json whenever a peer tries to
                     * rejoin
                     */
                }

                Peers[id].DisconnectNow((uint)opcode);
                Peers.Remove(id);
            }
            else if (cmd.Opcode == ENetServerOpcode.KickAll)
            {
                DisconnectOpcode opcode = (DisconnectOpcode)cmd.Data[0];

                Peers.Values.ForEach(peer =>
                {
                    if (opcode == DisconnectOpcode.Banned)
                    {
                        /* 
                         * TODO: Save the peer ip to banned.json and
                         * check banned.json whenever a peer tries to
                         * rejoin
                         */
                    }

                    peer.DisconnectNow((uint)opcode);
                });
                Peers.Clear();
            }
        }

        // Incoming
        while (incoming.TryDequeue(out (Packet, Peer) packetPeer))
        {
            PacketReader packetReader = new(packetPeer.Item1);
            byte opcode = packetReader.ReadByte();

            if (!PacketRegistry.ClientPacketTypeByOpcode.TryGetValue(opcode, out Type value))
            {
                Log($"Received malformed opcode: {opcode} (Ignoring)");
                return;
            }

            Type type = value;
            ClientPacket handlePacket = PacketRegistry.ClientPacketInfoByType[type].Instance;

            try
            {
                handlePacket.Read(packetReader);
            }
            catch (System.IO.EndOfStreamException e)
            {
                Log($"Received malformed packet: {opcode} {e.Message} (Ignoring)");
                return;
            }

            packetReader.Dispose();

            handlePacket.Handle(this, packetPeer.Item2);

            if (!IgnoredPackets.Contains(type) && Options.PrintPacketReceived)
            {
                Log($"Received packet: {type.Name} from client {packetPeer.Item2.ID}" +
                    $"{(Options.PrintPacketData ? $"\n{handlePacket.ToFormattedString()}" : "")}", BBColor.LightGreen);
            }
        }

        // Outgoing
        while (outgoing.TryDequeue(out ServerPacket packet))
        {
            SendType sendType = packet.GetSendType();

            if (sendType == SendType.Peer)
            {
                packet.Send();
            }
            else if (sendType == SendType.Broadcast)
            {
                packet.Broadcast(Host);
            }
        }
    }

    protected override void Connect(Event netEvent)
    {
        Peers[netEvent.Peer.ID] = netEvent.Peer;
        Log("Client connected - ID: " + netEvent.Peer.ID);
    }

    protected abstract void Disconnected(Event netEvent);

    protected override void Disconnect(Event netEvent)
    {
        Peers.Remove(netEvent.Peer.ID);
        Log("Client disconnected - ID: " + netEvent.Peer.ID);
        Disconnected(netEvent);
    }

    protected override void Timeout(Event netEvent)
    {
        Peers.Remove(netEvent.Peer.ID);
        Log("Client timeout - ID: " + netEvent.Peer.ID);
        Disconnected(netEvent);
    }

    protected override void Receive(Event netEvent)
    {
        Packet packet = netEvent.Packet;

        if (packet.Length > GamePacket.MaxSize)
        {
            Log($"Tried to read packet from client of size {packet.Length} when max packet size is {GamePacket.MaxSize}");
            packet.Dispose();
            return;
        }

        incoming.Enqueue((packet, netEvent.Peer));
    }

    protected void WorkerThread(ushort port, int maxClients)
    {
        Host = new Host();

        try
        {
            Host.Create(new Address { Port = port }, maxClients);
        }
        catch (InvalidOperationException e)
        {
            Log($"A server is running on port {port} already! {e.Message}");
            return;
        }

        Log("Server is running");

        WorkerLoop();

        Host.Dispose();
        Log("Server has stopped");
    }

    protected override void DisconnectCleanup(Peer peer)
    {
        base.DisconnectCleanup(peer);
        Peers.Remove(peer.ID);
    }
}
