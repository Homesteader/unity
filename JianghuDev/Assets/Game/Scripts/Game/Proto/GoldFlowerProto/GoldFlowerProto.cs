using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 金花创建房间返回
/// </summary>
[ProtoBuf.ProtoContract]
public class GoldFlowerCreateRoomAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public GoldFlowerCreateRoomData data;
}


[ProtoBuf.ProtoContract]
public class GoldFlowerCreateRoomData {
    [ProtoBuf.ProtoMember(1)]
    public GoldFlowerCreateRoomInfo roomInfo;
}


[ProtoBuf.ProtoContract]
public class GoldFlowerCreateRoomInfo {
    [ProtoBuf.ProtoMember(1)]
    public SendCreateRoomReq rule;
    [ProtoBuf.ProtoMember(2)]
    public int gameId;
    [ProtoBuf.ProtoMember(3)]
    public int conIndex;
    [ProtoBuf.ProtoMember(4)]
    public float difen;
    [ProtoBuf.ProtoMember(5)]
    public int mySeatId;
    [ProtoBuf.ProtoMember(6)]
    public float dichi;
    /// <summary>
    /// 这一句的状态 1是准备 2是开始
    /// </summary>
    [ProtoBuf.ProtoMember(7)]
    public int gameState;
    [ProtoBuf.ProtoMember(8)]
    public int timeDownOpt;
    [ProtoBuf.ProtoMember(9)]
    public GoldFlowerOpt opt;
    [ProtoBuf.ProtoMember(10)]
    public List<GoldFlowerPlayer> playerList;
    [ProtoBuf.ProtoMember(11)]
    public GoldFlowerHandCards handCardsList;
    /// <summary>
    /// 开始没有 0是还没开始过任何一小局 1是开始过了
    /// </summary>
    [ProtoBuf.ProtoMember(12)]
    public int roomState;
    [ProtoBuf.ProtoMember(13)]
    public string roomId;
    /// <summary>
    /// 当前轮到谁操作
    /// </summary>
    [ProtoBuf.ProtoMember(14)]
    public int turnSeatId;
    /// <summary>
    /// 庄家座位号
    /// </summary>
    [ProtoBuf.ProtoMember(15)]
    public int zhuangSeatId;
    /// <summary>
    /// 是否可以看牌
    /// </summary>
    [ProtoBuf.ProtoMember(16)]
    public bool canLookCard;
    /// <summary>
    /// 闷
    /// </summary>
    [ProtoBuf.ProtoMember(17)]
    public List<float> menRate;
    /// <summary>
    /// 看牌
    /// </summary>
    [ProtoBuf.ProtoMember(18)]
    public List<float> lookRate;
    /// <summary>
    /// 当前第几轮
    /// </summary>
    [ProtoBuf.ProtoMember(19)]
    public int round;
    /// <summary>
    /// true 的时候是平台房间
    /// </summary>
    [ProtoBuf.ProtoMember(20)]
    public bool goldPattern;
    [ProtoBuf.ProtoMember(23)]
    public int autoDestoryTime;//解散时间

}

[ProtoBuf.ProtoContract]
public class GoldFlowOptData {
    [ProtoBuf.ProtoMember(1)]
    public int seatId;
    [ProtoBuf.ProtoMember(2)]
    public int time;
    [ProtoBuf.ProtoMember(3)]
    public GoldFlowerOpt data;

}

[ProtoBuf.ProtoContract]
public class GoldFlowerOpt {
    /// <summary>
    /// 1是底注 2是全压 3是跟注 4是加注 5弃牌 6看牌 7是孤注一掷 8比牌 
    /// </summary>
    [ProtoBuf.ProtoMember(1)]
    public List<int> optList;
    /// <summary>
    /// 加注列表
    /// </summary>
    [ProtoBuf.ProtoMember(2)]
    public List<int> jiazhuList;
    /// <summary>
    /// 比牌需要的分数
    /// </summary>
    [ProtoBuf.ProtoMember(3)]
    public float bpScore;
}




