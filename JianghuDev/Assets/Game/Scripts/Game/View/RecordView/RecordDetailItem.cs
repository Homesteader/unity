using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordDetailItem : BaseViewWidget {

    public UITexture mIcon;//头像
    public UILabel mUserName;//玩家名字
    public UILabel mPoint;//分数
    public UISprite[] mCardBg;//牌背景
    public UISprite[] mCardNum;//牌数字

    //设置数据并显示
    public void SetData(RecordPlayerData p)
    {
        //头像
        Assets.LoadIcon(p.headUrl, (t) =>
        {
            mIcon.mainTexture = t;
        });
        //名字
        mUserName.text = p.username;
        //分数
        mPoint.text = p.point > 0 ? ("+" + p.point.ToString("f2")) : p.point.ToString("f2");
        //牌数字
        for (int i = 0; i < 3; i++)
        {
            TSTUtil.SetGameCardNum(p.card[i], XXGoldFlowerGameModel.Inst.mCardType,
            mCardBg[i], mCardNum[i]);
        }
    }
}
