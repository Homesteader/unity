using UnityEngine;
using cn.sharesdk.unity3d;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public class SdkManager : MonoBehaviour
{

    public static SdkManager instance;

    public ShareSDK ssdk;

    public Texture2D icon;

    void Awake()
    {
        instance = this;

        ssdk.authHandler += OnAuthResultHandler;///授权回调
        ssdk.shareHandler += OnShareResultHandler;///分享回调
        ssdk.showUserHandler += OnGetUserInfoResultHandler;///获取用户信息回调
        ssdk.getFriendsHandler += OnGetFriendsResultHandler;///获取好友回调
        ssdk.followFriendHandler += OnFollowFriendResultHandler;///
    }


    /// <summary>
    /// 授权回调
    /// </summary>
    /// <param name="reqID"></param>
    /// <param name="state"></param>
    /// <param name="type"></param>
    /// <param name="result"></param>
    void OnAuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        Global.Inst.GetController<NetLoadingController>().ShowLoading(false);
        SQDebug.Log("caonima===" + reqID + " state=" + state + " type=" + type);
        if (state == ResponseState.Success)
        {
            SQDebug.Log("微信授权成功正在获取微信信息");
            print("authorize success !" + "Platform :" + type);
            ssdk.GetUserInfo(PlatformType.WeChat);
        }
        else if (state == ResponseState.Fail)
        {
            SQDebug.LogError("微信授权失败");
#if UNITY_ANDROID
            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            SQDebug.LogError("微信授权失败");
            print("cancel !");
        }
    }

    /// <summary>
    /// 获取用户信息回调
    /// </summary>
    /// <param name="reqID"></param>
    /// <param name="state"></param>
    /// <param name="type"></param>
    /// <param name="result"></param>
    void OnGetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            SQDebug.Log("获取微信信息成功,正在登陆中...");
            print("get user info result :");
            print(MiniJSON.jsonEncode(result));
            print("Get userInfo success ! Platform :" + type);

            //获取用户信息
            print(MiniJSON.jsonEncode(ssdk.GetAuthInfo(PlatformType.WeChat)));
            SQDebug.LogError(MiniJSON.jsonEncode(ssdk.GetAuthInfo(PlatformType.WeChat)));

            Hashtable uuInfo = null;

#if UNITY_ANDROID
            uuInfo = ssdk.GetAuthInfo(PlatformType.WeChat);
#elif UNITY_IPHONE
            uuInfo = result;
