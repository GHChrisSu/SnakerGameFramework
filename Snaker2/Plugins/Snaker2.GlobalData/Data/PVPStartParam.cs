using System.Collections.Generic;
using ProtoBuf;
using SGF.Network.FSPLite;

namespace Snaker.GlobalData.Data
{
    [ProtoContract]
    public class PVPStartParam
    {
        [ProtoMember(1)] public FSPParam fspParam = new FSPParam();
        [ProtoMember(2)] public GameParam gameParam = new GameParam();
        [ProtoMember(3)] public List<PlayerData> players = new List<PlayerData>();
    }

    /// <summary>
    /// 游戏的启动参数
    /// </summary>
    [ProtoContract]
    public class GameParam
    {
        /// <summary>
        /// GameId,服务上可能同时开始多场游戏
        /// 每一局游戏都有一个编号，在PVP会用到
        /// </summary>
        [ProtoMember(1)] public uint id = 0;

        /// <summary>
        /// 地图数据，决定这场游戏用哪个地图
        /// </summary>
        [ProtoMember(2)] public MapData mapData = new MapData();


        /// <summary>
        /// 随机数种子，用于在不同的客户端产生相同的随机数
        /// </summary>
        [ProtoMember(3)] public int randSeed = 0;


        /// <summary>
        /// 游戏的模式
        /// </summary>
        [ProtoMember(4)] public GameMode mode = GameMode.EndlessPVE;


        /// <summary>
        /// 限时模式的时间
        /// </summary>
        [ProtoMember(5)] public int limitedTime = 180;//Second
    }


    /// <summary>
    /// 游戏模式
    /// </summary>
    public enum GameMode
    {
        EndlessPVE,
        TimelimitPVE,
        EndlessPVP,
        TimelimitPVP
    }


    [ProtoContract]
    public class MapData
    {
        /// <summary>
        /// 地图的ID，通过ID 可以找到地图的资源
        /// </summary>
        [ProtoMember(1)] public int id = 0;
        /// <summary>
        /// 地图的名字，用于在UI中显示
        /// </summary>
        [ProtoMember(2)] public string name = "";

    }

}