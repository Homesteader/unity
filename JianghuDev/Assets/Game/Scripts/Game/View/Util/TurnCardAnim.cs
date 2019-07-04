using UnityEngine;
using System.Collections;

/// <summary>
/// 翻牌动画
/// </summary>
public class TurnCardAnim : MonoBehaviour
{


    #region UI

    /// <summary>
    /// 牌
    /// </summary>
    public UISprite mPaiLabel;

    /// <summary>
    /// 牌背
    /// </summary>
    public GameObject mCardBg;

    /// <summary>
    /// 牌的正面
    /// </summary>
    public GameObject mShowCard;


    #endregion


    public string mCardNum = "";

    #region 外部调用


    /// <summary>
    /// 根据序号设置牌正面深度
    /// </summary>
    /// <param name="index">下标，从0开始</param>
    public void SetCardDeepsByIndex(int index)
    {
        mShowCard.GetComponent<UISprite>().depth = index * 2 + 10;//牌正面
        mPaiLabel.depth = mShowCard.GetComponent<UISprite>().depth + 1;//数字
    }

    /// <summary>
    /// 根据序号设置牌背面深度
    /// </summary>
    /// <param name="index">下标，从0开始</param>
    public void SetCardBgDeepsByIndex(int index)
    {
        mCardBg.GetComponent<UISprite>().depth = index * 2 + 20;
    }

    /// <summary>
    /// 设置牌是否显示
    /// </summary>
    /// <param name="isShow"></param>
    public virtual void SetCardAcive(bool isShow)
    {
        gameObject.SetActive(isShow);
    }


    /// <summary>
    /// true是显示了牌面
    /// </summary>
    /// <returns></returns>
    public bool GetCardShowState() {
        return !mCardBg.gameObject.activeSelf;
    }

    /// <summary>
    /// 设置牌的值
    /// </summary>
    /// <param name="card"></param>
    public virtual void SetCard(string card) {
        mCardNum = card;
        TSTUtil.SetGameCardNum(card, XXGoldFlowerGameModel.Inst.mCardType,
            mShowCard.GetComponent<UISprite>(), mPaiLabel);
    }

    /// <summary>
    /// 翻牌
    /// </summary>
    /// <param name="time">时间,翻牌一次所用时间</param>
    /// <param name="forword">翻牌方向 true是从背翻到值 false是从值翻到牌背</param>
    /// <param name="num">次数</param>
    public virtual void TurnCard(float time, bool forword, int num = 1)
    {
        if (forword)//牌背到牌面的翻牌
        {
            mCardBg.gameObject.SetActive(true);
            mShowCard.gameObject.SetActive(false);
            mCardBg.transform.localEulerAngles = new Vector3(0, 0, 0);
            mShowCard.transform.localEulerAngles = new Vector3(0, 0, 0);
            TweenRotation n2z = mShowCard.GetComponent<TweenRotation>();
            if (n2z != null) {
                Destroy(n2z);
            }
            n2z = mShowCard.AddComponent<TweenRotation>();
            n2z.ResetToBeginning();

            TweenRotation z2n = mCardBg.GetComponent<TweenRotation>();
            if (z2n != null) {
                Destroy(z2n);
            }
            z2n = mCardBg.AddComponent<TweenRotation>();
            z2n.ResetToBeginning();

            z2n.duration = time / 1.0f;
            n2z.duration = time / 1.0f;

            n2z.from = new Vector3(0, 90, 0);
            n2z.to = new Vector3(0, 0, 0);

            z2n.from = new Vector3(0, 0, 0);
            z2n.to = new Vector3(0, 90, 0);

            n2z.AddOnFinished(() =>
            {
                num--;
                if (n2z != null) {
                    Destroy(n2z);
                }
                if (num > 0)
                {
                    TurnCard(time, !forword, num);
                }
            });

            z2n.AddOnFinished(() =>
            {
                mCardBg.gameObject.SetActive(false);
                mShowCard.gameObject.SetActive(true);
                n2z.PlayForward();
                if (z2n != null) {
                    Destroy(z2n);
                }
            });

            z2n.PlayForward();

        }
        else//是从值翻到牌背
        {
            mCardBg.gameObject.SetActive(false);
            mShowCard.gameObject.SetActive(true);
            mCardBg.transform.localEulerAngles = new Vector3(0, 0, 0);
            mShowCard.transform.localEulerAngles = new Vector3(0, 0, 0);

            TweenRotation z2n = mShowCard.GetComponent<TweenRotation>();
            if (z2n != null) {
                Destroy(z2n);
            }
            z2n = mShowCard.AddComponent<TweenRotation>();
            z2n.ResetToBeginning();

            TweenRotation n2z = mCardBg.GetComponent<TweenRotation>();
            if (n2z != null) {
                Destroy(n2z);
            }
            n2z = mCardBg.AddComponent<TweenRotation>();
            n2z.ResetToBeginning();

            z2n.duration = time / 1.0f;
            n2z.duration = time / 1.0f;

            n2z.from = new Vector3(0, 90, 0);
            n2z.to = new Vector3(0, 0, 0);

            z2n.from = new Vector3(0, 0, 0);
            z2n.to = new Vector3(0, 90, 0);

            n2z.AddOnFinished(() =>
            {
                if (n2z != null) {
                    Destroy(n2z);
                }

                num--;
                if (num > 0)
                {
                    TurnCard(time, !forword, num);
                }
            });

            z2n.AddOnFinished(() =>
            {
                mCardBg.gameObject.SetActive(true);
                mShowCard.gameObject.SetActive(false);
                n2z.PlayForward();
                if (z2n != null) {
                    Destroy(z2n);
                }

            });

            z2n.PlayForward();
        }
    }

    /// <summary>
    /// 显示牌面
    /// </summary>
    public virtual void ShowCardNum(string num) {
        SetCard(num);
        mShowCard.gameObject.SetActive(true);
        mCardBg.gameObject.SetActive(false);
        mShowCard.transform.localEulerAngles = new Vector3(0, 0, 0);
        mCardBg.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// 显示牌背
    /// </summary>
    public virtual void ShowCardBg() {
        mShowCard.gameObject.SetActive(false);
        mCardBg.gameObject.SetActive(true);
        mShowCard.transform.localEulerAngles = new Vector3(0, 0, 0);
        mCardBg.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    #endregion
}
