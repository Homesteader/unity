using UnityEngine;
using System.Collections;

public class HelpBtnItem : MonoBehaviour {

    public UILabel mIndex;   //按钮索引值，通过配置表显示按钮点击后的内容
    public UILabel unSelectL;   //未选中按钮时的按钮文字
    public UILabel selectL;    //选中按钮文字


    /// <summary>
    /// 初始化按钮内容
    /// </summary>
    /// <param name="index"></param>
    /// <param name="btnLabel"></param>
	public void InitiaData(string index,string btnLabel)
    {
        mIndex.text = index;
        unSelectL.text = btnLabel;
        selectL.text = btnLabel;
    }
}
