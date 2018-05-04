using SGF.Module;
using SGF.Unity.UI;
using Snaker.Modules.Room;

namespace Snaker.Modules
{
    public class RoomModule:GeneralModule
    {
        private RoomManager m_mgrRoom;

        public override void Create(object args = null)
        {
            base.Create(args);
            m_mgrRoom = new RoomManager();
            m_mgrRoom.Init();
        }

        public override void Release()
        {
            if (m_mgrRoom != null)
            {
                m_mgrRoom.Clean();
                m_mgrRoom = null;
            }

            base.Release();
        }


        protected override void Show(object arg)
        {
            base.Show(arg);

            m_mgrRoom.Reset();

            //显示主UI
            UIManager.Instance.OpenPage(UIDef.UIRoomPage, m_mgrRoom);
        }
    }
}