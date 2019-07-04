using UnityEngine;
using System.Collections.Generic;

public class WelfareWidget : BaseViewWidget
{
    public Camera mSelectCamera;
    public GameObject texItem;
    public GameObject mTran;
    private int curIndex = 0;
    private int indexdirection = -1;
    private int childCount;
    private Vector3 pressVec;

    private List<GameObject> objs;

    private bool isPress = false;
    private float timeDown = 8;
    private float dis = 300;
    private Vector3 mTempPos;//临时位置
    private Vector3 mRepressPos;//松开点的位置
    private Vector3 mPresPos;
    //private Vector3 xdPos;
    //private Vector3 worldParntPos;
    // Use this for initialization
    public void Start()
    {
        base.Start();
        mSelectCamera = transform.parent.parent.parent.GetComponent<Camera>();
        //    worldParntPos = mTran.transform.parent.position;
        objs = new List<GameObject>();
        GetTextureMsg();
        childCount = objs.Count;
    }


    public void Update()
    {
        if (!isPress)
        {
            timeDown = timeDown - Time.deltaTime;
            if (timeDown <= 0)
            {
                MoveTex(dis);
                timeDown = 8;
                dis = 0.3f;
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

    private void GetTextureMsg()
    {
        List<ConfigDada> con = ConfigManager.GetConfigs<WelfareConfig>();
        SetWdt(con);
    }

    private void SetWdt(List<ConfigDada> list)
    {
        int con = list == null ? 0 : list.Count;

        for (int i = 0; i < con; i++)
        {
            WelfareConfig config = list[i] as WelfareConfig;
            if (config != null)
            {
                GameObject tex;
                tex = Instantiate(texItem, mTran.transform) as GameObject;
                if (tex != null)
                {
                    tex.SetActive(true);
                    Assets.LoadIcon(config.texUrel, (t) => tex.GetComponent<UITexture>().mainTexture = t);
                    objs.Add(tex);
                }
            }
        }
        mTran.GetComponent<UIGrid>().Reposition();
    }

    public void ClosBtnClick()
    {
        Close();
    }

    public void OnPressTex()
    {
        isPress = true;
        pressVec = mTran.transform.localPosition;
        mPresPos = mSelectCamera.ScreenToWorldPoint(GetTouchPos());
    }

    public void OnRePressTex()
    {
        mRepressPos = mSelectCamera.ScreenToWorldPoint(GetTouchPos());
        dis = Vector3.Magnitude(mRepressPos - mPresPos);
        if (dis >= 0.2)
        {
            indexdirection = mRepressPos.x - mPresPos.x > 0 ? -1 : 1;
        }
        timeDown = 0f;
        isPress = false;
    }

    private void MoveTex(float distance)
    {
        if (distance < 0.20)
        {
            mTran.transform.localPosition = pressVec;
        }
        else
        {
            curIndex = curIndex + indexdirection;

            if (curIndex < 0)
            {
                indexdirection = 1;
                curIndex = 0;
            }
            else if (curIndex >= childCount)
            {
                indexdirection = -1;
                curIndex = childCount - 1;
            }

            SQDebug.Log(curIndex);
            Vector3 ve = objs[curIndex].transform.localPosition;
            SQDebug.Log(ve);
            mTran.transform.localPosition = new Vector3(ve.x * -1, ve.y * -1);
        }
    }

}
