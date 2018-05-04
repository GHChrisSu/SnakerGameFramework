﻿using SGF.Unity.UI;

namespace SGF.Unity.ILR.UIILR
{
    public class ILRUIPanel
    {
        private UIPanel m_base;
        public UIPanel Base{get { return m_base; }}



        protected virtual void OnAwake() { }

        internal void OnAwakeInternal(UIPanel _base)
        {
            m_base = _base;
            OnAwake();
        }


        protected virtual void OnDestroy() { }
        internal void OnDestroyInternal() { OnDestroy(); }


        protected virtual void OnOpen(object arg = null) { }
        internal void OnOpenInternal(object arg) { OnOpen(arg); }

        protected virtual void OnClose(object arg = null) { }
        internal void OnCloseInternal(object arg) { OnClose(arg); }



        protected virtual void OnEnable() { }
        internal void OnEnableInternal() { OnEnable(); }

        protected virtual void OnDisable() { }
        internal void OnDisableInternal() { OnDisable(); }


    }






    public class ILRUIPanelBridge : UIPanel
    {

        public string AssemblyName = "";
        public string Namespace = "";
        public string TypeName = "";


        private ILRUIPanel m_impl;

        protected override void OnAwake()
        {
            base.OnAwake();

            string fullName = Namespace + "." + TypeName;
            m_impl = ILRManager.CreateInstance(fullName, AssemblyName) as ILRUIPanel;
            if (m_impl == null)
            {
                Debuger.LogError("无法在Assembly[{0}]中创建实例:{1}", AssemblyName, fullName);
            }
            else
            {
                m_impl.OnAwakeInternal(this);
            }
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_impl != null)
            {
                m_impl.OnDestroyInternal();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_impl != null)
            {
                m_impl.OnEnableInternal();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (m_impl != null)
            {
                m_impl.OnDisableInternal();
            }
        }


        protected override void OnOpen(object arg = null)
        {
            base.OnOpen(arg);
            if (m_impl != null)
            {
                m_impl.OnOpenInternal(arg);
            }
        }

        protected override void OnClose(object arg = null)
        {
            base.OnClose(arg);
            if (m_impl != null)
            {
                m_impl.OnCloseInternal(arg);
            }
        }


    }
}