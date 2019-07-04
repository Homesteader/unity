using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class RealNameCheckWidget : BaseViewWidget
{

    #region UI

    /// <summary>
    /// 身份证号码输入
    /// </summary>
    public UIInput mAccountInput;

    /// <summary>
    /// 手机号码输入
    /// </summary>
    public UIInput mPhoneNumInput;

    /// <summary>
    /// 真实姓名输入
    /// </summary>
    public UIInput mRealNamaInput;

    #endregion


    #region 按钮事件


    public void OnCloseClick()
    {
        Close<RealNameCheckWidget>();
    }

    /// <summary>
    /// 提交按钮点击
    /// </summary>
    public void OnSubmitClick()
    {
        string mNameValue = mRealNamaInput.value;
        string mIdCardValue = mAccountInput.value;
        string mPhoneNumValue = mPhoneNumInput.value;

        if ((string.IsNullOrEmpty(mPhoneNumValue) || mPhoneNumValue.Length != 11))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("请输入正确的手机号码", 1.0f);
            return;
        }

        if ((!Regex.IsMatch(mIdCardValue, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", RegexOptions.IgnoreCase)))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("请输入正确的身份证号码", 1.0f);
            return;
        }
        if (!Regex.IsMatch(mNameValue, @"^[\u4E00-\u9FA5A-Za-z]+$"))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("请输入格式正确的姓名", 1.0f);
            return;
        }

        Global.Inst.GetController<MainController>().SendCheckRealName(mNameValue, mIdCardValue, () =>
        {
            PlayerModel.Inst.UserInfo.realName = mNameValue;
            OnCloseClick();
        });
    }

    #endregion
}
