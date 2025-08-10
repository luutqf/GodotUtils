#if NETCODE_ENABLED
using GodotUtils.Netcode.Server;

namespace GodotUtils.Netcode;

public interface IGameServerFactory
{
    GodotServer CreateServer();
}
#endif
