using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MjCardSelect : MonoBehaviour
{

    public Camera mCamera;
    public Camera mSelectCamera;
    public MjCollectCard mCard;
    public MeshFilter mMesh;//牌
    public MJSenceRoot mSceneRoot;
    private bool mIsPress;//是否按下
    private bool mIsMove;//是否移动
    private Vector3 mPos;
    private Vector3 mMouseStartPos;//鼠标点击起始位置
    private Transform mTran;//选中的
    private MjMyHandCard mSelectCard;//选中的牌
    private Vector3 mTouch;//手位置
    private Vector3 mTempPos;//临时位置

    void Start()
    {
        //StartCoroutine(UpdateClick());
    }

    IEnumerator UpdateClick()
    {
        while (true)
        {
            //SetMouse();
            yield return new WaitForSeconds(0.01f);
        }
    }

    void OnGUI()
    {
        if (!mIsPress && Input.GetMouseButtonDown(0))
        {

            if (UICamera.hoveredObject != null && !UICamera.hoveredObject.name.Equals("WindowsRoot"))
                return;
            Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.transform.gameObject.layer == 8)
                {
                    mMouseStartPos = GetTouchPos();
                    mIsPress = true;
                    mTran = hit.transform.parent;
                    mSelectCard = mTran.GetComponent<MjMyHandCard>();
                    mMesh = mSelectCard.mMesh;
                    SQDebug.Log("选中的" + mTran.name);
                    if (!MJGameModel.Inst.isHu)
                    {

                        Global.Inst.GetController<MJGameController>()
                            .GetHuPromptCard(mSelectCard.canHuList, mSelectCard.transform.localPosition);

                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (mIsPress)
            {
                mIsPress = false;
                if (mIsMove)
                {
                    mIsMove = false;
                    OnMoveOver();
                }
                else
                {
                    OnCardClick();
                }
            }
            mIsMove = false;
        }
        if (mIsPress)
        {
            mTouch = GetTouchPos();
            //拖动
            if (!mIsMove && (mMouseStartPos.x - mTouch.x > 10 || mMouseStartPos.x - mTouch.x < -10 || mMouseStartPos.y - mTouch.y > 10 || mMouseStartPos.y - mTouch.y < -10))
            {
                mCard.gameObject.SetActive(true);
                mCard.mMesh.mesh = mMesh.mesh;
                mIsMove = true;
            }
            if (mIsMove)
            {
                mPos = mSelectCamera.ScreenToWorldPoint(GetTouchPos());
                mCard.transform.position = new Vector3(mPos.x, mPos.y, 0);
            }
        }
    }

    /// <summary>
    /// 获取点击位置
    /// </summary>
    /// <returns></returns>
    private Vector3 GetTouchPos()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return Input.mousePosition;
        else
        {
            if (Input.touchCount > 0)
            {
                mTempPos.x = Input.GetTouch(0).position.x;
                mTempPos.y = Input.GetTouch(0).position.y;
                return mTempPos;
            }
            else
                return Input.mousePosition;
        }
    }

    public void OnCardClick()
    {
        if (mSelectCard == null)
        {
            mIsPress = false;
            mIsMove = false;
            return;
        }
        if (MJGameModel.Inst.mStartGameData.roomInfo.roomState == eRoomState.CHANGETHREE)
        {
            #region 换3张状态下的麻将点击
            if (mSelectCard.mIsSelect)
            {
                mSelectCard.SetCardUnSelect();
                MJGameModel.Inst.mCurSlectCardList.Remove(mSelectCard);
                return;
            }
            int currSelectCard = mSelectCard.mNum;
            int currTypeTotle;
            eCardType currSelectCardType;
            if (currSelectCard < 10)
            {
                currTypeTotle = MJGameModel.Inst.handCardsTiao.Count;
                currSelectCardType = eCardType.TIAO;

            }
            else if (currSelectCard < 20)
            {
                currTypeTotle = MJGameModel.Inst.handCardsTong.Count;
                currSelectCardType = eCardType.TONG;
            }
            else
            {
                currTypeTotle = MJGameModel.Inst.handCardsWan.Count;
                currSelectCardType = eCardType.WAN;
            }
            if (currTypeTotle < 3)
                return;
            if (currSelectCardType == MJGameModel.Inst.CurSlectCardListType)
            {
                //第一张下  选中的上
                if (MJGameModel.Inst.mCurSlectCardList.Count >= 3)
                {
                    MJGameModel.Inst.mCurSlectCardList[0].SetCardUnSelect();
                    MJGameModel.Inst.mCurSlectCardList.RemoveAt(0);
                }
                mSelectCard.transform.localPosition = new Vector3(mTran.localPosition.x,
                    mTran.localPosition.y + 1, mTran.localPosition.z);
                mSelectCard.mIsSelect = true;
                MJGameModel.Inst.mCurSlectCardList.Add(mSelectCard);
            }
            else
            {
                //全下   选中上
                List<MjMyHandCard> mCurSlectList = MJGameModel.Inst.mCurSlectCardList;
                for (int i = mCurSlectList.Count - 1; i >= 0; i--)
                {
                    mCurSlectList[i].SetCardUnSelect();
                    mCurSlectList.Remove(mCurSlectList[i]);
                }
                MJGameModel.Inst.mCurSlectCardList.Clear();
                mSelectCard.transform.localPosition = new Vector3(mTran.localPosition.x,
                    mTran.localPosition.y + 1, mTran.localPosition.z);
                mSelectCard.mIsSelect = true;
                MJGameModel.Inst.mCurSlectCardList.Add(mSelectCard);
                MJGameModel.Inst.CurSlectCardListType = currSelectCardType;
            }
            #endregion
        }
        if (MJGameModel.Inst.mStartGameData.roomInfo.roomState == eRoomState.START)
        {
            #region 游戏中的点击
            if (!mSelectCard.mIsSelect) //未选中的话就抬起来
            {
                if (MJGameModel.Inst.mCurSlectCard != null)
                {
                    Transform tran = MJGameModel.Inst.mCurSlectCard.transform;
                    if (tran != null)
                        tran.localPosition = new Vector3(tran.localPosition.x, 0, tran.localPosition.z);
                    MJGameModel.Inst.mCurSlectCard.SetCardUnSelect();

                }
                mTran.localPosition = new Vector3(mTran.localPosition.x, mTran.localPosition.y + 1,
                    mTran.localPosition.z);
                mSelectCard.mIsSelect = true;
                MJGameModel.Inst.mCurSlectCard = mSelectCard; //设置当前选中的牌
                SetSameCardNum(mSelectCard.mNum);
            }
            else
            {
                if (mSelectCard != null)
                {
                    //mSelectCard.InstructCard();
                    mSelectCard.SendOutCard(mSelectCard.canHuList);
                }
            }
            #endregion
        }
    }

    private void OnMoveOver()
    {
        mCard.gameObject.SetActive(false);
        if (Input.mousePosition.y - mPos.y < 100)
            return;
        {
            //mSelectCard.InstructCard();
            mSelectCard.SendOutCard(mSelectCard.canHuList);
        }
    }

    /// <summary>
    /// 设置打出的牌里面与当前选择的牌相同的牌的遮罩
    /// </summary>
    /// <param name="num"></param>
    private void SetSameCardNum(int num)
    {
        mSceneRoot.SetSameCardMaskShow(num);
    }
}
