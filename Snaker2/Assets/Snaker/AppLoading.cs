using SGF.Unity.UI;
using SGF.Unity.UI.UILib.Control;
using UnityEngine;
using UnityEngine.UI;

namespace Snaker
{
    public class AppLoading:MonoBehaviour
    {

        public Text txtTitle;
        public Text txtTips;
        public CtlProgressBar progressBar;

        private static AppLoading ms_instance;

        private void Destroy()
        {
            ms_instance = null;
        }


        public static void Show(string title, float progress)
        {
            ms_instance = UIRoot.Find<AppLoading>("AppLoading");


            if (ms_instance != null)
            {
                if (!ms_instance.gameObject.activeSelf)
                {
                    ms_instance.gameObject.SetActive(true);
                }
                ms_instance.ShowProgress(title, progress);
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

        private void ShowProgress(string title, float progress)
        {
            if (txtTitle != null)
            {
                txtTitle.text = title + "(" + (int)(progress * 100) + "%)";
            }


            if (progressBar != null)
            {
                progressBar.SetData(progress);
            }
      

        }

    }
}