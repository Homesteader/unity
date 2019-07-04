using UnityEngine;
using System.Collections;

public class CheckBoxItemWidget : BaseViewWidget
{
    public Color mNormalColor;//这场颜色
    public Color mCheckedColor;//选中的颜色
    public UISprite mBackGround;   //按钮背景
    public UISprite mCheckMark;    //按钮切换图
    public UILabel mLabelName;    //按钮显示名字
    public UIToggle toggle;     //
    private CreatRuleData ruledata;  //规则数据
    private CallBack<int, bool> callback;   //回调 参数1. id 2. 选择值
    private int mRootId;//上一级id
    
    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="rootId">上一级id</param>
    /// <param name="data">配置表</param>
    public void SetData(int rootId, CreatRuleData data)
    {
        mRootId = rootId;
        this.ruledata = data;
        mLabelName.text = ruledata.name;
        eToggleType _type = (eToggleType)data.optionType;

        if (_type == eToggleType.toggle)//单选框
        {
            mBackGround.spriteName = "toggle_yuan_di";
            mCheckMark.spriteName = "toggle_yuan";
            mCheckMark.MakePixelPerfect();
            transform.localScale = Vector3.one * 1f;
        }
        else//复选框
        {
            mBackGround.spriteName = "toggle_gou_di";
            mCheckMark.spriteName = "toggle_gou";
            mCheckMark.MakePixelPerfect();
            mCheckMark.transform.localPosition = new Vector3(0, -8, 0);//对勾下移  一点
            transform.localScale = Vector3.one *1f;
        }

        if (data.none)
        {
            toggle.optionCanBeNone = true;
        }
        else {
            toggle.optionCanBeNone = false;
        }

        toggle.group = data.group;
        if (data.isSelected)
            toggle.value = true;
        else
            toggle.Set(false);
    }


    public void OnClick() {
        UILabel label = gameObject.GetComponentInChildren<UILabel>();
        if (toggle.value)
        {
            if (label!=null) {
                label.color = mCheckedColor;
            }
        }
        else {
            if (label != null)
            {
                label.color = mNormalColor; ;
            }
        }
    }

    /// <summary>
    /// 回去上一级id
    /// </summary>
    /// <returns></returns>
    public int GetRootId()
    {
        return mRootId;
    }

    /// <summary>
    /// 获取当前规则的id
    /// </summary>
    /// <returns></returns>
    public int GetIndex()
    {
        if (!toggle.value)//如果是未选中就返回-1
            return -1;
        return ruledata.Index;
    }

    /// <summary>
    /// 获取当前类型
    /// </summary>
    /// <returns></returns>
    public eToggleType GetOptType()
    {
        return (eToggleType)ruledata.optionType;
    }

}
