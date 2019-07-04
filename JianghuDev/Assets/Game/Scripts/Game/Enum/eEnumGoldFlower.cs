using UnityEngine;
using System.Collections;

/// <summary>
/// 金花的操作指令 1是底注 2是全压 3是跟注 4是加注 5弃牌 6看牌 7是孤注一掷 8比牌
/// </summary>
public enum eGFOptIns {
    /// <summary>
    /// 底注
    /// </summary>
    DiZhu = 1,
    /// <summary>
    /// 全压
    /// </summary>
    AllIn = 2,
    /// <summary>
    /// 跟注
    /// </summary>
    Follow = 3,
    /// <summary>
    /// 加注
    /// </summary>
    Add = 4,
    /// <summary>
    /// 弃牌
    /// </summary>
    DisCard = 5,
    /// <summary>
    /// 看牌
    /// </summary>
    LookCard = 6,
    /// <summary>
    /// 孤注一掷
    /// </summary>
    JustFuck = 7,
    /// <summary>
    /// 比牌
    /// </summary>
    Compare = 8,
}

/// <summary>
/// 金花的牌型 1是高牌 2是对子 3是顺子 4是同花  5是顺清  6是飞机 
/// </summary>
public enum eGFCardType {
    Nil = 0,
    /// <summary>
    /// 高牌
    /// </summary>
    GaoPai = 1,
    /// <summary>
    /// 对子
    /// </summary>
    Paire = 2,
    /// <summary>
    /// 顺子
    /// </summary>
    Shun = 3,
    /// <summary>
    /// 同花
    /// </summary>
    SameColor = 4,
    /// <summary>
    /// 顺清
    /// </summary>
    ShunQing = 5,
    /// <summary>
    /// 飞机
    /// </summary>
    Boom = 6,
}


public enum eGFGameState {
    /// <summary>
    /// 准备
    /// </summary>
    Ready = 1,
    /// <summary>
    /// 开始
    /// </summary>
    Start = 2,
}


public enum eGFRoomState {
    /// <summary>
    /// 开始过
    /// </summary>
    NoStart = 0,
    /// <summary>
    /// 没有开始过
    /// </summary>
    Started = 1,
}