using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJGameSettlementItem : BaseViewWidget
{
    public UITexture mIcon;//头像
    public UILabel mName;//名字
    public UILabel mId;//id
    public UILabel mPoint;//分数
    public UILabel mHuType;//胡牌类型
    public UITable mCardTable;//牌table
    public GameObject mTypeItem;//胡牌类型item
    public GameObject mCardNormalItem;//3张牌
    public GameObject mThreeLie;//3张平躺的牌
    public GameObject mCardOneItem;//1张牌
    public GameObject mCardGang;//四张牌
    public UIGrid mHandCard;//手牌
    public UISprite mZhuangSprite;//庄家图标
    public GameObject mBigWinner;//大赢家
    public UISprite mHuOrder;//胡牌顺序

    private int mMaxFanshu;//封顶番数


    public void SetData(MJGameSettlementPlayerInfo info, int maxFanshu, bool isBigWinner)
    {
        mMaxFanshu = maxFanshu;

        {
            Assets.LoadIcon(info.headUrl, (t) =>
            {
                mIcon.mainTexture = t;
            });
        }

        if (info.isZhuang == true)
        {
            if (mZhuangSprite != null)
            {
                mZhuangSprite.gameObject.SetActive(true);
            }
        }
        else
        {
            if (mZhuangSprite != null)
            {
                mZhuangSprite.gameObject.SetActive(false);
            }
        }

        //名字
        //mName.text = GameUtils.GetClampText(info.nickName, mName);
        mName.text = info.nickName;
        //id
        mId.text = info.userId.ToString();
        //分数
        mPoint.text = info.score >= 0 ? ("+" + info.score) : info.score.ToString();
        mPoint.color = info.score < 0 ? NGUIText.ParseColor("d53535") : NGUIText.ParseColor("46a30e");
        //胡牌细节
         mHuType.text = info.huDes;
        //牌型
        SetCard(info);
        //大赢家
        mBigWinner.SetActive(isBigWinner);
        //胡牌顺序
        if(info.huOrder > 0 && info.huOrder < 5)
        {
            mHuOrder.spriteName = "hu_" + info.huOrder;
            DelayRun((info.huOrder - 1) * 0.2f, () =>
            {
                mHuOrder.gameObject.SetActive(true);
            });
        }
    }

    private void SetCard(MJGameSettlementPlayerInfo info)
    {
        GameUtils.ClearChildren(mCardTable.transform);
        //杠
        CreateCardList(info.gang, eMJInstructionsType.GANG);
        //吃
        //CreateCardList(info.chi, eMJInstructionsType.CHI);
        //碰
        if (info.peng != null)
        {
            for (int i = 0; i < info.peng.Count; i++)
                InitPengItems(mThreeLie, info.peng[i].cards[0], info.peng[i].cards[0], info.peng[i].cards[0]);
        }
        //亮
        //CreateCardList(info.liang, eMJInstructionsType.LIANG);
        //手牌
        CreateHandCard(info.shoupai);
        //胡牌
        if (info.huPai != null && info.huPai.Count > 0)
        {
            Dictionary<int, UILabel> huList = new Dictionary<int, UILabel>();
            for (int i = 0; i < info.huPai.Count; i++)
            {
                if (huList.ContainsKey(info.huPai[i].card))
                {
                    int count = int.Parse(huList[info.huPai[i].card].text) + 1;
                    huList[info.huPai[i].card].text = count.ToString();
                    huList[info.huPai[i].card].gameObject.SetActive(true);
                }
                else
                {
                    GameObject obj = NGUITools.AddChild(mCardTable.gameObject, mCardOneItem);
                    obj.SetActive(true);
                    obj.transform.GetChild(0).GetComponent<UISprite>().spriteName = "top_" + info.huPai[i].card;
                    huList.Add(info.huPai[i].card, obj.transform.GetChild(1).GetComponent<UILabel>());
                    obj.transform.GetChild(2).gameObject.SetActive(true);
                }
            }
        }
        mCardTable.Reposition();
    }

    private void CreateCardList(List<GangStruct> list, eMJInstructionsType _type)
    {
        if (list == null)
            return;
        for (int i = 0; i < list.Count; i++)
            CreateItems(list[i], _type);
    }

    private void CreateItems(GangStruct card, eMJInstructionsType _type)
    {
        if (card == null || card.cards == null)
            return;
        if (_type == eMJInstructionsType.GANG)//杠
        {
            if (card.gangType == eGangType.ANGANG)//暗杠
                InitAngang(mCardGang, card.cards[0]);
            else
                InitPengItems(mCardGang, card.cards[0], card.cards[0], card.cards[0], card.cards[0]);
        }
        else
        {
            int len = card.cards.Count;
            if (len == 3)
                InitPengItems(mThreeLie, card.cards[0], card.cards[1], card.cards[2]);
            else if (len == 4)
                InitPengItems(mCardGang, card.cards[0], card.cards[1], card.cards[2], card.cards[3]);
        }
    }

    private void InitPengItems(GameObject item, int a, int b, int c, int d = 0)
    {
        Transform tran = NGUITools.AddChild(mCardTable.gameObject, item).transform;
        tran.gameObject.SetActive(true);
        tran.GetChild(0).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + a;
        tran.GetChild(1).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + b;
        tran.GetChild(2).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + c;
        if (d != 0)
            tran.GetChild(3).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + d;
    }

    private void InitAngang(GameObject item, int a)
    {
        Transform tran = NGUITools.AddChild(mCardTable.gameObject, item).transform;
        tran.gameObject.SetActive(true);
        tran.GetChild(0).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + a;
        tran.GetChild(1).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + a;
        tran.GetChild(2).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + a;
        tran.GetChild(3).gameObject.SetActive(false);
        tran.GetChild(4).gameObject.SetActive(true);
    }

    private void CreateHandCard(List<int> card)
    {
        if (card == null)
            return;
        GameObject obj = NGUITools.AddChild(mCardTable.gameObject, mHandCard.gameObject);
        obj.SetActive(true);
        GameObject item;
        for (int i = 0; i < card.Count; i++)
        {
            item = NGUITools.AddChild(obj, mCardOneItem);
            item.SetActive(true);
            item.transform.GetChild(0).GetComponent<UISprite>().spriteName = "top_" + card[i];
        }
        obj.GetComponent<UIGrid>().Reposition();
    }


}
