namespace GodotUtils.Netcode.Sandbox.Topdown;

public partial class NetControlPanel : NetControlPanelLow<GameClient, GameServer>
{
    protected override ENetOptions Options()
    {
        return new()
        {
            PrintPacketByteSize = true,
            PrintPacketData = true,
            PrintPacketReceived = true,
            PrintPacketSent = true
        };
    }
}
