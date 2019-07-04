using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomView : BaseView {

    public UILabel[] mIdLabels;//roomid显示

    private string mRoomId = "";//房间号

    protected override void Awake()
    {
        base.Awake();
        RefreshLabels();
    }

    #region 按钮点击
    //数字点击
    public void OnNumClick(GameObject go)
    {
        if (mRoomId.Length >= 6)
        {
            return;
        }
        int len = mRoomId.Length;
        mRoomId += go.name;
        RefreshLabels();
        if(mRoomId.Length >= 6)
        {
            Global.Inst.GetController<XXGoldFlowerGameController>().SendJoinRoomReq(mRoomId);
        }
    }

    //重输点击
    public void OnResetClick()
    {
        mRoomId = "";
        RefreshLabels();
    }

    //删除点击
    public void OnDeleteClick()
    {
        if (mRoomId.Length <= 0)
            return;
        int len = mRoomId.Length;
        mRoomId = mRoomId.Remove(len - 1);
        RefreshLabels();
    }
    #endregion
    //刷新房间号
    private void RefreshLabels()
    {
        //显示已有的位数
        for(int i = 0; i < mRoomId.Length; i++)
        {
            mIdLabels[i].text = mRoomId[i].ToString();
        }
        //没有的置空
        for(int i = mRoomId.Length; i < 6; i++)
        {
            mIdLabels[i].text = "";
        }
    }

}
