using UnityEngine;
using System.Collections;

public class CommonTipsController : BaseController
{
    private CommonTipsView mView;

    public CommonTipsController() : base("CommonTipsView", "Windows/CommonTipsView/CommonTipsView")
    {

    }

    public override BaseView OpenWindow()
    {
        mView = base.OpenWindow() as CommonTipsView;
        return mView;
    }

    /// <summary>
    /// 文字提示
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="time">显示时间</param>
    public void ShowTips(string content, float time = 2)
    {
        if (string.IsNullOrEmpty(content))
            return;
        if (mView == null)
            OpenWindow();
        mView.ShowTips(content, time);
    }


    /// <summary>
    /// 显示提示框
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="btn">按钮文字，多个按钮用|分割</param>
    /// <param name="btn1Call">按钮1回调</param>
    /// <param name="btn2Call">按钮2回调</param>
    /// <param name="btn3Call">按钮3回调</param>
    /// <param name="tittle">标题</param>
    public void ShowTips(string content, string btn, CallBack btn1Call = null, CallBack btn2Call = null, CallBack btn3Call = null, string tittle = "")
    {
        if (mView == null)
            OpenWindow();
        mView.gameObject.SetActive(true);
        mView.ShowTips(content, btn, btn1Call, btn2Call, btn3Call, tittle);
    }


    /// <summary>
    /// 显示提示框
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="btn">按钮文字，多个按钮用|分割</param>
    /// <param name="show">请设置为 true或者false 都行</param>
    /// <param name="btn1Call">按钮1回调</param>
    /// <param name="btn2Call">按钮2回调</param>
    /// <param name="btn3Call">按钮3回调</param>
    /// <param name="tittle">标题</param>
    public void ShowTips(string content, string btn, bool show, CallBack btn1Call = null, CallBack btn2Call = null, CallBack btn3Call = null, string tittle = "")
    {
        if (mView == null)
            OpenWindow();
        mView.gameObject.SetActive(true);
        mView.ShowTips(content, btn, show, btn1Call, btn2Call, btn3Call, tittle);
    }


    /// <summary>
    /// 设置点击按钮后提示框状态
    /// </summary>
    /// <param name="first">第一个按钮点击后是否关闭界面</param>
    /// <param name="second">第二个按钮点击后是否关闭界面</param>
    /// <param name="third">第3个按钮点击后是否关闭界面</param>
    public void SetViewCloseOnClick(bool first = false, bool second = false, bool third = false)
    {
        if (mView != null)
        {
            mView.SetViewCloseOnClick(first, second, third);
        }
    }
}
