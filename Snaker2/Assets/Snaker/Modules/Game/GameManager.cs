using SGF;
using SGF.Network.FSPLite;
using SGF.Network.FSPLite.Client;
using Snaker.GlobalData.Data;
using Snaker.Services.Online;

namespace Snaker.Modules.Game
{
    public class GameManager
    {
        private FSPManager m_fsp;
        private bool m_waitGameEnd = false;

        public FSPFrame CurrentFrame { get; private set; }

        public void Init()
        {
            GlobalEvent.onUpdate.AddListener(OnUpdate);
            
        }

        public void Clean()
        {
            GlobalEvent.onUpdate.RemoveListener(OnUpdate);
            if (m_fsp != null)
            {
                m_fsp.Stop();
                m_fsp = null;
            }
        }

        private void OnUpdate()
        {
            if (m_waitGameEnd)
            {
                m_waitGameEnd = false;
                m_fsp.Stop();
            }
            else
            {
                if (m_fsp != null)
                {
                    m_fsp.Tick();
                }
            }
        }

        

        public void Start(PVPStartParam param)
        {
            m_waitGameEnd = false;
            uint mainPlayerId = 0;
            uint mainUserId = OnlineManager.Instance.MainUserData.id;

            for (int i = 0; i < param.players.Count; i++)
            {
                if (param.players[i].userId == mainUserId)
                {
                    mainPlayerId = param.players[i].id;
                }
            }

            m_fsp = new FSPManager();
            m_fsp.Start(param.fspParam, mainPlayerId);
            m_fsp.onGameEnd += OnGameEnd;
            m_fsp.onGameExit += OnGameExit;

            m_fsp.SetFrameListener(OnEnterFrame);
        }


        private void OnGameEnd(int arg)
        {
            Debuger.Log();
            m_waitGameEnd = true;
            
        }

        private void OnGameExit(uint playerId)
        {
            Debuger.Log();
            if (playerId == m_fsp.MainPlayerId)
            {
                m_waitGameEnd = true;
            }
        }




        public void GameBegin()
        {
            m_fsp.SendGameBegin();

        }

        public void RoundBegin()
        {
            m_fsp.SendRoundBegin();
        }


        public void ControlStart()
        {
            m_fsp.SendControlStart();
        }

        public void SendCommand(int cmd, int arg)
        {
            m_fsp.SendFSP(cmd, arg);
        }

        public void RoundEnd()
        {
            m_fsp.SendRoundEnd();
        }

        public void GameEnd()
        {
            m_fsp.SendGameEnd();
        }




        public void GameExit()
        {
            m_fsp.SendGameExit();

        }



        private void OnEnterFrame(int frameId, FSPFrame frame)
        {
            if (frame == null)
            {
                // 这是空帧，也需要驱动逻辑
            }
            else
            {
                //处理业务逻辑的Frame里的命令
                for (int i = 0; i < frame.msgs.Count; i++)
                {
                    Debuger.Log("frameId:{0}, arg:{1}", frameId, frame.msgs[i]);
                }
            }

            //最后驱动渲染,一般来说，渲染层读取逻辑层的数据
            CurrentFrame = frame;
        }


    }
}