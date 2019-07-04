using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class MainController : BaseController
{
    public MainView mView;//view
    public MainController() : base("MainView", "Windows/MainView/MainView")
    {
        SetModel<MainViewModel>();
        NetProcess.RegisterResponseCallBack(ProtoIdMap.CMD_OnGetGoldChang, OnGetRoomCardGlodsUpdate);
        NetProcess.RegisterResponseCallBack(ProtoIdMap.CMD_GetPayBackInfo, OnGetPayBackInfo);
        NetProcess.RegisterResponseCallBack(ProtoIdMap.CMD_OnNewMessageGet, OnNewMessageGet);
        NetProcess.RegisterResponseCallBack(ProtoIdMap.CMD_OnNewMailShowRed, OnNewMailShowRed);
    }

    public override BaseView OpenWindow()
    {
        BaseView view = base.OpenWindow();
        if (mView == null)
            mView = view as MainView;
        return view;
    }


    #region 游戏服务器返回大厅服务器

    /// <summary>
    /// 返回大厅服务器
    /// </summary>
    public void BackToMain(string ip = null, int port = 0)
    {

        int old = NetProcess.mCurConnectSessionId;
        NetProcess.Connect(GameManager.Instance.Ip, GameManager.Instance.port, (b, i) =>
        {
            if (b)
            {
                NetProcess.ReleaseConnectExpectID(i);
                GameManager.Instance.StartHeartBreath(BackToMain);
                Engine.ConnectionManager.Instance.MessageEventQueue.ClearAll();
                LoginToMainServer(GameManager.Instance.Ip, GameManager.Instance.port);
            }
            else
            {
                GameManager.Instance.ShowNetTips();
            }
        });
    }

    /// <summary>
    /// 登录到大厅
    /// </summary>
    public void LoginToMainServer(string ip, int port)
    {
        LoginSR.SendLogin req = new LoginSR.SendLogin();
        req.token = PlayerModel.Inst.Token;
        if (string.IsNullOrEmpty(req.token))
            return;
        NetProcess.SendRequest<LoginSR.SendLogin>(req, ProtoIdMap.CMD_Login, (msg) =>
        {
            LoginSR.LoginBack data = msg.Read<LoginSR.LoginBack>();
            if (data.code == 1)
            {

                Scene now = SceneManager.GetActiveScene();
                if (now.name != "HALL" && now.name != "Start")
                {
                    SceneManager.LoadScene("HALL");
                    GC.Collect();
                    Resources.UnloadUnusedAssets();
                }

                MainViewModel.Inst.mNowIp = ip;
                MainViewModel.Inst.mNowPort = port;

                PlayerModel.Inst.Token = data.data.userInfo.token;
                PlayerModel.Inst.UserInfo = data.data.userInfo;
                SendGetNotice();
                OpenWindow();

                List<string> names = new List<string>();
                names.Add(typeof(MainView).Name);
                BaseView.CloseAllViewBut(names);
            }
            else
            {
                if (data.code == 2)
                {
                    Global.Inst.GetController<CommonTipsController>().ShowTips("您的登录已过期，请重新登录", "确定", true, () =>
                    {
                        Global.Inst.GetController<LoginController>().LoginOut();
                    }, null, null, "登录异常");
                }
                else
                {
                    GameUtils.ShowErrorTips(data.code);
                }

            }
        });
    }


    #endregion


    #region 发送消息


    public void SendJoinClubReq(string id, CallBack call)
    {
        SendJoinClubReq req = new global::SendJoinClubReq();
        req.id = id;
        NetProcess.SendRequest<SendJoinClubReq>(req, ProtoIdMap.CMD_SendJoinClub, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {
                if (call != null)
                {
                    call();
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 获取滚动公告
    /// </summary>
    public void SendGetNotice(CallBack call = null)
    {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, ProtoIdMap.CMD_SendGetNotice, (msg) =>
        {
            SendGetNoticeAck ack = msg.Read<SendGetNoticeAck>();
            if (ack.code == 1 && ack.data.messages != null)
            {
                MainViewModel.Inst.BroadMessage = ack.data.messages;
                //if (mView != null)
                //{
                //    mView.AddBroadMessage(ack.data.messages);
                //}
                if (call != null)
                    call();
            }
        });
    }

    /// <summary>
    /// 获取游戏IpIp
    /// </summary>
    /// <param name="gameId"></param>
    /// <param name="call"></param>
    public void SendGetGameIp(int gameId, CallBack call)
    {
        SendGetGameServerReq req = new SendGetGameServerReq();
        req.gameType = gameId;
        NetProcess.SendRequest<SendGetGameServerReq>(req, ProtoIdMap.CMD_SendGetGameServer, (msg) =>
        {
            GetGameServerAck data = msg.Read<GetGameServerAck>();
            if (data.code == 1)
            {
                SelectConnectGameServer(data, gameId);
            }
            else
            {
                GameUtils.ShowErrorTips(data.code);
            }
        });
    }

    /// <summary>
    /// 获取消息
    /// </summary>
    public void SendGetMessage(SendGetMessageReq req, CallBack call)
    {
        NetProcess.SendRequest<SendGetMessageReq>(req, ProtoIdMap.CMD_SendGetMessage, (msg) =>
        {
            SendGetMessageAck ack = msg.Read<SendGetMessageAck>();
            if (ack.code == 1)
            {
                if (ack.data != null)
                {
                    MainViewModel.Inst.UpdateMessageList(req.page, ack.data.list);
                }

                if (call != null)
                {
                    call();
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 处理消息
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="deal"></param>
    public void SendDealMessage(int messageId, int deal, Action call)
    {
        SendDealMessageReq req = new SendDealMessageReq();
        req.id = messageId;
        req.deal = deal;
        NetProcess.SendRequest<SendDealMessageReq>(req, ProtoIdMap.CMD_SendDealMessage, (msg) =>
        {
            DealMessageRecieveProto ack = msg.Read<DealMessageRecieveProto>();
            if (ack.code == 1)
            {
                if (call != null)
                {
                    call();
                }
                if (ack.data != null && !string.IsNullOrEmpty(ack.data.agentId))
                {
                    PlayerModel.Inst.UserInfo.agentId = ack.data.agentId;
                    //  mView.SetClubState(false);
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }


    /// <summary>
    /// 获取仓库信息
    /// </summary>
    /// <param name="call"></param>
    public void SendGetWareInfo(CallBack call)
    {

        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, ProtoIdMap.CMD_SendGetWareInfo, (msg) =>
        {
            SendGetWareInfoAck ack = msg.Read<SendGetWareInfoAck>();
            if (ack.code == 1)
            {
                PlayerModel.Inst.UserInfo.gold = ack.data.info.gold;
                PlayerModel.Inst.UserInfo.wareHouse = ack.data.info.ware;
                PlayerModel.Inst.UserInfo.roomCard = ack.data.info.roomCard;
                GlobalEvent.dispatch(eEvent.UPDATE_PROP);

                if (call != null)
                {
                    call();
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }


    /// <summary>
    /// 存仓库
    /// </summary>
    /// <param name="gold"></param>
    /// <param name="call"></param>
    public void SendSaveWare(float gold, Action call)
    {
        SendSaveOutWare req = new SendSaveOutWare();
        req.glod = gold;
        NetProcess.SendRequest<SendSaveOutWare>(req, ProtoIdMap.CMD_SendSaveInWare, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {
                PlayerModel.Inst.UserInfo.wareHouse += gold;
                if (call != null)
                {
                    call();
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }


    /// <summary>
    /// 取仓库
    /// </summary>
    /// <param name="gold"></param>
    /// <param name="call"></param>
    public void SendGetWare(float gold,string pwd, Action call)
    {
        SendGetOutWare req = new SendGetOutWare();
        req.glod = gold;
        req.pwd = pwd;
        NetProcess.SendRequest<SendGetOutWare>(req, ProtoIdMap.CMD_SendGetOutWare, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {
                PlayerModel.Inst.UserInfo.wareHouse -= gold;
                if (call != null)
                {
                    call();
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 发送反馈
    /// </summary>
    /// <param name="content"></param>
    public void SendFeedBack(string content, CallBack call)
    {
        FeedBackProto req = new FeedBackProto();
        req.content = content;
        NetProcess.SendRequest<FeedBackProto>(req, ProtoIdMap.CMD_SendFeedBack, (msg) =>
        {
            CommonRecieveProto data = msg.Read<CommonRecieveProto>();
            if (data.code == 1)
            {
                Global.Inst.GetController<CommonTipsController>().ShowTips("非常感谢您的宝贵意见，我们将及时处理");
                if (call != null)
                    call();
            }
            else
                GameUtils.ShowErrorTips(data.code);
        });
    }

    /// <summary>
    /// 实名认证
    /// </summary>
    /// <param name="realName"></param>
    /// <param name="idCard"></param>
    /// <param name="call"></param>
    public void SendCheckRealName(string realName, string idCard, Action call)
    {
        SendCheckRealName req = new global::SendCheckRealName();
        req.realName = realName;
        req.idCard = idCard;
        NetProcess.SendRequest<SendCheckRealName>(req, ProtoIdMap.CMD_SendCheckIdCard, (msg) =>
        {
            CommonRecieveProto data = msg.Read<CommonRecieveProto>();
            if (data.code == 1)
            {
                if (call != null)
                {
                    call();
                }
                Global.Inst.GetController<CommonTipsController>().ShowTips("认证成功!");
            }
            else
                GameUtils.ShowErrorTips(data.code);
        });
    }

    /// <summary>
    /// 获取排行榜信息
    /// </summary>
    /// <param name="type"></param>
    public void SendGetRankInfo(int type, CallBack call)
    {
        SendGetRankReq req = new SendGetRankReq();
        req.type = type;
        NetProcess.SendRequest<SendGetRankReq>(req, ProtoIdMap.CMD_SendGetRank, (msg) =>
        {
            SendGetRankAck data = msg.Read<SendGetRankAck>();
            if (data.code == 1)
            {
                if (data.data != null)
                {
                    MainViewModel.Inst.PointRankList = data.data.info != null ? data.data.info : new List<global::SendGetRankInfo>();
                }

                if (call != null)
                {
                    call();
                }
            }
        });
    }

    /// <summary>
    /// 获取盈利收入信息
    /// </summary>
    /// <param name="req"></param>
    public void SendGetNewPageGainInfo(SendGetGainDetail req, CallBack<PlayerRecordDetailNum> call)
    {
        NetProcess.SendRequest<SendGetGainDetail>(req, ProtoIdMap.CMD_SendGainDetail, (msg) =>
        {
            SendGetGainAck ack = msg.Read<SendGetGainAck>();
            if (ack.code == 1)
            {
                PlayerRecordDetailNum nums = null;
                if (ack.data != null)
                {
                    nums = ack.data.statisticsNum;
                    MainViewModel.Inst.UpdateGainList(req.type, req.page, ack.data.infoList);
                }

                if (call != null)
                {
                    call(nums);
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 获取代理接入打赏纪律
    /// </summary>
    /// <param name="req"></param>
    public void SendGetNewPageAgentBRInfo(SendGetAgentBRDetail req, CallBack<SendGetAgentBRTotal> call)
    {
        NetProcess.SendRequest<SendGetAgentBRDetail>(req, ProtoIdMap.CMD_SendGetBRRecord, (msg) =>
        {
            SendGetAgentBRAck ack = msg.Read<SendGetAgentBRAck>();
            if (ack.code == 1)
            {
                SendGetAgentBRTotal num = null;
                if (ack.data != null)
                {
                    num = ack.data.totalNum;
                    MainViewModel.Inst.UpdateAgentList(req.type, req.page, ack.data.infoList);
                }

                if (call != null)
                {
                    call(num);
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 获取玩家详细记录
    /// </summary>
    /// <param name="req"></param>
    /// <param name="call"></param>
    public void SendGetNewPagePlayerDetailRecord(SendGetAgentBRDetail req, CallBack<PlayerRecordDetailNum> call)
    {
        NetProcess.SendRequest<SendGetAgentBRDetail>(req, ProtoIdMap.CMD_SendGetPlayerRecordDetail, (msg) =>
        {
            PlayerRecordDetailBack ack = msg.Read<PlayerRecordDetailBack>();
            if (ack.code == 1)
            {
                if (ack.data != null)
                {
                    MainViewModel.Inst.UpdatePlayerRecordDetailList(req.page, ack.data.infoList);
                }
                if (call != null)
                {
                    if (ack.data != null)
                        call(ack.data.num);
                    else
                        call(null);
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 获取玩家接入打赏纪律
    /// </summary>
    /// <param name="req"></param>
    public void SendGetNewPageUserBRInfo(SendGetUserBRDetail req, CallBack call)
    {
        NetProcess.SendRequest<SendGetUserBRDetail>(req, ProtoIdMap.CMD_SendGetUserBRRecord, (msg) =>
        {
            SendGetUserBRAck ack = msg.Read<SendGetUserBRAck>();
            if (ack.code == 1)
            {
                if (ack.data != null)
                {
                    MainViewModel.Inst.UpdateUserList(req.type, req.page, ack.data.infoList);
                }

                if (call != null)
                {
                    call();
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }


    public void SendReadMessage(int id, CallBack call)
    {
        SendReadMessageReq req = new SendReadMessageReq();
        req.id = id;
        NetProcess.SendRequest<SendReadMessageReq>(req, ProtoIdMap.CMD_SendReadMessage, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {
                if (call != null)
                {
                    call();
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 获取转盘信息
    /// </summary>
    /// <param name="call"></param>
    public void SendGetPrizeInfo(CallBack<GetPrizeConfigBackData> call)
    {
        NetProcess.SendRequest<int>(0, ProtoIdMap.CMD_sendGetPrizeInfo, (msg) =>
        {
            GetPrizeConfigBack data = msg.Read<GetPrizeConfigBack>();

            if (data.code == 1)
            {
                if (call != null) call(data.data);
            }
            else Global.Inst.GetController<CommonTipsController>().ShowTips(CodeErrorTips.GetTips(data.code, data.desc));
        });
    }

    /// <summary>
    /// 抽奖返回
    /// </summary>
    /// <param name="call"></param>
    public void SendPrizeBack(CallBack<DrawBackData> call)
    {
        NetProcess.SendRequest<int>(0, ProtoIdMap.CMD_sendPrizeBack, (msg) =>
        {
            DrawBack data = msg.Read<DrawBack>();

            if (data.code == 1)
            {
                if (call != null) call(data.data);
            }
            else Global.Inst.GetController<CommonTipsController>().ShowTips(CodeErrorTips.GetTips(data.code, data.desc));

        });
    }

    /// <summary>
    /// 获取抽奖记录
    /// </summary>
    /// <param name="call"></param>
    public void SendGetPrizeRecord(CallBack<List<RecordInfo>> call)
    {
        NetProcess.SendRequest<int>(0, ProtoIdMap.CMD_sendGetPrizeRecord, (msg) =>
        {
            GetDrawRecord data = msg.Read<GetDrawRecord>();
            if (data.code == 1 && call != null)
            {
                call(data.data.recordInfo);
            }
            else Global.Inst.GetController<CommonTipsController>().ShowTips(CodeErrorTips.GetTips(data.code, data.desc));
        });
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="psd"></param>
    /// <param name="call"></param>
    public void FixPassword(string lastPwd, string newPwd, CallBack call)
    {
        SendFixPsdReq req = new SendFixPsdReq();
        req.lastPwd = lastPwd;
        req.newPwd = newPwd;
        NetProcess.SendRequest<SendFixPsdReq>(req, ProtoIdMap.CMD_FixPsd, (msg) =>
        {
            CommonRecieveProto data = msg.Read<CommonRecieveProto>();
            if (data.code == 1)
            {
                if(call!=null)
                    call();
            }
            else
                GameUtils.ShowErrorTips(data.code);
        });
    }
    #endregion

    #region 接收到服务器推送的消息

    /// <summary>
    /// 更新房卡和金币
    /// </summary>
    /// <param name="msg"></param>
    public void OnGetRoomCardGlodsUpdate(MessageData msg)
    {
        GetRooMCardGlodsUpdate ack = msg.Read<GetRooMCardGlodsUpdate>();
        PlayerModel.Inst.UpdateRoomCardGlods(ack.roomCard, ack.glods);
    }
    /// <summary>
    /// 购买支付返回
    /// </summary>
    /// <param name="msg"></param>
    private void OnGetPayBackInfo(MessageData msg)
    {
        GetRooMCardGlodsUpdate ack = msg.Read<GetRooMCardGlodsUpdate>();
        Global.Inst.GetController<CommonTipsController>().ShowTips("已为您放入仓库中", "前往", () => { mView.OnWareHouseClick(); }, null, null, "购买成功");
        //PlayerModel.Inst.UserInfo.gold = ack.glods;
        //PlayerModel.Inst.UserInfo.roomCard = ack.roomCard;
        //PlayerModel.Inst.UpdateRoomCardGlods(ack.roomCard, ack.glods);

    }

    /// <summary>
    /// 收到新消息
    /// </summary>
    /// <param name="msg"></param>
    private void OnNewMessageGet(MessageData msg)
    {
        SendGetMessageInfo ack = msg.Read<SendGetMessageInfo>();
        Global.Inst.GetController<CommonTipsController>().ShowTips(ack.content, "拒绝|同意", () =>
        {
            SendDealMessage(ack.messageId, 2, () =>
            {
                Global.Inst.GetController<CommonTipsController>().CloseWindow();
            });

        }, () =>
        {
            SendDealMessage(ack.messageId, 1, () =>
            {
                Global.Inst.GetController<CommonTipsController>().CloseWindow();
            });
        }, null, "新消息");
    }

    /// <summary>
    /// 新消息达到
    /// </summary>
    /// <param name="msg"></param>
    private void OnNewMailShowRed(MessageData msg)
    {
        if (mView != null)
        {
            //mView.SetMessageRedState(true);
        }
    }

    #endregion


    #region 选择游戏服务器

    /// <summary>
    /// 选择连接游戏服务器
    /// </summary>
    /// <param name="data"></param>
    /// <param name="gameId"></param>
    private void SelectConnectGameServer(GetGameServerAck data, int gameId)
    {
        GamePatternModel.Inst.mCurGameId = (eGameType)gameId;
        Global.Inst.GetController<GamePatternController>().ConnectGameServer(data.data.Serverinfo.ServerIp, int.Parse(data.data.Serverinfo.ServerPort));
    }


    #endregion
}
