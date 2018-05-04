using System;
using SGF;
using SGF.Server;
using SGF.Time;
using Snaker.GlobalData.Server;


namespace Snaker.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            InitDebuger();
            SGFTime.DateTimeAppStart = DateTime.Now;
            

            ServerManager.Instance.Init("Snaker.Server");
            ServerManager.Instance.StartServer(ServerID.ZoneServer);
            
            MainLoop.Run();

            ServerManager.Instance.StopAllServer();
            Console.WriteLine("GameOver");
            Console.ReadKey();
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
