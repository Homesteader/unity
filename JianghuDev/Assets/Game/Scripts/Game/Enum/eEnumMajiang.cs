using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum eMJRoomStatus
{
    READY = 0,     //准备
    STARTE = 1,    //游戏中
    GAMEOVER = 2,  //游戏结束
    FAPAI = 3,    //发牌
    JIESUAN = 4,   //结算
    DENGDAI = 5,
}

public enum eMJGameSubType//子类型
{
    xuezhan = 1,//血战
    sanrenliangfang = 2,//三人两房
}

public enum eMJOnlineState//在线状态
{
    none = 0,//在线
    leave = 1,//离开
    online = 2,//在线
    outline = 3,//离线
    giveUp = 4,//认输

}

public enum wMJSexType
{
    /// <summary>
    /// 男
    /// </summary>
    Man = 1,
    /// <summary>
    /// 女
    /// </summary>
    Woman = 2,
}

/// <summary>
/// 聊天类型
/// </summary>
public enum eMJChatCotentType
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

public enum eMJGangType
{
    /// <summary>
    /// 弯杠
    /// </summary>
    WANGANG = 1,
    /// <summary>
    /// 暗杠
    /// </summary>
    ANGANG = 2,
    /// <summary>
    /// 点杠
    /// </summary>
    DIANGANG = 3,
    /// <summary>
    /// 抢点杠
    /// </summary>
    QIANGDIANGANG = 4,
    /// <summary>
    /// 明杠中发白
    /// </summary>
    MingGangZFB = 5,
    /// <summary>
    /// 暗杠中发白
    /// </summary>
    AnGangZFB = 6,
}
/// <summary>
/// 胡牌子类型
/// </summary>
public enum eMJSubType
{
    WU = 1,        //没有
    QIANGGANG = 2, //抢杠胡
    GANGPAO = 3,   //杠上炮
    GANGHUA = 4,   //杠上花
}

public enum eMJChiType
{
    /// <summary>
    /// 前
    /// </summary>
    QIAN = 1,
    /// <summary>
    /// 后
    /// </summary>
    HOU = 2,
    /// <summary>
    /// 中
    /// </summary>
    ZHONG = 3,
}

public enum eMJInstructionsType
{
    READY = 1,// --准备
    CHANGETHREE = 2,//--换三张
    FIXEDCOLOR = 3, //--定缺
    HU = 4,  //--胡牌
    GANG = 5,   // --杠牌
    PENG = 6,// --碰牌
    GUO = 7, //--过牌
    TG = 8,  //--托管
    MO = 9,  //--摸牌
    ALLCHANGE = 10,// --所有玩家都选择了换牌
    ALLFIXED = 11, //--所有玩家都选了定缺
    HIT = 12,//打牌
    YAOPAI = 13, //--要牌
    YPDX = 14,// --一炮多响
    SCORE = 15, //--分数同步的序号
    CHATING = 16,// --查听
    EXITROOM = 17,//退出房间 

    CHI = 20,
    LIANG = 21,



}

/// <summary>
/// 托管类型
/// </summary>
public enum eMJTrusteeshipType
{
    trust = 1,//托管
    cancelTrust = 2,//取消托管
}

//**************************************************新枚举**********************************//////////////////////////
//查花猪,查叫,赔雨
public enum eEff
{
    NONE = 0,
    HJZY = 1,
    HUAZHU = 2,//花猪
    CHAJIAO = 3,//查叫
    PEIYU = 4//赔雨
}




// --房间的流程
/// <summary>
/// 房间状态    准备   换3张  定缺  游戏中 游戏结束 
/// </summary>
public enum eRoomState
{
    READY = 0,//准备状态
    CHANGETHREE = 1,//--换三张
    FIXEDCOLOR = 2,//--定缺
    START = 3,//--游戏进行
    GAMEOVER = 4,//--游戏结束
}

//--定缺的类型
/// <summary>
/// 定缺类型     筒 条 万
/// </summary>
public enum eFixedType
{
    NONE = 0,
    WAN = 1,
    TIAO = 2,
    TONG = 3,
}

//--换三张的类型
/// <summary>
/// 换3张类型    顺  逆   对家  
/// </summary>
public enum eChangType
{
    SSZ = 1,//--顺时针交换
    NSZ = 2,//,--逆时针交换
    DJ = 3,// --对家交换
}

//--操作类型
public enum eMJInstructionsType_old
{
    READY = 1,// --准备
    CHANGETHREE = 2,//--换三张
    FIXEDCOLOR = 3, //--定缺
    HU = 4,  //--胡牌
    GANG = 5,   // --杠牌
    PENG = 6,// --碰牌
    GUO = 7, //--过牌
    TG = 8,  //--托管
    MO = 9,  //--摸牌
    ALLCHANGE = 10,// --所有玩家都选择了换牌
    ALLFIXED = 11, //--所有玩家都选了定缺
    HIT = 12//打牌
}

//--胡的类型
public enum eHuType
{
    NONE = 0,
    ZIMO = 1,
    DIANPAO = 2,
}
//--胡的二级类型
public enum eHuSubType
{
    NONE = 0,
    DGH = 1,//--点杠花
    GSH = 2,//--杠上花
    GSP = 3,//--杠上炮
    PING = 4,//--平胡
    QGH = 5, //--抢杠胡
}
//--胡的三级类型
public enum eHuThrType
{
    NONE = 0,//--没有
    TIANHU = 1,//--天胡
    DIHU = 2,//--地胡
}

//--摸牌的类型
public enum eMoType
{
    STARTE = 1,//--开始游戏的摸牌
    SHUN = 2, //--顺序摸牌
    DIANGANG = 3,// --点杠后摸牌
    ZIMOGANG = 4, //-- 自摸杠后摸牌
}

//--杠的类型
public enum eGangType
{
    ANGANG = 1, //--暗杠
    WANGANG = 2, //--弯杠
    DIANGANG = 3, //--点杠
}
//--打牌的类型
public enum eHitType
{
    SHUN = 1,// --顺序打的
    GANG = 2, //--杠牌的时候打的
}
//--打牌的二级类型
public enum eHitSubType
{
    NONE = 0,
    HAIDI = 1,
}
/// <summary>
/// 牌的类型   筒条万
/// </summary>
public enum eCardType
{
    WAN = 1,
    TIAO = 2,
    TONG = 3,
}
//分数类型
public enum ScoreType
{
    NONE = 0,
    HJZY = 1,
}
/// <summary>
/// 色子
/// </summary>
public enum eTouZi
{
    ONE = 1, TWO = 2, THREE = 3, FOUR = 4, FIVE = 5, SIX = 6
}

