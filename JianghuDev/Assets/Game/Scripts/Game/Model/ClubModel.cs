using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClubModel : BaseModel
{

    public static ClubModel Inst;

    public override void SetController(BaseController c)
    {
        base.SetController(c);
        if (Inst == null)
            Inst = this;
    }


    /// <summary>
    /// 当前俱乐部的信息
    /// </summary>
    public ClubInfo mClubData;

    /// <summary>
    /// 当前俱乐部id
    /// </summary>
    public string mClubId;

    /// <summary>
    /// 玩家数据
    /// </summary>
    public List<ClubPlayerInfo> ClubPlayerInfoData;
    /// <summary>
    /// 联盟数据
    /// </summary>
    public List<SendGetClubConpanyInfo> ClubUnionInfoData;

    /// <summary>
    /// 所有玩家金币
    /// </summary>
    public float AllPlayerPeach;//

    /// <summary>
    /// 局数排行榜类型
    /// </summary>
    public string mRoundType;

    /// <summary>
    /// 局数排行榜
    /// </summary>
    public List<SendGetRankForGameRoundInfo> mRoundRank = new List<SendGetRankForGameRoundInfo>();

    /// <summary>
    /// 移除一个玩家
    /// </summary>
    /// <param name="uid"></param>
    public void RemovePlayer(string uid) {
        for (int i=0;i< ClubPlayerInfoData.Count;i++) {
            if (ClubPlayerInfoData[i].userId == uid) {
                ClubPlayerInfoData.Remove(ClubPlayerInfoData[i]);
                return;
            }
        }
    }

    /// <summary>
    /// 移除一个玩家
    /// </summary>
    /// <param name="uid"></param>
    public void RemoveCompany(string uid)
    {
        for (int i = 0; i < ClubUnionInfoData.Count; i++)
        {
            if (ClubUnionInfoData[i].cludId == uid)
            {
                ClubUnionInfoData.Remove(ClubUnionInfoData[i]);
                return;
            }
        }
    }
}
