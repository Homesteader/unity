using UnityEngine;
using System.Collections;

public class XXGlodFlowerBasePlayerInfo : MonoBehaviour
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
    /// 弃牌标志
    /// </summary>
    public GameObject mDisCardSp;

    /// <summary>
    /// 看牌标志
    /// </summary>
    public GameObject mLookCardSp;

    /// <summary>
    /// 比牌选中界面
    /// </summary>
    public GameObject mCompareSp;

    /// <summary>
    /// 倒计时
    /// </summary>
    public GameObject mLastTimeGo;

    /// <summary>
    /// 倒计时 
    /// </summary>
    public UISprite mLastTimeSp;
    public UILabel mInsCountDown;//操作倒计时

    /// <summary>
    /// 显示赢了
    /// </summary>
    public GameObject mWin;

    public UISprite mInsSprite;//操作

    #endregion

    /// <summary>
    /// 是否显示倒计时
    /// </summary>
    private bool mShowLastTime = false;
    /// <summary>
    /// 剩下多少时间
    /// </summary>
    private float mLastTime;
    /// <summary>
    /// 倒计时的总时间
    /// </summary>
    private float mLastAllTime;


    #region Unity

    private void Start()
    {
        string name = this.transform.parent.parent.name;
        if (name == "RightPlayer" || name == "LeftTopPlayer")
        {
            mInsCountDown.transform.parent.localPosition = new Vector3(-107.02f, 7.84f, 0);
        }
        else if(name == "SelfPlayer")
        {
            mInsCountDown.transform.parent.localPosition = new Vector3(214, 105, 0);
        }
        else
        {
            mInsCountDown.transform.parent.localPosition = new Vector3(107.02f, 7.84f, 0);
        }
    }

    private void Update()
    {
        if (mShowLastTime)
        {
            mLastTime -= Time.deltaTime;
            if (mLastTime <= 0)
            {
                mShowLastTime = false;
                mLastTimeGo.gameObject.SetActive(false);
            }
            else
            {
                float rate = mLastTime / mLastAllTime;
                mLastTimeSp.fillAmount = rate;
                mInsCountDown.text = ((int)mLastTime).ToString();
            }
        }
    }

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
    public void InitUI(string head, string name, string uid, float score, bool ready = false, bool offline = false, bool discard = false)
    {
        SetPlayerHead(head);
        SetReadyState(ready);
        SetOffLineState(offline);
        SetDisCardState(discard);
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
        //mReadySp.gameObject.SetActive(show);
    }

    /// <summary>
    /// 设置离线状态
    /// </summary>
    /// <param name="show"></param>
    public void SetOffLineState(bool show)
    {
        mOffLineSp.gameObject.SetActive(show);
    }

    //设置操作气泡
    public void SetInsTips(eGFOptIns ins)
    {
        string str = "";
        switch (ins)
        {
            case eGFOptIns.Add://加注
                str = "state_jiazhu";
                break;
            case eGFOptIns.Follow://跟注
                str = "state_genzhu";
                break;
            case eGFOptIns.Compare://比牌
                str = "state_bipai";
                break;
            case eGFOptIns.DisCard://弃牌
                str = "state_qipai";
                break;
            case eGFOptIns.LookCard://看牌
                str = "state_kanpai";
                break;
        }
        if (string.IsNullOrEmpty(str))
        {
            mInsSprite.gameObject.SetActive(false);
            return;
        }
        mInsSprite.gameObject.SetActive(true);
        if (this.transform.parent.parent.name.Contains("Right"))
        {
            str += "2";
            mInsSprite.transform.localPosition = new Vector3(-65,86,0);
        }
        else
        {
            str += "1";
            mInsSprite.transform.localPosition = new Vector3(65, 86, 0);
        }

        mInsSprite.spriteName = str;
    }

    /// <summary>
    /// 设置弃牌状态
    /// </summary>
    /// <param name="show"></param>
    public void SetDisCardState(bool show)
    {
        if (this.transform.parent.parent.name.Contains("Left"))
        {
            mDisCardSp.transform.localPosition = new Vector3(224, 0, 0);
            //mLookCardSp.transform.localScale = new Vector3(1.4f, 1f, 1.5f);
        }
        else
        if (this.transform.parent.parent.name.Contains("Right"))
        {
            mDisCardSp.transform.localPosition = new Vector3(-212, 0, 0);
            //mLookCardSp.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }


        mDisCardSp.gameObject.SetActive(show);
        if (show)
        {
            mLookCardSp.gameObject.SetActive(false);
        }
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
    /// 设置看牌转改
    /// </summary>
    /// <param name="show"></param>
    public void SetLookCarsState(bool show)
    {
        string name = this.transform.parent.parent.name;
        if (name == "LeftPlayer")
        {
            mLookCardSp.transform.localPosition = new Vector3(148, -141, 0);
        }
        else if (name == "RightPlayer")
        {
            mLookCardSp.transform.localPosition = new Vector3(-134, -141, 0);
        }
        else if (name == "RightTopPlayer" || name == "LeftTopPlayer")
        {
            mLookCardSp.transform.localPosition = new Vector3(0, -129, 0);
        }
        mLookCardSp.gameObject.SetActive(show);
    }

    /// <summary>
    /// 设置可比牌界面
    /// </summary>
    /// <param name="show"></param>
    public void SetCompareState(bool show)
    {
        mCompareSp.gameObject.SetActive(show);
    }

    /// <summary>
    /// 设置倒计时
    /// </summary>
    /// <param name="time"></param>
    public void SetLastTime(float time)
    {
        if (time <= 0)
        {
            mShowLastTime = false;
            mLastTimeGo.gameObject.SetActive(false);
        }
        else
        {
            mLastTimeGo.gameObject.SetActive(true);
            mShowLastTime = true;
            mLastAllTime = time;
            mLastTime = time;
            mInsCountDown.text = ((int)mLastTime).ToString();
        }
    }

    /// <summary>
    /// 设置赢了图
    /// </summary>
    /// <param name="show"></param>
    public void SetWinState(bool show)
    {
        //mWin.gameObject.SetActive(show);
        //if (IsInvoking("DelayHidelWin")) {
        //    CancelInvoke();
        //}
        //Invoke("DelayHidelWin", 2);
    }

    #endregion

    #region 辅助函数

    private void DelayHidelWin()
    {
        mWin.gameObject.SetActive(false);
    }

    #endregion


}
