using UnityEngine;
using System.Collections;

public class LoadingView : BaseView
{
    public UISlider mProgress;//进度条
    private float mProgressValue;//进度

    protected override void Start()
    {
        base.Start();
        mProgressValue = 0;
        //检测更新
        CheckVersion();
    }


    protected override void Update()
    {
        base.Update();
        if (mProgressValue <= 1)
        {
            mProgressValue += Time.deltaTime;
            mProgress.value = mProgressValue;
        }
    }

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
                    if ( Application.platform != RuntimePlatform.Android && GameManager.Instance.mVersion == txt.examineVersion)//不是安卓设备，并且是审核版本
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
        mProgressValue = 1;
        mProgress.value = mProgressValue;
        OtherConfig config = ConfigManager.GetConfigs<OtherConfig>()[0] as OtherConfig;
        if (config.isOpen != 1)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("服务器维护中", "确定");
            return;
        }
        Global.Inst.GetController<LoginController>().OpenWindow();
        Close();
    }
}
