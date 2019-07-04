using UnityEngine;
using System.Collections;

public class CommonTipsView : BaseView
{
    #region 提示框
    public GameObject mBoxTipsRoot;//提示框root
    public UILabel mBoxTipsTittle;//提示框标题
    public UILabel mBoxTipsContent;//提示框内容
    public GameObject[] mBoxTipsBtn;//按钮
    public UILabel[] mBoxTipsBtnLabel;//按钮文字
    public UIGrid mBoxTipsBtnGrid;//gridTran
    public CallBack mBoxBtn1Call;//第一个按钮回调方法
    public CallBack mBoxBtn2Call;//第二个按钮回调方法
    public CallBack mBoxBtn3Call;//第三个按钮回调方法

    #endregion
    #region 文字提示
    public GameObject mTextTipsRoot;//文字提示root
    public UILabel mTextTipsContent;//文字提示内容
    #endregion

    public GameObject mNotCloseTipsRoot;//文字提示，但是没有关闭按钮
    public UILabel mNotCloseTipsContent;//文字提示内容
    public UILabel mNotCloseTitle;
    public UIGrid mNotCloseTipsGrid;
    public GameObject[] mNotCloseBtns;
    public UILabel[] mNotCloseBoxTipsBtnLabel;//按钮文字
    public CallBack mNotCloseBoxBtn1Call;//第一个按钮回调方法
    public CallBack mNotCloseBoxBtn2Call;//第二个按钮回调方法
    public CallBack mNotCloseBoxBtn3Call;//第三个按钮回调方法

    private bool[] mIsCloseView = new bool[] { false, false, false };//点击按钮后是否关闭界面
    #region 显示提示

    /// <summary>
    /// 展示不带关闭按钮的弹窗
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="btn">按钮文字</param>
    /// <param name="show">避免和其他函数冲突，值不重要，随意就好</param>
    /// <param name="btn1Call">第一个按钮的回调</param>
    /// <param name="btn2Call">第二个按钮的回调</param>
    /// <param name="btn3Call">第三个按钮的回调</param>
    /// <param name="tittle">标题</param>
    public void ShowTips(string content, string btn, bool show = true, CallBack btn1Call = null, CallBack btn2Call = null, CallBack btn3Call = null, string tittle = "") {
        SetAllTipsClose();
        mNotCloseTipsRoot.gameObject.SetActive(true);
        //标题
        if (tittle != "")
        {
            mNotCloseTitle.text = tittle;
        }
        else {
            mNotCloseTitle.text = "提 示";
        }

        //内容
        mNotCloseTipsContent.text = content;
        if (string.IsNullOrEmpty(btn))
        {
            SQDebug.LogWarning("按钮不能为空");
            return;
        }
        //按钮文字
        string[] str = btn.Split('|');
        for (int i = 0; i < str.Length; i++)
        {
            mNotCloseBoxTipsBtnLabel[i].text = str[i];
            mNotCloseBtns[i].SetActive(true);
        }
        for (int i = str.Length; i < mNotCloseBtns.Length; i++)
            mNotCloseBtns[i].SetActive(false);
        mNotCloseTipsGrid.Reposition();
        //回调方法
        mNotCloseBoxBtn1Call = btn1Call;
        mNotCloseBoxBtn2Call = btn2Call;
        mNotCloseBoxBtn3Call = btn3Call;
    }

