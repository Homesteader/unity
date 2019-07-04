using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NNGameController : BaseController {

    public NiuniuGameView mView;//view
    public NNGameController() : base("NiuniuGameView", AssetsPathDic.NiuniuGameView)
    {
        SetModel<NiuniuModel>();
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnGoldLess, NetOnGoldLess);//金币不足
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnRoomCardLess, NetOnRoomCardLess);//房卡不足
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnPlayerSeatDown, NetOnPlayerSeatDown);//有玩家坐下
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnPlayerLeave, NetOnPlayerLeave);//玩家离开
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnPlayerReady, NetOnPlayerReady);//玩家准备
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnPlayerOnOffLine, NetOnOnOffLine);//玩家上下线
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnGameTalk, NetOnGameTalk);//同步游戏聊天
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnGameStartLastTime, NetOnGameStartLastTime);//倒计时
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnGameStart, NetOnGameStart);//游戏开始
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnSelfGameOpt, NetOnSelfOpt);//自己获得操作指令
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnPlayerOptResult, NetOnPlayerOptResult);//玩家操作结果
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnChangZhuang, NetOnChangZhuang);//换庄
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnCastCard, NetOnCastCard);//发牌
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnSmallSettle, NetOnSmallSettle);//小结算
        NetProcess.RegisterResponseCallBack(NNProtoIdMap.CMD_OnAutoChangDesk, NetOnAutoChangDesk);//自动换桌
    }

    public override BaseView OpenWindow()
    {
        BaseView view = base.OpenWindow();
        if (mView == null)
            mView = view as NiuniuGameView;
        return view;
    }


    #region 创建房间和加入房间

    /// <summary>
    /// 创建房间
    /// </summary>
    /// <param name="req"></param>
    public void SendCreateRoomReq(SendCreateRoomReq req)
    {
        NetProcess.SendRequest<SendCreateRoomReq>(req, NNProtoIdMap.CMD_SendCreateRoom, (msg) =>
        {
            NNCreateJoinRoomAck ack = msg.Read<NNCreateJoinRoomAck>();
            if (ack.code == 1) {
                ServerCreateJoinRoom(ack.data);
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
    public void SendJoinRoomReq(string roomId)
    {
        NNSendJoinRoomReq req = new NNSendJoinRoomReq();
        req.roomId = roomId;
        NetProcess.SendRequest<NNSendJoinRoomReq>(req, NNProtoIdMap.CMD_SendJoinRoom, (msg) =>
        {
            NNCreateJoinRoomAck ack = msg.Read<NNCreateJoinRoomAck>();
            if (ack.code == 1)
            {
                ServerCreateJoinRoom(ack.data);
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
        NetProcess.SendRequest<SendGoldFlowerJoinGoldRoom>(req, NNProtoIdMap.CMD_SendJoinGoldPattern, (msg) =>
        {
            NNCreateJoinRoomAck ack = msg.Read<NNCreateJoinRoomAck>();
            if (ack.code == 1)
            {
                NiuniuModel.Inst.CleanModel(true);
                ServerCreateJoinRoom(ack.data);
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
    public void SendChangGoldPattern()
    {
        NiuniuModel.Inst.mChangeDesk = true;
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, NNProtoIdMap.CMD_SendChangDesk, (msg) =>
        {
            NNCreateJoinRoomAck ack = msg.Read<NNCreateJoinRoomAck>();
            if (ack.code == 1)
            {
                NiuniuModel.Inst.CleanModel(true);
                ServerCreateJoinRoom(ack.data);
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
            if (mView != null)
            {
                mView.mSelfPlayer.SetChanagDeskBtnState(true);
                mView.mSelfPlayer.SetReadyBtnState(true);
            }
        });
    }

    #endregion


    #region 进入游戏

    /// <summary>
    /// 进入游戏
    /// </summary>
    /// <param name="data"></param>
    private void ServerCreateJoinRoom(NNCreateJoinRoomData data) {
#if YYVOICE
        //登陆呀呀语音(step=2)
        YYsdkManager.instance.LoginVoiceServer(PlayerModel.Inst.UserInfo.userId);
#endif

        if (PlayerModel.Inst.address != null)
        {
            NetProcess.SendRequest<SendAddrReq>(PlayerModel.Inst.address, NNProtoIdMap.CMD_SendAddress, (msgData) =>
             {
                 SQDebug.Log("gps 返回");
             }, false);
        }


        NiuniuModel.Inst.mGameId = (eGameType)data.roomInfo.gameId;
        NiuniuModel.Inst.mSubGameId = data.roomInfo.subGameId;
        NiuniuModel.Inst.mRoomRules = data.roomInfo.rule;
        NiuniuModel.Inst.mZhuangSeatId = data.roomInfo.zhuangSeatId;
        NiuniuModel.Inst.mMySeatId = data.roomInfo.mySeatId;
        NiuniuModel.Inst.mGoldPattern = data.roomInfo.pt;
        NiuniuModel.Inst.mGameState = (eNNGameState)data.roomInfo.gameState;
        NiuniuModel.Inst.mRoomId = data.roomInfo.roomId;
        NiuniuModel.Inst.mRoomState = data.roomInfo.roomState;

        if (data.roomInfo.playerList != null) {
            for (int i = 0; i < data.roomInfo.playerList.Count; i++) {
                PlayerSeatDown(data.roomInfo.playerList[i]);
            }
        }

        if (data.roomInfo.handCardsList!=null) {
            if (data.roomInfo.handCardsList.myCardsInfo!=null) {
                NiuniuModel.Inst.mQzListValue = data.roomInfo.handCardsList.myCardsInfo.qzListValue;
                NiuniuModel.Inst.mXzListValue = data.roomInfo.handCardsList.myCardsInfo.xzListValue;
                if (data.roomInfo.handCardsList.myCardsInfo.qzListValue!=null || data.roomInfo.handCardsList.myCardsInfo.xzListValue!=null) {
                    NiuniuModel.Inst.mGameed = true;
                    NiuniuModel.Inst.mGameedSeatIdList.Add(data.roomInfo.mySeatId);
                }
            }
            if (data.roomInfo.handCardsList.otherCardsInfo!=null) {
                for (int i=0;i< data.roomInfo.handCardsList.otherCardsInfo.Count;i++) {
                    if (data.roomInfo.handCardsList.otherCardsInfo[i].qzListValue!=null ||
                        data.roomInfo.handCardsList.otherCardsInfo[i].xzListValue!=null) {
                        NiuniuModel.Inst.mGameedSeatIdList.Add(data.roomInfo.handCardsList.otherCardsInfo[i].seatId);
                    }
                    
                }
            }
        }

        OpenWindow();

        mView.ServerCreateJoinRoom(data);
        if (mView != null)
        {
            mView.mSelfPlayer.SetChanagDeskBtnState(true);
            mView.mSelfPlayer.SetReadyBtnState(true);
        }
        List<string> names = new List<string>();
        names.Add(typeof(NiuniuGameView).Name);
        BaseView.CloseAllViewBut(names);
    }

    #endregion


    #region 辅助函数


    /// <summary>
    /// 玩家坐下
    /// </summary>
    /// <param name="player"></param>
    private void PlayerSeatDown(NNPlayerInfo player) {
        if (!NiuniuModel.Inst.mSeatList.Contains(player.seatId))
        {
            NiuniuModel.Inst.mSeatList.Add(player.seatId);
        }

        if (NiuniuModel.Inst.mPlayerInfoDic.ContainsKey(player.seatId))
        {
            NiuniuModel.Inst.mPlayerInfoDic[player.seatId] = player;
        }
        else {
            NiuniuModel.Inst.mPlayerInfoDic.Add(player.seatId, player);
        }
    }


    #endregion

    #region 被动

    /// <summary>
    /// 金币不足
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnGoldLess(MessageData msg)
    {
        CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
        Global.Inst.GetController<CommonTipsController>().ShowTips("金币不足");
    }

    /// <summary>
    /// 房卡不足
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnRoomCardLess(MessageData msg)
    {
        CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
        Global.Inst.GetController<CommonTipsController>().ShowTips("房卡不足");
    }

    /// <summary>
    /// 有玩家坐下
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnPlayerSeatDown(MessageData msg) {
        NNOnPlayerSeatDown ack = msg.Read<NNOnPlayerSeatDown>();
        PlayerSeatDown(ack.player);
        mView.NetOnPlayerSeatDown(ack);
    }

    /// <summary>
    /// 有玩家离开
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnPlayerLeave(MessageData msg) {
        NNOnPlayerLeave ack = msg.Read<NNOnPlayerLeave>();
        NiuniuModel.Inst.mPlayerInfoDic.Remove(ack.seatId);
        NiuniuModel.Inst.mSeatList.Remove(ack.seatId);
        if (ack.seatId == NiuniuModel.Inst.mMySeatId && NiuniuModel.Inst.mChangeDesk == false)
        {
            bool gold = NiuniuModel.Inst.mGoldPattern;
            NiuniuModel.Inst.CleanModel();
            Global.Inst.GetController<GamePatternController>().SendGetRoomList((patternView) =>
            {
                SQSceneLoader.It.LoadScene("HALL", () =>
                {
                    Global.Inst.GetController<GamePatternController>().OpenWindow();
                    CloseWindow();
                });
                if (!gold)
                {
                    patternView.BackToFriendWidget();
                }
            });
        }
        else {
            mView.NetOnPlayerLeave(ack.seatId);
        }
    }

    /// <summary>
    /// 有玩家准备
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnPlayerReady(MessageData msg) {
        NNOnPlayerReady ack = msg.Read<NNOnPlayerReady>();
        mView.NetOnPlayerReady(ack.seatId);
    }

    /// <summary>
    /// 游戏开始倒计时
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnGameStartLastTime(MessageData msg) {
        NNOnGameStartLastTime ack = msg.Read<NNOnGameStartLastTime>();
        mView.NetOnGameStartLastTime(ack.lastTime);
    }

    /// <summary>
    /// 玩家上下线
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnOnOffLine(MessageData msg) {
        NNOnPlayerOffLine ack = msg.Read<NNOnPlayerOffLine>();
        mView.NetOnOnOffLine(ack);
    }

    /// <summary>
    /// 同步游戏聊天
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnGameTalk(MessageData msg)
    {
        SendReceiveGameChat ack = msg.Read<SendReceiveGameChat>();
        mView.NetOnGameTalk(ack);
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnGameStart(MessageData msg) {
        NNOnGameStart ack = msg.Read<NNOnGameStart>();
        NiuniuModel.Inst.CleanModel();
        NiuniuModel.Inst.mZhuangSeatId = ack.zhuangSeatId;
        mView.NetOnGameStart(ack.zhuangSeatId);
    }

    /// <summary>
    /// 自己获得操作指令
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnSelfOpt(MessageData msg) {
        NNOnSelfOpt ack = msg.Read<NNOnSelfOpt>();
        if (ack.qzListValue!=null) {
            NiuniuModel.Inst.mQzListValue = ack.qzListValue;
        }
        if (ack.xzListValue!=null) {
            NiuniuModel.Inst.mXzListValue = ack.xzListValue;
        }
        NiuniuModel.Inst.mGameState = (eNNGameState)ack.gameState;
        if (ack.gameSeatIdList!=null) {
            NiuniuModel.Inst.mGameedSeatIdList = ack.gameSeatIdList;
            if (ack.gameSeatIdList.Contains(NiuniuModel.Inst.mMySeatId))
            {
                NiuniuModel.Inst.mGameed = true;
            }
        }
        mView.NetOnSelfOpt(ack);
    }

    /// <summary>
    /// 玩家操作结果
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnPlayerOptResult(MessageData msg) {
        NNOnPlayerOptResult ack = msg.Read<NNOnPlayerOptResult>();
        mView.NetOnPlayerOptResult(ack);
    }

    /// <summary>
    /// 同步玩家发牌
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnCastCard(MessageData msg) {
        NNonCastCard ack = msg.Read<NNonCastCard>();
        if (ack.gamedSeatIdList!=null) {
            NiuniuModel.Inst.mGameedSeatIdList = ack.gamedSeatIdList;
            if (ack.gamedSeatIdList.Contains(NiuniuModel.Inst.mMySeatId)) {
                NiuniuModel.Inst.mGameed = true;
            }
        }
        mView.NetOnCastCard(ack);
    }

    /// <summary>
    /// 同步小结算
    /// </summary>
    private void NetOnSmallSettle(MessageData msg) {
        NNonSmallSettle ack = msg.Read<NNonSmallSettle>();
        mView.NetOnSmallSettle(ack);
    }

    /// <summary>
    /// 换庄
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnChangZhuang(MessageData msg) {
        NNOnChangZhuang ack = msg.Read<NNOnChangZhuang>();
        NiuniuModel.Inst.mZhuangSeatId = ack.zhuangSeatId;
        mView.NetOnChangZhuang(ack);
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


    #endregion



    #region 主动


    /// <summary>
    /// 发送准备
    /// </summary>
    public void SendReady()
    {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, NNProtoIdMap.CMD_SendReady, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {

            }
            else if (ack.code == 104 || ack.code == 105 || ack.code == 24 || ack.code == 13 || ack.code == 7 || ack.code == 6)
            {
                bool gold = NiuniuModel.Inst.mGoldPattern;
                NiuniuModel.Inst.CleanModel();
                Global.Inst.GetController<GamePatternController>().SendGetRoomList((patternView) =>
                {
                    SQSceneLoader.It.LoadScene("HALL", () =>
                    {
                        Global.Inst.GetController<GamePatternController>().OpenWindow();
                        CloseWindow();
                    });
                    if (!gold)
                    {
                        patternView.BackToFriendWidget();
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
    /// 离开房间
    /// </summary>
    public void SendLeaveRoom()
    {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, NNProtoIdMap.CMD_SendLeaveRoom, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {

            }
            else if (ack.code == 104 || ack.code == 105 || ack.code == 24 || ack.code == 13 || ack.code == 7 || ack.code == 6)
            {
                bool gold = NiuniuModel.Inst.mGoldPattern;
                NiuniuModel.Inst.CleanModel();
                Global.Inst.GetController<GamePatternController>().SendGetRoomList((patternView) =>
                {
                    SQSceneLoader.It.LoadScene("HALL", () =>
                    {
                        Global.Inst.GetController<GamePatternController>().OpenWindow();
                        CloseWindow();
                    });
                    if (!gold)
                    {
                        patternView.BackToFriendWidget();
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
    /// 获取玩家距离
    /// </summary>
    public void SendGetPlayerDistances() {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, NNProtoIdMap.CMD_SendGetDistances, (msg) =>
        {
            NNSendDistanceAck ack = msg.Read<NNSendDistanceAck>();
            if (ack.code == 1)
            {
                GameDistanceWidget widget = BaseView.GetWidget<GameDistanceWidget>(AssetsPathDic.GameDistanceWidget, mView.transform);
                if (ack.data.distances != null)
                {
                    for (int i = 0; i < ack.data.distances.Count; i++)
                    {
                        NNSendDistanceInfo dis = ack.data.distances[i];
                        widget.AddDistancePaire(dis.leftHead, dis.leftName, dis.leftUid, dis.RightHead, dis.RightName, dis.RightUid, dis.distance + "Km");
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
    public void SendGetPlayerInfo(string uid, int seatId)
    {
        NNSendGetUserInfoReq req = new NNSendGetUserInfoReq();
        req.uid = uid;
        NetProcess.SendRequest<NNSendGetUserInfoReq>(req, NNProtoIdMap.CMD_SendGetUserInfo, (Msg) =>
        {
            NNSendGetUserInfoAck ack = Msg.Read<NNSendGetUserInfoAck>();
            if (ack.code == 1)
            {
                GameUserInfoWidget widget = BaseView.GetWidget<GameUserInfoWidget>(AssetsPathDic.GameUserInfoWidget, mView.transform);
                widget.SetData(!(seatId == NiuniuModel.Inst.mMySeatId), ack.data.info.headUrl, ack.data.info.nickName, ack.data.info.userId, ack.data.info.distance + "", seatId, (index) =>
                  {
                      SendReceiveGameChat chat = new SendReceiveGameChat();
                      chat.fromSeatId = NiuniuModel.Inst.mMySeatId;
                      chat.toSeatId = seatId;
                      chat.faceIndex = index;
                      chat.chatType = (int)eGameChatContentType.HDFace;
                      SendGameChat(chat);
                  });
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }


    /// <summary>
    /// 发送聊天
    /// </summary>
    /// <param name="req"></param>
    public void SendGameChat(SendReceiveGameChat req)
    {

        NetProcess.SendRequest<SendReceiveGameChat>(req, NNProtoIdMap.CMD_SendGameTalk, (Msg) =>
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
    /// 发送操作指令
    /// </summary>
    /// <param name="req"></param>
    public void SendGameOpt(NNSendGameOpt req) {
        NetProcess.SendRequest<NNSendGameOpt>(req, NNProtoIdMap.CMD_SendGameOpt, (Msg) =>
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

    #endregion

}
