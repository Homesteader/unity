using UnityEngine;
using System.Collections;

public class XXGoldFlowerXiQianWidget : BaseViewWidget {

    public TurnCardAnim[] mHandCards;

    //public UISprite mCardType;

    public UILabel mXides;


    public void Show(XXGlodFlowerPlayer player,float num) {
        eGFCardType type = player.GetCardType();
        if (type != eGFCardType.Nil)
        {
            //mCardType.gameObject.SetActive(true);
            //mCardType.spriteName = "jinhua_" + (int)type;
        }
        else
        {
            //mCardType.gameObject.SetActive(false);
        }

        for (int i = 0; i < mHandCards.Length; i++)
        {
            if (player.GetHandCard().Count > i)
            {
                mHandCards[i].SetCard(player.GetHandCard()[i]);
            }
        }

        mXides.text = "吃喜了！金币+"+num + "个";
        DelayRun(4, () =>
        {
            Close<XXGoldFlowerXiQianWidget>();
        });
    }

}
