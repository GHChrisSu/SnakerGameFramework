using ProtoBuf;
using Snaker.GlobalData.Data;



namespace Snaker.GlobalData.Proto
{
    [ProtoContract]
    public class LoginReq
    {
        [ProtoMember(1)]
        public uint id;
        [ProtoMember(2)]
        public string name;
    }

    [ProtoContract]
    public class LoginRsp
    {
        [ProtoMember(1)] public ReturnCode ret;
        [ProtoMember(2)] public UserData userdata;
    }


}