using UnityEngine;
using System.Collections;


public enum eEvent : int {

    /// <summary>
    /// 金币更新
    /// </summary>
    UPDATE_GOLDS,
    /// <summary>
    /// 房卡更新
    /// </summary>
    UPDARE_ROOM_CARD,
    /// <summary>
    /// 道具更新
    /// </summary>
    UPDATE_PROP,

}

/// <summary>
/// 游戏中更多按钮点出来的
/// </summary>
public enum eGameMore {
    Setting = 1,
    Distance = 2,
    Leave = 3
}
