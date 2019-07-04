using UnityEngine;
using System.Collections;

public class JoinClubWidget : BaseViewWidget
{


    public UIInput mInput;

    public void OnCloseClick()
    {
        CloseWidget<JoinClubWidget>();
    }

    public void OnSendClick()
    {
        if (string.IsNullOrEmpty(mInput.value))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("俱乐部id不能为空");
        }
        else
        {
            Global.Inst.GetController<MainController>().SendJoinClubReq(mInput.value, () =>
            {
                Global.Inst.GetController<CommonTipsController>().ShowTips("申请已发送");
                OnCloseClick();
                PlayerModel.Inst.UserInfo.clubId = mInput.value;
                //Global.Inst.GetController<MainController>().mView.SetClubState(false);
            });
        }
    }
}
