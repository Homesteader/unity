using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJVoiceData
{
    /// <summary>
    /// 语音时间
    /// </summary>
    public float mTime;
    /// <summary>
    /// 语音地址
    /// </summary>
    public string mUrl;
}


// 玩家的牌结构体
[ProtoBuf.ProtoContract]
public class CardsInfoStruct
{
    [ProtoBuf.ProtoMember(1)]
    public int seatId;//玩家的座位号
    [ProtoBuf.ProtoMember(2)]
    public List<int> handList;//手牌数据
    [ProtoBuf.ProtoMember(3)]
    public List<PengStruct> pengList;//碰牌列表
    [ProtoBuf.ProtoMember(4)]
    public List<GangStruct> gangList;//杠牌列表
    [ProtoBuf.ProtoMember(5)]
    public List<HuStruct> huList;//胡牌列表
    [ProtoBuf.ProtoMember(6)]
    public List<int> hitList;//打牌列表
    [ProtoBuf.ProtoMember(7)]
    public int cardNum;//手牌数量
    [ProtoBuf.ProtoMember(8)]
    public int currCard;//最后一张牌
    [ProtoBuf.ProtoMember(9)]
    public bool isHasCurrCard;//是否有最后一张牌

    [ProtoBuf.ProtoMember(10)]
    public List<FixedStruct> fixedList;//所有人定缺的什么
    [ProtoBuf.ProtoMember(11)]
    public bool isChanged = false;//是否换三张
    [ProtoBuf.ProtoMember(12)]
    public bool isFixedColor = false;//是否定缺
    [ProtoBuf.ProtoMember(13)]
    public int fixedType;//定缺类型
    [ProtoBuf.ProtoMember(14)]
    public List<int> changeTipCards;//换三张的牌
    [ProtoBuf.ProtoMember(15)]
    public bool isHasJiao;//是否有叫

    /// <summary>
    /// 清空数据
    /// </summary>
    public void ClearAllData()
    {
        pengList = null;
        gangList = null;
        hitList = null;
        huList = null;
        handList = null;
        cardNum = 0;
        changeTipCards = null;
        currCard = 0;
        fixedList = null;
        fixedType = 0;
        isChanged = false;
    }

    #region 主动操作牌
    /// <summary>
    /// 主动操作牌
    /// </summary>
    /// <param name="data"></param>
    public void InstructionsCard(MJoptInfoData data, ref int lastOutCard)
    {
        eMJInstructionsType _type = data.ins;
        CardsStruct chicard;
        switch (_type)
        {
            case eMJInstructionsType.GUO: //过
                break;
            case eMJInstructionsType.GANG: //杠
                if (gangList == null)
                    gangList = new List<GangStruct>();
                chicard = AddGangCard(ref gangList, data, lastOutCard);

                break;
            case eMJInstructionsType.PENG: //碰
                if (pengList == null)
                    pengList = new List<PengStruct>();
                chicard = AddPengCard(ref pengList, data, lastOutCard);
                //chicard.card.Sort();
                break;
            case eMJInstructionsType.HIT: //打牌
                if (hitList == null)
                    hitList = new List<int>();
                AddHitCard(ref hitList, data);
                lastOutCard = data.cards[0];
                break;
            case eMJInstructionsType.MO: //摸牌
                //if (mHandCard != null)//摸起来的牌加到手牌中并且去掉摸起来的牌
                MJGameModel.Inst.leaveCardNum--;
                MJGameModel.Inst.mStartGameData.startInfo.leaveCardNum--;
                if (handList != null)
                {
                    if (!MJGameModel.Inst.isHasFixeCard && data.seatId == MJGameModel.Inst.mMySeatId)
                    {
                        if (data.cards[0] > MJGameModel.Inst.eFixe[0] && data.cards[0] < MJGameModel.Inst.eFixe[1])
                        {
                            MJGameModel.Inst.isHasFixeCard = true;
                        }
                    }
                    currCard = data.cards[0];
                }
                else
                    isHasCurrCard = true;
                break;
            case eMJInstructionsType.YPDX:
            case eMJInstructionsType.HU://胡
                if (huList == null)
                    huList = new List<HuStruct>();
                chicard = AddHuCard(ref huList, data, lastOutCard);
                if (data.seatId == MJGameModel.Inst.mMySeatId)
                {
                    MJGameModel.Inst.isHu = huList != null && huList.Count > 0;
                }
                break;

        }
    }


