using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum eFuiScrollViewEvent
{
    /// <summary>
    /// 初始化item，参数为index
    /// </summary>
    InitItem,
    /// <summary>
    /// 刷新item，参数为要刷新的item（Gameobject）
    /// </summary>
    UpdateItem,
}


public class FUIScrollView : GameEventUnity
{
    public enum eMoveType
    {
        LeftAndUp,
        RightAndDown,
    }
    #region 公共数据
    /// <summary>
    /// 行数或列数
    /// </summary>
    public int LineCount = 10;
    /// <summary>
    /// 每行或每列数量
    /// </summary>
    public int CountPerLine = 1;
    /// <summary>
    /// 每个item宽度
    /// </summary>
    public float CellWidth = 20;
    /// <summary>
    /// 每个item高度
    /// </summary>
    public float CellHeight = 20;
    /// <summary>
    /// 起始位置
    /// </summary>
    public int StartIndex;
    /// <summary>
    /// 是否异步加载
    /// </summary>
    public bool IsAsynchronous;
    public Vector2 Offeset;
    #endregion

    private UIPanel mPanel;
    private UIScrollView mScrollView;
    private Vector3 mInitPanelPos;//panel初始位置
    private Vector2 mInitPanelClip;//panel初始clip

    private List<GameObject> mItemList = new List<GameObject>();//所有创建的item列表
    private Dictionary<int, int> mItemIndexDic = new Dictionary<int, int>();//item在mItemList中对应位置
    private int mMinIndex = 0;//当前最小index
    private int mMaxIndex = 0;//当前最大index
    private int mInitMinIndex = 0;//初始化创建时最小index
    private int mInitMaxIndex = 0;//初始化创建时最大index
    private int mAllCount = 0;//item总数量
    private float mNextUpPos = 0;//下一次比较的位置
    private float mNextDownPos = 0;//下一次比较的位置
    private Vector3 mCorrectPos;//初始矫正位置
    private float mOffsetByStartIndex;//根据初始index计算出的偏移量

    private bool mIsHorizontal = true;//是否是横向
    private bool mIsLoadOver = false;//是否加载完成
    void Awake()
    {
        mPanel = GetComponent<UIPanel>();
        if (mPanel == null)
        {
            SQDebug.Log("panel is null");
            return;
        }
        mScrollView = GetComponent<UIScrollView>();
        if (mScrollView == null)
        {
            SQDebug.Log("scrollview is null");
            return;
        }
        mIsHorizontal = mScrollView.movement == UIScrollView.Movement.Horizontal ? true : false;
        mPanel.onClipMove = OnClipMove;
        mInitPanelPos = mPanel.transform.localPosition;
        mInitPanelClip = mPanel.clipOffset;
    }

    void Start()
    {
    }

    #region 初始化

    private void InitData()
    {
        SetInitMinMaxIndex();
        float x = mPanel.baseClipRegion.x - mPanel.baseClipRegion.z / 2;
        float y = mPanel.baseClipRegion.w / 2 + mPanel.baseClipRegion.y;
        mCorrectPos = new Vector3(x, y, 0);
        mIsLoadOver = false;

        if (LineCount <= 0 || CountPerLine <= 0 || CellWidth <= 0 || CellHeight <= 0)
        {
            SQDebug.LogError("初始化数据错误");
            return;
        }
        if (StartIndex < 0 || StartIndex >= mAllCount)
        {
            SQDebug.LogError("初始化数据错误");
            return;
        }

        //设置滚动方向
        if (mScrollView.movement == UIScrollView.Movement.Horizontal)
        {
            mIsHorizontal = true;
            mPanel.transform.localPosition = new Vector3(mInitPanelPos.x - mOffsetByStartIndex, mInitPanelPos.y, 0);
            mPanel.clipOffset = new Vector2(mInitPanelClip.x + mOffsetByStartIndex, mInitPanelClip.y);
            mNextUpPos = mPanel.transform.localPosition.x - (Offeset.x + CellWidth * LineCount - mPanel.GetViewSize().x) + mOffsetByStartIndex;
            mNextDownPos = mPanel.transform.localPosition.x + mOffsetByStartIndex;
        }
        else if (mScrollView.movement == UIScrollView.Movement.Vertical)
        {
            mIsHorizontal = false;
            mPanel.transform.localPosition = new Vector3(mInitPanelPos.x, mInitPanelPos.y + mOffsetByStartIndex, 0);
            mPanel.clipOffset = new Vector2(mInitPanelClip.x, mInitPanelClip.y - mOffsetByStartIndex);
            mNextUpPos = mPanel.transform.localPosition.y + (Offeset.y + CellHeight * LineCount - mPanel.GetViewSize().y) - mOffsetByStartIndex;
            mNextDownPos = mPanel.transform.localPosition.y - mOffsetByStartIndex;
        }
        else
        {
            SQDebug.LogError("不支持该方向类型");
            return;
        }
        //添加item
        int count = LineCount * CountPerLine;
        if (mAllCount < count)
            count = mAllCount;
        StartCoroutine(InitItems());
    }

