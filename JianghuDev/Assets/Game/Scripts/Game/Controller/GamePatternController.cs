using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class GamePatternController : BaseController {

    public GamePatternView mView;//view
    public GamePatternController() : base("GamePatternView", AssetsPathDic.GamePatternView)
    {
        SetModel<GamePatternModel>();
        NetProcess.RegisterResponseCallBack(ProtoIdMap.CMD_OnGetRoomChang, OnRoomInfoChang);//房间信息发生变化
        NetProcess.RegisterResponseCallBack(ProtoIdMap.CMD_OnGamePatternPersonChanged, OnOnLinePersonChanged);//在线人数发生变化
    }

    public override BaseView OpenWindow()
    {
        BaseView view = base.OpenWindow();
        if (mView == null)
            mView = view as GamePatternView;
        return view;
    }

    #region 连接服务器

    /// <summary>
    /// 断线重连
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void Reconnect(string ip, int port) {
        ConnectGameServer(ip,port,true);
    }

    /// <summary>
    /// 进入游戏服务器
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="gameType"></param>
    public void ConnectGameServer(string ip, int port,bool reconnect = false)
    {
        Global.Inst.GetController<NetLoadingController>().ShowLoading(true, false);
        int old = NetProcess.mCurConnectSessionId;
        NetProcess.Connect(ip, port, (b, i) =>
        {
            Global.Inst.GetController<NetLoadingController>().ShowLoading(false);
            if (b)
            {
                LoginModel.Inst.mSessionId = i;
                NetProcess.ReleaseConnectExpectID(i);
                MainViewModel.Inst.mNowIp = ip;
                MainViewModel.Inst.mNowPort = port;
                GameManager.Instance.StartHeartBreath(Reconnect);
                Engine.ConnectionManager.Instance.MessageEventQueue.ClearAll();
                LoginToGameServer();
            }
            else
            {
                if (reconnect) {
                    GameManager.Instance.ReConnet();
                }
                else {
                    Global.Inst.GetController<CommonTipsController>().ShowTips("服务器维护中，请稍后再试", "确定", true, () =>
                    {
                        
                    }, null, null, "网络异常");
                }

            }
        });
    }

    /// <summary>
    /// 登陆到游戏服务器
    /// </summary>
    private void LoginToGameServer() {

        GamePatternLoginRequest req = new GamePatternLoginRequest();
        req.token = PlayerModel.Inst.Token;
        NetProcess.SendRequest<GamePatternLoginRequest>(req, MJProtoMap.CMD_Login, (msg) =>
        {

            GamePatternLoginResponse data = msg.Read<GamePatternLoginResponse>();
            GamePatternModel.Inst.mCurGameId = (eGameType)data.gameType;
            if (data.code == 1)
            {
#if GPS
                SixqinSDKManager.Inst.SendMsg(SixqinSDKManager.GET_LOCATION, 600000);
#endif
                if (string.IsNullOrEmpty(data.roomId))//不在游戏中
                {
                    if((eGameType)data.gameType == eGameType.MaJiang)//如果是在麻将的回放中就不处理
                    {
                        if(BaseView.ContainsView<MJGameBackUI>())
                            return;
                    }
                    SendGetRoomList();
                }
                else//在游戏中
                {
                    switch ((eGameType)data.gameType) {
                        case eGameType.MaJiang:
                            Global.Inst.GetController<MJGameController>().SendJoinRoom(data.roomId);
                            break;
                        case eGameType.NiuNiu:
                            Global.Inst.GetController<NNGameController>().SendJoinRoomReq(data.roomId);
                            break;
                        case eGameType.GoldFlower:
                            Global.Inst.GetController<XXGoldFlowerGameController>().SendJoinRoomReq(data.roomId);
                            break;
                        case eGameType.TenHalf:
                            Global.Inst.GetController<TenGameController>().SendJoinRoomReq(data.roomId);
                            break;
                    }
                }
            }
            else
            {
                Global.Inst.GetController<MainController>().BackToMain();
                GameUtils.ShowErrorTips(data.code);
            }
        });
    }


    #endregion

    #region 主动请求

    /// <summary>
    /// 获取房间列表
    /// </summary>
    public void SendGetRoomList(CallBack<GamePatternView> call = null) {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, ProtoIdMap.CMD_SendGetRoomList, (msg) => {
            SendGetRoomListAck ack = msg.Read<SendGetRoomListAck>();
            if (ack.code == 1) {
                //打开界面
                OpenWindow();
                List<string> names = new List<string>();
                names.Add(typeof(GamePatternView).Name);
                BaseView.CloseAllViewBut(names);

                GamePatternModel.Inst.mChouCheng = ack.data.chouCheng;
                GamePatternModel.Inst.mClubId =ack.data.clubId;

                PlayerModel.Inst.UserInfo.gold = ack.data.gold;
                PlayerModel.Inst.UserInfo.roomCard = ack.data.roomCard;

                //刷新界面
                if (ack.data.roomInfoList == null || ack.data.roomInfoList.Count == 0)
                {
                    GamePatternModel.Inst.mRoomList.Clear();
                }
                else {
                    GamePatternModel.Inst.mRoomList = ack.data.roomInfoList;
                }
                mView.SetData(ack.data.clubId, ack.data.totalNum, ack.data.onLineNum,ack.data.chouCheng, ack.data.clubName);
                if (call != null)
                    call(mView);
            }
        });

    }

    /// <summary>
    /// 设置抽成比例
    /// </summary>
    /// <param name="chouCheng"></param>
    public void SendChangChouCheng(float chouCheng,Action call) {
        SendChangChouCheng req = new global::SendChangChouCheng();
        req.clubId = GamePatternModel.Inst.mClubId;
        req.chouCheng = chouCheng;
        NetProcess.SendRequest<SendChangChouCheng>(req,ProtoIdMap.CMD_SendChangChouCheng, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1) {
                GamePatternModel.Inst.mChouCheng = chouCheng;
                Global.Inst.GetController<CommonTipsController>().ShowTips("设置抽成比例成功!");
            }
            else {
                if (call!=null) {
                    call();
                }
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 获取平台场的人数
    /// </summary>
    /// <param name="call"></param>
    public void SendGetGoldPeopleNum(CallBack<List<int>> call) {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, ProtoIdMap.CMD_SendGetGoldPeopleNum, (msg) =>
        {
            GamePatternSendGetGoldPeopleNumAck ack = msg.Read<GamePatternSendGetGoldPeopleNumAck>();
            if (ack.code==1) {
                if (call!=null && ack.data!=null) {
                    call(ack.data.nums);
                }
            }
        },false);
    }

    /// <summary>
    /// 检测是否还在游戏中
    /// </summary>
    /// <param name="call"></param>
    public void SendCheckInGame()
    {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, ProtoIdMap.CMD_SendCheckInGame, (msg) =>
        {
            GamePatternSendGetGoldPeopleNumAck ack = msg.Read<GamePatternSendGetGoldPeopleNumAck>();
            if (ack.code == 1)
            {
                Global.Inst.GetController<MainController>().BackToMain();
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        }, false);
    }

    #endregion


    #region 被动接受

    /// <summary>
    /// 房间信息法神变化
    /// </summary>
    /// <param name="msg"></param>
    private void OnRoomInfoChang(MessageData msg) {
        if (mView == null)
            return;
        OnRoomListChange ack = msg.Read<OnRoomListChange>();
        SendGetRoomListInfo info = ack.info;
        bool contain = false;
        if (GamePatternModel.Inst.mRoomList != null && !string.IsNullOrEmpty(info.model)) {//更新房间信息,且房间没被解散
            for (int i = 0; i < GamePatternModel.Inst.mRoomList.Count; i++)
            {
                if (GamePatternModel.Inst.mRoomList[i].roomId == info.roomId)
                {
                    contain = true;
                    bool nil = GamePatternModel.Inst.mRoomList[i].nil;
                    GamePatternModel.Inst.mRoomList[i] = info;
                    if (nil == info.nil) {//有空位还是有空位
                        mView.UpdateClubPatternWidget(i, info);//刷新一个
                    }
                    else {//空位状态改变
                        mView.InitClubPatternWidget();//全部刷新
                    }
                    break;
                }
            }
        }

        if (contain) {//更新成功 ，返回
            return;
        }

        if (string.IsNullOrEmpty(info.model))
        {//房间被解散了

            for (int i = 0; i < GamePatternModel.Inst.mRoomList.Count; i++)
            {
                if (GamePatternModel.Inst.mRoomList[i].roomId == info.roomId)
                {
                    GamePatternModel.Inst.mRoomList.Remove(GamePatternModel.Inst.mRoomList[i]);
                    break;
                }
            }
            mView.InitClubPatternWidget();
        }
        else {//新建了一个房间
            GamePatternModel.Inst.mRoomList.Add(info);
            mView.InitClubPatternWidget();
        }
    }

    /// <summary>
    /// 在线人数发生变化
    /// </summary>
    /// <param name="msg"></param>
    private void OnOnLinePersonChanged(MessageData msg) {
        OnGamePatternOnLinePersonChang ack = msg.Read<OnGamePatternOnLinePersonChang>();
        if (mView!=null) {
            mView.SetOnLinePeopleNum(ack.onLine);
        }
    }

    #endregion

}
