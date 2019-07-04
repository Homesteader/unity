using UnityEngine;
using System.Collections;

public class MJRecordPlayer : BaseViewWidget
{
    public UITexture mHeadIcon;//头像
    public UILabel mName;//名字
    public UILabel mPoint;//积分

    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="data"></param>
    public void SetData(MJRecordPlayerData data)
    {
        //头像
        Assets.LoadTexture(data.headUrl, (t) => { mHeadIcon.mainTexture = t; });
        //名字
        mName.text = data.nickName;
        //积分
        mPoint.text = data.score >= 0 ? ("+" + data.score) : data.score.ToString();
        //积分颜色
        mPoint.color = data.score >= 0 ? NGUIText.ParseColor("d53535") : NGUIText.ParseColor("46a30e");
    }
}
