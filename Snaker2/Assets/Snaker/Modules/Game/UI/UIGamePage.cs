using SGF.Unity.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Snaker.Modules.Game.UI
{
    public class UIGamePage:UIPage
    {
        private GameManager m_mgrGame;
        private Text m_txtResult;
        private InputField m_txtCmd;
        private InputField m_txtArg;

        protected override void OnOpen(object arg = null)
        {
            base.OnOpen(arg);

            m_mgrGame = arg as GameManager;

            this.AddUIClickListener("pnlInput/BtnGameBegin", OnBtnGameBegin);
            this.AddUIClickListener("pnlInput/BtnRoundBegin", OnBtnRoundBegin);
            this.AddUIClickListener("pnlInput/BtnControlStart", OnBtnControlStart);
            this.AddUIClickListener("pnlInput/BtnRoundEnd", OnBtnRoundEnd);
            this.AddUIClickListener("pnlInput/BtnGameEnd", OnBtnGameEnd);
            this.AddUIClickListener("pnlInput/BtnGameExit", OnBtnGameExit);
            this.AddUIClickListener("pnlInput/BtnLogicCmd", OnBtnLogicCmd);

            m_txtCmd = this.transform.Find("pnlInput/txtCmd").GetComponent<InputField>();
            m_txtArg = this.transform.Find("pnlInput/txtArg").GetComponent<InputField>();


            m_txtResult = this.transform.Find("pnlRender/txtResult").GetComponent<Text>();

        }

        private int m_cmd;
        private int m_arg;


        protected override void OnUpdate()
        {
            base.OnUpdate();

            var frame = m_mgrGame.CurrentFrame;
            if (frame != null)
            {
                m_txtResult.text = frame.ToString();
            }
            
        }

        private void OnBtnGameBegin()
        {
            m_mgrGame.GameBegin();
        }

        private void OnBtnRoundBegin()
        {
            m_mgrGame.RoundBegin();
        }

        private void OnBtnControlStart()
        {
            m_mgrGame.ControlStart();
        }

        private void OnBtnRoundEnd()
        {
            m_mgrGame.RoundEnd();
        }

        private void OnBtnGameEnd()
        {
            m_mgrGame.GameEnd();
        }

        private void OnBtnGameExit()
        {
            m_mgrGame.GameExit();
        }

        private void OnBtnLogicCmd()
        {
            int cmd = 1;
            int.TryParse(m_txtCmd.text, out cmd);

            int arg = 1;
            int.TryParse(m_txtArg.text, out arg);

            m_mgrGame.SendCommand(cmd, arg);
        }

        


    }
}