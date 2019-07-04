using UnityEngine;
using System.Collections;

public class ShareController : BaseController
{
    public ShareController() : base("ShareView", "Windows/ShareView/ShareView")
    {

    }

    /// <summary>
    /// 分享截图
    /// </summary>
    public void SetCaptureScreenshot()
    {
#if WECHAT
        SixqinSDKManager.Inst.SetCaptureScreenshot();
#endif
        ShareView view = OpenWindow() as ShareView;
        view.SetShareType(eShareType.game);
    }
}
