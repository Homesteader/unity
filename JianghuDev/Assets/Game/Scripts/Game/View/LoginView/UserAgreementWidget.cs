using UnityEngine;
using System.Collections;

public class UserAgreementWidget : BaseViewWidget
{
    public UILabel mText;//用户协议

    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="str"></param>
    public void SetData(string str)
    {
        mText.text = str;
    }

    /// <summary>
    /// 关闭按钮
    /// </summary>
    public void OnCloseClick()
    {
        Close<UserAgreementWidget>();
    }
}
