using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 创建和加入房间返回
/// </summary>
[ProtoBuf.ProtoContract]
public class TenCreateJoinRoomAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public TenCreateJoinRoomData data;
}

[ProtoBuf.ProtoContract]
public class TenCreateJoinRoomData
{
    [ProtoBuf.ProtoMember(1)]
    public TenCreateJoinRoomInfo roomInfo;
}

/// <summary>
/// 房间信息
/// </summary>
[ProtoBuf.ProtoContract]
public class TenCreateJoinRoomInfo
{
    [ProtoBuf.ProtoMember(1)]
    public SendCreateRoomReq rule;
    [ProtoBuf.ProtoMember(2)]
    public int subGameId;
    [ProtoBuf.ProtoMember(3)]
    public int gameId;
    [ProtoBuf.ProtoMember(4)]
    public int mySeatId;
    /// <summary>
    /// 房间状态 1是准备 2是下注 3是看牌 4是抢庄 
    /// </summary>
    [ProtoBuf.ProtoMember(5)]
    public int gameState;
    /// <summary>
    /// 开始没有 0是还没开始过任何一小局 1是开始过了
    /// </summary>
    [ProtoBuf.ProtoMember(6)]
    public int roomState;
    [ProtoBuf.ProtoMember(7)]
    public int timeDown;
    /// <summary>
    /// 庄家座位号
    /// </summary>
    [ProtoBuf.ProtoMember(8)]
    public int zhuangSeatId;
    [ProtoBuf.ProtoMember(9)]
    public List<TenPlayerInfo> playerList;
    [ProtoBuf.ProtoMember(10)]
    public TenPlayerHandCards handCardsList;
    [ProtoBuf.ProtoMember(11)]
    public string roomId;
    /// <summary>
    /// 是否是金币场
    /// </summary>
    [ProtoBuf.ProtoMember(12)]
    public bool pt;
    /// <summary>
    /// 抢庄家倍率
    /// </summary>
    [ProtoBuf.ProtoMember(13)]
    public int zhuangRatio;
}


[ProtoBuf.ProtoContract]
public class TenPlayerInfo
{
    [ProtoBuf.ProtoMember(1)]
    public string nickname;
    [ProtoBuf.ProtoMember(2)]
    public string headUrl;
    [ProtoBuf.ProtoMember(3)]
    public int seatId;
    [ProtoBuf.ProtoMember(4)]
    public string userId;
    [ProtoBuf.ProtoMember(5)]
    public bool isReady;
    [ProtoBuf.ProtoMember(6)]
    public int onLineState;
    [ProtoBuf.ProtoMember(7)]
    public float gold;
    [ProtoBuf.ProtoMember(8)]
    public int sex;
    [ProtoBuf.ProtoMember(9)]
    public float score;
    [ProtoBuf.ProtoMember(10)]
    public float longitude;
    [ProtoBuf.ProtoMember(11)]
    public float latitude;
    [ProtoBuf.ProtoMember(12)]
    public float totalWin;
}

[ProtoBuf.ProtoContract]
public class TenPlayerHandCards
{
    [ProtoBuf.ProtoMember(1)]
    public TenPlayerHandCardsInfo myCardsInfo;
    [ProtoBuf.ProtoMember(2)]
    public List<TenPlayerHandCardsInfo> otherCardsInfo;
}

