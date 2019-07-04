using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceView : BaseView{

    public Transform mContent;
    public UIGrid mGrid;//grid
    public GameObject mItem;//item


    protected override void Start()
    {
        base.Start();
        
    }

    public void SetData(string[] id)
    {
        InitDate(id);
    }

    //初始化数据
    private void InitDate(string[] id)
    {
        ServiceItem item;

        List<ConfigDada> data = ConfigManager.GetConfigs<ServiceConfig>();
        for(int i = 0; i < data.Count; i++)
        {
            item = NGUITools.AddChild(mGrid.gameObject, mItem).GetComponent<ServiceItem>();
            item.gameObject.SetActive(true);
            item.SetData(mContent, data[i] as ServiceConfig, id[i]);
        }
        mGrid.Reposition();
    }


    //关闭点击
    public void OnCloseClick()
    {
        Close();
    }
}
