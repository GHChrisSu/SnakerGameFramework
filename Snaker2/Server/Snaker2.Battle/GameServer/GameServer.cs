using System;
using SGF;
using SGF.IPCWork;
using SGF.Network.FSPLite.Server;
using SGF.Server;
using Snaker2.Battle.GameServer.Game;

namespace Snaker2.Battle.GameServer
{
    

    public class GameServer:ServerModule
    {
        private ServerContext m_context;

        public override void Start()
        {
            base.Start();

            FSPSession.ActiveTimeout = 10;//将超时时间设为10秒，以测试
            var fsp = new FSPManager();
            fsp.Init(port);

            var ipc = new IPCManager();
            ipc.Init(id);
            ipc.Start();

            m_context = new ServerContext();
            m_context.fsp = fsp;
            m_context.ipc = ipc;

            GameManager.Instance.Init(m_context);

            ConsoleInput.onInputLine.AddListener(OnInputLine);
            ConsoleInput.onInputKey.AddListener(OnInputKey);
        }

        public override void Stop()
        {
            if (m_context.fsp != null)
            {
                m_context.fsp.Clean();
                m_context.fsp = null;
            }

            if (m_context.ipc != null)
            {
                m_context.ipc.Clean();
                m_context.ipc = null;
            }

            GameManager.Instance.Clean();

            ConsoleInput.onInputLine.RemoveListener(OnInputLine);
            ConsoleInput.onInputKey.RemoveListener(OnInputKey);

            base.Stop();
        }


        public override void Tick()
        {
            ConsoleInput.Tick();
            m_context.fsp.Tick();
            m_context.ipc.Tick();
        }





        private void OnInputKey(ConsoleKey key)
        {
            Debuger.Log(key);
            if (key == ConsoleKey.F1)
            {
                m_context.fsp.Dump();
            }
            else if (key == ConsoleKey.F2)
            {
                //m_context.ipc.Dump();

            }
        }

        private void OnInputLine(string line)
        {
            Debuger.Log(line);
        }
    }
}