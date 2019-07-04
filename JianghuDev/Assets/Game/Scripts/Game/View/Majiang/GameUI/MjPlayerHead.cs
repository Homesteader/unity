using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MjPlayerHead : BaseViewWidget
{
    public GameObject mHeadRoot;//头像信息
    public UITexture mIcon;//头像
    public UILabel mName;//名字
    public UILabel mPoint;//金币
    public GameObject mPreObj;//准备
    public GameObject mScoreRoot;//分数特效位置
    public GameObject mScoreAddItem;//分数加
    public GameObject mScoreSubstract;//分数减
    public UISprite mCountDownSpr;//倒计时cd
    

    private eMJRoomStatus mState;
    private MJplayerInfo mPlayerInfo;
    private Vector3[] mReadyPos = new Vector3[] { new Vector3(0, -444, 0), new Vector3(139.3f, -92.16f, 0), new Vector3(14.2f, 18, 0), new Vector3(-150, -95, 0) };//准备时候的位置
    private Vector3[] mStartPos = new Vector3[] { new Vector3(-787.2f, -284.68f, 0), new Vector3(164.5f, 49.48f, 0), new Vector3(479.6f, 25.9f, 0), new Vector3(-178.5f, 88.9f, 0) };//开始过后的位置

    /// <summary>
    /// 设置头像显示
    /// </summary>
    /// <param name="info"></param>
    public void SetData(MJplayerInfo info, eMJRoomStatus state)
    {
        mState = state;
        mPlayerInfo = info;
        if (info == null)//没有这个玩家
        {
            mName.text = "";
            mPoint.text = "";
            mIcon.mainTexture = null;
            mPreObj.SetActive(false);
        }
        else
        {
            mHeadRoot.SetActive(true);
            mName.text = info.nickName;//GameUtils.GetClampText(info.nickName, mName);
            mPoint.text = info.gold.ToString("#0.00");
            Assets.LoadIcon(info.headUrl, (t) =>
            {
                mIcon.mainTexture = t;
            });
            int index = MJGameModel.Inst.mSeatToIndex[info.seatId] + 1;
            if (info.seatId == MJGameModel.Inst.mMySeatId)
                index = 0;
            if (state == eMJRoomStatus.READY)
            {
                mPreObj.SetActive(info.isReady);
                mHeadRoot.transform.localPosition = mReadyPos[index];
            }
            else
            {
                mPreObj.SetActive(false);
                mHeadRoot.transform.localPosition = mStartPos[index];
            }
        }
    }

    /// <summary>
    /// 设置金币
    /// </summary>
    /// <param name="point"></param>
    public void SetPoint(float point, float addScore = 0, int type = 0)
    {
        mPoint.text = point.ToString("#0.00");
    }
    /// <summary>
    /// 设置分数特效
    /// </summary>
    /// <param name="type"></param>
    public void SetReScoreEff(float point)
    {
        GameObject obj;
        if (point >= 0)
            obj = NGUITools.AddChild(mScoreRoot, mScoreAddItem);
        else
            obj = NGUITools.AddChild(mScoreRoot, mScoreSubstract);
        UILabel label = obj.GetComponent<UILabel>();
        label.text = point >= 0 ? "+" + point : point.ToString();
        obj.SetActive(true);
        GameObject.Destroy(obj, 1.5f);//3秒后销毁
    }

    /// <summary>
    /// 设置cd
    /// </summary>
    /// <param name="f"></param>
    public void SetCD(float f)
    {
        mCountDownSpr.fillAmount = f;
        UISprite[] sps = mCountDownSpr.GetComponentsInChildren<UISprite>();
        if (sps!=null) {
            for (int i=0;i<sps.Length;i++) {
                sps[i].fillAmount = f;
            }
        }
    }

    /// <summary>
    /// 设置开始位置
    /// </summary>
    public void SetStart()
    {
        if (mPlayerInfo == null)//没有这个玩家
        {
            mName.text = "";
            mPoint.text = "";
            mIcon.mainTexture = null;
            mPreObj.SetActive(false);
        }
        else
        {
            int index = MJGameModel.Inst.mSeatToIndex[mPlayerInfo.seatId] + 1;
            if (mPlayerInfo.seatId == MJGameModel.Inst.mMySeatId)
                index = 0;
            mState = eMJRoomStatus.STARTE;
            mPreObj.SetActive(false);
            mHeadRoot.transform.localPosition = mStartPos[index];
        }
    }

    /// <summary>
    /// 设置准备
    /// </summary>
    /// <param name="isReady"></param>
    public void SetReady(bool isReady)
    {
        mPreObj.SetActive(isReady);
    }

    #region 按钮点击
    /// <summary>
    /// 头像点击
    /// </summary>
    public void OnSelfClick()
    {
        bool isself = mRoomPlayerInfo.seatId == MJGameModel.Inst.mMySeatId;//MainPlayerModel.GetInstance().mUserInfo.userId;//是否是自己
        if (isself)
            return;
        MJGameUI view = Global.Inst.GetController<MJGameController>().mGameUI;
        if (view == null)
            return;
        GameUserInfoWidget infoview = GetWidget<GameUserInfoWidget>("Windows/GameCommonView/GameUserInfoWidget", view.transform);
        infoview.SetData(true, mRoomPlayerInfo.headUrl, mRoomPlayerInfo.nickName, mRoomPlayerInfo.uId, "", mRoomPlayerInfo.seatId, OnHudongClickCallback);
    }

    /// <summary>
    /// 添加好友按钮点击
    /// </summary>
    public void OnAddFriendClick()
    {
        //if (mPlayerInfo != null)
        //ZhidianSDKManaerger.Inst.SendMsg(ZhidianSDKManaerger.ADD_FRIEND, mPlayerInfo.userId);
    }
    #endregion

    /// <summary>
    /// 互动表情点击回调
    /// </summary>
    /// <param name="id"></param>
    private void OnHudongClickCallback(int id)
    {
        MJGameController mCtr = Global.Inst.GetController<MJGameController>();
        SendReceiveGameChat req = new SendReceiveGameChat();
        req.chatType = (int)eGameChatContentType.HDFace;
        req.faceIndex = id;
        req.fromSeatId = MJGameModel.Inst.mMySeatId;
        req.toSeatId = mRoomPlayerInfo.seatId;
        mCtr.SendGameChat(req);
    }




    private PlayerInfoStruct mRoomPlayerInfo; //玩家数据

    /// <summary>
    /// 设置玩家头像 数据
    /// </summary>
    public void SetPlayerIcon(PlayerInfoStruct info)
    {
        mRoomPlayerInfo = info;
        if (info == null) //没有这个玩家
        {
            mName.text = "";
            mPoint.text = "";
            mIcon.mainTexture = null;
            mPreObj.SetActive(false);
        }
        else
        {
            mHeadRoot.SetActive(true);
            //mName.text = GameUtils.GetClampText(info.nickName, mName);
            mName.text = info.nickName;
            mPoint.text = info.gold.ToString("#0.00");
            Assets.LoadIcon(info.headUrl, (t) =>
            {
                mIcon.mainTexture = t;
            });
        }
    }


}