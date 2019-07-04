using UnityEngine;
using System.Collections;

public class GoldPatternItem : MonoBehaviour
{

    public UITexture mBg;

    public UILabel mLv;

    public UILabel mBaseScore;

    public UILabel mLimit;
    public UILabel mLimit_shadow;

    public UILabel mPeopleNum;
    public UILabel mPeopleNum_shadow;

    public GameObject mTipsRoot;//标题提示root
    public UILabel mTips;//标题提示

    private GameGoldPatternConfigConfig mConfig;

    /// <summary>
    /// 初始化Item
    /// </summary>
    /// <param name="con"></param>
    public void InitUIByConfig(GameGoldPatternConfigConfig con)
    {
        mConfig = con;
        Assets.LoadTexture(mConfig.icon, (t) =>
        {
            mBg.mainTexture = t;
        });

        mLv.text = con.lvName;
        //底分
        mBaseScore.text = con.baseScore.ToString();
        mBaseScore.effectColor = NGUIText.ParseColor(con.effectColor, 0);
        mLimit.text = con.minStr;
        mLimit_shadow.text = con.minStr;
        mLimit.effectColor = mBaseScore.effectColor;
        if (!string.IsNullOrEmpty(con.tittle))
        {
            mTipsRoot.gameObject.SetActive(true);
            mTips.text = con.tittle;
        }
        else
        {
            mTipsRoot.gameObject.SetActive(false);
        }
    }


    public void SetPeoPleNum(int num)
    {
        mPeopleNum.text = num + "人";
        mPeopleNum_shadow.text = num + "人";
    }

    public void OnItemClick()
    {
        float gold = PlayerModel.Inst.UserInfo.gold;
        if (gold < mConfig.minFore)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("金币不足，无法进入该房间");
            return;
        }
        //if (gold > mConfig.minAfter)
        //{
        //    Global.Inst.GetController<CommonTipsController>().ShowTips("金币超过该房间上限，请换到高倍场");
        //    return;
        //}
        switch (GamePatternModel.Inst.mCurGameId)
        {
            case eGameType.MaJiang:
                Global.Inst.GetController<MJGameController>().SendJoinMJPattern(mConfig.lvId);
                break;
            case eGameType.NiuNiu:
                Global.Inst.GetController<NNGameController>().SendJoinGoldPattern(mConfig.lvId);
                break;
            case eGameType.GoldFlower:
                Global.Inst.GetController<XXGoldFlowerGameController>().SendJoinGoldPattern(mConfig.lvId);
                break;
            case eGameType.TenHalf:
                Debug.Log(mConfig);
                Global.Inst.GetController<TenGameController>().SendJoinGoldPattern(mConfig.lvId);
                break;
        }
    }
}
