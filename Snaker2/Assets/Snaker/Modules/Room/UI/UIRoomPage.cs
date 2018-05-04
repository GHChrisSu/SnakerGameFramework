using System.Collections.Generic;
using SGF.Math;
using SGF.Unity.UI;
using SGF.Unity.UI.UILib.Control;
using Snaker.GlobalData.Data;
using Snaker.Modules;
using Snaker.Modules.Room;
using UnityEngine.UI;

namespace Assets.Snaker.Modules.Room.UI
{
    public class UIRoomPage:UIPage
    {

        public Button btnJoinRoom;
        public Button btnRoomReady;
        public Button btnStartGame;

        public CtlList ctlPlayerList;
        private RoomManager m_mgrRoom;


        protected override void OnOpen(object arg)
        {
            base.OnOpen(arg);
            m_mgrRoom = arg as RoomManager;

            btnJoinRoom.onClick.AddListener(OnBtnJoinRoom);
            btnRoomReady.onClick.AddListener(OnBtnRoomReady);
            btnStartGame.onClick.AddListener(OnBtnStartGame);


            m_mgrRoom.onJoin.AddListener(OnJoinRoom);
            m_mgrRoom.onExit.AddListener(OnExitRoom);
            m_mgrRoom.onUpdateRoomInfo.AddListener(OnRoomUpdate);
            ctlPlayerList.Clear();
        }

        protected override void OnClose(object arg = null)
        {
            btnStartGame.onClick.RemoveListener(OnBtnStartGame);
            btnJoinRoom.onClick.RemoveListener(OnBtnJoinRoom);
            btnRoomReady.onClick.RemoveListener(OnBtnRoomReady);

            if (m_mgrRoom != null)
            {
                if (m_mgrRoom.IsInRoom)
                {
                    m_mgrRoom.ExitRoom();
                }

                m_mgrRoom.onJoin.RemoveListener(OnJoinRoom);
                m_mgrRoom.onExit.RemoveListener(OnExitRoom);
                m_mgrRoom.onUpdateRoomInfo.RemoveListener(OnRoomUpdate);
                m_mgrRoom = null;
            }

            base.OnClose(arg);
        }

        private void OnRoomUpdate(RoomData data)
        {
            ctlPlayerList.SetData(data.players);
        }

        private void OnBtnJoinRoom()
        {
            if (UIUtils.GetButtonText(btnJoinRoom) == "加入房间")
            {
                UIManager.Instance.OpenWindow(UIDef.UIRoomFindWnd, m_mgrRoom);
            }
            else
            {
                m_mgrRoom.ExitRoom();
            }
        }

        private void OnJoinRoom()
        {

        }

        private void OnExitRoom()
        {
            ctlPlayerList.Clear();
        }



        private void OnBtnRoomReady()
        {
            if (UIUtils.GetButtonText(btnRoomReady) == "开始准备")
            {
                m_mgrRoom.RoomReady(true);
            }
            else
            {
                m_mgrRoom.RoomReady(false);
            }
        }

        private void OnBtnStartGame()
        {
            m_mgrRoom.StartGame();

        }

        /// <summary>
        /// Raises the button test event.
        /// </summary>
        public void OnBtnTest()
        {
            List<PlayerData> list = new List<PlayerData>();
            for (int i = 0; i < 20; ++i)
            {
                PlayerData data = new PlayerData();
                data.name = "名字";
                data.id = (uint)i + 1;
                data.userId = (uint)SGFRandom.Default.Range(100000, 999999);
                data.sid = data.id;
                data.isReady = SGFRandom.Default.Rnd() > 0.5f;
                list.Add(data);
            }

            ctlPlayerList.SetData(list);
        }




        void Update()
        {
            if (m_mgrRoom != null)
            {
                UIUtils.SetButtonText(btnJoinRoom, m_mgrRoom.IsInRoom ? "退出房间" : "加入房间");
                UIUtils.SetButtonText(btnRoomReady, m_mgrRoom.IsReady ? "取消准备" : "开始准备");

                //UIUtils.SetActive(btnJoinRoom, !m_mgrRoom.IsReady);
                btnJoinRoom.enabled = !m_mgrRoom.IsReady;
                UIUtils.SetActive(btnRoomReady, m_mgrRoom.IsInRoom);
                UIUtils.SetActive(btnStartGame, m_mgrRoom.IsRoomOwner && m_mgrRoom.IsAllReady);
            }
        }

    }
}