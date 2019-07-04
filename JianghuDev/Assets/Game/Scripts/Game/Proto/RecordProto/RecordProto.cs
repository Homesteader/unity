using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract]
public class RecordData
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public List<RecordItemData> data;
}

[ProtoBuf.ProtoContract]
public class RecordItemData  {
    [ProtoBuf.ProtoMember(1)]
    public string time;//时间
    [ProtoBuf.ProtoMember(2)]
    public List<RecordPlayerData> record;//玩家数据
}

[ProtoBuf.ProtoContract]
public class RecordPlayerData
{
    [ProtoBuf.ProtoMember(1)]
    public string userId;//玩家id
    [ProtoBuf.ProtoMember(2)]
    public string username;//玩家名字
    [ProtoBuf.ProtoMember(3)]
    public string headUrl;//头像
    [ProtoBuf.ProtoMember(4)]
    public float point;//分数
    [ProtoBuf.ProtoMember(5)]
    public string[] card;//牌
}