    /// <summary>
    /// 被动接受的指令
    /// </summary>
    /// <param name="data"></param>
    /// <param name="lastOutCard"></param>
    public void PassiveInstructionsCard(MJoptInfoData data, ref int lastOutCard)
    {
        eMJInstructionsType _type = data.ins;
        switch (_type)
        {
            case eMJInstructionsType.CHI:
            case eMJInstructionsType.PENG:
                if (hitList != null)
                    hitList.RemoveAt(hitList.Count - 1);
                lastOutCard = 0;
                break;
            case eMJInstructionsType.GANG:
                if (data.cards.Count <= 4)
                {
                    if (hitList != null)
                        hitList.RemoveAt(hitList.Count - 1);
                    lastOutCard = 0;
                }
                break;
            case eMJInstructionsType.HU:
            case eMJInstructionsType.YPDX:
                if (hitList != null && lastOutCard != 0)
                {
                    if (data.subType == (int)eHuSubType.QGH)//抢杠胡移除一张手牌
                    {
                        if (data.otherSeatId == seatId)
                        {
                            if (handList != null)//自己
                            {
                                if (currCard > 0)//如果有摸起来的牌就把摸起来的牌放入手牌
                                {
                                    handList.Add(currCard);
                                    currCard = 0;
                                }
                                handList.Remove(data.cards[0]);
                            }
                            else//其他人
                            {
                                if (isHasCurrCard)//如果有摸起来的牌就把摸起来的牌放入手牌
                                {
                                    cardNum++;
                                    isHasCurrCard = false;
                                    currCard = 0;
                                }
                                cardNum--;
                            }
                        }
                        if (!data.huGl)
                        {

                        }
                    }
                    else
                        hitList.RemoveAt(hitList.Count - 1);
                    lastOutCard = 0;
                }
                break;
        }
    }

