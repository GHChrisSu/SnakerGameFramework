using ProtoBuf;

namespace Snaker.GlobalData.Proto
{
    public class ProtoCmd
    {
        public const uint LoginReq = 1;
        public const uint LoginRsp = 2;
        public const uint HeartBeatReq = 3;
        public const uint HeartBeatRsp = 4;
    }


    [ProtoContract]
    public class ReturnCode
    {
        public static ReturnCode Success = new ReturnCode();
        public static ReturnCode UnkownError = new ReturnCode(1, "UnkownError");

        public ReturnCode(int code, string info)
        {
            this.code = code;
            this.info = info;
        }

        public ReturnCode()
        {
            this.code = 0;
            this.info = "";
        }

        public override string ToString()
        {
            return string.Format("[code:{0}, info:{1}]", code, info);
        }


        [ProtoMember(1)] public int code = 0;
        [ProtoMember(2)] public string info = "";

    }

}