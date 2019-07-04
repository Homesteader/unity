using UnityEngine;
using System.Collections.Generic;

public class RoomRuleWidget : BaseViewWidget
{
    /// <summary>
    /// 规则名称
    /// </summary>
    public UILabel mRuleName;
    /// <summary>
    /// toggle
    /// </summary>
    public GameObject mCheckItem;

    /// <summary>
    /// 输入框
    /// </summary>
    public GameObject mInputItem;

    /// <summary>
    /// 排列
    /// </summary>
    public UIGrid mGrid;
    /// <summary>
    /// 背景
    /// </summary>
    public UISprite mBG;
    /// <summary>
    /// 麻将游戏房间规则 
    /// </summary>
    private CreatRule rule;


    protected override void Awake()
    {
        base.Awake();
    }


    /// <summary>
    /// 创建并更新按钮对象
    /// </summary>
    /// <param name="cr"></param>
    public void SetData(CreatRule cr)
    {
        rule = cr;
        //创建toggle
        NGUITools.DestroyChildren(mGrid.transform);

        int len = mGrid.transform.childCount;

        //规则名称
        mRuleName.text = cr.name + "：";

        if (cr.limit > 0)
        {
            mGrid.maxPerLine = 2;
            mGrid.cellWidth = 450;
        }

        for (int i = 0; i < cr.content.Length; i++)
        {
            if (cr.content[i].Index == 26 || cr.content[i].Index == 27)
            {
                GameObject obj = Assets.InstantiateChild(mGrid.gameObject, mCheckItem.gameObject);
                obj.gameObject.SetActive(false);
                CheckBoxItemWidget item = obj.GetComponent<CheckBoxItemWidget>();
                item.SetData(cr.id, cr.content[i]);
                CreateRoomModel.Inst.RuleItems.Add(item);
            }
            else
           if (cr.content[i].optionType == (int)eToggleType.Input)
            {
                GameObject obj = Assets.InstantiateChild(mGrid.gameObject, mInputItem.gameObject);
                obj.gameObject.SetActive(true);
                RoomRuleInputItem item = obj.GetComponent<RoomRuleInputItem>();
                item.SetData(cr.id, cr.content[i]);
                CreateRoomModel.Inst.InputItems.Add(item);
            }
            else if (CreateRoomModel.Inst.GetConnect(cr.content[i].connect))
            {
                GameObject obj = Assets.InstantiateChild(mGrid.gameObject, mCheckItem.gameObject);
                obj.gameObject.SetActive(true);
                CheckBoxItemWidget item = obj.GetComponent<CheckBoxItemWidget>();
                item.SetData(cr.id, cr.content[i]);
                CreateRoomModel.Inst.RuleItems.Add(item);
            }
        }

        mGrid.Reposition();
        //计算整个规则占用的高度
        Bounds bound = NGUIMath.CalculateRelativeWidgetBounds(mGrid.transform);
        if (bound == null)
            return;
        mBG.height = (int)bound.size.y + 30;
    }

}