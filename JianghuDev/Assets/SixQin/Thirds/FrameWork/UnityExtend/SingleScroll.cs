using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 用于单个纵向排列
/// </summary>
public class SingleScroll : GameEventUnity
{
    public enum eMoveType
    {
        LeftAndUp,
        RightAndDown,
    }
    #region 公共数据

    /// <summary>
    /// 起始位置
    /// </summary>
    public int StartIndex;
    /// <summary>
    /// 是否异步加载
    /// </summary>
    public bool IsAsynchronous;
    public Vector2 Offeset;
    public int ShowCount = 10;//最多显示10个item
    public float SpaceY = 10;//每个itemy轴上的间隔
    #endregion
    private Transform mTran;
    private UIPanel mPanel;
    private UIScrollView mScrollView;
    private Vector3 mInitPanelPos;//panel初始位置
    private Vector2 mInitPanelClip;//panel初始clip

    private List<ChatScrollItem> mItemList = new List<ChatScrollItem>();//所有创建的item列表
    private Dictionary<int, int> mItemIndexDic = new Dictionary<int, int>();//item在mItemList中对应位置
    private int mMinIndex = 0;//当前最小index
    private int mMaxIndex = 0;//当前最大index
    private int mInitMinIndex = 0;//初始化创建时最小index
    private int mInitMaxIndex = 0;//初始化创建时最大index
    private int mAllCount = 0;//item总数量
    private float mNextUpPos = 0;//下一次比较的位置
    private float mNextDownPos = 0;//下一次比较的位置
    private float mOffsetByStartIndex;//根据初始index计算出的偏移量
    private float mPanelHeight;//panel的高度

    private bool mIsHorizontal = true;//是否是横向
    private bool mIsLoadOver = false;//是否加载完成

    void Awake()
    {
        mTran = transform;
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
        //创建显示个数的item
        for (int i = 0; i < ShowCount; i++)
        {
            GameObject obj = new GameObject((i + 1).ToString());
            obj.transform.parent = mTran;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            ChatScrollItem item = obj.AddComponent<ChatScrollItem>();
            item.indexInScroll = i;
            mItemList.Add(item);

        }

        mIsHorizontal = mScrollView.movement == UIScrollView.Movement.Horizontal ? true : false;
        mPanel.onClipMove = OnClipMove;
        mInitPanelPos = mTran.localPosition;
        mInitPanelClip = mPanel.clipOffset;
        mPanelHeight = mPanel.height;
    }

    void Start()
    {
    }

    public float GetNextDownPos()
    {
        return mNextDownPos;
    }

    public int GetMaxIndex()
    {
        return mMaxIndex;
    }

    #region 初始化

    /// <summary>
    /// 更具显示的数量，生成Item
    /// </summary>
    public void InitInstantiateItems()
    {
        //创建显示个数的item
        for (int i = 0; i < ShowCount; i++)
        {
            GameObject obj = new GameObject((i + 1).ToString());
            obj.transform.parent = mTran;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            ChatScrollItem item = obj.AddComponent<ChatScrollItem>();
            item.indexInScroll = i;
            mItemList.Add(item);

        }
    }

    private void InitData()
    {
        float x = mPanel.baseClipRegion.x - mPanel.baseClipRegion.z / 2;
        float y = mPanel.baseClipRegion.w / 2 + mPanel.baseClipRegion.y;
        mIsLoadOver = false;

        //添加item
        StartCoroutine(InitItems());
    }

