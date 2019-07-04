using UnityEngine;
using System.Collections;

public class NetLoadingController : BaseController
{
    private NetLoadingView mView;
    public NetLoadingController():base("NetLoadingView", "Windows/NetLoadingView/NetLoadingView")
    {

    }

    public void ShowLoading(bool isshow,bool isNow = false)
    {
        if (mView == null)
            mView = OpenWindow() as NetLoadingView;
        mView.gameObject.SetActive(true);
        mView.ShowLoading(isshow,isNow);
    }
}
