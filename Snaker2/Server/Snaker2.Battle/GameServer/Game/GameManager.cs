using System.Collections.Generic;
using SGF.Common;
using SGF.Network.FSPLite;
using Snaker.GlobalData.Data;

namespace Snaker2.Battle.GameServer.Game
{
    public class GameManager:Singleton<GameManager>
    {
        private ServerContext m_context;
        


        public void Init(ServerContext context)
        {
            m_context = context;
            m_context.ipc.AddRPCListener(this);
        }

        public void Clean()
        {
            if (m_context != null)
            {
                m_context.ipc.RemoveRPCListener(this);
                m_context = null;
            }
        }

        private void StartGame(int src, GamesServerStartParam param)
        {
            m_context.fsp.CreateGame(param.roomId, param.authId);
            param.listPlayerFSPSessionId = m_context.fsp.AddPlayer(param.roomId, param.listPlayerId);

            FSPParam fspParam = m_context.fsp.GetParam();
            fspParam.authId = param.authId;
            m_context.ipc.Return(fspParam, param);

        }
    }
}