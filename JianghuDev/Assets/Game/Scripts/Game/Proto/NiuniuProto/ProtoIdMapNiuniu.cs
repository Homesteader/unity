using UnityEngine;
using System.Collections;

public class NNProtoIdMap  {


    #region 主动

    public const int CMD_SendCreateRoom = 4002; //创建房间

    public const int CMD_SendJoinRoom = 4003;//加入房间

    public const int CMD_SendReady = 4004;//准备开始

    public const int CMD_SendGameOpt = 4005;//发送指令 1是下注 2是亮牌 3是抢庄

    public const int CMD_SendShowCard = 4006;//亮牌

    public const int CMD_SendLeaveRoom = 4007;//离开房间

    public const int CMD_SendGetDistances = 4008;//获取玩家距离

    public const int CMD_SendGetUserInfo = 4009;//获取玩家信息

    public const int CMD_SendAddress = 4010;//更新地位信息

    public const int CMD_SendGameTalk = 4011;//游戏内部聊天


    public const int CMD_SendJoinGoldPattern = 4050;//进入平台房间

    public const int CMD_SendChangDesk = 4051;//换桌

    #endregion


    #region 被动

    public const int CMD_OnGoldLess = 4101;//金币不足

    public const int CMD_OnPlayerLeave = 4102;//有玩家离开房间

    public const int CMD_OnPlayerSeatDown = 4103;//有玩家坐下

    public const int CMD_OnPlayerReady = 4104;//有玩家准备

    public const int CMD_OnGameStart = 4105;//游戏开始

    public const int CMD_OnPlayerOptResult = 4106;//同步谁操作了指令

    public const int CMD_OnGameStartLastTime = 4107;//游戏开始倒计时

    public const int CMD_OnSelfGameOpt = 4108;//获得可操作列表

    public const int CMD_OnRoomCardLess = 4109;//房卡不足

    public const int CMD_OnSmallSettle = 4111;//小结算

    public const int CMD_OnPlayerOnOffLine = 4112;//玩家上下线

    public const int CMD_OnCastCard = 4113;//发牌

    public const int CMD_OnChangZhuang = 4115;//换庄

    public const int CMD_OnGameTalk = 4116;//同步玩家聊天

    public const int CMD_OnAutoChangDesk = 4117;//自动换桌

    #endregion


}