[ProtoBuf.ProtoContract]
public class GoldFlowerPlayer {
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
    /// <summary>
    /// 下注总分数
    /// </summary>
    [ProtoBuf.ProtoMember(10)]
    public float toalBet;
    [ProtoBuf.ProtoMember(11)]
    public float longitude;
    [ProtoBuf.ProtoMember(12)]
    public float latitude;
    /// <summary>
    /// 是否弃牌
    /// </summary>
    [ProtoBuf.ProtoMember(13)]
    public bool discard;
    /// <summary>
    /// 当前跟注
    /// </summary>
    [ProtoBuf.ProtoMember(14)]
    public float curGenZhu;
    /// <summary>
    /// 是否自动跟注
    /// </summary>
    [ProtoBuf.ProtoMember(15)]
    public bool autoGen;
    [ProtoBuf.ProtoMember(16)]
    public float totalWin;
}

[ProtoBuf.ProtoContract]
public class GoldFlowerHandCards {
    [ProtoBuf.ProtoMember(1)]
    public GoldFlowerHandCardsInfo myCardsInfo;
    [ProtoBuf.ProtoMember(2)]
    public List<GoldFlowerHandCardsInfo> otherCardsInfo;
}


[ProtoBuf.ProtoContract]
public class GoldFlowerHandCardsInfo {
    [ProtoBuf.ProtoMember(1)]
    public List<string> cards;
    [ProtoBuf.ProtoMember(2)]
    public int cardType;
    [ProtoBuf.ProtoMember(3)]
    public bool kpState;
    [ProtoBuf.ProtoMember(4)]
    public int seatId;
}


/// <summary>
/// 加入房间
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGoldFlowerJoinRoomReq
{
    [ProtoBuf.ProtoMember(1)]
    public string roomId;
}

/// <summary>
/// 有玩家坐下
/// </summary>
[ProtoBuf.ProtoContract]
public class OnGoldFlowerOnSeatDown {
    [ProtoBuf.ProtoMember(1)]
    public GoldFlowerPlayer info;
}

/// <summary>
/// 同步玩家准备
/// </summary>
[ProtoBuf.ProtoContract]
public class OnGoldFlowerReady {
    [ProtoBuf.ProtoMember(1)]
    public int seatId;
}

/// <summary>
/// 玩家离开房间
/// </summary>
[ProtoBuf.ProtoContract]
public class OnGoldFlowerPlayerLeave {
    [ProtoBuf.ProtoMember(1)]
    public int seatId;
}

/// <summary>
/// 游戏开始
/// </summary>
[ProtoBuf.ProtoContract]
public class OnGoldFlowerGameStart {
    /// <summary>
    /// 第几圈
    /// </summary>
    [ProtoBuf.ProtoMember(1)]
    public int round;
    /// <summary>
    /// 底注
    /// </summary>
    [ProtoBuf.ProtoMember(2)]
    public float dizhu;
    /// <summary>
    /// 闷的倍率
    /// </summary>
    [ProtoBuf.ProtoMember(3)]
    public List<float> menRate;
    /// <summary>
    /// 看牌的倍率
    /// </summary>
    [ProtoBuf.ProtoMember(4)]
    public List<float> lookRate;
    /// <summary>
    /// 庄家座位号
    /// </summary>
    [ProtoBuf.ProtoMember(5)]
    public int zhuangSeatId;
}

/// <summary>
/// 游戏开始倒计时
/// </summary>
[ProtoBuf.ProtoContract]
public class OngoldFlowerGameStartLastTime {
    [ProtoBuf.ProtoMember(1)]
    public int lastTime;
}

