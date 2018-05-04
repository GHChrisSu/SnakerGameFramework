using SGF.Unity.UI;
using SGF.Unity.UI.UILib.Control;
using Snaker.GlobalData.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Snaker.Modules.Room.UI
{
    public class CtlPlayerItem:CtlListItem
    {
        [SerializeField]
        private Text m_txtInfo;

        [SerializeField]
        private UIBehaviour m_ctlReady;

        [SerializeField]
        private PlayerData m_data;

        public override void UpdateItem(int index, object data)
        {
            m_data = data as PlayerData;
            if (m_data != null)
            {
                m_txtInfo.text = "[" + m_data.id + "] " + m_data.name + "(UID:" + m_data.userId + ") ";
                UIUtils.SetActive(m_ctlReady, m_data.isReady);
            }
        }

        void OnGUI()
        {
            if (m_data != null)
            {
                UIUtils.SetActive(m_ctlReady, m_data.isReady);
            }
        }

    }
}