    IEnumerator InitItems()
    {

        int index = mMinIndex;
        GameObject obj = null;
        for (int i = mInitMinIndex; i <= mInitMaxIndex; i++)
        {
            if (index >= mAllCount)//如果创建的item超过总数就抛出初始事件
            {
                obj = NGUITools.AddChild(gameObject, mItemList[0]);
                obj.SetActive(false);
                mItemList.Add(obj);
            }
            else
            {
                DispatchEvent(eFuiScrollViewEvent.InitItem, index);
                obj = mItemList[i];
            }

            SetInitItemPos(i, obj);
            if (!mItemIndexDic.ContainsKey(index))
                mItemIndexDic.Add(index, i);
            if (IsAsynchronous)
                yield return 1;
            index++;
        }
        mIsLoadOver = true;
        yield return 0;
    }

    /// <summary>
    /// 设置初始化item位置
    /// </summary>
    /// <param name="index">item的下标</param>
    /// <param name="obj"></param>
    private void SetInitItemPos(int index, GameObject obj)
    {
        if (obj == null)
        {
            SQDebug.LogError("obj is null");
            return;
        }
        int line = index / CountPerLine;
        int cell = index % CountPerLine;
        float posx = 0;
        float posy = 0;
        if (mIsHorizontal)//横向
        {
            posx = mCorrectPos.x + CellWidth / 2 + line * CellWidth + Offeset.x;
            posy = mCorrectPos.y - CellHeight / 2 - cell * CellHeight - Offeset.y;
        }
        else//纵向
        {
            posx = mCorrectPos.x + CellWidth / 2 + cell * CellWidth + Offeset.x;
            posy = mCorrectPos.y - CellHeight / 2 - line * CellHeight - Offeset.y;
        }
        obj.transform.localPosition = new Vector3(posx, posy, 0);
    }

    private void SetInitMinMaxIndex()
    {
        int allLine = (mAllCount + CountPerLine - 1) / CountPerLine;//总行数或列数
        int allcount = (mAllCount + CountPerLine - 1) / CountPerLine * CountPerLine;//如果占满所有格子的总数
        int createCount = CountPerLine * LineCount;//总共可以创建的格子
        int panelShowItemCount = 0;//panel能完全看到的行数或列数
        if (mIsHorizontal)
            panelShowItemCount = ((int)mPanel.baseClipRegion.z - (int)Offeset.x) / ((int)CellWidth);
        else
            panelShowItemCount = ((int)mPanel.baseClipRegion.w - (int)Offeset.y) / ((int)CellHeight);

        int startLine = (StartIndex + CountPerLine) / CountPerLine;
        if (mAllCount <= createCount)//总数不大于可以创建的格子数
        {
            mInitMinIndex = 0;
            mInitMaxIndex = mAllCount - 1;
            mMinIndex = 0;
            mMaxIndex = mAllCount - 1;
        }
        else
        {
            if (startLine + LineCount - 1 > allLine)//起始位置如果放在第一行或列超出最大行数
            {
                mInitMinIndex = 0;
                mMinIndex = (allLine - LineCount) * CountPerLine;
                mMaxIndex = mAllCount - 1;
                mInitMaxIndex = LineCount * CountPerLine - 1;
            }
            else
            {
                mInitMinIndex = 0;
                mInitMaxIndex = LineCount * CountPerLine - 1;
                mMinIndex = (startLine - 1) * CountPerLine;
                mMaxIndex = (startLine + LineCount - 1) * CountPerLine - 1;
                if (mMaxIndex >= mAllCount)
                    mMaxIndex = mAllCount - 1;
            }
        }
        mOffsetByStartIndex = GetInitPanelOffset(mMinIndex, mMaxIndex, startLine, panelShowItemCount);
    }

