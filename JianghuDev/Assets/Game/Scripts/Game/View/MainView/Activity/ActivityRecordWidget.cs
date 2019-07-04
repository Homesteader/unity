using UnityEngine;
using System.Collections.Generic;

public class ActivityRecordWidget : BaseViewWidget
{
    public GameObject mRecordItem;//记录item
    public Transform mItemParent;
    public UIPanel mPanel;//panel
    public UIScrollView mScroll;
    public SingleScroll mSingleScroll;//SingleaSroll

    private Vector3 mInitPanelPos;//panel初始位置
    private Vector3 mScrollPos;//滑动区域的初始位置
    private int mAllIndex;//总数量

    private List<RecordInfo> dataList;

    protected override void Awake()
    {
        base.Awake();
        mInitPanelPos = mPanel.transform.localPosition;
        mScrollPos = mScroll.transform.localPosition;

        mSingleScroll.AddEvent(eFuiScrollViewEvent.InitItem, OnItemInit);
        mSingleScroll.AddEvent(eFuiScrollViewEvent.UpdateItem, OnItemUpdate);
    }

    #region 初始化

    public void ShowData(List<RecordInfo> data)
    {
        dataList = data;
        mSingleScroll.SetData(data.Count);
    }

    private void OnItemInit(params object[] args)
    {
        int index = int.Parse(args[0].ToString());
        GameObject parent = args[1] as GameObject;
        GameObject obj = NGUITools.AddChild(parent, mRecordItem);
        obj.SetActive(true);
        obj.GetComponent<ActivityRecordItem>().SetData(dataList[index]);
    }


    private void OnItemUpdate(params object[] args)
    {
        int index = int.Parse(args[0].ToString());
        GameObject obj = args[1] as GameObject;
        obj.SetActive(true);
        obj.GetComponent<ActivityRecordItem>().SetData(dataList[index]);
    }
    #endregion

    public void OnCloseClick()
    {
        Close();
    }
}
