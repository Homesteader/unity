using UnityEngine;
using System.Collections;

public class MJGameSettlementFinalItemView : MonoBehaviour
{
    public UITexture mIcon;//头像
    public UILabel mName;//名字
    public UILabel mId;//ID
    public UILabel mZimo;//自摸次数
    public UILabel mDianPao;//点炮次数
    public UILabel mWinCount;//输赢局数
    public UILabel mScore;//总成绩
    public GameObject mDayingjia;//大赢家标签
    public GameObject mZuijiapaoshou;//最佳炮手标签
    public UISprite mZhuangSprite;//庄家的图标

    private Vector3 mDaYingJiaLocalPos;//大赢家位置
    private Vector3 mZuiJiaPaoLocalPos;//最佳炮手的位置

    private void Awake()
    {
        mDaYingJiaLocalPos = mDayingjia.transform.localPosition;
        mZuiJiaPaoLocalPos = mZuijiapaoshou.transform.localPosition;
    }


    public void SetData(MJGameSettlementFinalPlayerInfo info, bool zuijiapaoshou)
    {
        mDayingjia.transform.localPosition = mDaYingJiaLocalPos;
        mZuijiapaoshou.transform.localPosition = mZuiJiaPaoLocalPos;

        //头像
        Assets.LoadIcon(info.headUrl, (t) => { mIcon.mainTexture = t; });
        //名字
        mName.text = info.nickName;// GameUtils.GetClampText(info.nickName, mName);
        //id
        mId.text = "ID:" + info.userId.ToString();
        //分数
        mScore.text = info.score > 0 ? ("+" + info.score) : info.score.ToString();
        //自摸次数
        //mZimo.text = info.huPai.ToString();
        //点炮次数
        //mDianPao.text = info.dianPao.ToString();
        //输赢 局数
        mWinCount.text = info.winCount.ToString();
        //是否大赢家
        mDayingjia.SetActive(info.isBigwiner);
        //是否最佳炮手
        /*
        mZuijiapaoshou.SetActive(zuijiapaoshou);

        if (info.isBigwiner == false && zuijiapaoshou == true)
        {
            mZuijiapaoshou.transform.localPosition = mDaYingJiaLocalPos;
        }

        if (info.dianPao <= 0) {//无点炮的人不用显示最佳炮手
            mZuijiapaoshou.SetActive(false);
        }
        */
    }
}
