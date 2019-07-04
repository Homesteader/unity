using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 搓牌界面
/// </summary>
public class NiuNiuCuoCardWidget : BaseViewWidget {

    /// <summary>
    /// 取消按钮
    /// </summary>
    public UIButton mCancelBtn;

    /// <summary>
    /// 普通搓牌go
    /// </summary>
    public GameObject mNormalGo;

    /// <summary>
    /// 普通搓牌的手牌
    /// </summary>
    public NiuniuHandCard[] normalHandCards;

    /// <summary>
    /// 明牌搓牌go
    /// </summary>
    public GameObject mMinglGo;

    /// <summary>
    /// 明牌搓牌的手牌
    /// </summary>
    public NiuniuHandCard[] mingHandCards;

    /// <summary>
    /// 动画结束的回调
    /// </summary>
    private Action mCallBack;

    /// <summary>
    /// 总共需要移动的牌的张数
    /// </summary>
    private int mTotalCardsNum = 0;

    /// <summary>
    /// 计数器
    /// </summary>
    private int mNum = 0;

    /// <summary>
    /// 是否是明牌搓牌
    /// </summary>
    private bool mMing = false;

    /// <summary>
    /// 显示搓牌
    /// </summary>
    /// <param name="mingCard">是否是明牌</param>
    /// <param name="cards">手牌</param>
    /// <param name="callBaclk">结束回调</param>
    public void ShowCuoCards(bool mingCard,List<string> cards,Action callBaclk) {
        mCallBack = callBaclk;
        mTotalCardsNum = cards.Count;
        mNum = 0;

        if (mingCard) {//是明牌搓牌
            mMing = true;
            mMinglGo.gameObject.SetActive(true);
            mNormalGo.gameObject.SetActive(false);
            mingHandCards[mingHandCards.Length - 1].SetMove(-130, 130, 120, -120, CardMoveCallBack);
            for (int i=0;i<cards.Count;i++) {
                mingHandCards[i].SetCard(cards[i]);
                mingHandCards[i].ShowCardNum(cards[i]);
            }
        }
        else {
            mMing = false;
            mMinglGo.gameObject.SetActive(false);
            mNormalGo.gameObject.SetActive(true);
            for (int i=1;i< normalHandCards.Length;i++) {
                normalHandCards[i].SetMove(-130, 130, 120, -120, CardMoveCallBack);
            }
            for (int i = 0; i < cards.Count; i++)
            {
                normalHandCards[i].SetCard(cards[i]);
                normalHandCards[i].ShowCardNum(cards[i]);
            }
        }

    }

    /// <summary>
    /// 每张牌移动结束的回调
    /// </summary>
    private void CardMoveCallBack(GameObject go) {
        mNum++;
        TweenColor color = go.GetComponent<TweenColor>();
        if (color!=null) {
            color.PlayForward();
        }
        if (mNum >= mTotalCardsNum) {
            mCancelBtn.gameObject.SetActive(false);
            StartCoroutine(MoveDown());
        }
    }


    private IEnumerator MoveDown() {
        float time = .5f;
        if (!mMing)
        {
            time = 0.7f;
            for (int i = 0; i < normalHandCards.Length; i++)
            {
                normalHandCards[i].GetComponent<BoxCollider>().enabled = false;
                TweenPosition position = normalHandCards[i].GetComponent<TweenPosition>();
                if (position != null)
                {
                    position.from = normalHandCards[i].transform.localPosition;
                    position.PlayForward();
                }

                TweenRotation rotation = normalHandCards[i].GetComponent<TweenRotation>();
                if (rotation != null)
                {
                    rotation.from = normalHandCards[i].transform.localRotation.eulerAngles;
                    rotation.PlayForward();
                }
            }
        }
        yield return new WaitForSeconds(time);
        if (mCallBack!=null) {
            mCallBack();
        }
        OnCloseClick();
    }


    public void OnCloseClick() {
        Close<NiuNiuCuoCardWidget>();
    }
}
