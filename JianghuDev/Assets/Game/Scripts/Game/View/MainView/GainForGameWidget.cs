using UnityEngine;
using System.Collections;

public class GainForGameWidget : BaseViewWidget {
    public UILabel mDayNum;//今天数量
    public UILabel mWeekNum;//周数量
    public UILabel mMonthNum;//月数量
    public GameObject mToggleItem;
    public UIGrid mToggleGrid;

    public UISprite tooglesBG;

    public GameObject mMessageItem;
    public UIPanel mPanel;//panel
    public UIScrollView mScroll;
    public SingleScroll mSingleScroll;//SingleaSroll

    private Vector3 mInitPanelPos;//panel初始位置

    private Vector3 mScrollPos;//滑动区域的初始位置

    private int LastAll = 0;

    private int mAllIndex;


    #region Unity

    protected override void Awake()
    {
        base.Awake();
        mInitPanelPos = mPanel.transform.localPosition;
        mScrollPos = mScroll.transform.localPosition;

        mSingleScroll.AddEvent(eFuiScrollViewEvent.InitItem, OnItemInit);
        mSingleScroll.AddEvent(eFuiScrollViewEvent.UpdateItem, OnItemUpdate);
        //mScroll.MyDrogEnd += OnDragTopCall;
       // mScroll.onStoppedMoving += onStoppedMoving;
    }

    #endregion

    #region 初始化界面

    /// <summary>
    /// 初始化  NN ZJH MJ AGENT
    /// </summary>
    /// <param name="list"></param>
    public void InitUI(params string[] list) {
        GameObject go = null;
        int start = int.Parse(list[list.Length - 1]) -1;//初始按钮
        for (int i=0;i<list.Length - 1;i++) {
            go = NGUITools.AddChild(mToggleGrid.gameObject, mToggleItem.gameObject);
            go.name = list[i].ToLower();
            go.gameObject.SetActive(true);
            UIToggle toggle = go.GetComponentInChildren<UIToggle>();
            UILabel[] label = go.GetComponentsInChildren<UILabel>();
            string temp = "";
            switch (list[i])
            {
                case "PDK":
                    temp = "跑得快";
                    break;
                case "ZJH":
                    temp = "拼三张";
                    break;
                case "NN":
                    temp = "牛仔拼十";
                    break;
                case "AGENT":
                    temp = "代理";
                    break;
            }
            for (int k = 0; k < label.Length; k++)
            {
                label[k].text = temp;
            }

            if (i == start && toggle!=null) {
                toggle.value = true;
            }
        }

        mToggleGrid.Reposition();
        Bounds a = NGUIMath.CalculateRelativeWidgetBounds(mToggleGrid.transform);
        tooglesBG.width = (int)a.size.x + 60;
    }

    #endregion


    #region 显示数据

    /// <summary>
    /// 显示朋友圈
    /// </summary>
    private void ShowItems(PlayerRecordDetailNum nums)
    {
        InitNum(nums);
        mAllIndex = MainViewModel.Inst.mCurGainList.Count;
        mSingleScroll.gameObject.SetActive(true);
        mSingleScroll.SetData(MainViewModel.Inst.mCurGainList.Count);
    }

    /// <summary>
    /// 设置数量
    /// </summary>
    /// <param name="num"></param>
    private void InitNum(PlayerRecordDetailNum num)
    {

        if (num == null)
            num = new PlayerRecordDetailNum();
        mDayNum.text = (num.day > 0 ? "+" + num.day : num.day.ToString());//数量
        //mDayNum.color = num.day > 0 ? NGUIText.ParseColor("46a30e") : NGUIText.ParseColor("d53535");
        mWeekNum.text = (num.week > 0 ? "+" + num.week : num.week.ToString());//数量
        //mWeekNum.color = num.week > 0 ? NGUIText.ParseColor("46a30e") : NGUIText.ParseColor("d53535");
        mMonthNum.text = (num.month > 0 ? "+" + num.month : num.month.ToString());//数量
        //mMonthNum.color = num.month > 0 ? NGUIText.ParseColor("46a30e") : NGUIText.ParseColor("d53535");
    }
    #endregion