    /// <summary>
    /// 获取初始化panel的偏移量
    /// </summary>
    /// <param name="minIndex">当前最小index</param>
    /// <param name="MaxIndex">当前最大index</param>
    /// <param name="startLine">初始行</param>
    /// <param name="panelShowItemCount">panel能完全看到的行数或列数</param>
    /// <returns></returns>
    private float GetInitPanelOffset(int minIndex, int maxIndex, int startLine, int panelShowItemCount)
    {
        int minline = (minIndex + CountPerLine) / CountPerLine;
        int maxline = (maxIndex + CountPerLine) / CountPerLine;
        float offset = mIsHorizontal ? Offeset.x : Offeset.y;
        if (startLine == 1)
            offset = 0;
        if (maxline <= panelShowItemCount)//当前size能显示完所有item
            return 0;
        if (startLine <= maxline - panelShowItemCount - 1)//当前行作为第一行能显示完后面的行
        {
            if (mIsHorizontal)
                return (startLine - minline) * CellWidth + offset;
            else
                return (startLine - minline) * CellHeight + offset;
        }
        else
        {
            if (startLine + panelShowItemCount > maxline)//当前行只能在最后显示
            {
                if (mIsHorizontal)
                    return (maxline - minline + 1) * CellWidth + offset - mPanel.baseClipRegion.z;
                else
                    return (maxline - minline + 1) * CellHeight + offset - mPanel.baseClipRegion.w;
            }
            else
            {
                if (mIsHorizontal)
                    return (startLine - minline) * CellWidth + offset;
                else
                    return (startLine - minline) * CellHeight + offset;
            }
        }
    }
    #endregion

