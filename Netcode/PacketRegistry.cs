using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GodotUtils.Netcode;

public static class PacketRegistry
{
    public static Dictionary<Type, PacketInfo<ClientPacket>> ClientPacketInfo { get; private set; }
    public static Dictionary<byte, Type> ClientPacketTypes { get; private set; }

    public static Dictionary<Type, PacketInfo<ServerPacket>> ServerPacketInfo { get; private set; }
    public static Dictionary<byte, Type> ServerPacketTypes { get; private set; }

    static PacketRegistry()
    {
        ClientPacketInfo = MapPackets<ClientPacket>();
        ClientPacketTypes = ClientPacketInfo.ToDictionary(kvp => kvp.Value.Opcode, kvp => kvp.Key);

        ServerPacketInfo = MapPackets<ServerPacket>();
        ServerPacketTypes = ServerPacketInfo.ToDictionary(kvp => kvp.Value.Opcode, kvp => kvp.Key);
    }

    private static Dictionary<Type, PacketInfo<T>> MapPackets<T>()
    {
        List<Type> types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .OrderBy(x => x.Name)
            .ToList();

        Dictionary<Type, PacketInfo<T>> dict = [];

        for (byte i = 0; i < types.Count; i++)
        {
            dict.Add(types[i], new PacketInfo<T>
            {
                Opcode = i,
                Instance = (T)Activator.CreateInstance(types[i])
            });
        }

        return dict;
    }
}

public class PacketInfo<T>
{
    public byte Opcode { get; set; }
    public T Instance { get; set; }
}
