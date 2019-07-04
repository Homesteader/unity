using UnityEngine;
using System.Collections;

public class WareHouseSelfDetailWidget : BaseViewWidget
{
    public GameObject mRecordItem;//记录item
    public UIPanel mPanel;//panel
    public UIScrollView mScroll;
    public SingleScroll mSingleScroll;//SingleaSroll

    private Vector3 mInitPanelPos;//panel初始位置
    private Vector3 mScrollPos;//滑动区域的初始位置
    private int mAllIndex;//总数量
    private string mUserId;//玩家id
    private bool mIsAgent;//是否是代理

    protected override void Awake()
    {
        base.Awake();
        mInitPanelPos = mPanel.transform.localPosition;
        mScrollPos = mScroll.transform.localPosition;

        mSingleScroll.AddEvent(eFuiScrollViewEvent.InitItem, OnItemInit);
        mSingleScroll.AddEvent(eFuiScrollViewEvent.UpdateItem, OnItemUpdate);
        //mScroll.MyDrogEnd += OnDragTopCall;
    }

    /// <summary>
    /// 设置数据并显示数据
    /// </summary>
    /// <param name="day"></param>
    /// <param name="week"></param>
    /// <param name="month"></param>
    public void SetData(PlayerRecordDetailNum num, string userid, bool isAgent = true)
    {
        mIsAgent = isAgent;
        mUserId = userid;
        InitNum(num);
        ShowItems();
    }

    private void InitNum(PlayerRecordDetailNum num)
    {

        if (num == null)
            num = new PlayerRecordDetailNum();
        float day = num.day;
        float week = num.week;
        float month = num.month;
        if (!mIsAgent)//不是代理要反过来
        {
            day = float.Parse((-day).ToString("f2"));
            week = float.Parse((-week).ToString("f2"));
            month = float.Parse((-month).ToString("f2"));
        }
    }

    #region 初始化

    private void OnItemInit(params object[] args)
    {
        int index = int.Parse(args[0].ToString());
        GameObject parent = args[1] as GameObject;
        GameObject obj = NGUITools.AddChild(parent, mRecordItem);
        obj.SetActive(true);
        obj.GetComponent<WareRecordItem>().SetData(MainViewModel.Inst.mCurPlayerDetailList[index], transform, mIsAgent);

    }


    private void OnItemUpdate(params object[] args)
    {
        int index = int.Parse(args[0].ToString());
        GameObject obj = args[1] as GameObject;
        obj.SetActive(true);
        obj.GetComponent<WareRecordItem>().SetData(MainViewModel.Inst.mCurPlayerDetailList[index], transform, mIsAgent);
    }

    public void AddLast()
    {
        mSingleScroll.AddItemByLast();
    }

    #endregion




    /// <summary>
    /// 显示朋友圈
    /// </summary>
    private void ShowItems()
    {
        mAllIndex = MainViewModel.Inst.mCurPlayerDetailList.Count;
        mSingleScroll.gameObject.SetActive(true);
        mSingleScroll.SetData(MainViewModel.Inst.mCurPlayerDetailList.Count);
    }

    #region 滑动


    /// <summary>
    /// 滑动到最顶部
    /// </summary>
    private void OnDragTopCall()
    {
        UIPanel panel = mSingleScroll.GetComponent<UIPanel>();
        if (panel.clipOffset.y >= 200)
        {
            //请求最新的10条
            SQDebug.Log("请求最新的10条");
            SendGetAgentBRDetail req = new SendGetAgentBRDetail();
            req.num = 10;
            req.page = 1;
            req.type = mUserId;
            Global.Inst.GetController<MainController>().SendGetNewPagePlayerDetailRecord(req, (num) =>
            {
                SetData(num, mUserId, mIsAgent);
            });
        }
        if (panel.baseClipRegion.w < NGUIMath.CalculateRelativeWidgetBounds(mSingleScroll.transform).size.y)
        {
            OnDragDownCall();
        }
    }

    /// <summary>
    /// 滑动到最底部
    /// </summary>
    private void OnDragDownCall()
    {
        UIPanel panel = mSingleScroll.GetComponent<UIPanel>();
        SQDebug.Log(panel.transform.localPosition.y - mSingleScroll.GetNextDownPos());
        SQDebug.Log(mSingleScroll.GetMaxIndex() >= mAllIndex - 1);

        if (panel.transform.localPosition.y - mSingleScroll.GetNextDownPos() >= 200 && mSingleScroll.GetMaxIndex() >= mAllIndex - 1)
        {
            //请求以前的10条
            SQDebug.Log("请求以前的10条");

            SendGetAgentBRDetail req = new SendGetAgentBRDetail();
            req.num = 10;
            req.page = MainViewModel.Inst.mCurPlayerDetailPage + 1;
            req.type = mUserId;
            Global.Inst.GetController<MainController>().SendGetNewPagePlayerDetailRecord(req, (num) =>
            {
                InitNum(num);
                mSingleScroll.UpdateAllCount(MainViewModel.Inst.mCurPlayerDetailList.Count);
            });
        }
    }


    #endregion


    public void OnCloseClick()
    {
        Close();
    }
}
