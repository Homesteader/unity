using UnityEngine;
using System.Collections;

public class GeneralAgentItem : BaseViewWidget
{
    public UILabel mUserId;//玩家id
    public UILabel mUserName;//玩家名字
    public UITexture mUserIcon;//玩家头像
    public UILabel mTodayProfit;//今日收益
    public UILabel mMonthProfit;//本月收益
    public UILabel mLastMonthProfit;//上月收益


    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="data"></param>
    public void SetData(GeneralSubAgentData data)
    {
        //玩家id
        mUserId.text = "ID:" + data.userId;
        //玩家名字
        mUserName.text = data.nickname;
        //玩家头像
        Assets.LoadIcon(data.headUrl, (t) => { mUserIcon.mainTexture = t; });
        //今日收益
        mTodayProfit.text = float.Parse(data.dayProfit.ToString("#0.00")).ToString();
        //本月收益
        mMonthProfit.text = float.Parse(data.monthProfit.ToString("#0.00")).ToString();
        //上月收益
        mLastMonthProfit.text = float.Parse(data.lastMonthProfit.ToString("#0.00")).ToString();
    }

}
