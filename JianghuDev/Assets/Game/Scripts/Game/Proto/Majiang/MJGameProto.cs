using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class MJRoomInfoProto : MsgResponseBase
{
    public MJRoomInfo data;
}
//同步玩家加入房间
public class MJPlayerJoinRoomProto : MsgResponseBase
{
    public MJplayerInfo data;
}
//同步玩家准备或者离开
public class MJPlayerPreProto : MsgResponseBase
{
    public MJPlayerPreProtoData data;
}
public class MJPlayerPreProtoData
{
    public int seatId;//座位号
    public int state;//1:上线，0：下线
}

#region 解散房间
//解散房间
public class MJExitList
{
    public int seatId = 0;//上下线的人
    /// <summary>
    /// 标志，1同意，0等待，-1拒绝，2申请人
    /// </summary>
    public int state = 0;
}
public class MJDissolveProtoData
{
    public int timeDown = 0;//倒计时
    public List<MJExitList> applyExitList = new List<MJExitList>();//各家回应情况
}
#endregion

//房主解散房间

public class MJOwnerDissolveProtoData
{
    public string nickName;//房主昵称
}

//开始时玩家手上的牌
public class MJPlayerHandCardProto : MsgResponseBase
{
    public MJcardInfo playerListInfo;//房间中玩家显示在桌面上的牌
    public int cardCount;//剩余牌的数量
    public int currTurnSeatId;//当前处理牌的人
    public int currMgrSeatId;//当前局的庄家
    public int curGameNum;//当前是第几局
}


//可操作指令集合
public class MJInstructionsProto_old
{
    public List<MJoptInfoData> optList;//指令集合
}

public class MJOptInfoAllCards
{
    public List<int> cards;//--手牌
    public int currCard;//--摸的拍
    public bool isHasCurrCard;//--是否有摸的牌
    public int cardsNum;//--手牌有多少张
    public int seatId;//--当前玩家的座位号
    //public List<int> hitList;// 打牌列表--能有空的
    public List<MJliangcard> gangList;// 杠牌列表--能有空的
    public List<MJliangcard> pengList;// 碰牌列表--能有空的
    public List<MJliangcard> chiList;// 吃牌列表--能有空的
    public List<MJliangcard> liangList;// 亮牌列表--能有空的
}

public class MJOptInfoCards
{
    public MJOptInfoAllCards myCardsInfo;//手牌
}
public class MJoptInfoData_old
{
    public eMJInstructionsType ins;//   操作指令类型
    public List<int> card;//操作中处理牌
    public List<int> seatId;//操作的人
    public int type;//操作类型，参看具体的类型
    public int subType;//操作二级类型，参看具体的类型
    public int priId;//处理优先级
    public int cardCount;//剩余牌数量
    public int otherSeatId; //被操作影响的人
    public int[] huType;//胡牌类型
    public MJOptInfoCards cardInfo;//牌
}


public class MJoptInfo : MsgResponseBase//具体的处理指令
{
    public MJoptInfoData data;
}

public class MJCurInstructionsProto : MsgResponseBase
{
    public MJoptInfoData data;

}

#region 房间信息
//登录认证回复房间信息
public class MJRoomInfo
{
    //房间信息：
    public int roomId;//--房间号
    public int maxGameNum;//--玩的总共多少局
    public int cardCount;//--玩到剩余多少牌
    public int timeDown;//--房间倒计时
    public int currMgrSeatId;//--当前庄家是谁
    public int mySeatId;//--我的座位号
    public int curGameNum;//--当前玩到多少局
    public eMJRoomStatus state;//--当前游戏状态
    public int currTurnSeatId;//--当前轮到谁操作了
    public int currOptSeatId;//上一次操作的玩家
    public int currOptCard;//上一次打出的牌
    public int MaxMul;//封顶番数
    public float baseScore;//底分
    public int paytype;//房间类型   支付方式，1：房主支付，2：4人支付，3：群主支付

