using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
using System.Text;

public class GameManager : MonoBehaviour
{

    #region 公共变量

    /// <summary>
    /// 是否开启打印
    /// </summary>
    public bool mIsShowLog = false;

    public string ServerUrl = "";
    public bool mUseUrl = false;

    public string Ip = "192.168.0.121";
    public int port = 12345;
    public string LoginRequest = "";
    public static GameManager Instance;
    public DeviceType deviceType;
    public int iosIsOpenID = 0;
    private int mHadReconnectCount;//重连了几次
    public string mVersionTxtUrl = "http://39.104.54.205/xxzp/configs/guilinzizai_version.txt";

    public string mVersion = "1.0.0";


    #endregion

    #region 私有变量

    /// <summary>
    /// 心跳连接判定断线之后的回调处理 传入ip和端口号
    /// </summary>
    private CallBack<string, int> mBreathCallBack;

    /// <summary>
    /// 没有接收到心跳的次数
    /// </summary>
    public int mNoHeartTime = 0;
    private int mAutoReConnectNum = 0;//自动重连次数
    private float mLastReconnetTime;//上一次重连的时间

    #endregion

    #region Unity函数

    void Awake()
    {
        Instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //
        Application.targetFrameRate = 30;
        Input.multiTouchEnabled = false;
    }

    // Use this for initialization
    void Start()
    {
        //不可销毁对象
        GameObject.DontDestroyOnLoad(this);
        gameObject.AddComponent<SetTimeout>();
        //初始化模块
        InitModule();
        //游戏网络
        InitNetWork();
        //
        InitDelay();
        //

        

        CallBack call = () =>
        {
            NetProcess.InitNetWork(GameManager.Instance.Ip, GameManager.Instance.port);

            //初始化创建管理器
            InitSceneMgr();

            //初始化音效管理器
            SoundProcess.Create();

            InitSixSdkManager();
#if YYVOICE
        //开启YY语音
        YYsdkManager.Create();
#endif

            //加载配置信息
            LoadConfig(() =>
            {
                LoginController loading = Global.Inst.GetController<LoginController>();
                loading.OpenWindow();
            });
            SoundProcess.PlayMusic("BGSOUND");
        };

        if (mUseUrl)
        {
            GetIp(call);
        }
        else
            call();
    }

    #endregion

    #region 初始化功能组件

    /// <summary>
    /// 初始化网络
    /// </summary>
    private void InitNetWork()
    {
        GameObject NetObject = new GameObject();
        NetObject.name = "NetProcess";
        NetObject.AddComponent<NetProcess>();
        NetObject.AddComponent<HttpProcess>();
        GameObject.DontDestroyOnLoad(NetObject);
    }


    private void InitSceneMgr()
    {
        GameObject NetObject = new GameObject();
        NetObject.name = "SQSceneLoader";
        NetObject.AddComponent<SQSceneLoader>();
        GameObject.DontDestroyOnLoad(NetObject);
    }

    private void InitDelay()
    {
        GameObject obj = new GameObject();
        obj.name = "SQTimeOutTool";
        obj.AddComponent<SQTimeOutTool>();
        GameObject.DontDestroyOnLoad(obj);
    }


