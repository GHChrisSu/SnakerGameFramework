using System.Collections.Generic;
using System.Text;
using SGF.Common;
using SGF.Extension;
using SGF.Network.General.Server;
using Snaker.GlobalData.Data;

namespace Snaker.Server.ZoneServer
{
    public class Room
    {
        private static uint ms_lastRid = 0;

        public static uint NewRoomID()
        {
            return ++ms_lastRid;
        }

        //===============================================================

        private RoomData m_data;
        public RoomData data { get { return m_data; } }

        private DictionarySafe<uint, ISession> m_mapUserId2Session;

        public override string ToString()
        {
            return string.Format("<data:{0}, sessions:{1}>", m_data, m_mapUserId2Session.ToListString());
        }

        public string DumpString(string prefix = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<id:{0}, name:{1}, owner:{2}, players_count:{3}>\n",
                m_data.id, m_data.name, m_data.owner, m_data.players.Count);

            sb.AppendLine(prefix + "\tPlayerList:");
            for (int i = 0; i < m_data.players.Count; i++)
            {
                sb.AppendLine(prefix + "\t" + m_data.players[i].ToString());
            }

            sb.AppendLine(prefix + "\tSessionList:");
            foreach (var session in m_mapUserId2Session)
            {
                sb.AppendLine(prefix + "\t" + session.Value.ToString());
            }

            return sb.ToString();
        }

        //===============================================================

        public void Create(uint userId, string userName, ISession session, string roomName)
        {
            m_mapUserId2Session = new DictionarySafe<uint, ISession>();

            m_data = new RoomData();
            m_data.id = NewRoomID();
            m_data.name = roomName;
            m_data.owner = userId;

            AddPlayer(userId, userName, session);
        }

        public void AddPlayer(uint userId, string userName, ISession session)
        {
            PlayerData data = GetPlayerDataByUserId(userId);
            if (data == null)
            {
                data = new PlayerData();
                m_data.players.Add(data);
                data.sid = session.id;
            }

            for (int i = 0; i < m_data.players.Count; i++)
            {
                m_data.players[i].id = (uint)(i + 1);
            }

            data.isReady = false;
            data.userId = userId;
            data.name = userName;

            m_mapUserId2Session[userId] = session;
        }



        public void RemovePlayer(uint userId)
        {
            int i = GetPlayerIndexByUserId(userId);
            if (i >= 0)
            {
                m_data.players.RemoveAt(i);
            }

            m_mapUserId2Session.Remove(userId);

            if (userId == m_data.owner)
            {
                if (m_data.players.Count > 0)
                {
                    m_data.owner = m_data.players[0].userId;
                }
            }
        }



        public int GetPlayerCount()
        {
            return m_data.players.Count;
        }


        private int GetPlayerIndexByUserId(uint userId)
        {
            for (int i = 0; i < m_data.players.Count; i++)
            {
                if (m_data.players[i].userId == userId)
                {
                    return i;
                }
            }
            return -1;
        }

        private PlayerData GetPlayerDataByUserId(uint userId)
        {
            for (int i = 0; i < m_data.players.Count; i++)
            {
                if (m_data.players[i].userId == userId)
                {
                    return m_data.players[i];
                }
            }
            return null;
        }


        public ISession[] GetSessionList()
        {
            List<ISession> list = new List<ISession>();
            for (int i = 0; i < m_data.players.Count; i++)
            {
                uint userId = m_data.players[i].userId;
                list.Add(m_mapUserId2Session[userId]);
            }

            return list.ToArray();
        }


        public ISession GetSession(uint userId)
        {
            return m_mapUserId2Session[userId];
        }

        public bool CanStartGame()
        {
            if (m_data.players.Count > 0 && IsAllReady())
            {
                return true;
            }
            return false;
        }


        public bool IsAllReady()
        {
            bool isAllReady = true;
            for (int i = 0; i < m_data.players.Count; i++)
            {
                if (!m_data.players[i].isReady)
                {
                    isAllReady = false;
                    break;
                }
            }
            return isAllReady;
        }

        public void SetReady(uint userId, bool value)
        {
            var info = GetPlayerDataByUserId(userId);
            if (info != null)
            {
                info.isReady = value;
            }
        }


        public GameParam GetGameParam()
        {
            GameParam param = new GameParam();
            param.id = data.id;
            param.limitedTime = 180;//Second
            param.randSeed = 0;
            param.mode = GameMode.TimelimitPVP;

            param.mapData.id = 1;
            param.mapData.name = "默认地图";

            return param;
        }




    }
}