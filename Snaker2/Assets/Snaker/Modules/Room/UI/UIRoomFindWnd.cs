using System.Collections.Generic;
using SGF.Unity.UI;
using SGF.Unity.UI.UILib.Control;
using Snaker.GlobalData.Data;
using Snaker.Modules;
using UnityEngine.UI;

namespace Assets.Snaker.Modules.Room.UI
{
    public class UIRoomFindWnd : UIWindow
    {
        public Button btnCreate;
        public Text txtRoomName;

        public CtlList ctlRoomList;
        private RoomManager m_mgrRoom;


        protected override void OnOpen(object arg)
        {
            base.OnOpen(arg);
            m_mgrRoom = arg as RoomManager;

            btnCreate.onClick.AddListener(OnBtnCreateRoom);
            CtlRoomItem.onItemJoinClick.AddListener(OnItemJoinClick);


            m_mgrRoom.onUpdateRoomList.AddListener(OnUpdateRoomList);
            m_mgrRoom.UpdateRoomList();

            ctlRoomList.Clear();
        }

        protected override void OnClose(object arg = null)
        {
            CtlRoomItem.onItemJoinClick.RemoveAllListeners();
            btnCreate.onClick.RemoveListener(OnBtnCreateRoom);

            if (m_mgrRoom != null)
            {
                m_mgrRoom.onUpdateRoomList.RemoveListener(OnUpdateRoomList);
                m_mgrRoom = null;
            }

            base.OnClose(arg);
        }

        private void OnItemJoinClick(RoomData data)
        {
            m_mgrRoom.JoinRoom(data.id);
            this.Close();
        }

        public void OnBtnCreateRoom()
        {
            string name = txtRoomName.text;
            m_mgrRoom.CreateRoom(name);
            this.Close();
        }

        private void OnUpdateRoomList(List<RoomData> listRoom)
        {
            ctlRoomList.SetData(listRoom);
        }
    }
}