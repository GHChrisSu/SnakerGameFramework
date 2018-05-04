using System.Collections.Generic;
using ProtoBuf;

namespace Snaker.GlobalData.Data
{
    [ProtoContract]
    public class GamesServerStartParam
    {
        [ProtoMember(1)]
        public uint roomId;
        [ProtoMember(2)]
        public int authId;
        [ProtoMember(3)]
        public List<uint> listPlayerId = new List<uint>();
        [ProtoMember(4)]
        public List<uint> listPlayerFSPSessionId = new List<uint>();
    }
}