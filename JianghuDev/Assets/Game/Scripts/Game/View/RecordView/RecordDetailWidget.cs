using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordDetailWidget : BaseViewWidget {

    public UITexture mIcon;//头像
    public UILabel mUserName;//玩家名字
    public UILabel mPoint;//分数
    public GameObject mItem;//item
    public UIGrid mGrid;//grid
    public UISprite[] mCardBg;//牌背景
    public UISprite[] mCardNum;//牌数字

    //设置数据并显示
    public void SetData(RecordItemData data)
    {
        RecordPlayerData p = data.record.Find(o => o.userId == PlayerModel.Inst.UserInfo.userId);
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

        //其他玩家
        RecordDetailItem item;
        for(int i = 0; i < data.record.Count; i++)
        {
            if (data.record[i].userId == PlayerModel.Inst.UserInfo.userId)
                continue;
            item = NGUITools.AddChild(mGrid.gameObject, mItem).GetComponent<RecordDetailItem>();
            item.gameObject.SetActive(true);
            item.SetData(data.record[i]);
        }
        mGrid.Reposition();

    }


    //关闭点击
    public void OnCloseClick()
    {
        Close<RecordDetailWidget>();
    }
}
