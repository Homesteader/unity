using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJInsCardItem : MonoBehaviour
{
    public UIGrid mGrid;
    public GameObject[] mCardObj;//牌
    public UISprite[] mCardSpr;//牌数字
    private int cardNum;
    public UIWidget mWidget;
    private CallBack<OptItemStruct, MJliangcard> mCallback;
    private OptItemStruct mOpt;//操作
    private MJliangcard mCard;//操作的牌

    public void SetCards(OptItemStruct optList, MJliangcard cards, CallBack<OptItemStruct, MJliangcard> callback)
    {
        mOpt = optList;
        MJliangcard gangCards = new MJliangcard();
        if (gangCards.card == null)
            gangCards.card = new List<int>();
        gangCards.card.AddRange(cards.card);
        mCard = gangCards;
        cardNum = gangCards.card[0];
        //mCard = cards;
        mCallback = callback;
        for (int i = 0; i < gangCards.card.Count; i++)
        {
            mCardObj[i].SetActive(true);
           
            mCardSpr[i].spriteName = "top_" + gangCards.card[i];
        }
        for (int i = gangCards.card.Count; i < mCardSpr.Length; i++)
            mCardObj[i].SetActive(false);
        mGrid.Reposition();
        mWidget.width = (int)mGrid.cellWidth * gangCards.card.Count;
    }


    public void OnSelfClick()
    {
        //if (mCallback != null)
        //   mCallback(mOpt, mCard);


        OptRequest req = new OptRequest();
        req.ins = eMJInstructionsType.GANG;
        req.cards = new List<int>();
        req.cards.Add(cardNum);
        Global.Inst.GetController<MJGameController>().SendInstructions(req, null);
    }
}
