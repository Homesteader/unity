using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRoomItem : BaseViewWidget {

    public UISprite mTittle;//标题
    public UILabel mCoin;//进入最低金币
    public UILabel mBaseScore;//低分
    public UILabel mPlayerNum;//人数

    private GameGoldPatternConfigConfig mData;//数据


    //设置数据
    public void SetData(GameGoldPatternConfigConfig data, int num)
    {
        mData = data;
        //标题
        mTittle.spriteName = data.icon;
        //最低金币
        mCoin.text = data.minFore.ToString() + "金币入局";
        //底分
        mBaseScore.text = data.baseScore.ToString();
        //人数
        if (num < 3)
            mPlayerNum.text = num.ToString();
        else
            mPlayerNum.text = (num * 3 - 2).ToString();

    }


    //点击
    public void OnItemClick()
    {
        if(PlayerModel.Inst.UserInfo.gold < mData.minFore)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("金币不足，请充值");
            return;
        }
        Global.Inst.GetController<XXGoldFlowerGameController>().SendJoinGoldPattern(mData.lvId);
    }
}
