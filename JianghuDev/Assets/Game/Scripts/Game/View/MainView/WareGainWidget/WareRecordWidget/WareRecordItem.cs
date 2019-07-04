using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WareRecordItem : BaseViewWidget
{

    public UILabel mTime;//时间
    public UILabel mName;//玩家名字
    public UILabel mNum;//数量
    public UILabel mTag;//标记

    private SendGetAgentBRInfo mData;//数据
    private Transform mRoot;//root

    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="data"></param>
    public void SetData(SendGetAgentBRInfo data, Transform root, bool isAgent = true)
    {
        mData = data;
        mRoot = root;
        float gold = isAgent ? data.gold : -data.gold;
        gold = float.Parse(gold.ToString("f2"));
        string timestr = data.time;
        mTime.text = timestr.Replace(" ","\n");//时间
        mName.text = data.nickName;//名字
        mNum.text = gold > 0 ? "+" + gold : gold.ToString();//数量
        mNum.color = gold > 0 ? NGUIText.ParseColor("46a30e") : NGUIText.ParseColor("d53535");
        //标记
        if (mTag == null)
            return;
        if (isAgent)//代理
            mTag.text = gold <= 0 ? "打赏" : "获赏";
        else
            mTag.text = gold <= 0 ? "打赏" : "获赏";
        mTag.color = mNum.color;
    }



    #region 按钮点击
    /// <summary>
    /// 查看按钮点击
    /// </summary>
    public void OnLookClick()
    {
        MainViewModel.Inst.mCurPlayerDetailList = new List<SendGetAgentBRInfo>();
        SendGetAgentBRDetail req = new SendGetAgentBRDetail();
        req.num = 10;
        req.page = 1;
        req.type = mData.uid;
        Global.Inst.GetController<MainController>().SendGetNewPagePlayerDetailRecord(req, (num) =>
        {
            WareHousePlayerDetailWidget view = GetWidget<WareHousePlayerDetailWidget>("Windows/MainView/WareHouseWidget/WareHousePlayerDetailWidget", mRoot);
            view.SetData(num, mData.uid);
        });
    }
    #endregion
}
