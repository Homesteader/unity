using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MjCollectCard : MonoBehaviour
{

    public MeshFilter mMesh;//牌
    public MJSenceRoot mRoot;
    public GameObject mMask;//遮罩
    public GameObject mFixeMask;//定缺遮罩
    public GameObject mHuGlMask;//胡牌高亮设置
    public GameObject mPromptMask;//胡牌提示
    public PlayerPosType mPlayerType;
    public int mNum;
    public List<CanHuStruct> canHuList; //缓存 可以胡的数据


    public virtual void SetNum(int num, PlayerPosType type)
    {
        mNum = num;
        mPlayerType = type;
        mMesh.mesh = mRoot.mMeshs[num];
        canHuList = null;
        List<HitCardCanHuStruct> hitCList = MJGameModel.Inst.hitCardCanHuList;
        if (hitCList != null)
        {
            if (MJGameModel.Inst.hasCanHuListCards != null && MJGameModel.Inst.hasCanHuListCards.Contains(num))
            {

                for (int i = 0; i < hitCList.Count; i++)
                {
                    if (hitCList[i].hitCard == num && !MJGameModel.Inst.isHu)
                    {
                        SetPromptMas(hitCList[i].canHuList);
                    }
                }
            }
            else
                SetPromptMas(null);
        }
        else
        {
            SetPromptMas(null);
        }
    }

    /// <summary>
    /// 设置胡牌提示
    /// </summary>
    public void SetPromptMas(List<CanHuStruct> huList)
    {
        bool isShow;
        if (huList == null)
        {
            canHuList = null;
            isShow = false;
        }
        else
        {
            isShow = true;
            canHuList = huList;
        }
        if (mPromptMask != null)
            mPromptMask.SetActive(isShow);
    }

    /// <summary>
    /// 设置胡牌是否高亮
    /// </summary>
    /// <param name="isGL"></param>
    public void SetHuCardIsGL(bool isGL)
    {
        if (mFixeMask != null)
            mHuGlMask.SetActive(isGL);
    }

    /// <summary>
    /// 设置 定缺遮罩
    /// </summary>
    /// <param name="isshow"></param>
    public void SetFixeMaskShow(bool isShow)
    {
        if (mFixeMask != null)
            mFixeMask.SetActive(isShow);
    }

    /// <summary>
    /// 设置遮罩显示
    /// </summary>
    /// <param name="isshow"></param>
    public void SetMaskShow(bool isshow)
    {
        if (mMask != null)
            mMask.SetActive(isshow);
    }
    /// <summary>
    /// 换3张的牌上提
    /// </summary>
    public void ChangeThree()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 1,
           transform.localPosition.z);
    }

    public void SureChangeThree()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 2,
           transform.localPosition.z);
        //GameObject.Destroy(this);

    }
    /// <summary>
    /// 设置麻将倒下
    /// </summary>
    public void SetCardDown(bool isSelf = false)
    {
        Transform tran = mMesh.transform.parent;
        float angel = 90f;
        if (isSelf)
        {
            angel = -45f;
        }
        tran.Rotate(new Vector3(1, 0, 0), angel);
    }
}
