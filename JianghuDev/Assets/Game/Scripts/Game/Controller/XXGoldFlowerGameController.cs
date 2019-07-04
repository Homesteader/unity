using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XXGoldFlowerGameController : BaseController {

    public XXGoldFlowerGameView mView;//view
    public XXGoldFlowerGameController() : base("XXGoldFlowerGameView", AssetsPathDic.XXGoldFlowerGameView)
    {
        SetModel<XXGoldFlowerGameModel>();
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnGoldLess, NetOnGoldLess);//金币不足
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnPlayerLeave, NetOnPlayerLeaveRoom);//玩家离开房间
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnPlayerSeatDown, NetOnPlayerSeatDown);//有玩家坐下
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnPlayerReady, NetOnPlayerReady);//玩家准备
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnGameStart, NetOnGameStart);//游戏开始
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnGameOptResult, NetOnGameOptResult);//玩家操作结果
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnGameStartLastTime, NetOnGameStartLastTime);//倒计时
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnSelfGameOpt, NetOnSelfGameOpt);//可操作列表
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnRoomCardLess, NetOnRoomCardLess);//代理的房卡不足
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnSamllSettle, NetOnSmallSettle);//小结算
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnPlayerOnOffLine, NetOnOnOffLine);//玩家上下线
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnPlayerShowCard, NetOnPlayerShowCards);//有玩家亮牌
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnUpdateRound, NetOnUpdateRound);//更新轮数
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnGameTalk, NetOnGameTalk);//同步游戏聊天
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnLastFight, NetOnLastFight);//最后一站
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnAutoChangDesk, NetOnAutoChangDesk);//自动换桌
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnJiesan, NetOnResolve);//房间解散
        NetProcess.RegisterResponseCallBack(GoldFlowerProtoIdMap.CMD_OnDisolveTime, NetOnResolveTime);//房间解散时间
    }

    public override BaseView OpenWindow()
    {
        BaseView view = base.OpenWindow();
        if (mView == null)
            mView = view as XXGoldFlowerGameView;
        return view;
    }


    #region 创建房间和加入房间

    /// <summary>
    /// 创建房间
    /// </summary>
    /// <param name="req"></param>
    public void SendCreateRoomReq(SendCreateRoomReq req) {
        NetProcess.SendRequest<SendCreateRoomReq>(req, GoldFlowerProtoIdMap.CMD_SendCreateRoom, (msg) =>
        {
            GoldFlowerCreateRoomAck ack = msg.Read<GoldFlowerCreateRoomAck>();
            if (ack.code == 1) {
                XXGoldFlowerGameModel.Inst.CleanMode(true);
                ServerCreateJoinGame(ack.data);
            }
            else {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    /// <param name="roomId"></param>
    public void SendJoinRoomReq(string roomId) {
        SendGoldFlowerJoinRoomReq req = new SendGoldFlowerJoinRoomReq();
        req.roomId = roomId;
        NetProcess.SendRequest<SendGoldFlowerJoinRoomReq>(req, GoldFlowerProtoIdMap.CMD_SendJoinRoom, (msg) =>
        {
            GoldFlowerCreateRoomAck ack = msg.Read<GoldFlowerCreateRoomAck>();
            if (ack.code == 1)
            {
                XXGoldFlowerGameModel.Inst.CleanMode(true);
                ServerCreateJoinGame(ack.data);
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 加入金币场模式
    /// </summary>
    /// <param name="id"></param>
    public void SendJoinGoldPattern(int id)
    {
        SendGoldFlowerJoinGoldRoom req = new SendGoldFlowerJoinGoldRoom();
        req.id = id;
        SQDebug.Log("11111111111111");
        NetProcess.SendRequest<SendGoldFlowerJoinGoldRoom>(req, GoldFlowerProtoIdMap.CMD_SendJoinGoldPattern, (msg) =>
        {
            GoldFlowerCreateRoomAck ack = msg.Read<GoldFlowerCreateRoomAck>();
            if (ack.code == 1)
            {
                XXGoldFlowerGameModel.Inst.CleanMode(true);
                ServerCreateJoinGame(ack.data);
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 切换房间
    /// </summary>
    public void SendChangGoldPattern() {
        XXGoldFlowerGameModel.Inst.mChangDesk = true;
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, GoldFlowerProtoIdMap.CMD_SendChangGoldRoom, (msg) =>
        {
            GoldFlowerCreateRoomAck ack = msg.Read<GoldFlowerCreateRoomAck>();
            if (ack.code == 1)
            {
                XXGoldFlowerGameModel.Inst.CleanMode(true);
                ServerCreateJoinGame(ack.data);
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
            if (mView!=null) {
                mView.mSelfPlayer.SetChanagDeskBtnState(true);
                mView.mSelfPlayer.SetReadyBtnState(true);
            }
        });
    }

    //开始游戏
    public void StartGame()
    {
        NetProcess.SendRequest<CommonSendProto>(null, GoldFlowerProtoIdMap.CMD_SendStartGame, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    #endregion


    #region 进入游戏

    /// <summary>
    /// 进入游戏
    /// </summary>
    /// <param name="data"></param>
    public void ServerCreateJoinGame(GoldFlowerCreateRoomData data) {

#if YYVOICE
        //登陆呀呀语音(step=2)
        YYsdkManager.instance.LoginVoiceServer(PlayerModel.Inst.UserInfo.userId);
#endif

        if (PlayerModel.Inst.address != null)
        {
            NetProcess.SendRequest<SendAddrReq>(PlayerModel.Inst.address, GoldFlowerProtoIdMap.CMD_SendAddress, (msgData) =>
             {
                 SQDebug.Log("gps 返回");
             }, false);
        }

        XXGoldFlowerGameModel.Inst.mRoomRules = data.roomInfo.rule;
        XXGoldFlowerGameModel.Inst.mDichi = data.roomInfo.dichi;
        XXGoldFlowerGameModel.Inst.mDiFen = data.roomInfo.difen;
        XXGoldFlowerGameModel.Inst.mSubGameId = data.roomInfo.gameId;
        XXGoldFlowerGameModel.Inst.mRoomId = data.roomInfo.roomId;
        XXGoldFlowerGameModel.Inst.mMySeatId = data.roomInfo.mySeatId;
        XXGoldFlowerGameModel.Inst.mTurnSeatId = data.roomInfo.turnSeatId;
        XXGoldFlowerGameModel.Inst.mZhuangSeatId = data.roomInfo.zhuangSeatId;
        XXGoldFlowerGameModel.Inst.mRound = data.roomInfo.round;
        XXGoldFlowerGameModel.Inst.mStartInfo = new OnGoldFlowerGameStart();
        XXGoldFlowerGameModel.Inst.mStartInfo.lookRate = data.roomInfo.lookRate;
        XXGoldFlowerGameModel.Inst.mStartInfo.menRate = data.roomInfo.menRate;
        XXGoldFlowerGameModel.Inst.mGoldPattern = data.roomInfo.goldPattern;
        XXGoldFlowerGameModel.Inst.RoomState = (eGFGameState)data.roomInfo.gameState;//房间状态

        for (int i = 0; i < data.roomInfo.playerList.Count; i++) {
            PlayerSeatDown(data.roomInfo.playerList[i]);
        }

        OpenWindow();

        List<string> names = new List<string>();
        names.Add(typeof(XXGoldFlowerGameView).Name);
        BaseView.CloseAllViewBut(names);
        mView.ServerCreateJoinGame(data);
        if (mView != null)
        {
            mView.mSelfPlayer.SetChanagDeskBtnState(true);
            mView.mSelfPlayer.SetReadyBtnState(true);
        }
    }

    #endregion

    #region 被动接受

    /// <summary>
    /// 金币不足
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnGoldLess(MessageData msg) {
        CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
        Global.Inst.GetController<CommonTipsController>().ShowTips("您的金币不足");
    }

    /// <summary>
    /// 玩家离开房间
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnPlayerLeaveRoom(MessageData msg)
    {
        OnGoldFlowerPlayerLeave ack = msg.Read<OnGoldFlowerPlayerLeave>();
        XXGoldFlowerGameModel.Inst.mSeatIdList.Remove(ack.seatId);
        XXGoldFlowerGameModel.Inst.mPlayerInfoDic.Remove(ack.seatId);
        if (ack.seatId == XXGoldFlowerGameModel.Inst.mMySeatId && XXGoldFlowerGameModel.Inst.mChangDesk == false)//不是换桌造成的这种情况
        {
            bool gold = XXGoldFlowerGameModel.Inst.mGoldPattern;
            Global.Inst.GetController<MainController>().SendGetWareInfo(() =>
            {
                if (XXGoldFlowerGameModel.Inst.mGoldPattern)
                {//平台房
                    XXGoldFlowerGameModel.Inst.CleanMode();
                    Global.Inst.GetController<SelectRoomController>().SendGetGoldPeopleNum((data) =>
                    {
                        SelectRoomView v = Global.Inst.GetController<SelectRoomController>().OpenWindow() as SelectRoomView;
                        v.SetData(data);
                        Global.Inst.GetController<MainController>().OpenWindow();
                        CloseWindow();
                    });
                }
                else
                {
                    Global.Inst.GetController<MainController>().OpenWindow();
                    CloseWindow();
                }
            });
        }else
            mView.NetOnPlayerLeaveRoom(ack.seatId);
    }

    /// <summary>
    /// 有玩家坐下
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnPlayerSeatDown(MessageData msg) {
        OnGoldFlowerOnSeatDown ack = msg.Read<OnGoldFlowerOnSeatDown>();
        GoldFlowerPlayer player = ack.info;
        if (player.userId == PlayerModel.Inst.UserInfo.userId) {
            XXGoldFlowerGameModel.Inst.mMySeatId = player.seatId;
        }
        PlayerSeatDown(player);
        mView.NetOnPlayerSeatDown(player);
    }

    /// <summary>
    /// 同步玩家准备
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnPlayerReady(MessageData msg) {
        OnGoldFlowerReady ack = msg.Read<OnGoldFlowerReady>();
        mView.NetOnPlayerReady(ack.seatId);
    }

    /// <summary>
    /// 游戏开始倒计时
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnGameStartLastTime(MessageData msg) {
        OngoldFlowerGameStartLastTime ack = msg.Read<OngoldFlowerGameStartLastTime>();
        mView.NetOnGameStartLastTime(ack.lastTime);
    }

    /// <summary>
    /// 同步游戏开始
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnGameStart(MessageData msg) {
        OnGoldFlowerGameStart ack = msg.Read<OnGoldFlowerGameStart>();
        XXGoldFlowerGameModel.Inst.CleanMode();
        XXGoldFlowerGameModel.Inst.mStartInfo = ack;
        XXGoldFlowerGameModel.Inst.mZhuangSeatId = ack.zhuangSeatId;
        XXGoldFlowerGameModel.Inst.RoomState = eGFGameState.Start;
        mView.NetOnGameStart();
    }

    /// <summary>
    /// 同步玩家操作结果
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnGameOptResult(MessageData msg) {
        OnGoldFlowerPlayerOptResult ack = msg.Read<OnGoldFlowerPlayerOptResult>();
        mView.NetOnGameOptResult(ack);
    }

    /// <summary>
    /// 同步自己可操作列表
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnSelfGameOpt(MessageData msg) {
        GoldFlowOptData ack = msg.Read<GoldFlowOptData>();
        int oldSeatId = XXGoldFlowerGameModel.Inst.mTurnSeatId;
        XXGoldFlowerGameModel.Inst.mTurnSeatId = ack.seatId;
        mView.TurnOptSeat(oldSeatId, ack.seatId, ack.time);

        if (ack.seatId == XXGoldFlowerGameModel.Inst.mMySeatId || ack.seatId == 0)
        {
            if (ack.seatId == XXGoldFlowerGameModel.Inst.mMySeatId) {
                XXGoldFlowerGameModel.Inst.mOpt = ack.data;
            }
            mView.NetOnSelfGameOpt(ack.data);
        }
        
    }

    /// <summary>
    /// 代理的房卡不足
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnRoomCardLess(MessageData msg) {
        CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
        Global.Inst.GetController<CommonTipsController>().ShowTips("房卡不足！");
    }

    /// <summary>
    /// 收到小结算
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnSmallSettle(MessageData msg) {
        GoldSettlementData ack = msg.Read<GoldSettlementData>();
        XXGoldFlowerGameModel.Inst.RoomState = eGFGameState.Ready;
        mView.NetOnSmallSettle(ack);
    }

    /// <summary>
    /// 玩家上下线
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnOnOffLine(MessageData msg)
    {
        OnGoldFlowerOnOffLine ack = msg.Read<OnGoldFlowerOnOffLine>();
        mView.NetOnOnOffLine(ack);
    }

    /// <summary>
    /// 有玩家亮牌
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnPlayerShowCards(MessageData msg) {
        if (XXGoldFlowerGameModel.Inst.RoomState == eGFGameState.Start)
            return;
        OnGoldFlowerShowCard ack = msg.Read<OnGoldFlowerShowCard>();
        mView.NetOnPlayerShowCards(ack);
    }

    /// <summary>
    ///更新轮数
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnUpdateRound(MessageData msg) {
        OnGoldFlowerUpdateRound ack = msg.Read<OnGoldFlowerUpdateRound>();
        XXGoldFlowerGameModel.Inst.mRound = ack.round;
        mView.NetOnUpdateRound(ack.round);
    }

    /// <summary>
    /// 同步游戏聊天
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnGameTalk(MessageData msg) {
        SendReceiveGameChat ack = msg.Read<SendReceiveGameChat>();
        mView.NetOnGameTalk(ack);
    }

    /// <summary>
    /// 最后一站
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnLastFight(MessageData msg) {
        OnGetGoldFlowerLastFight ack = msg.Read<OnGetGoldFlowerLastFight>();
        mView.NetOnLastFight(ack);
    }

    /// <summary>
    /// 自动换桌
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnAutoChangDesk(MessageData msg) {
        CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
        SendChangGoldPattern();
        mView.NetOnAutoChangDesk();
    }

    //房间解散
    private void NetOnResolve(MessageData msg)
    {
        Global.Inst.GetController<CommonTipsController>().ShowTips("房间已解散");
        if (XXGoldFlowerGameModel.Inst.mGoldPattern)
        {//平台房
            XXGoldFlowerGameModel.Inst.CleanMode();
            Global.Inst.GetController<SelectRoomController>().SendGetGoldPeopleNum((data) =>
            {
                SelectRoomView v = Global.Inst.GetController<SelectRoomController>().OpenWindow() as SelectRoomView;
                v.SetData(data);
                Global.Inst.GetController<MainController>().OpenWindow();
                CloseWindow();
            });
        }
        else
        {
            Global.Inst.GetController<MainController>().OpenWindow();
            CloseWindow();
        }
    }

    //房间解散时间
    private void NetOnResolveTime(MessageData msg)
    {
        RecieveDisolveTime t = msg.Read<RecieveDisolveTime>();
        if (mView != null)
            mView.SetDisolveTime(t.time);
    }
    #endregion


    #region 辅助函数

    /// <summary>
    /// 玩家坐下
    /// </summary>
    /// <param name="player"></param>
    private void PlayerSeatDown(GoldFlowerPlayer player) {
        if (!XXGoldFlowerGameModel.Inst.mSeatIdList.Contains(player.seatId))
        {
            XXGoldFlowerGameModel.Inst.mSeatIdList.Add(player.seatId);
        }
        if (!XXGoldFlowerGameModel.Inst.mPlayerInfoDic.ContainsKey(player.seatId))
        {
            XXGoldFlowerGameModel.Inst.mPlayerInfoDic.Add(player.seatId, player);
        }
        else
        {
            XXGoldFlowerGameModel.Inst.mPlayerInfoDic[player.seatId] = player;
        }
        if (player.seatId == XXGoldFlowerGameModel.Inst.mMySeatId) {
            XXGoldFlowerGameModel.Inst.mAutoGen = player.autoGen;
        }
    }

    #endregion


    #region 主动请求

    /// <summary>
    /// 发送准备
    /// </summary>
    public void SendReady(CallBack call = null) {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, GoldFlowerProtoIdMap.CMD_SendRready, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {

            }
            else if (ack.code == 104 || ack.code == 105 || ack.code == 24 || ack.code == 13 || ack.code == 7 || ack.code == 6)
            {
                bool gold = XXGoldFlowerGameModel.Inst.mGoldPattern;
                XXGoldFlowerGameModel.Inst.CleanMode();
                Global.Inst.GetController<MainController>().OpenWindow();
                CloseWindow();
            }
            else
            {
                //GameUtils.ShowErrorTips(ack.code);
                if (call != null)
                    call();
            }
        });
    }

    /// <summary>
    /// 发送指令
    /// </summary>
    /// <param name="opt"></param>
    public void SendOpt(SendGoldFlowerOpt opt, CallBack call = null)
    {
        NetProcess.SendRequest<SendGoldFlowerOpt>(opt, GoldFlowerProtoIdMap.CMD_SendOpt, (msg) =>
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
    /// 发送亮牌
    /// </summary>
    public void SendShowCard() {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, GoldFlowerProtoIdMap.CMD_SendShowCard, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {   

            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 离开房间
    /// </summary>
    public void SendLeaveRoom() {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, GoldFlowerProtoIdMap.CMD_SendLeaveRoom, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {

            }
            else if (ack.code == 104 || ack.code == 105 || ack.code ==24 || ack.code==13|| ack.code==7|| ack.code==6) {
                Global.Inst.GetController<MainController>().SendGetWareInfo(() =>
                {
                    if (XXGoldFlowerGameModel.Inst.mGoldPattern)
                    {//平台房
                        XXGoldFlowerGameModel.Inst.CleanMode();
                        Global.Inst.GetController<SelectRoomController>().SendGetGoldPeopleNum((data) =>
                        {
                            SelectRoomView v = Global.Inst.GetController<SelectRoomController>().OpenWindow() as SelectRoomView;
                            v.SetData(data);
                            Global.Inst.GetController<MainController>().OpenWindow();
                            CloseWindow();
                        });
                    }
                    else
                    {
                        Global.Inst.GetController<MainController>().OpenWindow();
                        CloseWindow();
                    }
                });
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 获取玩家之间的距离
    /// </summary>
    public void SendGetPlayerDistances() {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, GoldFlowerProtoIdMap.CMD_SendGetPlayerDistances, (msg) =>
        {
            SendGoldFlowerDistanceAck ack = msg.Read<SendGoldFlowerDistanceAck>();
            if (ack.code == 1)
            {
                GameDistanceWidget widget = BaseView.GetWidget<GameDistanceWidget>(AssetsPathDic.GameDistanceWidget, mView.transform);
                if (ack.data.distances!=null) {
                    for (int i=0;i< ack.data.distances.Count;i++) {
                        SendGoldFlowerDistanceInfo dis = ack.data.distances[i];
                        widget.AddDistancePaire(dis.leftHead, dis.leftName, dis.leftUid, dis.RightHead, dis.RightName, dis.RightUid, dis.distance+"Km");
                    }
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 获取玩家信息
    /// </summary>
    /// <param name="uid"></param>
    public void SendGetPlayerInfo(string uid,int seatId){
        SendGetGoldFlowerUserInfoReq req = new SendGetGoldFlowerUserInfoReq();
        req.uid = uid;
        NetProcess.SendRequest<SendGetGoldFlowerUserInfoReq>(req, GoldFlowerProtoIdMap.CMD_SendGetPlayerInfo, (Msg) =>
        {
            SendGetGoldFlowerUserInfoAck ack = Msg.Read<SendGetGoldFlowerUserInfoAck>();
            if (ack.code ==1) {
                GameUserInfoWidget widget = BaseView.GetWidget<GameUserInfoWidget>(AssetsPathDic.GameUserInfoWidget, mView.transform);
                widget.SetData(!(seatId == XXGoldFlowerGameModel.Inst.mMySeatId), ack.data.info.headUrl, ack.data.info.nickName, ack.data.info.userId, ack.data.info.address+"", seatId, (index) =>
                {
                    SendReceiveGameChat chat = new SendReceiveGameChat();
                    chat.fromSeatId = XXGoldFlowerGameModel.Inst.mMySeatId;
                    chat.toSeatId = seatId;
                    chat.faceIndex = index;
                    chat.chatType = (int)eGameChatContentType.HDFace;
                    SendGameChat(chat);
                });
            }
            else {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 发送聊天
    /// </summary>
    /// <param name="req"></param>
    public void SendGameChat(SendReceiveGameChat req) {

        NetProcess.SendRequest<SendReceiveGameChat>(req, GoldFlowerProtoIdMap.CMD_SendGameTalk, (Msg) =>
        {
            CommonRecieveProto ack = Msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {
                
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 发起自动跟注
    /// </summary>
    /// <param name="state"></param>
    /// <param name="call"></param>
    public void SendAutoGen(int state, CallBack call) {
        SendAutoGenReq req = new SendAutoGenReq();
        req.auto = state;
        NetProcess.SendRequest<SendAutoGenReq>(req, GoldFlowerProtoIdMap.CMD_SendAutoGen, (Msg) =>
        {
            SendAutoGenAck ack = Msg.Read<SendAutoGenAck>();
            if (ack.code == 1)
            {
                if (call!=null) {
                    XXGoldFlowerGameModel.Inst.mAutoGen = ack.data.auto == 1 ? true : false;
                    call();
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    #endregion
}
