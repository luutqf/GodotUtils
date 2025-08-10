#if NETCODE_ENABLED
using Godot;

namespace GodotUtils.Netcode.Sandbox.Topdown;

public partial class NetControlPanel : NetControlPanelLow<GameClient, GameServer>
{
    protected override Button   StartServerBtn   { get; set; }
    protected override Button   StopServerBtn    { get; set; }
    protected override Button   StartClientBtn   { get; set; }
    protected override Button   StopClientBtn    { get; set; }
    protected override LineEdit IpLineEdit       { get; set; }
    protected override LineEdit UsernameLineEdit { get; set; }

    protected override ENetOptions Options { get; set; } = new()
    {
        PrintPacketByteSize = true,
        PrintPacketData = true,
        PrintPacketReceived = true,
        PrintPacketSent = true
    };
}
#endif
