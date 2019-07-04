using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public enum eMyArrangement
{
    Horizontal,
    Vertical,
    CellSnap,
}

public enum eMyPivot
{
    TopLeft,
    Top,
    TopRight,
    Left,
    Center,
    Right,
    BottomLeft,
    Bottom,
    BottomRight
}

public class MyGrid : MonoBehaviour
{
    public eMyArrangement arrangment = eMyArrangement.Horizontal;
    public float cellWidth = 100;
    public float cellHeight = 100;
    public int rowLimit = 0;
    public eMyPivot pivot = eMyPivot.TopLeft;

    public bool animateSmoothly = false;
    public bool hideInactive = false;


    private static MyGrid inst;
    public static MyGrid Inst { get { if (inst == null) inst = new MyGrid(); return inst; } }


    public void OnReposition(List<Transform> list, eMyArrangement arr, eMyPivot pivot, float width, bool anismooth = false)
    {
        switch (arr)
        {
            case eMyArrangement.Horizontal: UpdateHorizontal(list, pivot, width, anismooth); break;
            case eMyArrangement.Vertical: UpdateVertical(list, pivot, width, anismooth); break;
        }
    }

    private void UpdateHorizontal(List<Transform> list, eMyPivot pivot, float width, bool anismooth)
    {
        float x = width * (list.Count - 1) / 2;
        Vector3 pos = -Vector3.right * x;
        float delta = width;
        switch (pivot)
        {
            case eMyPivot.TopLeft: pos = Vector3.zero; break;
            case eMyPivot.Top: break;
            case eMyPivot.TopRight: pos = Vector3.zero; delta = -width; break;
            case eMyPivot.Left: pos = Vector3.zero; break;
            case eMyPivot.Center: break;
            case eMyPivot.Right: pos = Vector3.zero; delta = -width; break;
            case eMyPivot.BottomLeft: break;
            case eMyPivot.Bottom: pos = Vector3.zero; break;
            case eMyPivot.BottomRight: pos = Vector3.zero; delta = -width; break;
        }


        for (int i = 0, count = list.Count; i < count; i++)
        {
            UpdatePostion(list[i], pos, anismooth);
            pos += Vector3.right * delta;
        }
    }

    private void UpdateVertical(List<Transform> list, eMyPivot pivot, float height, bool anismooth)
    {
        float y = -height * (list.Count - 1) / 2;
        Vector3 pos = Vector3.up * y;
        float delta = height;
        switch (pivot)
        {
            case eMyPivot.TopLeft: pos = Vector3.zero; delta = -height; break;
            case eMyPivot.Top: pos = Vector3.zero; delta = -height; break;
            case eMyPivot.TopRight:; pos = Vector3.zero; delta = -height; break;
            case eMyPivot.Left: break;
            case eMyPivot.Center: break;
            case eMyPivot.Right: break;
            case eMyPivot.BottomLeft: pos = Vector3.zero; break;
            case eMyPivot.Bottom: pos = Vector3.zero; break;
            case eMyPivot.BottomRight: pos = Vector3.zero; break;
        }

        for (int i = 0, count = list.Count; i < count; i++)
        {
            UpdatePostion(list[i], pos, anismooth);
            pos += Vector3.up * delta;
        }
    }

    private void UpdatePostion(Transform trans, Vector3 pos, bool anismooth)
    {
        if (anismooth)
            trans.DOLocalMove(pos, 0.5f);
        else trans.localPosition = pos;
    }
}
