using SGF.Unity.UI;
using Snaker.Services.Online;
using UnityEngine;
using UnityEngine.UI;

namespace Snaker
{
    public class AppLoginPanel:MonoBehaviour
    {
        private static AppLoginPanel ms_instance;

        public Text txtName;
        public Button btnLogin;

        public static void Show()
        {
            ms_instance = UIRoot.Find<AppLoginPanel>("AppLoginPanel");

            if (ms_instance != null)
            {
                if (!ms_instance.gameObject.activeSelf)
                {
                    ms_instance.gameObject.SetActive(true);
                }
                ms_instance.txtName.text = AppConfig.Value.mainUserData.name;
            }

        }

        public static void Hide()
        {

            if (ms_instance != null)
            {
                if (ms_instance.gameObject.activeSelf)
                {
                    ms_instance.gameObject.SetActive(false);
                }
            }
        }


        public void OnBtnLogin()
        {
            string name = txtName.text;
            OnlineManager.Instance.Login(name);
        }
    }
}