    public string createData;//--创建房间参数,用encode封装成字符串的，需要decode解开
    public List<int> rule;//规则

    //退出倒计时信息：--类中具体字段参考3107

    public MJDissolveProtoData applyExitList;//

    public MJGameGpsWarningPlayerInfo[] gpsWarningInfo;//GPS预警

    public MJGameIpInfo[] ipWarningInfo;//ip预警

    //玩家信息：

    public List<MJplayerInfo> playerList;//

    public int[] touzi = new int[] { 1, 2 };//骰子点数

    //手牌信息：

    public MJcardInfo handCardsList;//

    //操作信息：类详情见指令2307中说明

    public MJoptInfo optInfo;//
    //操作指令
    public List<OptItemStruct> optList;
}

public class MJPlayerInfo //玩家信息：
{
    public List<MJplayerInfo> playerListInfo;//--玩家信息，类ansInfo 内容等于3102中返回的内容
}

public class MJcardInfo //手牌信息：
{
    public List<MJotherInfo> otherCardsInfo;//otherCardsInfo
    public MJmyInfo myCardsInfo;// 我自己的手牌

}
public class MJotherInfoList //其他玩家牌的信息列表
{
    public List<MJotherInfo> playerListInfo;//--其他玩家牌
}
public class MJotherInfo//其他玩家的牌信息
{
    public bool isHasCurrCard;//--是否有摸的牌
    public int cardsNum;//--手牌有多少张
    public int seatId;//--当前玩家的座位号
}
public class MJmyInfo //我的牌信息
{
    public List<int> cards;//--手牌
    public int currCard;//--摸的拍
}

public class MJChatProtoData
{
    public int token = 0;//发送协议的人的令牌
    public string msgText;//消息中的文本，比如聊天的话
    public eMJChatCotentType type;// 群聊还是私聊？？？
    public int seatId;// --收到的人，比如私聊，谁发送的
    public int targetSeatId;//发送给谁
    public int msg;//--协议类型
    public float voiceTime;//语音时间
}
//聊天消息
public class MJChatProto : MsgResponseBase
{
    public MJChatProtoData data;
}

#endregion

//玩家数据
public class MJplayerInfo
{
    public string nickName;//--昵称
    public string headUrl;//--头像
    public int diamond;//--钻石
    public string lastIp;//--IP
    public int seatId;//--座位
    public string userId;//--ID
    public int vip;//--VIP等级
    public bool isReady;// 是否是准备好
    public int onLineState;// 是否在线 //1:上线，0：下线
    public float longitude;// 经度
    public float latitude;// 纬度
    public float gold;// 金币
    public float score;// 积分
    public int sex;// 性别
    public bool isBaojiao;// 是否吃报听状态
    public string addr;//地址
    //public int mHuCard;//胡牌
    public List<HuStruct> mHuCard;//胡牌列表--能有空的
    public List<int> hitList;// 打牌列表--能有空的
    public List<GangStruct> gangList;// 杠牌列表--能有空的
    public List<PengStruct> pengList;// 碰牌列表--能有空的
    public List<MJliangcard> chiList;// 吃牌列表--能有空的
    public List<MJliangcard> liangList;// 亮牌列表--能有空的
}
public class MJliangcard //碰杠吃亮等亮在外面的牌的结构
{
    public List<int> card;//--牌的列表，比如：123,666，
    public List<int> seatId;//--碰谁的牌，3
    public int type;//--类型，参看具体的类型
}