    #region 滑动


    /// <summary>
    /// 停止移动
    /// </summary>
    public void onStoppedMoving()
    {
        UIPanel panel = mSingleScroll.GetComponent<UIPanel>();
        if (panel.clipOffset.y > 0)
        {
            mScroll.transform.localPosition = mScrollPos;
            UIPanel sc = mScroll.GetComponent<UIPanel>();
            sc.clipOffset = new Vector2(0, 0);
        }
    }

    /// <summary>
    /// 滑动到最顶部
    /// </summary>
    public void OnDragTopCall()
    {
        UIPanel panel = mSingleScroll.GetComponent<UIPanel>();
        if (panel.clipOffset.y >= 200)
        {
            //请求最新的10条
            SQDebug.Log("请求最新的10条");
            SendGetGainDetail req = new SendGetGainDetail();
            req.num = 10;
            req.page = 1;
            req.type = MainViewModel.Inst.mCurGainType;
            Global.Inst.GetController<MainController>().SendGetNewPageGainInfo(req, (num) =>
            {
                ShowItems(num);
            });
        }
        if (panel.baseClipRegion.w <NGUIMath.CalculateRelativeWidgetBounds(mSingleScroll.transform).size.y)
        {
            OnDragDownCall();
        }
    }

    /// <summary>
    /// 滑动到最底部
    /// </summary>
    public void OnDragDownCall()
    {
        UIPanel panel = mSingleScroll.GetComponent<UIPanel>();
        SQDebug.Log(panel.transform.localPosition.y - mSingleScroll.GetNextDownPos());
        SQDebug.Log(mSingleScroll.GetMaxIndex() >= mAllIndex - 1);

        if (panel.transform.localPosition.y - mSingleScroll.GetNextDownPos() >= 200 && mSingleScroll.GetMaxIndex() >= mAllIndex - 1)
        {
            //请求以前的10条
            SQDebug.Log("请求以前的10条");

            SendGetGainDetail req = new SendGetGainDetail();
            req.num = 10;
            req.page = MainViewModel.Inst.mCurGainPage + 1;
            req.type = MainViewModel.Inst.mCurGainType;
            Global.Inst.GetController<MainController>().SendGetNewPageGainInfo(req, (num) =>
            {
                InitNum(num);
                mSingleScroll.UpdateAllCount(MainViewModel.Inst.mCurGainList.Count);
            });
        }
    }


    #endregion


    #region 初始化

    private void OnItemInit(params object[] args)
    {
        int index = int.Parse(args[0].ToString());
        GameObject parent = args[1] as GameObject;
        GameObject obj = NGUITools.AddChild(parent, mMessageItem);
        obj.SetActive(true);
        obj.GetComponent<GainForGameItem>().SetGainData(MainViewModel.Inst.mCurGainList[index]);

    }


    private void OnItemUpdate(params object[] args)
    {
        int index = int.Parse(args[0].ToString());
        GameObject obj = args[1] as GameObject;
        obj.GetComponent<GainForGameItem>().SetGainData(MainViewModel.Inst.mCurGainList[index]);
    }

    public void AddLast()
    {
        mSingleScroll.AddItemByLast();
    }

    #endregion


    #region 按钮点击事件

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick() {
        MainViewModel.Inst.mCurGainType = "";
        MainViewModel.Inst.mCurGainPage = 0;
        MainViewModel.Inst.mCurGainList.Clear();
        Close<BaseViewWidget>();
    }

    /// <summary>
    /// toggle点击事件
    /// </summary>
    /// <param name="go"></param>
    public void OnTogggleClick(GameObject go) {
        if (UIToggle.current.value) {
            SendGetGainDetail req = new SendGetGainDetail();
            req.num = 10;
            req.page = 1;
            req.type = go.name;
            Global.Inst.GetController<MainController>().SendGetNewPageGainInfo(req, (num) =>
            {
                ShowItems(num);
            });
        }
    }

    #endregion



}
