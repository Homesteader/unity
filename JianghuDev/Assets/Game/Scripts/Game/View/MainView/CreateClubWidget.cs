using UnityEngine;
using System.Collections;

public class CreateClubWidget : BaseViewWidget {

    public UIInput mInput;


    public void OnCloseClick() {
        Close<CreateClubWidget>();
    }


    public void OnCreateClick() {
        if (string.IsNullOrEmpty(mInput.value)) {
            Global.Inst.GetController<CommonTipsController>().ShowTips("俱乐部名称不能为空");
        }
        else {
            Global.Inst.GetController<ClubController>().SendCreateClub(mInput.value);
        }
    }
}