    #region 外部调用
    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="count">item总数</param>
    public void SetData(int count)
    {
        Reset();
        if (StartIndex < 0)
            StartIndex = 0;
        else if (StartIndex >= count)
            StartIndex -= 1;
        mAllCount = count;
        InitData();
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="count">item总数</param>
    /// <param name="startIndex">其实位置，从0开始</param>
    public void SetData(int count, int startIndex)
    {
        Reset();
        if (startIndex < 0)
            StartIndex = 0;
        else if (startIndex >= count)
            StartIndex -= 1;
        mAllCount = count;
        InitData();
    }


    /// <summary>
    /// 外部初始化item
    /// </summary>
    /// <param name="index"></param>
    /// <param name="obj"></param>
    public void InitItem(int index, GameObject obj)
    {
        mItemList.Add(obj);
    }
    #endregion

    #region 滑动
    private void OnClipMove(UIPanel panel)
    {
        if (!mIsLoadOver)//未初始化完成不能滑动
            return;
        if (mIsHorizontal)//横向
        {
            if (panel.transform.localPosition.x < mNextUpPos)//往左滑动
            {
                if (mMaxIndex >= mAllCount - 1)//超过最大数量就返回
                    return;
                mNextUpPos -= CellWidth;
                mNextDownPos -= CellWidth;
                MoveOver(eMoveType.LeftAndUp);
            }
            else if (panel.transform.localPosition.x > mNextDownPos)//往右滑动
            {
                if (mMinIndex <= 0)//到第一个位置就返回
                    return;
                mNextUpPos += CellWidth;
                mNextDownPos += CellWidth;
                MoveOver(eMoveType.RightAndDown);
            }
        }
        else//纵向
        {
            if (panel.transform.localPosition.y > mNextUpPos)//往上滑动
            {
                if (mMaxIndex >= mAllCount - 1)//超过最大数量就返回
                    return;
                mNextUpPos += CellHeight;
                mNextDownPos += CellHeight;
                MoveOver(eMoveType.LeftAndUp);
            }
            else if (panel.transform.localPosition.y < mNextDownPos)//往下滑动
            {
                if (mMinIndex <= 0)//到第一个位置就返回
                    return;
                mNextUpPos -= CellHeight;
                mNextDownPos -= CellHeight;
                MoveOver(eMoveType.RightAndDown);
            }

        }
    }

    private void MoveOver(eMoveType movetype)
    {
        int index = 0;
        Transform trans;
        GameObject obj = null;
        if (movetype == eMoveType.LeftAndUp)//向左或向上滑动
        {

            if (mItemIndexDic.TryGetValue(mMaxIndex, out index))
                trans = mItemList[index].transform;
            else
            {
                SQDebug.LogError("maxIndex is null");
                return;
            }

            for (int i = 0; i < CountPerLine; i++)
            {
                if (mItemIndexDic.TryGetValue(mMinIndex + i, out index))//找到index在list列表中对应位置
                {
                    obj = mItemList[index];
                    if (mMaxIndex + 1 + i > mAllCount - 1)
                    {
                        obj.gameObject.SetActive(false);
                    }
                    else
                    {
                        obj.gameObject.SetActive(true);
                        DispatchEvent(eFuiScrollViewEvent.UpdateItem, mMaxIndex + 1 + i, obj);
                    }
                    SetMoveItemPos(i, movetype, obj, trans);//设置obj位置
                    mItemIndexDic.Remove(mMinIndex + i);
                    if (!mItemIndexDic.ContainsKey(mMaxIndex + i + 1))//将新的index映射到list
                        mItemIndexDic.Add(mMaxIndex + i + 1, index);
                }
            }
            //重新设置当前最小index和最大index
            mMinIndex += CountPerLine;
            mMaxIndex += CountPerLine;
            if (mMaxIndex > mAllCount - 1)
                mMaxIndex = mAllCount - 1;
        }
        else//向右或向下滑动
        {
            if (mItemIndexDic.TryGetValue(mMinIndex, out index))
                trans = mItemList[index].transform;
            else
            {
                SQDebug.LogError("minIndex is null");
                return;
            }
            int mindex = mMaxIndex - mMaxIndex % CountPerLine;
            for (int i = 0; i < CountPerLine; i++)
            {
                if (mItemIndexDic.TryGetValue(mindex + i, out index))//找到index在list列表中对应位置
                {
                    obj = mItemList[index];
                    obj.gameObject.SetActive(true);
                    DispatchEvent(eFuiScrollViewEvent.UpdateItem, mMinIndex - CountPerLine + i, obj);
                    SetMoveItemPos(i, movetype, obj, trans);//设置obj位置
                    mItemIndexDic.Remove(mindex + i);
                    if (!mItemIndexDic.ContainsKey(mMinIndex - CountPerLine + i))//将新的index映射到list
                        mItemIndexDic.Add(mMinIndex - CountPerLine + i, index);
                }
            }
            //重新设置当前最小index和最大index
            mMinIndex -= CountPerLine;
            mMaxIndex = mindex - 1;
        }
    }

    /// <summary>
    /// 设置滑动后item位置
    /// </summary>
    /// <param name="index"></param>
    /// <param name="movetype"></param>
    /// <param name="obj">需要改变位置的item</param>
    /// <param name="trans">参照item，向上或向左滑动为当前mMinIndex位置的item，向下或向右滑动为当前mMaxIndex位置item</param>
    private void SetMoveItemPos(int index, eMoveType movetype, GameObject obj, Transform trans)
    {
        float posx = 0;
        float posy = 0;
        if (movetype == eMoveType.LeftAndUp)
        {
            if (mIsHorizontal)
            {
                posx = trans.localPosition.x + CellWidth;
                posy = mCorrectPos.y - CellHeight / 2 - index * CellHeight - Offeset.y;
            }
            else
            {
                posx = mCorrectPos.x + CellWidth / 2 + index * CellWidth + Offeset.x;
                posy = trans.localPosition.y - CellHeight;
            }
        }
        else if (movetype == eMoveType.RightAndDown)
        {
            if (mIsHorizontal)
            {
                posx = trans.localPosition.x - CellWidth;
                posy = mCorrectPos.y - CellHeight / 2 - index * CellHeight - Offeset.y;
            }
            else
            {
                posx = mCorrectPos.x + CellWidth / 2 + index * CellWidth + Offeset.x;
                posy = trans.localPosition.y + CellHeight;
            }
        }
        obj.transform.localPosition = new Vector3(posx, posy, 0);
    }
    #endregion

    private void Reset()
    {
        mScrollView.DisableSpring();
        mIsLoadOver = false;
        ClearAllChild(transform);
        //yield return 1;
        mItemList.Clear();
        mItemIndexDic.Clear();
        transform.localPosition = mInitPanelPos;
        mPanel.clipOffset = mInitPanelClip;
    }
    public void ClearAllChild(Transform container)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            if (null != container.GetChild(i))
            {
                UnityEngine.Object.Destroy(container.GetChild(i).gameObject);
            }
        }
    }
}
