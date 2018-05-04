using SGF;
using SGF.Module;
using SGF.Unity.UI;
using Snaker.Modules.Home;
using Snaker.Modules.Home.UI;

namespace Snaker.Modules
{
    public class HomeModule:GeneralModule
    {
        public void TryReLogin()
        {
            //需要利用Unity的特性，从新启动App
        }

		protected override void Show (object arg)
		{
			//UIManager.Instance.EnterMainPage<UIHomePage>();
            Debuger.Log();
		    //UIManager.Instance.OpenPageInScene("Main", UIDef.UIHomePage);
		    //UIManager.Instance.EnterMainPage();
		    UIManager.Instance.OpenPage(UIDef.UIHomePage);
        }


    }
}
