using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 向服务器发送购买物品ID
/// </summary>
[ProtoBuf.ProtoContract]
public class PayItemId
{
    [ProtoBuf.ProtoMember(1)]
    public int id;  //物品id
    [ProtoBuf.ProtoMember(2)]
    public string version;//版本号
    [ProtoBuf.ProtoMember(3)]
    public string subject;//商品名称
    [ProtoBuf.ProtoMember(4)]
    public string body;//商品描述
    [ProtoBuf.ProtoMember(5)]
    public string appinfo;//应用信息
    [ProtoBuf.ProtoMember(6)]
    public string payChannelId;//支付方式 wechat,alipay
}


/// <summary>
/// 服务器返回的具体支付数据
/// </summary>
[ProtoBuf.ProtoContract]
public class PayInfoItem
{
    [ProtoBuf.ProtoMember(1)]
    public string appKey; //交易发起所属appKey
    [ProtoBuf.ProtoMember(2)]
    public string amount; //订单总金额，单位分
    [ProtoBuf.ProtoMember(3)]
    public string orderNo; //订单号
    [ProtoBuf.ProtoMember(4)]
    public string notifyUrl;// 支付结果通知地址
    [ProtoBuf.ProtoMember(5)]
    public string ip;//ip地址
}

/// <summary>
/// 充值数据
/// </summary>
[ProtoBuf.ProtoContract]
public class PayInfoData
{
    [ProtoBuf.ProtoMember(1)]
    public PayInfoItem payData;  //充值数据
}

/// <summary>
/// 充值消息返回
/// </summary>
[ProtoBuf.ProtoContract]
public class PayInfoBack
{
    [ProtoBuf.ProtoMember(1)]
    public int code;     //错误代码
    [ProtoBuf.ProtoMember(2)]
    public string desc;    //错误描述
    [ProtoBuf.ProtoMember(3)]
    public PayInfoData data;  //返回数据
}


[ProtoBuf.ProtoContract]
public class SendPayRoomCardReq
{
    [ProtoBuf.ProtoMember(1)]
    public int id;
    [ProtoBuf.ProtoMember(2)]
    public string pay;
}
/// <summary>
/// 充值消息返回
/// </summary>
[ProtoBuf.ProtoContract]
public class PayUrlBack
{
    [ProtoBuf.ProtoMember(1)]
    public int code;     //错误代码
    [ProtoBuf.ProtoMember(2)]
    public string desc;    //错误描述
    [ProtoBuf.ProtoMember(3)]
    public PayUrlBackData data;  //
}

[ProtoBuf.ProtoContract]
public class PayUrlBackData
{
    [ProtoBuf.ProtoMember(1)]
    public string url; //
}

[ProtoBuf.ProtoContract]
public class SendPayBuyRoomCardReq
{
    [ProtoBuf.ProtoMember(1)]
    public int id;
}


[ProtoBuf.ProtoContract]
public class SendPayBuyRoomCardAck
{
    [ProtoBuf.ProtoMember(1)]
    public int code;     //错误代码
    [ProtoBuf.ProtoMember(2)]
    public string desc;    //错误描述
    [ProtoBuf.ProtoMember(3)]
    public SendPayBuyRoomCardData data;  //返回数据
}


[ProtoBuf.ProtoContract]
public class SendPayBuyRoomCardData
{
    [ProtoBuf.ProtoMember(1)]
    public float roomCard;
    [ProtoBuf.ProtoMember(2)]
    public float gold;
}