#region 小结算
public class MJGameSettlementInfo
{
    public bool isEnd;   //是否打完之后的小结算
    public bool isHu;   //该局是否胡牌
    public List<MJGameSettlementPlayerInfo> settleContainer;//小结算数据
}
public class MJsettleContainer1
{
    public List<MJGameSettlementPlayerInfo> playrInfo;//玩家的结算数据列表
}
public class MJGameSettlementPlayerInfo  //玩家的结算信息
{
    public int seatId;   //座位号
    public string userId;   //ID
    public string nickName;    //昵称
    public float score;    //输赢积分
    public bool isZhuang;  //是不是庄家
    public int fanShu;   //胡牌番数
    public string detail;   //得分细节说明
    //public int huDePai;   //胡的那张牌
    public string headUrl;//--头像
    public List<int> shoupai;//手牌
    public List<int> des;//胡牌类型 不用这个
    public string huDes;//胡牌类型
    //public List<int> peng;//碰牌
    public List<PengStruct> peng;//碰牌
    public List<HuStruct> huPai;//胡的牌
    public List<GangStruct> gang; //杠牌信息
    public List<CardsStruct> chi;  //吃牌信息
    public List<MJliangcard> liang; //亮牌信息 
    public int huOrder;//胡顺序
}
[ProtoBuf.ProtoContract]
public class litSemResponse
{
    [ProtoBuf.ProtoMember(1)]
    public eRoomState roomState; //当前房间的状态
    [ProtoBuf.ProtoMember(2)]
    public List<litSemStruct> litSemList; //结算列表

    [ProtoBuf.ProtoMember(3)]
    public List<FlowSemStruct> flowSemList;//查叫查花猪赔雨
}

//
[ProtoBuf.ProtoContract]
public class FlowSemStruct
{
    [ProtoBuf.ProtoMember(1)]
    public int seatId; //座位号(需要查花猪,查叫,赔雨的玩家座位号)
    [ProtoBuf.ProtoMember(2)]
    public List<effStruct> effList; //包含多种被查(查花猪,查叫,赔雨)
}
[ProtoBuf.ProtoContract]
public class effStruct
{
    [ProtoBuf.ProtoMember(1)]
    public int type; //分数类型( 转雨 查花猪,查大叫,赔雨)
    [ProtoBuf.ProtoMember(2)]
    public List<ScoreStruct> scoreList; //该类型下各玩家获得的分数情况
}

//小结算的信息
[ProtoBuf.ProtoContract]
public class litSemStruct
{
    [ProtoBuf.ProtoMember(1)]
    public int seatId; //座位号
    [ProtoBuf.ProtoMember(2)]
    public float currScore; //当前输赢的分数
    [ProtoBuf.ProtoMember(3)]
    public List<int> handList; //当前剩余的手牌
    [ProtoBuf.ProtoMember(4)]
    public float gold;//金币数
    [ProtoBuf.ProtoMember(5)]
    public string huIntro;//胡牌类型：对对胡，清一色等
    [ProtoBuf.ProtoMember(6)]
    public int huOrder;//胡牌顺序
}
#endregion

#region 大结算
public class MJGameSettlementFinalInfo
{
    public bool isEnd;   //是否打完之后的大结算
    public List<MJGameSettlementFinalPlayerInfo> totalContainr;//大结算数据
    public MJGameSettlementPlayerInfo bestHu;//最佳牌型
}
public class MJGameSettlementFinalPlayerInfo  //玩家的结算信息
{
    public bool isBigwiner;  //是不是大赢家
    public string headUrl;   //头像
    public int seatId;   //座位号
    public string nickName;    //昵称
    public string userId;   //ID
    public float score;    //输赢积分
    public int dianPao;   //放炮次数
    //public int huPai;    //胡牌次数
    //public int zhuang;    //庄家次数
    public int winCount;//胜局次数

}

//大结算返回
[ProtoBuf.ProtoContract]
public class BigSemResponse
{
    [ProtoBuf.ProtoMember(1)]
    public List<BigSemStruct> bigSemList; //结算列表
}

//大结算的个人信息
[ProtoBuf.ProtoContract]
public class BigSemStruct
{
    [ProtoBuf.ProtoMember(1)]
    public int seatId; //座位号
    [ProtoBuf.ProtoMember(2)]
    public float score; //总分
    [ProtoBuf.ProtoMember(3)]
    public int winCount; //胜局数
}
#endregion

#region ip预警
public class MJGameIpWarningInfo
{
    public MJGameIpInfo[] list;
}

