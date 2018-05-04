using System;
using SGF.Event;
using SGF.Network.Core;

namespace Snaker
{

    /// <summary>
    /// 全局事件
    /// 有些事件不确定应该是由谁发出
    /// 就可以通过全局事件来收和发
    /// </summary>
    public static class GlobalEvent
    {
        public static SGFEvent onUpdate = new SGFEvent();
        public static SGFEvent onFixedUpdate = new SGFEvent();

        /// <summary>
        /// 登录成功
        /// </summary>
        public static SGFEvent onLoginSuccess = new SGFEvent();

        /// <summary>
        /// 登录失败
        /// </summary>
        public static SGFEvent<int,string> onLoginFailed = new SGFEvent<int, string>();


    }
}