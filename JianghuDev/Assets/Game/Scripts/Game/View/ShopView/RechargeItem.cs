using UnityEngine;
using System.Collections;

public class RechargeItem : BaseViewWidget
{
    public UILabel mTittle;//标题（获得）
    public UILabel mCost;//花费
    public UISprite mSprite;//图标

    private RechargeData mData;//充值条目数据


    public void SetData(RechargeData data)
    {
        mData = data;
        mSprite.spriteName = data.spName;
        mSprite.MakePixelPerfect();
        mTittle.text = data.desc;//获得
        mCost.text = data.cost.ToString();//花费
    }


    /// <summary>
    /// 充值点击
    /// </summary>
    public void OnRechargeClick()
    {
        SQDebug.Log("======>" + mData.desc);
        Global.Inst.GetController<CommonTipsController>().ShowTips("是否充值：" + mData.desc + "?", "取消|充值", null, () =>
         {
             PayView view = Global.Inst.GetController<PayController>().OpenWindow() as PayView;
             view.SetData(mData); //可以充值 释放
            // Global.Inst.GetController<CommonTipsController>().ShowTips("即将开放,请联系客服处理!");
        }, null, "充值");

    }
}
