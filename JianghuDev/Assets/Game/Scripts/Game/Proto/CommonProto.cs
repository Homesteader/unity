using UnityEngine;
using System.Collections;


/// <summary>
/// 通用返回
/// </summary>
[ProtoBuf.ProtoContract]
public class CommonRecieveProto
{
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
}


/// <summary>
/// 通用请求
/// </summary>
[ProtoBuf.ProtoContract]
public class CommonSendProto
{

}




#region 撞线通知

/// <summary>
/// 其他设备登录该账号
/// </summary>
[ProtoBuf.ProtoContract]
public class CommonHitUser {
    [ProtoBuf.ProtoMember(1)]
    public int code;
    [ProtoBuf.ProtoMember(2)]
    public string desc;
    [ProtoBuf.ProtoMember(3)]
    public CommonHitUserData data;
}


[ProtoBuf.ProtoContract]
public class CommonHitUserData {
    [ProtoBuf.ProtoMember(1)]
    public int id;
    [ProtoBuf.ProtoMember(2)]
    public string name;
}

#endregion


#region 发送经纬度

[ProtoBuf.ProtoContract]
public class SendAddrReq
{
    [ProtoBuf.ProtoMember(1)]
    public float longitude;//经度
    [ProtoBuf.ProtoMember(2)]
    public float latitude;//纬度
    [ProtoBuf.ProtoMember(3)]
    public string address;//地址
}

#endregion

