using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MjPengCard : MonoBehaviour
{
    public MeshFilter[] mMeshs;//背景
    public MJSenceRoot mRoot;
    public GameObject mArrow;//箭头，标记是谁打的
    public GameObject mChiPanel;//吃的牌是哪张
    public GameObject[] mMask;//遮罩
    public GameObject mOtherAngang;//其他玩家暗杠，自己只能看到背面

    private eMJInstructionsType mInsType;//操作类型
    private Vector3 mArrowPos1 = new Vector3(0, 7.5f, -1.73f);//箭头位置1
    private Vector3 mArrowPos2 = new Vector3(0, 9.03f, -1.37f);//箭头位置2
    private int[] mNum;//牌数字

    void Awake()
    {
        mNum = new int[mMeshs.Length];
    }

    /// <summary>
    /// 设置数字
    /// </summary>
    /// <param name="num1">第1个数字</param>
    /// <param name="num2">第2个数字</param>
    /// <param name="num3">第3个数字</param>
    /// <param name="num4">第4个数字，如果为-1表示没有第四个</param>
    private void SetNum(eMJInstructionsType instype, int num1, int num2, int num3, int num4 = -1)
    {
        mInsType = instype;

        //设置数据
        mNum[0] = num1;
        mNum[1] = num2;
        mNum[2] = num3;
        mNum[3] = num4;
        mNum[4] = num4;


        mMeshs[0].mesh = mRoot.mMeshs[num1];
        mMeshs[1].mesh = mRoot.mMeshs[num2];
        mMeshs[2].mesh = mRoot.mMeshs[num3];
        if (num4 == -1)
            mMeshs[3].gameObject.SetActive(false);
        else
        {
            mMeshs[3].gameObject.SetActive(true);
            mMeshs[3].mesh = mRoot.mMeshs[num4];
        }
        mMeshs[4].gameObject.SetActive(false);
    }

    /// <summary>
    /// 设置暗杠
    /// </summary>
    private void SetAnGang(PlayerPosType playerPosType)
    {
        mMeshs[3].gameObject.SetActive(false);
        mMeshs[4].gameObject.SetActive(true);
        //if (playerPosType != PlayerPosType.Self && mOtherAngang != null)//不是自己暗杠，只能看到牌背面
        //{
        //    mOtherAngang.SetActive(true);
        //    mMeshs[4].gameObject.SetActive(false);
        //    for (int i = 0; i < mMeshs.Length - 1; i++)
        //        mMeshs[i].mesh = null;
        //}
        //else
        mOtherAngang.SetActive(false);
    }

    private void SetArrowShow(bool isshow)
    {
        mArrow.SetActive(isshow);
        mChiPanel.SetActive(isshow);
    }

    public void SetPengNum(eMJInstructionsType _type, int myseatid, PengStruct cards)
    {
        SetArrowShow(false);
        if (cards.cards.Count == 1)
            SetNum(_type, cards.cards[0], cards.cards[0], cards.cards[0]);
        else if (cards.cards.Count < 4)
            SetNum(_type, cards.cards[0], cards.cards[1], cards.cards[2]);
        else
            SetNum(_type, cards.cards[0], cards.cards[1], cards.cards[2], cards.cards[3]);
        if (_type == eMJInstructionsType.PENG)//碰
        {
            if (cards.otherSeatId != null && cards.otherSeatId != myseatid && cards.otherSeatId != 0)
            {
                SetArrowPos(myseatid, cards.otherSeatId, 3);
            }
        }
        //else if (_type == eOptStatus)//吃
        //{
        //    mChiPanel.SetActive(true);
        //    SetChiPanelPos((eMJChiType)cards.type);
        //}
    }

    /// <summary>
    /// 设置杠牌
    /// </summary>
    /// <param name="_type"></param>
    /// <param name="myseatid"></param>
    /// <param name="cards"></param>
    /// <param name="playerPosType"></param>
    public void SetGangNum(eMJInstructionsType _type, int myseatid, GangStruct cards, PlayerPosType playerPosType)
    {
        SetArrowShow(false);
        mOtherAngang.SetActive(false);
        if (cards.cards.Count == 1)
            SetNum(_type, cards.cards[0], cards.cards[0], cards.cards[0], cards.cards[0]);
        else if (cards.cards.Count < 4)
            SetNum(_type, cards.cards[0], cards.cards[1], cards.cards[2]);
        else
            SetNum(_type, cards.cards[0], cards.cards[1], cards.cards[2], cards.cards[3]);
        if (_type == eMJInstructionsType.GANG && (cards.gangType == eGangType.DIANGANG || cards.gangType == eGangType.WANGANG))//普通杠
        {
            if (cards.otherSeatId != null && cards.otherSeatId != myseatid && cards.otherSeatId != 0)
            {
                SetArrowPos(myseatid, cards.otherSeatId, 4);
            }
        }
        if (_type == eMJInstructionsType.GANG && cards.gangType == eGangType.ANGANG)//暗杠
            SetAnGang(playerPosType);
    }

    /// <summary>
    /// 设置碰和杠箭头位置
    /// </summary>
    /// <param name="myseat"></param>
    /// <param name="otherseat"></param>
    /// <param name="count"></param>
    private void SetArrowPos(int myseat, int otherseat, int count)
    {
        mArrow.SetActive(true);
        int t = otherseat - myseat;
        Vector3 pos = count == 3 ? mArrowPos1 : mArrowPos2;
        Quaternion rotate;
        if (MJGameModel.Inst.totalPlayerCount == 2)
        {
            rotate = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (MJGameModel.Inst.totalPlayerCount == 3)
        {
            if (myseat == MJGameModel.Inst.mMySeatId || otherseat == MJGameModel.Inst.mMySeatId)
            {
                if (t == 1 || t == -2)//右边的
                {
                    rotate = Quaternion.Euler(new Vector3(0, 90, 0));
                }
                else
                {
                    rotate = Quaternion.Euler(new Vector3(0, -90, 0));
                }
            }
            else
            {
                rotate = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }
        else
        {
            if (t == 2 || t == -2)//对家
            {
                rotate = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (t == -1 || t == 3)//左边的玩家
            {
                rotate = Quaternion.Euler(new Vector3(0, -90, 0));
            }
            else//右边
                rotate = Quaternion.Euler(new Vector3(0, 90, 0));
        }
        mArrow.transform.localPosition = pos;
        mArrow.transform.localRotation = rotate;
    }

    /// <summary>
    /// 设置吃的牌
    /// </summary>
    /// <param name="_type"></param>
    private void SetChiPanelPos(eMJChiType _type)
    {
        int index = 0;
        if (_type == eMJChiType.ZHONG)
            index = 1;
        else if (_type == eMJChiType.HOU)
            index = 2;
        float x = mMeshs[index].transform.parent.localPosition.x;
        mChiPanel.transform.localPosition = new Vector3(x, 7.53f, 0);
    }


    /// <summary>
    /// 设置遮罩显示
    /// </summary>
    /// <param name="num"></param>
    public void SetMaskShow(int num)
    {
        if (mMask == null)
            return;
        for (int i = 0; i < mMask.Length; i++)
        {
            if (num <= 0)
                mMask[i].SetActive(false);
            else if (mNum[i] == num && mMeshs[i].gameObject.activeInHierarchy)
                mMask[i].SetActive(true);
            else
                mMask[i].SetActive(false);
        }
    }
}
