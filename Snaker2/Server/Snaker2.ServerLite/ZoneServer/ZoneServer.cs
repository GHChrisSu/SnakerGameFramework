using System;
using SGF;
using SGF.IPCWork;
using SGF.Network.General.Server;
using SGF.Server;
using Snaker.ServerLite.ZoneServer.Online;


namespace Snaker.Server.ZoneServer
{
    public class ZoneServer : ServerModule
    {
        private ServerContext m_context;

        public override void Start()
        {
            base.Start();
            var net = new NetManager();
            net.Init(port);
           
            var ipc = new IPCManager();
            ipc.Init(id);
            ipc.Start();

            m_context = new ServerContext();
            m_context.net = net;
            m_context.ipc = ipc;

            OnlineManager.Instance.Init(m_context);
            RoomManager.Instance.Init(m_context);

            ConsoleInput.onInputLine.AddListener(OnInputLine);
            ConsoleInput.onInputKey.AddListener(OnInputKey);
        }



        public override void Stop()
        {
            if (m_context.net != null)
            {
                m_context.net.Clean();
                m_context.net = null;
            }

            if (m_context.ipc != null)
            {
                m_context.ipc.Clean();
                m_context.ipc = null;
            }

            ConsoleInput.onInputLine.RemoveListener(OnInputLine);
            ConsoleInput.onInputKey.RemoveListener(OnInputKey);

            OnlineManager.Instance.Clean();
            RoomManager.Instance.Clean();

            base.Stop();
        }


        public override void Tick()
        {
            ConsoleInput.Tick();
            m_context.net.Tick();
            m_context.ipc.Tick();
        }


        //==============================================================


        private void OnInputKey(ConsoleKey key)
        {
            Debuger.Log(key);
            if (key == ConsoleKey.F1)
            {
                m_context.net.Dump();
            }
            else if (key == ConsoleKey.F2)
            {
                OnlineManager.Instance.Dump();
            }
            else if (key == ConsoleKey.F3)
            {
                RoomManager.Instance.Dump();
            }
            else if (key == ConsoleKey.F4)
            {
                m_context.net.Dump();
                OnlineManager.Instance.Dump();
                RoomManager.Instance.Dump();
            }
        }

        private void OnInputLine(string line)
        {
            Debuger.Log(line);
            if (line == "dump")
            {
                m_context.net.Dump();
                OnlineManager.Instance.Dump();
                RoomManager.Instance.Dump();

            }
        }


    }
}