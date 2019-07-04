using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#region xxgame



/// <summary>
/// 福利图片
/// </summary>
[ProtoBuf.ProtoContract]
public class GetWelfarepro
{

    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public List<string> texlist;

}



/// <summary>
/// 获取仓库信息返回
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetWareInfoAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetWareInfoData data;
}

[ProtoBuf.ProtoContract]
public class SendGetWareInfoData {
    [ProtoBuf.ProtoMember(1)]
    public SendGetWareInfo info;
}


[ProtoBuf.ProtoContract]
public class SendGetWareInfo
{
    [ProtoBuf.ProtoMember(1)]
    public float gold;
    [ProtoBuf.ProtoMember(2)]
    public float ware;
    [ProtoBuf.ProtoMember(3)]
    public float roomCard;
}


/// <summary>
/// 存取仓库
/// </summary>
[ProtoBuf.ProtoContract]
public class SendSaveOutWare {
    [ProtoBuf.ProtoMember(1)]
    public float glod;
}

[ProtoBuf.ProtoContract]
public class SendGetOutWare
{
    [ProtoBuf.ProtoMember(1)]
    public float glod;
    [ProtoBuf.ProtoMember(2)]
    public string pwd;//密码
}


/// <summary>
/// 获取公告返回
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetNoticeAck{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetNoticeAckData data;
}

[ProtoBuf.ProtoContract]
public class SendGetNoticeAckData {
    [ProtoBuf.ProtoMember(1)]
    public List<string> messages;
}

[ProtoBuf.ProtoContract]
public class SendGetActivityAck
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetActivityData data;
}

[ProtoBuf.ProtoContract]
public class SendGetActivityData {
    [ProtoBuf.ProtoMember(1)]
    public List<SendGetActivityDataList> list;
}

[ProtoBuf.ProtoContract]
public class SendGetActivityDataList {
    [ProtoBuf.ProtoMember(1)]
    public string addr;//图片路径
    [ProtoBuf.ProtoMember(2)]
    public string lineto;//跳转链接
}



/// <summary>
/// 发送获取游戏Ip
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetGameServerReq
{
    [ProtoBuf.ProtoMember(1)]
    public int gameType;//游戏id
}

/// <summary>
/// 发送获取Ip返回
/// </summary>
[ProtoBuf.ProtoContract]
public class GetGameServerAck
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public GameServerInfoData data;
}

[ProtoBuf.ProtoContract]
public class GameServerInfoData {
    [ProtoBuf.ProtoMember(1)]
    public GameServerInfo Serverinfo;
}


[ProtoBuf.ProtoContract]
public class GameServerInfo
{
    /// <summary>
    /// 1是麻将 2是牛牛 3是金花
    /// </summary>
    [ProtoBuf.ProtoMember(1)]
    public int gameType;
    [ProtoBuf.ProtoMember(2)]
    public string name;
    [ProtoBuf.ProtoMember(3)]
    public string ServerIp;
    [ProtoBuf.ProtoMember(4)]
    public string ServerPort;
}
#endregion


/// <summary>
/// 玩家信息
/// </summary>
[ProtoBuf.ProtoContract]
public class PlayerInfo
{
    [ProtoBuf.ProtoMember(1)]
    public int roomCard;//房卡
    [ProtoBuf.ProtoMember(2)]
    public int golds;//金币数量
    [ProtoBuf.ProtoMember(3)]
    public string userId;//玩家id
    [ProtoBuf.ProtoMember(4)]
    public string nickname;//昵称
    [ProtoBuf.ProtoMember(5)]
    public string headImgUrl;//头像
    [ProtoBuf.ProtoMember(6)]
    public int sex;//性别 1：男 其他：女
    [ProtoBuf.ProtoMember(7)]
    public string eMail;//是否有未读邮件 'true'为有
    [ProtoBuf.ProtoMember(8)]
    public string task;//是否有未领取任务 'true'为有
    [ProtoBuf.ProtoMember(13)]
    public bool haveAgent;//是否绑定代理
}