    private GangStruct AddGangCard(ref List<GangStruct> mycard, MJoptInfoData data, int lastoutcard)
    {
        //我牌里面加一组
        if (mycard == null)
            mycard = new List<GangStruct>();
        GangStruct chicard = new GangStruct();
        chicard.cards = new List<int>();
        chicard.otherSeatId = data.otherSeatId;
        switch (data.type)
        {
            case (int)eGangType.ANGANG:
                chicard.gangType = eGangType.ANGANG;
                break;
            case (int)eGangType.DIANGANG:
                chicard.gangType = eGangType.DIANGANG;
                break;
            case (int)eGangType.WANGANG:
                chicard.gangType = eGangType.WANGANG;
                break;
        }

        if (currCard > 0)//摸起来的牌加到手牌中并且去掉摸起来的牌
        {
            handList.Add(currCard);
            currCard = 0;
        }
        else if (isHasCurrCard)
        {
            cardNum += 1;
            isHasCurrCard = false;
        }
        //手牌中减去杠牌
        if (data.ins == eMJInstructionsType.GANG)
        {
            int len = 3;
            if (data.otherSeatId == seatId || data.otherSeatId == 0)//暗杠
            {
                if (pengList != null)//如果是先碰的牌再杠
                {
                    PengStruct card = pengList.Find(o => o.cards[0] == data.cards[0]);
                    if (card != null)
                    {
                        pengList.Remove(card);
                        len = 1;
                    }
                    else
                        len = 4;
                }
                else
                    len = 4;
            }
            else//别人打来杠
                len = 3;
            if (handList != null && currCard == data.cards[0])//如果摸起来的牌也是杠牌要去掉
                currCard = 0;
            for (int i = 0; i < len; i++)
            {
                if (handList != null)
                {
                    if (handList.Contains(data.cards[0]))
                        handList.Remove(data.cards[0]);
                    else
                    {
                        if (currCard == data.cards[0])
                            currCard = 0;
                    }
                }
                else
                    cardNum--;
            }
            chicard.cards.Add(data.cards[0]);
        }
        else if (data.ins == eMJInstructionsType.PENG)
        {
            int len = 2;
            for (int i = 0; i < len; i++)
            {
                if (handList != null)
                {
                    if (data.cards.Count == 1)
                        handList.Remove(data.cards[0]);
                    else
                        handList.Remove(data.cards[i]);
                }
                else
                    cardNum--;
            }
            chicard.cards.Add(data.cards[0]);
        }
        else if (data.ins == eMJInstructionsType.CHI)
        {
            int len = data.cards.Count;
            for (int i = 0; i < len; i++)
            {
                if (lastoutcard != data.cards[i])
                {
                    if (handList != null)
                    {
                        if (data.cards.Count == 1)
                            handList.Remove(data.cards[0]);
                        else if (data.cards[i] != lastoutcard)
                            handList.Remove(data.cards[i]);
                    }
                    else
                        cardNum--;
                }
                chicard.cards.Add(data.cards[i]);
            }

        }
        else
        {
            for (int i = 0; i < data.cards.Count; i++)
            {
                if (handList != null)
                {
                    if (handList.Contains(data.cards[i]))
                        handList.Remove(data.cards[i]);
                    else
                    {
                        if (currCard == data.cards[i])
                            currCard = 0;
                    }
                }
                else
                    cardNum--;
                chicard.cards.Add(data.cards[i]);
            }
        }
        mycard.Add(chicard);
        return chicard;
    }
    private PengStruct AddPengCard(ref List<PengStruct> mycard, MJoptInfoData data, int lastoutcard)
    {
        //我牌里面加一组
        if (mycard == null)
            mycard = new List<PengStruct>();
        PengStruct chicard = new PengStruct();
        chicard.cards = new List<int>();

        chicard.otherSeatId = data.otherSeatId;
        //chicard.type = data.type;
        if (handList != null && currCard > 0)//摸起来的牌加到手牌中并且去掉摸起来的牌
        {
            handList.Add(currCard);
            currCard = 0;
        }
        else if (isHasCurrCard)
        {
            cardNum += 1;
            isHasCurrCard = false;
        }
        //手牌中减去杠牌
        if (data.ins == eMJInstructionsType.PENG)
        {
            int len = 2;
            for (int i = 0; i < len; i++)
            {
                if (handList != null)
                {
                    if (data.cards.Count == 1)
                        handList.Remove(data.cards[0]);
                    else
                        handList.Remove(data.cards[i]);
                }
                else
                    cardNum--;
            }
            chicard.cards.Add(data.cards[0]);
        }
        mycard.Add(chicard);
        if (handList != null)
        {
            currCard = handList[handList.Count - 1];
            handList.RemoveAt(handList.Count - 1);
        }
        return chicard;
    }
    private HuStruct AddHuCard(ref List<HuStruct> mycard, MJoptInfoData data, int lastoutcard)
    {
        //我牌里面加一组
        if (mycard == null)
            mycard = new List<HuStruct>();
        HuStruct chicard = new HuStruct();
        if (handList != null && currCard > 0)//摸起来的牌加到手牌中并且去掉摸起来的牌
        {
            handList.Add(currCard);
            currCard = 0;
        }
        else if (isHasCurrCard)
        {
            cardNum += 1;
            isHasCurrCard = false;
        }
        if (data.ins == eMJInstructionsType.HU || data.ins == eMJInstructionsType.YPDX)
        {
            if (data.type == (int)eHuType.ZIMO)
            {
                if (handList != null)
                {
                    handList.RemoveAt(handList.Count - 1);
                    currCard = 0;
                }
                else
                    cardNum--;
            }
            chicard.card = data.cards[0];
            chicard.isGl = data.huGl;
            chicard.huType = data.type;
        }

        mycard.Add(chicard);
        return chicard;
    }
    public List<int> AddHitCard(ref List<int> mycard, MJoptInfoData data)
    {
        //我牌里面加一组
        if (mycard == null)
            mycard = new List<int>();
        List<int> icard = new List<int>();//要操作的牌
        if (handList != null && currCard > 0)//摸起来的牌加到手牌中并且去掉摸起来的牌
        {
            handList.Add(currCard);
            currCard = 0;
        }
        else if (isHasCurrCard)
        {
            cardNum += 1;
            isHasCurrCard = false;
        }
        //手牌中减去打出去的牌
        for (int i = 0; i < data.cards.Count; i++)
        {
            if (handList != null)
                handList.Remove(data.cards[i]);
            else
                cardNum--;
            mycard.Add(data.cards[i]);

        }

        if (handList != null)
        {
            handList = MJGameModel.Inst.ChangList(handList, this.fixedType);
            foreach (var item in handList)
            {
                if (item > MJGameModel.Inst.eFixe[0] && item < MJGameModel.Inst.eFixe[1])
                {
                    MJGameModel.Inst.isHasFixeCard = true;
                    break;
                }
                MJGameModel.Inst.isHasFixeCard = false;
            }
        }
        return mycard;
    }
    #endregion
}

public class MJGamePlayerData
{
    public MJplayerInfo mPlayerInfo;//玩家信息
    public MJmyInfo mHandCard;//手牌
    public MJotherInfo mOtherCard;//其他人手牌
    public List<MJVoiceData> mVoiceList = new List<MJVoiceData>();//语音信息

    public CardsInfoStruct mCardsInfoStruct;//玩家牌的信息
    public CardsInfoStruct[] allPlayersCardsInfoStruct;//所有玩家的牌的数据