/// <summary>
/// 同步玩家操作
/// </summary>
[ProtoBuf.ProtoContract]
public class OnGoldFlowerPlayerOptResult
{
    [ProtoBuf.ProtoMember(1)]
    public List<OnGoldFlowerPlayerOptLastScore> lastScores;
    [ProtoBuf.ProtoMember(2)]
    public float dichi;
    /// <summary>
    /// 总的下注
    /// </summary>
    [ProtoBuf.ProtoMember(3)]
    public float TotalXiaZhu; 
    /// <summary>
    /// 1是时候不管座位号的
    /// </summary>
    [ProtoBuf.ProtoMember(4)]
    public int ins;
    [ProtoBuf.ProtoMember(5)]
    public int seatId;
    [ProtoBuf.ProtoMember(6)]
    public float curXiaZhu;
    [ProtoBuf.ProtoMember(7)]
    public int otherSeatId;
    [ProtoBuf.ProtoMember(8)]
    public List<string> cards;
    /// <summary>
    /// 1是高牌 2是对子 3是顺子 4是同花  5是顺金 6是飞机 
    /// </summary>
    [ProtoBuf.ProtoMember(9)]
    public int cardType;
    /// <summary>
    /// 赢的玩家
    /// </summary>
    [ProtoBuf.ProtoMember(10)]
    public int winSeatId;
    /// <summary>
    /// 还剩下多少钱
    /// </summary>
    [ProtoBuf.ProtoMember(11)]
    public float lastScore;
    /// <summary>
    /// 孤注一掷的时候的比牌座位号
    /// </summary>
    [ProtoBuf.ProtoMember(12)]
    public List<int> pkSeatId;
    /// <summary>
    /// 孤注一掷的时候是否赢了
    /// </summary>
    [ProtoBuf.ProtoMember(13)]
    public bool win;
    [ProtoBuf.ProtoMember(14)]
    public int disType;//弃牌类型 0：主动弃牌  1：比牌输
}

/// <summary>
/// 下注之后还剩下多少
/// </summary>
[ProtoBuf.ProtoContract]
public class OnGoldFlowerPlayerOptLastScore {
    [ProtoBuf.ProtoMember(1)]
    public int seatId;
    [ProtoBuf.ProtoMember(2)]
    public float lastScore;
}

/// <summary>
/// 发送操作
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGoldFlowerOpt {
    [ProtoBuf.ProtoMember(1)]
    public int ins;
    [ProtoBuf.ProtoMember(2)]
    public int bet;
    [ProtoBuf.ProtoMember(3)]
    public int otherSeatId;
}


/// <summary>
/// 玩家上下线
/// </summary>
[ProtoBuf.ProtoContract]
public class OnGoldFlowerOnOffLine {
    [ProtoBuf.ProtoMember(1)]
    public int seatId;
    /// <summary>
    /// 1是上线 其他是下线
    /// </summary>
    [ProtoBuf.ProtoMember(2)]
    public int state;
}

/// <summary>
/// 玩家小结算
/// </summary>
[ProtoBuf.ProtoContract]
public class OnGoldFlowerSmallSettle
{
    [ProtoBuf.ProtoMember(1)]
    public int time;
    [ProtoBuf.ProtoMember(2)]
    public List<OnGoldFlowerSmallSettleWinList> winList;
    [ProtoBuf.ProtoMember(3)]
    public List<string> cards;
    [ProtoBuf.ProtoMember(4)]
    public int cardType;
    [ProtoBuf.ProtoMember(5)]
    public List<GoldFlowerCardsInfo> playerCardsList;
    /// <summary>
    /// 输喜钱的玩家
    /// </summary>
    [ProtoBuf.ProtoMember(6)]
    public List<GoldFlowerLoseXiQian> loseXiList;
    [ProtoBuf.ProtoMember(7)]
    public float totalWin;

}

[ProtoBuf.ProtoContract]
public class OnGoldFlowerSmallSettleWinList {
    [ProtoBuf.ProtoMember(1)]
    public float winScore;
    [ProtoBuf.ProtoMember(2)]
    public int seatId;
    [ProtoBuf.ProtoMember(3)]
    public float score;
    [ProtoBuf.ProtoMember(4)]
    public float xiQian;
}


[ProtoBuf.ProtoContract]
public class GoldFlowerCardsInfo {
    [ProtoBuf.ProtoMember(1)]
    public int seatId;
    [ProtoBuf.ProtoMember(2)]
    public List<string> card;
    [ProtoBuf.ProtoMember(3)]
    public int cardType;
}

/// <summary>
/// 输掉喜钱的玩家列表
/// </summary>
[ProtoBuf.ProtoContract]
public class GoldFlowerLoseXiQian {
    [ProtoBuf.ProtoMember(1)]
    public float xiQian;
    [ProtoBuf.ProtoMember(2)]
    public int seatId;
    [ProtoBuf.ProtoMember(3)]
    public GoldFlowerCardsInfo cardsInfo;


}

