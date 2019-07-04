using UnityEngine;
using System.Collections;

//方向类型
public enum eDirectionType
{
    x,//x轴
    y,//y轴
    z,//z轴
}


public enum eNNGameState {
    /// <summary>
    /// 准备
    /// </summary>
    Ready = 1,
    /// <summary>
    /// 下注
    /// </summary>
    XiaZhu = 2,
    /// <summary>
    /// 看牌
    /// </summary>
    LookCard = 3,
    /// <summary>
    /// 抢庄
    /// </summary>
    QiangZhuang =4,
}

/// <summary>
/// 牌型
/// </summary>
public enum eNNCardsType {
    /// <summary>
    /// 无牛
    /// </summary>
    Nil = 0,
    /// <summary>
    /// 牛1
    /// </summary>
    One = 1,
    /// <summary>
    /// 牛2
    /// </summary>
    Two = 2,
    /// <summary>
    /// 牛三
    /// </summary>
    Three = 3,
    /// <summary>
    /// 牛四
    /// </summary>
    Four = 4,
    /// <summary>
    /// 牛五
    /// </summary>
    Five = 5,
    /// <summary>
    /// 牛六
    /// </summary>
    Six = 6,
    /// <summary>
    /// 牛七
    /// </summary>
    Seven = 7,
    /// <summary>
    /// 牛八
    /// </summary>
    Eight = 8,
    /// <summary>
    /// 牛九
    /// </summary>
    Nine = 9,
    /// <summary>
    /// 牛10
    /// </summary>
    Twen = 10,
    /// <summary>
    /// 五花牛
    /// </summary>
    WHN = 11,
    /// <summary>
    /// 炸弹牛
    /// </summary>
    ZDN = 12,
    /// <summary>
    /// 五小牛
    /// </summary>
    WXN = 13,
}

/// <summary>
/// 操作 1下注 2亮牌 3是抢庄
/// </summary>
public enum eNNOpt {
    /// <summary>
    /// 同步房间状态
    /// </summary>
    Nil = 0,
    /// <summary>
    /// 下注
    /// </summary>
    XZ = 1,
    /// <summary>
    /// 亮牌
    /// </summary>
    LP = 2,
    /// <summary>
    /// 抢庄
    /// </summary>
    QZ = 3
}

public enum eNNSubGameType {
    /// <summary>
    /// 明牌抢庄
    /// </summary>
    MingPai = 1,
    /// <summary>
    /// 普通
    /// </summary>
    NorMal = 2,
}