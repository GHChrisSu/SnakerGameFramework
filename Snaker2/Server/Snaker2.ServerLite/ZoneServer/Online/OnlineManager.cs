using System.Text;
using SGF;
using SGF.Common;
using SGF.Network.Core.RPCLite;
using SGF.Network.General.Server;
using SGF.Utils;
using Snaker.GlobalData.Data;
using Snaker.GlobalData.Proto;
using Snaker.Server.ZoneServer;


namespace Snaker.ServerLite.ZoneServer.Online
{
    public class OnlineManager:Singleton<OnlineManager>
    {
        private MapList<uint, UserData> m_mapUserData;
        private NetManager m_net;

        public void Init(ServerContext context)
        {
            m_net = context.net;
            m_net.SetAuthCmd(ProtoCmd.LoginReq);
            m_net.AddListener<LoginReq>(ProtoCmd.LoginReq, OnLoginReq);
            m_net.AddListener<HeartBeatReq>(ProtoCmd.HeartBeatReq, OnHeartBeatReq);
            m_net.RegisterRPCListener(this);

            m_mapUserData = new MapList<uint, UserData>();
        }

        public void Clean()
        {
            if (m_net != null)
            {
                m_net.UnRegisterRPCListener(this);
                m_net = null;
            }
            
        }


        public void Dump()
        {

            StringBuilder sb = new StringBuilder();
            UserData[] list = m_mapUserData.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                sb.AppendLine("\t" + list[i].ToString());
            }

            Debuger.LogWarning("\nUser ({0}):\n{1}", m_mapUserData.Count, sb);

        }


        //====================================================================================


        private void OnLoginReq(ISession session, uint index, LoginReq req)
        {
            Debuger.Log("session:{0}, index:{1}, name:{2}", session.id, index, req.name);
            bool success = false;

            UserData ud = GetUserData(req.name);
            if (ud == null)
            {
                ud = CreateUserData(session.id, req.name);
                ActiveUser(session, ud);
                success = true;
            }
            else
            {
                if (req.id == ud.id)
                {
                    //重新登录
                    ActiveUser(session, ud);
                    success = true;

                }
                else
                {
                    //正常登录，但是尝试占用已有的名字
                    if (!ud.svrdata.online)
                    {
                        //如果该名字已经离线，则可以占用
                        ActiveUser(session, ud);
                        success = true;
                    }
                    
                }

            }

            if (success)
            {
                LoginRsp rsp = new LoginRsp();
                rsp.ret = ReturnCode.Success;
                rsp.userdata = ud;
                m_net.Send(session, index, ProtoCmd.LoginRsp, rsp);
            }
            else
            {
                LoginRsp rsp = new LoginRsp();
                rsp.ret = new ReturnCode(1, "名字已经被占用了！");
                m_net.Send(session, index, ProtoCmd.LoginRsp, rsp);
            }
        }



        private void ActiveUser(ISession session, UserData ud)
        {
            ud.svrdata.online = true;
            ud.svrdata.lastHeartBeatTime = (uint)TimeUtils.GetTotalSecondsSince1970();
            ud.svrdata.sid = session.id;
            session.SetAuth(ud.id);
        }

        private void OnHeartBeatReq(ISession session, uint index, HeartBeatReq req)
        {
            UserData ud = GetUserData(session.uid);
            if (ud != null)
            {
                ud.svrdata.lastHeartBeatTime = (uint)TimeUtils.GetTotalSecondsSince1970();
                session.ping = req.ping;

                HeartBeatRsp rsp = new HeartBeatRsp();
                rsp.ret = ReturnCode.Success;
                rsp.timestamp = req.timestamp;
                m_net.Send(session, index, ProtoCmd.HeartBeatRsp, rsp);
            }
            else
            {
                Debuger.LogWarning("找不到Session 对应的UserData! session:{0}", session);
            }
        }

        [RPC]
        private void Logout(ISession session)
        {
            ReleaseUserData(session.uid);
            m_net.Return();
        }



        //====================================================================================

        public UserData CreateUserData(uint id, string name)
        {
            UserData data = new UserData();
            data.name = name;
            data.id = id;
            data.pwd = "";
            m_mapUserData.Add(id, data);
            return data;
        }

        public void ReleaseUserData(uint id)
        {
            m_mapUserData.Remove(id);
        }

        public UserData GetUserData(string name)
        {
            int cnt = m_mapUserData.Count;
            var list = m_mapUserData.AsList();
            for (int i = 0; i < cnt; i++)
            {
                if (list[i].name == name)
                {
                    return list[i];
                }
            }

            return null;
        }

        public UserData GetUserData(uint id)
        {
            return m_mapUserData[id];
        }

    }
}