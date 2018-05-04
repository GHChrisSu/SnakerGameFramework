using SGF;
using SGF.Module;
using SGF.Unity.ILR.UIILR;
using SGF.Unity.UI;
using Snaker.GlobalData.Data;
using Snaker.GlobalUI;
using Snaker.Services.Online;
using UnityEngine.UI;

namespace Snaker.Modules.Home.UI
{
    public class UIHomePage:ILRUIPage
    {
        private Text m_txtUserInfo;

        protected override void OnOpen(object arg = null)
        {
            base.OnOpen(arg);

            //Debuger.LogWarning("UIHomePage打开了~");

            m_txtUserInfo = this.Base.Find<Text>("txtUserInfo");

            UpdateUserInfo();
        }

        protected override void OnClose(object arg = null)
        {
            
            base.OnClose(arg);
        }


        private void UpdateUserInfo()
        {
            UserData ud = OnlineManager.Instance.MainUserData;
            m_txtUserInfo.text = ud.name + "(Lv."+ud.level+")";
        }


        public void OnBtnUserInfo()
        {
            UIAPI.ShowMsgBox("重新登录", "是否重新登录？", "确定|取消", o =>
            {
                if ((int) o == 0)
                {
                    HomeModule module = ModuleManager.Instance.GetModule(ModuleDef.HomeModule) as HomeModule;
                    module.TryReLogin();
                }
            });
        }




        public void OnBtnSetting()
        {
            //OpenModule(ModuleDef.SettingModule);
        }

        public void OnBtnDailyCheckIn()
        {
            //OpenModule(ModuleDef.DailyCheckInModule);
        }

        public void OnBtnActivity()
        {
            //OpenModule(ModuleDef.ActivityModule);
        }

        public void OnBtnBuyCoin()
        {
            //OpenModule(ModuleDef.ShopModule, "BuyCoin");
        }

        public void OnBtnFreeCoin()
        {
            //OpenModule(ModuleDef.ShareModule);
        }


        public void OnBtnEndlessPVE()
        {
            //ModuleManager.Instance.ShowModule(ModuleDef.PVEModule, (int)GameMode.EndlessPVE);
        }

        public void OnBtnTimelimitPVE()
        {
            //ModuleManager.Instance.ShowModule(ModuleDef.PVEModule, (int)GameMode.TimelimitPVE);
            
        }

        public void OnBtnPVP()
        {
            ModuleManager.Instance.ShowModule(ModuleDef.RoomModule);
            
        }


    }
}
