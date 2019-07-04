using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceGiveWidget : BaseViewWidget {

    public UILabel mUserName;//客服名字
    public UILabel mUserId;//客服id
    public UIInput mInputNum;//输入框
    public UILabel mCoinNum;//金币数量
    public UILabel mGiveTip;//赠送提示

    private int mMinNum = 0;//提现最小数字
    private ServiceConfig mData;//数据

    protected override void Start()
    {
        base.Start();
        //oinNum.text = PlayerModel.Inst.UserInfo.gold.ToString();
        OtherConfig con = ConfigManager.GetConfigs<OtherConfig>()[0] as OtherConfig;
        mMinNum = con.tixianMinNum;
        //赠送最低提示
        mGiveTip.text = string.Format("赠送数量（最低{0}金币）", mMinNum);
    }


    //设置数据并显示
    public void SetData(ServiceConfig data)
    {
        mData = data;
        //客服id
        mUserId.text = data.userId;
        //客服名字
        mUserName.text = data.username;
        //当前金币
        mCoinNum.text = PlayerModel.Inst.UserInfo.gold.ToString("f2");
    }


    //关闭点击
    public void OnCloseClick()
    {
        Close<ServiceGiveWidget>();
    }

    //提交
    public void OnCommitClick()
    {
        int num = 0;
        if(!int.TryParse( mInputNum.value, out num))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("请输入数字");
            return;
        }
        if(num < mMinNum)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("最低提现"+mMinNum);
            return;
        }
        if(PlayerModel.Inst.UserInfo.gold < num)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips( "金币不足！");
            return;
        }
        Global.Inst.GetController<ClubController>().SendBorrowGold(num, mData.userId, (ware) =>
        {
            string str = string.Format("{0}年{1}月{2}日{3:D2}:{4:D2}:{5:D2}", System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
            string str2 = PlayerModel.Inst.UserInfo.userId + "成功赠送" + num + "金币给" + mData.userId;
            Global.Inst.GetController<CommonTipsController>().ShowTips(str + "\n" + str2, "确定");
            //当前金币
            mCoinNum.text = PlayerModel.Inst.UserInfo.gold.ToString("f2");
        });
    }
}
