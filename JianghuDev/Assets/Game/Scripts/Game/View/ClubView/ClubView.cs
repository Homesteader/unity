using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClubView : BaseView
{
    public Transform mContent;
    public UILabel mClubId;//俱乐部id
    public UILabel mPlayerNum;//总人数
    public UILabel mClubRich;//俱乐部珍珠

    public UILabel mWinNum;//盈利金币数量


    public FUIScrollView mScroll;//scroll
    public UIScrollView mUIScroll;//ui scroll
    public UIPanel mPanel;
    public GameObject mPlayerItem;//玩家item

    //数据
    private ClubInfo mData;//数据
    private float mRefreshTime;//刷新时间，3秒

    protected override void Awake()
    {
        base.Awake();
        mScroll.AddEvent(eFuiScrollViewEvent.InitItem, OnInitItem);
        mScroll.AddEvent(eFuiScrollViewEvent.UpdateItem, OnUpdateItem);
    }


    protected override void Start()
    {
        base.Start();
        
        //mUIScroll.MyDrogEnd += OnDragTopCall;
    }



    public void SetData(ClubInfo data)
    {
        mData = data;
        //俱乐部id
        mClubId.text = data.clubId.ToString();
        //人数
        mPlayerNum.text = data.people.ToString();
        //设置仓库金币数
        SetWarehouseNum();
        mWinNum.text = data.winGold.ToString();
        //刷新列表
        int count = ClubModel.Inst.ClubPlayerInfoData == null ? 0 : ClubModel.Inst.ClubPlayerInfoData.Count;
        mScroll.SetData(count);
    }

    /// <summary>
    /// 设置仓库金币数量
    /// </summary>
    /// <param name="t"></param>
    private void SetWarehouseNum()
    {
        //珍珠
        mClubRich.text =  PlayerModel.Inst.UserInfo.wareHouse.ToString();
    }

    #region 按钮点击

    /// <summary>
    /// 局数排行榜点击
    /// </summary>
    public void OnRandRankClick()
    {
        RankForGameRoundWidget widget = BaseView.GetWidget<RankForGameRoundWidget>("Windows/ClubView/RankForGameRoundWidget", transform);
        widget.InitUI("MJ", "ZJH", "NN",1+"");
    }

    /// <summary>
    /// 出借点击
    /// </summary>
    public void OnChuJieClick() {
        ClubBorrowWidget view = BaseView.GetWidget<ClubBorrowWidget>("Windows/ClubView/ClubBorrowWidget", transform);
        view.SetCallBack((t) =>
        {
            Global.Inst.GetController<ClubController>().SendGetClubPlayerInfo();
        });
    }

    /// <summary>
    /// 获赏 打赏 打赏 消费记录
    /// </summary>
    public void OnRecordClick()
    {
        SendGetAgentBRDetail req = new SendGetAgentBRDetail();
        req.num = 10;
        req.page = 1;
        req.type = "gh";
        Global.Inst.GetController<MainController>().SendGetNewPageAgentBRInfo(req, (num) =>
        {
            WareHouseRecordDetailWidget view = GetWidget<WareHouseRecordDetailWidget>("Windows/MainView/WareHouseWidget/WareHouseRecordDetailWidget", transform);
            view.ShowItems();
        });
    }

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick()
    {
        Close();
    }


    /// <summary>
    /// 添加玩家点击
    /// </summary>
    public void OnAddPlayersClick()
    {
        ClubAddWidget view = GetWidget<ClubAddWidget>("Windows/ClubView/ClubAddWidget", mContent);
        view.SetData(eClubAddType.player, (id) =>
        {
            Global.Inst.GetController<ClubController>().SendAddUser(id);
        });
    }

    /// <summary>
    /// 联盟点击
    /// </summary>
    public void OnUnionClick()
    {
        Global.Inst.GetController<ClubController>().SendGetClubCompany(ClubModel.Inst.mClubId);
    }

    /// <summary>
    /// 添加联盟点击
    /// </summary>
    public void OnAddUnionClick()
    {
        ClubAddWidget view = GetWidget<ClubAddWidget>("Windows/ClubView/ClubAddWidget", mContent);
        view.SetData(eClubAddType.club, (id) =>
        {
            Global.Inst.GetController<ClubController>().SendAddClubConpany(id);
        });
    }

    /// <summary>
    /// 查看盈利点击
    /// </summary>
    public void OnLookAgentGainClick() {
        Global.Inst.GetController<ClubController>().SendGetAgentGain();
    }

    #endregion



    #region scroll

    /// <summary>
    /// 初始化item
    /// </summary>
    /// <param name="args"></param>
    private void OnInitItem(params object[] args)
    {
        int index = (int)args[0];
        ClubPlayerInfo data = ClubModel.Inst.ClubPlayerInfoData[index];
        GameObject obj = NGUITools.AddChild(mScroll.gameObject, mPlayerItem);
        obj.SetActive(true);
        obj.GetComponent<ClubOnePlayerWidget>().SetData(data, mContent);
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
        ClubPlayerInfo data = ClubModel.Inst.ClubPlayerInfoData[index];
        obj.SetActive(true);
        obj.GetComponent<ClubOnePlayerWidget>().SetData(data, mContent);
    }


    /// <summary>
    /// 滑动到最顶部
    /// </summary>
    private void OnDragTopCall()
    {
        if (mPanel.clipOffset.y >= 50)
        {
            if (Time.realtimeSinceStartup < mRefreshTime)//三秒后才能刷新，避免重复执行
                return;
            //请求最新的10条
            SQDebug.Log("请求最新的10条");
            SendGetGainDetail req = new SendGetGainDetail();
            req.num = 10;
            req.page = 1;
            req.type = MainViewModel.Inst.mCurGainType;
            Global.Inst.GetController<ClubController>().SendGetClubPlayerInfoAndUpdate( (data) =>
            {
                SetData(data);
            });
            mRefreshTime = Time.realtimeSinceStartup + 3;
        }
    }
    #endregion


    #region 外部刷新

    public void UpdateUI()
    {
        int count = ClubModel.Inst.ClubPlayerInfoData == null ? 0 : ClubModel.Inst.ClubPlayerInfoData.Count;
        mScroll.SetData(count);
    }

    /// <summary>
    /// 更新数据界面
    /// </summary>
    public void UpdateByNet() {
        SendGetGainDetail req = new SendGetGainDetail();
        req.num = 10;
        req.page = 1;
        req.type = MainViewModel.Inst.mCurGainType;
        Global.Inst.GetController<ClubController>().SendGetClubPlayerInfoAndUpdate((data) =>
       {
           SetData(data);
       });
    }

    #endregion


}