    IEnumerator InitItems()
    {

        ChatScrollItem item;
        float itemHeight;//item的高度

        int start = 0;
        int count = ShowCount;
        int index = start;//数据中index
        int n = 0;
        for (int i = start; i < count; i++)
        {
            item = mItemList[n];
            if (index >= mAllCount)//如果创建的item超过总数就抛出初始事件
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                DispatchEvent(eFuiScrollViewEvent.InitItem, index, item.gameObject);
                itemHeight = NGUIMath.CalculateRelativeWidgetBounds(item.transform).size.y;
                item.contentHeight = itemHeight;//所占用的高度
                item.indexInData = n;//在数据里面的位置
            }

            SetInitItemPos(n);
            if (!mItemIndexDic.ContainsKey(index))
                mItemIndexDic.Add(index, n);
            n++;
            if (IsAsynchronous)
                yield return 1;
            index++;
        }
        mMinIndex = start;//滑动顶上的index
        if (count > mAllCount)
        {
            if (mAllCount == 0)
                mMaxIndex = 0;
            else
                mMaxIndex = start + mAllCount - 1;
        }
        else
            mMaxIndex = mAllCount == 0 ? 0 : count - 1;//滑动底下的index
        //初始化panel的位置和裁剪
        SetInitPanelPosAndClip();
        mIsLoadOver = true;
        yield return 0;
    }

    /// <summary>
    /// 设置初始化item位置
    /// </summary>
    /// <param name="index">item的下标</param>
    private void SetInitItemPos(int index)
    {
        ChatScrollItem obj = mItemList[index];
        ChatScrollItem last = index <= 0 ? null : mItemList[index - 1];

        if (last == null)
            obj.transform.localPosition = Vector3.zero;
        else
        {
            Vector3 pos = last.transform.localPosition;//前一个item的位置
            pos.y = pos.y - last.contentHeight - SpaceY;//前一个item位置减去占用的高度和间隔就是当前item的初始位置
            obj.transform.localPosition = pos;
        }
    }

    /// <summary>
    /// 初始化panel的位置和裁剪
    /// </summary>
    private void SetInitPanelPosAndClip()
    {
        Vector3 pos = mInitPanelPos;
        float allHeight = 0;//所有item占用的高度
        int itemCount = 0;//有效的item数量
        for (int i = 0; i < mItemList.Count; i++)
        {
            allHeight += mItemList[i].contentHeight;
            if (mItemList[i].contentHeight != 0)
                itemCount++;
        }


        allHeight += (itemCount - 1) * SpaceY;//加上每个item直接的间隙
        float sizeHeight = mPanel.height;//panel裁剪的范围（高度）
        //if (allHeight <= sizeHeight)//
        {
            //设置下次检测滑动到底端的位置
            if (allHeight < 0)
                allHeight = 0;
            mNextDownPos = pos.y + (allHeight - sizeHeight); ;
            //设置下次检测滑动到顶端的位置
            mNextUpPos = pos.y;
            return;
        }
        pos.y += allHeight - sizeHeight;
        mTran.localPosition = pos;//panel的位置
        Vector2 clip = mPanel.clipOffset;
        clip.y -= (allHeight - sizeHeight);
        mPanel.clipOffset = clip;//panel的裁剪
        //设置下次检测滑动到底端的位置
        mNextDownPos = pos.y;
        //设置下次检测滑动到顶端的位置
        mNextUpPos = pos.y - (allHeight - sizeHeight);
        SQDebug.Log("mNextDownPos:" + mNextDownPos + " mNextUpPos:" + mNextUpPos);
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


    #endregion

    #region 滑动
    private void OnClipMove(UIPanel panel)
    {
        if (!mIsLoadOver)//未初始化完成不能滑动
            return;

        //纵向
        if (mTran.localPosition.y > mNextUpPos)//往上滑动
        {
            if (mMaxIndex >= mAllCount - 1)//超过最大数量就返回
            {
                return;
            }

            MoveOver(eMoveType.LeftAndUp);
        }
        else if (mTran.localPosition.y < mNextDownPos)//往下滑动
        {
            if (mMinIndex <= 0)//到第一个位置就返回
            {
                return;
            }

            MoveOver(eMoveType.RightAndDown);
        }
    }

    private void MoveOver(eMoveType movetype)
    {
        int index = 0;
        Transform trans;
        ChatScrollItem item = null;
        float itemHeight = 0;//item高度
        if (movetype == eMoveType.LeftAndUp)//向左或向上滑动
        {
            if (mTran.localPosition.y <= mNextDownPos)
                return;
            if (mItemIndexDic.TryGetValue(mMaxIndex, out index))
                trans = mItemList[index].transform;
            else
            {
                SQDebug.LogError("maxIndex is null");
                return;
            }
            //最上边的item移动到最下边
            //最新的最底面的item
            index = mItemIndexDic[mMinIndex];//数据位置对应的gameobject位置
            item = mItemList[index];
            itemHeight = item.contentHeight;
            mMaxIndex++;
            //新的最底端item数据对应的gameobject位置
            int topIndex = mItemIndexDic[mMinIndex];
            mItemIndexDic[mMaxIndex] = topIndex;
            //下次滑动到顶端的位置
            //SQDebug.Log("mNextUpPos:" + mNextUpPos);
            mNextUpPos += (itemHeight + SpaceY);
            //新的最上边的item index
            mItemIndexDic.Remove(mMinIndex);//删除之前最顶端的记录
            mMinIndex++;

            item.gameObject.SetActive(true);
            DispatchEvent(eFuiScrollViewEvent.UpdateItem, mMaxIndex, item.transform.GetChild(0).gameObject);
            //重新设置最底下item的位置和大小
            item.contentHeight = NGUIMath.CalculateRelativeWidgetBounds(item.transform).size.y;
            ChatScrollItem lastMax = mItemList[mItemIndexDic[mMaxIndex - 1]];//之前最大的item
            Vector3 vec = lastMax.transform.localPosition;
            vec.y -= (lastMax.contentHeight + SpaceY);
            item.transform.localPosition = vec;//最终位置
            //下次滑动到底端的位置
            float last = mNextDownPos;
            mNextDownPos += (item.contentHeight + SpaceY);
            //SQDebug.Log("mMinIndex：" + mMinIndex + "   mMaxIndex: " + mMaxIndex + " contentHeight:" + item.contentHeight + "mNextUpPos:" + mNextUpPos + "mNextDownPos:" + mNextDownPos + item.contentHeight  + " last:" + (mNextDownPos - last - 50));
        }
        else//向右或向下滑动
        {
            if (mTran.localPosition.y >= mNextUpPos)
                return;
            //最下边的item移动到最上边
            //最新的最顶端的item
            index = mItemIndexDic[mMaxIndex];//数据位置对应的gameobject位置
            item = mItemList[index];
            itemHeight = item.contentHeight;
            mMinIndex--;
            //新的最顶端item数据对应的gameobject位置
            int topIndex = mItemIndexDic[mMaxIndex];
            mItemIndexDic[mMinIndex] = topIndex;
            //下次滑动到顶端的位置
            mNextDownPos -= (itemHeight + SpaceY);
            //新的最底端的item index
            mItemIndexDic.Remove(mMaxIndex);//删除之前最底端的记录
            mMaxIndex--;

            item.gameObject.SetActive(true);
            DispatchEvent(eFuiScrollViewEvent.UpdateItem, mMinIndex, item.transform.GetChild(0).gameObject);
            //重新设置最底下item的位置和大小
            item.contentHeight = NGUIMath.CalculateRelativeWidgetBounds(item.transform).size.y;
            ChatScrollItem lastMin = mItemList[mItemIndexDic[mMinIndex + 1]];//之前最小的item
            Vector3 vec = lastMin.transform.localPosition;
            vec.y += (item.contentHeight + SpaceY);
            item.transform.localPosition = vec;//最终位置
            //下次滑动到顶端的位置
            float last = mNextUpPos;
            mNextUpPos -= (item.contentHeight + SpaceY);
            //SQDebug.Log("mMinIndex：" + mMinIndex + "   mMaxIndex: " + mMaxIndex + " contentHeight:" + item.contentHeight + "mNextUpPos:" + mNextUpPos + item.contentHeight + "mNextDownPos:" + mNextDownPos + " last:"+(mNextUpPos - last + 50));
        }
    }

    #endregion


    #region 添加
    #region 从最后添加
    /// <summary>
    /// 从最后添加一个item
    /// </summary>
    public void AddItemByLast()
    {
        AddOneItemByLast();
    }

    public void UpdateAllCount(int mAllCount) {
        this.mAllCount = mAllCount;
    }


    /// <summary>
    /// 从最后添加一个item
    /// </summary>
    private void AddOneItemByLast()
    {

        bool b = IsTheLastItemOnShow(mAllCount);//最后一个item是否在panel显示范围内
        if (!b)//如果不在直接总数加1
        {
            mAllCount += 1;//总数量+1
        }
        else//如果在范围内将新的item添加到最后
        {
            mScrollView.DisableSpring();
            //最上边的item移动到最下边
            //最新的最底面的item
            int index = 0;//数据位置对应的gameobject位置
            if (mAllCount < ShowCount)//总数<可以展示的最大数量
            {
                if (mAllCount != 0)
                    index = mMaxIndex + 1;
            }
            else
                index = mItemIndexDic[mMinIndex];
            ChatScrollItem item = mItemList[index];
            item.gameObject.SetActive(true);
            if (mAllCount >= ShowCount)//总数>=可以展示的最大数量
            {
                float itemHeight = item.contentHeight;
                mMaxIndex++;
                //新的最底端item数据对应的gameobject位置
                int topIndex = mItemIndexDic[mMinIndex];
                mItemIndexDic[mMaxIndex] = topIndex;
                //下次滑动到顶端的位置
                //SQDebug.Log("mNextUpPos:" + mNextUpPos);
                mNextUpPos += (itemHeight + SpaceY);
                //新的最上边的item index
                mItemIndexDic.Remove(mMinIndex);//删除之前最顶端的记录
                mMinIndex++;
                DispatchEvent(eFuiScrollViewEvent.UpdateItem, mMaxIndex, item.transform.GetChild(0).gameObject);
                SQDebug.Log("刷新");
            }
            else
            {
                //新的最底端item数据对应的gameobject位置
                int topIndex = mItemIndexDic[mMaxIndex];
                if (mAllCount != 0)
                {
                    mMaxIndex++;
                    mItemIndexDic[mMaxIndex] = topIndex + 1;
                }
                DispatchEvent(eFuiScrollViewEvent.InitItem, mMaxIndex, item.gameObject);
                SQDebug.Log("创建");
            }
            //重新设置最底下item的位置和大小
            item.contentHeight = NGUIMath.CalculateRelativeWidgetBounds(item.transform).size.y;
            if (mAllCount != 0)//总数为0
            {
                //重新设置最底下item的位置和大小
                ChatScrollItem lastMax = mItemList[mItemIndexDic[mMaxIndex - 1]];//之前最大的item
                Vector3 vec = lastMax.transform.localPosition;
                vec.y -= (lastMax.contentHeight + SpaceY);
                item.transform.localPosition = vec;//最终位置
                                                   //下次滑动到底端的位置
                float last = mNextDownPos;
                mNextDownPos += (item.contentHeight + SpaceY);
                if (mNextDownPos + mInitPanelPos.y > mPanelHeight)//如果底端位置超过了panel的高度才重新计算裁剪
                {
                    //重新设置panel的位置和裁剪
                    vec = mTran.localPosition;
                    float delta = mNextDownPos - vec.y;
                    vec.y = mNextDownPos;
                    mTran.localPosition = vec;//位置
                                              //裁剪
                    Vector2 clip = mPanel.clipOffset;
                    clip.y -= delta;
                    mPanel.clipOffset = clip;
                }
            }
            else
            {
                float last = mNextDownPos;
                mNextDownPos += (item.contentHeight + SpaceY);
            }
            //总数+1
            mAllCount += 1;

            //SQDebug.Log("mMinIndex：" + mMinIndex + "   mMaxIndex: " + mMaxIndex + " contentHeight:" + item.contentHeight + "mNextUpPos:" + mNextUpPos + item.contentHeight + "mNextDownPos:" + mNextDownPos + " last:" + (mNextUpPos - last + 50));
        }
    }
    #endregion

    #region 从最开始添加

    /// <summary>
    /// 从头添加item
    /// </summary>
    /// <param name="count">添加数量</param>
    public void AddItemByAhead(int count)
    {
        AddOneItemByAhead(count);
    }

    /// <summary>
    /// 从头添加item
    /// </summary>
    /// <param name="count">添加数量</param>
    private void AddOneItemByAhead(int count)
    {
        if (count == 0)
            return;
        mMinIndex += count;//最小index往后移动count
        mMaxIndex += count;//最大index往后移动count
        mAllCount += count;
        int[] keys = new int[mItemIndexDic.Count];
        int[] values = new int[mItemIndexDic.Count];
        mItemIndexDic.Keys.CopyTo(keys, 0);
        mItemIndexDic.Values.CopyTo(values, 0);
        mItemIndexDic.Clear();
        for (int i = 0; i < keys.Length; i++)//重新建立索引
            mItemIndexDic.Add(keys[i] + count, values[i]);
    }

    #endregion
    #endregion


    /// <summary>
    /// 是否最后一个item在panel的显示范围内
    /// </summary>
    /// <returns></returns>
    private bool IsTheLastItemOnShow(int allcount)
    {
        if (allcount == 0)
            return true;
        if (mMaxIndex != allcount - 1)//最大index不是最后一个，肯定不在范围内
            return false;
        int index = mItemIndexDic[mMaxIndex];//最后一个item在list中对应index
        ChatScrollItem item = mItemList[index];
        float y = mTran.localPosition.y + item.contentHeight;//如果当期panel位置加上item高度
        if (y > mNextDownPos)//大于最大滑动到底端的位置，说明item在panel显示范围内
            return true;
        return false;
    }

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
        InitInstantiateItems();
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

    /// <summary>
    /// 改变向上滑动到底端的位置
    /// </summary>
    /// <param name="y"></param>
    public void AddDownPos(float y, int index)
    {
        mNextDownPos += y;
        for (int i = index + 1; i <= mMaxIndex; i++)
        {
            int k = mItemIndexDic[i];
            Vector3 vec = mItemList[k].transform.localPosition;
            vec.y -= y;
            mItemList[k].transform.localPosition = vec;
        }
    }
}