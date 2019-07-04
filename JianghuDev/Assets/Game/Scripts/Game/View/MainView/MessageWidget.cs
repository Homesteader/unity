using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageWidget : BaseViewWidget {

    public GameObject mMessageItem;
    public UITable mTable;


    #region Unity

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    


    #region 初始化
    //设置数据并显示
    public void SetData(List<string> list)
    {
        InitData(list);
    }

    //初始化数据
    private void InitData(List<string> list)
    {
        int len = list == null ?0: list.Count;
        UILabel label;
        GameObject obj;
        for ( int  i = 0; i < len; i++)
        {
            obj = NGUITools.AddChild(mTable.gameObject, mMessageItem);
            obj.SetActive(true);
            label = obj.transform.GetChild(0).GetComponent<UILabel>();
            label.text = list[i];
        }
        mTable.Reposition();
    }


  
    
    #endregion


    #region 按钮点击事件

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick() {
        
        Close<MessageWidget>();
    }
    

    #endregion
}