[ProtoBuf.ProtoContract]
public class TenPlayerHandCardsInfo
{
    [ProtoBuf.ProtoMember(1)]
    public TenPlayerHandCardsData cards;
    [ProtoBuf.ProtoMember(2)]
    public TenPlayerHandCardsType cardsType;
    [ProtoBuf.ProtoMember(3)]
    public bool isLp;
    [ProtoBuf.ProtoMember(4)]
    public bool isXz;
    [ProtoBuf.ProtoMember(5)]
    public bool isQz;
    [ProtoBuf.ProtoMember(6)]
    public bool isReady;
    [ProtoBuf.ProtoMember(7)]
    public bool isZhuang;
    [ProtoBuf.ProtoMember(8)]
    public float xz;
    /// <summary>
    /// 你抢了多少
    /// </summary>
    [ProtoBuf.ProtoMember(9)]
    public int qzbs;
    [ProtoBuf.ProtoMember(10)]
    public List<int> canXzList;
    [ProtoBuf.ProtoMember(11)]
    public List<int> canQzList;
    [ProtoBuf.ProtoMember(12)]
    public int seatId;
    /// <summary>
    /// 下注的值
    /// </summary>
    [ProtoBuf.ProtoMember(13)]
    public List<float> xzListValue;
    /// <summary>
    /// 抢庄的值
    /// </summary>
    [ProtoBuf.ProtoMember(14)]
    public List<int> qzListValue;
}

[ProtoBuf.ProtoContract]
public class TenPlayerHandCardsData
{
    /// <summary>
    /// 名牌
    /// </summary>
    [ProtoBuf.ProtoMember(1)]
    public List<string> mp;
    /// <summary>
    /// 暗的牌
    /// </summary>
    [ProtoBuf.ProtoMember(2)]
    public List<string> cards;
}

[ProtoBuf.ProtoContract]
public class TenPlayerHandCardsType
{
    /// <summary>
    ///  0-13   10牛牛,11五花牛,12 炸弹牛,13 五小牛
    /// </summary>
    [ProtoBuf.ProtoMember(1)]
    public int point;
    [ProtoBuf.ProtoMember(2)]
    public TenPlayerHandCardsTypeMax max;
    /// <summary>
    /// 顺序
    /// </summary>
    [ProtoBuf.ProtoMember(3)]
    public List<int> order;
    [ProtoBuf.ProtoMember(4)]
    public List<string> cards;
    /// <summary>
    /// 倍率
    /// </summary>
    [ProtoBuf.ProtoMember(5)]
    public int ratio;
}

/// <summary>
/// 最大牌是哪张
/// </summary>
[ProtoBuf.ProtoContract]
public class TenPlayerHandCardsTypeMax
{
    [ProtoBuf.ProtoMember(1)]
    public string card;
    [ProtoBuf.ProtoMember(2)]
    public int num;
}


[ProtoBuf.ProtoContract]
public class TenSendJoinRoomReq
{
    [ProtoBuf.ProtoMember(1)]
    public string roomId;
}


/// <summary>
/// 有玩家坐下
/// </summary>
[ProtoBuf.ProtoContract]
public class TenOnPlayerSeatDown
{
    [ProtoBuf.ProtoMember(1)]
    public TenPlayerInfo player;
}

/// <summary>
/// 同步玩家离开
/// </summary>
[ProtoBuf.ProtoContract]
public class TenOnPlayerLeave
{
    [ProtoBuf.ProtoMember(1)]
    public int seatId;
}

/// <summary>
/// 同步玩家准备
/// </summary>
[ProtoBuf.ProtoContract]
public class TenOnPlayerReady
{
    [ProtoBuf.ProtoMember(1)]
    public int seatId;
}

/// <summary>
/// 开始游戏倒计时
/// </summary>
[ProtoBuf.ProtoContract]
public class TenOnGameStartLastTime
{
    [ProtoBuf.ProtoMember(1)]
    public int lastTime;
}


/// <summary>
/// 玩家上下线
/// </summary>
[ProtoBuf.ProtoContract]
public class TenOnPlayerOffLine
{
    [ProtoBuf.ProtoMember(1)]
    public int seatId;
    /// <summary>
    /// 1是上线 其他是下线
    /// </summary>
    [ProtoBuf.ProtoMember(2)]
    public int state;
}



/// <summary>
/// 获取玩家距离返回 3508
/// </summary>
[ProtoBuf.ProtoContract]
public class TenSendDistanceAck
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public TenSendDistanceData data;
}

[ProtoBuf.ProtoContract]
public class TenSendDistanceData
{
    [ProtoBuf.ProtoMember(1)]
    public List<TenSendDistanceInfo> distances;
}


