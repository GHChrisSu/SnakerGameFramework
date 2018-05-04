using SGF;
using SGF.Common;
using SGF.Network.Core;
using SGF.Network.Core.RPCLite;
using SGF.Network.General.Client;
using Snaker.GlobalData.Data;
using Snaker.GlobalData.Proto;
using Snaker.GlobalData.Server;


namespace Snaker.Services.Online
{
    public class OnlineManager:Singleton<OnlineManager>
    {
        private NetManager m_net;
        private UserData m_mainUserData;
        private HeartBeatHandler m_heartbeat;

        public UserData MainUserData { get { return m_mainUserData; } }

        public static NetManager Net
        {
            get { return Instance.m_net; }
        }


        public void Init()
        {
            m_net = new NetManager();
            m_net.Init(typeof(KCPConnection), ServerID.ZoneServer, 0);
            m_net.RegisterRPCListener(this);

            GlobalEvent.onUpdate.AddListener(OnUpdate);

            m_heartbeat = new HeartBeatHandler();
            m_heartbeat.Init(m_net);
            m_heartbeat.onTimeout.AddListener(OnHeartBeatTimeout);
        }

        public void Clean()
        {
            GlobalEvent.onUpdate.RemoveListener(OnUpdate);

            if (m_net != null)
            {
                m_net.Clean();
                m_net = null;
            }

            if (m_heartbeat != null)
            {
                m_heartbeat.onTimeout.RemoveListener(OnHeartBeatTimeout);
                m_heartbeat.Clean();
                m_heartbeat = null;
            }
        }

        private void Connect()
        {
            m_net.Connect("127.0.0.1", 4540);
        }

        private void CloseConnect()
        {
            m_net.Close();
        }

        private void OnUpdate()
        {
            m_net.Tick();
        }


        private void OnHeartBeatTimeout()
        {
            Debuger.LogError("");
            CloseConnect();

            m_heartbeat.Stop();
            
            ReLogin();
        }

        private void ReLogin()
        {
            Connect();
            
            LoginReq req = new LoginReq();
            req.id = m_mainUserData.id;
            req.name = m_mainUserData.name;

            m_net.Send<LoginReq, LoginRsp>(ProtoCmd.LoginReq, req, OnLoginRsp, 30, OnLoginErr);
        }

        public void Login(string name)
        {
            Connect();

            LoginReq req = new LoginReq();
            req.id = 0;
            req.name = name;

            m_net.Send<LoginReq, LoginRsp>(ProtoCmd.LoginReq, req, OnLoginRsp, 5, OnLoginErr);
        }

        private void OnLoginRsp(LoginRsp rsp)
        {
            Debuger.Log("ret:{0}, userdata:{1}", rsp.ret, rsp.userdata);
            if (rsp.ret.code == 0)
            {
                m_mainUserData = rsp.userdata;
                AppConfig.Value.mainUserData = m_mainUserData;
                AppConfig.Save();


                //启动心跳
                m_heartbeat.Start();

                GlobalEvent.onLoginSuccess.Invoke();
            }
            else
            {
                GlobalEvent.onLoginFailed.Invoke(rsp.ret.code, rsp.ret.info);
            }
        }




        private void OnLoginErr(NetErrorCode errcode)
        {
            Debuger.LogError("ErrCode:{0}", errcode);

            CloseConnect();

            if (errcode == NetErrorCode.Timeout)
            {
                GlobalEvent.onLoginFailed.Invoke((int) errcode, "登录超时");
            }
            else
            {
                GlobalEvent.onLoginFailed.Invoke((int)errcode, "系统原因");
            }
        }



        [RPCRequest]
        public void Logout()
        {
            //停止心跳
            m_heartbeat.Stop();

            if (m_mainUserData != null)
            {
                m_net.Invoke("Logout");
            }

            m_mainUserData = null;
        }


        [RPC]
        private void OnLogout()
        {
            Debuger.Log();
            CloseConnect();
        }
        
        
    }
}