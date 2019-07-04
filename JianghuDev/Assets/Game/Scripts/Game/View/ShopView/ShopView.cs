using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopView : BaseView
{
    public UIGrid[] mCardGrid;//房卡grid  第一个是房卡，第二个是金币
    public GameObject mCardItem;//房卡item 
    public GameObject[] mCardRoot;//房卡root   第一个是房卡，第二个是金币
    
    public UILabel mPeachNum;//金币数量
    public UILabel mCardNum;//房卡数量

    private bool[] mIsInit = new bool[] { false, false };//是否初始化，第一个是房卡，第二个是金币

    protected override void Awake()
    {
        base.Awake();
        AddEventListenerTarget(GlobalEvent.inst, eEvent.UPDATE_PROP, OnUpdateProp);
    }

    private void OnUpdateProp(params object[] args)
    {
        mCardNum.text = PlayerModel.Inst.UserInfo.roomCard.ToString();//房卡
        mPeachNum.text = PlayerModel.Inst.UserInfo.gold.ToString();//金币
    }

    #region 初始化
  

    /// <summary>
    /// 初始化房卡
    /// </summary>
    public void InitCard(int index)
    {
        if (mIsInit[index])
            return;
        mCardNum.text = PlayerModel.Inst.UserInfo.roomCard.ToString();//房卡
        mPeachNum.text = PlayerModel.Inst.UserInfo.gold.ToString();//金币
        mIsInit[index] = true;
        RechargeConfig con = ConfigManager.GetConfigs<RechargeConfig>()[index] as RechargeConfig;
        List<RechargeData> data = con.CardConfig;
        InitItems(mCardGrid[index], mCardRoot[index], mCardItem, data);
    }

    /// <summary>
    /// 初始化item
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="item"></param>
    /// <param name="data"></param>
    private void InitItems(UIGrid grid, GameObject root, GameObject item, List<RechargeData> data)
    {
        GameObject obj;
        root.SetActive(true);
        grid.gameObject.SetActive(true);
        for (int i = 0; i < data.Count; i++)
        {
            obj = NGUITools.AddChild(grid.gameObject, item);
            obj.SetActive(true);
            obj.GetComponent<RechargeItem>().SetData(data[i]);
        }
        grid.Reposition();
    }

    #endregion
  

    #region 按钮点击

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick()
    {
        Close();
    }
    #endregion
}
