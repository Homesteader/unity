using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class ReturnZhenZhuWidget : BaseViewWidget {

    #region UI

    /// <summary>
    /// uid号码输入
    /// </summary>
    public UIInput mIdInput;

    /// <summary>
    /// 数量输入
    /// </summary>
    public UIInput mNumInput;

    public UILabel mWare;


    #endregion

    private CallBack<float> mCallBack;

    protected override void Start()
    {
        base.Start();
        mWare.text = "可打赏" + PlayerModel.Inst.UserInfo.gold + "个";
    }


    public void SetCallBack(CallBack<float> call) {
        mCallBack = call;
    }


    public void SetId(string id) {
        mIdInput.value = id;
     //   mIdInput.GetComponentInChildren<BoxCollider>().enabled = false;
    }

    #region 按钮事件


    public void OnCloseClick() {
        UpdateUI();
        Close<ReturnZhenZhuWidget>();
    }

    /// <summary>
    /// 提交按钮点击
    /// </summary>
    public void OnSubmitClick() {
        int uid = 0;
        if (int.TryParse(mIdInput.value, out uid)) {
            int num = 0;
            if (int.TryParse(mNumInput.value, out num))
            {
                if (num>0) {
                    Global.Inst.GetController<ClubController>().SendCheckUserInfo(mIdInput.value, (nickName) =>
                     {
                         Global.Inst.GetController<CommonTipsController>().ShowTips("是否打赏给玩家：" + nickName + "  " + int.Parse(mNumInput.value) + "金币？", "取消|确定", null,()=> {
                             Global.Inst.GetController<ClubController>().SendReturnGold(num, mIdInput.value, () =>
                            {
                               Global.Inst.GetController<CommonTipsController>().ShowTips("打赏成功!");
                               mWare.text = "可打赏" + PlayerModel.Inst.UserInfo.gold + "个";
                               CloseWidget<ReturnZhenZhuWidget>();

                            });
                         });
                     });
                }
                else {
                    Global.Inst.GetController<CommonTipsController>().ShowTips("请输入正整数");
                }
            }
            else {
                Global.Inst.GetController<CommonTipsController>().ShowTips("请输入正整数");
            }
        }
        else {
            Global.Inst.GetController<CommonTipsController>().ShowTips("请输入正确的用户id");
        }
    }

    #endregion


    private void UpdateUI()
    {
        Global.Inst.GetController<MainController>().SendGetWareInfo(() =>
        {
            GameObject widget = null;

           
        });
    }
}
