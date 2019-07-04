using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MjChiCard : MonoBehaviour
{
    public MeshFilter[] mMesh;//牌
    public MJSenceRoot mRoot;
    public Camera mCamera;
    private List<int> mOpt;//指令
    private MJoptInfoData mOptInfo;
    private bool mIsPress;//是否已按下
    private Transform mTran;
    private CallBack<MJoptInfoData, List<int>> mSelectCall;

    private void Awake()
    {
        mTran = transform;
    }

    public void SetData(MJoptInfoData optList, List<int> info, CallBack<MJoptInfoData, List<int>> call)
    {
        mOpt = info;
        mOptInfo = optList;
        for (int i = 0; i < info.Count; i++)
            mMesh[i].mesh = mRoot.mMeshs[info[i]];
        gameObject.SetActive(true);
        mSelectCall = call;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (UICamera.hoveredObject != null)
                return;
            Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                Debug.Log("选中的" + hit.transform.name);
                if (hit.transform == mTran)
                {
                    mIsPress = true;
                    Debug.Log("选中的" + mTran.name);
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (mIsPress)
            {
                mIsPress = false;
                SendChi();
            }
        }
    }


    private void SendChi()
    {
        if (mSelectCall != null)
            mSelectCall(mOptInfo, mOpt);
    }
}
