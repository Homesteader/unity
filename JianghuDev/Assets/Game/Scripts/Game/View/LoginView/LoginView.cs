using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoginView : BaseView
{
    public UIInput mInput;//输入框
    public GameObject mLoadingTips;//加载提示
    public GameObject mLoginBtn;//登录按钮
    private bool mIsAgree = true;//是否同意用户协议

    private float mPressedTime = 0.0f;

    private bool mPressed = false;

    protected override void Awake()
    {
        base.Awake();
        string name = PlayerPrefs.GetString("2710_login", "");
        if (!string.IsNullOrEmpty(name))
            mInput.value = name;
#if WECHAT
        mInput.gameObject.SetActive(false);
#else
        mInput.gameObject.SetActive(true);
#endif
        mLoadingTips.SetActive(true);
        mLoginBtn.SetActive(false);//加载完才显示登录按钮
        
    }

    protected override void Start()
    {
        base.Start();
        CheckVersion();//检查版本更新
    }

    protected override void Update()
    {
        base.Update();
        
    }


    #region 按钮点击
    


    /// <summary>
    /// 点击登录按钮
    /// </summary>
    public void OnLoginClick()
    {
        if(!mIsAgree)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("请先同意用户协议", "确定");
            return;
        }
        //登录
        //登录成功回调
        CallBack callback = () =>
        {
            Global.Inst.GetController<MainController>().OpenWindow();
            Close();
        };
#if WECHAT
        Global.Inst.GetController<NetLoadingController>().ShowLoading(true);
        string loginStr = PlayerPrefs.GetString("wechat_login_data", "");//获取保存到本地的登录信息
        LoginSR.SendLogin data = null;
        if (!string.IsNullOrEmpty(loginStr))
            data = Json.Deserialize<LoginSR.SendLogin>(loginStr);
        if (data != null && !string.IsNullOrEmpty(data.openId))//如果登录信息不为空就直接登录
        {
            Global.Inst.GetController<LoginController>().LoginToServer(data, () => {
                callback();
            });
        }
        else//SDK登录
        {
            Global.Inst.GetController<NetLoadingController>().ShowLoading(true);
            SixqinSDKManager.Inst.AuthPlat(cn.sharesdk.unity3d.PlatformType.WeChat, (d) =>
            {
                Global.Inst.GetController<LoginController>().LoginToServer(d, () =>
                {
                    callback();
                });
            });
        }
#else
        string id = mInput.value;
        if(string.IsNullOrEmpty(id))
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("id不能为空");
            return;
        }
        LoginSR.SendLogin data = new LoginSR.SendLogin();
        data.openId = id;
        data.nickname = "特伦苏" + id;
        data.sex = 1;
        data.headUrl = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1532170478982&di=8b36bc5c0de739edde5e0b6e40784fde&imgtype=0&src=http%3A%2F%2Ftouxiang.yeree.com%2Fpics%2F4a%2F3756897.jpg";
        PlayerPrefs.SetString("2710_login", id);
        Global.Inst.GetController<LoginController>().LoginToServer(data, ()=> {
            callback();
        });
#endif
    }
    

    #endregion

    #region 检测版本

    /// <summary>
    /// 检测版本更新
    /// </summary>
    private void CheckVersion()
    {

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            DelayRun(1, OpenLogin);
            return;
        }

        Assets.LoadTxtFileCallText(GameManager.Instance.mVersionTxtUrl, (t) =>
        {
            if (string.IsNullOrEmpty(t))
            {
                Global.Inst.GetController<CommonTipsController>().ShowTips("检测版本失败", "确定", true, () =>
                {
                    CheckVersion();
                });
            }
            else
            {
                GameVersion txt = Json.Deserialize<GameVersion>(t);
                if (txt == null)
                {
                    Global.Inst.GetController<CommonTipsController>().ShowTips("检测版本失败", "确定", true, () =>
                    {
                        CheckVersion();
                    });
                }
                else
                {
                    if (Application.platform != RuntimePlatform.Android && GameManager.Instance.mVersion == txt.examineVersion)//不是安卓设备，并且是审核版本
                    {
                        LoginModel.Inst.IsVisitor = true;
                        GameManager.Instance.Ip = txt.examineIp;
                        NetProcess.InitNetWork(GameManager.Instance.Ip, GameManager.Instance.port);
                        ConfigManager.LoadAllConfig(txt.ios_resUrl, txt.ios_resVersion, OpenLogin);
                    }
                    else
                    {
                        LoginModel.Inst.IsVisitor = false;
                        string[] local = GameManager.Instance.mVersion.Split('.');
                        string[] net = new string[3];
                        string ver = "";
                        string appUrl = "";
                        string resUrl = "";
                        int resId = 0;
                        if (Application.platform == RuntimePlatform.Android)
                        {
                            net = txt.android_version.Split('.');
                            ver = txt.android_version;
                            appUrl = txt.android_appUrl;
                            resUrl = txt.android_resUrl;
                            resId = txt.android_resVersion;
                        }
                        else if (Application.platform == RuntimePlatform.IPhonePlayer)
                        {
                            net = txt.ios_version.Split('.');
                            ver = txt.ios_version;
                            appUrl = txt.ios_appUrl;
                            resUrl = txt.ios_resUrl;
                            resId = txt.ios_resVersion;
                        }

                        if (GameManager.Instance.mVersion != ver)
                        {
                            UpdateVersion(net, local, appUrl, resUrl, resId);
                        }
                        else
                        {
                            //OpenLogin();
                            ConfigManager.LoadAllConfig(resUrl, resId, OpenLogin);
                        }
                    }
                }
            }
        }, false);
    }

    /// <summary>
    /// 判断a是否大于b
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>1是相等，2是大于，-1是小于</returns>
    private void UpdateVersion(string[] a, string[] b, string appUrl, string resUrl, int resId)
    {

        int a1 = int.Parse(a[0]);
        int a2 = int.Parse(a[1]);
        int a3 = int.Parse(a[2]);

        int b1 = int.Parse(b[0]);
        int b2 = int.Parse(b[1]);
        int b3 = int.Parse(b[2]);

        if (a1 > b1 || a2 > b2)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("发现新版本，请前往下载", "下载", true, () =>
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    Application.OpenURL(appUrl);
                    //SixqinSDKManager.Inst.StartDownLoad(appUrl);
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    Application.OpenURL(appUrl);
                }
            });
        }
        else
        {
            //提示更新
            Global.Inst.GetController<CommonTipsController>().ShowTips("发现新版本，是否前往下载", "下载|取消", true, () =>
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    Application.OpenURL(appUrl);
                    //SixqinSDKManager.Inst.StartDownLoad(appUrl);
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    Application.OpenURL(appUrl);
                }
            },
            () =>
            {
                ConfigManager.LoadAllConfig(resUrl, resId, OpenLogin);
            });
        }

    }

    #endregion

    /// <summary>
    /// 打开登录界面
    /// </summary>
    private void OpenLogin()
    {
        OtherConfig config = ConfigManager.GetConfigs<OtherConfig>()[0] as OtherConfig;
        if (config.isOpen != 1)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("服务器维护中", "确定");
            return;
        }
        mLoadingTips.SetActive(false);
        mLoginBtn.SetActive(true);//加载完才显示登录按钮
        Debug.Log("退出");
        PlayerPrefs.DeleteKey("wechat_login_data");//清除登录信息
        if (Application.platform != RuntimePlatform.WindowsEditor)
            SixqinSDKManager.Inst.CanelLogin(cn.sharesdk.unity3d.PlatformType.WeChat);
    }

}
