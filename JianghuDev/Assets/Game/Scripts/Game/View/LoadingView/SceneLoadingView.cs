using UnityEngine;
using System.Collections;

public class SceneLoadingView : BaseView {

    /// <summary>
    /// 滑动条
    /// </summary>
    public UISlider mLoadSlider;


    protected override void Awake()
    {
        base.Awake();
        mLoadSlider.value = 0;
    }

    #region 

    /// <summary>
    /// 显示加载进度
    /// </summary>
    /// <param name="progress"></param>
    public void ShowLoadProgress(float progress) {
        mLoadSlider.value = progress;
    }


    #endregion
}
