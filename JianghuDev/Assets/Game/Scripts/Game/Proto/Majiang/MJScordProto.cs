using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJRecordListProto : MsgResponseBase
{

    public class MJScordListItem
    {
        //headUrl   头像地址
        public string createDate; //游戏时间
        public string fileUrl;//回放的文件下载地址，需要+"_"+totalCount
        public string homeowners;//房主昵称
        public string id;//??
        public string roomId;//房间号
        public int totalCount;//总局数
        public string usersInfo;//玩家信息
    }

    public class Data
    {
        public MJScordListItem[] scordList;
    }

    public Data data;
}

[ProtoBuf.ProtoContract]
public class MJRecordDetailInfo
{
    [ProtoBuf.ProtoMember(1)]
    public List<MJRecordItemData> recodeList;//战绩列表
}

[ProtoBuf.ProtoContract]
public class MJRecordItemData
{
    [ProtoBuf.ProtoMember(1)]
    public MJRecordPlayerData[] usersInfo;//战绩里包含的玩家信息
    [ProtoBuf.ProtoMember(2)]
    public string fileUrl2;//回放文件下载地址
    [ProtoBuf.ProtoMember(3)]
    public string createTime;//时间
    [ProtoBuf.ProtoMember(4)]
    public string roomName;//游戏名字
}

[ProtoBuf.ProtoContract]
public class MJRecordPlayerData
{
    [ProtoBuf.ProtoMember(1)]
    public int uid;//玩家id
    [ProtoBuf.ProtoMember(2)]
    public string headUrl;//头像
    [ProtoBuf.ProtoMember(3)]
    public string nickName;//昵称
    [ProtoBuf.ProtoMember(4)]
    public float score;//分数
}

[ProtoBuf.ProtoContract]
public class MJRecordData
{
    [ProtoBuf.ProtoMember(1)]
    public RoomStruct roomInfo;//房间的数据
    [ProtoBuf.ProtoMember(2)]
    public StartGameStruct startInfo;//开始游戏的数据
    [ProtoBuf.ProtoMember(3)]
    public MJoptInfoData[] effOptList;//操作指令
    [ProtoBuf.ProtoMember(4)]
    public litSemResponse semData;//小结算
}