[ProtoBuf.ProtoContract]
public class TenSendDistanceInfo
{
    [ProtoBuf.ProtoMember(1)]
    public float distance;//距离
    [ProtoBuf.ProtoMember(2)]
    public string leftName;//左侧玩家名字
    [ProtoBuf.ProtoMember(3)]
    public string leftUid;//左侧玩家UID
    [ProtoBuf.ProtoMember(4)]
    public string leftHead;//左侧玩家头像
    [ProtoBuf.ProtoMember(5)]
    public string RightName;//右侧玩家名字
    [ProtoBuf.ProtoMember(6)]
    public string RightUid;//右侧玩家UID
    [ProtoBuf.ProtoMember(7)]
    public string RightHead;//右侧玩家头像
}




/// <summary>
/// 查看玩家信息
/// </summary>
[ProtoBuf.ProtoContract]
public class TenSendGetUserInfoReq
{
    [ProtoBuf.ProtoMember(1)]
    public string uid;
}

/// <summary>
/// 返回
/// </summary>
[ProtoBuf.ProtoContract]
public class TenSendGetUserInfoAck
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public TenSendGetUserInfoData data;
}


[ProtoBuf.ProtoContract]
public class TenSendGetUserInfoData
{
    [ProtoBuf.ProtoMember(1)]
    public TenSendGetUserInfo info;
}

[ProtoBuf.ProtoContract]
public class TenSendGetUserInfo
{
    [ProtoBuf.ProtoMember(1)]
    public float distance;
    [ProtoBuf.ProtoMember(2)]
    public string nickName;
    [ProtoBuf.ProtoMember(3)]
    public string address;
    [ProtoBuf.ProtoMember(4)]
    public string userId;
    [ProtoBuf.ProtoMember(5)]
    public string headUrl;
}


/// <summary>
/// 游戏开始
/// </summary>
[ProtoBuf.ProtoContract]
public class TenOnGameStart
{
    [ProtoBuf.ProtoMember(1)]
    public int zhuangSeatId;
}


/// <summary>
/// 自己得到操作指令
/// </summary>
[ProtoBuf.ProtoContract]
public class TenOnSelfOpt
{
    /// <summary>
    ///  1下注 2亮牌 3是抢庄
    /// </summary>
    [ProtoBuf.ProtoMember(1)]
    public int ins;
    /// <summary>
    /// 可以抢庄的数组
    /// </summary>
    [ProtoBuf.ProtoMember(2)]
    public List<int> canQzList;
    /// <summary>
    /// 抢庄的值
    /// </summary>
    [ProtoBuf.ProtoMember(3)]
    public List<int> qzListValue;
    [ProtoBuf.ProtoMember(4)]
    public int lastTime;
    /// <summary>
    /// 游戏走到哪个流程了
    /// </summary>
    [ProtoBuf.ProtoMember(5)]
    public int gameState;
    /// <summary>
    /// 手牌 有几张牌就发几张
    /// </summary>
    [ProtoBuf.ProtoMember(6)]
    public List<string> cards;
    /// <summary>
    /// 参与这局游戏的玩家座位
    /// </summary>
    [ProtoBuf.ProtoMember(7)]
    public List<int> gameSeatIdList;
    /// <summary>
    /// 可以下注的list
    /// </summary>
    [ProtoBuf.ProtoMember(8)]
    public List<int> canXzList;
    /// <summary>
    /// 下注的值
    /// </summary>
    [ProtoBuf.ProtoMember(9)]
    public List<float> xzListValue;
}


/// <summary>
/// 发送操作指令
/// </summary>
[ProtoBuf.ProtoContract]
public class TenSendGameOpt
{
    [ProtoBuf.ProtoMember(1)]
    public int ins;
    /// <summary>
    /// 抢庄的值
    /// </summary>
    [ProtoBuf.ProtoMember(2)]
    public int qzValue;
    /// <summary>
    /// 下注的值
    /// </summary>
    [ProtoBuf.ProtoMember(3)]
    public float xzValue;
}


