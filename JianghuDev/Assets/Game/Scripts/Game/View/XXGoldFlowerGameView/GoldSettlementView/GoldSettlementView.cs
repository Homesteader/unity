using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldSettlementView : BaseViewWidget {

    public UISprite mMyResult;//自己的结果
    public UIGrid mGrid;//grid
    public GameObject mItem;//item

    public void SetData(GoldSettlementItemData[] data)
    {
        GoldSettlementItem item;
        for(int i = 0; i < data.Length; i++)
        {
            item = NGUITools.AddChild(mGrid.gameObject, mItem).GetComponent<GoldSettlementItem>();
            item.gameObject.SetActive(true);
            item.SetData(data[i]);
            if(data[i].userId == PlayerModel.Inst.UserInfo.userId)//如果是自己
            {
                mMyResult.spriteName = data[i].score > 0 ? "label_win" : "label_lose";
            }
        }
        mGrid.Reposition();
    }


    //返回点击
    public void OnBackClick()
    {
        Global.Inst.GetController<XXGoldFlowerGameController>().SendLeaveRoom();
    }

    //下一局
    public void OnNextClick()
    {
        Global.Inst.GetController<XXGoldFlowerGameController>().SendReady(()=>
        {
            Close<GoldSettlementView>();
        });
    }

}
