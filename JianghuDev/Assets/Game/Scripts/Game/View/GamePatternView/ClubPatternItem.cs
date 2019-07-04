using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClubPatternItem : MonoBehaviour
{

    public UILabel mTitle;

    public UISprite mState;
   
    public UISprite mStateBG;

    public UILabel mBaseScoreLabel;//底分
    public UILabel mBaseScoreLabel2;//底分小数

    public UILabel mIntoLabel;

    public GameObject mThreeSp;

    public UIScrollView mScrollView;

    public GameObject mHeandItem;

    public UIGrid mGrid;

    private SendGetRoomListInfo mData;

    /// <summary>
    /// 初始化UI
    /// </summary>
    /// <param name="heads"></param>
    /// <param name="rules"></param>
    public void InitUI(SendGetRoomListInfo data)
    {
        mData = data;
        if (data.userInfo != null)
        {
            NGUITools.DestroyChildren(mGrid.transform);
            for (int i = 0; i < data.userInfo.Count; i++)
            {
                GameObject item = Assets.InstantiateChild(mGrid.gameObject, mHeandItem, mScrollView);
                item.gameObject.SetActive(true);
                Assets.LoadIcon(data.userInfo[i].headUrl, (t) =>
                {
                    item.GetComponentInChildren<UITexture>().mainTexture = t;
                });
            }
        }
        mTitle.text = data.name;
        mState.spriteName = data.gameStatus == 1 ? "waiting" : "playing";
        mStateBG.spriteName = data.gameStatus == 1 ? "gamestate_bg1" : "gamestate_bg";
        string str = data.rule.baseScore.ToString("f2");
        string[] str1 = str.Split('.');
        if (str1.Length == 2)
        {
            mBaseScoreLabel.text = str1[0];
            mBaseScoreLabel2.text = "." + str1[1];
        }
        else
        {
            mBaseScoreLabel.text = str;
            mBaseScoreLabel2.text = "";
        }

        mIntoLabel.text = data.rule.into.ToString();
        if (GamePatternModel.Inst.mCurGameId == eGameType.MaJiang && data.rule.ruleIndexs.Contains(6))
        {
            mThreeSp.gameObject.SetActive(true);
        }
        else
        {
            mThreeSp.gameObject.SetActive(false);
        }
        mGrid.Reposition();
    }

    /// <summary>
    /// 点击事件
    /// </summary>
    public void OnItemClick()
    {
        string rule = GamePatternModel.Inst.DeserializeRuleJosn(mData.rule, true);
        GameRoomInfoWidget mRoomInfoWidget = BaseView.GetWidget<GameRoomInfoWidget>(AssetsPathDic.GameRoomInfoWidget, Global.Inst.GetController<GamePatternController>().mView.transform);
        mRoomInfoWidget.ShowContent(rule);
        mRoomInfoWidget.ShowJoinRoomBtn(mData.roomId);
    }
}
