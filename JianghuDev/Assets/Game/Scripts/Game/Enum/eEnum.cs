using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏类型
/// </summary>
public enum eGameType {
    MaJiang = 1,
    GoldFlower = 2,
    NiuNiu = 3,
    TenHalf = 4,        //十点半
}


public enum eToggleType
{
    toggle = 1,//单选
    checkBox = 2,//复选框
    Input = 3,//输入
}

/// <summary>
/// 分享类型
/// </summary>
public enum eShareType
{
    screenshot=1,//截图
    game = 2,//分享游戏
}


//道具类型
public enum ePropType
{
    /// <summary>
    /// 金币
    /// </summary>
    golds = 1,
    /// <summary>
    /// 房卡
    /// </summary>
    cards = 2,
    /// <summary>
    /// 其他
    /// </summary>
    other = 3,
}

/// <summary>
/// 道具详细类型
/// </summary>



public enum LoginType : int {
    /// <summary>
    /// 没有任何登录
    /// </summary>
    Nil = 0,
    /// <summary>
    /// 输入账号密码登录
    /// </summary>
    Account = 1,
    /// <summary>
    /// 微信登录
    /// </summary>
    WeChat = 2,
    /// <summary>
    /// QQ登录
    /// </summary>
    QQ = 3,
}



#region 聊天
/// <summary>
public enum eChatTextDirectionType
{
    /// <summary>
    /// 聊天框指向左上
    /// </summary>
    LeftUP = 0,
    /// <summary>
    /// 聊天框指向右上
    /// </summary>
    LeftDown,
    /// <summary>
    /// 聊天框指向右上
    /// </summary>
    RightUp,
    /// <summary>
    /// 聊天框指向右下
    /// </summary>
    RightDown,
}

/// <summary>
///     /// <summary>
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
/// </summary>
public enum eGameChatContentType
{
    /// <summary>
    /// 表情
    /// </summary>
    Face = 1,
    /// <summary>
    /// 普通文字
    /// </summary>
    Chat = 2,
    /// <summary>
    /// 文字语音
    /// </summary>
    TexTVoice = 3,
    /// <summary>
    /// 互动表情
    /// </summary>
    HDFace = 4,
    /// <summary>
    /// 语音
    /// </summary>
    Voice = 5,
}
#endregion

#region 排行榜
/// <summary>
/// 排行榜类型
/// </summary>
public enum eRankType
{ 
    /// <summary>
    /// 富豪榜
    /// </summary>
    Rich = 1,
    /// <summary>
    /// 比赛榜
    /// </summary>
    Match = 2,
    /// <summary>
    /// 胜局榜
    /// </summary>
    Win = 3,
}

#endregion

#region 任务类型
public enum eTaskType
{
    Share = 1,//分享
    ShareRecord,//分享战绩
    PlayFriendModel,//玩好友房
    PlayMatchModel,//玩比赛场
}

#endregion

#region 俱乐部添加类型
public enum eClubAddType
{
    player,//玩家
    club,//俱乐部
}

#endregion
