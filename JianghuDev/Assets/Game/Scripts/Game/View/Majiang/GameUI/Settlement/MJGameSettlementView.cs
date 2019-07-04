using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJGameSettlementView : BaseViewWidget
{
    public UIButton mContinueBtn;//继续按钮
    public UIButton mBackBtn;//返回按钮
    public UIButton mShareBtn;//分享按钮
    public GameObject mCloseBtn;//关闭按钮
    public GameObject mItem;//item
    public UIGrid mGrid;//gridTran
    public UILabel mContinueLabel;//继续按钮文字

    private MJGameSettlementInfo mInfo;
    private CallBack mBackClickCall;//返回点击回调

   

    public void SetData(MJGameSettlementInfo info, int maxFanshu, bool isshowBtn = true)
    {

        mInfo = info;
        mContinueBtn.gameObject.SetActive(isshowBtn);
        //mBackBtn.gameObject.SetActive(false);
        mShareBtn.gameObject.SetActive(isshowBtn);
        if (info.settleContainer != null && info.settleContainer != null)
        {
            int len = info.settleContainer.Count;
            int count = mGrid.transform.childCount;
            GameObject obj;
            float maxPoint = -10000000f;
            for (int i = 0; i < len; i++)
            {
                if (info.settleContainer[i].score > maxPoint)
                    maxPoint = info.settleContainer[i].score;
            }
            bool isBigWinner = false;//是否是大赢家
            
            for (int i = 0; i < len; i++)
            {
                if (i < count)
                    obj = mGrid.transform.GetChild(i).gameObject;
                else
                    obj = NGUITools.AddChild(mGrid.gameObject, mItem);
                obj.SetActive(true);
                isBigWinner = maxPoint == info.settleContainer[i].score && maxPoint > 0 ? true : false;
                obj.GetComponent<MJGameSettlementItem>().SetData(info.settleContainer[i], 0, isBigWinner);
            }
            mGrid.Reposition();
        }
    }

    /// <summary>
    /// 设置继续按钮倒计时
    /// </summary>
    /// <param name="time"></param>
    public void SetContinueLabel(string time)
    {
        mContinueLabel.text = "继 续(" + time + "s)";
    }

    /// <summary>
    /// 设置返回按钮显示
    /// </summary>
    /// <param name="call"></param>
    public void SetBackBtnShow(CallBack call)
    {
        mBackClickCall = call;
        mContinueBtn.gameObject.SetActive(false);
        mBackBtn.gameObject.SetActive(true);
        mCloseBtn.SetActive(false);
    }

    /// <summary>
    /// 分享
    /// </summary>
    public void OnShareClick()
    {
        Global.Inst.GetController<ShareController>().SetCaptureScreenshot();
    }

    /// <summary>
    /// 继续
    /// </summary>
    public void OnContinueClick()
    {
        MJGameController mMJctr = Global.Inst.GetController<MJGameController>();
        MJGameModel.Inst.ResetData();
        if (mInfo == null)//解散过后
        {
            mMJctr.ConnectedToHallServer(null);
        }
        else
        {
            MJGameSettlementFinalInfo info = MJGameModel.Inst.mFinalSettlementInfo;
            if (info != null)
            {
                if (info != null)
                {
                    mMJctr.mGameUI.ServerSettlementFinal(info);
                    CloseWidget<MJGameSettlementView>();
                    //gameObject.SetActive(false);
                    return;
                }
                else
                {
                    Global.Inst.GetController<MJGameController>().ConnectedToHallServer(null);
                }
            }
            else
            {
                //准备
                mMJctr.mGameUI.SetSelfPreShow(false);
                OptRequest req = new OptRequest();
                req.ins = eMJInstructionsType.READY;
                Global.Inst.GetController<MJGameController>().SendOptRequest(req);
                Close();
            }
        }
    }

    /// <summary>
    /// 返回
    /// </summary>
    public void OnBackClick()
    {
        if (mBackClickCall != null)
            mBackClickCall();
    }

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick()
    {
        OptRequest req = new OptRequest();
        req.ins = eMJInstructionsType.EXITROOM;
        Global.Inst.GetController<MJGameController>().SendInstructions(req, null);
    }
}