public class MJGameIpInfo
{
    public string ip;
    public int[] seatId;
}
#endregion


#region GPS预警
public class MJGameGpsWarningInfo
{
    public MJGameGpsWarningPlayerInfo[] gpsWarningInfo;
}

public class MJGameGpsWarningPlayerInfo
{
    public string playerA;
    public string playerB;
}
#endregion

#region 玩家信息
public class MJPlayerDetailInfo
{
    public string headUrl;
    public int userSex;//性别
    public string userId;//
    public int roomCard;//房卡数量
    public string IP;
    public string addr;//地址
    public int distance;//距离
    public string nickName;//名字
}

public class MJGetPlayerInfoData
{
    public MJPlayerDetailInfo PerInfo;
}

public class MJGetPlayerInfoProto : MsgResponseBase
{
    public MJGetPlayerInfoData data;
}

#endregion


//************************************************************************//

#region 麻将 开始

[ProtoBuf.ProtoContract]
public class MJJoinRoomRequest//加入房间
{
    [ProtoBuf.ProtoMember(1)]
    public string roomId;
}

[ProtoBuf.ProtoContract]
public class StartGameBackData
{
    [ProtoBuf.ProtoMember(1)]
    public int code;//错误码
    [ProtoBuf.ProtoMember(2)]
    public StartGameRespone data;//数据
}


//开始游戏和断线重连需要的数据
[ProtoBuf.ProtoContract]
public class StartGameRespone
{
    [ProtoBuf.ProtoMember(1)]
    public RoomStruct roomInfo;//房间的数据
    [ProtoBuf.ProtoMember(2)]
    public StartGameStruct startInfo;//开始游戏的数据
}

//房间的数据结构
[ProtoBuf.ProtoContract]
public class RoomStruct
{
    [ProtoBuf.ProtoMember(1)]
    public eRoomState roomState;//房间的状态       0准备   1 换3张  2  3   4
    [ProtoBuf.ProtoMember(2)]
    public int currGameCount;//当前的局数
    [ProtoBuf.ProtoMember(3)]
    public int maxGameCount;//最大的局数
    [ProtoBuf.ProtoMember(4)]
    public int mySeatId;  //我自己的座位号
    [ProtoBuf.ProtoMember(5)]
    public List<PlayerInfoStruct> playerList;//房间里所有玩家的信息 
    [ProtoBuf.ProtoMember(6)]
    public string roomId;//房间号
    [ProtoBuf.ProtoMember(7)]
    public int maxPlayer;//最大玩家数量
    [ProtoBuf.ProtoMember(8)]
    public List<OptItemStruct> optList;//可操作列表
    [ProtoBuf.ProtoMember(9)]
    public SendCreateRoomReq createData;//创建房间数据
    [ProtoBuf.ProtoMember(10)]
    public int lastHitSeatId;//上一次打牌的玩家
    [ProtoBuf.ProtoMember(11)]
    public string mode;//模式 JULEBU = "m1",LIANMENG = "m2",HAOYOU = "m3",JINBI = "m4",
    [ProtoBuf.ProtoMember(12)]
    public int turnLeaveTime;//剩余时间
    [ProtoBuf.ProtoMember(13)]
    public int turnFixedTime;//操作固定时间
}

