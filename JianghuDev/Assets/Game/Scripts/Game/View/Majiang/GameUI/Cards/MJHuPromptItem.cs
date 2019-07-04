using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJHuPromptItem : MonoBehaviour
{
    public UISprite mCardSpr;//牌数字
    public UILabel mBet;//番数
    public UILabel mCount;//剩余张数
    /// <summary>
    /// 设置提示的牌
    /// </summary>
    public void SetCard(CanHuStruct canHuCard)
    {
        mCardSpr.spriteName = "top_" + canHuCard.card;
        mBet.text = canHuCard.multiple + "翻";
        mCount.text = canHuCard.leaveCardCount + "张";
    }
}