/// <summary>
/// 同步玩家操作结果
/// </summary>
[ProtoBuf.ProtoContract]
public class TenOnPlayerOptResult
{
    [ProtoBuf.ProtoMember(1)]
    public int ins;
    [ProtoBuf.ProtoMember(2)]
    public int seatId;
    /// <summary>
    /// 抢庄的值
    /// </summary>
    [ProtoBuf.ProtoMember(3)]
    public int qzValue;
    /// <summary>
    /// 下注的值
    /// </summary>
    [ProtoBuf.ProtoMember(4)]
    public float xzValue;
    [ProtoBuf.ProtoMember(5)]
    public List<string> cards;
    [ProtoBuf.ProtoMember(6)]
    public TenPlayerHandCardsType cardsType;
    /// <summary>
    /// 座位号数组
    /// </summary>
    [ProtoBuf.ProtoMember(7)]
    public List<int> seatList;
}


/// <summary>
/// 同步玩家发牌
/// </summary>
[ProtoBuf.ProtoContract]
public class TenonCastCard
{
    [ProtoBuf.ProtoMember(1)]
    public List<string> selfCards;
    [ProtoBuf.ProtoMember(2)]
    public TenPlayerHandCardsType cardsType;
    [ProtoBuf.ProtoMember(3)]
    public int lastTime;
    /// <summary>
    /// 参与游戏的座位号
    /// </summary>
    [ProtoBuf.ProtoMember(4)]
    public List<int> gamedSeatIdList;
    /// <summary>
    /// 是否能看牌
    /// </summary>
    [ProtoBuf.ProtoMember(5)]
    public bool canShowCard;
}


/// <summary>
/// 同步小结算
/// </summary>
[ProtoBuf.ProtoContract]
public class TenonSmallSettle
{
    [ProtoBuf.ProtoMember(1)]
    public List<TenonSmallSettleWinLoseList> winList;
    [ProtoBuf.ProtoMember(2)]
    public List<TenonSmallSettleWinLoseList> lostList;
    [ProtoBuf.ProtoMember(3)]
    public List<TenonSmallSettleLastList> lastScore;
    [ProtoBuf.ProtoMember(4)]
    public int lastTime;
    [ProtoBuf.ProtoMember(5)]
    public List<TenonSmallSettleScore> scoreList;
}


[ProtoBuf.ProtoContract]
public class TenonSmallSettleScore
{
    [ProtoBuf.ProtoMember(1)]
    public int seatId;
    [ProtoBuf.ProtoMember(2)]
    public float score;
}

/// <summary>
/// 哪些玩家盈利
/// </summary>
[ProtoBuf.ProtoContract]
public class TenonSmallSettleWinLoseList
{
    [ProtoBuf.ProtoMember(1)]
    public float loseScore;
    [ProtoBuf.ProtoMember(2)]
    public int winSeatId;
    [ProtoBuf.ProtoMember(3)]
    public int loseSeatId;
    [ProtoBuf.ProtoMember(4)]
    public float winScore;
}

/// <summary>
/// 结算之后还剩下多少分
/// </summary>
[ProtoBuf.ProtoContract]
public class TenonSmallSettleLastList
{
    [ProtoBuf.ProtoMember(1)]
    public int seatId;
    [ProtoBuf.ProtoMember(2)]
    public float score;
    [ProtoBuf.ProtoMember(3)]
    public float totalWin;
}

/// <summary>
/// 换庄
/// </summary>
[ProtoBuf.ProtoContract]
public class TenOnChangZhuang
{
    [ProtoBuf.ProtoMember(1)]
    public int zhuangSeatId;
    /// <summary>
    /// 是否是随机庄
    /// </summary>
    [ProtoBuf.ProtoMember(2)]
    public bool random;
    /// <summary>
    /// 随机庄家的座位号
    /// </summary>
    [ProtoBuf.ProtoMember(3)]
    public List<int> randomSeatList;
}