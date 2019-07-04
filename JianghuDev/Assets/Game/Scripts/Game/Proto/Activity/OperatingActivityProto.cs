using UnityEngine;
using System.Collections.Generic;

// 转盘配置 
[ProtoBuf.ProtoContract]
public class GetPrizeConfigBack
{
    [ProtoBuf.ProtoMember(1)]
    public int code;       //错误代码
    [ProtoBuf.ProtoMember(2)]
    public string desc;    //错误描述
    [ProtoBuf.ProtoMember(3)]
    public GetPrizeConfigBackData data;    //数据	
}
[ProtoBuf.ProtoContract]
public struct GetPrizeConfigBackData
{
    [ProtoBuf.ProtoMember(1)]
    public List<PrizeInfo> info;
    [ProtoBuf.ProtoMember(2)]
    public int drawNum;        //可抽奖
    [ProtoBuf.ProtoMember(3)]
    public int drawTotal;      //已抽奖
    [ProtoBuf.ProtoMember(4)]
    public int games;          //游戏局数
    [ProtoBuf.ProtoMember(5)]
    public string rule;        //规则
 }
[ProtoBuf.ProtoContract]
public struct PrizeInfo
{
    [ProtoBuf.ProtoMember(1)]
    public int prizeId;
    [ProtoBuf.ProtoMember(2)]
    public string name;
    [ProtoBuf.ProtoMember(3)]
    public string type;
    [ProtoBuf.ProtoMember(4)]
    public int num;
    [ProtoBuf.ProtoMember(5)]
    public string note;
}

// 抽奖返回
[ProtoBuf.ProtoContract]
public class DrawBack
{
    [ProtoBuf.ProtoMember(1)]
    public int code;     //错误代码
    [ProtoBuf.ProtoMember(2)]
    public string desc;    //错误描述
    [ProtoBuf.ProtoMember(3)]
    public DrawBackData data;    //数据		
}
[ProtoBuf.ProtoContract]
public struct DrawBackData
{
    [ProtoBuf.ProtoMember(1)]
    public PrizeInfo prizeInfo;    // 中奖数据
}


// 获取抽奖记录 
[ProtoBuf.ProtoContract]
public class GetDrawRecord
{
    [ProtoBuf.ProtoMember(1)]
    public int code;     //错误代码
    [ProtoBuf.ProtoMember(2)]
    public string desc;    //错误描述
    [ProtoBuf.ProtoMember(3)]
    public GetDrawRecordData data;    //数据	
}

[ProtoBuf.ProtoContract]
public struct GetDrawRecordData
{
    [ProtoBuf.ProtoMember(1)]
    public List<RecordInfo> recordInfo;
}

[ProtoBuf.ProtoContract]
public struct RecordInfo
{
    [ProtoBuf.ProtoMember(1)]
    public string name;
    [ProtoBuf.ProtoMember(2)]
    public int num;
    [ProtoBuf.ProtoMember(3)]
    public string state;
    [ProtoBuf.ProtoMember(4)]
    public string time;
}