using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRoomView : BaseView {

    public UIGrid mGrid;//grid
    public GameObject mItem;//item


    protected override void Start()
    {
        base.Start();
    }

    public void SetData(List<int> data)
    {
        InitData(data);
    }

    //初始化数据
    private void InitData(List<int> data)
    {
        GameGoldPatternConfig con = ConfigManager.GetConfigs<GameGoldPatternConfig>()[eGameType.GoldFlower.GetHashCode()] as GameGoldPatternConfig;
        SelectRoomItem item;
        for(int i = 0; i < con.config.Count; i++)
        {
            item = NGUITools.AddChild(mGrid.gameObject, mItem).GetComponent<SelectRoomItem>();
            item.gameObject.SetActive(true);
            item.SetData(con.config[i], data[i]);
        }
        mGrid.Reposition();
    }




    //关闭点击
    public void OnCloseClick()
    {
        Close();
    }
}
