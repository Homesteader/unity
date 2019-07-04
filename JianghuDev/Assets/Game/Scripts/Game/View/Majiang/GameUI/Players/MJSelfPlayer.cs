using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJSelfPlayer : MJPlayerBase
{
    public UIGrid mInstructionsGrid;//操作指令grid
    public MJInstructionsItem[] mInstructionsItems;//可操作的指令
    public GameObject mSpeciaInstructionRoot;
    public UITable mSpeciaInstructionTable;//特殊操作table
    public MJInsCardItem mInsCardItem;//特殊操作item
    public UIWidget mWideget;
    public GameObject mChiTipsObj;//吃显示的提示
    public GameObject mPromptObj;//胡牌列表
    public UIGrid mPromptGrid;//胡牌grid
    public UITexture mPromptBg1;//胡牌背景1
    public UISprite mProptBg2;//胡牌bg2
    public MJHuPromptItem mHuPromptItem;//item 
    public GameObject mTrustShipRoot;//托管root


    private void Awake()
    {
    }

    public override void ResetUI(bool depth = false)
    {
        base.ResetUI(depth);
        for (int i = 0; i < mInstructionsGrid.transform.childCount; i++)
        {
            mInstructionsGrid.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < mInstructionsItems.Length; i++)
        {
            if (mInstructionsItems[i] != null)
                mInstructionsItems[i].gameObject.SetActive(false);
        }
        if (mSpeciaInstructionRoot != null)
        {
            mSpeciaInstructionRoot.SetActive(false);
        }
        mTrustShipRoot.SetActive(false);
    }

    public void SetData(MJPlayerInfo info)
    {

    }

    #region 特殊操作
    /// <summary>
    /// 设置特殊操作
    /// </summary>
    /// <param name="info"></param>
    public void SetSpeciaInstructions(OptItemStruct info)
    {
        mSpeciaInstructionRoot.SetActive(true);
        List<MJliangcard> list = new List<MJliangcard>();
        mChiTipsObj.SetActive(false);
        /*
        if (info.ins == eMJInstructionsType.baojiao)//报叫
        {
            mChiTipsObj.SetActive(true);
            for (int i = 0; i < info.card.Count; i++)
            {
                MJliangcard lc = new MJliangcard();
                List<int> l = new List<int>();
                l.Add(info.card[i]);
                lc.card = l;
                list.Add(lc);
            }
            MJGameModel.Inst.mIsTing = true;//听牌状态
        }
        else if (info.ins == eMJInstructionsType.chi)//吃
        {
            
            list = GetChiCards();
        }
        */
        if (info.ins == eMJInstructionsType.GANG)//杠
        {
            for (int i = 0; i < info.cards.Count; i++)
            {
                MJliangcard lc = new MJliangcard();
                List<int> l = new List<int>();
                for (int j = 0; j < 4; j++)
                    l.Add(info.cards[i]);
                lc.card = l;
                list.Add(lc);
            }
        }
        InitSpecia(info, list);
    }

    private void InitSpecia(OptItemStruct info, List<MJliangcard> cards)
    {
        int len = cards.Count;
        int count = mSpeciaInstructionTable.transform.childCount;
        MJInsCardItem item;
        for (int i = 0; i < len; i++)
        {
            if (i < count)
                item = mSpeciaInstructionTable.transform.GetChild(i).GetComponent<MJInsCardItem>();
            else
                item = NGUITools.AddChild(mSpeciaInstructionTable.gameObject, mInsCardItem.gameObject).GetComponent<MJInsCardItem>();
            item.gameObject.SetActive(true);
            item.SetCards(info, cards[i], OnSpeciaInsItemCall);
            //item.SetCards(info, cards[i], null);
        }
        for (int i = len; i < count; i++)
            mSpeciaInstructionTable.transform.GetChild(i).gameObject.SetActive(false);
        mSpeciaInstructionTable.Reposition();
        mSpeciaInstructionTable.gameObject.SetActive(true);
        Bounds w = NGUIMath.CalculateRelativeWidgetBounds(mSpeciaInstructionTable.transform);
        mWideget.width = (int)w.size.x + 50;
    }

    /// <summary>
    /// 特殊操作点击返回
    /// </summary>
    /// <param name="opt"></param>
    /// <param name="cards"></param>
    public void OnSpeciaInsItemCall(OptItemStruct opt, MJliangcard cards)
    {
        ResetUI();
        OptRequest req = new OptRequest();
        if (opt.ins == eMJInstructionsType.GANG)
        {
            req.ins = eMJInstructionsType.GANG;
            cards.card = new List<int>();
        }
        //else if (opt.ins == eMJInstructionsType.chi)
        //    opt.type = cards.type;
        opt.cards = cards.card;
        req.cards.Add(cards.card[0]);
        Global.Inst.GetController<MJGameController>().SendInstructions(req, null);
    }



    #region 吃

    public List<MJliangcard> GetChiCards()
    {
        List<MJliangcard> cards = new List<MJliangcard>();
        int card = MJGameModel.Inst.mLastOutCard;
        int myseat = MJGameModel.Inst.mMySeatId;
        List<int> handcard = MJGameModel.Inst.allPlayersCardsInfoStruct[myseat].handList;
        if (handcard.Contains(card - 2) && handcard.Contains(card - 1) && card % 10 > 2)//有两张左边的牌，吃的牌是3，有1和2 后
        {
            MJliangcard lc = new MJliangcard();
            List<int> c = new List<int>();
            c.Add(card - 2);
            c.Add(card - 1);
            c.Add(card);
            lc.card = c;
            lc.type = (int)eMJChiType.HOU;
            cards.Add(lc);
        }
        if (handcard.Contains(card - 1) && handcard.Contains(card + 1) && card % 10 > 1)//有左右边的牌，吃的牌是3，有2和4 中
        {
            MJliangcard lc = new MJliangcard();
            List<int> c = new List<int>();
            c.Add(card - 1);
            c.Add(card);
            c.Add(card + 1);
            lc.card = c;
            lc.type = (int)eMJChiType.ZHONG;
            cards.Add(lc);
        }
        if (handcard.Contains(card + 1) && handcard.Contains(card + 2) && card > 0)//有两张左边的牌，吃的牌是3，有4和5 前
        {
            MJliangcard lc = new MJliangcard();
            List<int> c = new List<int>();
            c.Add(card);
            c.Add(card + 1);
            c.Add(card + 2);
            lc.card = c;
            lc.type = (int)eMJChiType.QIAN;
            cards.Add(lc);
        }
        return cards;
    }
    #endregion
    #endregion
    #region 显示可操作的指令
    /// <summary>
    /// 显示可操作指令
    /// </summary>
    /// <param name="info"></param>
    public void ShowInstructions(List<OptItemStruct> info)
    {
        for (int i = 0; i < mInstructionsItems.Length; i++)
        {
            if (mInstructionsItems[i] != null)
                mInstructionsItems[i].gameObject.SetActive(false);
        }
        if (info == null)
            return;
        for (int i = 0; i < info.Count; i++)
        {
            if ((int)info[i].ins < 15)
            {
                if ((int)info[i].ins == 12)
                {
                    MJGameModel.Inst.isMyHit = true;
                }
                else
                {
                    if (mInstructionsItems[(int)info[i].ins - 1] != null)
                        mInstructionsItems[(int)info[i].ins - 1].SetOpt(info[i], this);
                }
            }
        }
        int myseat = MJGameModel.Inst.mMySeatId - 1;
        //myInfo m = GameModel.Inst.mPlayerdata[myseat].mHandCard;
        //if (m != null && m.currCard == 0)//有牌要打出时
        //    mPlayer.SetOneOutCardPos();
        mInstructionsGrid.Reposition();

    }
    #endregion

    #region 胡牌列表

    /// <summary>
    /// 设置 可以胡的牌的 番数 和 张数
    /// </summary>
    public override void SetHuPromptCard(List<CanHuStruct> canHuList, Vector3 vec3, bool isShow)
    {
        base.SetHuPromptCard(canHuList, vec3, isShow);
        if (!isShow)
        {
            mPromptObj.SetActive(false);
            return;
        }
        int count = 0;
        if (canHuList != null && canHuList.Count > 0)
            count = canHuList.Count;
        mPromptObj.SetActive(true);
        if(count == 0)
        {
            mPromptObj.SetActive(false);
            return;
        }
        int len = mPromptGrid.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            MJHuPromptItem card;
            if (i < len)
            {
                card = mPromptGrid.transform.GetChild(i).GetComponent<MJHuPromptItem>();
            }
            else
            {
                card = GameObject.Instantiate(mHuPromptItem, mPromptGrid.transform) as MJHuPromptItem;
            }
            card.gameObject.SetActive(true);
            card.SetCard(canHuList[i]);
        }
        for (int i = count; i < len; i++)
        {
            mPromptGrid.transform.GetChild(i).gameObject.SetActive(false);
        }
        mPromptGrid.Reposition();
        //mPromptGrid.transform.localPosition = vec3;

        mPromptBg1.width = (int)mPromptGrid.cellWidth * count + 73;
        mProptBg2.width = mPromptBg1.width - 15;
    }

    #endregion


    #region 托管
    /// <summary>
    /// 取消托管点击
    /// </summary>
    public void OnCancelTrustClick()
    {
        OptRequest req = new OptRequest();
        req.ins = eMJInstructionsType.TG;
        req.type = eMJTrusteeshipType.cancelTrust.GetHashCode();//取消托管
        Global.Inst.GetController<MJGameController>().SendInstructions(req, null);
    }

    /// <summary>
    /// 设置托管是否显示
    /// </summary>
    /// <param name="isshow"></param>
    public override void SetTrustShipTagShow(bool isshow)
    {
        base.SetTrustShipTagShow(isshow);
        mTrustShipRoot.SetActive(isshow);
    }
    #endregion
}
