using UnityEngine;
using System.Collections;

public class SceneLoadingController : BaseController {

    public SceneLoadingView mView;

    public SceneLoadingController() : base("SceneLoadingView", "Windows/LoadingView/SceneLoadingView")
    {
        
    }

    public override BaseView OpenWindow()
    {
        mView = base.OpenWindow() as SceneLoadingView;
        mView.gameObject.SetActive(true);
        return mView;
    }
}
