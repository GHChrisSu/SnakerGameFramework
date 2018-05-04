using System;
using SGF;
using SGF.Common;
using SGF.Event;
using SGF.Module;
using SGF.Time;

namespace Snaker.Services.Version
{
    public class VersionManager: Singleton<VersionManager>
    {
        public static SGFEvent onUpdateComplete = new SGFEvent();
        public static SGFEvent<float> onUpdateProgress = new SGFEvent<float>();
        private float m_progress = 0;

        public void Init()
        {
            GlobalEvent.onUpdate.AddListener(OnUpdate);
        }


        public void Clean()
        {
            GlobalEvent.onUpdate.RemoveListener(OnUpdate);
        }

        private void OnUpdate()
        {
            m_progress += 0.05f;
            if (m_progress > 1)
            {
                m_progress = 1;
            }
            onUpdateProgress.Invoke(m_progress);

            Console.Write("模拟版本更新:" + (int)(m_progress * 100) + "%\r");

            if (m_progress >= 1)
            {
                Console.WriteLine();
                GlobalEvent.onUpdate.RemoveListener(OnUpdate);
                onUpdateComplete.Invoke();
            }
        }
    }
}