/// <summary>
/// 有玩家亮牌
/// </summary>
[ProtoBuf.ProtoContract]
public class OnGoldFlowerShowCard {
    [ProtoBuf.ProtoMember(1)]
    public List<string> cards;
    [ProtoBuf.ProtoMember(2)]
    public int cardType;
    [ProtoBuf.ProtoMember(3)]
    public int seatId;
}

/// <summary>
/// 更新轮数
/// </summary>
[ProtoBuf.ProtoContract]
public class OnGoldFlowerUpdateRound {
    [ProtoBuf.ProtoMember(1)]
    public int round;
}



/// <summary>
/// 获取玩家距离返回 3508
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGoldFlowerDistanceAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGoldFlowerDistanceData data;
}

[ProtoBuf.ProtoContract]
public class SendGoldFlowerDistanceData {
    [ProtoBuf.ProtoMember(1)]
    public List<SendGoldFlowerDistanceInfo> distances;
}


[ProtoBuf.ProtoContract]
public class SendGoldFlowerDistanceInfo {
    [ProtoBuf.ProtoMember(1)]
    public float distance;
    [ProtoBuf.ProtoMember(2)]
    public string leftName;
    [ProtoBuf.ProtoMember(3)]
    public string leftUid;
    [ProtoBuf.ProtoMember(4)]
    public string leftHead;
    [ProtoBuf.ProtoMember(5)]
    public string RightName;
    [ProtoBuf.ProtoMember(6)]
    public string RightUid;
    [ProtoBuf.ProtoMember(7)]
    public string RightHead;
}

/// <summary>
/// 查看玩家信息
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetGoldFlowerUserInfoReq {
    [ProtoBuf.ProtoMember(1)]
    public string uid;
}

/// <summary>
/// 返回
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetGoldFlowerUserInfoAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetGoldFlowerUserInfoData data;
}


[ProtoBuf.ProtoContract]
public class SendGetGoldFlowerUserInfoData {
    [ProtoBuf.ProtoMember(1)]
    public SendGetGoldFlowerUserInfo info;
}

[ProtoBuf.ProtoContract]
public class SendGetGoldFlowerUserInfo {
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
/// 最后一站
/// </summary>
[ProtoBuf.ProtoContract]
public class OnGetGoldFlowerLastFight
{
    [ProtoBuf.ProtoMember(1)]
    public List<int> pkSeatList;
    [ProtoBuf.ProtoMember(2)]
    public List<int> winSeatList;
}

/// <summary>
/// 加入金币场
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGoldFlowerJoinGoldRoom {
    [ProtoBuf.ProtoMember(1)]
    public int id;
}

/// <summary>
/// 自动跟注
/// </summary>
[ProtoBuf.ProtoContract]
public class SendAutoGenReq {
    /// <summary>
    /// 1是自动跟注 2不是自动跟注
    /// </summary>
    [ProtoBuf.ProtoMember(1)]
    public int auto;
}

[ProtoBuf.ProtoContract]
public class SendAutoGenAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendAutoGenData data;
}

[ProtoBuf.ProtoContract]
public class SendAutoGenData {
    [ProtoBuf.ProtoMember(1)]
    public int auto;
}

//结算
[ProtoBuf.ProtoContract]
public class GoldSettlementData
{
    [ProtoBuf.ProtoMember(1)]
    public GoldSettlementItemData[] data;
}

[ProtoBuf.ProtoContract]
public class GoldSettlementItemData
{
    [ProtoBuf.ProtoMember(1)]
    public string userId;//玩家id
    [ProtoBuf.ProtoMember(2)]
    public string icon;//头像
    [ProtoBuf.ProtoMember(3)]
    public float xi;//喜钱
    [ProtoBuf.ProtoMember(4)]
    public float score;//输赢分数
    [ProtoBuf.ProtoMember(5)]
    public float gold;//金币总数
    [ProtoBuf.ProtoMember(6)]
    public string[] cards;//牌
    [ProtoBuf.ProtoMember(7)]
    public int seatId;//座位号
}

//解散时间
[ProtoBuf.ProtoContract]
public class RecieveDisolveTime
{
    [ProtoBuf.ProtoMember(1)]
    public int time;
}