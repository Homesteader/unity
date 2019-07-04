using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJGameBackUI : BaseView
{

    public MJSelfPlayer mSelfPlayer;//我自己
    public List<MJOtherPlayer> mOtherPlayer;//其他玩家 右上左
    public List<MJPlayerBase> mAllPlayer;//其他玩家 0 123456    我自己失踪在1位置
    public MJGamePlayerInfoView mPlayerInfoView;//玩家信息界面
    public MJGameSettlementView mGameSettlementView;//小结算界面
    public MJGameSettlementFinalView mGameSettlementFinalView;//大结算界面
    public MJChangeThreeWidget mMJChangeThree;//换3张 显示
    public MJFixeDcolorWidget mMJFixeDcolor;//定缺  显示
    public UILabel mLeftCardNum;//当前剩余牌数量
    public UILabel mPlayNum;//局数
    public UISprite mPuaseSpr;//暂停按钮图标
    public UILabel mRate;//播放速度

    public int mMySeatId;//我的座位号

    public MJSenceRoot mMJSceneRoot;//场景root

    public GameObject mTWoBtGameObject;//只有上一局，下一句按钮的grid
    public GameObject mFourBtGameObject;//有四个按钮的grid

    public Vector3 mCurCardOffset = new Vector3(-5, 123, 0);//打出的牌特效偏移量

    private int mPlayRate;//播放速度
    private bool mIsPause;//是否暂停
    private MJGameModel mModel;
    private MJGameBackModel mBackModel;

    private float mCurTime;
    private int mCurIndex = 0;//当前指令index
    private int mMaxIndex;//最大指令index
    private float mDeltaTime;
    private const float mTime = 1.5f;//速度
    private bool mIsStart = false;//是否开始
    private int mMaxFanshu;//最大番数
    private int mLeftCardCount;//剩余牌数量

    private MJGameBackController mBackController;//回放controller

    protected override void Awake()
    {
        base.Awake();
        mBackController = Global.Inst.GetController<MJGameBackController>();
        mModel = MJGameModel.Inst;
        mBackModel = MJGameBackModel.Inst;
        InitMJScene();
        mFourBtGameObject.gameObject.SetActive(true);
        mDeltaTime = 2f;

    }


    protected override void Update()
    {
        base.Update();
        if (!mIsStart)
            return;
        if (mIsPause)
            return;
        if (mCurIndex < mMaxIndex)
        {
            mCurTime -= Time.deltaTime;
            if (mCurTime <= 0)
            {
                //执行操作
                // Instructions(mBackModel.mOptList[mCurIndex]);
                mBackController.ChoiseIns(mBackModel.CurRecordDetailData.effOptList[mCurIndex]);
                if (mBackModel.CurRecordDetailData.effOptList[mCurIndex].ins == eMJInstructionsType.ALLCHANGE)//换三张
                    mCurTime = mDeltaTime + 3.5f;
                else
                    mCurTime = mDeltaTime;
                if (mCurIndex + 1 < mMaxIndex && mBackModel.CurRecordDetailData.effOptList[mCurIndex + 1].ins == eMJInstructionsType.SCORE)//如果下一次是分数就在这一帧执行
                    mBackController.ChoiseIns(mBackModel.CurRecordDetailData.effOptList[++mCurIndex]);
                mCurIndex++;
            }
        }
        else
        {
            mIsStart = false;
            mBackController.OnSettlement();
        }
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        mModel.ResetData();
        mBackModel.mOptList = null;
    }

    private void InitData()
    {
        //暂停按钮图标
        mIsPause = false;
        mPuaseSpr.spriteName = "btn_pause";
        //播放速度
        mPlayRate = 1;
        mRate.text = "x" + mPlayRate;
        mDeltaTime = 2f;
        mCurIndex = 0;
        mMaxIndex = 0;
        if (mBackModel.CurRecordDetailData != null && mBackModel.CurRecordDetailData.effOptList != null)
            mMaxIndex = mBackModel.CurRecordDetailData.effOptList.Length;
    }

    public void InitMJScene()
    {
        if (mMJSceneRoot == null)
        {
            mMJSceneRoot = GameObject.Find("MJBackSenceAll").GetComponent<MJSenceRoot>();
            mSelfPlayer.mPlayer = mMJSceneRoot.mMyself;
            mMJSceneRoot.mMyself.mPlayerPosType = mSelfPlayer.mPlayerPosType;
            for (int i = 0; i < mOtherPlayer.Count; i++)
            {
                mOtherPlayer[i].mPlayer = mMJSceneRoot.mOther[i];
                mMJSceneRoot.mOther[i].mPlayerPosType = mOtherPlayer[i].mPlayerPosType;
            }
        }
    }

    /// <summary>
    /// 设置局数
    /// </summary>
    /// <param name="num">当前局数</param>
    /// <param name="allnum">总局数</param>
    public void SetPlayNum(int num, int allnum, bool isshow)
    {
        mPlayNum.transform.parent.gameObject.SetActive(isshow);
        mPlayNum.text = num + "/" + allnum;
        mMJSceneRoot.SetPlayNum(num, allnum);
    }
    /// 小结算后重置UI
    /// </summary>
    public void ReSetUI()
    {
        //重置场景中的物体
        mMJSceneRoot.ResetScene();
        //重置UI
        mSelfPlayer.ResetUI();
        for (int i = 0; i < mAllPlayer.Count; i++)
            mAllPlayer[i].ResetUI();
    }



    #region 按钮点击

    /// <summary>
    /// 返回按钮点击
    /// </summary>
    public void OnBackClick()
    {
        mIsPause = true;
        Global.Inst.GetController<MJGameBackController>().BackToClub();
    }

    /// <summary>
    /// 上一局
    /// </summary>
    public void OnLastMatchClick()
    {
        mSelfPlayer.mZhuangSprite.gameObject.SetActive(false);
        //ShowAllPlayerHead();
        mIsStart = false;
        mGameSettlementView.gameObject.SetActive(false);
        Global.Inst.GetController<MJGameBackController>().GetLastRecord(null);
        mFourBtGameObject.gameObject.SetActive(true);
        mTWoBtGameObject.gameObject.SetActive(false);
        mSelfPlayer.SetCurOutCardEffect(false);

    }

    /// <summary>
    /// 下一局
    /// </summary>
    public void OnNextMatchClick()
    {
        mSelfPlayer.mZhuangSprite.gameObject.SetActive(false);
        //ShowAllPlayerHead();
        mIsStart = false;
        mGameSettlementView.gameObject.SetActive(false);
        Global.Inst.GetController<MJGameBackController>().GetNextRecord(null);
        mFourBtGameObject.gameObject.SetActive(true);
        mTWoBtGameObject.gameObject.SetActive(false);
        mSelfPlayer.SetCurOutCardEffect(false);

    }

    /// <summary>
    /// 暂停点击
    /// </summary>
    public void OnPauseClick()
    {
        mIsPause = !mIsPause;
        if (mIsPause)//暂停时显示继续
            mPuaseSpr.spriteName = "btn_play";
        else
            mPuaseSpr.spriteName = "btn_pause";
    }

    /// <summary>
    /// 播放速度点击
    /// </summary>
    public void OnRateClick()
    {
        mPlayRate += 1;
        if (mPlayRate > 3)
            mPlayRate = 1;
        mRate.text = "x" + mPlayRate;
        mDeltaTime = 1f / mPlayRate * mTime;
    }
    #endregion


    #region 初始化

    /// <summary>
    /// 初始化 玩家数据
    /// </summary>
    /// <param name="info"></param>
    public void InfoAllPlayerData(StartGameRespone info)
    {
        InitData();//初始化操作列表信息
        if (info.roomInfo.roomState == eRoomState.READY || info.roomInfo.roomState == eRoomState.GAMEOVER)// 不显示剩余牌数量
        {
            SetLeftCardNum(info.startInfo.leaveCardNum, false);
        }
        else
        {
            SetLeftCardNum(info.startInfo.leaveCardNum, true);
        }

        InitMJScene();
        //准备
        InitReady(info);
        //头像
        foreach (var item in info.roomInfo.playerList)
        {
            int index = MJGameModel.Inst.mnewSeatToIndex[item.seatId];
            InitOnePlayerIcon(mAllPlayer[index], item);
            SetPlayerSeatId(item.seatId, mAllPlayer[index]);//座位号///**********************
            InitOnePlayerOnlineState(mAllPlayer[index], item);//在线状态
        }
        //手牌
        BuildHandMahjong(MJGameModel.Inst.allPlayersCardsInfoStruct);
        //打出的牌
        InitAllPlayerCollectCards(MJGameModel.Inst.allPlayersCardsInfoStruct);
        //碰的牌
        InfoAllPlayerPengCard(MJGameModel.Inst.allPlayersCardsInfoStruct);
        //听牌标记
        //InitAllTingTag(info.playerList);
        //胡牌标记
        InitAllHuTag(MJGameModel.Inst.allPlayersCardsInfoStruct);
        //定缺标记
        InitAllFixeType(MJGameModel.Inst.allPlayersCardsInfoStruct, info.roomInfo.roomState);
        //设置 打牌指针 轮到谁操作
        mMJSceneRoot.SetLight(MJGameModel.Inst.mCurInsSeatId);
        //cd
        SetAllPlayerCD(MJGameModel.Inst.mCurInsSeatId);
        //设置 色子显示
        mMJSceneRoot.SetTouZiNum(info.startInfo.dices);
        //桌上的牌
        if (info.roomInfo.roomState == eRoomState.READY || info.roomInfo.roomState == eRoomState.GAMEOVER)//准备和结束不显桌上的牌
            mMJSceneRoot.SetDeskCard(0, info.startInfo.dices, true);
        else
            mMJSceneRoot.SetDeskCard(info.startInfo.leaveCardNum, info.startInfo.dices, true);
        //桌子位置
        mMJSceneRoot.SetDescRotate(MJGameModel.Inst.mMySeatId - 1);
        //桌子上信息
        int _roomId = info.roomInfo.mode == "m4" ? 0 : int.Parse(info.roomInfo.roomId);//如果是平台房就不显示房间号
        mMJSceneRoot.SetDescInfo(999, int.Parse(info.roomInfo.roomId), info.roomInfo.currGameCount, info.roomInfo.maxGameCount, 9999);//  番数       低分  没有
        //上一次打出牌特效
        if (info.roomInfo.roomState == eRoomState.START && info.roomInfo.lastHitSeatId != 0)
        {
            int index = MJGameModel.Inst.mnewSeatToIndex[info.roomInfo.lastHitSeatId];
            SetCurCardEffect(mAllPlayer[index], eMJInstructionsType.HIT, mCurCardOffset);//打出的牌特效

        }
        SetRoomPlayNum(MJGameModel.Inst.mStartGameData.roomInfo.currGameCount, true);//设局数显示
        //我的操作列表
        mSelfPlayer.ShowInstructions(info.roomInfo.optList);
        //MJGameModel.Inst.mIsTing = false;//听牌
        MJGameModel.Inst.isFirstGetStartGameData = false;

        mIsStart = true;
    }
    #region 初始化玩家所有数据

    /// <summary>
    /// 初始化或刷新自己的牌
    /// </summary>
    /// <param name="player"></param>
    /// <param name="info"></param>
    /// <param name="myinfo"></param>
    public void InitSelfPlayerCards(MJPlayerBase player, PlayerInfoStruct info, CardsInfoStruct myinfo,MJoptInfoData opt = null)
    {
        //手牌

        SQDebug.Log(myinfo.currCard);
        InitOnePlayerHandCards(player, myinfo.handList, 0, myinfo.currCard);
        //打出的牌
        InitOnePlayerCollectCards(player, myinfo);
        //碰的牌
        InitOnePlayerPengCard(player, myinfo);
        //胡牌
        if (opt != null)
        {
            bool zimo = false;
            if (opt.huType != null)
            {
                for (int i = 0; i < opt.huType.Length; i++)
                {
                    if (opt.huType[i] == (int)eHuType.ZIMO)
                    {
                        zimo = true;
                    }
                }
            }
            if (zimo)
            {
                InitHuCard(player, myinfo, (int)eHuType.ZIMO);
            }
            else
            {
                InitHuCard(player, myinfo);
            }
        }
        else
        {
            //胡牌
            InitHuCard(player, myinfo);
        }
    }

    /// <summary>
    /// 设置当前打出牌位置
    /// </summary>
    /// <param name="player"></param>
    /// <param name="ins"></param>
    public void SetCurCardEffect(MJPlayerBase player, eMJInstructionsType ins, Vector3 offset)
    {
        if (ins == eMJInstructionsType.HIT)
        {
            player.SetCurOutCardEffect(true);
        }
        else
        {
            if (MJGameModel.Inst.mLastOutCard == 0)
                player.SetCurOutCardEffect(false);
        }
    }


    /// <summary>
    /// 初始化胡牌
    /// </summary>
    /// <param name="player"></param>
    /// <param name="info"></param>
    public void InitHuCard(MJPlayerBase player, CardsInfoStruct info,int huType=-1)
    {
        if (info.huList != null && info.huList.Count >= 0)
            player.InitHuCards(eMJInstructionsType.HU, info.huList, huType);
    }


    /// <summary>
    /// 刷新自己的手牌
    /// </summary>
    public void RefreshMyCards()
    {
        MJPlayerBase player = mAllPlayer[1];
        int seatid = MJGameModel.Inst.mMySeatId;
        InitOnePlayerHandCards(player, MJGameModel.Inst.allPlayersCardsInfoStruct[seatid].handList, 0, MJGameModel.Inst.allPlayersCardsInfoStruct[seatid].currCard);
    }

    /// <summary>
    /// 初始化其他玩家牌
    /// </summary>
    /// <param name="player"></param>
    /// <param name="info"></param>
    /// <param name="oinfo"></param>
    public void InitOtherPlayerCards(MJPlayerBase player, PlayerInfoStruct info, CardsInfoStruct oinfo)
    {
        //手牌
        int cur = oinfo.isHasCurrCard ? 1 : 0;
        InitOnePlayerHandCards(player, null, oinfo.cardNum, cur);
        //打出的牌
        InitOnePlayerCollectCards(player, oinfo);
        InitOnePlayerPengCard(player, oinfo);
        //胡牌
        InitHuCard(player, oinfo);
    }


    #endregion

    #region    胡 听牌标记
    private void InitAllHuTag(CardsInfoStruct[] info)
    {
        foreach (var item in info)
        {
            if (item != null)
            {
                int index = 0;
                List<HuStruct> pinfo = item.huList;
                index = MJGameModel.Inst.mnewSeatToIndex[item.seatId];
                if (item.huList != null) {
                    mAllPlayer[index].SetHuTag(pinfo.Count > 0);
                    for (int i=0;i< item.huList.Count;i++) {
                        if (item.huList[i].huType == (int)eHuType.ZIMO) {
                            mAllPlayer[index].SetZiMoHu();
                        }
                    }
                }
            }
        }
    }


    private void InitAllFixeType(CardsInfoStruct[] info, eRoomState state)
    {
        if (state == eRoomState.FIXEDCOLOR)
            return;
        foreach (var item in info)
        {
            if (item != null)
            {
                eFixedType _type = (eFixedType)item.fixedType;
                if (_type != eFixedType.NONE && item.isChanged)
                {
                    int index = MJGameModel.Inst.mnewSeatToIndex[item.seatId];
                    mAllPlayer[index].UpdataFixe(_type, item.seatId == MJGameModel.Inst.mMySeatId);
                }
            }
        }

    }

    #endregion

    #region 初始化玩家头像
    /// <summary>
    /// 设置积分  特效
    /// </summary>
    /// <param name="seatid"></param>
    /// <param name="point"></param>
    public void SetScore(int seatid, float point, float addScore, int type = 0)
    {
        int index = MJGameModel.Inst.mnewSeatToIndex[seatid];
        mAllPlayer[index].mHead.SetPoint(point, addScore, type);
        mAllPlayer[index].mHead.SetReScoreEff(addScore);
    }

    /// <summary>
    /// 设置玩家离开
    /// </summary>
    public void SetAllPlayerOutLine()
    {
        if (MJGameModel.Inst.mRoomPlayers != null)
        {
            int index;
            PlayerInfoStruct info;
            for (int i = 0; i < MJGameModel.Inst.mRoomPlayers.Length; i++)
            {
                info = MJGameModel.Inst.mRoomPlayers[i];
                if (info != null)
                {
                    index = MJGameModel.Inst.mnewSeatToIndex[info.seatId];
                    if (info.onLineType == eMJOnlineState.leave.GetHashCode())//如果是离线就隐藏头像
                    {
                        mAllPlayer[index].GetOutRoom();
                    }
                }
            }
        }
    }
    #endregion

    #region 设置玩家座位号
    public void SetPlayerSeatId(int seatid, MJPlayerBase player)
    {
        player.mMyseat = seatid;
        player.mPlayer.mMySeatid = seatid;
    }
    #endregion 

    #region 初始化玩家手牌

    public void InitOnePlayerHandCards(MJPlayerBase player, List<int> card, int count, int cur)
    {
        player.InitHandCards(card, count, cur);
    }
    /// <summary>
    /// 结算前 手牌显示
    /// </summary>
    /// <param name="index"></param>
    /// <param name="cards"></param>
    public void GetPlayerHand(int index, List<int> cards, bool isSelf)
    {
        mAllPlayer[index].GetPlayerHandCards(cards, isSelf);

    }

    #endregion

    #region 初始化玩家打出的牌
    public void InitOnePlayerCollectCards(MJPlayerBase player, CardsInfoStruct info)
    {
        if (info == null)
            player.InitCollectCard(null);
        else
            player.InitCollectCard(info.hitList);
    }
    #endregion

    #region 初始化玩家碰杠吃等牌
    public void InitOnePlayerPengCard(MJPlayerBase player, CardsInfoStruct info)
    {
        if (info == null)
            player.InitPengCards(eMJInstructionsType.PENG, null);
        else
        {
            List<MJliangcard> list = new List<MJliangcard>();
            if (info.gangList != null)
                player.InitGangCards(eMJInstructionsType.GANG, info.gangList);
            if (info.pengList != null)
            {
                player.InitPengCards(eMJInstructionsType.PENG, info.pengList);
            }
        }
    }
    #endregion
    #endregion

    #region 同步服务器操作


    /// <summary>
    /// 谁操作了牌
    /// </summary>
    /// <param name="data"></param>
    public void ServerWhoInstructions(MJoptInfoData data)
    {
        int index = data.seatId - 1;
        int index2 = data.otherSeatId - 1;
        if (data.ins == eMJInstructionsType.MO)
        {
            mMJSceneRoot.SetDeskCard(0, MJGameModel.Inst.mTouzi, false);//桌上的牌
            SetLeftCardNum(MJGameModel.Inst.mStartGameData.startInfo.leaveCardNum, true);
        }
        if (data.seatId != MJGameModel.Inst.mMySeatId)
            mSelfPlayer.ShowInstructions(null);
        mSelfPlayer.mSpeciaInstructionRoot.SetActive(false);
        MJGameModel.Inst.mIsTing = false;//听牌
                                         //主动操作的

        {
            int k = MJGameModel.Inst.mnewSeatToIndex[data.seatId];
            InitSelfPlayerCards(mAllPlayer[k], MJGameModel.Inst.mRoomPlayers[data.seatId], MJGameModel.Inst.allPlayersCardsInfoStruct[data.seatId],data);
            mMJSceneRoot.SetEffectShow(data, data.seatId, data.huType, data.subType);//特效
            SetCurCardEffect(mAllPlayer[k], data.ins, mCurCardOffset);//打出的牌特效
        }
        //轮到谁操作
        mMJSceneRoot.SetLight(index + 1);
        SetAllPlayerCD(index + 1);
        //被动操作的
        if (index != index2 && index2 >= 0)
        {
            int k = MJGameModel.Inst.mnewSeatToIndex[data.otherSeatId];

            InitSelfPlayerCards(mAllPlayer[k], MJGameModel.Inst.mRoomPlayers[data.otherSeatId], MJGameModel.Inst.allPlayersCardsInfoStruct[data.otherSeatId],data);

        }

        InitAllHuTag(MJGameModel.Inst.allPlayersCardsInfoStruct);//刷新玩家  胡牌标记
    }

    /// <summary>
    /// 设置玩家cd
    /// </summary>
    /// <param name="seatid"></param>
    /// <param name="t"></param>
    private void SetAllPlayerCD(int seatid)
    {
        int index = MJGameModel.Inst.mnewSeatToIndex[seatid];
        for (int i = 0; i < mAllPlayer.Count; i++)
        {
            if (mAllPlayer[i] != null)
            {
                if (index == i)
                    mAllPlayer[i].SetCD(1);
                else
                    mAllPlayer[i].SetCD(0);
            }
        }

    }

    /// <summary>
    /// 设置某个玩家cd
    /// </summary>
    /// <param name="seatid"></param>
    /// <param name="t"></param>
    private void SetOnePlayerCD(int seatid, float t)
    {
        int index = MJGameModel.Inst.mnewSeatToIndex[seatid];
        mAllPlayer[index].SetCD(t);
    }

    /// <summary>
    /// 流局
    /// </summary>
    public void ServerNoWinner()
    {
        mMJSceneRoot.SetNoWinnerShow();
    }

    /// <summary>
    /// 小结算
    /// </summary>
    /// <param name="info"></param>
    private void ServerSettlement(MJGameSettlementInfo info)
    {
        if (mGameSettlementView == null)
        {
            mGameSettlementView = GetWidget<MJGameSettlementView>("Windows/Majiang/GameUI/GameSettlementView", transform);
        }
        mGameSettlementView.gameObject.SetActive(true);
        mGameSettlementView.SetData(info, MJGameModel.Inst.mMaxMaxMul);
        mGameSettlementView.SetBackBtnShow(OnBackClick);
    }

    /// <summary>
    /// 大结算
    /// </summary>
    /// <param name="info"></param>
    private void ServerSettlementFinal(MJGameSettlementFinalInfo info)
    {
        if (mGameSettlementFinalView == null)
        {
            mGameSettlementFinalView = GetWidget<MJGameSettlementFinalView>("Windows/Majiang/GameUI/GameSettlementFinalView", transform);
        }
        mGameSettlementFinalView.gameObject.SetActive(true);
        mGameSettlementFinalView.SetData(info);
    }

    #endregion

    #region 剩余牌,剩余时间
    /// <summary>
    /// 设置剩余牌数量
    /// </summary>
    /// <param name="num"></param>
    /// <param name="isshow"></param>
    public void SetLeftCardNum(int num, bool isshow)
    {
        mLeftCardNum.transform.parent.gameObject.SetActive(isshow);
        mLeftCardNum.text = num.ToString();
    }


    #endregion

    /// <summary>
    /// 设置局数
    /// </summary>
    /// <param name="num">当前局数</param>
    /// <param name="allnum">总局数</param>
    public void SetRoomPlayNum(int num, bool isshow)
    {
        mPlayNum.transform.parent.gameObject.SetActive(isshow);
        mPlayNum.text = num + "/" + MJGameModel.Inst.mStartGameData.roomInfo.maxGameCount;
        mMJSceneRoot.SetPlayNum(num, MJGameModel.Inst.mStartGameData.roomInfo.maxGameCount);
    }
    /// <summary>
    /// 玩家头像加载
    /// </summary>
    /// <param name="player"></param>
    /// <param name="info"></param>
    public void InitOnePlayerIcon(MJPlayerBase player, PlayerInfoStruct info)
    {
        player.SetIcon(info);
    }

    /// <summary>
    /// 玩家在线状态
    /// </summary>
    /// <param name="player"></param>
    /// <param name="data"></param>
    private void InitOnePlayerOnlineState(MJPlayerBase player, PlayerInfoStruct data)
    {
        player.SetOffLine((eMJOnlineState)data.onLineType);
    }

    /// <summary>
    /// 生成手牌麻将
    /// </summary>
    public void BuildHandMahjong(CardsInfoStruct[] cardsInfoList)
    {
        int index;
        foreach (var item in cardsInfoList)
        {
            if (item != null)
            {
                index = MJGameModel.Inst.mnewSeatToIndex[item.seatId];
                //if (item.seatId == MJGameModel.Inst.mMySeatId)
                {
                    InitOnePlayerHandCards(mAllPlayer[index], item.handList, item.cardNum, item.currCard);
                }
                //else
                //{
                //    int cur = item.isHasCurrCard ? 1 : 0;
                //    InitOnePlayerHandCards(mAllPlayer[index], null, item.cardNum, cur);
                //}

            }
        }
    }
    #region 打出的牌
    /// <summary>
    /// 打牌列表
    /// </summary>
    /// <param name="cardsInfoList"></param>
    public void InitAllPlayerCollectCards(CardsInfoStruct[] cardsInfoList)
    {
        int index;
        foreach (var item in cardsInfoList)
        {
            if (item != null)
            {
                index = MJGameModel.Inst.mnewSeatToIndex[item.seatId];

                InfoOnePlayerCollectCards(mAllPlayer[index], item.hitList);
            }
        }
    }

    public void InfoOnePlayerCollectCards(MJPlayerBase player, List<int> hitList)
    {
        if (hitList == null)
            player.InitCollectCard(null);
        else
            player.InitCollectCard(hitList);
    }
    #endregion

    #region 初始化玩家碰杠吃等牌

    public void InfoAllPlayerPengCard(CardsInfoStruct[] cardsInfoList)
    {
        int index;
        //玩家信息为空
        if (cardsInfoList == null)
        {
            for (int i = 0; i < mAllPlayer.Count; i++)
                InfoOnePlayerPengCard(mAllPlayer[i], null);
        }
        else
        {
            foreach (var item in cardsInfoList)
            {
                if (item != null)
                {
                    index = MJGameModel.Inst.mnewSeatToIndex[item.seatId];

                    InfoOnePlayerPengCard(mAllPlayer[index], item);
                }

            }
        }
    }

    public void InfoOnePlayerPengCard(MJPlayerBase player, CardsInfoStruct info)
    {
        if (info == null)
            player.InitPengCards(eMJInstructionsType.PENG, null);
        else
        {
            List<MJliangcard> list = new List<MJliangcard>();
            if (info.gangList != null)
                player.InitGangCards(eMJInstructionsType.GANG, info.gangList);

            if (info.huList != null)
                player.InitHuCards(eMJInstructionsType.HU, info.huList);
            if (info.pengList != null)
            {
                //list.AddRange(info.pengList);
                player.InitPengCards(eMJInstructionsType.PENG, info.pengList);
            }
        }
    }
    #endregion



    /// <summary>
    /// 准备
    /// </summary>
    /// <param name="info"></param>
    private void InitReady(StartGameRespone info)
    {
        if (info.roomInfo.roomState == eRoomState.READY || info.roomInfo.roomState == eRoomState.GAMEOVER)//在准备阶段或是结束阶段
        {

            for (int i = 0; i < info.roomInfo.playerList.Count; i++)
            {
                PlayerInfoStruct item = info.roomInfo.playerList[i];
                if (item.seatId == MJGameModel.Inst.mMySeatId)//自己
                {
                }
                mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[item.seatId]].SetReady(item.isReady);
            }
        }
        else
        {
            //全部不显示准备
            for (int i = 0; i < info.roomInfo.playerList.Count; i++)
            {
                PlayerInfoStruct item = info.roomInfo.playerList[i];
                mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[item.seatId]].SetReady(false);
            }
        }
    }
    #region 换3张处理

    /// <summary>
    /// 换3张状态
    /// </summary>
    public void ChangeThreeUI(MJInstructionsProto data)
    {
        for (int i = 0; i < MJGameModel.Inst.mRoomPlayers.Length; i++)
        {
            PlayerInfoStruct item = MJGameModel.Inst.mRoomPlayers[i];
            if (item != null)
            {
                if (item.seatId == MJGameModel.Inst.mMySeatId)
                {
                    mMJChangeThree.IntoThreeWidget(true);
                }
                else
                {
                    mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[item.seatId]].SetChangeStateShow(true);
                }
            }
        }
        InitChaneThreeHandCards(mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[MJGameModel.Inst.mMySeatId]], data.optList[0].cards);
    }

    private void InitChaneThreeHandCards(MJPlayerBase player, List<int> cards)
    {
        player.InitChaneThreeHandCards(cards);

    }
    #endregion
    /// <summary>
    /// 定缺处理
    /// </summary>
    public void ChangeFixeDcolorUI(MJInstructionsProto data)
    {
        foreach (var item in MJGameModel.Inst.mRoomPlayers)
        {
            if (item != null)
            {
                if (item.seatId == MJGameModel.Inst.mMySeatId)
                    mMJFixeDcolor.IntoView(data.optList[0].type);
                else
                    mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[item.seatId]].SetDingqueShow(true, "定缺中");
            }
        }

    }

    /// <summary>
    /// 获取消息后  刷新手牌
    /// </summary>
    public void UpdataHandCardsAfterGetMsg()
    {
        foreach (var item in MJGameModel.Inst.mRoomPlayers)
        {
            if (item != null)
            {
                if (item.seatId != MJGameModel.Inst.mMySeatId)
                {
                    mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[item.seatId]].SetChangeStateShow(false);
                }
                int index = MJGameModel.Inst.mnewSeatToIndex[item.seatId];
                InitOnePlayerHandCards(mAllPlayer[index], MJGameModel.Inst.allPlayersCardsInfoStruct[item.seatId].handList, 0, 0);
            }
        }
    }

    /// <summary>
    /// 播放 换牌动画
    /// </summary>
    public void PlayChangeThreeAnim(MJoptInfoData data)
    {
        UpdataHandCardsAfterGetMsg();
        SQDebug.Log("换牌动画");
        SetPlayAnim(data);
        DelayRun(2.1f, () =>
        {
            Global.Inst.GetController<MJGameController>().ExcuteOptInChange();
        });
    }

    private void SetPlayAnim(MJoptInfoData data)
    {
        int seatid;
        int index;
        //三张牌摆出来
        for (int i = 0; i < data.changeList.Length; i++)
        {
            seatid = data.changeList[i].seatId;
            index = MJGameModel.Inst.mnewSeatToIndex[seatid];
            if (mAllPlayer[index] != null)
            {
                mAllPlayer[index].SetChangeThreeCardsNum(data.changeList[i].outCards);//显示换三张牌
                mAllPlayer[index].ChangeThreeObj(true);
            }
        }
        DelayRun(1.5f, () =>
        {
            RotaCards(data.type);
        });

        DelayRun(3.5f, () =>
        {
            for (int i = 0; i < MJGameModel.Inst.mRoomPlayers.Length; i++)
            {
                if (MJGameModel.Inst.mRoomPlayers[i] == null)
                    continue;
                seatid = MJGameModel.Inst.mRoomPlayers[i].seatId;
                index = MJGameModel.Inst.mnewSeatToIndex[seatid];
                if (mAllPlayer[index] != null)
                {
                    mAllPlayer[index].ChangeThreeObj(false);
                    mAllPlayer[index].RotaThreeObj(0, 0);
                }
            }
            UpdataHandCardsAfterGetMsg();
        });
    }
    private void RotaCards(int dataType)
    {
        int playerNum = MJGameModel.Inst.totalPlayerCount;
        int t = 1;
        float augle = 90f;
        switch (dataType)
        {
            case (int)eChangType.NSZ:
                t = -1;
                break;
            case (int)eChangType.SSZ:
                t = 1;
                break;
            case (int)eChangType.DJ:
                t = 0;
                return;
        }
        int index;
        int seatid;
        int myseatId = MJGameModel.Inst.mMySeatId;//我的座位号

        for (int i = 0; i < MJGameModel.Inst.mRoomPlayers.Length; i++)
        {
            if (MJGameModel.Inst.mRoomPlayers[i] == null)
                continue;
            seatid = MJGameModel.Inst.mRoomPlayers[i].seatId;
            index = MJGameModel.Inst.mnewSeatToIndex[seatid];
            if (mAllPlayer[index] != null)
            {
                augle = 90f * t;
                if (playerNum == 3)
                {
                    if (t == -1 && (seatid - myseatId == 1 || seatid - myseatId == -2))//逆时针
                    {
                        augle = -180f;
                    }
                    if (t == 1 && (myseatId - seatid == 1 || myseatId - seatid == -2))//顺时针
                    {
                        augle = 180f;
                    }
                }
                mAllPlayer[index].RotaThreeObj(t, augle);
            }
        }
    }

    /// <summary>
    /// 定缺成功后  事件处理
    /// </summary>
    /// <param name="data"></param>
    public void SureFixeDcolor(MJoptInfoData data)
    {

        UpdataHandCardsAfterGetMsg();
    }
    /// <summary>
    /// 所有人都定缺完成
    /// </summary>
    public void SureAllFixeDcolor(MJoptInfoData data)
    {
        mMJFixeDcolor.gameObject.SetActive(false);

        for (int i = 0; i < data.fixedList.Count; i++)
        {
            bool isMy = data.fixedList[i].seatId == MJGameModel.Inst.mMySeatId;
            mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[data.fixedList[i].seatId]].UpdataFixe(data.fixedList[i].type, isMy);
        }
        UpdataHandCardsAfterGetMsg();//刷新手牌
    }
    /// <summary>
    /// 查叫 显示
    /// </summary>
    /// <param name="flowSemList"></param>
    public void GetChaJiaoData(List<FlowSemStruct> flowSemList)
    {
        if (flowSemList != null && flowSemList.Count > 0)
        {
            StartCoroutine(SetChatJiaoIenum(flowSemList));
        }
        else
        {
            for (int i = 0; i < mAllPlayer.Count; i++)
            {
                if (mAllPlayer[i] != null)
                {
                    mAllPlayer[i].SetChajiaoScores(0, eEff.NONE, 0, false);
                }
            }
        }
    }

    /// <summary>
    /// 设置查叫显示
    /// </summary>
    /// <param name="flowSemList"></param>
    /// <returns></returns>
    IEnumerator SetChatJiaoIenum(List<FlowSemStruct> flowSemList)
    {
        int seatId = 0;
        for (int i = 0; i < flowSemList.Count; i++)
        {
            FlowSemStruct chajiaoInfo = flowSemList[i];//要查叫的玩家
            seatId = chajiaoInfo.seatId;//被查叫的玩家id
            if (chajiaoInfo.effList == null)
                continue;
            for (int j = 0; j < flowSemList[i].effList.Count; j++)//被查叫玩家查叫类型
            {
                effStruct efstruct = chajiaoInfo.effList[j];
                eEff etype = (eEff)efstruct.type;//查叫类型
                for (int k = 0; k < efstruct.scoreList.Count; k++)
                {
                    ScoreStruct scoredata = efstruct.scoreList[k];//受影响的玩家
                                                                  //显示分数
                    if (seatId == scoredata.seatId)
                        mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[scoredata.seatId]].SetChajiaoScores(scoredata.seatId, etype, scoredata.score);
                    else
                        mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[scoredata.seatId]].SetChajiaoScores(scoredata.seatId, eEff.NONE, scoredata.score);
                }
                yield return new WaitForSeconds(1f);
                SetChatjiaoScoresHide();
            }
        }
        ServerSettlement(MJGameModel.Inst.mSettlData);
        SetChatjiaoScoresHide();
    }

    private void SetChatjiaoScoresHide()
    {
        for (int i = 0; i < mAllPlayer.Count; i++)
        {
            if (mAllPlayer[i] != null)
            {
                mAllPlayer[i].SetChajiaoScores(0, eEff.NONE, 0, false);
            }
        }
    }
}
