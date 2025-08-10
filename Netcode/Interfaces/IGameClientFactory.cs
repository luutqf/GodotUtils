#if NETCODE_ENABLED
using GodotUtils.Netcode.Client;

namespace GodotUtils.Netcode;

public interface IGameClientFactory
{
    GodotClient CreateClient();
}
#endif