/// <summary>
/// 活动数据
/// </summary>
[ProtoBuf.ProtoContract]
public class ActivityData
{
    [ProtoBuf.ProtoMember(1)]
    public string index; //图片下编
    [ProtoBuf.ProtoMember(2)]
    public string addr;//图片路径
    [ProtoBuf.ProtoMember(3)]
    public string lineto;//跳转链接
}

/// <summary>
/// 接收到广播消息
/// </summary>
[ProtoBuf.ProtoContract]
public class BroadRecieve
{
    [ProtoBuf.ProtoMember(1)]
    public List<string> broadcast;//广播消息
}


#region 反馈
/// <summary>
/// 反馈
/// </summary>
[ProtoBuf.ProtoContract]
public class FeedBackProto
{
    [ProtoBuf.ProtoMember(1)]
    public string content;//反馈内容
}
#endregion


#region 更新房卡和金币

/// <summary>
/// 房卡和金币的更新
/// </summary>
[ProtoBuf.ProtoContract]
public class GetRooMCardGlodsUpdate
{
    [ProtoBuf.ProtoMember(1)]
    public float roomCard = -1;
    [ProtoBuf.ProtoMember(2)]
    public float glods = -1;
}


#endregion

/// <summary>
/// 实名认证
/// </summary>
[ProtoBuf.ProtoContract]
public class SendCheckRealName {
    [ProtoBuf.ProtoMember(1)]
    public string realName;
    [ProtoBuf.ProtoMember(2)]
    public string idCard;
}


/// <summary>
/// 获取排名信息
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetRankReq {
    /// <summary>
    /// 1是珍珠 2是积分
    /// </summary>
    [ProtoBuf.ProtoMember(1)]
    public int type;
}

/// <summary>
/// 排行榜返回
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetRankAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetRankData data;
}


[ProtoBuf.ProtoContract]
public class SendGetRankData {
    [ProtoBuf.ProtoMember(1)]
    public List<SendGetRankInfo> info;
}


[ProtoBuf.ProtoContract]
public class SendGetRankInfo {
    [ProtoBuf.ProtoMember(1)]
    public string headUrl;
    [ProtoBuf.ProtoMember(2)]
    public string nickName;
    [ProtoBuf.ProtoMember(3)]
    public float gold;
    [ProtoBuf.ProtoMember(4)]
    public float score;
    [ProtoBuf.ProtoMember(5)]
    public string userId;//玩家id
    [ProtoBuf.ProtoMember(6)]
    public string weChat;//玩家id
}

/// <summary>
/// 查看代理盈利返回
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetAgentWinAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetAgentWinData data;
}


[ProtoBuf.ProtoContract]
public class SendGetAgentWinData {
    [ProtoBuf.ProtoMember(1)]
    public List<SendGetAgentWinList> agentList;
    [ProtoBuf.ProtoMember(2)]
    public int agentNum;//代理总人数
    [ProtoBuf.ProtoMember(3)]
    public SendGetAgentWinList higherAgent;//上级代理
}


[ProtoBuf.ProtoContract]
public class SendGetAgentWinList {
    [ProtoBuf.ProtoMember(1)]
    public string headUrl;
    [ProtoBuf.ProtoMember(2)]
    public string nickname;
    [ProtoBuf.ProtoMember(3)]
    public string userId;
    /// <summary>
    /// 一级代理创造的收入
    /// </summary>
    [ProtoBuf.ProtoMember(4)]
    public float oneBenefit;
    [ProtoBuf.ProtoMember(5)]
    public SendGetAgentWinTwo two;
    [ProtoBuf.ProtoMember(6)]
    public float selfPeach;//本身金币
    [ProtoBuf.ProtoMember(7)]
    public float warehousePeach;//仓库金币
}

