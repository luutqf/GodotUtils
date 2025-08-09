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

    protected ConcurrentQueue<Cmd<ENetServerOpcode>> ENetCmds { get; } = new();
    protected System.Timers.Timer EmitLoop { get; set; }

    private readonly ConcurrentQueue<(Packet, Peer)> _incoming = new();
    private readonly ConcurrentQueue<ServerPacket> _outgoing = new();

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
        ENetCmds.Enqueue(new Cmd<ENetServerOpcode>(ENetServerOpcode.KickAll, opcode));
    }

    protected abstract void Emit();

    protected void EnqueuePacket(ServerPacket packet)
    {
        _outgoing.Enqueue(packet);
    }

    protected override void ConcurrentQueues()
    {
        ProcessEnetCommands();
        ProcessIncomingPackets();
        ProcessOutgoingPackets();
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

        _incoming.Enqueue((packet, netEvent.Peer));
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

        try
        {
            WorkerLoop();
        }
        finally
        {
            Host.Dispose();
        }
        
        Log("Server has stopped");
    }

    protected override void DisconnectCleanup(Peer peer)
    {
        base.DisconnectCleanup(peer);
        Peers.Remove(peer.ID);
    }

    private void ProcessEnetCommands()
    {
        while (ENetCmds.TryDequeue(out Cmd<ENetServerOpcode> cmd))
        {
            switch (cmd.Opcode)
            {
                case ENetServerOpcode.Stop:
                    HandleStopCommand();
                    break;

                case ENetServerOpcode.Kick:
                    HandleKickCommand(cmd);
                    break;

                case ENetServerOpcode.KickAll:
                    HandleKickAllCommand(cmd);
                    break;
            }
        }
    }

    private void HandleStopCommand()
    {
        KickAll(DisconnectOpcode.Stopping);

        if (CTS.IsCancellationRequested)
        {
            Log("Server is in the middle of stopping");
            return;
        }

        CTS.Cancel();
    }

    private void HandleKickCommand(Cmd<ENetServerOpcode> cmd)
    {
        uint id = (uint)cmd.Data[0];
        DisconnectOpcode opcode = (DisconnectOpcode)cmd.Data[1];

        if (!Peers.TryGetValue(id, out Peer peer))
        {
            Log($"Tried to kick peer with id '{id}' but this peer does not exist");
            return;
        }

        peer.DisconnectNow((uint)opcode);
        Peers.Remove(id);
    }

    private void HandleKickAllCommand(Cmd<ENetServerOpcode> cmd)
    {
        DisconnectOpcode opcode = (DisconnectOpcode)cmd.Data[0];

        Peers.Values.ForEach(peer => peer.DisconnectNow((uint)opcode));
        Peers.Clear();
    }

    private void ProcessIncomingPackets()
    {
        while (_incoming.TryDequeue(out (Packet, Peer) packetPeer))
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
            finally
            {
                packetReader.Dispose();
            }

            handlePacket.Handle(this, packetPeer.Item2);

            LogPacketReceived(type, packetPeer.Item2.ID, handlePacket);
        }
    }

    private void LogPacketReceived(Type type, uint clientId, ClientPacket packet)
    {
        if (!IgnoredPackets.Contains(type) && Options.PrintPacketReceived)
        {
            string packetData = Options.PrintPacketData ? $"\n{packet.ToFormattedString()}" : string.Empty;

            Log($"Received packet: {type.Name} from client {clientId}{packetData}", BBColor.LightGreen);
        }
    }

    private void ProcessOutgoingPackets()
    {
        while (_outgoing.TryDequeue(out ServerPacket packet))
        {
            SendType sendType = packet.GetSendType();

            switch (sendType)
            {
                case SendType.Peer:
                    packet.Send();
                    break;

                case SendType.Broadcast:
                    packet.Broadcast(Host);
                    break;
            }
        }
    }
}
