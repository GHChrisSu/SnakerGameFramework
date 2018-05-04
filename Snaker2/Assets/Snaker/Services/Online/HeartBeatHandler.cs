using SGF;
using SGF.Event;
using SGF.Network.Core;
using SGF.Network.General.Client;
using SGF.Time;
using SGF.Utils;
using Snaker.GlobalData.Proto;


namespace Snaker.Services.Online
{
    public class HeartBeatHandler
    {
        private NetManager m_net;
        private uint m_ping;
        private float m_lastHeartBeatTime = 0;

        public SGFEvent onTimeout = new SGFEvent();

        public void Init(NetManager net)
        {
            m_net = net;
        }

        public void Clean()
        {
            Stop();
            m_net = null;
        }


        public void Start()
        {
            GlobalEvent.onUpdate.AddListener(OnUpdate);
        }

        public void Stop()
        {
            GlobalEvent.onUpdate.RemoveListener(OnUpdate);
        }

        private void OnUpdate()
        {
            float current = SGFTime.GetTimeSinceStartup();
            if (current - m_lastHeartBeatTime > 5.0f)
            {
                m_lastHeartBeatTime = current;

                HeartBeatReq req = new HeartBeatReq();
                req.ping = (ushort)m_ping;
                req.timestamp = (uint)TimeUtils.GetTotalMillisecondsSince1970();
                m_net.Send<HeartBeatReq, HeartBeatRsp>(ProtoCmd.HeartBeatReq, req, OnHeartBeatRsp, 15,
                    OnHeartBeatError);
                
            }
        }

        private void OnHeartBeatRsp(HeartBeatRsp rsp)
        {
            //Debuger.Log();
            if (rsp.ret.code == 0)
            {
                uint current = (uint)TimeUtils.GetTotalMillisecondsSince1970();
                uint dt = current - rsp.timestamp;
                m_ping = dt / 2;
            }
        }

        private void OnHeartBeatError(NetErrorCode code)
        {
            if (code == NetErrorCode.Timeout)
            {
                Stop();
                onTimeout.Invoke();
            }
        }

    }
}