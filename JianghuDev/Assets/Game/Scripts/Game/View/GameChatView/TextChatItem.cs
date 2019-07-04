using UnityEngine;
using System.Collections;

public class TextChatItem : BaseViewWidget
{
    public UILabel mLabel;//文字

    private CallBack<int> mClickCall;//点击回调
    private TSTGameTxtChatConfig mCon;//配置表


    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="con">配置表</param>
    /// <param name="call">点击回调方法</param>
    public void SetData(TSTGameTxtChatConfig con, CallBack<int> call)
    {
        mCon = con;
        mClickCall = call;
        mLabel.text = mCon.name;
    }


    /// <summary>
    /// 点击自己
    /// </summary>
    public void OnSelfClick()
    {
        if (mClickCall != null)
            mClickCall(mCon.id);
    }
}
