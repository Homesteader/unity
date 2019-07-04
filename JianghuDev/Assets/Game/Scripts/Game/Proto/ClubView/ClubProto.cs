using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 通用返回
/// </summary>
[ProtoBuf.ProtoContract]
public class DealMessageRecieveProto
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public DealMessageRecieveData data;
}

[ProtoBuf.ProtoContract]
public class DealMessageRecieveData
{
    [ProtoBuf.ProtoMember(1)]
    public string agentId;
}

/// <summary>
/// 借和还金币
/// </summary>
[ProtoBuf.ProtoContract]
public class SendBorrowReturnGold {
    [ProtoBuf.ProtoMember(1)]
    public float  glod;
    [ProtoBuf.ProtoMember(2)]
    public string uid;
}

[ProtoBuf.ProtoContract]
public class SendBorrowReturnGoldAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendBorrowReturnGoldData data;
}

[ProtoBuf.ProtoContract]
public class SendBorrowReturnGoldData {
    [ProtoBuf.ProtoMember(1)]
    public float ware;
}

/// <summary>
/// 取消联盟
/// </summary>
[ProtoBuf.ProtoContract]
public class SendCancalCompanyReq {
    [ProtoBuf.ProtoMember(1)]
    public string clubId;
}

/// <summary>
/// 请求俱乐部联盟
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetClubConpanyReq {
    [ProtoBuf.ProtoMember(1)]
    public string clubId;
}


[ProtoBuf.ProtoContract]
public class SendGetClubConpanyAck
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetClubConpanyData data;
}

[ProtoBuf.ProtoContract]
public class SendGetClubConpanyData {
    [ProtoBuf.ProtoMember(1)]
    public List<SendGetClubConpanyInfo> list;
    [ProtoBuf.ProtoMember(2)]
    public int num;
}

[ProtoBuf.ProtoContract]
public class SendGetClubConpanyInfo {
    [ProtoBuf.ProtoMember(1)]
    public string clubName;
    [ProtoBuf.ProtoMember(2)]
    public string cludId;
    [ProtoBuf.ProtoMember(3)]
    public float gold;
    [ProtoBuf.ProtoMember(4)]
    public string userId;//玩家id
}

/// <summary>
/// 删除俱乐部成员
/// </summary>
[ProtoBuf.ProtoContract]
public class SendDelClubUserReq {
    [ProtoBuf.ProtoMember(1)]
    public string uid;
    [ProtoBuf.ProtoMember(2)]
    public string clubId;
}

/// <summary>
/// 处理消息
/// </summary>
[ProtoBuf.ProtoContract]
public class SendDealMessageReq {
    [ProtoBuf.ProtoMember(1)]
    public int id;
    [ProtoBuf.ProtoMember(2)]
    public int deal;
}

[ProtoBuf.ProtoContract]
public class SendGetMessageReq {
    [ProtoBuf.ProtoMember(1)]
    public int page;
    [ProtoBuf.ProtoMember(2)]
    public int num;
}

[ProtoBuf.ProtoContract]
public class SendGetMessageAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetMessageData data;
}

[ProtoBuf.ProtoContract]
public class SendGetMessageData
{
    [ProtoBuf.ProtoMember(1)]
    public List<SendGetMessageInfo> list;
}

[ProtoBuf.ProtoContract]
public class SendGetMessageInfo {
    [ProtoBuf.ProtoMember(1)]
    public string content;
    [ProtoBuf.ProtoMember(2)]
    public int messageId;
    /// <summary>
    /// 1是需要处理
    /// </summary>
    [ProtoBuf.ProtoMember(3)]
    public int type;
    /// <summary>
    /// 2是未读
    /// </summary>
    [ProtoBuf.ProtoMember(4)]
    public int redState;
    [ProtoBuf.ProtoMember(5)]
    public string time;
}