[ProtoBuf.ProtoContract]
public class SendGetAgentWinTwo
{
    [ProtoBuf.ProtoMember(1)]
    public string userId;
    /// <summary>
    /// 人数
    /// </summary>
    [ProtoBuf.ProtoMember(2)]
    public int agentTotal;
    /// <summary>
    /// 二级代理创造的收入
    /// </summary>
    [ProtoBuf.ProtoMember(3)]
    public float sum;
}

/// <summary>
/// 获取盈利收入的详情
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetGainDetail
{
    /// <summary>
    /// 页数
    /// </summary>
    [ProtoBuf.ProtoMember(1)]
    public int page;
    /// <summary>
    /// 每一页有多少条
    /// </summary>
    [ProtoBuf.ProtoMember(2)]
    public int num;
    /// <summary>
    /// 类型 NN ZJH MJ AGENT
    /// </summary>
    [ProtoBuf.ProtoMember(3)]
    public string type;
}


/// <summary>
/// 详情返回
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetGainAck
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetGainData data;
}


[ProtoBuf.ProtoContract]
public class SendGetGainData
{
    [ProtoBuf.ProtoMember(1)]
    public List<SendGetGainInfo> infoList;
    [ProtoBuf.ProtoMember(2)]
    public int totalNum;
    [ProtoBuf.ProtoMember(3)]
    public PlayerRecordDetailNum statisticsNum;//统计数量
}

[ProtoBuf.ProtoContract]
public class SendGetGainInfo
{
    [ProtoBuf.ProtoMember(1)]
    public string uid;
    [ProtoBuf.ProtoMember(2)]
    public string nickName;
    [ProtoBuf.ProtoMember(3)]
    public float gold;
    [ProtoBuf.ProtoMember(4)]
    public string time;
    [ProtoBuf.ProtoMember(5)]
    public int messageId;
}



/// <summary>
/// 获取盈利收入的详情
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetAgentBRDetail
{
    /// <summary>
    /// 页数
    /// </summary>
    [ProtoBuf.ProtoMember(1)]
    public int page;
    /// <summary>
    /// 每一页有多少条
    /// </summary>
    [ProtoBuf.ProtoMember(2)]
    public int num;
    /// <summary>
    /// 类型 jc jr gh rc
    /// </summary>
    [ProtoBuf.ProtoMember(3)]
    public string type;
}



/// <summary>
/// 详情返回
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetAgentBRAck
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetAgentBRData data;
}


[ProtoBuf.ProtoContract]
public class SendGetAgentBRData
{
    [ProtoBuf.ProtoMember(1)]
    public List<SendGetAgentBRInfo> infoList;
    [ProtoBuf.ProtoMember(2)]
    public SendGetAgentBRTotal totalNum;
}


[ProtoBuf.ProtoContract]
public class SendGetAgentBRTotal
{
    [ProtoBuf.ProtoMember(1)]
    public float today;
    [ProtoBuf.ProtoMember(2)]
    public float weak;
    [ProtoBuf.ProtoMember(3)]
    public float month;
}



[ProtoBuf.ProtoContract]
public class PlayerRecordDetailBack
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public PlayerRecordDetailInfo data;
}

[ProtoBuf.ProtoContract]
public class PlayerRecordDetailInfo
{
    [ProtoBuf.ProtoMember(1)]
    public List<SendGetAgentBRInfo> infoList;
    [ProtoBuf.ProtoMember(2)]
    public PlayerRecordDetailNum num;//数量
}
[ProtoBuf.ProtoContract]
public class PlayerRecordDetailNum
{
    [ProtoBuf.ProtoMember(1)]
    public float day;//今天
    [ProtoBuf.ProtoMember(2)]
    public float week;//本周
    [ProtoBuf.ProtoMember(3)]
    public float month;//本月
}

