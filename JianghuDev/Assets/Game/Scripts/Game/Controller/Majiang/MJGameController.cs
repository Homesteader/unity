using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MJGameController : BaseController
{
    public MJGameUI mGameUI;
    public MJGameModel mModel;
    private const string VOICE_MJ_MAN = "LZMJ/Majiang/Man/";//牌音效 男
    private const string VOICE_MJ_WOMAN = "LZMJ/Majiang/Woman/";//牌音效 女
    private const string VOICE_INS_MAN = "LZMJ/Ins/Man/";//操作 男
    private const string VOICE_INS_WOMAN = "LZMJ/Ins/Woman/";//操作 女
    private const string VOICE_CHAT_MAN = "Chat/Man/";//聊天 男
    private const string VOICE_CHAT_WOMAN = "Chat/Woman/";//操作 女
    private const string VOICE_HITCARD = "LZMJ/Other/dapai";//打牌的声音
    private const string VOICE_GETCARD = "LZMJ/Other/mopai";//摸牌的声音
    private const string VOICE_INS_OTHER = "LZMJ/Other/";//其他操作声音

    public MJGameController() : base("GameUI", "Windows/Majiang/GameUI")
    {

        NetProcess.RegisterResponseCallBack(MJProtoMap.Cmd_GetGameStart, HandleStartGameData);//麻将游戏开始 发牌
        NetProcess.RegisterResponseCallBack(MJProtoMap.Cmd_GetOnOptList, GetOnOptListACK);//同步 玩家可以操作什么指令
        NetProcess.RegisterResponseCallBack(MJProtoMap.Cmd_GetOnWhoOptIns, GetInstructionsACK);//同步谁操作了什么指令
        NetProcess.RegisterResponseCallBack(MJProtoMap.Cmd_GetSettlement, OnSettlement);//同步小结算
        NetProcess.RegisterResponseCallBack(MJProtoMap.Cmd_GetBigSem, OnSettlementFinal);//大结算
        NetProcess.RegisterResponseCallBack(MJProtoMap.CMD_PlayerJoinInRoom, OnPlayerJoinInRoom);//有玩家加入房间
        NetProcess.RegisterResponseCallBack(MJProtoMap.CMD_OnGetChat, NetOnGameTalk);//同步游戏聊天
        NetProcess.RegisterResponseCallBack(MJProtoMap.CMD_ReadyCountDown, OnReadyCountDown);//同步游戏准备倒计时

        //NetProcess.RegisterResponseCallBack(MJProtoMap.Cmd_GetErrorCode, (msg) =>
        //{
        //    ResponeErrorCode data = msg.Read<ResponeErrorCode>();
        //    GameUtils.ShowErrorTips(data.code);
        //});//错误列表


        SetModel<MJGameModel>();
        mModel = MJGameModel.Inst;
    }


    #region 给服务器发送消息

    public void CreateRoom(SendCreateRoomReq req)
    {
        Global.Inst.GetController<GamePatternController>().mView = null;
        NetProcess.SendRequest<SendCreateRoomReq>(req, MJProtoMap.CMD_CreateRoom, (msg) =>
        {
            StartGameBackData data = msg.Read<StartGameBackData>();
            if (data.code == 1)
            {
                //发送位置信息给服务器
                if (PlayerModel.Inst.address != null)
                {
                    NetProcess.SendRequest<SendAddrReq>(PlayerModel.Inst.address, MJProtoMap.CMD_SendAddress, (msgData) =>
                    {
                        SQDebug.Log("gps 返回");
                    }, false);
                }
                HandleStartGameData(data.data);
            }
            else
                GameUtils.ShowErrorTips(data.code);
        });
    }


    /// <summary>
    /// 连接大厅
    /// </summary>
    /// <param name="call"></param>
    public void ConnectedToHallServer(CallBack call)
    {

        //SQSceneLoader.It.LoadScene("Home", null);
        SQSceneLoader.It.LoadScene("Start", null);
        CloseWindow();
    }

    /// <summary>
    /// 加入金币场模式
    /// </summary>
    /// <param name="id"></param>
    public void SendJoinMJPattern(int id)
    {
        SendGoldFlowerJoinGoldRoom req = new SendGoldFlowerJoinGoldRoom();
        req.id = id;
        NetProcess.SendRequest<SendGoldFlowerJoinGoldRoom>(req, MJProtoMap.CMD_SendJoinMJPattern, (msg) =>
        {
            StartGameBackData data = msg.Read<StartGameBackData>();
            if (data.code == 1)
            {
                if (mGameUI != null)
                {
                    mGameUI.ReSetUI();
                    MJGameModel.Inst.ResetData();
                }
                HandleStartGameData(data.data);
            }
            else
                GameUtils.ShowErrorTips(data.code);
        });
    }

    /// <summary>
    /// 换桌
    /// </summary>
    public void SendChangeDesk()
    {
        NetProcess.SendRequest<CommonSendProto>(null, MJProtoMap.CMD_SendChangeDesk, (msg) =>
        {
            StartGameBackData data = msg.Read<StartGameBackData>();
            if (data.code == 1)
            {
                if (mGameUI != null)
                {
                    mGameUI.ReSetUI(true);
                    MJGameModel.Inst.ResetData();
                    MJGameModel.Inst.mState = eMJRoomStatus.GAMEOVER;
                    MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.GAMEOVER;//改变房间状态
                    SetTimeout.remove(ShowSmallSettle);
                    SetTimeout.remove(ChangHandsCardsShow);
                }
                HandleStartGameData(data.data);
            }
            else
            {
                GameUtils.ShowErrorTips(data.code);
                BackToClub();
            }
        });
    }

    /// <summary>
    /// 发送聊天
    /// </summary>
    /// <param name="req"></param>
    public void SendGameChat(SendReceiveGameChat req)
    {

        NetProcess.SendRequest<SendReceiveGameChat>(req, MJProtoMap.CMD_SendChat, (Msg) =>
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
    /// 获取玩家之间的距离
    /// </summary>
    public void SendGetPlayerDistances()
    {
        NetProcess.SendRequest<CommonSendProto>(null, MJProtoMap.CMD_GetGpsInfo, (msg) =>
        {
            SendGoldFlowerDistanceAck ack = msg.Read<SendGoldFlowerDistanceAck>();
            if (ack.code == 1)
            {
                GameDistanceWidget widget = BaseView.GetWidget<GameDistanceWidget>(AssetsPathDic.GameDistanceWidget, mBaseView.transform);
                if (ack.data.distances != null)
                {
                    for (int i = 0; i < ack.data.distances.Count; i++)
                    {
                        SendGoldFlowerDistanceInfo dis = ack.data.distances[i];
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


    public void LoadStartScene(System.Action filish = null)
    {
        if (filish != null)
        {
            filish();
        }

    }


    /// <summary>
    /// 获取房间信息
    /// </summary>
    public void SendJoinRoom(string roomId)
    {
        MJJoinRoomRequest req = new MJJoinRoomRequest();
        req.roomId = roomId;
        NetProcess.SendRequest<MJJoinRoomRequest>(req, MJProtoMap.CMD_GoIntoRoom, (msg) =>
        {
            StartGameBackData data = msg.Read<StartGameBackData>();
            if (data.code == 1)
            {
                //发送位置信息给服务器
                if (PlayerModel.Inst.address != null)
                {
                    NetProcess.SendRequest<SendAddrReq>(PlayerModel.Inst.address, MJProtoMap.CMD_SendAddress, (msgData) =>
                    {
                        SQDebug.Log("gps 返回");
                    }, false);
                }
                if (mGameUI != null)
                {
                    mGameUI.ReSetUI(true);
                    MJGameModel.Inst.ResetData();
                }
                HandleStartGameData(data.data);
            }
            else
                GameUtils.ShowErrorTips(data.code);
        });
    }

    /// <summary>
    /// 申请解散
    /// </summary>
    public void SendDissolution(CallBack call)
    {

    }

    /// <summary>
    /// 同意或拒绝解散
    /// </summary>
    /// <param name="state"></param>
    /// <param name="call"></param>
    public void SendIsAgreeDissoveRoom(int state, CallBack call)
    {

    }

    /// <summary>
    /// 离开房间
    /// </summary>
    public void SendLeaveRoom(CallBack call)
    {

    }

    /// <summary>
    /// 操作指令
    /// </summary>
    /// <param name="info"></param>
    public void SendInstructions(OptRequest info, CallBack call)
    {
        NetProcess.SendRequest<OptRequest>(info, MJProtoMap.Cmd_SendDealIns, (msg) =>
        {
            CommonRecieveProto data = msg.Read<CommonRecieveProto>();
            if (data.code == 1)
            {
                Debug.Log("操作指令");
                if (call != null)
                    call();
            }
            else if (data.code > 0)
            {
                GameUtils.ShowErrorTips(data.code);
            }
        }, false);
    }

    /// <summary>
    /// 聊天
    /// </summary>
    /// <param name="info"></param>
    public void SendChat(SendReceiveGameChat info)
    {
        NetProcess.SendRequest<SendReceiveGameChat>(info, ProtoIdMap.CMD_FinishTask, (msg) =>
        {
            CommonRecieveProto data = msg.Read<CommonRecieveProto>();
            //if (data.code == 1)
            {
                Debug.Log("聊天");
            }
        });
    }
    #endregion
    #region 接受服务器推送
    /// <summary>
    /// 有玩家加入房间
    /// </summary>
    /// <param name="msg"></param>
    private void OnPlayerJoinInRoom(MessageData msg)
    {
        PlayerInfoStruct data = msg.Read<PlayerInfoStruct>();
        MJGameModel.Inst.mRoomPlayers[data.seatId] = data;
        int count = MJGameModel.Inst.totalPlayerCount == 2 ? 4 : MJGameModel.Inst.totalPlayerCount;
        MJGameModel.Inst.mnewSeatToIndex[data.seatId] = SeatIdToIndex(MJGameModel.Inst.mMySeatId, data.seatId, count);
        MJGameModel.Inst.mSeatToDirectionIndex[data.seatId] = SeatIdToDirectionIndex(MJGameModel.Inst.mMySeatId, data.seatId, MJGameModel.Inst.totalPlayerCount);
        mGameUI.ServerPlayerJoinGame(data);
    }

    /// <summary>
    /// 小结算
    /// </summary>
    /// <param name="msg"></param>
    private void OnSettlement(MessageData msg)
    {
        litSemResponse data = msg.Read<litSemResponse>();
        CardsInfoStruct[] playerCardsDatas = MJGameModel.Inst.allPlayersCardsInfoStruct;
        bool isNoWinner = true;//是否是流局
        for (int i = 0; i < playerCardsDatas.Length; i++)
        {
            if (playerCardsDatas[i] != null && playerCardsDatas[i].huList != null && playerCardsDatas[i].huList.Count > 0)
            {
                isNoWinner = false;
                break;
            }
        }
        if (isNoWinner) //流局
        {
            mGameUI.ServerNoWinner();
        }
        #region 小结算数据处理
        MJGameSettlementInfo mSettlData = new MJGameSettlementInfo();
        MJGameModel.Inst.mLitSem = data;
        mSettlData.isEnd = true;
        mSettlData.isHu = !isNoWinner;
        mSettlData.settleContainer = new List<MJGameSettlementPlayerInfo>();
        for (int i = 0; i < data.litSemList.Count; i++)
        {
            MJGameSettlementPlayerInfo oneSettlementPlayerInfo = new MJGameSettlementPlayerInfo();
            int seatID = data.litSemList[i].seatId;
            PlayerInfoStruct onePData = MJGameModel.Inst.mRoomPlayers[seatID];//玩家信息
            CardsInfoStruct cdata = MJGameModel.Inst.allPlayersCardsInfoStruct[seatID];//牌信息
            oneSettlementPlayerInfo.seatId = seatID;
            oneSettlementPlayerInfo.userId = onePData.uId;
            oneSettlementPlayerInfo.nickName = onePData.nickName;
            oneSettlementPlayerInfo.score = data.litSemList[i].currScore;
            oneSettlementPlayerInfo.headUrl = onePData.headUrl;
            oneSettlementPlayerInfo.shoupai = data.litSemList[i].handList;
            oneSettlementPlayerInfo.peng = cdata.pengList;
            oneSettlementPlayerInfo.gang = cdata.gangList;
            oneSettlementPlayerInfo.huPai = cdata.huList;
            oneSettlementPlayerInfo.huOrder = data.litSemList[i].huOrder;
            oneSettlementPlayerInfo.huDes = data.litSemList[i].huIntro;
            mSettlData.settleContainer.Add(oneSettlementPlayerInfo);
        }
        #endregion

        MJGameModel.Inst.mSettlData = mSettlData;
        MJGameModel.Inst.mState = eMJRoomStatus.GAMEOVER;
        MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.GAMEOVER;//改变房间状态
        SetTimeout.add(1, ChangHandsCardsShow);

        SetTimeout.add(5, ShowSmallSettle);

        //清理离开游戏的玩家
        if (mGameUI != null)
            mGameUI.SetAllPlayerOutLine();
    }

    /// <summary>
    /// 翻转手牌
    /// </summary>
    private void ChangHandsCardsShow()
    {
        if (mGameUI != null)
        {
            MJGameModel.Inst.mState = eMJRoomStatus.GAMEOVER;
            MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.GAMEOVER;//改变房间状态
            WaitUpdataHandCards(MJGameModel.Inst.mLitSem.litSemList);
            mGameUI.GetChaJiaoData(MJGameModel.Inst.mLitSem.flowSemList);
        }
    }

    /// <summary>
    /// 显示小结算
    /// </summary>
    private void ShowSmallSettle()
    {
        MJGameModel.Inst.mState = eMJRoomStatus.GAMEOVER;
        MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.GAMEOVER;//改变房间状态
        if (mGameUI != null)
            WaitSecond(MJGameModel.Inst.mSettlData);
    }

    /// <summary>
    /// 大结算
    /// </summary>
    /// <param name="msg"></param>
    private void OnSettlementFinal(MessageData msg)
    {

        BigSemResponse data = msg.Read<BigSemResponse>();
        MJGameSettlementFinalInfo bigData = new MJGameSettlementFinalInfo();
        #region  大结算 数据处理
        PlayerInfoStruct[] playerDatas = MJGameModel.Inst.mRoomPlayers;
        bigData.isEnd = true;
        float bigWinScore = -1;
        bigData.totalContainr = new List<MJGameSettlementFinalPlayerInfo>();
        for (int i = 0; i < data.bigSemList.Count; i++)
        {
            BigSemStruct singleData = data.bigSemList[i];
            MJGameSettlementFinalPlayerInfo oneBigData = new MJGameSettlementFinalPlayerInfo();
            int index = MJGameModel.Inst.mnewSeatToIndex[singleData.seatId];
            PlayerInfoStruct onePData = playerDatas[singleData.seatId];

            oneBigData.seatId = singleData.seatId;
            oneBigData.userId = onePData.uId;
            oneBigData.headUrl = onePData.headUrl;
            oneBigData.nickName = onePData.nickName;

            oneBigData.score = singleData.score;
            if (oneBigData.score > bigWinScore)
            {
                bigWinScore = singleData.score;
            }
            oneBigData.winCount = singleData.winCount;
            bigData.totalContainr.Add(oneBigData);
        }
        for (int i = 0; i < bigData.totalContainr.Count; i++)
        {
            if (bigData.totalContainr[i].score == bigWinScore)
            {
                bigData.totalContainr[i].isBigwiner = true;
            }
            else
            {
                bigData.totalContainr[i].isBigwiner = false;
            }
        }
        #endregion
        mModel.mFinalSettlementInfo = bigData;
    }



    #endregion
    #region 协程

    public void WaitSecond(MJGameSettlementInfo data)
    {
        if (mGameUI == null || data == null)
            return;
        mModel.mStartGameData.roomInfo.roomState = eRoomState.READY;
        mGameUI.ServerSettlement(data);
        mGameUI.ReSetUI();
        mModel.ResetData();
        if (data.settleContainer != null)
        {
            MJGameSettlementPlayerInfo p;
            for (int i = 0; i < data.settleContainer.Count; i++)
            {
                p = data.settleContainer[i];
                //mModel.mPlayerdata[p.seatId - 1].mPlayerInfo.score += p.score;
                mGameUI.SetScore(p.seatId, MJGameModel.Inst.mRoomPlayers[p.seatId].gold, p.score);
            }
        }
        mModel.mCurPlayCount++;
        mGameUI.SetPlayNum(mModel.mCurPlayCount, true);//设置局数显示
        mGameUI.SetLeftCardNum(0, false);//隐藏剩余牌数量
    }
    /// <summary>
    /// 结算前的 手牌显示
    /// </summary>
    /// <param name="litSemList"></param>
    private void WaitUpdataHandCards(List<litSemStruct> litSemList)
    {
        for (int i = 0; i < litSemList.Count; i++)
        {
            int index = MJGameModel.Inst.mnewSeatToIndex[litSemList[i].seatId];
            mGameUI.GetPlayerHand(index, litSemList[i].handList, litSemList[i].seatId == MJGameModel.Inst.mMySeatId);
        }
    }

    #endregion
    #region 新增 代码/***************************************************/

    /// <summary>
    /// 开始游戏数据处理
    /// </summary>
    private void HandleStartGameData(MessageData msg)
    {
        #region 准备房间 玩家数据移植
        StartGameRespone data = msg.Read<StartGameRespone>();
        HandleStartGameData(data);

    }

    private void HandleStartGameData(StartGameRespone data, bool isStart = false)
    {
        MJGameModel.Inst.mStartGameData = data;
        SQDebug.Log("准备房间 玩家数据移植:" + Time.deltaTime);
        MJGameModel.Inst.ReadyCountDownTime = 0;//准备倒计时
        mModel.mPlayCount = data.roomInfo.maxGameCount;//最大局数
        mModel.mCurPlayCount = data.roomInfo.currGameCount;//当前局数
        MJGameModel.Inst.mRoomId = data.roomInfo.roomId;//房间号
        MJGameModel.Inst.mMySeatId = data.roomInfo.mySeatId;//我自己的座位号
        MJGameModel.Inst.mZhuangSeatId = data.startInfo.zhuangSeatId;//庄家座位号
        MJGameModel.Inst.TurnFixedTime = data.roomInfo.turnFixedTime;//操作固定时间
        MJGameModel.Inst.mRoomMgSeatId = 1; //房主座位号
        int totalPlayerCount = data.roomInfo.maxPlayer; //房间总共人数
        MJGameModel.Inst.totalPlayerCount = totalPlayerCount;
        totalPlayerCount = totalPlayerCount == 2 ? 4 : totalPlayerCount;
        MJGameModel.Inst.mnewSeatToIndex = new int[totalPlayerCount + 1]; //玩家座位号 数组     
        MJGameModel.Inst.mSeatToDirectionIndex = new int[5];//所有玩家座位号对应桌面东南西北高亮显示的index
        MJGameModel.Inst.mRoomPlayers = new PlayerInfoStruct[totalPlayerCount + 1]; //玩家 数据缓存数组
        foreach (var itemRoomPlayer in data.roomInfo.playerList)//玩家信息数据
        {
            MJGameModel.Inst.mRoomPlayers[itemRoomPlayer.seatId] = itemRoomPlayer;
            MJGameModel.Inst.mnewSeatToIndex[itemRoomPlayer.seatId] = SeatIdToIndex(MJGameModel.Inst.mMySeatId, itemRoomPlayer.seatId, totalPlayerCount);
            MJGameModel.Inst.mSeatToDirectionIndex[itemRoomPlayer.seatId] = SeatIdToDirectionIndex(MJGameModel.Inst.mMySeatId, itemRoomPlayer.seatId, data.roomInfo.maxPlayer);
        }
        #endregion
        MJGameModel.Inst.isGetStartGameData = true;
        MJGameModel.Inst.totalCardNum = data.startInfo.totalCardNum; //总共有多少张牌
        MJGameModel.Inst.leaveCardNum = data.startInfo.leaveCardNum; //剩余多少张牌
        MJGameModel.Inst.mCurInsSeatId = data.startInfo.currTurnSeatId;//当前指针 指的玩家座位号
        MJGameModel.Inst.isMyHit = data.startInfo.currTurnSeatId == MJGameModel.Inst.mMySeatId;//是否轮到我操作
        MJGameModel.Inst.allPlayersCardsInfoStruct = new CardsInfoStruct[totalPlayerCount + 1];
        //可操作列表
        if (data.roomInfo.optList != null)
        {
            for (int i = 0; i < data.roomInfo.optList.Count; i++)
            {
                if (data.roomInfo.optList[i].ins == eMJInstructionsType.HIT)
                    GetCanHuList(data.roomInfo.optList[i]);
            }
        }
        //手牌信息
        if (data.startInfo.cardsInfoList != null)
        {
            foreach (var item in data.startInfo.cardsInfoList)
            {
                MJGameModel.Inst.allPlayersCardsInfoStruct[item.seatId] = item;
                //找出我定缺的类型
                if (item.seatId == MJGameModel.Inst.mMySeatId)//我自己
                {
                    if (item.isFixedColor)//已经定缺
                    {
                        MJGameModel.Inst.SetMyFixedType(item.fixedType);
                        if (item.handList != null)//手牌排序
                            item.handList = MJGameModel.Inst.ChangList(item.handList, item.fixedType);
                    }
                    else
                        MJGameModel.Inst.SetMyFixedType(0);
                }

                if (item.handList != null)
                {
                    foreach (var VARIABLE in item.handList)
                    {
                        MJGameModel.Inst.mySelfCards.Add(VARIABLE);
                    }
                    if (item.isHasCurrCard)
                    {
                        MJGameModel.Inst.mySelfCards.Add(item.currCard);
                    }

                    foreach (var oneItem in MJGameModel.Inst.mySelfCards)
                    {
                        if (oneItem < 10)
                            MJGameModel.Inst.handCardsTiao.Add(oneItem);
                        else if (oneItem < 20)
                            MJGameModel.Inst.handCardsTong.Add(oneItem);
                        else if (oneItem < 30)
                            MJGameModel.Inst.handCardsWan.Add(oneItem);
                    }
                }
            }
        }
        //MJGameModel.Inst.UpdataModelDada(data);

        if (isStart && !MJGameModel.Inst.IsContainsDingque && !MJGameModel.Inst.IsContainsChange)//如果开始发牌且没有定缺也没有换三张，房间状态改为开始
        {
            MJGameModel.Inst.mState = eMJRoomStatus.STARTE;
            MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.START;//改变房间状态
        };

        if (mGameUI != null)
        {
            List<string> view = new List<string>();
            view.Add("GameUI");
            BaseView.CloseAllViewBut(view);
            mGameUI.GetGameStartFormCtr();

        }
        else
        {
            SQSceneLoader.It.LoadScene("mahjong_scenes", () =>
            {
                mGameUI = OpenWindow() as MJGameUI;
                List<string> view = new List<string>();
                view.Add("GameUI");
                BaseView.CloseAllViewBut(view);
                mGameUI.GetGameStartFormCtr();
            });
        }
    }



    /// <summary>
    /// 执行在换三张时推送的命令
    /// </summary>
    public void ExcuteOptInChange()
    {
        if (MJGameModel.Inst.OptCachInChange != null)
            GetOnOptListACK(MJGameModel.Inst.OptCachInChange);
        MJGameModel.Inst.OptCachInChange = null;
    }

    /// <summary>
    /// 同步玩家可以操作什么指令
    /// </summary>
    /// <param name="msg"></param>
    private void GetOnOptListACK(MessageData msg)
    {
        MJInstructionsProto data = msg.Read<MJInstructionsProto>();
        SQDebug.Log("我收到同步操作啦" + Time.deltaTime);
        //mGameUI.GetInstructionsACK(data);
        MJGameModel.Inst.mSycOptListResponse = data;
        if (!MJGameModel.Inst.isFirstGetStartGameData)
        {
            //当前状态为换三张且没有缓存操作，且该操作不是换三张
            if (MJGameModel.Inst.mStartGameData.roomInfo.roomState == eRoomState.CHANGETHREE && MJGameModel.Inst.OptCachInChange == null && data.optList[0].ins != eMJInstructionsType.CHANGETHREE)
            {
                MJGameModel.Inst.OptCachInChange = msg;
                return;
            }
            ChoiseRoomState(data);
        }
    }

    public void ChoiseRoomState(MJInstructionsProto data)
    {

        switch (data.optList[0].ins)
        {
            case eMJInstructionsType.CHANGETHREE: //换3张
                MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.CHANGETHREE;
                SQDebug.Log("房间状态为:====>>>换3张");
                mGameUI.DelayRun(1f, () => { mGameUI.ChangeThreeUI(data); });
                mGameUI.SetLeftTime(MJGameModel.Inst.TurnFixedTime, true);

                break;
            case eMJInstructionsType.FIXEDCOLOR://定缺
                if (MJGameModel.Inst.mStartGameData.roomInfo.roomState == eRoomState.START)
                    break;
                MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.FIXEDCOLOR;
                SQDebug.Log("房间状态为:====>>>定缺");
                mGameUI.DelayRun(0.1f, () => { mGameUI.ChangeFixeDcolorUI(data); });
                mGameUI.SetLeftTime(MJGameModel.Inst.TurnFixedTime, true);
                break;
            default://
                MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.START;//改变房间状态
                SQDebug.Log("房间状态为:====>>>游戏中");
                bool isHasDataBefore = (MJGameModel.Inst.hasCanHuListCards != null && MJGameModel.Inst.hasCanHuListCards.Count != 0);//处理数据之前是否有数据
                List<OptItemStruct> optList = MergeInsData(data.optList);
                bool isHasDataBehind = (MJGameModel.Inst.hasCanHuListCards != null && MJGameModel.Inst.hasCanHuListCards.Count != 0);//处理数据之后是否有数据
                mGameUI.ServerShowInstructions(optList, isHasDataBefore != isHasDataBehind);
                break;
        }
    }

    /// <summary>
    /// 颗操作列表的    杠   合并
    /// </summary>
    /// <param name="optList"></param>
    /// <returns></returns>
    private List<OptItemStruct> MergeInsData(List<OptItemStruct> optList)
    {
        if (MJGameModel.Inst.hitCardCanHuList != null)
            MJGameModel.Inst.hitCardCanHuList.Clear();
        if (MJGameModel.Inst.hasCanHuListCards != null)
            MJGameModel.Inst.hasCanHuListCards.Clear();
        if (optList == null)
            return null;
        OptItemStruct gangOpts = new OptItemStruct();
        bool ishasGamgOpt = false;
        for (int i = optList.Count - 1; i >= 0; i--)
        {
            if (optList[i].ins == eMJInstructionsType.GANG)
            {
                if (ishasGamgOpt)
                {
                    if (gangOpts.cards == null)
                        gangOpts.cards = new List<int>();
                    gangOpts.cards.Add(optList[i].cards[0]);
                    optList.Remove(optList[i]);
                    continue;
                }
                ishasGamgOpt = true;
                gangOpts = optList[i];
            }
            if (optList[i].ins == eMJInstructionsType.HIT)
                GetCanHuList(optList[i]);
        }
        if (gangOpts.ins == eMJInstructionsType.GANG)
            optList.Add(gangOpts);
        return optList;
    }



    /// <summary>
    /// 发送 操作的指令请求
    /// </summary>
    /// <param name="data"></param>
    public void SendOptRequest(OptRequest req, System.Action finish = null, bool show = true)
    {
        NetProcess.SendRequest<OptRequest>(req, MJProtoMap.Cmd_SendDealIns, (msg) =>
        {
            CommonRecieveProto data = msg.Read<CommonRecieveProto>();
            if (data.code == 107)//金币不足，返回俱乐部界面
            {
                GameUtils.ShowErrorTips(data.code);
                Global.Inst.GetController<MJGameController>().BackToClub();
            }
        }, show);
        if (finish != null)
        {
            finish();
        }
        if (req.ins == eMJInstructionsType.HIT && mGameUI.mSelfPlayer != null)
        {
            mGameUI.mSelfPlayer.ShowInstructions(null);
        }
    }

    /// <summary>
    /// 换3张确定后的数据处理
    /// </summary>
    /// <param name="data"></param>
    public void UpdateHandCardsAfterChangeThree(MJoptInfoData data)
    {
        foreach (var item in data.cards)
        {
            if (item < 10)
                MJGameModel.Inst.handCardsTiao.Remove(item);
            else if (item < 20)
                MJGameModel.Inst.handCardsTong.Remove(item);
            else if (item < 30)
                MJGameModel.Inst.handCardsWan.Remove(item);
            MJGameModel.Inst.mySelfCards.Remove(item);
        }
        MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].handList = MJGameModel.Inst.mySelfCards;
        MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].currCard = 0;
    }

    /// <summary>
    ///   所有玩家 换3张成功后的数据处理
    /// </summary>
    /// <param name="data"></param>
    public void UpdateHandCardsAfterAllChangeThree(MJoptInfoData data)
    {
        foreach (var item in data.cards)
        {
            if (item < 10)
                MJGameModel.Inst.handCardsTiao.Add(item);
            else if (item < 20)
                MJGameModel.Inst.handCardsTong.Add(item);
            else if (item < 30)
                MJGameModel.Inst.handCardsWan.Add(item);
            MJGameModel.Inst.mySelfCards.Add(item);
        }
        MJGameModel.Inst.mySelfCards.Sort();
        List<int> allHand = MJGameModel.Inst.mySelfCards;
        if (MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].isHasCurrCard)
        {
            MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId]
                .currCard = allHand[allHand.Count - 1];
            MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].currCard = allHand[allHand.Count - 1];
            allHand.RemoveAt(allHand.Count - 1);

        }
        MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].handList = MJGameModel.Inst.mySelfCards;
    }
    /// <summary>
    /// 定缺成功    数据处理
    /// </summary>
    public void UpdateHandCardsAfterFix(MJoptInfoData data)
    {
        MJGameModel.Inst.SetMyFixedType(data.type);
        CardsInfoStruct mMJmyInfo = MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId];
        List<int> newHandCards = mMJmyInfo.handList;
        if (mMJmyInfo != null && mMJmyInfo.currCard != 0)
            newHandCards.Add(mMJmyInfo.currCard);
        mMJmyInfo.fixedType = data.type;
        newHandCards = MJGameModel.Inst.ChangList(newHandCards, data.type);

        for (int i = 0; i < newHandCards.Count; i++)
        {
            if (newHandCards[i] > MJGameModel.Inst.eFixe[0] && newHandCards[i] < MJGameModel.Inst.eFixe[1])
            {
                MJGameModel.Inst.isHasFixeCard = true;
                break;
            }
            MJGameModel.Inst.isHasFixeCard = false;
        }

        if (MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].isHasCurrCard)
        {
            MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId]
                .currCard = newHandCards[newHandCards.Count - 1];
            MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].currCard = newHandCards[newHandCards.Count - 1];
            newHandCards.RemoveAt(newHandCards.Count - 1);
        }
        MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId]
                    .handList = newHandCards;
    }


    /// <summary>
    /// 同步谁操作了什么指令
    /// </summary>
    /// <param name="msg"></param>
    private void GetInstructionsACK(MessageData msg)
    {
        MJoptInfoData data = msg.Read<MJoptInfoData>();
        if (MJGameModel.Inst.msgQueue.Count > 0 || mGameUI == null)
        {
            SQDebug.Log("消息加入队列!!!");
            MJGameModel.Inst.msgQueue.Enqueue(data);
            return;
        }
        ChoiseIns(data);
    }
    public void ChoiseIns(MJoptInfoData data)
    {
        switch (data.ins)
        {
            case eMJInstructionsType.READY: //准备
                MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.READY;
                mGameUI.ServerPlayerPre(data.seatId);
                break;
            case eMJInstructionsType.CHANGETHREE: //换3张 确定
                UpdateHandCardsAfterChangeThree(data);//数据处理
                mGameUI.SureChangeThreeUI(data);
                break;
            case eMJInstructionsType.ALLCHANGE:// 换3张  所有玩家都确定
                UpdateHandCardsAfterAllChangeThree(data);//数据处理
                mGameUI.PlayChangeThreeAnim(data);
                if (!MJGameModel.Inst.IsContainsDingque)//如果没有定缺，房间状态改为开始
                {
                    MJGameModel.Inst.mState = eMJRoomStatus.STARTE;
                    MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.START;//改变房间状态
                }
                break;
            case eMJInstructionsType.FIXEDCOLOR:// 定缺成功
                UpdateHandCardsAfterFix(data);//数据处理
                mGameUI.SureFixeDcolor(data);
                break;
            case eMJInstructionsType.ALLFIXED:// 所有人完成 定缺
                mGameUI.SureAllFixeDcolor(data);
                MJGameModel.Inst.mState = eMJRoomStatus.STARTE;
                MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.START;//改变房间状态
                break;
            case eMJInstructionsType.SCORE:// 分数
                OnPlayerScoreIns(data);
                break;
            case eMJInstructionsType.CHATING:// 查胡
                mGameUI.SetChaHu(data.canHuList);
                break;
            case eMJInstructionsType.YPDX:// 一炮的多项
                List<MJoptInfoData> dataList = new List<MJoptInfoData>();
                mGameUI.SetYPDXEffUI(data);//一炮多响特效显示
                dataList = YPDXData(data);
                for (int i = 0; i < dataList.Count; i++)
                {
                    OnPlayerInstructions(dataList[i]);
                }
                break;
            case eMJInstructionsType.EXITROOM://退出房间，在线状态
                MJGameModel.Inst.mRoomPlayers[data.seatId].onLineType = eMJOnlineState.leave.GetHashCode();
                if (mGameUI != null)
                    mGameUI.ServerOnlineState(data.seatId, data.type);
                break;
            case eMJInstructionsType.TG://托管
                if (mGameUI != null)
                    mGameUI.ServerPlayerTrustship(data.seatId, (eMJTrusteeshipType)data.type);
                break;
            default:
                //mContrl.OnPlayerInstructCards(data);
                MJGameModel.Inst.mState = eMJRoomStatus.STARTE;
                MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.START;//改变房间状态
                OnPlayerInstructions(data);
                break;
        }
        PlaySound(data);

    }
    /// <summary>
    /// 获取 打什么胡什么提示
    /// </summary>
    public void GetCanHuList(OptItemStruct data)
    {
        if (MJGameModel.Inst.hitCardCanHuList != null)
            MJGameModel.Inst.hitCardCanHuList.Clear();
        MJGameModel.Inst.hitCardCanHuList = data.hitCardCanHuList;
        if (MJGameModel.Inst.hasCanHuListCards == null)
            MJGameModel.Inst.hasCanHuListCards = new List<int>();
        MJGameModel.Inst.hasCanHuListCards.Clear();
        if (data != null && data.hitCardCanHuList != null)
        {
            for (int i = 0; i < data.hitCardCanHuList.Count; i++)
            {
                MJGameModel.Inst.hasCanHuListCards.Add(data.hitCardCanHuList[i].hitCard);
            }
        }
        //MJGameModel.Inst.canHuList = data.canHuList;

    }
    /// <summary>
    /// 一炮多响 数据处理
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public List<MJoptInfoData> YPDXData(MJoptInfoData data)
    {
        List<MJoptInfoData> dataList = new List<MJoptInfoData>();
        List<int> newSeatIdList = new List<int>();
        newSeatIdList = data.seatIdList;
        for (int i = 0; i < newSeatIdList.Count; i++)
        {

            MJoptInfoData newData = new MJoptInfoData();
            newData.ins = eMJInstructionsType.YPDX;
            newData.otherSeatId = data.otherSeatId;
            newData.seatId = newSeatIdList[i];
            newData.subType = data.subType;
            newData.thrType = data.thrType;
            newData.type = data.type;
            newData.cards = new List<int>();
            newData.cards = data.cards;
            if (i == 0)
                newData.huGl = false;
            else
                newData.huGl = true;
            dataList.Add(newData);
        }
        return dataList;
    }

    /// <summary>
    /// 分数 变化
    /// </summary>
    /// <param name="data"></param>
    private void OnPlayerScoreIns(MJoptInfoData data)
    {
        List<ScoreStruct> scors = new List<ScoreStruct>();
        scors = data.scoreList;
        foreach (var item in scors)
        {
            MJGameModel.Inst.mRoomPlayers[item.seatId].gold += item.score;
            mGameUI.SetScore(item.seatId, MJGameModel.Inst.mRoomPlayers[item.seatId].gold, item.score, data.type);
        }
    }

    /// <summary>
    /// 碰  打   杠  摸  胡  操作
    /// </summary>
    /// <param name="data"></param>
    private void OnPlayerInstructions(MJoptInfoData data)
    {
        int index = data.seatId - 1;
        mModel.mCurInsSeatId = data.seatId;//当前操作的玩家座位号
        mModel.allPlayersCardsInfoStruct[data.seatId].InstructionsCard(data, ref mModel.mLastOutCard);
        if (data.otherSeatId != data.seatId && data.otherSeatId != 0)//被动操作的人数据
        {
            mModel.allPlayersCardsInfoStruct[data.otherSeatId].PassiveInstructionsCard(data, ref mModel.mLastOutCard);
        }

        mGameUI.ServerWhoInstructions(data);

    }
    /// <summary>
    /// 获取 胡牌提示
    /// </summary>
    public void GetHuPromptCard(List<CanHuStruct> canHuList, Vector3 vec3)
    {
        mGameUI.SetHuPromptCard(canHuList, vec3);

    }

    /// <summary>
    /// 座位号转 index 自己始终坐的是index 为1的位置
    /// </summary>
    /// <param name="seatId"> 座位号</param>
    /// <param name="maxSeat"> 座位总数</param>
    /// <returns></returns>
    public int SeatIdToIndex(int mySelfSeatId, int seatId, int maxSeat)
    {
        int addNum = mySelfSeatId - 1;
        if (mySelfSeatId == seatId)
        {
            return 1;
        }
        if (seatId < mySelfSeatId)
        {
            return seatId + maxSeat - addNum == maxSeat ? 0 : seatId + maxSeat - addNum;
        }
        else
        {
            return seatId - addNum >= maxSeat ? seatId - addNum - maxSeat : seatId - addNum;
        }
    }

    /// <summary>
    /// 所有玩家座位号对应桌面东南西北高亮显示的index
    /// </summary>
    /// <param name="myselftId">我自己的座位号</param>
    /// <param name="seatId">玩家座位号</param>
    /// <param name="maxSeat">最大人数，真实人数</param>
    /// <returns></returns>
    public int SeatIdToDirectionIndex(int myselftId, int seatId, int maxSeat)
    {
        if (seatId == myselftId)
            return seatId - 1;
        if (maxSeat == 4)
            return seatId - 1;
        else if (maxSeat == 2)
        {
            return seatId - 1;
        }
        else if (maxSeat == 3)
        {
            int d = seatId - myselftId;//其他玩家座位号和我的差值
            if (d == 2 || d == -2)
                return 3;
            else
                return seatId - 1;
        }
        return seatId - 1;
    }
    #region
    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="data"></param>
    public void PlaySound(MJoptInfoData data)
    {
        switch (data.ins)
        {
            case eMJInstructionsType.PENG:
            case eMJInstructionsType.GANG:
            case eMJInstructionsType.HU:
                PlayInsVoice(data);
                break;
            case eMJInstructionsType.HIT:
                SoundProcess.PlaySound(VOICE_HITCARD);//播放打牌的声音
                PlayCardVoice(data.cards[0], data.seatId);
                break;
            case eMJInstructionsType.MO://摸牌
                //SoundProcess.PlaySound(VOICE_GETCARD);//播放摸牌的声音
                break;
        }
    }

    /// <summary>
    /// 播放牌声音
    /// </summary>
    /// <param name="cardId">牌id</param>
    /// <param name="seatId">座位号</param>
    private void PlayCardVoice(int cardId, int seatId)
    {
        int sex = MJGameModel.Inst.mRoomPlayers[seatId].sex;//性别
        string card = cardId.ToString();
        ConfigDada con = ConfigManager.GetConfigs<MJGameCardVoiceConfig>().Find(o => o.conIndex.Equals(card));
        if (con != null)
        {
            MJGameCardVoiceConfig voiceCon = con as MJGameCardVoiceConfig;
            if (voiceCon.voice == null || voiceCon.voice.Length == 0)
                return;
            int index = Random.Range(0, voiceCon.voice.Length);//随机选一个声音
            if (sex == 1)//男
                SoundProcess.PlaySound(VOICE_MJ_MAN + voiceCon.voice[index]);
            else//女
                SoundProcess.PlaySound(VOICE_MJ_WOMAN + voiceCon.voice[index]);
        }
    }
    /// <summary>
    /// 播放操作声音
    /// </summary>
    /// <param name="data"></param>
    private void PlayInsVoice(MJoptInfoData data)
    {
        if (data.ins == eMJInstructionsType.YPDX)//一炮多响
        {
            if (data.seatIdList != null)
            {
                string ins = data.ins.GetHashCode().ToString();
                for (int i = 0; i < data.seatIdList.Count; i++)
                {
                    int sex = MJGameModel.Inst.mRoomPlayers[data.seatIdList[i]].sex;//性别
                    ConfigDada con = ConfigManager.GetConfigs<MJGameInsVoiceConfig>().Find(o => o.conIndex.Equals(ins));
                    MJGameInsVoiceConfig voiceCon = con as MJGameInsVoiceConfig;
                    if (voiceCon == null || voiceCon.voice == null || voiceCon.voice.Length == 0)
                        return;
                    int index = Random.Range(0, voiceCon.voice.Length);//随机选一个声音
                    if (sex == 1)//男
                        SoundProcess.PlaySound(VOICE_INS_MAN + voiceCon.voice[index]);
                    else//女
                        SoundProcess.PlaySound(VOICE_INS_WOMAN + voiceCon.voice[index]);
                }
            }
        }
        else if (data.ins == eMJInstructionsType.PENG || data.ins == eMJInstructionsType.GANG)//碰和杠
        {
            string ins = data.ins.GetHashCode().ToString();
            int sex = MJGameModel.Inst.mRoomPlayers[data.seatId].sex;//性别
            ConfigDada con = ConfigManager.GetConfigs<MJGameInsVoiceConfig>().Find(o => o.conIndex.Equals(ins));
            MJGameInsVoiceConfig voiceCon = con as MJGameInsVoiceConfig;
            if (voiceCon == null || voiceCon.voice == null || voiceCon.voice.Length == 0)
                return;
            int index = Random.Range(0, voiceCon.voice.Length);//随机选一个声音
            if (sex == 1)//男
                SoundProcess.PlaySound(VOICE_INS_MAN + voiceCon.voice[index]);
            else//女
                SoundProcess.PlaySound(VOICE_INS_WOMAN + voiceCon.voice[index]);
            //if (data.ins == eMJInstructionsType.GANG)//杠要播放刮风和下雨的声音
            //SoundProcess.PlaySound(VOICE_INS_OTHER + voiceCon.subTypeVoice[data.type]);
        }
        else if (data.ins == eMJInstructionsType.HU)
        {
            string ins = data.ins.GetHashCode().ToString();
            int sex = MJGameModel.Inst.mRoomPlayers[data.seatId].sex;//性别
            ConfigDada con = ConfigManager.GetConfigs<MJGameInsVoiceConfig>().Find(o => o.conIndex.Equals(ins));
            MJGameInsVoiceConfig voiceCon = con as MJGameInsVoiceConfig;
            if (voiceCon == null || voiceCon.voice == null || voiceCon.voice.Length == 0)
                return;
            string str = sex == 1 ? VOICE_INS_MAN : VOICE_INS_WOMAN;
            if ((eHuThrType)data.thrType != eHuThrType.NONE && voiceCon.thrTypeVoice != null && voiceCon.thrTypeVoice.Length > data.thrType)//第三级类型
            {
                SoundProcess.PlaySound(str + voiceCon.thrTypeVoice[data.thrType]);
                return;
            }
            if ((eHuSubType)data.subType != eHuSubType.NONE && voiceCon.subTypeVoice != null && voiceCon.subTypeVoice.Length > data.subType)//第二级类型
            {
                SoundProcess.PlaySound(str + voiceCon.subTypeVoice[data.subType]);
                return;
            }
            if ((eHuType)data.type != eHuType.NONE && voiceCon.voice != null && voiceCon.voice.Length > data.type)//第一级类型
            {
                SoundProcess.PlaySound(str + voiceCon.voice[data.type]);
                return;
            }
        }
    }

    public void PlayChatVoice(int index, int sex)
    {
        //if (ConfigManager.Create().mChatConfig.data[0].voice.Length > index)
        {
            int id = 1; //配置表里面取//ConfigManager.Create().mChatConfig.data[0].voice[index];
            if (sex == 1)//男
                SoundProcess.PlaySound(VOICE_CHAT_MAN + id);
            else
                SoundProcess.PlaySound(VOICE_CHAT_WOMAN + id);
        }
    }
    #endregion
    /// <summary>
    /// 推送聊天
    /// </summary>
    /// <param name="msg"></param>
    private void NetOnGameTalk(MessageData msg)
    {
        SendReceiveGameChat data = msg.Read<SendReceiveGameChat>();
        if (mGameUI != null)
            mGameUI.ServerGetChat(data);
    }
    #endregion
    /// <summary>
    /// 准备倒计时
    /// </summary>
    /// <param name="msg"></param>
    private void OnReadyCountDown(MessageData msg)
    {
        MJReadyCountDownRespons data = msg.Read<MJReadyCountDownRespons>();
        MJGameModel.Inst.ReadyCountDownTime = data.leaveTime + Time.realtimeSinceStartup;//准备倒计时
        if (mGameUI != null)
            mGameUI.ServerReadyCountDown(data.leaveTime);
    }


    /// <summary>
    /// 返回俱乐部界面
    /// </summary>
    public void BackToClub()
    {
        Global.Inst.GetController<GamePatternController>().SendGetRoomList((patternView) =>
        {
            SQSceneLoader.It.LoadScene("HALL", () =>
            {
                Global.Inst.GetController<GamePatternController>().OpenWindow();
                CloseWindow();
            });

            if (MJGameModel.Inst.mStartGameData.roomInfo.mode == "m4")
            {
                patternView.BackToGlodWidget();
            }

        });
    }
}
