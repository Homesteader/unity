using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MJGameBackController : BaseController
{

    public MJGameBackModel mModel;
    private MJGameBackUI mGameUI;
    private MJGameController mjController;

    public MJGameBackController() : base("MJGameBackUI", "Windows/Majiang/GameBackUI")
    {
        SetModel<MJGameBackModel>();
        mModel = MJGameBackModel.Inst;
    }


    public void ShowRecord()
    {
        SQSceneLoader.It.LoadScene("PlayBackScene", () =>
        {
            List<string> views = new List<string>();
            views.Add("MJGameBackUI");
            views.Add("NetLoadingView");
            views.Add("SceneLoadingView");

            StartGameRespone data = new StartGameRespone();
            data.roomInfo = mModel.CurRecordDetailData.roomInfo;
            data.startInfo = mModel.CurRecordDetailData.startInfo;
            HandleStartGameData(data);//初始化数据
            BaseView.CloseAllViewBut(views);
            mGameUI = OpenWindow() as MJGameBackUI;
            mGameUI.InfoAllPlayerData(data);
        });
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
        });
    }

    /// <summary>
    /// 下一局
    /// </summary>
    /// <param name="call"></param>
    public void GetNextRecord(CallBack<MJRecordData> call)
    {
        mModel.mCurIndex++;
        if (mModel.mCurIndex > mModel.mTottleCount)
            mModel.mCurIndex = 0;
        if (mModel.RecordAllData == null || mModel.RecordAllData.Count <= mModel.mCurIndex)
            return;
        GetRecord(mModel.RecordAllData[mModel.mCurIndex].fileUrl2, call);
    }

    /// <summary>
    /// 上一局
    /// </summary>
    /// <param name="call"></param>
    public void GetLastRecord(CallBack<MJRecordData> call)
    {
        mModel.mCurIndex--;
        if (mModel.mCurIndex <= 0)
            mModel.mCurIndex = mModel.mTottleCount - 1;
        if (mModel.RecordAllData == null || mModel.mCurIndex < 0 || mModel.RecordAllData.Count <= mModel.mCurIndex)
            return;
        GetRecord(mModel.RecordAllData[mModel.mCurIndex].fileUrl2, call);
    }

    /// <summary>
    /// 获取回放信息
    /// </summary>
    /// <param name="index"></param>
    /// <param name="call"></param>
    public void GetRecord(string path, CallBack<MJRecordData> call)
    {

        if (mModel.RecordDetailsDic.ContainsKey(path))
        {
            if (call != null)
            {
                mModel.CurRecordDetailData = DeserializeData<MJRecordData>(mModel.RecordDetailsDic[path]);
                call(mModel.CurRecordDetailData);
            }
        }
        else
        {
            //path = "file://E:/Project/ZhenzhuMJ/record.txt";
            Assets.LoadTxtFileCallBytes(path, (str) =>
            {
                if (str == null)
                    Global.Inst.GetController<CommonTipsController>().ShowTips("获取回放信息失败");
                else
                {
                    MJRecordData data = DeserializeData<MJRecordData>(str);
                    string s = Json.Serializer<MJRecordData>(data);
                    SQDebug.Log("回放数据：" + s);
                    mModel.RecordDetailsDic[path] = str;
                    mModel.CurRecordDetailData = data;
                    if (call != null)
                        call(data);
                }
            });
        }
    }


    /// <summary>
    /// 获取战绩
    /// </summary>
    public void GetAllDetailData(CallBack call)
    {
        NetProcess.SendRequest<CommonSendProto>(null, MJProtoMap.CDM_GetAllDetailInfo, (msg) =>
        {
            MJRecordDetailInfo data = msg.Read<MJRecordDetailInfo>();
            MJGameBackModel.Inst.RecordAllData = data == null ? null : data.recodeList;
            if (MJGameBackModel.Inst.RecordAllData!=null) {
                MJGameBackModel.Inst.RecordAllData.Reverse();
            }
            if (call != null)
                call();
        });
    }

    private T DeserializeData<T>(byte[] data)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                //将流的位置归0
                ms.Position = 0;
                //使用工具反序列化对象
                T result = ProtoBuf.Serializer.Deserialize<T>(ms);
                return result;
            }
        }
        catch (System.Exception e)
        {
            SQDebug.Log("反序列化失败  " + e.ToString());
            return default(T);
        }
    }


    private void HandleStartGameData(StartGameRespone data)
    {
        mjController = Global.Inst.GetController<MJGameController>();
        #region 准备房间 玩家数据移植
        //StartGameRespone data = msg.Read<StartGameRespone>();

        MJGameModel.Inst.mStartGameData = data;
        SQDebug.Log("准备房间 玩家数据移植:" + Time.deltaTime);
        MJGameModel.Inst.mPlayCount = data.roomInfo.maxGameCount;//最大局数
        MJGameModel.Inst.mCurPlayCount = data.roomInfo.currGameCount;//当前局数
        MJGameModel.Inst.mRoomId = data.roomInfo.roomId;//房间号
        MJGameModel.Inst.mZhuangSeatId = data.startInfo.zhuangSeatId;
        foreach (var itemRoomPlayer in data.roomInfo.playerList)//玩家信息数据
        {
            if (itemRoomPlayer.uId == PlayerModel.Inst.UserInfo.userId)
            {
                MJGameModel.Inst.mMySeatId = itemRoomPlayer.seatId;
                break;
            }
        }
        //MJGameModel.Inst.mMySeatId = data.roomInfo.mySeatId;//我自己的座位号
        MJGameModel.Inst.mRoomMgSeatId = 1; //房主座位号
        int totalPlayerCount = data.roomInfo.maxPlayer; //房间总共人数
        MJGameModel.Inst.totalPlayerCount = totalPlayerCount;
        totalPlayerCount = totalPlayerCount == 2 ? 4 : totalPlayerCount;
        MJGameModel.Inst.AllPlayerEfixe = new List<int[]>();//所有玩家定缺区间
        for (int i = 0; i < 5; i++)
            MJGameModel.Inst.AllPlayerEfixe.Add(new int[2]);
        MJGameModel.Inst.mnewSeatToIndex = new int[totalPlayerCount + 1]; //玩家座位号 数组     
        MJGameModel.Inst.mSeatToDirectionIndex = new int[5];//所有玩家座位号对应桌面东南西北高亮显示的index
        MJGameModel.Inst.mRoomPlayers = new PlayerInfoStruct[totalPlayerCount + 1]; //玩家 数据缓存数组
        foreach (var itemRoomPlayer in data.roomInfo.playerList)//玩家信息数据
        {
            MJGameModel.Inst.mRoomPlayers[itemRoomPlayer.seatId] = itemRoomPlayer;
            MJGameModel.Inst.mnewSeatToIndex[itemRoomPlayer.seatId] = mjController.SeatIdToIndex(MJGameModel.Inst.mMySeatId, itemRoomPlayer.seatId, totalPlayerCount);
            MJGameModel.Inst.mSeatToDirectionIndex[itemRoomPlayer.seatId] = mjController.SeatIdToDirectionIndex(MJGameModel.Inst.mMySeatId, itemRoomPlayer.seatId, data.roomInfo.maxPlayer);
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
                    mjController.GetCanHuList(data.roomInfo.optList[i]);
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
    }

    /// <summary>
    /// 执行指令
    /// </summary>
    /// <param name="data"></param>
    public void ChoiseIns(MJoptInfoData data)
    {
        switch (data.ins)
        {
            case eMJInstructionsType.CHANGETHREE: //换3张 确定
                //UpdateHandCardsAfterChangeThree(data);//数据处理
                //mGameUI.SureChangeThreeUI(data);
                break;
            case eMJInstructionsType.ALLCHANGE:// 换3张  所有玩家都确定
                UpdateHandCardsAfterAllChangeThree(data);//数据处理

                break;
            case eMJInstructionsType.FIXEDCOLOR:// 定缺成功
                mjController.UpdateHandCardsAfterFix(data);//数据处理
                mGameUI.SureFixeDcolor(data);
                break;
            case eMJInstructionsType.ALLFIXED:// 所有人完成 定缺
                SetOtherFixColor(data);
                mGameUI.SureAllFixeDcolor(data);
                MJGameModel.Inst.mState = eMJRoomStatus.STARTE;
                MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.START;//改变房间状态
                break;
            case eMJInstructionsType.SCORE:// 分数
                OnPlayerScoreIns(data);
                break;
            case eMJInstructionsType.YPDX:// 一炮的多项
                List<MJoptInfoData> dataList = new List<MJoptInfoData>();
                dataList = mjController.YPDXData(data);
                for (int i = 0; i < dataList.Count; i++)
                {
                    OnPlayerInstructions(dataList[i]);
                }
                break;
            default:
                MJGameModel.Inst.mState = eMJRoomStatus.STARTE;
                MJGameModel.Inst.mStartGameData.roomInfo.roomState = eRoomState.START;//改变房间状态
                OnPlayerInstructions(data);
                break;
        }
        mjController.PlaySound(data);

    }

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
        MJGameModel.Inst.mCurInsSeatId = data.seatId;//当前操作的玩家座位号
        MJGameModel.Inst.allPlayersCardsInfoStruct[data.seatId].InstructionsCard(data, ref MJGameModel.Inst.mLastOutCard);
        if (data.otherSeatId != data.seatId && data.otherSeatId != 0)//被动操作的人数据
        {
            MJGameModel.Inst.allPlayersCardsInfoStruct[data.otherSeatId].PassiveInstructionsCard(data, ref MJGameModel.Inst.mLastOutCard);
        }

        mGameUI.ServerWhoInstructions(data);

    }



    /// <summary>
    ///   所有玩家 换3张成功后的数据处理
    /// </summary>
    /// <param name="data"></param>
    private void UpdateHandCardsAfterAllChangeThree(MJoptInfoData data)
    {
        if (data.changeList == null)
            return;
        //拿出去三张
        int seatid;
        int card;
        for (int i = 0; i < data.changeList.Length; i++)
        {
            seatid = data.changeList[i].seatId;
            for (int j = 0; j < data.changeList[i].outCards.Count; j++)
            {
                card = data.changeList[i].outCards[j];
                if (MJGameModel.Inst.allPlayersCardsInfoStruct[seatid].currCard > 0)
                {
                    int ccard = MJGameModel.Inst.allPlayersCardsInfoStruct[seatid].currCard;
                    MJGameModel.Inst.allPlayersCardsInfoStruct[seatid].handList.Add(ccard);
                    MJGameModel.Inst.allPlayersCardsInfoStruct[seatid].currCard = 0;
                }
                MJGameModel.Inst.allPlayersCardsInfoStruct[seatid].handList.Remove(card);//从手牌中移除交换的牌
            }

        }
        ChangeThreeOver(data);
    }

    private void ChangeThreeOver(MJoptInfoData data)
    {

        if (mGameUI != null)
            mGameUI.PlayChangeThreeAnim(data);
        //换三张完成
        int seatid;
        int card;
        if (data.changeList == null)
            return;
        for (int i = 0; i < data.changeList.Length; i++)
        {
            seatid = data.changeList[i].seatId;
            for (int j = 0; j < data.changeList[i].inCards.Count; j++)
            {
                card = data.changeList[i].inCards[j];
                MJGameModel.Inst.allPlayersCardsInfoStruct[seatid].handList.Add(card);//从手牌中移除交换的牌
            }
            MJGameModel.Inst.allPlayersCardsInfoStruct[seatid].handList.Sort();
        }
    }

    /// <summary>
    /// 设置玩家定缺
    /// </summary>
    /// <param name="data"></param>
    private void SetOtherFixColor(MJoptInfoData data)
    {
        if (data.fixedList == null)
            return;
        int seatId;
        List<int> newHandCards = null;
        CardsInfoStruct mMJmyInfo;
        for (int i = 0; i < data.fixedList.Count; i++)
        {
            seatId = data.fixedList[i].seatId;
            mMJmyInfo = MJGameModel.Inst.allPlayersCardsInfoStruct[seatId];
            newHandCards = MJGameModel.Inst.allPlayersCardsInfoStruct[seatId].handList;
            if (newHandCards != null && mMJmyInfo.currCard != 0)
                newHandCards.Add(mMJmyInfo.currCard);
            newHandCards = MJGameModel.Inst.ChangList(newHandCards, data.fixedList[i].type.GetHashCode());
            mMJmyInfo.fixedType = data.fixedList[i].type.GetHashCode();
            if (mMJmyInfo.currCard != 0)//如果有摸起来的牌就把手牌中的最后一张当作摸起来的牌
            {
                mMJmyInfo.currCard = newHandCards[newHandCards.Count - 1];
                newHandCards.RemoveAt(newHandCards.Count - 1);
            }
            MJGameModel.Inst.allPlayersCardsInfoStruct[seatId].handList = newHandCards;
            MJGameModel.Inst.AllPlayerEfixe[seatId] = MJGameModel.Inst.GetPlayerFixedType(data.fixedList[i].type);
        }
    }

    /// <summary>
    /// 小结算
    /// </summary>
    /// <param name="msg"></param>
    public void OnSettlement()
    {
        if (mModel.CurRecordDetailData == null)
            return;
        litSemResponse data = mModel.CurRecordDetailData.semData;
        if (data == null)
            return;
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
            oneSettlementPlayerInfo.huDes = data.litSemList[i].huIntro;
            mSettlData.settleContainer.Add(oneSettlementPlayerInfo);
        }
        #endregion

        MJGameModel.Inst.mSettlData = mSettlData;
        mGameUI.GetChaJiaoData(data.flowSemList);
    }
}
