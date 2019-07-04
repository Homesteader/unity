using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MjMyHandCard : MjCollectCard
{

    public Camera mCamera;
    public Camera mSelectCamera;
    public MjCollectCard mCard;
    public Transform mTran;
    private bool mIsPress; //是否按下
    private bool mIsMove; //是否移动
    public bool mIsSelect; //是否被选中
    private bool mIsCanSelect = true; //是否可以被选中
    private Vector3 mPos;
    private Vector3 mMouseStartPos; //鼠标点击起始位置
    private bool mIsBaojiao; //是否处于报叫

    private void Awake()
    {
    }

    public override void SetNum(int num, PlayerPosType type)
    {
        base.SetNum(num, type);
        mIsSelect = false;
        mIsBaojiao = false;
        mIsCanSelect = true;
    }

    /*
        public void InstructCard()
        {
            if (!MJGameModel.Inst.mIsTing && MJGameModel.Inst.mStartGameData.roomInfo.roomState == eRoomState.START) //不是在报叫
            {
                Debug.Log("出牌");
                OptRequest data = new OptRequest();
                data.ins = eMJInstructionsType.HIT;
                data.cards = new List<int>();
                data.cards.Add(mNum);
                //data.otherSeatId = MJGameModel.Inst.mMySeatId;
                //data.seatId = data.otherSeatId;
                Global.Inst.GetController<MJGameController>().SendInstructions(data, null);
            }
            else //听牌
            {
                return;
                OptRequest data = new OptRequest();
                //data.ins = eMJInstructionsType.baojiao;
                data.cards = new List<int>();
                data.cards.Add(mNum);
                //data.otherSeatId = MJGameModel.Inst.mMySeatId;
                //data.seatId = data.otherSeatId;
                Global.Inst.GetController<MJGameController>().SendInstructions(data, null);
            }
        }
        */

    /// <summary>
    /// 设置牌未选中
    /// </summary>
    public void SetCardUnSelect()
    {
        Transform tran = transform;
        tran.localPosition = new Vector3(tran.localPosition.x, 0, tran.localPosition.z);
        mIsSelect = false;
        mRoot.SetSameCardMaskShow(-1);
    }

    /// <summary>
    /// 设置报叫
    /// </summary>
    /// <param name="isCanSelect">是否可以选择</param>
    public void SetBaojiao(bool isCanSelect)
    {
        mIsBaojiao = true;
        mIsSelect = true;
        mIsCanSelect = isCanSelect;
        if (mIsCanSelect)
            transform.localPosition = new Vector3(transform.localPosition.x, 1, transform.localPosition.z);
    }

    /// <summary>
    /// 打牌
    /// </summary>
    public void SendOutCard(List<CanHuStruct> canHuList = null)
    {
        if (!MJGameModel.Inst.mIsTing && MJGameModel.Inst.mStartGameData.roomInfo.roomState == eRoomState.START) //不是在报叫
        {

            OptRequest data = new OptRequest();
            data.ins = eMJInstructionsType.HIT;
            data.cards = new List<int>();
            data.cards.Add(mNum);
            MJGameController mGameCtr = Global.Inst.GetController<MJGameController>();
            #region 判断点击的牌是否符合要求   符合要求发送消息   出牌   不符合设置为没选中


            if (MJGameModel.Inst.isHu)
            {
                int curCard = MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].currCard;
                if (curCard > 0 && mNum == curCard)
                {

                }
                else
                {
                    SetCardUnSelect();
                    return;
                }
            }

            if (MJGameModel.Inst.isHasFixeCard)
            {
                if (MJGameModel.Inst.eFixe[0] == 0 || mNum > MJGameModel.Inst.eFixe[0] && mNum < MJGameModel.Inst.eFixe[1])
                {

                }
                else
                {
                    SetCardUnSelect();
                    return;
                }
            }
            if (!MJGameModel.Inst.isMyHit)
            {
                SetCardUnSelect();
                return;
            }
            #endregion
            MJGameModel.Inst.isMyHit = false;
            Debug.Log("出牌");
            mGameCtr.SendOptRequest(data, () =>
            {


            }, false);
            MJGameModel.Inst.hasCanHuListCards = null;
        }
    }
}
