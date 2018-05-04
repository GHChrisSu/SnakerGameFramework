﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SGF.Event;
using SGF.Network.Core;
using SGF.Time;
using SGF.Utils;

namespace SGF.Network.General.Client
{
    public class KCPConnection:IConnection
    {
        public SGFEvent<byte[], int> onReceive { get; private set; }


        public bool Connected { get; private set; }
        public int id { get; private set; }
        public int bindPort { get; private set; }


        private string m_ip;
        private int m_port;
        private IPEndPoint m_RemoteEndPoint;

        private Socket m_SystemSocket;
        private Thread m_ThreadRecv;
        private byte[] m_RecvBufferTemp = new byte[4096];

        private KCP m_Kcp;
        private SwitchQueue<byte[]> m_RecvBufQueue = new SwitchQueue<byte[]>();



        public void Init(int connId, int bindPort)
        {
            Debuger.Log("connId:{0}, bindPort:{1}", connId, bindPort);

            this.id = connId;
            this.bindPort = bindPort;
            onReceive = new SGFEvent<byte[], int>();
        }


        public void Clean()
        {
            Debuger.Log();
            onReceive.RemoveAllListeners();
            Close();
        }

        
        public void Connect(string ip, int port)
        {
            Debuger.Log("ip:{0}, port:{1}", ip, port);
            m_ip = ip;
            m_port = port;
            m_RemoteEndPoint = IPUtils.GetHostEndPoint(ip, port);

            m_SystemSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_SystemSocket.Bind(new IPEndPoint(IPAddress.Any, bindPort));

            m_Kcp = new KCP(0, HandleKcpSend);
            m_Kcp.NoDelay(1, 10, 2, 1);
            m_Kcp.WndSize(128, 128);

            Connected = true;

            m_ThreadRecv = new Thread(Thread_Recv){IsBackground = true};
            m_ThreadRecv.Start();

        }

        public void Close()
        {
            Connected = false;

            if (m_Kcp != null)
            {
                m_Kcp.Dispose();
                m_Kcp = null;
            }


            if (m_ThreadRecv != null)
            {
                m_ThreadRecv.Interrupt();
                m_ThreadRecv = null;
            }

            if (m_SystemSocket != null)
            {
                try
                {
                    m_SystemSocket.Shutdown(SocketShutdown.Both);

                }
                catch (Exception e)
                {
                    Debuger.LogWarning(e.Message + e.StackTrace);
                }

                m_SystemSocket.Close();
                m_SystemSocket = null;
            }

        }




        //=================================================================================
        //接收线程 
        //=================================================================================

        private void Thread_Recv()
        {
            while (Connected)
            {
                try
                {
                    DoReceiveInThread();
                }
                catch (Exception e)
                {
                    Debuger.LogWarning(e.Message + "\n" + e.StackTrace);
                    Thread.Sleep(1);
                }
                
            }
        }


        private void DoReceiveInThread()
        {
            EndPoint remotePoint = IPUtils.GetIPEndPointAny(AddressFamily.InterNetwork, 0);
            int cnt = m_SystemSocket.ReceiveFrom(m_RecvBufferTemp, m_RecvBufferTemp.Length, SocketFlags.None, ref remotePoint);

            if (cnt > 0)
            {
                if (!m_RemoteEndPoint.Equals(remotePoint))
                {
                    Debuger.LogError("收到非目标服务器的数据！");
                    return;
                }

                byte[] dst = new byte[cnt];
                Buffer.BlockCopy(m_RecvBufferTemp, 0, dst, 0, cnt);

                m_RecvBufQueue.Push(dst);
            }

        }



        //=================================================================================
        //接收逻辑
        //=================================================================================
        private void DoReceiveInMain()
        {
            m_RecvBufQueue.Switch();

            while (!m_RecvBufQueue.Empty())
            {
                var recvBufferRaw = m_RecvBufQueue.Pop();
                int ret = m_Kcp.Input(recvBufferRaw, recvBufferRaw.Length);

                //收到的不是一个正确的KCP包
                if (ret < 0)
                {
                    Debuger.LogError("收到不正确的KCP包!Ret:{0}", ret);
                    return;
                }

                m_NeedKcpUpdateFlag = true;

                for (int size = m_Kcp.PeekSize();size > 0; size = m_Kcp.PeekSize())
                {
                    var recvBuffer = new byte[size];
                    if (m_Kcp.Recv(recvBuffer) > 0)
                    {
                        onReceive.Invoke(recvBuffer, size);
                    }
                }

            }

        }




        private void HandleKcpSend(byte[] bytes, int len)
        {
            m_SystemSocket.SendTo(bytes, 0, len, SocketFlags.None, m_RemoteEndPoint);
        }


        public bool Send(byte[] bytes, int len)
        {
            if (!Connected)
            {
                return false;
            }
            return m_Kcp.Send(bytes, len) > 0;
        }

        private uint m_NextKcpUpdateTime = 0;
        private bool m_NeedKcpUpdateFlag = false;

        public void Tick()
        {
            if (Connected)
            {
                DoReceiveInMain();

                uint current = (uint) TimeUtils.GetTotalMillisecondsSince1970();

                if (m_NeedKcpUpdateFlag || current >= m_NextKcpUpdateTime)
                {
                    m_Kcp.Update(current);
                    m_NextKcpUpdateTime = m_Kcp.Check(current);
                    m_NeedKcpUpdateFlag = false;
                }

            }
            else
            {
                //处理重连
            }
        }
    }
}