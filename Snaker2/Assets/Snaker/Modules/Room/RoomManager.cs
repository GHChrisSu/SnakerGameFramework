using System.Collections.Generic;
using SGF;
using SGF.Event;
using SGF.Module;
using SGF.Network.Core.RPCLite;
using Snaker.GlobalData.Data;
using Snaker.Services.Online;

namespace Snaker.Modules
{
    public class RoomManager
    {
        public readonly SGFEvent<List<RoomData>> onUpdateRoomList = new SGFEvent<List<RoomData>>();
        public readonly SGFEvent onJoin = new SGFEvent();
        public readonly SGFEvent onExit = new SGFEvent();
        public readonly SGFEvent<RoomData> onUpdateRoomInfo = new SGFEvent<RoomData>();


        private List<RoomData> m_listRoom = new List<RoomData>();

        private uint m_mainUserId;
        private bool m_isInRoom = false;
        private bool m_isReady = false;
        private bool m_isAllReady = false;

        
        private RoomData m_currRoom = new RoomData();


        public bool IsReady { get { return m_isReady; } }
        public bool IsInRoom { get { return m_isInRoom; } }
        public bool IsAllReady { get { return m_isAllReady; } }

        public bool IsRoomOwner { get { return (m_currRoom.owner == m_mainUserId); } }


        public void Init()
        {
            OnlineManager.Net.RegisterRPCListener(this);
        }

        public void Clean()
        {
            OnlineManager.Net.UnRegisterRPCListener(this);
        }

        public void Reset()
        {
            m_isReady = false;
            m_isInRoom = false;
            m_mainUserId = OnlineManager.Instance.MainUserData.id;

        }


        //==========================================================================
        /// <summary>
        /// 获取房间列表
        /// </summary>
        [RPCRequest]
        public void UpdateRoomList()
        {
            OnlineManager.Net.Invoke("UpdateRoomList");
        }

        [RPCResponse]
        private void OnUpdateRoomList(RoomListData data)
        {
            if (data != null || data.rooms.Count == 0)
            {
                m_listRoom = data.rooms;
                for (int i = 0; i < m_listRoom.Count; i++)
                {
                    Debuger.Log(m_listRoom[i].ToString());
                }

                onUpdateRoomList.Invoke(m_listRoom);
            }
            else
            {
                m_listRoom = new List<RoomData>();
                Debuger.LogWarning("房间列表为空！");
            }
        }

        public List<RoomData> GetRoomList()
        {
            return m_listRoom;
        }


        //=======================================================================

        /// <summary>
        /// 创建房间
        /// </summary>
        [RPCRequest]
        public void CreateRoom(string name)
        {
            OnlineManager.Net.Invoke("CreateRoom", name);
        }

        [RPCResponse]
        private void OnCreateRoom(RoomData data)
        {
            Debuger.Log(data.ToString());

            m_currRoom = data;
            m_isInRoom = true;

            onJoin.Invoke();

            onUpdateRoomInfo.Invoke(m_currRoom);
        }


        //=======================================================================

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        [RPCRequest]
        public void JoinRoom(uint roomId)
        {
            OnlineManager.Net.Invoke("JoinRoom", roomId);
        }

        [RPCResponse]
        private void OnJoinRoom(RoomData data)
        {
            Debuger.Log(data.ToString());
            m_currRoom = data;
            m_isInRoom = true;
            onJoin.Invoke();
            onUpdateRoomInfo.Invoke(m_currRoom);
        }


        private void OnJoinRoomError(string msg, uint roomId)
        {
            Debuger.LogError(msg);
        }

        //=======================================================================

        /// <summary>
        /// 退出房间
        /// </summary>
        [RPCRequest]
        public void ExitRoom()
        {
            OnlineManager.Net.Invoke("ExitRoom", m_currRoom.id);

            m_isReady = false;
            m_isInRoom = false;

            onExit.Invoke();
        }


        //=======================================================================

        [RPCNotify]
        private void NotifyRoomUpdate(RoomData data)
        {
            Debuger.Log(data.ToString());
            m_currRoom = data;
            List<PlayerData> players = m_currRoom.players;

            m_isInRoom = false;
            m_isReady = false;
            m_isAllReady = true;

            for (int i = 0; i < players.Count; ++i)
            {
                if (players[i].userId == m_mainUserId)
                {
                    m_isInRoom = true;
                    m_isReady = players[i].isReady;
                }

                if (!players[i].isReady)
                {
                    m_isAllReady = false;
                }
            }

            Debuger.Log("Player Count: {0}", players.Count);
            onUpdateRoomInfo.Invoke(m_currRoom);
        }


        //=======================================================================


        /// <summary>
        /// 房间准备、取消准备
        /// </summary>
        [RPCRequest]
        public void RoomReady(bool ready)
        {
            OnlineManager.Net.Invoke("RoomReady", m_currRoom.id, ready);
        }

        private void OnRoomReadyError(string msg, uint roomId)
        {
            Debuger.LogError(msg);


        }

        //=======================================================================

        [RPCRequest]
        public void StartGame()
        {
            OnlineManager.Net.Invoke("StartGame", m_currRoom.id);
        }

        private void OnStartGameError(string msg, int roomId)
        {
            Debuger.LogError(msg);
        }


        [RPCNotify]
        private void NotifyGameStart(PVPStartParam param)
        {
            Debuger.LogWarning(param.ToString());
            
            PVPStartParam startParam = param;
            //onNotifyGameStart.Invoke(startParam);
            
            ModuleManager.Instance.ShowModule(ModuleDef.GameModule, startParam);
        }

        /// <summary>
        /// 通知游戏结算
        /// </summary>
        /// <param name="args"></param>
        /// <param name="targetAddress"></param>
        [RPCNotify]
        private void NotifyGameResult(int reason)
        {
            Debuger.Log("reason:{0}", reason);
            //onNotifyGameResult.Invoke();
        }



    }
}