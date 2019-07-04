using UnityEngine;
using System.Collections;
using System;

public class MJRecordItem : BaseViewWidget
{
    public UILabel mType;//游戏类型
    public UILabel mTime;//时间
    public UIGrid mGrid;//gridTran;
    public GameObject mPlayerItem;//玩家item

    private MJRecordItemData mData;//数据

    #region 初始化
    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="data"></param>
    public void SetData(MJRecordItemData data)
    {
        mData = data;
        //类型
        mType.text = data.roomName;
        //时间
        mTime.text = data.createTime.Replace(' ', '\n');
        //初始化玩家
        InitPlayers(data.usersInfo);
    }

    /// <summary>
    /// 初始化玩家
    /// </summary>
    /// <param name="usersInfo"></param>
    private void InitPlayers(MJRecordPlayerData[] usersInfo)
    {
        GameObject obj;
        Transform grid = mGrid.transform;
        int len = usersInfo == null ? 0 : usersInfo.Length;
        int gridChildCount = grid.childCount;
        for (int i = 0; i < len; i++)
        {
            if (i < gridChildCount)
                obj = grid.GetChild(i).gameObject;
            else
                obj = NGUITools.AddChild(grid.gameObject, mPlayerItem);
            obj.SetActive(true);
            obj.GetComponent<MJRecordPlayer>().SetData(usersInfo[i]);
        }
        //隐藏多余的
        for (int i = len; i < gridChildCount; i++)
            grid.GetChild(i).gameObject.SetActive(false);
        mGrid.Reposition();
    }
    #endregion

    #region 按钮点击
    /// <summary>
    /// 回放按钮点击
    /// </summary>
    public void OnReplayClick()
    {
        Global.Inst.GetController<MJGameBackController>().GetRecord(mData.fileUrl2, (d) =>
        {
            Global.Inst.GetController<MJGameBackController>().ShowRecord();
        });
    }
    #endregion
}
