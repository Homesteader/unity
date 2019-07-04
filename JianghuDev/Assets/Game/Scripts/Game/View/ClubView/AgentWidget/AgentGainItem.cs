using UnityEngine;
using System.Collections;

public class AgentGainItem : BaseViewWidget
{
    /// <summary>
    /// 头像
    /// </summary>
    public UITexture mHead;
    /// <summary>
    /// 昵称
    /// </summary>
    public UILabel mNickNameLabel;
    /// <summary>
    /// uid
    /// </summary>
    public UILabel mUIDLabel;

    public UILabel mPeachNum;//金币
    /// <summary>
    /// 一级代理的抽成
    /// </summary>
    public UILabel mGainLabel;

    /// <summary>
    /// 二级代理
    /// </summary>
    public GameObject mTwoGo;

    /// <summary>
    /// 二级代理人数
    /// </summary>
    public UILabel mTwoPeopleNumLabel;
    /// <summary>
    /// 二级代理抽成
    /// </summary>
    public UILabel mTwoAgainCutNum;

    private Transform mRoot;//root节点

    private SendGetAgentWinList mData;
    private AgentGainWidget mRootView;//父对象

    public void InitUI(SendGetAgentWinList data, Transform root, AgentGainWidget view)
    {
        mRootView = view;
        mData = data;
        mRoot = root;
        //头像
        Assets.LoadIcon(data.headUrl, (t) => { mHead.mainTexture = t; });
        //昵称
        mNickNameLabel.text = data.nickname;
        //id
        mUIDLabel.text = "ID：" + data.userId;
        //总桃数
        mPeachNum.text = (data.selfPeach + data.warehousePeach).ToString();
        //下级代理抽成
        mGainLabel.text = data.oneBenefit.ToString();
        //下下级代理
        if (data.two != null)
        {
            mTwoGo.gameObject.SetActive(true);
            //下下级代理人数
            mTwoPeopleNumLabel.text = data.two.agentTotal.ToString();
            //下下级代理抽成
            mTwoAgainCutNum.text = data.two.sum.ToString();

        }
        else
        {
            mTwoGo.gameObject.SetActive(false);
        }

    }


    public void OnItemClick()
    {
        ClubBorrowWidget view = GetWidget<ClubBorrowWidget>("Windows/ClubView/ClubBorrowWidget", mRoot);
        view.SetData(mData.userId);
        view.SetCallBack((t) =>
        {
            if (mRootView != null)
                mRootView.RefreshWare();
        });
    }

}
