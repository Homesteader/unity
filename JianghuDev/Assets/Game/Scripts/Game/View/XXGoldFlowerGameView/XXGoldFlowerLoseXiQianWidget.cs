using UnityEngine;
using System.Collections;

public class XXGoldFlowerLoseXiQianWidget : BaseViewWidget {

    public TurnCardAnim[] mHandCards;

    public UISprite mCardType;

    public UILabel mWinPlayerNameLabel;

    public UILabel mXides;


    public void Show(XXGlodFlowerPlayer player, float num, GoldFlowerCardsInfo cardsInfo) {
        eGFCardType type = (eGFCardType)cardsInfo.cardType;
        if (type != eGFCardType.Nil)
        {
            mCardType.gameObject.SetActive(true);
            mCardType.spriteName = "jinhua_" + (int)type;
        }
        else
        {
            mCardType.gameObject.SetActive(false);
        }

        for (int i = 0; i < mHandCards.Length; i++)
        {
            mHandCards[i].SetCard(cardsInfo.card[i]);
        }

        mWinPlayerNameLabel.text = XXGoldFlowerGameModel.Inst.mPlayerInfoDic[cardsInfo.seatId].nickname + "拿到";

        mXides.text = "被吃喜了！金币-"+num + "个";
        DelayRun(4, () =>
        {
            Close<XXGoldFlowerLoseXiQianWidget>();
        });
    }

}
