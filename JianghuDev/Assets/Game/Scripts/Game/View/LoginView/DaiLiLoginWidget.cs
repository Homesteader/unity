using UnityEngine;
using System.Collections;

public class DaiLiLoginWidget : BaseViewWidget {

    public UIInput mAccount;

    public UIInput mPwd;


    public void OnLoginBtnClick() {
        if (string.IsNullOrEmpty(mAccount.value)) {
            Global.Inst.GetController<CommonTipsController>().ShowTips("账号不能为空!");
            return;
        }
        if (string.IsNullOrEmpty(mPwd.value)) {
            Global.Inst.GetController<CommonTipsController>().ShowTips("密码不能为空!");
            return;
        }
        LoginSR.SendLogin req = new LoginSR.SendLogin();
        req.account = mAccount.value;
        req.password = GameUtils.GetMd5(mPwd.value);
        Global.Inst.GetController<LoginController>().LoginToServer(req, () =>
        {
            Global.Inst.GetController<MainController>().OpenWindow();
            Global.Inst.GetController<LoginController>().CloseWindow();
        });
    }


    public void OnCloseBtnClick() {
        gameObject.SetActive(false);
    }
}