    #region 主动操作牌
    /// <summary>
    /// 主动操作牌
    /// </summary>
    /// <param name="data"></param>
    public void InstructionsCard(MJoptInfoData data, ref int lastOutCard)
    {
        eMJInstructionsType _type = data.ins;
        CardsStruct chicard;
        switch (_type)
        {
            case eMJInstructionsType.GUO: //过
                break;
            case eMJInstructionsType.GANG: //杠
                chicard = AddGangCard(ref mPlayerInfo.gangList, data, lastOutCard);

                break;
            case eMJInstructionsType.PENG: //碰
                chicard = AddPengCard(ref mPlayerInfo.pengList, data, lastOutCard);
                //chicard.card.Sort();
                break;
            case eMJInstructionsType.HIT: //打牌
                AddHitCard(ref mPlayerInfo.hitList, data);
                lastOutCard = data.cards[0];
                break;
            case eMJInstructionsType.MO: //摸牌
                //if (mHandCard != null)//摸起来的牌加到手牌中并且去掉摸起来的牌
                MJGameModel.Inst.leaveCardNum--;
                MJGameModel.Inst.mStartGameData.startInfo.leaveCardNum--;
                if (data.seatId == MJGameModel.Inst.mMySeatId)
                {
                    if (!MJGameModel.Inst.isHasFixeCard)
                    {
                        if (data.cards[0] > MJGameModel.Inst.eFixe[0] && data.cards[0] < MJGameModel.Inst.eFixe[1])
                        {
                            MJGameModel.Inst.isHasFixeCard = true;
                        }
                    }
                    mHandCard.currCard = data.cards[0];
                }
                else if (mOtherCard != null)
                    mOtherCard.isHasCurrCard = true;
                break;
            case eMJInstructionsType.YPDX:
            case eMJInstructionsType.HU://胡
                chicard = AddHuCard(ref mPlayerInfo.mHuCard, data, lastOutCard);
                if (data.seatId == MJGameModel.Inst.mMySeatId)
                {
                    MJGameModel.Inst.isHu = mPlayerInfo.mHuCard != null;
                }
                break;

        }
    }


    /// <summary>
    /// 被动接受的指令
    /// </summary>
    /// <param name="data"></param>
    /// <param name="lastOutCard"></param>
    public void PassiveInstructionsCard(MJoptInfoData data, ref int lastOutCard)
    {
        eMJInstructionsType _type = data.ins;
        switch (_type)
        {
            case eMJInstructionsType.CHI:
            case eMJInstructionsType.PENG:
                if (mPlayerInfo.hitList != null)
                    mPlayerInfo.hitList.RemoveAt(mPlayerInfo.hitList.Count - 1);
                lastOutCard = 0;
                break;
            case eMJInstructionsType.GANG:
                if (data.cards.Count <= 4)
                {
                    if (mPlayerInfo.hitList != null)
                        mPlayerInfo.hitList.RemoveAt(mPlayerInfo.hitList.Count - 1);
                    lastOutCard = 0;
                }
                break;
            case eMJInstructionsType.HU:
                if (mPlayerInfo.hitList != null && lastOutCard != 0)
                {
                    if (data.subType == (int)eHuSubType.QGH)
                    {
                        if (!data.huGl)
                        {
                            if (mHandCard != null && data.otherSeatId == mPlayerInfo.seatId)
                            {
                                if (mHandCard.cards != null)
                                    mHandCard.cards.Remove(data.cards[0]);
                                else
                                    mOtherCard.cardsNum--;
                            }
                        }
                    }
                    mPlayerInfo.hitList.RemoveAt(mPlayerInfo.hitList.Count - 1);
                    lastOutCard = 0;
                }
                break;
        }
    }

