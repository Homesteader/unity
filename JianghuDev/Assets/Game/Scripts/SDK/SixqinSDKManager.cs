using UnityEngine;
using cn.sharesdk.unity3d;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

public class SixqinSDKManager : MonoBehaviour
{
    public static SixqinSDKManager Inst;

    /// <summary>
    /// 当前平台
    /// </summary>
    public PlatformType mCurPlat;

    private AndroidJavaObject mAJ;
    private string mDownloadUrl;//下载链接

    private string imagePath = "/ScreenShot.png";//截图路径
    private string iconPath = "/Icons.png";//分享icon路径

    private ShareSDK ssdk;

    /// <summary>
	/// 注意,这里也第一个字符为"_"来区别里面的方法是IOS的外部接口还是自己写的方法
	/// </summary>
	private Dictionary<string, MethodInfo> mIOSMethodDic;
    private string openInstallArgs;//扫二维码数据

    #region 发送给SDK的方法名
    [DllImport("__Internal")]
    private static extern string getIPv6(string mHost, string mPort);

    #region ios
#if UNITY_IPHONE
    //[DllImport("__Internal")] public static extern void _Init();
    //[DllImport("__Internal")] public static extern void _Login();
    [DllImport("__Internal")] public static extern void _Pay(string appSign, string ip, string subject, int amount, string body, string orderNo, string notifyUrl, string cpChannel, string description,string extra, string payStyle,string appInfo);
    [DllImport("__Internal")] public static extern void _CopyTextToClipBoard(string text);
    //[DllImport("__Internal")] public static extern void _InvateFriend(int roomId);
    //[DllImport("__Internal")] public static extern void _LoginOut();
    //[DllImport("__Internal")] public static extern void _SetRecieveInvate();
    //[DllImport("__Internal")] public static extern void _WXShare(int plat, string tittle, string desc, string texturePath, string url);
    //[DllImport("__Internal")] public static extern void _AddFriend(string userId);
    //[DllImport("__Internal")] public static extern void _SetFloatShow(int state);
    //[DllImport("__Internal")] public static extern void _WXShareImg(int plat, string path);
    //[DllImport("__Internal")] public static extern void _SelectHeadImgPic(string path);
	//[DllImport("__Internal")] public static extern void _SelectNormalImg(string path);
    [DllImport("__Internal")] public static extern void _GetLocation(int time);
    [DllImport("__Internal")] public static extern void _GetInstallData();
#endif
    #endregion

    #region 安卓
    public const string INIT_SQ_SDK = "InitSQSdk";//初始化SDK
    public const string INIT_GAME = "Init";//初始化房间
    public const string LOGIN = "Login";//登录
    public const string RECHARGE = "Pay";//充值
    public const string INVATE_FRIEND = "InvateFriend";//邀请好友
    public const string LOGIN_OUT = "LoginOut";//注销登录
    public const string COPY_TEXT = "CopyTextToClipBoard";//复制文本
    public const string SET_RECIEVE_INVATE = "SetRecieveInvate";//注册收到邀请
    public const string WX_Share = "WXShare";//分享
    public const string WX_ShareImg = "WXShareImg";//分享图片
    public const string GET_LOCATION = "GetLocation";//获取经纬度,参数定位周期（毫秒）
    public const string SELECT_HEAD_ICON = "SelectHeadImgPic";//获取头像
    public const string SELECT_NORMAL_IMG = "SelectNormalImg";//获取普通图片
    public const string GET_INSTALL_DATA = "GetInstallData";//获取openinstall数据
    #endregion
    #endregion

    private CallBack<string, string> mSelectPicCallBack;//选择图片回调
    private CallBack<string> mSelectHeadPicCallBack;//选择头像回调
    private CallBack<LoginSR.SendLogin> mGetAuthDataCallBack;//获取玩家信息回调

