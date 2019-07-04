using UnityEngine;
using System.Collections;

public class RoomRuleInputItem : MonoBehaviour {

    public UILabel mName;

    public UIInput mInput;


    private CreatRuleData ruledata;  //规则数据
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
        mName.text = ruledata.name;
        mInput.value = data.value+"";
    }

    /// <summary>
    /// 回去上一级id 如果是未选中就返回-1
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
        return ruledata.Index;
    }

    /// <summary>
    /// 获取当前类型
    /// </summary>
    /// <returns></returns>
    public eToggleType GetOptType() {
        return (eToggleType)ruledata.optionType;
    }

    /// <summary>
    /// 获取输入的值
    /// </summary>
    /// <returns></returns>
    public float GetInputValue() {
        return float.Parse(mInput.value);
    }
}
