using UnityEngine;
using System.Collections.Generic;

public class CreateRoomView : BaseView
{
    public UIGrid mGrid;//grid
    public GameObject mItem;//item

    private List<ConfigDada> mData;//配置表

    protected override void Awake()
    {
        base.Awake();
        InitData();
    }

    //初始化数据
    private void InitData()
    {
        List<ConfigDada> con = ConfigManager.GetConfigs<JinhuaCreateConfig>();
        mData = con;
        GameObject obj;
        JinhuaCreateConfig config;
        for (int i = 0; i < con.Count; i++)
        {
            config = con[i] as JinhuaCreateConfig;
            obj = NGUITools.AddChild(mGrid.gameObject, mItem);
            obj.name = config.id.ToString();
            if (i == 0)
                obj.GetComponent<UIToggle>().value = true;
            obj.transform.GetChild(1).GetComponent<UILabel>().text = config.baseScore.ToString();
            obj.SetActive(true);
        }
        mGrid.Reposition();
    }

    #region 按钮点击
    //关闭按钮点击
    public void OnCloseClick()
    {
        Close();
    }

    //创建按钮点击
    public void OnCreateClick()
    {
        int len = mGrid.transform.childCount;
        int id = 1;//最终选中的id
        int index = 0;
        UIToggle t;
        for(int i = 0; i < len; i++)
        {
            t = mGrid.transform.GetChild(i).GetComponent<UIToggle>();
            if (t.value)
            {
                id = int.Parse(t.name);
                index = i;
                break;
            }
        }
        JinhuaCreateConfig con = mData[index] as JinhuaCreateConfig;
        if (PlayerModel.Inst.UserInfo.gold < con.minScore)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("至少需要" + con.minScore + "金币");
            return;
        }
        SendCreateRoomReq req = new SendCreateRoomReq();
        req.baseScore = con.baseScore;
        req.into = con.minScore;
        req.gameId = eGameType.GoldFlower.GetHashCode();
        Global.Inst.GetController<XXGoldFlowerGameController>().SendCreateRoomReq(req);
    }
    
    #endregion
}
