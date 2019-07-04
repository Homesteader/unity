using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FGrid : MonoBehaviour
{
    public enum FArrangement
    {
        Horizontal,
        Vertical,
    }
    /// <summary>
    /// 排列方式，横向和纵向
    /// </summary>
    public FArrangement arrangement = FArrangement.Horizontal;
    /// <summary>
    /// 每个元素的宽度
    /// </summary>
    public float CellWidth = 100f;
    /// <summary>
    /// 每个元素的高度
    /// </summary>
    public float CellHeight = 100f;
    /// <summary>
    /// 每一列或者每一行的最大数量,0表示不限制
    /// </summary>
    public int maxPerLine = 0;
    /// <summary>
    /// 是否隐藏未显示的
    /// </summary>
    public bool hideInactive = true;

   

    public List<Transform> GetChildList()
    {
        List<Transform> list = new List<Transform>();
        int count = transform.childCount;
        Transform child;

        for (int i = 0; i < count; i++)
        {
            child = transform.GetChild(i);
            //排除隐藏的
            if (hideInactive && (!child || !child.gameObject.activeInHierarchy))
                continue;
            list.Add(child);
        }
        return list;
    }


    public virtual void Reposition()
    {
        enabled = true;
        List<Transform> list = GetChildList();
        int count = list.Count;//总数
        int curline = 0;//当前第几列或第几行
        int index = 0;//当前第几个

        Transform item;
        for (int i = 0; i < count; i++)
        {
            item = list[i];
            if (arrangement == FArrangement.Horizontal)
                item.localPosition = new Vector3(CellWidth * index, 0, CellHeight * curline);
            else
                item.localPosition = new Vector3(CellWidth * curline, 0, CellHeight * index);

            if (++index >= maxPerLine && maxPerLine != 0)
            {
                index = 0;
                curline++;
            }
        }
    }
}
