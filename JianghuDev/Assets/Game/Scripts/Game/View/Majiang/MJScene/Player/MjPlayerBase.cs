using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MjPlayerBase : MonoBehaviour
{
    public MJSenceRoot mRoot;
    public MjCollectCard mHandCard;//手牌item
    public MjCollectCard mCollectCard;//打出的牌
    public MjPengCard mPengCard;//碰的牌
    public MjCollectCard mCurCard;//手上摸的牌
    public MjCollectCard mHuCard;//胡的牌
    public MjChiCard mChiCard;//可以吃的牌

    public GameObject mChangeThree;//换3张的牌
    public List<MjCollectCard> mHandCardList = new List<MjCollectCard>();//手牌list
    public List<MjCollectCard> mCollectCardList = new List<MjCollectCard>();//收集list
    public List<MjPengCard> mPengCardList = new List<MjPengCard>();//碰牌
    private List<MjPengCard> mGangCardList = new List<MjPengCard>();//杠牌
    private List<MjPengCard> mChiCardList = new List<MjPengCard>();//吃牌
    private List<MjPengCard> mOtherCardList = new List<MjPengCard>();//qita

    private List<MjCollectCard> mHuCardList = new List<MjCollectCard>();//胡

    public List<MjCollectCard> mAllHandCardList = new List<MjCollectCard>();//所有的手牌    换3张使用

    public List<MjCollectCard> mSureChangeThreeList = new List<MjCollectCard>();//   换3张确定的牌
    public MjCollectCard[] mChangeThreeCards;//换三张牌，回放的时候需要显示出来
    public FGrid mChangeThreeGrid;//换3张的牌
    public FGrid mHandGrid;//手牌
    public FCollegeGrid mCollectGrid;//已打出的牌
    public FGrid mPengGrid;//碰牌
    public FGrid mHuGrid;//碰牌
    public FGrid mChiGrid;//可以吃的牌
    public PlayerPosType mPlayerPosType;//玩家位置类型
    public Camera mCamera;
    public Transform mCurCardArrow;//当前打出牌特效

    public int mMySeatid;//我的座位号
    private Vector3 mInitHandCardRootPos;//手牌root初始位置

    void Awake()
    {
        mInitHandCardRootPos = mHandGrid.transform.localPosition;
    }

    /// <summary>
    /// 重置牌
    /// </summary>
    public void ResetCards()
    {
        Transform tran = mHandGrid.transform;
        if (mPlayerPosType != PlayerPosType.Self)
            tran.localRotation = Quaternion.Euler(Vector3.zero);
        mHandGrid.transform.localPosition = mInitHandCardRootPos;
        //手牌
        for (int i = 0; i < mHandCardList.Count; i++)
        {
            mHandCardList[i].SetFixeMaskShow(false);
            mHandCardList[i].SetNum(0, mPlayerPosType);
            mHandCardList[i].gameObject.SetActive(false);

        }
        //收集的牌
        for (int i = 0; i < mCollectCardList.Count; i++)
        {
            mCollectCardList[i].gameObject.SetActive(false);
            mCollectCardList[i].SetMaskShow(false);
        }
        //碰牌
        for (int i = 0; i < mPengCardList.Count; i++)
        {
            mPengCardList[i].gameObject.SetActive(false);
            mPengCardList[i].SetMaskShow(-1);
        }
        //杠牌
        for (int i = 0; i < mGangCardList.Count; i++)
        {
            mGangCardList[i].gameObject.SetActive(false);
            mGangCardList[i].SetMaskShow(-1);
        }
        //吃牌
        for (int i = 0; i < mChiCardList.Count; i++)
        {
            mChiCardList[i].gameObject.SetActive(false);
            mChiCardList[i].SetMaskShow(-1);
        }
        //胡的牌
        for (int i = 0; i < mHuCardList.Count; i++)
        {
            mHuCardList[i].gameObject.SetActive(false);
            mHuCardList[i].SetMaskShow(false);
        }
        //其他
        for (int i = 0; i < mOtherCardList.Count; i++)
        {
            mOtherCardList[i].gameObject.SetActive(false);
            mOtherCardList[i].SetMaskShow(-1);
        }
        //摸的牌
        mCurCard.gameObject.SetActive(false);
        //胡的牌
        //mHuCard.gameObject.SetActive(false);
    }

    #region 手牌相关
    /// <summary>
    /// 初始化手牌
    /// </summary>
    /// <param name="cards">手牌</param>
    /// <param name="count">手牌数量（是自己可以任意填）</param>
    /// <param name="curcard">当前摸在手上的牌（>0表示有）</param>
    public virtual void InitHandCards(List<int> cards, int count, int curcard = 0)
    {
        mAllHandCardList.Clear();
        int len = cards == null ? count : cards.Count;
        int gridLen = mHandCardList.Count;
        MjCollectCard card;
        for (int i = 0; i < len; i++)
        {
            if (i < gridLen)
                card = mHandCardList[i];
            else
            {
                card = NGUITools.AddChild(mHandGrid.gameObject, mHandCard.gameObject).GetComponent<MjCollectCard>();
                mHandCardList.Add(card);
            }
            card.gameObject.SetActive(true);
            if (cards != null && cards.Count > 0)
            {
                card.SetNum(cards[i], mPlayerPosType);

            }
            mAllHandCardList.Add(card);
            SetFixeMask(card);//定缺遮罩
        }
        for (int i = len; i < gridLen; i++)
            mHandCardList[i].gameObject.SetActive(false);
        mHandGrid.Reposition();
        if (mPlayerPosType == PlayerPosType.Self)//设置自己手牌位置，打完牌或者碰杠后居中
            SetMyHandCardPos(len);
        //设置摸起来的牌
        if (curcard > 0)
        {
            List<Transform> list = mHandGrid.GetChildList();
            if (list.Count > 0)
            {
                if (mPlayerPosType == PlayerPosType.Self)
                {
                    Vector3 pos = list[list.Count - 1].position;
                    float x = mHandGrid.CellWidth > 0 ? 1 : -1;
                    mCurCard.transform.position = new Vector3(pos.x + 3.5f, pos.y, pos.z);
                }
                else
                {
                    Vector3 pos = list[list.Count - 1].localPosition;
                    float x = mHandGrid.CellWidth > 0 ? 1 : -1;
                    mCurCard.transform.localPosition = new Vector3(pos.x + 4.8f * x, pos.y, pos.z);
                }
                mCurCard.gameObject.SetActive(true);
                mCurCard.SetNum(curcard, mPlayerPosType);
                SetFixeMask(mCurCard); //定缺遮罩
                mAllHandCardList.Add(mCurCard);
            }
        }
        else
        {
            mCurCard.canHuList = null;
            mCurCard.gameObject.SetActive(false);
        }
        //关闭打出的牌里面与我选中牌相同牌的遮罩
        if (mPlayerPosType == PlayerPosType.Self)
            mRoot.SetSameCardMaskShow(-1);
    }

    /// <summary>
    /// 获取所有的玩家的手
    /// </summary>
    public void GetAllPlayerHandCards(List<int> cards, bool isSelf)
    {
        if (cards != null && cards.Count > 0)
        {
            int len = cards.Count;
            int gridLen = mHandCardList.Count;
            MjCollectCard card;
            for (int i = 0; i < len; i++)
            {
                if (i < gridLen)
                    card = mHandCardList[i];
                else
                {
                    card = NGUITools.AddChild(mHandGrid.gameObject, mHandCard.gameObject).GetComponent<MjCollectCard>();
                }
                card.gameObject.SetActive(true);
                card.SetNum(cards[i], mPlayerPosType);
                //card.SetCardDown(isSelf);
            }
            for (int i = len; i < gridLen; i++)
                mHandCardList[i].gameObject.SetActive(false);
            mHandGrid.Reposition();
            Transform tran = mHandGrid.transform;
            float angel = 90f;
            if (isSelf)
                return;
            tran.Rotate(new Vector3(1, 0, 0), angel);
        }
    }


    /// <summary>
    /// 设置摸起来的牌位置
    /// </summary>
    /// <param name="curcard"></param>
    public void SetCurCardPos(int curcard)
    {
        List<Transform> list = mHandGrid.GetChildList();
        if (list.Count > 0)
        {
            Vector3 pos = list[list.Count - 1].localPosition;
            float x = mHandGrid.CellWidth > 0 ? 1 : -1;
            mCurCard.transform.localPosition = new Vector3(pos.x + 7f * x, pos.y, pos.z);
            mCurCard.gameObject.SetActive(true);
            mCurCard.SetNum(curcard, mPlayerPosType);
        }
    }

    /// <summary>
    /// 设置当没有摸起来的牌，但是要打牌的时候，最后一张手牌往右边移动
    /// </summary>
    public void SetOneOutCardPos()
    {
        List<Transform> list = mHandGrid.GetChildList();
        if (list.Count > 0)
        {
            Vector3 pos = list[list.Count - 1].localPosition;
            float x = mHandGrid.CellWidth > 0 ? 1 : -1;
            list[list.Count - 1].localPosition = new Vector3(pos.x + 1f * x, pos.y, pos.z);
        }
    }

    /// <summary>
    /// 初始化手牌，除自己外的玩家
    /// </summary>
    /// <param name="count">手牌数量</param>
    /// <param name="isCurCard">是否有摸起来的牌</param>
    public virtual void InitHandCards(int count, bool isCurCard)
    {
        int card = isCurCard ? 1 : 0;
        InitHandCards(null, count, card);
    }



    protected virtual void AddOneHandCard(int num = -1)
    {
    }
    #endregion

    #region 打出的牌

    public virtual void InitCollectCard(List<int> cards)
    {
        int len = cards == null ? 0 : cards.Count;
        int gridLen = mCollectCardList.Count;
        MjCollectCard card;
        for (int i = 0; i < len; i++)
        {
            if (i < gridLen)
                card = mCollectCardList[i];
            else
            {
                card = NGUITools.AddChild(mCollectGrid.gameObject, mCollectCard.gameObject).GetComponent<MjCollectCard>();
                mCollectCardList.Add(card);
            }
            card.gameObject.SetActive(true);
            //设置数字
            card.SetNum(cards[i], mPlayerPosType);
        }
        for (int i = len; i < gridLen; i++)
            mCollectCardList[i].gameObject.SetActive(false);
        mCollectGrid.Reposition();
        //SetCollectCardPos(len);
    }

    /// <summary>
    /// 获取当前打出的牌
    /// </summary>
    /// <returns></returns>
    public virtual GameObject GetCurOutCard()
    {
        int len = mCollectGrid.GetChildList().Count;
        if (len > 0)
            return mCollectCardList[len - 1].gameObject;
        else
            return null;
    }

    /// <summary>
    /// 设置当前牌特效是否显示
    /// </summary>
    /// <param name="isshow"></param>
    public virtual void SetCurOutCardShow(bool isshow)
    {
        if (mCurCardArrow == null)
        {
            return;
        }
        mCurCardArrow.gameObject.SetActive(isshow);
        if (isshow)
        {
            GameObject obj = GetCurOutCard();
            if (obj == null)
            {
                mCurCardArrow.gameObject.SetActive(false);
            }
            else
            {
                mCurCardArrow.position = obj.transform.position + new Vector3(0, 8, 0);

            }
        }
    }


    #endregion

    #region 设置收集的牌和碰吃的牌里面需要显示遮罩的牌
    /// <summary>
    /// 设置收集的牌和碰吃的牌里面需要显示遮罩的牌
    /// </summary>
    /// <param name="num">小于0表示隐藏</param>
    public void SetSameCardMaskShow(int num)
    {
        SetCollectSameCardMaskShow(num);//打出的牌
        SetPengSameCardMaskShow(num);//碰的牌和杠的牌
    }
    /// <summary>
    /// 设置收集的牌里面需要显示遮罩的牌      胡的牌里的遮罩
    /// </summary>
    /// <param name="num">小于0表示隐藏</param>
    public void SetCollectSameCardMaskShow(int num)
    {
        int gridLen = mCollectCardList.Count;
        for (int i = 0; i < gridLen; i++)
        {
            if (mCollectCardList[i].mNum == num)
                mCollectCardList[i].SetMaskShow(true);
            else
                mCollectCardList[i].SetMaskShow(false);
        }

        int huLen = mHuCardList.Count;
        for (int i = 0; i < huLen; i++)
        {
            if (mHuCardList[i].mNum == num)
                mHuCardList[i].SetMaskShow(true);
            else
                mHuCardList[i].SetMaskShow(false);
        }
    }

    /// <summary>
    /// 设置碰牌或者吃牌里面需要显示遮罩的牌
    /// </summary>
    /// <param name="num">小于0表示隐藏</param>
    private void SetPengSameCardMaskShow(int num)
    {
        int gridLen = mPengCardList.Count;
        for (int i = 0; i < gridLen; i++)//碰牌
            mPengCardList[i].SetMaskShow(num);
        gridLen = mChiCardList.Count;
        for (int i = 0; i < gridLen; i++)//吃牌
            mChiCardList[i].SetMaskShow(num);
    }


    /// <summary>
    /// 定缺后的  定缺遮罩
    /// </summary>
    private void SetFixeMask(MjCollectCard card)
    {

        //if (MJGameModel.Inst.eFixeType != null && MJGameModel.Inst.eFixeType != 0)
        {
            int[] fixe = null;
            if (mMySeatid == MJGameModel.Inst.mMySeatId)//是我自己
                fixe = MJGameModel.Inst.eFixe;
            else//其他人
            {
                if (MJGameModel.Inst.AllPlayerEfixe != null && MJGameModel.Inst.AllPlayerEfixe.Count > 0)
                    fixe = MJGameModel.Inst.AllPlayerEfixe[mMySeatid];
            }
            if (fixe == null)
            {
                card.SetFixeMaskShow(false);
                return;
            }
            if (card.mNum < fixe[1] && card.mNum > fixe[0])
            {
                card.SetFixeMaskShow(true);
            }
            else
            {
                card.SetFixeMaskShow(false);
            }
        }

    }


    #endregion

    #region 设置自己手牌位置，打完牌或者碰杠后居中
    /// <summary>
    /// 设置自己手牌位置，打完牌或者碰杠后居中
    /// </summary>
    /// <param name="count">牌数量</param>
    private void SetMyHandCardPos(int count)
    {
        Vector3 vec = mInitHandCardRootPos;
        vec.x = (13 - count) / 4 * Mathf.Abs(mHandGrid.CellWidth) + mInitHandCardRootPos.x;
        mHandGrid.transform.localPosition = vec;
    }
    #endregion

    #region 碰或者杠  胡
    public void InitPengCards(eMJInstructionsType instype, List<PengStruct> cards)
    {
        if (mPengGrid == null)
            return;
        int len = cards == null ? 0 : cards.Count;
        List<MjPengCard> cardlist;
        if (instype == eMJInstructionsType.PENG)
            cardlist = mChiCardList;
        //else if (instype == eOptStatus.PENG)
        //    cardlist = mPengCardList;
        else
            cardlist = mOtherCardList;
        int gridLen = cardlist.Count;
        MjPengCard card;
        for (int i = 0; i < len; i++)
        {
            if (i < gridLen)
                card = cardlist[i];
            else
            {
                card = NGUITools.AddChild(mPengGrid.gameObject, mPengCard.gameObject).GetComponent<MjPengCard>();
                cardlist.Add(card);
            }
            card.gameObject.SetActive(true);

            //设置数字
            card.SetPengNum(instype, mMySeatid, cards[i]);
        }
        for (int i = len; i < gridLen; i++)
            cardlist[i].gameObject.SetActive(false);
        mPengGrid.Reposition();
        //SetPengCardPos(len);
    }

    public void InitGangCard(eMJInstructionsType instype, List<GangStruct> cards)
    {
        if (mPengGrid == null)
            return;
        int len = cards == null ? 0 : cards.Count;
        int gridLen = mGangCardList.Count;
        MjPengCard card;
        for (int i = 0; i < len; i++)
        {
            if (i < gridLen)
                card = mGangCardList[i];
            else
            {
                card = NGUITools.AddChild(mPengGrid.gameObject, mPengCard.gameObject).GetComponent<MjPengCard>();
                mGangCardList.Add(card);
            }
            card.gameObject.SetActive(true);

            //设置数字
            card.SetGangNum(instype, mMySeatid, cards[i], mPlayerPosType);
        }
        for (int i = len; i < gridLen; i++)
            mGangCardList[i].gameObject.SetActive(false);
        mPengGrid.Reposition();
    }
    public void InitHuCard(eMJInstructionsType instype, List<HuStruct> cards)
    {
        if (mHuGrid == null)
            return;
        int len = cards == null ? 0 : cards.Count;
        int gridLen = mHuCardList.Count;
        MjCollectCard card;
        for (int i = 0; i < len; i++)
        {
            if (i < gridLen)
                card = mHuCardList[i];
            else
            {
                card = NGUITools.AddChild(mHuGrid.gameObject, mHuCard.gameObject).GetComponent<MjCollectCard>();
                mHuCardList.Add(card);
            }
            card.gameObject.SetActive(true);
            //设置数字
            card.SetNum(cards[i].card, mPlayerPosType);
            card.SetHuCardIsGL(cards[i].isGl);//设置胡牌 是否高亮
        }
        for (int i = len; i < gridLen; i++)
            mHuCardList[i].gameObject.SetActive(false);
        mHuGrid.Reposition();
    }
    #endregion

    #region 换三张
    /// <summary>
    /// 换3张处理手牌
    /// </summary>
    public virtual void InitChaneThreeHandCards(List<int> data)
    {
        MJGameModel.Inst.mCurSlectCardList.Clear();
        List<MjMyHandCard> newChangeThree = new List<MjMyHandCard>();
        if (data == null)
            return;
        for (int i = 0; i < data.Count; i++)
        {
            for (int j = 0; j < mAllHandCardList.Count; j++)
            {
                MjMyHandCard obj = mAllHandCardList[j] as MjMyHandCard;

                if (obj.mNum == data[i] && !newChangeThree.Contains(obj))
                {
                    newChangeThree.Add(obj);
                    break;
                }
            }
        }
        for (int i = 0; i < newChangeThree.Count; i++)
        {
            newChangeThree[i].ChangeThree();
            newChangeThree[i].mIsSelect = true;
            MJGameModel.Inst.mCurSlectCardList.Add(newChangeThree[i]);
        }
        if (newChangeThree[0].mNum < 10)
            MJGameModel.Inst.CurSlectCardListType = eCardType.TIAO;
        else if (newChangeThree[0].mNum < 20)
            MJGameModel.Inst.CurSlectCardListType = eCardType.TONG;
        else if (newChangeThree[0].mNum < 30)
            MJGameModel.Inst.CurSlectCardListType = eCardType.WAN;
    }
    /// <summary>
    ///换3张 后的事件处理
    /// </summary>
    public void InitSureChaneThree(MJoptInfoData data)
    {
        List<MjMyHandCard> newChangeThree = new List<MjMyHandCard>();
        for (int i = 0; i < data.cards.Count; i++)
        {
            for (int j = 0; j < mAllHandCardList.Count; j++)
            {
                MjMyHandCard obj = mAllHandCardList[j] as MjMyHandCard;

                if (obj.mNum == data.cards[i] && !newChangeThree.Contains(obj))
                {
                    newChangeThree.Add(obj);
                    break;
                }
            }
        }
        for (int i = 0; i < newChangeThree.Count; i++)
        {
            newChangeThree[i].SureChangeThree();
            mHandCardList.Remove(newChangeThree[i]);
            mSureChangeThreeList.Add(newChangeThree[i]);
        }
        //mChangeThreeGrid.Reposition();
        mHandGrid.Reposition();

    }

    /// <summary>
    /// 设置换三张是否显示
    /// </summary>
    /// <param name="isShow"></param>
    public void ChangeThreeObj(bool isShow)
    {
        mChangeThree.SetActive(isShow);
    }

    /// <summary>
    /// 设置换三张牌数值
    /// </summary>
    /// <param name="cards"></param>
    public void SetChangeThreeCardsNum(List<int> cards)
    {
        if (cards == null)
            return;
        for (int i = 0; i < mChangeThreeCards.Length; i++)
            mChangeThreeCards[i].SetNum(cards[i], mPlayerPosType);
    }

    /// <summary>
    /// 旋转三张牌
    /// </summary>
    /// <param name="t"></param>
    /// <param name="augle"></param>
    public void RotaThreeObj(int t, float augle)
    {
        mChangeThree.transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (t != 0)
        {
            iTween.RotateTo(mChangeThree, iTween.Hash("rotation", new Vector3(0, augle, 0), "islocal", true, "time", 1, "easetype", iTween.EaseType.linear, "oncompletetarget", gameObject, "oncomplete", "OnRotateOver"));
        }
    }
    /// <summary>
    /// 旋转完成
    /// </summary>
    private void OnRotateOver()
    {
        StartCoroutine("DelayHideThree");
    }

    IEnumerator DelayHideThree()
    {
        yield return new WaitForSeconds(1);
        mChangeThree.SetActive(false);
        mChangeThree.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    #endregion
}
