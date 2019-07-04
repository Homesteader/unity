using UnityEngine;
using System.Collections;

public class WareQuItem : BaseViewWidget {

    public UILabel mBeibao;//背包中的金币

    public UILabel mWare;//仓库中金币

    public UIInput mNumInput;//数量
    public UIInput mPsdInput;//密码



    private float mBeiBaoNum;

    private float mWareNum;

    protected override void Awake()
    {
        base.Awake();
        AddEventListenerTarget(GlobalEvent.inst, eEvent.UPDATE_GOLDS, OnGoldUpdate);
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
        mWare.text = ware.ToString("f2");

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
            Global.Inst.GetController<CommonTipsController>().ShowTips("请输入要取出的金币数量");
            return;
        }
        if (num < 0)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("取出的金币数量要大于0");
            return;
        }

        if (num > PlayerModel.Inst.UserInfo.wareHouse)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("取出的金币数量大于拥有的数量");
            return;
        }
        if (string.IsNullOrEmpty(mPsdInput.value))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("密码不能为空");
            return;
        }

        Global.Inst.GetController<MainController>().SendGetWare(num, mPsdInput.value, () =>
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("取出成功");
            SetNum(PlayerModel.Inst.UserInfo.gold, PlayerModel.Inst.UserInfo.wareHouse);
        });
        //Global.Inst.GetController<MainController>().SendGetWareInfo(() => { });
        

    }

}
