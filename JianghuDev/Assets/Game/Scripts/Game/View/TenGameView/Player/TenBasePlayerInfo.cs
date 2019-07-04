using UnityEngine;
using System.Collections;

public class TenBasePlayerInfo : MonoBehaviour
{


    #region UI

    /// <summary>
    /// 玩家头像
    /// </summary>
    public UITexture mHeadTexture;

    /// <summary>
    /// 玩家昵称
    /// </summary>
    public UILabel mNickName;

    /// <summary>
    /// 金币或者积分数量
    /// </summary>
    public UILabel mGoldScore;

    /// <summary>
    /// 准备标志
    /// </summary>
    public GameObject mReadySp;

    /// <summary>
    /// 离线标志
    /// </summary>
    public GameObject mOffLineSp;

    /// <summary>
    /// 庄家图标
    /// </summary>
    public GameObject mZhuangSp;

    /// <summary>
    /// 随机庄动画
    /// </summary>
    public GameObject mRandomZhuangAnim;

    #endregion

    #region 外部调用

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="head"></param>
    /// <param name="name"></param>
    /// <param name="uid"></param>
    /// <param name="score"></param>
    /// <param name="ready"></param>
    /// <param name="offline"></param>
    /// <param name="discard"></param>
    public void InitUI(string head, string name, string uid, float score, bool ready = false, bool offline = false)
    {
        SetPlayerHead(head);
        SetReadyState(ready);
        SetOffLineState(offline);
        SetUserName(name);
        SetUserScore(score);
    }

    /// <summary>
    /// 设置玩家头像
    /// </summary>
    /// <param name="url"></param>
    public void SetPlayerHead(string url)
    {
        Assets.LoadIcon(url, (t) =>
        {
            mHeadTexture.mainTexture = t;
        });
    }

    /// <summary>
    /// 设置准备状态
    /// </summary>
    /// <param name="show"></param>
    public void SetReadyState(bool show)
    {
        mReadySp.gameObject.SetActive(show);
    }

    /// <summary>
    /// 设置离线状态
    /// </summary>
    /// <param name="show"></param>
    public void SetOffLineState(bool show)
    {
        mOffLineSp.gameObject.SetActive(show);
    }


    /// <summary>
    /// 设置庄家状态
    /// </summary>
    /// <param name="show"></param>
    public void SetZhuangState(bool show)
    {
        mZhuangSp.gameObject.SetActive(show);
    }

    /// <summary>
    /// 设置玩家名字
    /// </summary>
    /// <param name="name"></param>
    public void SetUserName(string name)
    {
        mNickName.text = name;
    }

    /// <summary>
    /// 设置积分或者金币
    /// </summary>
    /// <param name="score"></param>
    public void SetUserScore(float score)
    {
        mGoldScore.text = score.ToString();
    }

    /// <summary>
    /// 得到金币或者积分数量
    /// </summary>
    /// <returns></returns>
    public float GetUserScore() {
        return float.Parse(mGoldScore.text);
    }

    /// <summary>
    /// 设置庄家动画
    /// </summary>
    /// <param name="show"></param>
    public void SetRandomZhuangAnimState(bool show) {
        mRandomZhuangAnim.gameObject.SetActive(show);
    }

    /// <summary>
    /// 得到庄家图标位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetZhuangPosition() {
        return mZhuangSp.transform.position;
    }

    #endregion
}
