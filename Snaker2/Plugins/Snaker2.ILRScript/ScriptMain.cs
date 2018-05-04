using SGF;
using SGF.Module;

namespace Snaker
{
    public class ScriptMain
    {
        public static bool Init()
        {
            Debuger.Log();

            ModuleManager.Instance.CreateModule(ModuleDef.HomeModule);
            //ModuleManager.Instance.CreateModule(ModuleDef.PVEModule);
            //ModuleManager.Instance.CreateModule(ModuleDef.PVPModule);
            //ModuleManager.Instance.CreateModule(ModuleDef.RoomModule);
            //ModuleManager.Instance.CreateModule(ModuleDef.GameModule);

            ModuleManager.Instance.ShowModule(ModuleDef.HomeModule);

            return true;
        }
    }
}