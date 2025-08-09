using System.Threading.Tasks;
using System.Threading;
using System;

namespace GodotUtils.Netcode.Client;

public abstract class GodotClient : ENetClient
{
    /// <summary>
    /// Fires when the client connects to the server. Thread safe.
    /// </summary>
    public event Action Connected;

    /// <summary>
    /// Fires when the client disconnects or times out from the server. Thread safe.
    /// </summary>
    public event Action<DisconnectOpcode> Disconnected;

    /// <summary>
    /// Fires when the client times out from the server. Thread safe.
    /// </summary>
    public event Action Timedout;

    /// <summary>
    /// Is the client connected to the server? Thread safe.
    /// </summary>
    public bool IsConnected => Interlocked.Read(ref _connected) == 1;

    /// <summary>
    /// <para>
    /// A thread safe way to connect to the server. IP can be set to "127.0.0.1" for 
    /// localhost and port can be set to something like 25565.
    /// </para>
    /// 
    /// <para>
    /// Options contains settings for enabling certain logging features and ignored 
    /// packets are packets that do not get logged to the console.
    /// </para>
    /// </summary>
    public async void Connect(string ip, ushort port, ENetOptions options = default, params Type[] ignoredPackets)
    {
        Options = options;

        Log("Client is starting");
        Starting();
        InitIgnoredPackets(ignoredPackets);

        _running = 1;
        CTS = new CancellationTokenSource();
        using Task task = Task.Run(() => WorkerThread(ip, port), CTS.Token);

        try
        {
            await task;
        }
        catch (Exception e)
        {
            Logger.LogErr(e, "Client");
        }
    }

    /// <summary>
    /// Stop the client. This function is thread safe.
    /// </summary>
    public override void Stop()
    {
        if (_running == 0)
        {
            Log("Client has stopped already");
            return;
        }

        _enetCmds.Enqueue(new Cmd<ENetClientOpcode>(ENetClientOpcode.Disconnect));
    }

    /// <summary>
    /// Send a packet to the server. Packets are defined to be reliable by default. This
    /// function is thread safe.
    /// </summary>
    public void Send(ClientPacket packet)
    {
        if (!IsConnected)
        {
            Log($"Can not send packet '{packet.GetType()}' because client is not connected to the server");
            return;
        }

        packet.Write();
        packet.SetPeer(_peer);
        Outgoing.Enqueue(packet);
    }

    /// <summary>
    /// This function should be called in the _PhysicsProcess in the Godot thread. 
    /// </summary>
    public void HandlePackets()
    {
        while (_godotPackets.TryDequeue(out PacketData packetData))
        {
            PacketReader packetReader = packetData.PacketReader;
            ServerPacket handlePacket = packetData.HandlePacket;
            Type type = packetData.Type;

            handlePacket.Read(packetReader);
            packetReader.Dispose();

            handlePacket.Handle(this);

            if (!IgnoredPackets.Contains(type) && Options.PrintPacketReceived)
            {
                Log($"Received packet: {type.Name}" +
                    $"{(Options.PrintPacketData ? $"\n{handlePacket.ToFormattedString()}" : "")}", BBColor.Deepskyblue);
            }
        }

        while (_godotCmdsInternal.TryDequeue(out Cmd<GodotOpcode> cmd))
        {
            GodotOpcode opcode = cmd.Opcode;

            if (opcode == GodotOpcode.Connected)
            {
                Connected?.Invoke();
            }
            else if (opcode == GodotOpcode.Disconnected)
            {
                DisconnectOpcode disconnectOpcode = (DisconnectOpcode)cmd.Data[0];
                Disconnected?.Invoke(disconnectOpcode);
            }
            else if (opcode == GodotOpcode.Timeout)
            {
                Timedout?.Invoke();
            }
        }
    }
}
