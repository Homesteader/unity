using UnityEngine;
using System.Collections;

public class ClubUnionWidget : BaseViewWidget
{
    public FUIScrollView mScroll;//scroll
    public GameObject mItem;//item 
    public UILabel mCompanyPeoPle;
    public UILabel mTotalTaozi;
    public UILabel mKeTiaoPei;

    protected override void Awake()
    {
        base.Awake();
        mScroll.AddEvent(eFuiScrollViewEvent.InitItem, OnInitItem);
        mScroll.AddEvent(eFuiScrollViewEvent.UpdateItem, OnUpdateItem);
    }


    public void InitUILabel(int num)
    {
        mCompanyPeoPle.text = num.ToString();
        mTotalTaozi.text = ClubModel.Inst.AllPlayerPeach.ToString();
        mKeTiaoPei.text = PlayerModel.Inst.UserInfo.wareHouse.ToString();
    }


    protected override void Start()
    {
        base.Start();
        int count = ClubModel.Inst.ClubUnionInfoData == null ? 0 : ClubModel.Inst.ClubUnionInfoData.Count;
        mScroll.SetData(count);
    }

    #region scroll
    /// <summary>
    /// 初始化item
    /// </summary>
    /// <param name="args"></param>
    private void OnInitItem(params object[] args)
    {
        int index = (int)args[0];
        SendGetClubConpanyInfo data = ClubModel.Inst.ClubUnionInfoData[index];
        GameObject obj = NGUITools.AddChild(mScroll.gameObject, mItem);
        obj.SetActive(true);
        obj.GetComponent<ClubOneUnionWidget>().SetData(index, data, this);
        mScroll.InitItem(index, obj);
    }

    /// <summary>
    /// 刷新item
    /// </summary>
    /// <param name="args"></param>
    private void OnUpdateItem(params object[] args)
    {
        int index = (int)args[0];
        GameObject obj = args[1] as GameObject;
        SendGetClubConpanyInfo data = ClubModel.Inst.ClubUnionInfoData[index];
        obj.SetActive(true);
        obj.GetComponent<ClubOneUnionWidget>().SetData(index, data, this);
    }
    #endregion


    #region 刷新ui

    public void UpdateUI()
    {
        int count = ClubModel.Inst.ClubUnionInfoData == null ? 0 : ClubModel.Inst.ClubUnionInfoData.Count;
        mScroll.SetData(count);
    }

    #endregion

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick()
    {
        Close();
    }

    public void OnInviteClick()
    {
        ClubAddWidget view = GetWidget<ClubAddWidget>("Windows/ClubView/ClubAddWidget", Global.Inst.GetController<ClubController>().mView.transform);
        view.SetData(eClubAddType.club, (id) =>
        {
            Global.Inst.GetController<ClubController>().SendAddClubConpany(id);
        });
    }

    public void OnExitClick()
    {
        Global.Inst.GetController<CommonTipsController>().ShowTips("是否要退出联盟？", "取消|确定", null, () =>
        {
            Global.Inst.GetController<ClubController>().SendCancelCompany("", () =>
            {
                Global.Inst.GetController<CommonTipsController>().ShowTips("退出成功");
                Close();
            });
        });
    }

    public void OnTiaoPeiClick()
    {
        ClubBorrowWidget view = BaseView.GetWidget<ClubBorrowWidget>("Windows/ClubView/ClubBorrowWidget", Global.Inst.GetController<ClubController>().mView.transform);
        view.SetCallBack((ware) =>
        {
            mKeTiaoPei.text = "可调配：" + PlayerModel.Inst.UserInfo.wareHouse;
        });
    }


    public void OnXxClick()
    {

    }

    /// <summary>
    /// 刷新
    /// </summary>
    public void RefreshTiaopei()
    {
        mKeTiaoPei.text = "可调配：" + PlayerModel.Inst.UserInfo.wareHouse;
    }

}
