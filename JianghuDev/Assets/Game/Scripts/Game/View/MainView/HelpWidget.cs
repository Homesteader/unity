using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HelpWidget : BaseViewWidget
{
    public UILabel mHelpText;//帮助文本
    private List<ConfigDada> list;//配置表内容
    private int num;   //帮助内容  0-桂林字牌 1-桂林麻将 2-桂林斗地主

    public UIScrollView mScroll; //框

    public HelpBtnItem mBtnItem;  //按钮预设体
    public UIGrid mBtnGrid;  

    protected override void Awake()
    {
        base.Awake();
        num = 0;
        list = ConfigManager.GetConfigs<HelpConfig>();
        //CreatBtn();
    }

    private void CreatBtn()
    {
        HelpBtnItem ob;
        HelpConfig conData;
        for (int i = 0; i < list.Count; i++)
        {
            conData = list[i] as HelpConfig;
            ob = GameUtils.AddChild<HelpBtnItem>(mBtnItem, mBtnGrid.transform);
            ob.gameObject.SetActive(true);
            ob.InitiaData(conData.index, conData.btnName);
        }
        mBtnGrid.Reposition();
    }

    #region 按钮点击
    /// <summary>
    /// 按钮点击切换
    /// </summary>
    /// <param name="toggle"></param>
    /// <param name="label"></param>
    public void OnToggleChange(UIToggle toggle, UILabel label)
    {
        if (!toggle.value)
            return;
        num = int.Parse(label.text);
        mScroll.ResetPosition();
        //刷新内容
        HelpConfig con1 = list[num] as HelpConfig;
        mHelpText.text = con1.desc;
    }
    
    #endregion

    /// <summary>
    /// 点击关闭
    /// </summary>
    public void OnCloseClick()
    {
        Close<HelpWidget>();
    }
}
