using UnityEngine;
using System.Collections;

public class MJRecordWidget : BaseViewWidget
{
    public FUIScrollView mScroll;//滚动
    public GameObject mItem;//item


    protected override void Awake()
    {
        base.Awake();
        mScroll.AddEvent(eFuiScrollViewEvent.InitItem, OnInitItem);
        mScroll.AddEvent(eFuiScrollViewEvent.UpdateItem, OnUpdateItem);
    }

    protected override void Start()
    {
        base.Start();


        InitData();
    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    private void InitData()
    {
        int len = MJGameBackModel.Inst.RecordAllData == null ? 0 : MJGameBackModel.Inst.RecordAllData.Count;
        if (MJGameBackModel.Inst.RecordAllData != null)
            MJGameBackModel.Inst.RecordAllData.Reverse();
        mScroll.SetData(len);
    }



    #region scroll 
    /// <summary>
    /// 初始化item
    /// </summary>
    /// <param name="args"></param>
    private void OnInitItem(params object[] args)
    {
        int index = (int)args[0];
        MJRecordItemData data = MJGameBackModel.Inst.RecordAllData[index];
        GameObject obj = NGUITools.AddChild(mScroll.gameObject, mItem);
        obj.SetActive(true);
        obj.GetComponent<MJRecordItem>().SetData(data);
        mScroll.InitItem(index, obj);
    }

    /// <summary>
    /// 刷新item
    /// </summary>
    /// <param name="args"></param>
    private void OnUpdateItem(params object[] args)
    {
        int index = (int)args[0];
        GameObject obj = (GameObject)args[1];
        MJRecordItemData data = MJGameBackModel.Inst.RecordAllData[index];
        obj.SetActive(true);
        obj.GetComponent<MJRecordItem>().SetData(data);
    }
    #endregion

    #region 按钮点击
    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick()
    {
        Close();
    }
    #endregion
}
