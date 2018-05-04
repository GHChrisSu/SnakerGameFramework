using ProtoBuf;
using SGF.Time;
using SGF.Utils;

namespace Snaker.GlobalData.Data
{
    [ProtoContract]
    public class UserData
    {
        public static int OnlineTimeout = 40;

        [ProtoMember(1)] public uint id;
        [ProtoMember(2)] public string name;
        [ProtoMember(3)] public string pwd;
        [ProtoMember(4)] public int level;//用户等级
        [ProtoMember(5)] public int defaultSnakeId;//用户的Snake的ID

        public ServerUserData svrdata = new ServerUserData();

        public override string ToString()
        {
            return string.Format("<id:{0}, name:{1}, online:{2}>", id, name, svrdata.online);

        }
    }

    public class ServerUserData
    {
        public uint sid = 0;
        public uint lastHeartBeatTime = 0;

        private bool m_online = false;

        public bool online
        {
            get
            {
                if (m_online)
                {   
                    uint dt = (uint)TimeUtils.GetTotalSecondsSince1970() - lastHeartBeatTime;
                    if (dt > UserData.OnlineTimeout)
                    {
                        m_online = false;
                    }
                }
                return m_online;
            }
            set { m_online = value; }

        }
    }
}