/// <summary>
/// 添加联盟
/// </summary>
[ProtoBuf.ProtoContract]
public class SendAddClubConpany {
    [ProtoBuf.ProtoMember(1)]
    public string clubId;
}

/// <summary>
/// 添加爱俱乐部成员
/// </summary>
[ProtoBuf.ProtoContract]
public class SendAddClubUser {
    [ProtoBuf.ProtoMember(1)]
    public string userId;
}

/// <summary>
/// 俱乐部信息返回
/// </summary>
[ProtoBuf.ProtoContract]
public class ClubInfoAck
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public ClubInfoData data;

}

[ProtoBuf.ProtoContract]
public class ClubInfoData
{
    [ProtoBuf.ProtoMember(1)]
    public ClubInfo ClubInfo;
}

[ProtoBuf.ProtoContract]
public class ClubInfo
{
    [ProtoBuf.ProtoMember(1)]
    public string clubId;
    [ProtoBuf.ProtoMember(2)]
    public int people;
    [ProtoBuf.ProtoMember(3)]
    public float gold;
    /// <summary>
    /// 盈利的金币数量
    /// </summary>
    [ProtoBuf.ProtoMember(4)]
    public float winGold;
}


[ProtoBuf.ProtoContract]
public class SendCreateClubReq {
    [ProtoBuf.ProtoMember(1)]
    public string name;
}


[ProtoBuf.ProtoContract]
public class SendCreateClubAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
}


[ProtoBuf.ProtoContract]
public class ClubPlayerInfo
{
    [ProtoBuf.ProtoMember(1)]
    public string nickName;//名字
    [ProtoBuf.ProtoMember(2)]
    public string headUrl;//头像
    [ProtoBuf.ProtoMember(3)]
    public string userId;//玩家id
    [ProtoBuf.ProtoMember(4)]
    public float richNum;//珍珠
    [ProtoBuf.ProtoMember(5)]
    public int playCount;//局数

}

[ProtoBuf.ProtoContract]
public class ClubPlayerInfoAck
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public ClubPlayerInfoData data;
}

[ProtoBuf.ProtoContract]
public class ClubPlayerInfoData {
    [ProtoBuf.ProtoMember(1)]
    public List<ClubPlayerInfo> list;//玩家数据
    [ProtoBuf.ProtoMember(2)]
    public float allPlayerPeach;//所有玩家金币数
}


#region 联盟俱乐部
[ProtoBuf.ProtoContract]
public class ClubUnionData
{
    [ProtoBuf.ProtoMember(1)]
    public string clubId;//俱乐部id
    [ProtoBuf.ProtoMember(2)]
    public string clubName;//俱乐部名字
    [ProtoBuf.ProtoMember(3)]
    public string clubIconUrl;//俱乐部头像
    [ProtoBuf.ProtoMember(4)]
    public float richValue;//珍珠数量
    [ProtoBuf.ProtoMember(5)]
    public bool isUnion;//是否联盟
}

[ProtoBuf.ProtoContract]
public class ClubUnionInfoRecieve
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public List<ClubUnionData> data;
}
#endregion

[ProtoBuf.ProtoContract]
public class SendGetOutWinGoldReq
{
    [ProtoBuf.ProtoMember(1)]
    public int type;
}

[ProtoBuf.ProtoContract]
public class SendGetOutWinGoldAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetOutWinGoldData data;
}


[ProtoBuf.ProtoContract]
public class SendGetOutWinGoldData {
    /// <summary>
    /// 当前俱乐部的前
    /// </summary>
    [ProtoBuf.ProtoMember(1)]
    public float gold;
}


[ProtoBuf.ProtoContract]
public class SendCheckUserInfoReq {
    [ProtoBuf.ProtoMember(1)]
    public string uid;
}


[ProtoBuf.ProtoContract]
public class SendCheckUserInfoAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string des;
    [ProtoBuf.ProtoMember(3)]
    public SendCheckUserInfoData data;
}


