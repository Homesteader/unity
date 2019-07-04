using UnityEngine;
using System.Collections;

#region 发送聊天信息

[ProtoBuf.ProtoContract]
public class SendReceiveGameChat//发送聊天信息  主动
{
    /// <summary>
    /// 表情
    /// </summary>
    ///Face = 1,
    /// <summary>
    /// 普通文字
    /// </summary>
    ///Chat = 2,
    /// <summary>
    /// 文字语音
    /// </summary>
    ///TexTVoice = 3,
    /// <summary>
    /// 互动表情
    /// </summary>
    /// HDFace = 4,
    /// <summary>
    /// 语音
    /// </summary>
    ///Voice = 5,
    /// </summary>
    [ProtoBuf.ProtoMember(1)]
    public int chatType;
    /// <summary>
    /// 聊天内容  语音为网址
    /// </summary>
    [ProtoBuf.ProtoMember(2)]
    public string content;
    /// <summary>
    /// 表情id
    /// </summary> 
    [ProtoBuf.ProtoMember(3)]
    public int faceIndex;
    /// <summary>
    /// 语音播放时长
    /// </summary>   
    [ProtoBuf.ProtoMember(4)]
    public int voiceChatTime;
    /// <summary>
    /// 语音播放时长
    /// </summary>   
    [ProtoBuf.ProtoMember(5)]
    /// <summary>
    /// 发送人座位号
    /// </summary>   
    public int fromSeatId;        
    [ProtoBuf.ProtoMember(6)]
    /// <summary>
    /// 接收人座位号
    /// </summary>   
    public int toSeatId; 
    [ProtoBuf.ProtoMember(7)]
    public int sex;
}

#endregion
