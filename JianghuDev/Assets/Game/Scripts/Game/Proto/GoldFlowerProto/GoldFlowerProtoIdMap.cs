using UnityEngine;
using System.Collections;

public class GoldFlowerProtoIdMap {

    #region 主动

    public const int CMD_SendCreateRoom = 3502;//创建房间

    public const int CMD_SendJoinRoom = 3503;//加入房间

    public const int CMD_SendRready = 3504;//准备

    public const int CMD_SendOpt = 3505;//操作

    public const int CMD_SendShowCard = 3506;//亮牌

    public const int CMD_SendLeaveRoom = 3507;//离开房间

    public const int CMD_SendGetPlayerDistances = 3508;//查看玩家之间的距离

    public const int CMD_SendGetPlayerInfo = 3509;//查看玩家信息

    public const int CMD_SendAddress = 3510;//发送定位信息

    public const int CMD_SendGameTalk = 3511;//发送聊天

    public const int CMD_SendAutoGen = 3512;//自动跟注

    public const int CMD_SendJoinGoldPattern = 3550;//加入金币场

    public const int CMD_SendChangGoldRoom = 3551;//切换房间

    public const int CMD_SendStartGame = 3513;//开始游戏

    #endregion


    #region 被动

    public const int CMD_OnGoldLess = 3601;//金币不足

    public const int CMD_OnPlayerLeave = 3602;//玩家离开房间

    public const int CMD_OnPlayerSeatDown = 3603;//加入房间

    public const int CMD_OnPlayerReady = 3604;//玩家准备

    public const int CMD_OnGameStart = 3605;//游戏开始

    public const int CMD_OnGameOptResult = 3606;//同步谁操作指令

    public const int CMD_OnGameStartLastTime = 3607;//同步倒计时

    public const int CMD_OnSelfGameOpt = 3608;//可操作列表

    public const int CMD_OnRoomCardLess = 3609;//房卡不足

    public const int CMD_OnSamllSettle = 3611;//小结算

    public const int CMD_OnPlayerOnOffLine = 3612;//玩家上下线

    public const int CMD_OnPlayerShowCard = 3613;//有玩家亮牌

    public const int CMD_OnUpdateRound = 3615;//更新轮数

    public const int CMD_OnGameTalk = 3616;//同步聊天信息

    public const int CMD_OnLastFight = 3617;//最后一战

    public const int CMD_OnAutoChangDesk = 3619;//自动换桌

    public const int CMD_OnJiesan = 3620;//房间解散
    public const int CMD_OnDisolveTime = 3621;//解散时间

    #endregion


}
