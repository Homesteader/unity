using UnityEngine;
using System.Collections;

public class RankItem : BaseViewWidget
{
    public UILabel mRankL;//后三名以后的显示
    public UISprite mRankS;//前三名的显示
    public UITexture mIcon;//头像
    public UILabel mName;//名字
    public UILabel mUserId;//玩家id
    public UILabel mNum;//数值
    public UILabel mWechat;//微信
    public UIGrid grid;
    public GameObject mPoint;//积分/金币
    public UISprite mBg;//背景


    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="data"></param>
    public void SetData(SendGetRankInfo data, int index, int type)
    {
        //排名
        switch (index)
        {
            case 1://
                mRankL.gameObject.SetActive(false);
                mRankS.gameObject.SetActive(true);
                mRankS.spriteName = "rank_gold";
                break;
            case 2:
                mRankL.gameObject.SetActive(false);
                mRankS.gameObject.SetActive(true);
                mRankS.spriteName = "rank_silver";
                break;
            case 3:
                mRankL.gameObject.SetActive(false);
                mRankS.gameObject.SetActive(true);
                mRankS.spriteName = "rank_bronz";
                break;
            default:
                mRankL.gameObject.SetActive(true);
                mRankS.gameObject.SetActive(false);
                mRankL.text = index.ToString();
                break;
        }
        //背景
        if (data.userId == PlayerModel.Inst.UserInfo.userId)//自己
        {
            mBg.spriteName = "bg2";
        }
        else
        {
            mBg.spriteName = "bg_1";
        }
        //排名
        mRankL.text = index.ToString();
        //头像
        Assets.LoadIcon(data.headUrl, (t) => { mIcon.mainTexture = t; });
        //名字
        mName.text = data.nickName;
        if (!string.IsNullOrEmpty(data.weChat))
        {
            mWechat.gameObject.SetActive(true);
            mWechat.text = data.weChat;
        }
        else
        {
            mWechat.gameObject.SetActive(false);
        }
        //玩家id
        //mUserId.text = data.ID;
        //数值
        string titleStr;
        if (type == 1)
        {//金币排行
            mNum.text = data.gold.ToString();
            titleStr = "金币：";
        }
        else
        {
            //积分排行
            mNum.text = data.score.ToString();
            titleStr = "积分：";
        }
        mPoint.SetActive(true);
        mPoint.gameObject.GetComponent<UILabel>().text = titleStr;
        grid.enabled = true;
        grid.Reposition();
    }
}
