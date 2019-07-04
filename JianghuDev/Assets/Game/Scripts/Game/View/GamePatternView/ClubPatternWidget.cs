using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClubPatternWidget : MonoBehaviour {

    public FUIScrollView mScrollView;

    public ClubPatternItem mItem;

    private List<SendGetRoomListInfo> mRoomList = new List<SendGetRoomListInfo>();


    private void Awake()
    {
        mScrollView.AddEvent(eFuiScrollViewEvent.InitItem, OnInitItem);
        mScrollView.AddEvent(eFuiScrollViewEvent.UpdateItem, OnUpdateItem);
    }


    void Start()
    {

    }


    public void Init() {

        mRoomList = GamePatternModel.Inst.GetConditionRoomList();

        float down = GamePatternModel.Inst.mBaseScoreDown < 0 ? -1 : GamePatternModel.Inst.mBaseScoreDown;
        float top = GamePatternModel.Inst.mBaseScoreTop < 0 ? -1 : GamePatternModel.Inst.mBaseScoreTop;
        List<SendGetRoomListInfo> temp = new List<SendGetRoomListInfo>();
        if (down < 0 && top < 0)
        {//全部
            mRoomList = mRoomList;
        }
        else if (top < 0 && down > 0)
        {//没有上限 
            for (int i = 0; i < mRoomList.Count; i++)
            {
                if (mRoomList[i].rule.baseScore >= down)
                {
                    temp.Add(mRoomList[i]);
                }
            }
            mRoomList = temp;
        }
        else if (down>0 && top>0)
        {
            for (int i = 0; i < mRoomList.Count; i++)
            {
                if (mRoomList[i].rule.baseScore >= down && mRoomList[i].rule.baseScore<=top)
                {
                    temp.Add(mRoomList[i]);
                }
            }
            mRoomList = temp;
        }

        int count = mRoomList == null ? 0 : mRoomList.Count;
        mScrollView.SetData(count);
    }


    public void UpdateClubPatternWidget(int index, SendGetRoomListInfo data) {
        //GameObject go = mScrollView.GetChildByIndex(index);
        //if (go!=null) {
        //    mRoomList[index] = data;
        //    ClubPatternItem item = go.GetComponent<ClubPatternItem>();
        //    if (item!=null) {
        //        item.InitUI(data);
        //    }
        //}
    }

    #region scroll
    /// <summary>
    /// 初始化item
    /// </summary>
    /// <param name="args"></param>
    private void OnInitItem(params object[] args)
    {
        int index = (int)args[0];
        SendGetRoomListInfo data = mRoomList[index];
        GameObject obj = NGUITools.AddChild(mScrollView.gameObject, mItem.gameObject);
        obj.SetActive(true);
        obj.GetComponent<ClubPatternItem>().InitUI(data);
        mScrollView.InitItem(index, obj);
    }

    /// <summary>
    /// 刷新item
    /// </summary>
    /// <param name="args"></param>
    private void OnUpdateItem(params object[] args)
    {
        int index = (int)args[0];
        GameObject obj = args[1] as GameObject;
        SendGetRoomListInfo data = mRoomList[index];
        obj.SetActive(true);
        obj.GetComponent<ClubPatternItem>().InitUI(data);
    }
    #endregion

}
