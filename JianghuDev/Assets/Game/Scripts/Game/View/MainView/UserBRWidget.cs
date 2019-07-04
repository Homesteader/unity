using UnityEngine;
using System.Collections;

public class UserBRWidget : BaseViewWidget {

    public GameObject mToggleItem;
    public UIGrid mToggleGrid;

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
    }

    #endregion

    #region 初始化界面

    /// <summary>
    /// 初始化 JR GH
    /// </summary>
    /// <param name="list"></param>
    public void InitUI(params string[] list) {
        GameObject go = null;
        for (int i=0;i<list.Length;i++) {
            go = NGUITools.AddChild(mToggleGrid.gameObject, mToggleItem.gameObject);
            go.name = list[i].ToLower();
            go.gameObject.SetActive(true);
            UIToggle toggle = go.GetComponentInChildren<UIToggle>();
            UILabel[] label = go.GetComponentsInChildren<UILabel>();
            string temp = "";
            switch (list[i])
            {//JR GH
                case "JR":
                    temp = "获赏记录";
                    break;
                case "GH":
                    temp = "打赏记录";
                    break;
            }
            for (int k = 0; k < label.Length; k++)
            {
                label[k].text = temp;
            }

            if (i == 0 && toggle!=null) {
                toggle.value = true;
            }
        }

        mToggleGrid.Reposition();
    }

    #endregion


    #region 显示数据

    /// <summary>
    /// 显示朋友圈
    /// </summary>
    public void ShowItems()
    {
        mAllIndex = MainViewModel.Inst.mCurUserList.Count;
        mSingleScroll.gameObject.SetActive(true);
        mSingleScroll.SetData(MainViewModel.Inst.mCurUserList.Count);
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
            SendGetUserBRDetail req = new SendGetUserBRDetail();
            req.num = 10;
            req.page = 1;
            req.type = MainViewModel.Inst.mCurUserType;
            Global.Inst.GetController<MainController>().SendGetNewPageUserBRInfo(req, () =>
            {
                ShowItems();
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
    public void OnDragDownCall()
    {
        UIPanel panel = mSingleScroll.GetComponent<UIPanel>();
        SQDebug.Log(panel.transform.localPosition.y - mSingleScroll.GetNextDownPos());
        SQDebug.Log(mSingleScroll.GetMaxIndex() >= mAllIndex - 1);

        if (panel.transform.localPosition.y - mSingleScroll.GetNextDownPos() >= 200 && mSingleScroll.GetMaxIndex() >= mAllIndex - 1)
        {
            //请求以前的10条
            SQDebug.Log("请求以前的10条");

            SendGetUserBRDetail req = new SendGetUserBRDetail();
            req.num = 10;
            req.page = MainViewModel.Inst.mCurUserPage + 1;
            req.type = MainViewModel.Inst.mCurUserType;
            Global.Inst.GetController<MainController>().SendGetNewPageUserBRInfo(req, () =>
            {
                mSingleScroll.UpdateAllCount(MainViewModel.Inst.mCurUserList.Count);
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
        obj.GetComponent<GainForGameItem>().InitUserUI(MainViewModel.Inst.mCurUserList[index].time,
            MainViewModel.Inst.mCurUserList[index].nickName, MainViewModel.Inst.mCurUserList[index].gold,
            MainViewModel.Inst.mCurUserType);

    }


    private void OnItemUpdate(params object[] args)
    {
        int index = int.Parse(args[0].ToString());
        GameObject obj = args[1] as GameObject;
        obj.GetComponent<GainForGameItem>().InitUserUI(MainViewModel.Inst.mCurUserList[index].time,
            MainViewModel.Inst.mCurUserList[index].nickName, MainViewModel.Inst.mCurUserList[index].gold,
            MainViewModel.Inst.mCurUserType);
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
        MainViewModel.Inst.mCurUserType = "";
        MainViewModel.Inst.mCurUserPage = 0;
        MainViewModel.Inst.mCurUserList.Clear();
        Close<UserBRWidget>();
    }

    /// <summary>
    /// toggle点击事件
    /// </summary>
    /// <param name="go"></param>
    public void OnTogggleClick(GameObject go) {
        if (UIToggle.current.value) {
            SendGetUserBRDetail req = new SendGetUserBRDetail();
            req.num = 10;
            req.page = 1;
            req.type = go.name;
            Global.Inst.GetController<MainController>().SendGetNewPageUserBRInfo(req, () =>
            {
                ShowItems();
            });
        }
    }

    #endregion



}
