using UnityEngine;
using System.Collections;

public class NetLoadingView : BaseView
{
    public GameObject mLoading;//加载中
    private const float WaitTime = 1f;//等待多少秒后才显示loading
    private bool mIsShowing;//是否在显示loading中
    private float mTime;
    public void ShowLoading(bool isshow,bool isNow= false)
    {
        mLoading.SetActive(false);
        gameObject.gameObject.SetActive(isshow);
        mTime = WaitTime;
        mIsShowing = false;
        if (isNow) {
            mLoading.SetActive(true);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (mTime > 0)
        {
            mTime -= Time.deltaTime;
        }
        else if (!mIsShowing)
        {
            mIsShowing = true;
            mLoading.SetActive(true);
        }
    }

}
