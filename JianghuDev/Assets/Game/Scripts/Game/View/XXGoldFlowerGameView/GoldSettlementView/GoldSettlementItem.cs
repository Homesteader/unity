using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldSettlementItem : BaseViewWidget {

    public UITexture mIcon;//头像
    public UILabel mGold;//金币
    public UILabel mXi;//喜
    public UILabel mScore;//输赢分数
    public UISprite mResult;//结果
    public UISprite[] mCardBg;//牌底
    public UISprite[] mCardNum;//牌数字
    


    public void SetData(GoldSettlementItemData data)
    {
        //头像
        Assets.LoadIcon(data.icon, (t) =>
        {
            mIcon.mainTexture = t;
        });
        //金币
        mGold.text = data.gold.ToString("f2");
        //喜
        mXi.text = data.xi.ToString("f2");
        //分数
        mScore.text = data.score.ToString("f2");
        //结果
        mResult.spriteName = data.score > 0 ? "st_win" : "st_lose";
        //牌
        for(int i = 0; i < data.cards.Length; i++)
        {
            TSTUtil.SetGameCardNum(data.cards[i], XXGoldFlowerGameModel.Inst.mCardType,
            mCardBg[i], mCardNum[i]);
        }
    }
}
