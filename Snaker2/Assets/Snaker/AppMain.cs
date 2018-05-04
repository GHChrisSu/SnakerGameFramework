using System;
using SGF;
using SGF.Module;
using SGF.Network.Core;
using SGF.Time;
using SGF.Unity.ILR;
using SGF.Unity.ILR.ModuleILR;
using SGF.Unity.UI;
using Snaker.GlobalUI;
using Snaker.Services.Online;
using Snaker.Services.Version;
using UnityEngine;

namespace Snaker
{
    public class AppMain:MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            //初始化时间
            SGFTime.DateTimeAppStart = DateTime.Now;

            //初始化Debuger
            InitDebuger();

            //初始化AppConfig
            AppConfig.Init();

            //初始化版本
            InitVersion();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.playmodeStateChanged -= OnEditorPlayModeChanged;
            UnityEditor.EditorApplication.playmodeStateChanged += OnEditorPlayModeChanged;
#endif
        }


#if UNITY_EDITOR
        private void OnEditorPlayModeChanged()
        {
            if (Application.isPlaying == false)
            {
                UnityEditor.EditorApplication.playmodeStateChanged -= OnEditorPlayModeChanged;
                //退出游戏逻辑
                Exit("Editor的播放模式变化！");
            }
        }
#endif

        private void Exit(string reason)
        {
            //清理模块管理器
            ModuleManager.Instance.Clean();
            //清理UI管理器
            UIManager.Instance.Clean();
            //清理在线管理器
            OnlineManager.Instance.Clean();
            //清楚IRL管理器
            ILRManager.Instance.Clean();
            //清楚版本管理器
        }


        private void InitDebuger()
        {
            //初始化Debuger的日志开关
            Debuger.Init(Application.persistentDataPath + "/DebugerLog/", new UnityDebugerConsole());
            Debuger.EnableLog = true;
            Debuger.EnableSave = true;
            Debuger.Log();
        }

        private void InitVersion()
        {
            //初始化版本管理器，并更新版本
            VersionManager.Instance.Init();
            VersionManager.onUpdateProgress += (progress) =>
            {
                AppLoading.Show("版本更新中...", progress);
            };
            VersionManager.onUpdateComplete += () =>
            {
                AppLoading.Hide();

                //当版本更新完成后，或者版本检查完成后，初始化服务层的模块
                InitServices();
            };

            

        }

        private void InitServices()
        {
            //初始化ILRManager
            ILRManager.Instance.Init(RunMode.Script, false);
            ILRManager.Instance.AddSearchDirectory(Application.streamingAssetsPath + "/ILR/");

            //如果有热更新，可能从Http下载下来，等等
            ILRManager.Instance.LoadAssembly(ModuleDef.ScriptAssemblyName);


            //初始化模块管理器
            ModuleManager.Instance.Init();
            ModuleManager.Instance.RegisterModuleActivator(new NativeModuleActivator(ModuleDef.Namespace,ModuleDef.NativeAssemblyName));
            ModuleManager.Instance.RegisterModuleActivator(new ILRModuleActivator(ModuleDef.Namespace, ModuleDef.ScriptAssemblyName));

            //初始化UI管理器
            UIManager.Instance.Init("ui/");
            UIManager.MainScene = "Main";
            UIManager.MainPage = "Home/UIHomePage";
            UIManager.SceneLoading = "UISceneLoading";

            //初始化在线管理器
            OnlineManager.Instance.Init();

            //显示登录界面
            AppLoginPanel.Show();

            //如果登录成功了，初始化普通业务模块
            GlobalEvent.onLoginSuccess += OnLoginSuccess;
            GlobalEvent.onLoginFailed += OnLoginFailed;

        }


        private void OnLoginSuccess()
        {
            GlobalEvent.onLoginSuccess -= OnLoginSuccess;

            //隐藏登录界面
            AppLoginPanel.Hide();


            //直接初始化业务层模块
            //ModuleManager.Instance.CreateModule(ModuleDef.HomeModule);
            //ModuleManager.Instance.CreateModule(ModuleDef.PVEModule);
            //ModuleManager.Instance.CreateModule(ModuleDef.PVPModule);
            //ModuleManager.Instance.CreateModule(ModuleDef.RoomModule);
            //ModuleManager.Instance.CreateModule(ModuleDef.GameModule);
            
            //ModuleManager.Instance.ShowModule(ModuleDef.HomeModule);

            //通过ILRScript来启动业务模块

            
            bool ret = (bool)ILRManager.Instance.Invoke("Snaker.ScriptMain", "Init");
            if (!ret)
            {
                UIAPI.ShowMsgBox("初始化失败", "初始化热更业务模块", "确定", (arg) =>
                {
                    AppLoginPanel.Show();
                });
            }
            
        }

        private void OnLoginFailed(int code, string info)
        {
//            GlobalEvent.onLoginFailed -= OnLoginFailed;
            AppLoginPanel.Hide();
            UIAPI.ShowMsgBox("登录失败", info, "确定", (arg) =>
            {
                AppLoginPanel.Show();
            });

            //显示失败信息
        }



        private void Update()
        {
            GlobalEvent.onUpdate.Invoke();
        }

        private void FixedUpdate()
        {
            GlobalEvent.onFixedUpdate.Invoke();
        }


    }
}