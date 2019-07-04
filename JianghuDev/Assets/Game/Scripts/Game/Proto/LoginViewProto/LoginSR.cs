using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoginSR
{

    #region
    /// <summary>
    /// 登录
    /// </summary>
    /*********************************************///start
    [ProtoBuf.ProtoContract]
    public class SendLogin
    {
        [ProtoBuf.ProtoMember(1)]
        public string openId;       //微信openId
        [ProtoBuf.ProtoMember(2)]
        public string nickname;     //微信昵称
        [ProtoBuf.ProtoMember(3)]
        public string headUrl;      //微信头像
        [ProtoBuf.ProtoMember(4)]
        public int sex;           //微信性别 1:男 其他：女
        [ProtoBuf.ProtoMember(5)]
        public string token;
        [ProtoBuf.ProtoMember(6)]
        public string account; //账号
        [ProtoBuf.ProtoMember(7)]
        public string password; //密码
        [ProtoBuf.ProtoMember(8)]
        public bool isVisitor;//是否是游客

    }

    /********************************************///end

    #endregion


    #region 游戏服务器登录大厅服务器

    /// <summary>
    /// 游戏服务器登录大厅服务器
    /// </summary>
    [ProtoBuf.ProtoContract]
    public class GameLoginMain {
        [ProtoBuf.ProtoMember(1)]
        public string userId;
        [ProtoBuf.ProtoMember(2)]
        public string token;
    }

    #endregion

    [ProtoBuf.ProtoContract]
    public class LoginBack
    {
        [ProtoBuf.ProtoMember(1)]
        public int code;
        [ProtoBuf.ProtoMember(2)]
        public string desc;
        [ProtoBuf.ProtoMember(3)]
        public LoginAuthData data;//数据
        [ProtoBuf.ProtoMember(4)]
        public string roomId;// 房间id
    }

    [ProtoBuf.ProtoContract]
    public class LoginAuthData
    {
        [ProtoBuf.ProtoMember(1)]
        public LoginUserInfo userInfo;
        [ProtoBuf.ProtoMember(2)]
        public LoginSrverState gameServer;
    }

    [ProtoBuf.ProtoContract]
    public class LoginUserInfo {
        [ProtoBuf.ProtoMember(1)]
        public string userId;
        [ProtoBuf.ProtoMember(2)]
        public string openId;
        [ProtoBuf.ProtoMember(3)]
        public string nickname;
        [ProtoBuf.ProtoMember(4)]
        public int sex;
        [ProtoBuf.ProtoMember(5)]
        public string headUrl;
        [ProtoBuf.ProtoMember(6)]
        public string realName;
        [ProtoBuf.ProtoMember(7)]
        public string idNumber;
        [ProtoBuf.ProtoMember(8)]
        public string ip;
        [ProtoBuf.ProtoMember(9)]
        public string clubId;
        [ProtoBuf.ProtoMember(10)]
        public string state;//没用的
        [ProtoBuf.ProtoMember(11)]
        public string account;
        [ProtoBuf.ProtoMember(12)]
        public float roomCard;
        [ProtoBuf.ProtoMember(13)]
        public float gold;
        [ProtoBuf.ProtoMember(14)]
        public float wareHouse;//仓库
        [ProtoBuf.ProtoMember(15)]
        public string token;
        /// <summary>
        /// 1是代理 2不是代理
        /// </summary>
        [ProtoBuf.ProtoMember(16)]
        public int agent;//代理标识
        [ProtoBuf.ProtoMember(17)]
        public float longitude;//经度
        [ProtoBuf.ProtoMember(18)]
        public float latitude;//纬度
        [ProtoBuf.ProtoMember(19)]
        public string address;
        [ProtoBuf.ProtoMember(20)]
        public bool newMessage;
        [ProtoBuf.ProtoMember(21)]
        public float point;//积分
        /// <summary>
        /// 他的代理id
        /// </summary>
        [ProtoBuf.ProtoMember(22)]
        public string agentId;
        [ProtoBuf.ProtoMember(23)]
        public bool isGeneralAgent;//是否是总代理
        [ProtoBuf.ProtoMember(24)]
        public bool IsUseChat;//是否可以使用聊天
    }

    [ProtoBuf.ProtoContract]
    public class LoginSrverState{
        [ProtoBuf.ProtoMember(1)]
        public int gameType;
        [ProtoBuf.ProtoMember(2)]
        public string gameName;
        [ProtoBuf.ProtoMember(3)]
        public string ip;
        [ProtoBuf.ProtoMember(4)]
        public string port;
    }

    /// <summary>
    /// 异地登录
    /// </summary>
    [ProtoBuf.ProtoContract]
    public class LoingInOtherRecieve
    {
        
    }


    #region 扫二维码加好友
    [ProtoBuf.ProtoContract]
    public class SendInstallAddFriend
    {
        [ProtoBuf.ProtoMember(1)]
        public string agent;//代理id
    }
    #endregion
}

