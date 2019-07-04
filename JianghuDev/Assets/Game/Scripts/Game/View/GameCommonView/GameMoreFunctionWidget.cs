using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class GameMoreFunctionWidget : BaseViewWidget
{

    public GameObject mBtnItem;

    public UIGrid mGrid;


    /// <summary>
    /// 显示图片名字
    /// </summary>
    public Dictionary<eGameMore, string> mShowSpDic = new Dictionary<eGameMore, string>();

    /// <summary>
    /// Item列表
    /// </summary>
    private eGameMore[] mTypeArgs;
    /// <summary>
    /// 回调字典
    /// </summary>
    private Dictionary<string, Action> mActionDic = new Dictionary<string, Action>();
    private CallBack mCloseCall;//关闭回调


    protected override void Awake()
    {
        base.Awake();


        mShowSpDic.Add(eGameMore.Setting, "btn_setting");
        mShowSpDic.Add(eGameMore.Distance, "btn_gps");
        mShowSpDic.Add(eGameMore.Leave, "btn_leaveRoom");

    }

    /// <summary>
    /// 关闭点击
    /// </summary>
    public void OnCloseClick()
    {
        if (mCloseCall != null)
            mCloseCall();
        Close<GameMoreFunctionWidget>();
    }

    /// <summary>
    /// 按钮点击
    /// </summary>
    public void OnBtnItemClick(GameObject go)
    {
        Action call = null;
        if (mActionDic.TryGetValue(go.name, out call))
        {
            if (call != null)
            {
                call();
            }
        }
        OnCloseClick();
    }

    /// <summary>
    /// 设置btn
    /// </summary>
    /// <param name="list"></param>
    public void SetBtnItems(params eGameMore[] args)
    {
        NGUITools.DestroyChildren(mGrid.transform);
        mActionDic.Clear();
        mTypeArgs = args;
        if (args != null)
        {
            for (int i = 0; i < args.Length; i++)
            {
                GameObject temp = Assets.InstantiateChild(mGrid.gameObject, mBtnItem);
                temp.gameObject.SetActive(true);
                temp.name = args[i].ToString();
                temp.GetComponentInChildren<UISprite>().spriteName = mShowSpDic[args[i]];
            }
        }
        mGrid.Reposition();
    }

    /// <summary>
    /// 设置回调
    /// </summary>
    /// <param name="args"></param>
    public void SetBtnCallBack(params Action[] args)
    {
        for (int i = 0; i < mTypeArgs.Length; i++)
        {
            if (i < args.Length)
            {
                mActionDic.Add(mTypeArgs[i].ToString(), args[i]);
            }
        }
    }

    public void SetBtnCloseCall(CallBack call)
    {
        mCloseCall = call;
    }
}
