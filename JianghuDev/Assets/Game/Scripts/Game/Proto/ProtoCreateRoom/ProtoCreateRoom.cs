using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ProtoBuf.ProtoContract]
public class SendCreateRoom
{
    [ProtoBuf.ProtoMember(1)]
    public int GameID;          //游戏编号
    [ProtoBuf.ProtoMember(2)]
    public int RuleID;          //分类游戏编号
    [ProtoBuf.ProtoMember(3)]
    public List<RoomRule> rules;  //规则
}

[ProtoBuf.ProtoContract]
public class RoomRule
{
    [ProtoBuf.ProtoMember(1)]
    public int id;              //规则分类ID
    [ProtoBuf.ProtoMember(2)]
    public List<RoomRuleItem> datas;     //规则数据
}

[ProtoBuf.ProtoContract]
public class RoomRuleItem
{
    [ProtoBuf.ProtoMember(1)]
    public int id;
    [ProtoBuf.ProtoMember(2)]
    public float value = -1;
}

/// <summary>
/// 创建房间
/// </summary>

[ProtoBuf.ProtoContract]
public class SendCreateRoomReq
{
    [ProtoBuf.ProtoMember(1)]
    public int gameId;//游戏id
    [ProtoBuf.ProtoMember(2)]
    public int subType;//游戏子类型
    [ProtoBuf.ProtoMember(3)]
    public List<int> ruleIndexs;//选择的规则id
    [ProtoBuf.ProtoMember(4)]
    public float baseScore;//底分
    [ProtoBuf.ProtoMember(5)]
    public float into;//带入 
}