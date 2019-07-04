using UnityEngine;
using System.Collections;

public class ClubAddWidget : BaseViewWidget
{
    public UILabel mInputDesc;//输入提示
    public UIInput mInput;//输入框
    public UISprite title;

    private eClubAddType mType;//类型
    private CallBack<string> mCall;//添加按钮点击回调

    public void SetData(eClubAddType type, CallBack<string> call)
    {
        mType = type;
        mCall = call;
        title.spriteName = type == eClubAddType.player ? "tianjwj_title" : "yaoq_title";
        title.MakePixelPerfect();
        mInput.defaultText = "在这里输入ID";
        if (mType == eClubAddType.player)
        {
            mInputDesc.text = "请输入玩家ID:";
        }
        else
        {
            mInputDesc.text = "请输入俱乐部ID:";
        }
    }

    #region 按钮点击
    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick()
    {
        Close();
    }

    /// <summary>
    /// 添加按钮点击
    /// </summary>
    public void OnAddClick()
    {
        if (string.IsNullOrEmpty(mInput.value))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("ID不能为空");
            return;
        }
        if (mCall != null)
            mCall(mInput.value);
        Close();
    }
    #endregion

}
