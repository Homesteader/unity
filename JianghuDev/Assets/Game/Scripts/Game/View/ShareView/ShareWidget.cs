using UnityEngine;
using System.Collections;
using cn.sharesdk.unity3d;

public class ShareWidget : BaseViewWidget
{
    #region
    /// <summary>
    /// 点击分享给朋友
    /// </summary>
    public void OnShareToFriendClick()
    {
#if WECHAT
            SixqinSDKManager.Inst.ShowGameToMyFriends(PlatformType.WeChat);
#endif
    }
    /// <summary>
    /// 点击分享到朋友圈
    /// </summary>
    public void OnShareToCircleClick()
    {
#if WECHAT
            SixqinSDKManager.Inst.ShowGameToWeChatMoments(PlatformType.WeChatMoments);
#endif
    }


    public void OnCloseClick()
    {
        Close<ShareWidget>();
    }
#endregion
}