    void Awake()
    {
        Inst = this;
        mIOSMethodDic = new Dictionary<string, MethodInfo>();
        MethodInfo[] infos = GetType().GetMethods();
        for (int i = 0; i < infos.Length; i++)
        {
            if (infos[i].Name.Substring(0, 1) == "_")
            {
                mIOSMethodDic.Add(infos[i].Name, infos[i]);
            }
        }
        //return;
        ssdk = gameObject.AddComponent<ShareSDK>();
        ssdk.authHandler += OnAuthResultHandler;///授权回调
        ssdk.shareHandler += OnShareResultHandler;///分享回调
        ssdk.showUserHandler += OnGetUserInfoResultHandler;///获取用户信息回调
        ssdk.getFriendsHandler += OnGetFriendsResultHandler;///获取好友回调
        ssdk.followFriendHandler += OnFollowFriendResultHandler;///
    }
    void Start()
    {

#if UNITY_ANDROID
        SendMsg(INIT_SQ_SDK, gameObject.name);//初始化SDK，设置回调物体
        //SendMsg("InitPay", "3015488239");//初始化支付
#elif UNITY_IPHONE
        SetTimeout.add(600, OnTimeToGetLocation, false, false);
#endif

    }

    /// <summary>
    /// 向sdk发送消息
    /// </summary>
    /// <param name="funcname">方法名</param>
    /// <param name="args">参数</param>
    public void SendMsg(string funcname, params object[] args)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return;
        }
        string logArgs = "[";
        for (int i = 0; i < args.Length; i++)
        {
            logArgs += args[i] + ",";
        }
        logArgs += "]";
        SQDebug.Log("SDKSendMsg\n" + funcname + "\n" + logArgs + "\n");
        try
        {
#if UNITY_IPHONE
			MethodInfo method = null;
			if (mIOSMethodDic.TryGetValue ("_" + funcname, out method))
			{
				method.Invoke (null, args);
			}
			else
			{
				SQDebug.Log ("Cannot Find IOS API : "+ funcname );
			}
#endif
#if UNITY_ANDROID
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    jo.Call(funcname, args);
                }
            }