    /// <summary>
    /// 显示文字提示
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="time">关闭时间</param>
    public void ShowTips(string content, float time = 2)
    {
        SetAllTipsClose();
        mTextTipsContent.text = content;
        mTextTipsRoot.SetActive(true);
        StopAllCoroutines();
        DelayRun(time, () =>
        {
            mTextTipsRoot.SetActive(false);
        });
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
        SetAllTipsClose();
        mBoxTipsRoot.gameObject.SetActive(true);
        //标题
        if (tittle != "")
        {
            mBoxTipsTittle.text = tittle;
        }
        else
        {
            mBoxTipsTittle.text = "提 示";
        }
        //内容
        mBoxTipsContent.text = content;
        if (string.IsNullOrEmpty(btn))
        {
            SQDebug.LogWarning("按钮不能为空");
            return;
        }
        //按钮文字
        string[] str = btn.Split('|');
        for (int i = 0; i < str.Length; i++)
        {
            mBoxTipsBtnLabel[i].text = str[i];
            mBoxTipsBtn[i].SetActive(true);
        }
        for (int i = str.Length; i < mBoxTipsBtn.Length; i++)
            mBoxTipsBtn[i].SetActive(false);
        mBoxTipsBtnGrid.Reposition();
        //回调方法
        mBoxBtn1Call = btn1Call;
        mBoxBtn2Call = btn2Call;
        mBoxBtn3Call = btn3Call;
        //重置点击是否关闭界面
        for (int i = 0; i < mIsCloseView.Length; i++)
            mIsCloseView[i] = false;
    }
    #endregion

    private void SetAllTipsClose()
    {
        mTextTipsRoot.SetActive(false);
        mBoxTipsRoot.SetActive(false);
    }


    /// <summary>
    /// 设置点击按钮后是否关闭界面
    /// </summary>
    /// <param name="first">第一个按钮点击后是否关闭界面</param>
    /// <param name="second">第二个按钮点击后是否关闭界面</param>
    /// <param name="third">第3个按钮点击后是否关闭界面</param>
    public void SetViewCloseOnClick(bool first = false, bool second = false, bool third = false)
    {
        mIsCloseView[0] = first;
        mIsCloseView[1] = second;
        mIsCloseView[2] = third;
    }
    #region 按钮点击事件

    /// <summary>
    /// 不带按钮的弹窗关闭按钮点击事件
    /// </summary>
    public void OnCloseClick() {
        mTextTipsRoot.SetActive(false);
    }

    /// <summary>
    /// /=带按钮的弹窗关闭按钮点击事件
    /// </summary>
    public void OnBoxCloseClick() {
        mBoxTipsRoot.SetActive(false);
    }

    /// <summary>
    /// 按钮1点击
    /// </summary>
    public void OnBtn1Click()
    {
        mBoxTipsRoot.SetActive(mIsCloseView[0]);

        if (mBoxBtn1Call != null)
            mBoxBtn1Call();
        
    }
    /// <summary>
    /// 按钮2点击
    /// </summary>
    public void OnBtn2Click()
    {
        mBoxTipsRoot.SetActive(mIsCloseView[1]);

        if (mBoxBtn2Call != null)
            mBoxBtn2Call();
    }
    /// <summary>
    /// 按钮3点击
    /// </summary>
    public void OnBtn3Click()
    {
        mBoxTipsRoot.SetActive(mIsCloseView[2]);

        if (mBoxBtn3Call != null)
            mBoxBtn3Call();
    }

    /// <summary>
    /// 不带关闭按钮的按钮1点击
    /// </summary>
    public void OnNotCloseBtn1Click() {
        mNotCloseTipsRoot.SetActive(false);
        if (mNotCloseBoxBtn1Call!=null) {
            mNotCloseBoxBtn1Call();
        }
    }

    /// <summary>
    /// 不带关闭按钮的按钮2点击
    /// </summary>
    public void OnNotCloseBtn2Click() {
        mNotCloseTipsRoot.SetActive(false);
        if (mNotCloseBoxBtn2Call != null)
        {
            mNotCloseBoxBtn2Call();
        }
    }


    /// <summary>
    /// 不带关闭按钮的按钮3点击
    /// </summary>
    public void OnNotCloseBtn3Click()
    {
        mNotCloseTipsRoot.SetActive(false);
        if (mNotCloseBoxBtn3Call != null)
        {
            mNotCloseBoxBtn3Call();
        }
    }

    #endregion
}
