using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJGameBackModel : BaseModel
{
    public static MJGameBackModel Inst;

    public override void SetController(BaseController c)
    {
        base.SetController(c);
        Inst = this;
    }

    /// <summary>
    /// 指令
    /// </summary>
    public MJoptInfoData[] mOptList;
    /// <summary>
    /// 总共玩的局数（有战报的局数）
    /// </summary>
    public int mTottleCount;
    /// <summary>
    /// 当前播放到第几局（从1开始）
    /// </summary>
    public int mCurIndex;
    /// <summary>
    /// 房间信息
    /// </summary>
    public MJRecordListProto.MJScordListItem mRoomData;
    /// <summary>
    /// 当前数据
    /// </summary>
    public MJScordDetailInfo mDetailInfo;


    public List<MJRecordItemData> RecordAllData;//回放数据

    public Dictionary<string, byte[]> RecordDetailsDic = new Dictionary<string, byte[]>();//回放详细信息

    public MJRecordData CurRecordDetailData;//当前回放信息
}
