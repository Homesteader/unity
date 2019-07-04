using UnityEngine;
using System.Collections;
using System;

public class XXGoldFlowerPkCardWidget : BaseViewWidget {

    public UITexture mLeftBg;

    public UITexture mRightBg;

    public GameObject mLeftPlayer;
    public UITexture mLeftHead;
    public UILabel mLeftNickName;
    public UILabel mLeftUID;
    public UISprite mLeftWinLostSp;
    public UITexture mLeftWinLoseTex;


    public GameObject mPKAnimation;

    public GameObject mRightPlayer;
    public UITexture mRightHead;
    public UILabel mRightNickName;
    public UILabel mRightUID;
    public UISprite mRightWinLostSp;
    public UITexture mRightWinLoseTex;


    /// <summary>
    /// 如果是左边玩家赢，为true
    /// </summary>
    private bool mLeftWin = false;
    /// <summary>
    /// 动画结束回调
    /// </summary>
    private Action mCallBack;

    /// <summary>
    /// 初始化玩家信息
    /// </summary>
    /// <param name="leftHead"></param>
    /// <param name="leftName"></param>
    /// <param name="leftUid"></param>
    /// <param name="rightHead"></param>
    /// <param name="rightName"></param>
    /// <param name="rightUid"></param>
    public void SetPlayers(string leftHead,string leftName,string leftUid,string rightHead,string rightName,string rightUid) {
        Assets.LoadIcon(leftHead, (t) => { mLeftHead.mainTexture = t; });
        mLeftNickName.text =""+ leftName;
        mLeftUID.text = "UID：" + leftUid;

        Assets.LoadIcon(rightHead, (t) => { mRightHead.mainTexture = t; });
        mRightNickName.text = "" + rightName;
        mRightUID.text = "UID：" + rightUid;
    }

    /// <summary>
    /// 开始pk，如果是左边玩家赢，带入参数为true
    /// </summary>
    /// <param name="leftWin"></param>
    public void StartPk(bool leftWin,Action callBack) {
        mCallBack = callBack;
        mLeftWin = leftWin;
        mLeftPlayer.GetComponent<TweenPosition>().PlayForward();
        mRightPlayer.GetComponent<TweenPosition>().PlayForward();
    }


    public void OnCardMoveFinished() {
        mPKAnimation.gameObject.SetActive(true);
        StartCoroutine(PkLose());
    }


    private IEnumerator PkLose() {
        yield return new WaitForSeconds(.5f);
        mPKAnimation.GetComponent<TweenScale>().enabled = false;
        mLeftWinLostSp.gameObject.SetActive(true);
        mRightWinLostSp.gameObject.SetActive(true);
        if (mLeftWin)
        {//左边玩家赢
            mLeftWinLostSp.spriteName = "win_state";
            mRightWinLostSp.spriteName = "lose_state";

            mRightBg.color = Color.black;
        }
        else {
            mLeftWinLostSp.spriteName = "lose_state";
            mRightWinLostSp.spriteName = "win_state";

            mLeftBg.color = Color.black;
        }

        yield return new WaitForSeconds(1.5f);
        if (mCallBack!=null) {
            mCallBack();
        }
        Close<XXGoldFlowerPkCardWidget>();
    }

}
