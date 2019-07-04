using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PayView : BaseView
{
    private RechargeData mData;//充值条目数据

    public void SetData(RechargeData data)
    {
        mData = data;
    }

    /// <summary>
    /// 支付宝
    /// </summary>
    public void OnAliPayClick()
    {
        Pay("10001");
    }

    /// <summary>
    /// 微信支付
    /// </summary>
    public void OnWXPayClick()
    {
        Pay("20001");
    }

    /// <summary>
    /// 关闭点击
    /// </summary>
    public void OnCloseClick()
    {
        Close();
    }

    private void Pay(string _type)
    {
        // Global.Inst.GetController<PayController>().SendPayInfo(mData, _type);
        Global.Inst.GetController<PayController>().SendGetPayUrl(mData.id, _type);

    }
}
