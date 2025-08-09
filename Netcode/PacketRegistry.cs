using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GodotUtils.Netcode;

public static class PacketRegistry
{
    public static Dictionary<Type, PacketInfo<ClientPacket>> ClientPacketMap { get; private set; }
    public static Dictionary<byte, Type> ClientPacketMapBytes { get; private set; }

    public static Dictionary<Type, PacketInfo<ServerPacket>> ServerPacketMap { get; private set; }
    public static Dictionary<byte, Type> ServerPacketMapBytes { get; private set; }

    static PacketRegistry()
    {
        ClientPacketMap = MapPackets<ClientPacket>();
        ClientPacketMapBytes = ClientPacketMap.ToDictionary(kvp => kvp.Value.Opcode, kvp => kvp.Key);

        ServerPacketMap = MapPackets<ServerPacket>();
        ServerPacketMapBytes = ServerPacketMap.ToDictionary(kvp => kvp.Value.Opcode, kvp => kvp.Key);
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
