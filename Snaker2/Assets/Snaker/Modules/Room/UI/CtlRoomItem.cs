using SGF.Event;
using SGF.Unity.UI.UILib.Control;
using Snaker.GlobalData.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Snaker.Modules.Room.UI
{
    public class CtlRoomItem:CtlListItem
    {
        public static SGFEvent<RoomData> onItemJoinClick = new SGFEvent<RoomData>();

        [SerializeField]
        private Text m_txtInfo;

        [SerializeField]
        private Button m_btnJoin;


        [SerializeField]
        private RoomData m_data;

        public override void UpdateItem(int index, object data)
        {
            m_data = data as RoomData;
            if (m_data != null)
            {
                m_txtInfo.text = "[" + m_data.id + "] " + m_data.name + "(人数:" + m_data.players.Count + ") ";
                m_btnJoin.onClick.RemoveAllListeners();
                m_btnJoin.onClick.AddListener(OnBtnJoin);
            }
        }

        private void OnBtnJoin()
        {
            onItemJoinClick.Invoke(m_data);
        }

        void OnGUI()
        {
            if (m_data != null)
            {

            }
        }
    }
}