// 房间的个人信息
[ProtoBuf.ProtoContract]
public class PlayerInfoStruct
{
    [ProtoBuf.ProtoMember(1)]
    public int seatId; //座位号  
    [ProtoBuf.ProtoMember(2)]
    public string headUrl; //头像地址
    [ProtoBuf.ProtoMember(3)]
    public string nickName; //昵称
    [ProtoBuf.ProtoMember(4)]
    public bool isReady; //是否准备了
    [ProtoBuf.ProtoMember(5)]
    public float score; //当前的积分
    [ProtoBuf.ProtoMember(6)]
    public float gold; //当前的金币数量
    [ProtoBuf.ProtoMember(7)]
    public string uId; //玩家ID
    [ProtoBuf.ProtoMember(8)]
    public int sex;//性别
    [ProtoBuf.ProtoMember(9)]
    public int onLineType;//在线状态
    [ProtoBuf.ProtoMember(10)]
    public int tgType;//托管状态
    [ProtoBuf.ProtoMember(11)]
    public bool isGiveUp;//玩家是否认输了
}
// 开始游戏的消息 request请求 response返回
[ProtoBuf.ProtoContract]
public class StartGameStruct
{
    [ProtoBuf.ProtoMember(1)]
    public int totalCardNum; //总共有多少张牌
    [ProtoBuf.ProtoMember(2)]
    public int leaveCardNum; //剩余多少张牌
    [ProtoBuf.ProtoMember(3)]
    public int currTurnSeatId; //当前指针指向谁
    [ProtoBuf.ProtoMember(4)]
    public List<int> dices; //骰子的数组
    [ProtoBuf.ProtoMember(5)]
    public List<CardsInfoStruct> cardsInfoList;//所有玩家牌的数据结构列表
    [ProtoBuf.ProtoMember(6)]
    public int zhuangSeatId;
}


// 胡牌的结构体
[ProtoBuf.ProtoContract]
public class HuStruct : CardsStruct
{
    [ProtoBuf.ProtoMember(1)]
    public int card;//胡的牌
    [ProtoBuf.ProtoMember(2)]
    public bool isGl;//是否高亮
    [ProtoBuf.ProtoMember(3)]
    public int huType;//胡牌类型
}
// 碰的结构体
[ProtoBuf.ProtoContract]
public class PengStruct : CardsStruct
{
    [ProtoBuf.ProtoMember(1)]
    public List<int> cards; //碰的牌
    [ProtoBuf.ProtoMember(2)]
    public int otherSeatId;//谁打过来的
}

[ProtoBuf.ProtoContract]
public class CardsStruct
{

}

// 杠的结构体
[ProtoBuf.ProtoContract]
public class GangStruct : CardsStruct
{
    [ProtoBuf.ProtoMember(1)]
    public List<int> cards; //杠的牌
    [ProtoBuf.ProtoMember(2)]
    public eGangType gangType;//杠的类型(暗杠点杠弯杠)
    [ProtoBuf.ProtoMember(3)]
    public int otherSeatId;//谁打过来的杠
    [ProtoBuf.ProtoMember(4)]
    public List<int> paySeatIds;//需要支付杠钱的玩家座位号
    [ProtoBuf.ProtoMember(5)]
    public bool isEff;
    [ProtoBuf.ProtoMember(6)]
    public float score;

}

#endregion


// 客户端主动发送过来操作
[ProtoBuf.ProtoContract]
public class OptRequest
{
    [ProtoBuf.ProtoMember(1)]
    public eMJInstructionsType ins; //操作序号
    [ProtoBuf.ProtoMember(2)]
    public List<int> cards; //操作的牌
    [ProtoBuf.ProtoMember(3)]
    public int type; //操作的类型
}

// 同步给玩家谁做了什么操作
[ProtoBuf.ProtoContract]
public class MJoptInfoData
{
    [ProtoBuf.ProtoMember(1)]
    public eMJInstructionsType ins; //操作序号
    [ProtoBuf.ProtoMember(2)]
    public List<int> cards; //操作的牌
    [ProtoBuf.ProtoMember(3)]
    public int type; //操作的类型
    [ProtoBuf.ProtoMember(4)]
    public int seatId; //操作的座位号
    [ProtoBuf.ProtoMember(5)]
    public List<FixedStruct> fixedList;//所有人定缺的什么
    [ProtoBuf.ProtoMember(6)]
    public int otherSeatId;//其他人的座位号
    [ProtoBuf.ProtoMember(7)]
    public int subType;//操作的二级类型
    [ProtoBuf.ProtoMember(8)]
    public int thrType;//操作的三级类型
    [ProtoBuf.ProtoMember(9)]
    public List<ScoreStruct> scoreList;//获得的分数列表
    [ProtoBuf.ProtoMember(10)]
    public List<int> seatIdList;//一炮多向胡牌的玩家列表
    [ProtoBuf.ProtoMember(11)]
    public List<CanHuStruct> canHuList;//查听可以胡的列表
    [ProtoBuf.ProtoMember(12)]
    public int multiple;//可操作胡的番数
    [ProtoBuf.ProtoMember(19)]
    public int[] huType;//胡牌类型 
    [ProtoBuf.ProtoMember(20)]
    public bool huGl;//是否胡牌高亮
    [ProtoBuf.ProtoMember(13)]
    public RecordBackChangeList[] changeList;//回放换三张列表
    [ProtoBuf.ProtoMember(14)]
    public bool isHasJiao;//是否有叫

}

