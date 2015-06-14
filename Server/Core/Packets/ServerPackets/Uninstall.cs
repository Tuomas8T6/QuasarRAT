﻿using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Uninstall : IPacket
    {
        public Uninstall()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}