    private GangStruct AddGangCard(ref List<GangStruct> mycard, MJoptInfoData data, int lastoutcard)
    {
        //我牌里面加一组
        if (mycard == null)
            mycard = new List<GangStruct>();
        GangStruct chicard = new GangStruct();
        chicard.cards = new List<int>();
        chicard.otherSeatId = data.otherSeatId;
        switch (data.type)
        {
            case (int)eGangType.ANGANG:
                chicard.gangType = eGangType.ANGANG;
                break;
            case (int)eGangType.DIANGANG:
                chicard.gangType = eGangType.DIANGANG;
                break;
            case (int)eGangType.WANGANG:
                chicard.gangType = eGangType.WANGANG;
                break;
        }

        if (mHandCard != null && mHandCard.currCard > 0)//摸起来的牌加到手牌中并且去掉摸起来的牌
        {
            mHandCard.cards.Add(mHandCard.currCard);
            mHandCard.currCard = 0;
        }
        else if (mOtherCard != null && mOtherCard.isHasCurrCard)
        {
            mOtherCard.cardsNum += 1;
            mOtherCard.isHasCurrCard = false;
        }
        //手牌中减去杠牌
        if (data.ins == eMJInstructionsType.GANG)
        {
            /* int len = 3;
             if (data.type== (int)eGangType.ANGANG)
             {
                 len = 4;
             }

     */

            int len = 3;
            if (data.otherSeatId == mPlayerInfo.seatId || data.otherSeatId == 0)//暗杠
            {
                if (mPlayerInfo.pengList != null)//如果是先碰的牌再杠
                {
                    PengStruct card = mPlayerInfo.pengList.Find(o => o.cards[0] == data.cards[0]);
                    if (card != null)
                    {
                        mPlayerInfo.pengList.Remove(card);
                        len = 1;
                    }
                    else
                        len = 4;
                }
                else
                    len = 4;
            }
            else//别人打来杠
                len = 3;
            if (mHandCard != null && mHandCard.currCard == data.cards[0])//如果摸起来的牌也是杠牌要去掉
                mHandCard.currCard = 0;
            for (int i = 0; i < len; i++)
            {
                if (mHandCard != null && data.seatId == MJGameModel.Inst.mMySeatId)
                {
                    if (mHandCard.cards.Contains(data.cards[0]))
                        mHandCard.cards.Remove(data.cards[0]);
                    else
                    {
                        if (mHandCard.currCard == data.cards[0])
                            mHandCard.currCard = 0;
                    }
                }
                else
                    mOtherCard.cardsNum--;
            }
            chicard.cards.Add(data.cards[0]);
        }
        else if (data.ins == eMJInstructionsType.PENG)
        {
            int len = 2;
            for (int i = 0; i < len; i++)
            {
                if (mHandCard != null)
                {
                    if (data.cards.Count == 1)
                        mHandCard.cards.Remove(data.cards[0]);
                    else
                        mHandCard.cards.Remove(data.cards[i]);
                }
                else
                    mOtherCard.cardsNum--;
            }
            chicard.cards.Add(data.cards[0]);
        }
        else if (data.ins == eMJInstructionsType.CHI)
        {
            int len = data.cards.Count;
            for (int i = 0; i < len; i++)
            {
                if (lastoutcard != data.cards[i])
                {
                    if (mHandCard != null)
                    {
                        if (data.cards.Count == 1)
                            mHandCard.cards.Remove(data.cards[0]);
                        else if (data.cards[i] != lastoutcard)
                            mHandCard.cards.Remove(data.cards[i]);
                    }
                    else
                        mOtherCard.cardsNum--;
                }
                //chicard.type = data.type;
                chicard.cards.Add(data.cards[i]);
            }

        }
        else
        {
            for (int i = 0; i < data.cards.Count; i++)
            {
                if (mHandCard != null)
                {
                    if (mHandCard.cards.Contains(data.cards[i]))
                        mHandCard.cards.Remove(data.cards[i]);
                    else
                    {
                        if (mHandCard.currCard == data.cards[i])
                            mHandCard.currCard = 0;
                    }
                }
                else
                    mOtherCard.cardsNum--;
                chicard.cards.Add(data.cards[i]);
            }
        }
        if (mHandCard != null)
            //mHandCard.cards.Sort();
            mycard.Add(chicard);
        return chicard;
    }
    private PengStruct AddPengCard(ref List<PengStruct> mycard, MJoptInfoData data, int lastoutcard)
    {
        //我牌里面加一组
        if (mycard == null)
            mycard = new List<PengStruct>();
        PengStruct chicard = new PengStruct();
        chicard.cards = new List<int>();

        chicard.otherSeatId = data.otherSeatId;
        //chicard.type = data.type;
        if (mHandCard != null && mHandCard.currCard > 0)//摸起来的牌加到手牌中并且去掉摸起来的牌
        {
            mHandCard.cards.Add(mHandCard.currCard);
            mHandCard.currCard = 0;
        }
        else if (mOtherCard != null && mOtherCard.isHasCurrCard)
        {
            mOtherCard.cardsNum += 1;
            mOtherCard.isHasCurrCard = false;
        }
        //手牌中减去杠牌
        if (data.ins == eMJInstructionsType.PENG)
        {
            int len = 2;
            for (int i = 0; i < len; i++)
            {
                if (mHandCard != null && data.seatId == MJGameModel.Inst.mMySeatId)
                {
                    if (data.cards.Count == 1)
                        mHandCard.cards.Remove(data.cards[0]);
                    else
                        mHandCard.cards.Remove(data.cards[i]);
                }
                else
                    mOtherCard.cardsNum--;
            }
            chicard.cards.Add(data.cards[0]);
        }
        if (mHandCard != null)
            //mHandCard.cards.Sort();
            mycard.Add(chicard);
        if (mHandCard != null && data.seatId == MJGameModel.Inst.mMySeatId)
        {
            mHandCard.currCard = mHandCard.cards[mHandCard.cards.Count - 1];
            mHandCard.cards.RemoveAt(mHandCard.cards.Count - 1);
        }
        return chicard;
    }
    private HuStruct AddHuCard(ref List<HuStruct> mycard, MJoptInfoData data, int lastoutcard)
    {
        //我牌里面加一组
        if (mycard == null)
            mycard = new List<HuStruct>();
        HuStruct chicard = new HuStruct();
        if (mHandCard != null && mHandCard.currCard > 0)//摸起来的牌加到手牌中并且去掉摸起来的牌
        {
            mHandCard.cards.Add(mHandCard.currCard);
            mHandCard.currCard = 0;
        }
        else if (mOtherCard != null && mOtherCard.isHasCurrCard)
        {
            mOtherCard.cardsNum += 1;
            mOtherCard.isHasCurrCard = false;
        }
        if (data.ins == eMJInstructionsType.HU)
        {
            if (data.type == (int)eHuType.ZIMO)
            {
                if (mHandCard != null && data.seatId == MJGameModel.Inst.mMySeatId)
                {
                    mHandCard.cards.RemoveAt(mHandCard.cards.Count - 1);
                    mHandCard.currCard = 0;
                }
                else
                    mOtherCard.cardsNum--;
            }
            chicard.card = data.cards[0];
            chicard.isGl = data.huGl;
        }

        if (mHandCard != null)
            mycard.Add(chicard);
        return chicard;
    }
    public List<int> AddHitCard(ref List<int> mycard, MJoptInfoData data)
    {
        //我牌里面加一组
        if (mycard == null)
            mycard = new List<int>();
        List<int> icard = new List<int>();//要操作的牌
        if (mHandCard != null && mHandCard.currCard > 0)//摸起来的牌加到手牌中并且去掉摸起来的牌
        {
            mHandCard.cards.Add(mHandCard.currCard);
            mHandCard.currCard = 0;
        }
        else if (mOtherCard != null && mOtherCard.isHasCurrCard)
        {
            mOtherCard.cardsNum += 1;
            mOtherCard.isHasCurrCard = false;
        }
        //手牌中减去打出去的牌
        for (int i = 0; i < data.cards.Count; i++)
        {
            if (mHandCard != null && data.seatId == MJGameModel.Inst.mMySeatId)
                mHandCard.cards.Remove(data.cards[i]);
            else
                mOtherCard.cardsNum--;
            mycard.Add(data.cards[i]);

        }

        if (mHandCard != null && data.seatId == MJGameModel.Inst.mMySeatId)
        {
            mHandCard.cards = MJGameModel.Inst.ChangList(mHandCard.cards, (int)MJGameModel.Inst.eFixeType);
            foreach (var item in mHandCard.cards)
            {
                if (item > MJGameModel.Inst.eFixe[0] && item < MJGameModel.Inst.eFixe[1])
                {
                    MJGameModel.Inst.isHasFixeCard = true;
                    break;
                }
                MJGameModel.Inst.isHasFixeCard = false;
            }
        }
        return mycard;
    }
    #endregion
}