[ProtoBuf.ProtoContract]
public class RecordBackChangeList
{
    [ProtoBuf.ProtoMember(1)]
    public int seatId;//座位号
    [ProtoBuf.ProtoMember(2)]
    public List<int> inCards;//换回来的牌
    [ProtoBuf.ProtoMember(3)]
    public List<int> outCards;//换出去的牌
}



//打什么可以胡什么的结构体
[ProtoBuf.ProtoContract]
public class HitCardCanHuStruct
{
    [ProtoBuf.ProtoMember(1)]
    public int hitCard; //打什么牌
    [ProtoBuf.ProtoMember(2)]
    public List<CanHuStruct> canHuList; //可以胡的列表
}

//可以胡的结构体
[ProtoBuf.ProtoContract]
public class CanHuStruct
{
    [ProtoBuf.ProtoMember(1)]
    public int card; //牌
    [ProtoBuf.ProtoMember(2)]
    public int leaveCardCount; //剩余多少张
    [ProtoBuf.ProtoMember(3)]
    public int multiple; //胡这个牌的番薯
}

//分数结构体
[ProtoBuf.ProtoContract]
public class ScoreStruct
{
    [ProtoBuf.ProtoMember(1)]
    public int seatId; //座位号
    [ProtoBuf.ProtoMember(2)]
    public float score; //分数
}
//定缺的结构体
[ProtoBuf.ProtoContract]
public class FixedStruct
{
    [ProtoBuf.ProtoMember(1)]
    public int seatId; //座位号
    [ProtoBuf.ProtoMember(2)]
    public eFixedType type; //定缺的类型
}

//可操作列表 同步
[ProtoBuf.ProtoContract]
public class MJInstructionsProto
{
    [ProtoBuf.ProtoMember(1)]
    public List<OptItemStruct> optList;//可操作列表
}

//可操作指令的结构体
[ProtoBuf.ProtoContract]
public class OptItemStruct
{
    [ProtoBuf.ProtoMember(1)]
    public eMJInstructionsType ins; //可操作序号
    [ProtoBuf.ProtoMember(2)]
    public List<int> cards; //可操作的牌
    [ProtoBuf.ProtoMember(3)]
    public eFixedType type; //可操作的类型
    [ProtoBuf.ProtoMember(4)]
    public int otherSeatId;//其他人的座位号
    [ProtoBuf.ProtoMember(5)]
    public int subType;//可操作的二级类型
    [ProtoBuf.ProtoMember(6)]
    public int thrType;//可操作的三级类型
    [ProtoBuf.ProtoMember(7)]
    public int multiple;//可操作胡的番数
    [ProtoBuf.ProtoMember(8)]
    public List<HitCardCanHuStruct> hitCardCanHuList;//打什么牌可以胡什么 
}



//错误码
[ProtoBuf.ProtoContract]
public class ResponeErrorCode
{
    [ProtoBuf.ProtoMember(1)]
    public int code; //返回代码
    [ProtoBuf.ProtoMember(2)]
    public string desc; //返回错误码描述
}

[ProtoBuf.ProtoContract]//准备倒计时
public class MJReadyCountDownRespons
{
    [ProtoBuf.ProtoMember(1)]
    public int leaveTime;//准备倒计时 
}