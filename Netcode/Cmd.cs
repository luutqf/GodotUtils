﻿namespace GodotUtils.Netcode;

public class Cmd<TOpcode>
{
    public TOpcode Opcode { get; set; }
    public object[] Data { get; set; }

    public Cmd(TOpcode opcode, params object[] data)
    {
        Opcode = opcode;
        Data = data;
    }
}