public class MJGameModel : BaseModel
{

    public static MJGameModel Inst;

    public Queue<MJoptInfoData> msgQueue = new Queue<MJoptInfoData>();//操作  消息 队列

    public override void SetController(BaseController c)
    {
        base.SetController(c);
        Inst = this;
    }

    /// <summary> 房间ID </summary>
    public string mRoomId;
    /// <summary> 我自己的位置 </summary>
    public int mMySeatId;
    /// <summary> 庄家的座位号 </summary>
    public int mZhuangSeatId;
    /// <summary> 房主座位ID </summary>
    public int mRoomMgSeatId;
    /// <summary> 其他玩家作为号对应的index </summary>
    public int[] mSeatToIndex = new int[5];
    /// <summary> 上一次打出来的牌 </summary>
    public int mLastOutCard;
    /// <summary> 上一次打出来牌的座位 </summary>
    public int mLastOutCardSeat;
    /// <summary> 当前状态 </summary>
    public eMJRoomStatus mState;
    /// <summary> 骰子数字 </summary>
    public int[] mTouzi = new int[] { 1, 2 };
    /// <summary> 底分 </summary>
    public int mBaseScore;
    /// <summary> 总局数 </summary>
    public int mPlayCount;
    /// <summary> 当前局数 </summary>
    public int mCurPlayCount;
    /// <summary> 本期玩法 </summary>
    public List<int> mCurRules;
    /// <summary> 封顶番数 </summary>
    public int mMaxMaxMul;
    /// <summary> 最终结算界面 </summary>
    public MJGameSettlementFinalInfo mFinalSettlementInfo;
    public litSemResponse mLitSem;//服务器给的小结算数据
    /// <summary> 准备倒计时 </summary>
    public float ReadyCountDownTime;

