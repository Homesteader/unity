using UnityEngine;
using System.Collections;


public class TenProtoIdMap
{


    #region 主动

    public const int CMD_SendCreateRoom = 5002; //创建房间

    public const int CMD_SendJoinRoom = 5003;//加入房间

    public const int CMD_SendReady = 5004;//准备开始

    public const int CMD_SendGameOpt = 5005;//发送指令 1是下注 2是亮牌 3是抢庄

    public const int CMD_SendShowCard = 5006;//亮牌

    public const int CMD_SendLeaveRoom = 5007;//离开房间

    public const int CMD_SendGetDistances = 5008;//获取玩家距离

    public const int CMD_SendGetUserInfo = 5009;//获取玩家信息

    public const int CMD_SendAddress = 5010;//更新地位信息

    public const int CMD_SendGameTalk = 5011;//游戏内部聊天


    public const int CMD_SendJoinGoldPattern = 5050;//进入平台房间

    public const int CMD_SendChangDesk = 5051;//换桌

    #endregion


    #region 被动

    public const int CMD_OnGoldLess = 5101;//金币不足

    public const int CMD_OnPlayerLeave = 5102;//有玩家离开房间

    public const int CMD_OnPlayerSeatDown = 5103;//有玩家坐下

    public const int CMD_OnPlayerReady = 5104;//有玩家准备

    public const int CMD_OnGameStart = 5105;//游戏开始

    public const int CMD_OnPlayerOptResult = 5106;//同步谁操作了指令

    public const int CMD_OnGameStartLastTime = 5107;//游戏开始倒计时

    public const int CMD_OnSelfGameOpt = 5108;//获得可操作列表

    public const int CMD_OnRoomCardLess = 5109;//房卡不足

    public const int CMD_OnSmallSettle = 5111;//小结算

    public const int CMD_OnPlayerOnOffLine = 5112;//玩家上下线

    public const int CMD_OnCastCard = 5113;//发牌

    public const int CMD_OnChangZhuang = 5115;//换庄

    public const int CMD_OnGameTalk = 5116;//同步玩家聊天

    public const int CMD_OnAutoChangDesk = 5117;//自动换桌

    #endregion


}
