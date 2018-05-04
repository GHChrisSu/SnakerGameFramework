using ProtoBuf;
using SGF;
using SGF.Codec;
using SGF.Utils;
using Snaker.GlobalData.Data;
using UnityEngine;

namespace Snaker
{
    /// <summary>
    /// App的配置定义
    /// </summary>
    [ProtoContract]
    public class AppConfig
    {

        /// <summary>
        /// 主用户数据
        /// </summary>
        [ProtoMember(1)] public UserData mainUserData = new UserData();
        [ProtoMember(2)] public bool enableBgMusic = true;
        [ProtoMember(3)] public bool enableSoundEffect = true;


        private static AppConfig m_value = new AppConfig();
        public static AppConfig Value { get { return m_value; } }


#if UNITY_EDITOR
        public readonly static string Path = Application.persistentDataPath + "/AppConfig_Editor.data";
#else
        public readonly static string Path = Application.persistentDataPath + "/AppConfig.data";
#endif




        public static void Init()
        {
            Debuger.Log("Path = " + Path);
            //加载配置

            var data = FileUtils.ReadFile(Path);
            if (data != null && data.Length > 0)
            {
                var cfg = PBSerializer.NDeserialize(data, typeof(AppConfig));
                if (cfg != null)
                {
                    m_value = cfg as AppConfig;
                }
            }
        }

        public static void Save()
        {
            Debuger.Log("Value = " + m_value);

            if (m_value != null)
            {
                byte[] data = PBSerializer.NSerialize(m_value);
                FileUtils.SaveFile(Path, data);
            }
        }
    }
}