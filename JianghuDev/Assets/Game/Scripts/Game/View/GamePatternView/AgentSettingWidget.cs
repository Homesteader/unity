using UnityEngine;
using System.Collections;

public class AgentSettingWidget : BaseViewWidget {

    public UISlider mSlider;

    public UIInput mInput;

    private bool changInput = false;


    private void Update()
    {
        if (mInput.isSelected)
        {
            changInput = true;
        }
        else {
            changInput = false;
        }
    }

    /// <summary>
    /// 设置抽成比例 0——100
    /// </summary>
    /// <param name="value"></param>
    public void SetValueRate(int value) {
        mSlider.value = (value * 1.0f) / 100;
        mInput.value = value.ToString();
    }

    /// <summary>
    /// 输入框的值改变事件
    /// </summary>
    public void OnInputValueChanged() {
        if (string.IsNullOrEmpty(mInput.value))
        {
            mInput.value = 0 + "";
            mSlider.value = (0 * 1.0f) / 100;
            return;
        }
        int result =0;
        if (!int.TryParse(mInput.value,out result)) {
            mInput.value = 0 + "";
            mSlider.value = (0 * 1.0f) / 100;
            return;
        }
        int value = int.Parse(mInput.value);
        if (value >= 100)
        {
            mInput.value = 100 + "";
            mSlider.value = 1;
        }
        else if (value <= 0)
        {
            mInput.value = 0 + "";
            mSlider.value = 0;
        }
        else
        {
            mSlider.value = (value * 1.0f) / 100;
        }
    }

    /// <summary>
    /// 滚动条改变事件
    /// </summary>
    public void OnSliderValueChanged() {
        if (changInput) {
            return;
        }
        float value = mSlider.value;
        mInput.value = (int)(value * 100)+"";
    }


    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick() {
        Close<AgentSettingWidget>();
    }

    /// <summary>
    /// 提交按钮点击
    /// </summary>
    public void OnSubmitClick() {
        Global.Inst.GetController<GamePatternController>().SendChangChouCheng(int.Parse(mInput.value)*1.0f/100.0f,()=> {
            SetValueRate((int)GamePatternModel.Inst.mChouCheng);
        });
    }

}
