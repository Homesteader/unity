using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LoginController : BaseController
{

    public LoginController() : base("LoginView", "Windows/LoginView/LoginView")
    {
        SetModel<LoginModel>();
        NetProcess.RegisterResponseCallBack(ProtoIdMap.CMD_RecieveLoginInOther, OnLoginInOther);
    }

    /// <summary>
    /// 登录大厅服务器
    /// </summary>
    /// <param name="req"></param>
    public void LoginToServer(LoginSR.SendLogin req, CallBack call)
    {
        LoginModel.Inst.LoginData = req;
        //连接服务器
        ConnectServer(() =>
        {
            //服务器连接成功后登录服务器
            NetProcess.SendRequest<LoginSR.SendLogin>(req, ProtoIdMap.CMD_Login, (msg) =>
            {
                LoginSR.LoginBack data = msg.Read<LoginSR.LoginBack>();
                if (data.code == 1)
                {
                    if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
                        SixqinSDKManager.Inst.SendMsg(SixqinSDKManager.GET_INSTALL_DATA);//发送扫二维码添加的好友
                    

                    PlayerModel.Inst.Token = data.data.userInfo.token;
                    PlayerModel.Inst.UserInfo = data.data.userInfo;
                    if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor) {
                        //SixqinSDKManager.Inst.InitJPUSH(PlayerModel.Inst.UserInfo.userId);//初始化jpush
                    }
                    ////公众号
                    //PlayerModel.Inst.PublicSign = data.data.publicSign;
                    //广播
                    MainViewModel.Inst.BroadMessage.Clear();

                    Global.Inst.GetController<MainController>().SendGetNotice();
                    if (!string.IsNullOrEmpty(data.roomId))//在房间中
                    {
                        Global.Inst.GetController<XXGoldFlowerGameController>().SendJoinRoomReq(data.roomId);
                    }
                    else
                    {
                        if (call != null)
                            call();
                    }
                }
                else if (data.code == 12) {
                    PlayerModel.Inst.Token = data.data.userInfo.token;
                    PlayerModel.Inst.UserInfo = data.data.userInfo;
                    if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
                    {
                        SixqinSDKManager.Inst.InitJPUSH(PlayerModel.Inst.UserInfo.userId);//初始化jpush
                    }
                    Global.Inst.GetController<GamePatternController>().ConnectGameServer(data.data.gameServer.ip, int.Parse(data.data.gameServer.port));
                }
                else {
                    GameUtils.ShowErrorTips(data.code);
                }

            });
        });
    }


   

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="call"></param>
    private void ConnectServer(CallBack call)
    {
        NetProcess.ReleaseAllConnect();
        string ip = GameManager.Instance.Ip;
        int port = GameManager.Instance.port;
        SQDebug.Log("开始连接服务器 ip:" + ip + "  port:" + port);
        NetProcess.Connect(ip, port, (b, id) =>
        {
            if (b)
            {
                NetProcess.ReleaseConnectExpectID(id);
                MainViewModel.Inst.mNowIp = ip;
                MainViewModel.Inst.mNowPort = port;

                GameManager.Instance.StartHeartBreath((tip, tport)=>
                {
                    LoginToServer(LoginModel.Inst.LoginData, null);
                });
                LoginModel.Inst.mSessionId = id;
                SQDebug.Log("网络连接成功 ip:" + ip + "  port:" + port);
                if (call != null)
                    call();
            }
            else
            {
                GameManager.Instance.CancelHeartBreath();
                Global.Inst.GetController<CommonTipsController>().ShowTips("网络连接失败，请检测网络后重新连接", "确定", () =>
                {
                    ConnectServer(call);
                });
            }
        });
    }


    /// <summary>
    /// 关闭其他界面，回到登录界面
    /// </summary>
    private void CloseOtherViewOpenLogin()
    {
        Scene now = SceneManager.GetActiveScene();
        if (now.name != "HALL" && now.name != "Start")
        {
            SceneManager.LoadScene("HALL");
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
        }
        OpenWindow();
        List<string> view = new List<string>();
        view.Add("LoginView");
        view.Add("CommonTipsView");
        BaseView.CloseAllViewBut(view);
    }

    #region 发送install加好友
    /// <summary>
    /// 扫二维码添加好友
    /// </summary>
    public void SendInstallData()
    {
        //if (PlayerModel.Inst.UserInfo.haveAgent || string.IsNullOrEmpty(PlayerModel.Inst.OpenInstallArgs))//已经绑定过代理就不再发送
        //    return;
        //bool isInMainView = NetProcess.IsHallConnect();//是否在大厅
        //if (!isInMainView)
        //    return;
        //LoginSR.SendInstallAddFriend req = new LoginSR.SendInstallAddFriend();
        //req.agent = PlayerModel.Inst.OpenInstallArgs;
        ////发送sid给服务器，发送成功过后删除key
        //NetProcess.SendRequest<LoginSR.SendInstallAddFriend>(req, ProtoIdMap.CMD_InstallAddFriend, (msg) =>
        //{
        //    CommonRecieveProto data = msg.Read<CommonRecieveProto>();
        //    PlayerModel.Inst.UserInfo.haveAgent = true;
        //});
    }
    #endregion

    /// <summary>
    /// 退出登录
    /// </summary>
    public void LoginOut()
    {
        GameManager.Instance.CancelHeartBreath();
        NetProcess.ReleaseAllConnect();
        CloseOtherViewOpenLogin();
    }

    #region 接收到服务器消息

    /// <summary>
    /// 账号在其他玩家登录
    /// </summary>
    /// <param name="msg"></param>
    private void OnLoginInOther(MessageData msg)
    {
        GameManager.Instance.CancelHeartBreath();
        NetProcess.ReleaseAllConnect();
        LoginSR.LoingInOtherRecieve data = msg.Read<LoginSR.LoingInOtherRecieve>();
        Global.Inst.GetController<CommonTipsController>().ShowTips("您的账号在其他地方登录，请确认后重新登录", "确定", true, () =>
           {
               CloseOtherViewOpenLogin();
           });
    }


    #endregion
}
