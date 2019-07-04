using UnityEngine;
using System.Collections;

public class GeneralAgentWidget : BaseViewWidget
{
    public UILabel mTodayProfit;//今日收益
    public UILabel mMonthProfit;//本月收益
    public UILabel mLastMonthProfit;//上月收益
    public UILabel mHigherAgentName;//上级代理名字
    public UILabel mHigherAgentId;//上级代理id
    public UIGrid mHigherAgrentGrid;//gridTran
    public FUIScrollView mScroll;//scroll
    public GameObject mAgentItem;//代理item

    //数据
    private GeneralSubAgentData[] mItemData;//代理数据

    protected override void Awake()
    {
        base.Awake();
        mScroll.AddEvent(eFuiScrollViewEvent.InitItem, OnInitItem);
        mScroll.AddEvent(eFuiScrollViewEvent.UpdateItem, OnUpdateItem);
    }

    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="data"></param>
    public void SetData(GeneralAgrentData data)
    {
        //今日收益
        mTodayProfit.text = float.Parse(data.gpData.dayProfit.ToString("#0.00")).ToString();
        //本月收益
        mMonthProfit.text = float.Parse(data.gpData.monthProfit.ToString("#0.00")).ToString();
        //上月收益
        mLastMonthProfit.text = float.Parse(data.gpData.lastMonthProfit.ToString("#0.00")).ToString();
        //上级代理名字
        mHigherAgentName.text = data.higherupAgrent.nickname;
        //上级代理id
        if (data.higherupAgrent.userId == "a888888")
            mHigherAgentId.gameObject.SetActive(false);
        else
        {
            mHigherAgentId.gameObject.SetActive(true);
            mHigherAgentId.text = "ID:" + data.higherupAgrent.userId;
        }
        //显示下级代理
        mItemData = data.subAgent;
        int count = mItemData == null ? 0 : mItemData.Length;
        mScroll.SetData(count);
    }


    //public void testSet()
    //{
    //    GeneralAgrentData data = new GeneralAgrentData();
    //    mItemData = data.subAgent;
    //    mItemData = new GeneralSubAgentData[] { new GeneralSubAgentData(), new GeneralSubAgentData(), new GeneralSubAgentData() };
    //    int count = mItemData == null ? 0 : mItemData.Length;
    //    mScroll.SetData(count);
    //}

    #region scroll
    private void OnInitItem(params object[] args)
    {
        int index = (int)args[0];
        GameObject obj = NGUITools.AddChild(mScroll.gameObject, mAgentItem);
        obj.SetActive(true);
        obj.GetComponent<GeneralAgentItem>().SetData(mItemData[index]);
        mScroll.InitItem(index, obj);
    }


    private void OnUpdateItem(params object[] args)
    {
        int index = (int)args[0];
        GameObject obj = args[1] as GameObject;
        obj.SetActive(true);
        obj.GetComponent<GeneralAgentItem>().SetData(mItemData[index]);
    }
    #endregion


    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick()
    {
        Close();
    }
}
