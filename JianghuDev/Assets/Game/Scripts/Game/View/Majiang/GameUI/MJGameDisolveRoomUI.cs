using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJGameDisolveRoomUI : BaseViewWidget
{
    public GameObject mItem;
    public UIGrid mGrid;
    public UIButton mRefuseBtn;//拒绝按钮
    public UIButton mAgreeBtn;//同意按钮
    public UILabel mTimeLabel;//解散倒计时

    private const string mTimeCountDown = "解散倒计时：{0}秒";
    private float mTime;//倒计时


    public void SetData(MJDissolveProtoData data)
    {
        mTime = data.timeDown;
        List<MJExitList> list = data.applyExitList;
        int len = list.Count;
        int count = mGrid.transform.childCount;
        GameObject obj;
        int agreeCount = 0;
        bool isRefuse = true;
        int refusecount = 0;
        for (int i = 0; i < len; i++)
        {
            if (i < count)
                obj = mGrid.transform.GetChild(i).gameObject;
            else
                obj = NGUITools.AddChild(mGrid.gameObject, mItem);
            obj.SetActive(true);
            SetItem(list[i], obj.GetComponent<UILabel>());
            if (list[i].state >= 1)
                agreeCount++;
            if (list[i].state == 0)
                isRefuse = false;
            if (list[i].state == -1)
                refusecount++;
        }
        mGrid.Reposition();
        if (agreeCount > 2 || isRefuse || refusecount >= 2) //所有人都同意或者有两个以上的人拒绝就关闭
            Close<MJGameDisolveRoomUI>();

    }

    private void SetItem(MJExitList data, UILabel label)
    {
        int myseat = MJGameModel.Inst.mMySeatId;
        string name = MJGameModel.Inst.mRoomPlayers[data.seatId].nickName;
        label.text = name; //GameUtils.GetClampText(name, label);

        if (data.state == 2)
        {
            label.transform.GetChild(0).GetComponent<UILabel>().text = "申请解散房间";
            if (myseat == data.seatId)
            {
                mRefuseBtn.isEnabled = false;
                mAgreeBtn.isEnabled = false;
            }
        }
        else if (data.state == 1)
        {
            label.transform.GetChild(0).GetComponent<UILabel>().text = "同意解散房间";
            if (myseat == data.seatId)
            {
                mRefuseBtn.isEnabled = false;
                mAgreeBtn.isEnabled = false;
            }
        }
        else if (data.state == -1)
        {
            label.transform.GetChild(0).GetComponent<UILabel>().text = "拒绝解散房间";
            if (myseat == data.seatId)
            {
                mRefuseBtn.isEnabled = false;
                mAgreeBtn.isEnabled = false;
            }
        }
        else if (data.state == 0)
        {
            label.transform.GetChild(0).GetComponent<UILabel>().text = "等待中...";
            if (myseat == data.seatId)
            {
                mRefuseBtn.isEnabled = true;
                mAgreeBtn.isEnabled = true;
            }
        }
    }

    private void Update()
    {
        if (mTime >= 0)
        {
            mTimeLabel.text = string.Format(mTimeCountDown, (int)mTime);
            mTime -= Time.deltaTime;
        }
        else
        {
            Close<MJGameDisolveRoomUI>();
        }
    }


    /// <summary>
    /// 拒绝按钮点击
    /// </summary>
    public void OnRefuseClick()
    {
        Debug.Log("拒绝解散");
        Global.Inst.GetController<MJGameController>().SendIsAgreeDissoveRoom(-1, () =>
        {
            mAgreeBtn.isEnabled = false;
            mRefuseBtn.isEnabled = false;
        });
    }


    /// <summary>
    /// 同意按钮点击
    /// </summary>
    public void OnAgreeClick()
    {
        Debug.Log("同意解散");
        Global.Inst.GetController<MJGameController>().SendIsAgreeDissoveRoom(1, () =>
        {
            mAgreeBtn.isEnabled = false;
            mRefuseBtn.isEnabled = false;
        });
    }
}
