using UnityEngine;
using System.Collections;

public class PlayerWarehouseWidget : BaseViewWidget {

    public UILabel mCangKuLabel;//仓库含量

    public UILabel mCurLabel;//当前珍珠数量

    public UIInput mInput;//输入框

    protected override void Start()
    {
        mCangKuLabel.text = PlayerModel.Inst.UserInfo.wareHouse.ToString();
        mCurLabel.text = PlayerModel.Inst.UserInfo.gold.ToString();
    }

    #region ui事件

    /// <summary>
    /// 关闭点击
    /// </summary>
    public void OnCloseClick() {
        UpdateUI();
        Close<PlayerWarehouseWidget>();
    }

    /// <summary>
    /// 存入点击
    /// </summary>
    public void OnSaveInClick() {
        int num = 0;

        if (int.TryParse(mInput.value, out num)) {
            if (num <= 0)
            {
                Global.Inst.GetController<CommonTipsController>().ShowTips("请输入正整数!");
                return;
            }
            else {
                Global.Inst.GetController<MainController>().SendSaveWare(num, () =>
                {
                    PlayerModel.Inst.UserInfo.wareHouse += num;
                    mCangKuLabel.text = PlayerModel.Inst.UserInfo.wareHouse.ToString();
                    mCurLabel.text = (PlayerModel.Inst.UserInfo.gold - num).ToString();
                    Global.Inst.GetController<CommonTipsController>().ShowTips("存入成功");
                    UpdateUI();
                });
            }
        }
        else {
            Global.Inst.GetController<CommonTipsController>().ShowTips("请输入正整数!");
        }
    }

    /// <summary>
    /// 取出点击
    /// </summary>
    public void OnGetOutClick() {
        int num = 0;

        if (int.TryParse(mInput.value, out num))
        {
            if (num <= 0)
            {
                Global.Inst.GetController<CommonTipsController>().ShowTips("请输入正整数!");
                return;
            }
            else
            {
                //Global.Inst.GetController<MainController>().SendGetWare(num, () =>
                //{
                //    PlayerModel.Inst.UserInfo.wareHouse -= num;
                //    mCangKuLabel.text = PlayerModel.Inst.UserInfo.wareHouse.ToString();
                //    mCurLabel.text = (PlayerModel.Inst.UserInfo.gold + num).ToString();
                //    Global.Inst.GetController<CommonTipsController>().ShowTips("取出成功");
                //    UpdateUI();
                //});
            }
        }
        else
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("请输入正整数!");
        }
    }

    #endregion

    private void UpdateUI() {
        Global.Inst.GetController<MainController>().SendGetWareInfo(() =>
        {
            mCangKuLabel.text = PlayerModel.Inst.UserInfo.wareHouse.ToString();
            mCurLabel.text = PlayerModel.Inst.UserInfo.gold.ToString();
            GameObject widget = null;

            

        });
    }
}
