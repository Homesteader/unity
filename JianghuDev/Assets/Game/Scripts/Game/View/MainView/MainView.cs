using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainView : BaseView
{
    #region ui
    public Transform mContent;//
    public UITexture mUserHeadImg;//玩家头像
    public UILabel mUserName;//玩家名字
    public UILabel mId;//玩家id
    public UILabel mCoin;//金币数量
    

    #endregion



    protected override void Awake()
    {
        base.Awake();
        AddEventListenerTarget(GlobalEvent.inst, eEvent.UPDATE_PROP, OnPropUpdate);
    }


    protected override void Start()
    {
        base.Start();
        SetData();
    }




    private void SetData()
    {
        LoginSR.LoginUserInfo info = PlayerModel.Inst.UserInfo;
        //头像
        Assets.LoadIcon(info.headUrl, (t) =>
        {
            mUserHeadImg.mainTexture = t;
        });

        
        mUserName.text = info.nickname;//名字
        mId.text = info.userId;//id
        SetCoin(info.gold);//金币
        
    }

  

    /// <summary>
    /// 设置金币数量
    /// </summary>
    /// <param name="coin"></param>
    private void SetCoin(float coin)
    {
        mCoin.text = coin.ToString();
    }

    

    #region 按钮点击


    /// <summary>
    /// 仓库点击
    /// </summary>
    public void OnWareHouseClick()
    {
        //Global.Inst.GetController<MainController>().SendGetWareInfo(() =>
        //{
            GetWidget<WareHouseWidget>("MainView/WareHouseWidget/WareHouseWidget", transform);
        //});
    }

    //战绩点击
    public void OnRecordClick()
    {
        Global.Inst.GetController<RecordController>().GetRecord((data) =>
        {
            RecordView v = Global.Inst.GetController<RecordController>().OpenWindow() as RecordView;
            v.SetData(data);
        });
        
    }

    //客服点击
    public void OnServiceClick()
    {
        Global.Inst.GetController<ServiceController>().GetServiceInfo((info) =>
        {
            ServiceView v = Global.Inst.GetController<ServiceController>().OpenWindow() as ServiceView;
            v.SetData(info);
        });
        
    }


    /// <summary>
    /// 玩家头像点击
    /// </summary>
    public void OnHeadClick()
    {
        PlayerInfoWidget view = GetWidget<PlayerInfoWidget>("MainView/PlayerInfoWidget/PlayerInfoWidget", mContent);
        return;
        Global.Inst.GetController<MainController>().SendGetWareInfo(() =>
        {
            LoginSR.LoginUserInfo info = PlayerModel.Inst.UserInfo;
        });
    }




    //公告点击
    public void OnNoticeClick()
    {
        Global.Inst.GetController<MainController>().SendGetNotice(()=> {
            MessageWidget v = GetWidget<MessageWidget>("MainView/MessageWidget", mContent);
            v.SetData(MainViewModel.Inst.BroadMessage);
        });
    }

    //玩法点击
    public void OnRuleClick()
    {
        GetWidget<HelpWidget>("MainView/HelpWidget", mContent);
    }

    //分享点击
    public void OnShareClick()
    {
        Global.Inst.GetController<ShareController>().OpenWindow();
    }

    /// <summary>
    /// 设置点击
    /// </summary>
    public void OnSettingClick()
    {
        SettingWidget view = GetWidget<SettingWidget>("MainView/SettingWidget", mContent);
        view.SetData(true);
    }

    //牛牛点击
    public void OnNiuniuClick()
    {
        Global.Inst.GetController<CommonTipsController>().ShowTips("暂未开放，敬请期待！");
    }

    //金花点击
    public void OnJinhuaClick()
    {
        Global.Inst.GetController<ModelSelectController>().OpenWindow();
    }

    //麻将点击
    public void OnMajiangClick()
    {
        Global.Inst.GetController<CommonTipsController>().ShowTips("暂未开放，敬请期待！");
    }

    #endregion


    #region 广播
    /// <summary>
    /// 设置广播
    /// </summary>
    private void SetBroadMessage()
    {
    }

    /// <summary>
    /// 增加广播消息
    /// </summary>
    /// <param name="msg"></param>
    public void AddBroadMessage(List<string> msg)
    {
    }
    #endregion

    /// <summary>
    /// 当有道具改变的时候刷新金币和房卡
    /// </summary>
    /// <param name="args"></param>
    private void OnPropUpdate(params object[] args)
    {
        SetCoin(PlayerModel.Inst.UserInfo.gold);
    }
}
