using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FCollegeGrid : FGrid {

    public int mAddDeltaNum = 0;//每行或者每一列增加的个数

    public override void Reposition()
    {
        enabled = true;
        List<Transform> list = GetChildList();
        int count = list.Count;//总数
        int curline = 0;//当前第几列或第几行
        int index = 0;//当前第几个
        float initX =0;
        float initY = 0;
        int max = maxPerLine;

        Transform item;
        for (int i = 0; i < count; i++)
        {
            item = list[i];
            if (arrangement == FArrangement.Horizontal)
                item.localPosition = new Vector3(CellWidth * index + initX, 0, CellHeight * curline + initY);
            else
                item.localPosition = new Vector3(CellWidth * curline + initX, 0, CellHeight * index + initY);

            if (++index >= max && maxPerLine != 0)
            {
                index = 0;
                curline++;
                max += mAddDeltaNum;
                initX = arrangement == FArrangement.Horizontal ? -mAddDeltaNum * curline / 2 * CellWidth : 0;
                initY = arrangement == FArrangement.Vertical ? -mAddDeltaNum * curline / 2 * CellHeight : 0;
            }
        }
    }
}
