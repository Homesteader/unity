using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameChatView : BaseView
{
    public Transform mContent;//content
    public GameObject mItem;//item
    public UIGrid mGrid;//gridTran

    private CallBack<int> mItemClickCallback;//聊天点击回调

    protected override void Start()
    {
        base.Start();

    }

    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="callback">聊天点击回调</param>
    public void SetData(CallBack<int> callback)
    {
        mItemClickCallback = callback;
        ShowAllItems();
    }

    /// <summary>
    /// 设置聊天显示位置
    /// </summary>
    /// <param name="pos">显示位置（世界坐标）</param>
    public void SetPos(Vector3 pos)
    {
        mContent.position = pos;
    }

    /// <summary>
    /// 显示所有聊天
    /// </summary>
    private void ShowAllItems()
    {
        List<ConfigDada> con = ConfigManager.GetConfigs<TSTGameTxtChatConfig>();
        if (con == null)
            return;
        GameObject obj;
        for (int i = 0; i < con.Count; i++)
        {
            obj = NGUITools.AddChild(mGrid.gameObject, mItem);
            obj.SetActive(true);
            obj.GetComponent<TextChatItem>().SetData(con[i] as TSTGameTxtChatConfig, OnItemClickCallback);
        }
    }

    /// <summary>
    /// 聊天点击返回
    /// </summary>
    /// <param name="id"></param>
    private void OnItemClickCallback(int id)
    {
        Close();
        if (mItemClickCallback != null)
            mItemClickCallback(id);
    }


    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick()
    {
        Close();
    }
}
