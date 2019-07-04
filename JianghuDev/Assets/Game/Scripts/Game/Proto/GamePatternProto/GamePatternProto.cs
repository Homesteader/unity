using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 登录游戏服务器
/// </summary>
[ProtoBuf.ProtoContract]
public class GamePatternLoginRequest
{
    [ProtoBuf.ProtoMember(1)]
    public string token;
}

[ProtoBuf.ProtoContract]
public class GamePatternLoginResponse
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string roomId;//房间号
    [ProtoBuf.ProtoMember(3)]
    public int gameType;//游戏类型
}


[ProtoBuf.ProtoContract]
public class GamePatternSendGetGoldPeopleNumAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public GamePatternSendGetGoldPeopleNumData data;
}


[ProtoBuf.ProtoContract]
public class GamePatternSendGetGoldPeopleNumData {
    [ProtoBuf.ProtoMember(1)]
    public List<int> nums;

}


/// <summary>
/// 获取房间列表返回
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetRoomListAck {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public SendGetRoomListData data;
}

/// <summary>
/// 返回的数据
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetRoomListData {
    [ProtoBuf.ProtoMember(1)]
    public string clubId;
    [ProtoBuf.ProtoMember(2)]
    public int totalNum;
    [ProtoBuf.ProtoMember(3)]
    public int onLineNum;
    [ProtoBuf.ProtoMember(4)]
    public List<SendGetRoomListInfo> roomInfoList;
    [ProtoBuf.ProtoMember(5)]
    public float chouCheng;
    [ProtoBuf.ProtoMember(6)]
    public float roomCard;
    [ProtoBuf.ProtoMember(7)]
    public float gold;
    [ProtoBuf.ProtoMember(8)]
    public string clubName;//俱乐部名字
}

/// <summary>
/// 房间信息
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetRoomListInfo {
    [ProtoBuf.ProtoMember(1)]
    public string roomId;
    [ProtoBuf.ProtoMember(2)]
    public string name;
    [ProtoBuf.ProtoMember(3)]
    public int gameStatus;
    /// <summary>
    /// m1是俱乐部 m2是联盟 m3是好友
    /// </summary>
    [ProtoBuf.ProtoMember(4)]
    public string model;
    [ProtoBuf.ProtoMember(5)]
    public List<SendGetRoomListPlayer> userInfo;
    /// <summary>
    /// true 有空位
    /// </summary>
    [ProtoBuf.ProtoMember(6)]
    public bool nil;
    [ProtoBuf.ProtoMember(7)]
    public SendCreateRoomReq rule;
}

/// <summary>
/// 房间中的玩家信息
/// </summary>
[ProtoBuf.ProtoContract]
public class SendGetRoomListPlayer {
    [ProtoBuf.ProtoMember(1)]
    public int seatId;
    [ProtoBuf.ProtoMember(2)]
    public string headUrl;
    [ProtoBuf.ProtoMember(3)]
    public string uid;
}


[ProtoBuf.ProtoContract]
public class OnRoomListChange {
    [ProtoBuf.ProtoMember(1)]
    public SendGetRoomListInfo info;
}


/// <summary>
/// 在线人数发生变化
/// </summary>
[ProtoBuf.ProtoContract]
public class OnGamePatternOnLinePersonChang {
    [ProtoBuf.ProtoMember(1)]
    public int onLine;
}


/// <summary>
/// 改变抽成
/// </summary>
[ProtoBuf.ProtoContract]
public class SendChangChouCheng {
    [ProtoBuf.ProtoMember(1)]
    public string clubId;
    [ProtoBuf.ProtoMember(2)]
    public float chouCheng;
}