using UnityEngine;
using System.Collections;

public class FeedbackWidget : BaseViewWidget
{
    public UIInput mInput;//输入框

    #region 按钮点击
    /// <summary>
    /// 点击提交
    /// </summary>
    public void OnCommitClick()
    {
        if(string.IsNullOrEmpty(mInput.value))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("输入的内容不能为空");
            return;
        }
        Global.Inst.GetController<MainController>().SendFeedBack(mInput.value, ()=> {
            Close<FeedbackWidget>();
        });
    }

    /// <summary>
    /// 关闭点击
    /// </summary>
    public void OnCloseClick()
    {
        Close<FeedbackWidget>();
    }
    #endregion
}
