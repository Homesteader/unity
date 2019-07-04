using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPsdWidget : BaseViewWidget {

    public UIInput mLastPsd;//旧密码
    public UIInput mNewPsd;//新密码
    public UIInput mSurePsd;//确认新密码



    //关闭点击
    public void OnCloseClick()
    {
        Close<FixPsdWidget>();
    }


    //提交点击
    public void OnCommitClick()
    {
        if(string.IsNullOrEmpty(mLastPsd.value))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("旧密码不能为空");
            return;
        }
        if (string.IsNullOrEmpty(mNewPsd.value) || mNewPsd.value.Length < 6)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("新密码不能少于6位字符");
            return;
        }
        if (mSurePsd.value != mNewPsd.value)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("两次输入的密码不一致");
            return;
        }


        Global.Inst.GetController<MainController>().FixPassword(mLastPsd.value, mNewPsd.value, () =>
         {
             Global.Inst.GetController<CommonTipsController>().ShowTips("修改成功");
             Close<FixPsdWidget>();
         });
    }
}
