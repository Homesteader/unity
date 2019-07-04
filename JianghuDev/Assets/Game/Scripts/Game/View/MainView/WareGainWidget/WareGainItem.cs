using UnityEngine;
using System.Collections;

public class WareGainItem : BaseViewWidget {

    public UILabel mBeibao;//背包中的金币

    //public UILabel mWare;//仓库中金币

    public UIInput mNumInput;//数量
    public UIInput mUserIdInput;//玩家id


    private int mMinNum = 0;//提现最小数字
    private float mBeiBaoNum;

    private float mWareNum;

    protected override void Awake()
    {
        base.Awake();
        AddEventListenerTarget(GlobalEvent.inst, eEvent.UPDATE_GOLDS, OnGoldUpdate);
        OtherConfig con = ConfigManager.GetConfigs<OtherConfig>()[0] as OtherConfig;
        mMinNum = con.tixianMinNum;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetNum(PlayerModel.Inst.UserInfo.gold, PlayerModel.Inst.UserInfo.wareHouse);
    }


    private void SetNum(float beibao, float ware)
    {
        mBeiBaoNum = beibao;
        mWareNum = ware;
        mBeibao.text = beibao.ToString("f2");
        //mWare.text = ware.ToString("f2");

    }

    //金币刷新
    private void OnGoldUpdate(params object[] args)
    {
        SetNum(PlayerModel.Inst.UserInfo.gold, PlayerModel.Inst.UserInfo.wareHouse);
    }


    public void OnSubmitClick()
    {
        int num = 0;
        if (!int.TryParse(mNumInput.value, out num))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("请输入要赠送的金币数量");
            return;
        }
        if (num < 0)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("赠送的金币数量要大于0");
            return;
        }

        if (num > PlayerModel.Inst.UserInfo.gold)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("赠送的金币数量大于拥有的数量");
            return;
        }
        if (string.IsNullOrEmpty(mUserIdInput.value))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("赠送玩家ID不能为空");
            return;
        }

        if (num < mMinNum)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("最低赠送" + mMinNum);
            return;
        }

        Global.Inst.GetController<ClubController>().SendBorrowGold(num, mUserIdInput.value, (ware) =>
        {
            string str = string.Format("{0}年{1}月{2}日{3:D2}:{4:D2}:{5:D2}", System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
            string str2 = PlayerModel.Inst.UserInfo.userId + "成功赠送" + num + "金币给" + mUserIdInput.value;
            Global.Inst.GetController<CommonTipsController>().ShowTips(str + "\n" + str2, "确定");
            SetNum(PlayerModel.Inst.UserInfo.gold, PlayerModel.Inst.UserInfo.wareHouse);
        });
    }

}
