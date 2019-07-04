using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordView : BaseView {
    public Transform mContent;
    public GameObject mItem;//item
    public UIGrid mGrid;//grid

    //设置数据
    public void SetData(List<RecordItemData> data)
    {
        if (data == null)
            return;
        RecordItem item;
        for (int i = 0; i < data.Count; i++)
        {
            item = NGUITools.AddChild(mGrid.gameObject, mItem).GetComponent<RecordItem>();
            item.gameObject.SetActive(true);
            item.SetData(mContent, data[i]);
        }
        mGrid.Reposition();
    }


    //关闭点击
    public void OnCloseClick()
    {
        Close();
    }
}