[ProtoBuf.ProtoContract]
public class SendGetAgentBRInfo
{
    [ProtoBuf.ProtoMember(1)]
    public string uid;
    [ProtoBuf.ProtoMember(2)]
    public string nickName;
    [ProtoBuf.ProtoMember(3)]
    public float gold;
    [ProtoBuf.ProtoMember(4)]
    public string time;
    [ProtoBuf.ProtoMember(5)]
    public int messageId;
    [ProtoBuf.ProtoMember(6)]
    public string headUrl;
}




/// <summary>
/// 获取盈利收入的详情
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetUserBRDetail
{
    /// <summary>
    /// 页数
    /// </summary>
    [ProtoBuf.ProtoMember(1)]
    public int page;
    /// <summary>
    /// 每一页有多少条
    /// </summary>
    [ProtoBuf.ProtoMember(2)]
    public int num;
    /// <summary>
    /// 类型 jc jr gh rc
    /// </summary>
    [ProtoBuf.ProtoMember(3)]
    public string type;
}


/// <summary>
/// 详情返回
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetUserBRAck
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetUserBRData data;
}


[ProtoBuf.ProtoContract]
public class SendGetUserBRData
{
    [ProtoBuf.ProtoMember(1)]
    public List<SendGetUserBRInfo> infoList;
    [ProtoBuf.ProtoMember(2)]
    public int totalNum;
}

[ProtoBuf.ProtoContract]
public class SendGetUserBRInfo
{
    [ProtoBuf.ProtoMember(1)]
    public string uid;
    [ProtoBuf.ProtoMember(2)]
    public string nickName;
    [ProtoBuf.ProtoMember(3)]
    public float gold;
    [ProtoBuf.ProtoMember(4)]
    public string time;
    [ProtoBuf.ProtoMember(5)]
    public int messageId;
    [ProtoBuf.ProtoMember(6)]
    public string headUrl;
}

[ProtoBuf.ProtoContract]
public class SendReadMessageReq {
    [ProtoBuf.ProtoMember(1)]
    public int id;
}


//修改密码
[ProtoBuf.ProtoContract]
public class SendFixPsdReq
{
    [ProtoBuf.ProtoMember(1)]
    public string lastPwd;//旧密码
    [ProtoBuf.ProtoMember(2)]
    public string newPwd;//新密码
}


[ProtoBuf.ProtoContract]
public class SendJoinClubReq
{
    [ProtoBuf.ProtoMember(1)]
    public string id;
}
[ProtoBuf.ProtoContract]
public class SendLuckDishResultData
{
    [ProtoBuf.ProtoMember(1)]
    public int type;//返回码
}


[ProtoBuf.ProtoContract]
public class GetLuckDishCountBackData
{
    [ProtoBuf.ProtoMember(1)]
    public int code;//返回码
    [ProtoBuf.ProtoMember(2)]
    public string desc;//描述说明
    [ProtoBuf.ProtoMember(3)]
    public GetLuckDishCountData data;
}
[ProtoBuf.ProtoContract]
public class GetLuckDishCountData
{
    [ProtoBuf.ProtoMember(1)]
    public bool isExistCount;//是否存在
    [ProtoBuf.ProtoMember(2)]
    public int time;//时间
}
[ProtoBuf.ProtoContract]
public class GetLuckDishResultBackData
{
    [ProtoBuf.ProtoMember(1)]
    public int code;//返回码
    [ProtoBuf.ProtoMember(2)]
    public string desc;//描述说明
    [ProtoBuf.ProtoMember(3)]
    public GetLuckDishResultData data;
}
[ProtoBuf.ProtoContract]
public class GetLuckDishResultData
{
    [ProtoBuf.ProtoMember(1)]
    public int gold;//
    [ProtoBuf.ProtoMember(2)]
    public int diamond;//
    [ProtoBuf.ProtoMember(3)]
    public int ticket;//奖券
    [ProtoBuf.ProtoMember(4)]
    public int id;//奖励id
}