    private void LoadConfig(CallBack call)
    {
        ConfigManager.Creat();
        //ConfigManager.LoadAllConfig(SQResourceLoader.LoadAssetBundle("AssetsResources/allgameconfigs"), 4, call);
        //return;
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            ConfigManager.LoadAllConfig(SQResourceLoader.LoadAssetBundle("AssetsResources/allgameconfigs"), 11, call);
        }
        else
            call();
    }

    /// <summary>
    /// 初始化SDK
    /// </summary>
    private void InitSixSdkManager()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            return;
        GameObject NetObject = new GameObject();
        NetObject.name = "SixqinSDKManager";
        NetObject.AddComponent<SixqinSDKManager>();
        GameObject.DontDestroyOnLoad(NetObject);
    }

    #endregion

    #region 初始化模块

    private void InitModule()
    {
        Global.Inst.RegisterController<LoginController>();//登录
        Global.Inst.RegisterController<CommonTipsController>();//公共提示框
        Global.Inst.RegisterController<NetLoadingController>();//网络loading
        Global.Inst.RegisterController<LoadingController>();//加载界面
        Global.Inst.RegisterController<MainController>();//主界面
        Global.Inst.RegisterController<ShopController>();//充值
        Global.Inst.RegisterController<PayController>();    //支付
        Global.Inst.RegisterController<SceneLoadingController>();//scene切换
        Global.Inst.RegisterController<XXGoldFlowerGameController>();//扎金花
        Global.Inst.RegisterController<ClubController>();//俱乐部
        Global.Inst.RegisterController<GamePatternController>();//模式选择
        Global.Inst.RegisterController<CreateRoomController>();//创建房间12
        Global.Inst.RegisterController<MJGameController>();//麻将
        Global.Inst.RegisterController<GameInteractionController>();//播放互动表情
        Global.Inst.RegisterController<NNGameController>();//牛牛游戏
        Global.Inst.RegisterController<MJGameBackController>();//回放
        Global.Inst.RegisterController<GameChatController>();//聊天
        Global.Inst.RegisterController<ShareController>();//分享
        Global.Inst.RegisterController<AgentGainController>();//代理
        Global.Inst.RegisterController<TenGameController>();//代理
        Global.Inst.RegisterController<JoinRoomController>();//加入房间
        Global.Inst.RegisterController<ModelSelectController>();//模式选择
        Global.Inst.RegisterController<ServiceController>();//客服
        Global.Inst.RegisterController<RecordController>();//战绩
        Global.Inst.RegisterController<SelectRoomController>();//选择房间
    }

    #endregion

    private void GetIp(CallBack call)
    {
        if (mUseUrl)
        {
            Ip = SQToolHelper.DoGetHostAddresses(ServerUrl);
            if (string.IsNullOrEmpty(Ip))
            {
                Global.Inst.GetController<CommonTipsController>().ShowTips("网络连接错误，请检查网络后重新连接", "确定", () =>
                {
                    GetIp(call);
                });
            }
            else
                call();
        }
    }


    #region 移动平台app后台处理


    void OnApplicationPause(bool b)
    {
        if (!b)//唤醒
        {
            SQDebug.Log("程序获得焦点");
            if (!NetProcess.IsCurExistConnected() && !BaseView.ContainsView<LoginView>())//当前有链接但是已断开，重连
            {
                SQDebug.Log("application focus");
                ShowNetTips();
            }
        }
    }

    void OnApplicationQuit()
    {
        NetProcess.ReleaseAllConnect();
        
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            OnApplicationPause(false);
        }
        if(Input.GetKeyUp(KeyCode.Z))
            NetProcess.ReleaseAllConnect();
    }
    #endregion




    #region 心跳处理
    /// <summary>
    /// 重置重连时间
    /// </summary>
    public void ResetConnetTime()
    {
        mLastReconnetTime = -10;
    }


    /// <summary>
    /// 开始心跳连接
    /// </summary>
    /// <param name="call">心跳判定断线之后的回调</param>
    public void StartHeartBreath(CallBack<string, int> call = null)
    {

        if (call != null)
        {
            mBreathCallBack = call;
        }
        SQDebug.Log("开始心跳");
        if (!IsInvoking("SendHeartBreath"))
        {
            SQDebug.Log("开启心跳连接");
            InvokeRepeating("SendHeartBreath", 0.01f, 5.0f);//5s发送一次心跳连接
        }
    }


    /// <summary>
    /// 取消心跳连接
    /// </summary>
    public void CancelHeartBreath()
    {
        CancelInvoke("SendHeartBreath");
        SQDebug.Log("取消心跳连接");
        mNoHeartTime = 0;
    }


    /// <summary>
    /// 发送心跳连接
    /// </summary>
    public void SendHeartBreath()
    {
        mNoHeartTime++;
        if (mNoHeartTime >= 3)
        {//超过两次没有收到心跳恢复
            SQDebug.Log("通过心跳判定的断网处理");
            ShowNetTips();
            return;
        }
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, ProtoIdMap.CMD_Breath, (msg) =>
        {
            mNoHeartTime = 0;
            mAutoReConnectNum = 0;
        }, false);

    }

    /// <summary>
    /// 显示网络提示
    /// </summary>
    public void ShowNetTips()
    {
        GameManager.Instance.CancelHeartBreath();
        NetProcess.ReleaseAllConnect();
        Global.Inst.GetController<NetLoadingController>().ShowLoading(true);
        SetTimeout.remove(ReConnet);
        if (Time.realtimeSinceStartup - mLastReconnetTime > 5)//重连间隔要大于5秒
        {
            SQDebug.Log("直接重连");

            ReConnet();
        }
        else
        {
            float time = 5 + mLastReconnetTime - Time.realtimeSinceStartup;
            SQDebug.Log("等" + time + "秒重连");
            SetTimeout.add(time, ReConnet);
        }
    }

    /// <summary>
    /// 重连
    /// </summary>
    public void ReConnet()
    {
        mLastReconnetTime = Time.realtimeSinceStartup;//重连时间
        LoginModel.Inst.mSessionId = -1;
        Global.Inst.GetController<NetLoadingController>().ShowLoading(false);
        GameManager.Instance.CancelHeartBreath();
        NetProcess.ReleaseAllConnect();
        CallBack call = () =>
        {
            SQDebug.Log("重连次数" + mAutoReConnectNum);
            if (mBreathCallBack == null)
            {
                Global.Inst.GetController<MainController>().BackToMain(Ip, port);
            }
            else
            {
                mBreathCallBack(MainViewModel.Inst.mNowIp, MainViewModel.Inst.mNowPort);
            }

        };
        if (mAutoReConnectNum < 3)//如果自动重连次数少于两次就弹出提示
        {
            mAutoReConnectNum++;
            call();
        }
        else
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("网络连接失败", "尝试连接|退出游戏", true, call, () =>
            {
                Application.Quit();
            }, null, "网络异常");
        }
    }
    #endregion
    
}