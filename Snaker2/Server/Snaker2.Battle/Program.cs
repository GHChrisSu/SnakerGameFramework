using System;
using SGF;
using SGF.Server;
using SGF.Time;
using Snaker.GlobalData.Server;


namespace Snaker2.Battle
{
    class Program
    {
        static void Main(string[] args)
        {
            InitDebuger();
            SGFTime.DateTimeAppStart = DateTime.Now;


            ServerManager.Instance.Init("Snaker2.Battle");
            ServerManager.Instance.StartServer(ServerID.GameServer);
            

            MainLoop.Run();

            ServerManager.Instance.StopAllServer();
        }

        private static void InitDebuger()
        {
            Debuger.Init(AppDomain.CurrentDomain.BaseDirectory + "/ServerLog/");
            Debuger.EnableLog = true;
            Debuger.EnableSave = true;
            Debuger.Log();
        }
    }
}