#endif
        }
        catch (System.Exception e)
        {
            SQDebug.Log("SDKSendError\n" + e.Message + e.StackTrace + "\n");
        }
    }


    #region JPUSH

    /// <summary>
    /// 初始化jpush
    /// </summary>
    public void InitJPUSH(string userid)
    {
#if !UNITY_EDITOR
#endif
    }

    #endregion

    #region shareSDK


    #region 微信和qq
    #region 回调
    /// <summary>
    /// 授权回调
    /// </summary>
    /// <param name="reqID"></param>
    /// <param name="state"></param>
    /// <param name="type"></param>
    /// <param name="result"></param>
    void OnAuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        
        SQDebug.Log("caonima===" + reqID + " state=" + state + " type=" + type);
        if (state == ResponseState.Success)
        {
            SQDebug.Log("微信授权成功正在获取微信信息");
            print("authorize success !" + "Platform :" + type);
            ssdk.GetUserInfo(type);
        }
        else if (state == ResponseState.Fail)
        {
            Global.Inst.GetController<NetLoadingController>().ShowLoading(false);
            Global.Inst.GetController<CommonTipsController>().CloseWindow();
            SQDebug.Log("授权失败");
#if UNITY_ANDROID
            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            Global.Inst.GetController<NetLoadingController>().ShowLoading(false);
            Global.Inst.GetController<CommonTipsController>().CloseWindow();
            SQDebug.Log("微信授权失败");
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
            SQDebug.Log(type + "用户信息：" + MiniJSON.jsonEncode(ssdk.GetAuthInfo(type)));
            //SQDebug.Log(MiniJSON.jsonEncode(ssdk.GetAuthInfo(type)));

            Hashtable uuInfo = null;

#if UNITY_ANDROID
            uuInfo = ssdk.GetAuthInfo(type);
#elif UNITY_IPHONE
            uuInfo = result;
#endif
            if (uuInfo.Contains("openID") || uuInfo.Contains("res") || uuInfo.Contains("openid"))
            {
                GetLoginData(uuInfo, result, type);
            }
            else
            {
                SQDebug.Log("没发送登陆指令。。。。。。。" + uuInfo);
            }
        }
        else if (state == ResponseState.Fail)
        {
            Global.Inst.GetController<NetLoadingController>().ShowLoading(false);
            SQDebug.Log("微信获取信息失败");
#if UNITY_ANDROID
            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            Global.Inst.GetController<NetLoadingController>().ShowLoading(false);
            SQDebug.Log("微信获取信息失败");
            print("cancel !");
        }
    }
    /// <summary>
    /// 解析登录信息
    /// </summary>
    /// <param name="uuInfo"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private LoginSR.SendLogin GetLoginData(Hashtable uuInfo, Hashtable result, PlatformType type)
    {
#if UNITY_ANDROID
        if (uuInfo.Contains("openID") || uuInfo.Contains("res"))
        {
            LoginSR.SendLogin req = new LoginSR.SendLogin();
            req.headUrl = uuInfo["userIcon"].ToString();
            req.nickname = uuInfo["userName"].ToString();
            //req.openId = uuInfo["userID"].ToString();
            req.openId = uuInfo["unionID"].ToString();
            if (uuInfo.ContainsKey("userGender"))
                req.sex = uuInfo["userGender"].ToString().Equals("m") ? 1 : 2;
            else
                req.sex = 1;

            SQDebug.Log("发送登陆指令。。。。。。。");
            string str = Json.Serializer<LoginSR.SendLogin>(req);
            if (!string.IsNullOrEmpty(str))//将登录信息保存在本地
                PlayerPrefs.SetString("wechat_login_data", str);//获取保存到本地的登录信息
            Global.Inst.GetController<NetLoadingController>().ShowLoading(false);
            if (mGetAuthDataCallBack != null)
                mGetAuthDataCallBack(req);
            //loginData.Add("nickName", Helper.Base64Encode(uuInfo["userName"].ToString()));
            return req;
        }
#elif UNITY_IPHONE
        if (uuInfo.Contains("openid") || uuInfo.Contains("res"))
        {
            LoginSR.SendLogin req = new LoginSR.SendLogin();
            //req.openId = uuInfo["openid"].ToString();
            req.headUrl = uuInfo["headimgurl"].ToString();
            req.nickname = uuInfo["nickname"].ToString();//("nickName", Helper.Base64Encode(uuInfo["nickname"].ToString()));
            req.openId = uuInfo["unionid"].ToString();
            if (uuInfo.ContainsKey("sex"))
                req.sex = uuInfo["sex"].ToString().Equals("m") ? 1 : 2;
            else
                req.sex = 1;
            //MainPlayer.It.LoginToServer(loginData);
        Global.Inst.GetController<NetLoadingController>().ShowLoading(false);
        if (mGetAuthDataCallBack != null)
                mGetAuthDataCallBack(req);
            SQDebug.Log("发送登陆指令。。。。。。。");
        return req;
        }
#endif
        return null;
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
            SQDebug.Print("share successfully - share result :");
            SQDebug.Print(MiniJSON.jsonEncode(result));

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
    #endregion

    /// <summary>
    /// 授权
    /// </summary>
    public void AuthPlat(PlatformType plat, CallBack<LoginSR.SendLogin> call)
    {
        mGetAuthDataCallBack = call;
        if (!ssdk.IsAuthorized(plat))
        {
            SQDebug.Log("没有授权，登陆");
            ssdk.Authorize(plat);
        }
        else
        {
            SQDebug.Log("授权了，登陆");
            ssdk.GetUserInfo(plat);
        }
    }

    /// <summary>
    /// 关闭授权
    /// </summary>
    public void CanelLogin(PlatformType plat)
    {
        if (ssdk.IsAuthorized(plat))
        {
            ssdk.CancelAuthorize(plat);
        }
    }



    /// <summary>
    /// 邀请微信好友
    /// </summary>
    /// <param name="roomId">23456</param>
    /// <param name="des">描述信息</param>
    /// <param name="plat">平台</param>
    public void InviteFriends(string roomId, string des, PlatformType plat)
    {
        SQDebug.Log(Application.persistentDataPath + iconPath);
        //
        ShareDescConfig con = ConfigManager.GetConfigs<ShareDescConfig>()[0] as ShareDescConfig;

        ShareContent content = new ShareContent();
        content.SetTitle(con.Title);
        string str = string.Format(con.downUrl, PlayerModel.Inst.UserInfo.clubId);
        SQDebug.Log("分享连接:" + str);
        content.SetUrl(str);
        content.SetText("房间：" + roomId + "，" + des + "，邀您来战！");
        content.SetImageUrl(con.iconUrl);
        content.SetShareType(ContentType.Webpage);

        ssdk.ShareContent(plat, content);
    }

    //分享游戏给我的好友
    public void ShowGameToMyFriends(PlatformType plat)
    {
        SQDebug.Log(Application.persistentDataPath + iconPath);
        ShareDescConfig con = ConfigManager.GetConfigs<ShareDescConfig>()[1] as ShareDescConfig;
        ShareContent content = new ShareContent();
        content.SetTitle(con.Title);//
        content.SetText(con.Content);//
        content.SetImageUrl(con.iconUrl);
        string str = string.Format(con.downUrl, PlayerModel.Inst.UserInfo.clubId);
        SQDebug.Log("分享连接:" + str);
        content.SetUrl(str);

        content.SetShareType(ContentType.Webpage);

        ssdk.ShareContent(plat, content);
    }

    /// <summary>
    /// 分享到朋友圈
    /// </summary>
    public void ShowGameToWeChatMoments(PlatformType plat)
    {
        SQDebug.Log(Application.persistentDataPath + iconPath);

        ShareDescConfig con = ConfigManager.GetConfigs<ShareDescConfig>()[2] as ShareDescConfig;
        ShareContent content = new ShareContent();
        content.SetTitle(con.Title);//
        content.SetText(con.Content);//
        content.SetImageUrl(con.iconUrl);
        string str = string.Format(con.downUrl, PlayerModel.Inst.UserInfo.clubId);
        SQDebug.Log("分享连接:" + str);
        content.SetUrl(str);

        content.SetShareType(ContentType.Webpage);

        ssdk.ShareContent(plat, content);
    }

    /// <summary>
    /// 分享截屏到对应的平台
    /// </summary>
    /// <param name="plat"></param>
    public void ShowSceneShotTo(PlatformType plat)
    {
        StartCoroutine(JiePing(plat));
    }

    /// <summary>
    /// 分享图片
    /// </summary>
    /// <param name="plat"></param>
    public void ShareImg(PlatformType plat)
    {
        ShareContent content = new ShareContent();
        content.SetImagePath(Application.persistentDataPath + imagePath);
        content.SetShareType(ContentType.Image);

        ssdk.ShareContent(plat, content);
    }
    /// <summary>
    /// 截图
    /// </summary>
    public void SetCaptureScreenshot()
    {
        CaptureScreenshot2(new Rect(0, 0, Screen.width, Screen.height), Application.persistentDataPath + imagePath);
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
    #endregion
    #endregion

    #region 下载
    /// <summary>
    /// 开始下载
    /// </summary>
    /// <param name="downloadUrl">下载链接</param>
    public void StartDownLoad(string downloadUrl)
    {
        mDownloadUrl = downloadUrl;
        SQDebug.Log("开始下载");
        //if (mDownLoadAPkUI == null)
        //mDownLoadAPkUI = DownLoadAPkUI.OpenWindow();
        //mDownLoadAPkUI.SetDownloadProgress(0);
        //mAJ.Call("DownLoadAPK", downloadUrl, gameObject.name);
    }


    #region 接收消息
    /// <summary>
    /// 下载进度
    /// </summary>
    /// <param name="progress"></param>
    private void OnDownLoadProgress(string progress)
    {
        SQDebug.Log("下载进度：" + progress + "%");
        //if (mDownLoadAPkUI == null)
        //mDownLoadAPkUI = DownLoadAPkUI.OpenWindow();
        //mDownLoadAPkUI.SetDownloadProgress(int.Parse(progress));
    }

    /// <summary>
    /// 下载失败
    /// </summary>
    /// <param name="progress"></param>
    private void OnDownLoadFail(string progress)
    {
        SQDebug.Log("下载失败");

        Global.Inst.GetController<CommonTipsController>().ShowTips("下在失败，请检测网络设置后重新下载", "确定", () =>
        {
            StartDownLoad(mDownloadUrl);
        });
    }

    /// <summary>
    /// 下载完成
    /// </summary>
    /// <param name="progress"></param>
    private void OnDownLoadSuccess(string progress)
    {
        SQDebug.Log("下载完成");
        Global.Inst.GetController<CommonTipsController>().ShowTips("新版本下载完成，请安装新版本！");
    }

    /// <summary>
    /// 取消下载
    /// </summary>
    /// <param name="progress"></param>
    private void OnDownLoadCancel(string progress)
    {
        SQDebug.Log("取消下载");
    }
    #endregion
    #endregion

    #region 获取经纬度回调
    /// <summary>
    /// ios每十分钟获取一次经纬度
    /// </summary>
    private void OnTimeToGetLocation()
    {
        SendMsg(GET_LOCATION);
    }

    /// <summary>
    /// 定位返回
    /// </summary>
    /// <param name="str"></param>
    private void OnGetLocation(string str)
    {
        SQDebug.Log("获取经纬度成功" + str);
        string[] s = str.Split('|');

        SendAddrReq req = new SendAddrReq();
        req.latitude = float.Parse(s[0]);
        req.longitude = float.Parse(s[1]);
        req.address = s[2];
        SQDebug.Log("获取的定位地址为：" + req.address);
        SQDebug.Log("当前网络连接状态为：" + LoginModel.Inst.mSessionId);

        PlayerModel.Inst.address = req;

        //if (LoginModel.Inst != null && LoginModel.Inst.mSessionId != -1)
        //{
        //    NetProcess.SendRequest<TSTSendAddrReq>(req, TSTProtoIdMap.TST_CMD_SendLocation, (msgData) =>
        //    {
        //        //MsgResponseBase mg = msgData.Read<MsgResponseBase>();
        //        SQDebug.Log("gps 返回");
        //    }, false);
        //}
    }
    #endregion

    #region 充值错误返回
    /// <summary>
    /// 安卓充值返回
    /// </summary>
    /// <param name="args"></param>
    private void OnRechargeResult(string args)
    {
        if (args.Equals("success"))
            Global.Inst.GetController<CommonTipsController>().ShowTips("充值成功");
        else
            Global.Inst.GetController<CommonTipsController>().ShowTips("充值失败");
    }
    /// <summary>
    /// ios充值返回
    /// </summary>
    /// <param name="args"></param>
    private void OnIosRechargeResult(string args)
    {
        if (args.Equals("fail"))
            Global.Inst.GetController<CommonTipsController>().ShowTips("充值失败");
        else
        {

        }
    }
    #endregion

    #region 图片选择
    #region 普通图片
    /// <summary>
    /// 选择普通图片
    /// </summary>
    /// <param name="call"></param>
    public void SelectNormalPic(CallBack<string, string> call)
    {
        mSelectPicCallBack = call;
        SendMsg(SELECT_NORMAL_IMG, Application.persistentDataPath);
    }

    /// <summary>
    /// 普通图片选择返回
    /// </summary>
    /// <param name="str"></param>
    private void OnNormalSelectSuccess(string str)
    {
        SQDebug.Log("普通图片返回:" + str);
        string[] s = null;
        if (!string.IsNullOrEmpty(str))
            s = str.Split('|');

        if (string.IsNullOrEmpty(str) || str.Equals("null") || s == null || s.Length < 2)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("图片不可用");
            return;
        }
        if (mSelectPicCallBack != null)
            mSelectPicCallBack(s[0], s[1]);
        mSelectPicCallBack = null;
    }
    #endregion

    #region 头像选择
    /// <summary>
    /// 头像选择
    /// </summary>
    /// <param name="call"></param>
    public void SelectHeadPic(CallBack<string> call)
    {
        mSelectHeadPicCallBack = call;
        SendMsg(SELECT_HEAD_ICON, Application.persistentDataPath);
    }

    /// <summary>
    /// 头像选择返回
    /// </summary>
    /// <param name="str"></param>
    private void OnHeadSelectSuccess(string str)
    {
        SQDebug.Log("头像返回:" + str);
        if (string.IsNullOrEmpty(str) || str.Equals("null"))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("图片不可用");
            return;
        }
        if (mSelectHeadPicCallBack != null)
            mSelectHeadPicCallBack(str);
        mSelectHeadPicCallBack = null;
    }
    #endregion
    #endregion

    #region openinstall



    /// <summary>
    /// 接收到install返回
    /// </summary>
    /// <param name="args"></param>
    private void OnRecieveInstallData(string args)
    {

        Hashtable table = new Hashtable();
        if (!string.IsNullOrEmpty(args))
            table = MiniJSON.jsonDecode(args) as Hashtable;
        if (table != null && table.ContainsKey("userId"))
        {
            string sid = table["userId"].ToString();
            openInstallArgs = sid;
        }

        if (PlayerModel.Inst != null && PlayerModel.Inst.UserInfo != null)
        {
            PlayerModel.Inst.OpenInstallArgs = openInstallArgs;
            Global.Inst.GetController<LoginController>().SendInstallData();
        }
        SQDebug.Log("接收到installdata：" + args);
    }
    #endregion

    /// <summary>
    /// 获取键盘高度
    /// </summary>
    /// <returns></returns>
    public int GetKeybordHeight()
    {
#if UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                return jo.Call<int>("GetKeyBordHeight");
            }
        }
#endif

        return 0;
    }

}
