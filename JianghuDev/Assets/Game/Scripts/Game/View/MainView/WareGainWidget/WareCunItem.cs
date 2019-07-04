using UnityEngine;
using System.Collections;

public class WareCunItem : BaseViewWidget {

    public UILabel mBeibao;

    public UILabel mWare;

    public UIInput mInput;
    
    

    private float mBeiBaoNum;

    private float mWareNum;


    protected override void Awake()
    {
        base.Awake();
        AddEventListenerTarget(GlobalEvent.inst, eEvent.UPDATE_GOLDS,  OnGoldUpdate);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetNum(PlayerModel.Inst.UserInfo.gold, PlayerModel.Inst.UserInfo.wareHouse);
    }


    private void SetNum(float beibao,float ware) {
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
        if (!int.TryParse(mInput.value, out num))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("请输入要存入的金币数量");
            return;
        }
        if (num < 0)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("存入的金币数量要大于0");
            return;
        }

        if (num > PlayerModel.Inst.UserInfo.gold)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("存入的金币数量大于拥有的数量");
            return;
        }
        Global.Inst.GetController<MainController>().SendSaveWare(num, () =>
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("存入成功");
            SetNum(PlayerModel.Inst.UserInfo.gold, PlayerModel.Inst.UserInfo.wareHouse);
        });


    }
    

}
