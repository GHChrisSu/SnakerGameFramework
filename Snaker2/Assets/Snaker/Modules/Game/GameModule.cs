using System.Diagnostics;
using SGF;
using SGF.Module;
using SGF.Unity.UI;
using Snaker.GlobalData.Data;
using Snaker.Modules.Game;


namespace Snaker.Modules
{
    public class GameModule:GeneralModule
    {
        private GameManager m_mgrGame;

        public override void Create(object args = null)
        {
            base.Create(args);

            m_mgrGame = new GameManager();
            m_mgrGame.Init();
        }

        public override void Release()
        {
            if (m_mgrGame != null)
            {
                m_mgrGame.Clean();
                m_mgrGame = null;
            }

            base.Release();
        }

        protected override void Show(object arg)
        {
            base.Show(arg);
            Debuger.Log();

            PVPStartParam param = arg as PVPStartParam;
            m_mgrGame.Start(param);

            //显示主UI
            UIManager.Instance.OpenPage(UIDef.UIGamePage, m_mgrGame);
            
        }
    }
}