    //小结算 数据
    public MJGameSettlementInfo mSettlData;
    /// <summary> 当前选中的牌 </summary>
    public MjMyHandCard mCurSlectCard;
    /// <summary> 当前操作牌的玩家座位号 </summary>
    public int mCurInsSeatId;
    /// <summary> 当前房间类型 </summary>
    public int mRoomType;
    /// <summary> 之前的背景音量 </summary>
    public float mLastMusicVolume;
    /// <summary> 之前的音效音量 </summary>
    public float mLastSoundVolume;
    /// <summary> 是否正在录音 </summary>
    public bool mIsVoice;
    /// <summary> 是否正在播放声音 </summary>
    public bool mIsPlayVoice;
    /// <summary> 是否处于听牌状态 </summary>
    public bool mIsTing;
    /// <summary>
    /// 操作固定时间
    /// </summary>
    public int TurnFixedTime;//操作固定时间
    public void ResetData()
    {
        msgQueue.Clear();
        if (mStartGameData.roomInfo.currGameCount < mStartGameData.roomInfo.maxGameCount)
            mStartGameData.roomInfo.currGameCount++;
        if (allPlayersCardsInfoStruct != null)
        {
            for (int i = 0; i < allPlayersCardsInfoStruct.Length; i++)
            {
                if (allPlayersCardsInfoStruct[i] == null)
                    continue;
                allPlayersCardsInfoStruct[i].ClearAllData();
            }
        }
        //mLitSem = null;
        mSettlData = null;
        mCurSlectCardList.Clear();
        mySelfCards.Clear();
        isHu = false;
        eFixe = new int[2] { 0, 0 };
        isHasFixeCard = true;
        isMyHit = false;
        hitCardCanHuList = null;
        hasCanHuListCards = null;
        mSycOptListResponse = null;
        OptCachInChange = null;
        AllPlayerEfixe = null;
        mZhuangSeatId = 0;
    }

    /// <summary> 是否是东家 </summary>
    public bool IsMg
    {
        get
        {
            return mRoomMgSeatId == mMySeatId;
        }
    }

    #region 获取最佳牌型
    /// <summary>
    /// 得到胡牌类型
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public string GetHuType(MJGameSettlementPlayerInfo info)
    {
        if (info == null)
            return "";
        string str = "";
        MJGameHupaiConfig con = null;//ConfigManager.Create().mHuPaiType.data[0];
        if (info.des != null)
        {
            #region 闭门听
            /* int bimenting = 0;
              for (int i = 0; i < info.des.Count; i++)
              {
                  if (info.des[i] == 38 || info.des[i] == 17)//报听和不开门同时存在显示闭门听
                  {
                      bimenting++;
                      if (bimenting >= 2)
                      {
                          if (info.des.Contains(29))//点炮
                              str += "闭门炮" + "   ";
                          else if (info.score > 0)
                              str += "闭门听" + "   ";
                          else if (info.score == 0 && !info.des.Contains(30))//不存在闭门
                              str += "闭门" + "   ";
                          break;
                      }
                  }
              }
              */
            #endregion
            for (int i = 0; i < info.des.Count; i++)
            {
                if (info.des[i] >= con.hupaiType.Length)
                {
                    continue;
                }
                //else if (bimenting >= 2 && (info.des[i] == 38 || info.des[i] == 17))//如果是闭门听就不显示报听和不开门
                //    continue;
                else
                {
                    str += con.hupaiType[info.des[i]] + "   ";
                }
            }
        }
        bool isMGangZFB = false;//是否有明杠中发白
        bool isAGangZFB = false;//是否有暗杠中发白
                                //if (info.gang != null && info.huDePai > 0)//必须要胡牌的人才显示杠的类型
        if (info.gang != null && info.huPai.Count > 0)//必须要胡牌的人才显示杠的类型
        {
            GangStruct card;
            Dictionary<int, int> gangNum = new Dictionary<int, int>();//杠类型对应数量
            for (int i = 0; i < info.gang.Count; i++)
            {
                card = info.gang[i];
                /*
                if (card.cards[0] >= 41 && card.cards[0] <= 43)//杠的中发白不计入杠，单独显示明杠中发白，暗杠中发白
                {
                    if (card.type != 2 && !isMGangZFB)//明杠中发白
                    {
                        if (gangNum.ContainsKey((int)eMJGangType.MingGangZFB))
                            gangNum[(int)eMJGangType.MingGangZFB]++;
                        else
                            gangNum[(int)eMJGangType.MingGangZFB] = 1;
                    }
                    else if (card.type == 2 && !isAGangZFB)//暗杠中发白
                    {
                        if (gangNum.ContainsKey((int)eMJGangType.AnGangZFB))
                            gangNum[(int)eMJGangType.AnGangZFB]++;
                        else
                            gangNum[(int)eMJGangType.AnGangZFB] = 1;
                    }
                }
                else*/
                {
                    if (gangNum.ContainsKey((int)card.gangType))
                        gangNum[(int)card.gangType]++;
                    else
                        gangNum[(int)card.gangType] = 1;
                }
            }
            foreach (KeyValuePair<int, int> item in gangNum)
            {
                if (item.Value > 1)
                    str += con.gangType[item.Key] + "x" + item.Value + "  ";
                else
                    str += con.gangType[item.Key] + "  ";
            }
        }
        return str;
    }
    #endregion