[ProtoBuf.ProtoContract]
public class SendCheckUserInfoData {
    [ProtoBuf.ProtoMember(1)]
    public string nickName;
}


[ProtoBuf.ProtoContract]
public class SendGetGainNumAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string des;
    [ProtoBuf.ProtoMember(3)]
    public SendGetGainNumData data;
}

[ProtoBuf.ProtoContract]
public class SendGetGainNumData {
    [ProtoBuf.ProtoMember(1)]
    public SendGetGainNumInfo info;
}

[ProtoBuf.ProtoContract]
public class SendGetGainNumInfo {
    [ProtoBuf.ProtoMember(1)]
    public float club;
    [ProtoBuf.ProtoMember(2)]
    public float agent;
}


[ProtoBuf.ProtoContract]
public class SendGetRankForGameRoundReq {
    [ProtoBuf.ProtoMember(1)]
    public string type;
}


/// <summary>
/// 排行榜返回
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetRankForGameRoundAck
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetRankForGameRoundData data;
}


[ProtoBuf.ProtoContract]
public class SendGetRankForGameRoundData
{
    [ProtoBuf.ProtoMember(1)]
    public List<SendGetRankForGameRoundInfo> info;
}


[ProtoBuf.ProtoContract]
public class SendGetRankForGameRoundInfo
{
    [ProtoBuf.ProtoMember(1)]
    public string headUrl;
    [ProtoBuf.ProtoMember(2)]
    public string nickName;
    [ProtoBuf.ProtoMember(3)]
    public float gold;
    [ProtoBuf.ProtoMember(4)]
    public int round;
    [ProtoBuf.ProtoMember(5)]
    public string userId;//玩家id
}

#region 总代
[ProtoBuf.ProtoContract]
public struct GeneralAgrentRecieve
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public GeneralAgrentData data;
}

[ProtoBuf.ProtoContract]
public struct GeneralAgrentData
{
    [ProtoBuf.ProtoMember(1)]
    public GeneralProfitData gpData;//总代收益
    [ProtoBuf.ProtoMember(2)]
    public HigherUpAgentData higherupAgrent;//上级代理
    [ProtoBuf.ProtoMember(3)]
    public GeneralSubAgentData[] subAgent;//下级代理
}

//总代收益
[ProtoBuf.ProtoContract]
public struct GeneralProfitData
{
    [ProtoBuf.ProtoMember(1)]
    public float dayProfit;//今日收益
    [ProtoBuf.ProtoMember(2)]
    public float monthProfit;//本月收益
    [ProtoBuf.ProtoMember(3)]
    public float lastMonthProfit;//上月收益
}

//上级代理信息
[ProtoBuf.ProtoContract]
public struct HigherUpAgentData
{
    [ProtoBuf.ProtoMember(1)]
    public string headUrl;//头像
    [ProtoBuf.ProtoMember(2)]
    public string nickname;//名字
    [ProtoBuf.ProtoMember(3)]
    public string userId;//id
}

//总代下一级代理
[ProtoBuf.ProtoContract]
public struct GeneralSubAgentData
{
    [ProtoBuf.ProtoMember(1)]
    public string headUrl;//头像
    [ProtoBuf.ProtoMember(2)]
    public string nickname;//名字
    [ProtoBuf.ProtoMember(3)]
    public string userId;//id
    [ProtoBuf.ProtoMember(4)]
    public float dayProfit;//今日收益
    [ProtoBuf.ProtoMember(5)]
    public float monthProfit;//本月收益
    [ProtoBuf.ProtoMember(6)]
    public float lastMonthProfit;//上月收益
}
#endregion

//获取客服信息返回
[ProtoBuf.ProtoContract]
public class ReceiveServiceInfo
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public ServiceInfo data;
}

//客服信息
[ProtoBuf.ProtoContract]
public class ServiceInfo
{
    [ProtoBuf.ProtoMember(1)]
    public string[] wechatId;
}