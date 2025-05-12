using GodotUtils.Netcode.Client;

namespace GodotUtils.Netcode;

public interface IGameClientFactory
{
    ENetClient CreateClient();
}