#endif
            if (uuInfo.Contains("openID") || uuInfo.Contains("res"))
            {
                Dictionary<string, object> loginData = new Dictionary<string, object>();
                loginData.Add("openId", uuInfo["openID"].ToString());
                loginData.Add("headUrl", uuInfo["userIcon"].ToString());
                loginData.Add("nickName", Helper.Base64Encode(uuInfo["userName"].ToString()));
                loginData.Add("sex", result["sex"]);
                //Global.It.mLoginCtrl.WeChatLogin(loginData);

                SQDebug.LogError("发送登陆指令。。。。。。。");
            }
            else
            {
                SQDebug.LogError("没发送登陆指令。。。。。。。" + uuInfo);
            }
        }
        else if (state == ResponseState.Fail)
        {
            SQDebug.LogError("微信获取信息失败");
#if UNITY_ANDROID
            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            SQDebug.LogError("微信获取信息失败");
            print("cancel !");
        }
    }

    /// <summary>
    /// 获取好友回调
    /// </summary>
    /// <param name="reqID"></param>
    /// <param name="state"></param>
    /// <param name="type"></param>
    /// <param name="result"></param>
    void OnGetFriendsResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            print("get friend list result :");
            print(MiniJSON.jsonEncode(result));
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            print("cancel !");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reqID"></param>
    /// <param name="state"></param>
    /// <param name="type"></param>
    /// <param name="result"></param>
    void OnFollowFriendResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            print("Follow friend successfully !");
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            print("cancel !");
        }
    }


    /// <summary>
    /// 分享回调
    /// </summary>
    /// <param name="reqID"></param>
    /// <param name="state"></param>
    /// <param name="type"></param>
    /// <param name="result"></param>
    void OnShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        SQDebug.PrintToScreen("caonima===" + reqID + " state=" + state + " type=" + type);
        if (state == ResponseState.Success)
        {
            print("share successfully - share result :");
            print(MiniJSON.jsonEncode(result));
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            print("cancel !");
        }
    }

    /// <summary>
    /// 微信授权
    /// </summary>
    public void AuthWeChat()
    {
        if (!ssdk.IsAuthorized(PlatformType.WeChat))
        {
            SQDebug.PrintToScreen("没有授权，登陆");
            ssdk.Authorize(PlatformType.WeChat);
        }
        else
        {
            SQDebug.PrintToScreen("授权了，登陆");
            ssdk.GetUserInfo(PlatformType.WeChat);
        }
    }

    /// <summary>
    /// 关闭授权
    /// </summary>
    public void CanelWeChat()
    {
        if (ssdk.IsAuthorized(PlatformType.WeChat))
        {
            ssdk.CancelAuthorize(PlatformType.WeChat);
        }
    }

    private string imagePath = "/ScreenShot.png";

    private string iconPath = "ShareIcon/icon.png";

    /// <summary>
    /// 邀请微信好友
    /// </summary>
    /// <param name="str"></param>
    public void InviteFriends(string nameId, string str = "")
    {
        if (!File.Exists(Application.persistentDataPath + iconPath))
        {
            SQDebug.PrintToScreen("VistiteFriend222=" + Application.persistentDataPath + iconPath);
            byte[] b = icon.EncodeToPNG();

            SQDebug.PrintToScreen("VistiteFriend2221=" + b.Length);
            try
            {
                File.WriteAllBytes(Application.persistentDataPath + iconPath, b);
            }
            catch (Exception ex)
            {
                SQDebug.PrintToScreen(ex);
            }


            SQDebug.PrintToScreen("VistiteFriend2223=" + Application.persistentDataPath + iconPath);
        }

        SQDebug.LogError(Application.persistentDataPath + iconPath);
        //
        ShareContent content = new ShareContent();
        content.SetTitle("58麻将0429");
        content.SetUrl("http://139.224.57.97/down/index.html");
        content.SetText("房间号：" + nameId + "\n" + str);
        content.SetImagePath(Application.persistentDataPath + iconPath);
        content.SetShareType(ContentType.Webpage);

        SQDebug.PrintToScreen("VistiteFriend2224=");
        ssdk.ShareContent(PlatformType.WeChat, content);
    }

    public void ShareSettlement()
    {

    }


    //分享游戏给我的好友
    public void ShowGameToMyFriends()
    {
        //byte[] b = icon.EncodeToPNG();
        //File.WriteAllBytes(iconPath, b);

        ShareContent content = new ShareContent();
        content.SetTitle("，玩法最全面的沈阳麻将！");
        content.SetUrl("http://139.224.57.97/down/index.html");

        content.SetText("起手一枝花，手气好到家！");
        content.SetImagePath(Application.persistentDataPath + iconPath);
        content.SetShareType(ContentType.Webpage);
        SQDebug.Log("分享");
        ssdk.ShareContent(PlatformType.WeChat, content);
    }

    /// <summary>
    /// 分享到朋友圈
    /// </summary>
    public void ShowGameToWeChatMoments()
    {
        byte[] b = icon.EncodeToPNG();
        File.WriteAllBytes(Application.persistentDataPath + iconPath, b);
        SQDebug.LogError(Application.persistentDataPath + iconPath);

        ShareContent content = new ShareContent();
        content.SetTitle("，玩法最全面的！起手一枝花，手气好到家！");
        content.SetText("，玩法最全面的！起手一枝花，手气好到家！");
        content.SetImagePath(Application.persistentDataPath + iconPath);
        content.SetUrl("http://139.224.57.97/down/index.html");

        content.SetShareType(ContentType.Webpage);

        ssdk.ShareContent(PlatformType.WeChatMoments, content);
    }


    public void ShareGame(PlatformType plat)
    {
        //byte[] b = icon.EncodeToPNG();
        //File.WriteAllBytes(Application.persistentDataPath + iconPath, b);
        SQDebug.LogError(Application.persistentDataPath + iconPath);

        ShareContent content = new ShareContent();
        content.SetTitle("，沈阳麻将！");
        content.SetText("，玩法最全面的！起手一枝花，手气好到家！");
        content.SetImagePath(Application.persistentDataPath + imagePath);
        content.SetUrl("http://139.224.57.97/down/index.html");

        content.SetShareType(ContentType.Webpage);
        SQDebug.Log("分享");
        ssdk.ShareContent(plat, content);
    }

    /// <summary>
    /// 分享给微信好友
    /// </summary>
    public void ShowToWeChatFriend()
    {
        StartCoroutine(JiePing(PlatformType.WeChat));
    }

    /// <summary>
    /// 分享到朋友圈
    /// </summary>
    public void ShowToWeChatMonments()
    {
        StartCoroutine(JiePing(PlatformType.WeChatMoments));
    }


    /// <summary>
    /// 截屏分享
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    IEnumerator JiePing(PlatformType platType = PlatformType.WeChat)
    {
        yield return new WaitForSeconds(0.2f);

        CaptureScreenshot2(new Rect(0, 0, Screen.width, Screen.height), Application.persistentDataPath + imagePath);

        yield return new WaitForSeconds(0.5f);

        ShareContent content = new ShareContent();
        content.SetImagePath(Application.persistentDataPath + imagePath);
        content.SetShareType(ContentType.Image);

        ssdk.ShareContent(platType, content);
    }

    Texture2D CaptureScreenshot2(Rect rect, string imagePath)
    {
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(imagePath, bytes);
        return screenShot;
    }

    /// <summary>
    /// 截图
    /// </summary>
    public void CaptureScreenshot(CallBack call)
    {
        StartCoroutine(ScreenShot(call));
    }

    private IEnumerator ScreenShot(CallBack call)
    {
        yield return new WaitForSeconds(0.2f);
        yield return new WaitForEndOfFrame();
        CaptureScreenshot2(new Rect(0, 0, Screen.width, Screen.height), Application.persistentDataPath + imagePath);
        if (call != null)
            call();
    }
}