    public StartGameRespone mStartGameData;//玩家开始时的数据
    public bool isGetStartGameData = false;//是否收到 开始时的数据
    public bool isFirstGetStartGameData = true;//是否是第一次手打同步可操作列表的数据
    public List<int> mySelfCards = new List<int>();//我自己的手牌数据
    public PlayerInfoStruct[] mRoomPlayers;//玩家信息数据
    public CardsInfoStruct[] allPlayersCardsInfoStruct;

    ///所有玩家的手牌数据
    public bool isOpenUI = false;

    public int totalPlayerCount;//房间 总人数
    public int[] mnewSeatToIndex;//  其他玩家作为号对应的index 
    public int[] mSeatToDirectionIndex;//所有玩家座位号对应桌面东南西北高亮显示的index
    public int totalCardNum; //总共有多少张牌
    public int leaveCardNum; //剩余多少张牌
    public List<int> handCardsTong = new List<int>();//手牌   筒
    public List<int> handCardsTiao = new List<int>();//手牌   条
    public List<int> handCardsWan = new List<int>();//手牌   万


    public List<MjMyHandCard> mCurSlectCardList = new List<MjMyHandCard>();//  当前选中 换3张的牌 
    public eCardType CurSlectCardListType;//换3张 的牌的类型
    public eFixedType eFixeType;//定缺类型
    public int[] eFixe = new int[2];//   定缺区间
    public List<int[]> AllPlayerEfixe = new List<int[]>();//所有玩家定缺区间
    public bool isHasFixeCard = true;//缺的牌是否还有
    public bool isHu = false;//是否胡了
    public bool isMyHit = false;//是否该我操作

    public List<HitCardCanHuStruct> hitCardCanHuList;//打什么可以 胡什么牌
    public List<int> hasCanHuListCards;//有胡牌提示的 牌

    public MessageData OptCachInChange;//换三张时缓存的指令
    public MJInstructionsProto mSycOptListResponse;//缓存指令
    public int mLastCard;//缓存上一次操作的牌


    /// <summary>
    /// 定缺排序
    /// </summary>
    /// <param name="oldList"></param>
    /// <param name="efix"></param>
    /// <returns></returns>
    public List<int> ChangList(List<int> oldList, int efix)
    {
        SQDebug.Log("==>>>手牌 排序");
        List<int> fixeList = new List<int>();
        int min = 0;
        int max = 0;
        switch (efix)
        {
            case (int)eFixedType.TIAO:
                min = 0;
                max = 10;
                break;
            case (int)eFixedType.TONG:
                min = 10;
                max = 20;
                break;
            case (int)eFixedType.WAN:
                min = 20;
                max = 30;
                break;
        }
        isHasFixeCard = false;
        for (int i = oldList.Count - 1; i >= 0; i--)
        {
            if (oldList[i] > min && oldList[i] < max)
            {
                isHasFixeCard = true;
                fixeList.Add(oldList[i]);
                oldList.Remove(oldList[i]);
            }
        }
        oldList.Sort();
        fixeList.Sort();
        if (fixeList == null)
        {
            return oldList;
        }

        for (int i = 0; i < fixeList.Count; i++)
        {
            oldList.Add(fixeList[i]);
        }
        return oldList;
    }

    /// <summary>
    /// 设置定缺类型
    /// </summary>
    /// <param name="_type">定缺类型</param>
    public void SetMyFixedType(int _type)
    {
        eFixeType = (eFixedType)_type;
        switch (_type)
        {
            case (int)eFixedType.WAN:
                eFixe = new int[] { 20, 30 };
                break;
            case (int)eFixedType.TIAO:
                eFixe = new int[] { 0, 10 };
                break;
            case (int)eFixedType.TONG:
                eFixe = new int[] { 10, 20 };
                break;
        }
    }

    /// <summary>
    /// 获取定缺区间
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    public int[] GetPlayerFixedType(eFixedType _type)
    {
        int[] fdata = null;
        switch (_type)
        {
            case eFixedType.WAN:
                fdata = new int[] { 20, 30 };
                break;
            case eFixedType.TIAO:
                fdata = new int[] { 0, 10 };
                break;
            case eFixedType.TONG:
                fdata = new int[] { 10, 20 };
                break;
        }
        return fdata;
    }


    /// <summary>
    /// 是否有定缺
    /// </summary>
    public bool IsContainsDingque
    {
        get
        {
            if (mStartGameData.roomInfo.createData.subType == 1)
                return true;
            return false;
        }
    }

    /// <summary>
    /// 是否包含换三张
    /// </summary>
    public bool IsContainsChange
    {
        get
        {
            if (mStartGameData.roomInfo.createData.ruleIndexs.Contains(6))//是否包含换三张
                return true;
            return false;
        }
    }
}
