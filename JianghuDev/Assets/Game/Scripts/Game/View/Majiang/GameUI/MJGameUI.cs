using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//using YunvaIM;

public class MJGameUI : BaseView
{
    public Transform mContent;//content
    public UILabel mRoomId;//房间号
    public GameObject mReadyRoot;//准备界面
    public UIButton mReadyBtn;//准备按钮
    public UIButton mInvitationBtn;//邀请按钮
    public GameObject mVoiceBtn;//聊天按钮
    public UILabel mVoiceCDTimeLabel;//聊天cd
    public GameObject mMicBtn;//麦克风按钮
    public UIButton mMenuBtn;//菜单按钮
    public MJSelfPlayer mSelfPlayer;//我自己
    public List<MJOtherPlayer> mOtherPlayer;//其他玩家 右上左
    public List<MJPlayerBase> mAllPlayer;//其他玩家 0 123456    我自己失踪在1位置

    private int mLastCardSeatId;//上一个打出牌的玩家座位号
    private int mLastCard;// 上一次打出的牌

    //子界面
    public MJChangeThreeWidget mMJChangeThree;//换3张 显示

    public MJFixeDcolorWidget mMJFixeDcolor;//定缺  显示

    public GameObject mBtnPromptRoot;//胡牌按钮提示Grid
    public GameObject mChahuBtn;//查胡按钮

    //public GameChatWidget mChatView;//文字聊天界面
    private SettingWidget mSettingView;//设置界面
    //public LookUserInfoWidget mPlayerInfoView;//玩家信息界面
    private MJGameSettlementView mGameSettlementView;//小结算界面
    private MJGameSettlementFinalView mGameSettlementFinalView;//大结算界面
    private MJGameInfoView mGameInfoView;//信息界面
    //分享
    private GameObject mShareRoot;//分享root
    public GameObject mCurCardEffect;//当前打出的牌特效
    public UILabel mLeftCardNum;//当前剩余牌数量
    public UILabel mPlayNum;//局数
    public Vector3 mCurCardOffset = new Vector3(-5, 123, 0);//打出的牌特效偏移量
    public UILabel mCountDown;//倒计时
    public UILabel mReadyCountDown;//准备倒计时
    public GameObject mChangeDeskBtn;//换桌按钮

    public int mMySeatId;//我的座位号

    public MJSenceRoot mMJSceneRoot;//场景root

    public SpriteAnimation mVoiceAnimation;//语音动画

    public UILabel mTimeLabel;//时间

    public SpriteAnimation mHuDongAnimationMove;//移动的互动动画
    public Dictionary<int, Transform> mHuDongFacePosDic = new Dictionary<int, Transform>();//已座位号为key，互动表情的位置为value


    private bool mIsSpeeking;//是否已经按下录音按钮
    private float mCurRecordTime;//当前录音时间
    private float mLeftTime;//剩余操作时间
    private string roomId;//当前房间id
    private GameObject mbg;
    private string mCurMode;//当前模式，m4是平台房
    private float mReadyCountDownTime;//准备倒计时
    private int mCurOptPlayerIndex;//当前操作的玩家
    private float mVoiceCDTime;//聊天cd
    private MJGameController mContrl;

    protected override void Awake()
    {
        base.Awake();
        mContrl = Global.Inst.GetController<MJGameController>();
        //AddEventListenerTarget(GlobalEvent.inst, eEvent.Chat, MJChatEvent);
        InitMJScene();
#if YYVOICE
        //登陆呀呀语音
        YYsdkManager.instance.LoginVoiceServer(PlayerModel.Inst.UserInfo.userId);
#endif
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Update()
    {
        base.Update();
        mTimeLabel.text = string.Format("{0:D2}:{1:D2}:{2:D2}", System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
        if (mLeftTime > 0)
            SetLeftTime();
        //聊天cd
        SetChatCD();
        //测试
        //if (Input.GetKeyUp(KeyCode.Q))
        //GetChaJiaoData(null);
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        MJGameModel.Inst.ResetData();//销毁时清除数据，防止退出时数据还保留
    }

    public void InitMJScene()
    {
        if (mMJSceneRoot == null)
        {
            mMJSceneRoot = GameObject.Find("MJSenceAll").GetComponent<MJSenceRoot>();
            mSelfPlayer.mPlayer = mMJSceneRoot.mMyself;
            mMJSceneRoot.mMyself.mPlayerPosType = mSelfPlayer.mPlayerPosType;
            for (int i = 0; i < mOtherPlayer.Count; i++)
            {
                if (mOtherPlayer[i] == null)
                    continue;
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
    public void SetPlayNum(int num, bool isshow)
    {
        mPlayNum.transform.parent.gameObject.SetActive(false);
        mPlayNum.text = num + "/" + mContrl.mModel.mPlayCount;
        mMJSceneRoot.SetPlayNum(num, mContrl.mModel.mPlayCount);
    }

    /// <summary>
    /// 小结算后重置UI
    /// </summary>
    public void ReSetUI(bool depth = false)
    {
        //重置场景中的物体
        mMJSceneRoot.ResetScene();
        //重置UI
        mSelfPlayer.ResetUI();
        GetChaJiaoData(null);
        for (int i = 0; i < mOtherPlayer.Count; i++)
            mOtherPlayer[i].ResetUI(depth);
        //是否有叫的按钮
        SetChahuBtnShow(false);
    }
    #region 初始化
    #region 初始化玩家所有数据

    /// <summary>
    /// 初始化或刷新自己的牌
    /// </summary>
    /// <param name="player"></param>
    /// <param name="info"></param>
    /// <param name="myinfo"></param>
    public void InitSelfPlayerCards(MJPlayerBase player, PlayerInfoStruct info, CardsInfoStruct myinfo, MJoptInfoData opt= null)
    {
        //手牌

        SQDebug.Log(myinfo.currCard);
        InitOnePlayerHandCards(player, myinfo.handList, 0, myinfo.currCard);
        //打出的牌
        InitOnePlayerCollectCards(player, myinfo);
        //碰的牌
        InitOnePlayerPengCard(player, myinfo);
        if (opt != null)
        {
            bool zimo = false;
            if (opt.type == (int)eHuType.ZIMO)
            {
                zimo = true;
            }
            if (zimo)
            {
                InitHuCard(player, myinfo, (int)eHuType.ZIMO);
            }
            else {
                InitHuCard(player, myinfo);
            }
        }
        else {
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
            if (mContrl.mModel.mLastOutCard == 0)
                player.SetCurOutCardEffect(false);
        }
    }


    /// <summary>
    /// 初始化胡牌
    /// </summary>
    /// <param name="player"></param>
    /// <param name="info"></param>
    public void InitHuCard(MJPlayerBase player, CardsInfoStruct info, int hutype = -1)
    {
        if (info.huList != null && info.huList.Count >= 0)
            player.InitHuCards(eMJInstructionsType.HU, info.huList, hutype);
    }

    /// <summary>
    /// 查胡按钮显示
    /// </summary>
    /// <param name="isshow"></param>
    private void SetChahuBtnShow(bool isshow)
    {
        mChahuBtn.SetActive(isshow);
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
    public void InitOtherPlayerCards(MJPlayerBase player, PlayerInfoStruct info, CardsInfoStruct oinfo, MJoptInfoData opt = null)
    {
        //手牌
        int cur = oinfo.isHasCurrCard ? 1 : 0;
        InitOnePlayerHandCards(player, null, oinfo.cardNum, cur);
        //打出的牌
        InitOnePlayerCollectCards(player, oinfo);
        InitOnePlayerPengCard(player, oinfo);

        if (opt != null)
        {
            bool zimo = false;
            if (opt.type == (int)eHuType.ZIMO)
            {
                zimo = true;
            }
            if (zimo)
            {
                InitHuCard(player, oinfo, (int)eHuType.ZIMO);
            }
            else
            {
                InitHuCard(player, oinfo);
            }
        }
        else
        {
            //胡牌
            InitHuCard(player, oinfo);
        }
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
                index = mContrl.mModel.mnewSeatToIndex[item.seatId];
                if (item.huList != null) {
                    mAllPlayer[index].SetHuTag(pinfo.Count > 0);
                    for (int i = 0; i < item.huList.Count;i++)
                    {
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
        if (state == eRoomState.FIXEDCOLOR || state == eRoomState.CHANGETHREE)
            return;
        foreach (var item in info)
        {
            if (item != null)
            {
                eFixedType _type = (eFixedType)item.fixedType;
                if (_type != eFixedType.NONE && item.isFixedColor)
                {
                    int index = mContrl.mModel.mnewSeatToIndex[item.seatId];
                    mAllPlayer[index].UpdataFixe(_type, item.seatId == MJGameModel.Inst.mMySeatId);
                }
            }
        }

    }

    #endregion

    /// <summary>
    /// 是否弃牌
    /// </summary>
    /// <param name="seatid"></param>
    public void SetPlayerGiveUp(MJPlayerBase player, bool isGiveUp)
    {
        player.SetIsGiveUp(isGiveUp);
    }

    #region 初始化玩家头像
    /// <summary>
    /// 设置积分  特效
    /// </summary>
    /// <param name="seatid"></param>
    /// <param name="point"></param>
    public void SetScore(int seatid, float point, float addScore, int type = 0)
    {
        int index = mContrl.mModel.mnewSeatToIndex[seatid];
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

    #region 准备倒计时
    /// <summary>
    /// 设置准备倒计时是否显示 开始发牌和有玩家离开时关闭准备倒计时
    /// </summary>
    /// <param name="isshow"></param>
    /// <param name="time">倒计时时间</param>
    private void SetReadyCountShow(bool isshow, int time)
    {
        mReadyCountDown.gameObject.SetActive(isshow);
        mReadyCountDownTime = time + Time.realtimeSinceStartup;
        StopCoroutine("ReadyCountDown");
        if (isshow)
        {
            StartCoroutine("ReadyCountDown");
        }
    }


    IEnumerator ReadyCountDown()
    {
        while (mReadyCountDownTime > Time.realtimeSinceStartup)
        {
            yield return new WaitForSeconds(0.3f);
            mReadyCountDown.text = ((int)(mReadyCountDownTime - Time.realtimeSinceStartup)).ToString();
            if (mGameSettlementView != null)
                mGameSettlementView.SetContinueLabel(mReadyCountDown.text);
        }
    }
    #endregion

    #region 设置玩家座位号
    public void SetPlayerSeatId(int seatid, MJPlayerBase player)
    {
        player.mMyseat = seatid;
        if (mHuDongFacePosDic.ContainsKey(seatid))
        {
            return;
        }
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
    /// 玩家加入房间
    /// </summary>
    /// <param name="info"></param>
    public void ServerPlayerJoinGame(PlayerInfoStruct info)
    {
        int index = MJGameModel.Inst.mnewSeatToIndex[info.seatId];
        InitOnePlayerIcon(mAllPlayer[index], info);
    }

    /// <summary>
    /// 玩家准备
    /// </summary>
    /// <param name="seatId"></param>
    public void ServerPlayerPre(int seatId)
    {

        if (seatId == mContrl.mModel.mMySeatId)
        {
            mSelfPlayer.SetReady(true);
        }
        else
        {
            int index = mContrl.mModel.mnewSeatToIndex[seatId];
            mAllPlayer[index].SetReady(true);
        }
    }

    /// <summary>
    /// 准备倒计时
    /// </summary>
    /// <param name="time"></param>
    public void ServerReadyCountDown(int time)
    {
        if (time > 0)
            SetReadyCountShow(true, time);
        else
            SetReadyCountShow(false, 0);
    }

    /// <summary>
    /// 聊天
    /// </summary>
    /// <param name="data"></param>
    public void ServerGetChat(SendReceiveGameChat data)
    {
        GameInteractionView view = Global.Inst.GetController<GameInteractionController>().OpenWindow() as GameInteractionView;
        eGameChatContentType chattype = (eGameChatContentType)data.chatType;
        int from;
        int to;
        Vector3 fpos;
        Vector3 tpos;
        switch (chattype)
        {
            case eGameChatContentType.HDFace://互动表情
                from = MJGameModel.Inst.mnewSeatToIndex[data.fromSeatId];
                to = MJGameModel.Inst.mnewSeatToIndex[data.toSeatId];
                fpos = mAllPlayer[from].GetChatPos(chattype);
                tpos = mAllPlayer[to].GetChatPos(chattype);
                view.AddOneInteractionFace(fpos, tpos, data);
                break;
            case eGameChatContentType.TexTVoice://文字语音
            case eGameChatContentType.Voice://语音
                from = MJGameModel.Inst.mnewSeatToIndex[data.fromSeatId];
                fpos = mAllPlayer[from].GetChatPos(chattype);
                view.AddOneChat(fpos, mAllPlayer[from].GetChatDirection(), data);
                break;
        }
    }


    /// <summary>
    /// 显示可操作列表
    /// </summary>
    /// <param name="info"></param>
    public void ServerShowInstructions(List<OptItemStruct> info, bool isRefreshHandCards)
    {
        //打哪张胡哪张
        if (isRefreshHandCards || MJGameModel.Inst.hasCanHuListCards != null && MJGameModel.Inst.hasCanHuListCards.Count != 0)//可以刷新或者是有可以胡牌的列表
            RefreshMyCards();
        mSelfPlayer.ShowInstructions(info);
    }

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
            //是否有叫的按钮
            if (data.seatId == MJGameModel.Inst.mMySeatId)
                SetChahuBtnShow(false);
            mMJSceneRoot.SetDeskCard(0, mContrl.mModel.mTouzi, false);//桌上的牌
            SetLeftCardNum(MJGameModel.Inst.mStartGameData.startInfo.leaveCardNum, true);
        }
        else if (data.ins == eMJInstructionsType.HIT && data.seatId == MJGameModel.Inst.mMySeatId)
        {
            //是否有叫的按钮
            SetChahuBtnShow(data.isHasJiao);
        }
        if (data.seatId != mContrl.mModel.mMySeatId)
            mSelfPlayer.ShowInstructions(null);
        mSelfPlayer.mSpeciaInstructionRoot.SetActive(false);
        mSelfPlayer.SetHuPromptCard(null, Vector3.zero, false);
        MJGameModel.Inst.mIsTing = false;//听牌
        //主动操作的
        if (index == mContrl.mModel.mMySeatId - 1)//自己
        {
            if (data.ins == eMJInstructionsType.HU || data.ins == eMJInstructionsType.YPDX)//如果是在金币房模式下赢了要显示换桌
                SetChangeDescBtnShow(eRoomState.START, mCurMode, true);
            int k = mContrl.mModel.mnewSeatToIndex[data.seatId];
            SpeciaeIns(data.ins, mSelfPlayer);
            InitSelfPlayerCards(mSelfPlayer, MJGameModel.Inst.mRoomPlayers[data.seatId], MJGameModel.Inst.allPlayersCardsInfoStruct[data.seatId],data);
            mMJSceneRoot.SetEffectShow(data, data.seatId, data.huType, data.subType);//特效
            SetCurCardEffect(mSelfPlayer, data.ins, mCurCardOffset);//打出的牌特效
        }
        else
        {
            int k = mContrl.mModel.mnewSeatToIndex[data.seatId];
            SpeciaeIns(data.ins, mAllPlayer[k]);
            InitOtherPlayerCards(mAllPlayer[k], MJGameModel.Inst.mRoomPlayers[data.seatId], MJGameModel.Inst.allPlayersCardsInfoStruct[data.seatId],data);
            mMJSceneRoot.SetEffectShow(data, data.seatId, data.huType, data.subType);//特效
            SetCurCardEffect(mAllPlayer[k], data.ins, mCurCardOffset);//打出的牌特效
        }
        //轮到谁操作
        mMJSceneRoot.SetLight(index + 1);
        SetAllPlayerCD(index + 1, 1);
        //被动操作的
        if (index != index2 && index2 >= 0)
        {
            if (data.otherSeatId == mContrl.mModel.mMySeatId)
                InitSelfPlayerCards(mSelfPlayer, MJGameModel.Inst.mRoomPlayers[data.otherSeatId], MJGameModel.Inst.allPlayersCardsInfoStruct[data.otherSeatId],data);
            else
            {
                int k = mContrl.mModel.mnewSeatToIndex[data.otherSeatId];
                InitOtherPlayerCards(mAllPlayer[k], MJGameModel.Inst.mRoomPlayers[data.otherSeatId], MJGameModel.Inst.allPlayersCardsInfoStruct[data.otherSeatId],data);
            }
        }

        InitAllHuTag(MJGameModel.Inst.allPlayersCardsInfoStruct);//刷新玩家  胡牌标记
        SetLeftTime(MJGameModel.Inst.TurnFixedTime, true);
    }

    /// <summary>
    /// 设置玩家cd
    /// </summary>
    /// <param name="seatid"></param>
    /// <param name="t"></param>
    private void SetAllPlayerCD(int seatid, float t)
    {

        if (mAllPlayer[mCurOptPlayerIndex] != null)//上一次cd设为0
            mAllPlayer[mCurOptPlayerIndex].SetCD(0);
        int index = MJGameModel.Inst.mnewSeatToIndex[seatid];
        mCurOptPlayerIndex = index;
        if (MJGameModel.Inst.mStartGameData != null && MJGameModel.Inst.mStartGameData.roomInfo.roomState > eRoomState.FIXEDCOLOR && MJGameModel.Inst.mStartGameData.roomInfo.roomState < eRoomState.GAMEOVER)//准备和结算之间
        {
            for (int i = 0; i < mAllPlayer.Count; i++)
            {
                if (mAllPlayer[i] != null)
                {
                    if (index == i)
                    {
                        mCurOptPlayerIndex = i;
                        mAllPlayer[i].SetCD(t);
                    }
                    else
                        mAllPlayer[i].SetCD(0);
                }
            }
        }
        else
        {
            for (int i = 0; i < mAllPlayer.Count; i++)
            {
                if (mAllPlayer[i] != null)
                {
                    mAllPlayer[i].SetCD(0);
                }
            }
        }
    }

    /// <summary>
    /// 设置某个玩家cd
    /// </summary>
    /// <param name="seatid"></param>
    /// <param name="t"></param>
    private void SetOnePlayerCD(int index, float t)
    {
        mCurOptPlayerIndex = index;
        if (mAllPlayer[index] != null && MJGameModel.Inst.mStartGameData != null && MJGameModel.Inst.mStartGameData.roomInfo.roomState > eRoomState.FIXEDCOLOR && MJGameModel.Inst.mStartGameData.roomInfo.roomState < eRoomState.GAMEOVER)//准备和结算之间)
            mAllPlayer[index].SetCD(t);
    }

    /// <summary>
    /// 设置一炮多响特效
    /// </summary>
    /// <param name="data"></param>
    public void SetYPDXEffUI(MJoptInfoData data)
    {
        mMJSceneRoot.SetYPDXEff(data);
    }

    /// <summary>
    /// 在线状态
    /// </summary>
    /// <param name="seatId">座位号</param>
    /// <param name="onlineState">状态</param>
    public void ServerOnlineState(int seatId, int onlineState)
    {
        eMJOnlineState state = (eMJOnlineState)onlineState;
        int index = MJGameModel.Inst.mnewSeatToIndex[seatId];
        if (state == eMJOnlineState.leave && seatId == MJGameModel.Inst.mMySeatId)//是自己离开返回俱乐部界面
            Global.Inst.GetController<MJGameController>().BackToClub();
        else
        {
            eRoomState roomstate = MJGameModel.Inst.mStartGameData.roomInfo.roomState;
            if (state == eMJOnlineState.leave && (roomstate == eRoomState.READY || roomstate == eRoomState.GAMEOVER))//如果是在准备状态或是游戏结束状态离开房间，隐藏掉玩家头像
            {
                mAllPlayer[index].GetOutRoom();
                mAllPlayer[index].SetOffLine(eMJOnlineState.online);
            }
            else
                mAllPlayer[index].SetOffLine(state);
        }
        //关闭准备倒计时
        SetReadyCountShow(false, 0);
    }

    /// <summary>
    /// 托管
    /// </summary>
    /// <param name="seatid">玩家id</param>
    /// <param name="trustType">托管类型：托管、取消托管</param>
    public void ServerPlayerTrustship(int seatid, eMJTrusteeshipType trustType)
    {
        int index = MJGameModel.Inst.mnewSeatToIndex[seatid];
        mAllPlayer[index].SetTrustShipTagShow(trustType == eMJTrusteeshipType.trust);
    }


    /// <summary>
    /// 小结算
    /// </summary>
    /// <param name="info"></param>
    public void ServerSettlement(MJGameSettlementInfo info)
    {
        mSelfPlayer.SetCurOutCardEffect(false);//隐藏打出牌的特效
        SetChangeDescBtnShow(eRoomState.READY, mCurMode);
        if (mGameSettlementView == null)
        {
            mGameSettlementView = GetWidget<MJGameSettlementView>("Windows/Majiang/GameUI/GameSettlementView", transform);
        }
        mGameSettlementView.gameObject.SetActive(true);
        mGameSettlementView.SetData(info, MJGameModel.Inst.mMaxMaxMul);
    }

    /// <summary>
    /// 大结算
    /// </summary>
    /// <param name="info"></param>
    public void ServerSettlementFinal(MJGameSettlementFinalInfo info)
    {
        if (mGameSettlementFinalView == null)
        {
            mGameSettlementFinalView = GetWidget<MJGameSettlementFinalView>("Windows/Majiang/GameUI/GameSettlementFinalView", transform);
        }
        mGameSettlementFinalView.gameObject.SetActive(true);
        mGameSettlementFinalView.SetData(info);
    }

    /// <summary>
    /// 流局
    /// </summary>
    public void ServerNoWinner()
    {
        mMJSceneRoot.SetNoWinnerShow();
    }
    #endregion

    #region 按钮点击
    /// <summary>
    /// 准备按钮点击
    /// </summary>
    /// <param name="btn"></param>
    public void OnReadyClick(UIButton btn)
    {
        OptRequest req = new OptRequest();
        req.ins = eMJInstructionsType.READY;
        mContrl.SendOptRequest(req);
        mReadyRoot.SetActive(false);
    }

    /// <summary>
    /// 邀请好友按钮
    /// </summary>
    /// <param name="btn"></param>
    public void OnInvitationClick(UIButton btn)
    {
        string rule = GamePatternModel.Inst.DeserializeRuleJosn(MJGameModel.Inst.mStartGameData.roomInfo.createData, false);
        SQDebug.Log(rule);
#if WECHAT
        SixqinSDKManager.Inst.InviteFriends(roomId, rule, cn.sharesdk.unity3d.PlatformType.WeChat);
#endif
    }


    /// <summary>
    /// 规则按钮点击
    /// </summary>
    public void OnRuleClick()
    {
        GameRoomInfoWidget view = GetWidget<GameRoomInfoWidget>(AssetsPathDic.GameRoomInfoWidget, transform);
        string s = GamePatternModel.Inst.DeserializeRuleJosn(MJGameModel.Inst.mStartGameData.roomInfo.createData, true, MJGameModel.Inst.mStartGameData.roomInfo.mode == "m4");
        view.ShowContent(s);
    }

    /// <summary>
    /// 语音按钮按下
    /// </summary>
    public void OnVoicePress()
    {
        mVoiceAnimation.gameObject.SetActive(true);
        mVoiceAnimation.SetBegin("yx_yuyin_0", 1, 5, 500000);
        string filePath = string.Format("{0}/{1}.amr", Application.persistentDataPath, DateTime.Now.ToFileTime());
        SQDebug.Log("FilePath:" + filePath);
#if YYVOICE
        YunVaImSDK.instance.RecordStartRequest(filePath, 1);
#endif
    }

    /// <summary>
    /// 语音按钮抬起来
    /// </summary>
    public void OnVoiceUp()
    {
        mVoiceAnimation.gameObject.SetActive(false);
#if YYVOICE
        uint mCurRecordTime = 0;
        YunVaImSDK.instance.RecordStopRequest((data1) =>
        {
            mCurRecordTime = data1.time;
            SQDebug.Log("停止录音返回:" + data1.strfilepath);
        },
          (data2) =>
          {
              SQDebug.Log("上传返回:" + data2.fileurl);
              MJGameController mCtr = Global.Inst.GetController<MJGameController>();
              SendReceiveGameChat req = new SendReceiveGameChat();
              req.chatType = (int)eGameChatContentType.Voice;
              req.content = data2.fileurl;
              req.voiceChatTime = (int) mCurRecordTime;
              req.fromSeatId = MJGameModel.Inst.mMySeatId;
              mCtr.SendGameChat(req);
          },
          (data3) =>
          {
              SQDebug.Log("识别返回:" + data3.text);
          });
#endif
    }

    /// <summary>
    /// 聊天表情点击
    /// </summary>
    public void OnChatClick()
    {
        if (mVoiceCDTime > 0)
            return;
        GameChatView view = Global.Inst.GetController<GameChatController>().OpenWindow() as GameChatView;
        view.SetData(OnTextChatClickCallback);
    }

    /// <summary>
    /// 文字聊天点击返回
    /// </summary>
    /// <param name="index"></param>
    private void OnTextChatClickCallback(int index)
    {
        MJGameController mCtr = Global.Inst.GetController<MJGameController>();
        int myseatid = MJGameModel.Inst.mMySeatId;
        SendReceiveGameChat req = new SendReceiveGameChat();
        req.chatType = (int)eGameChatContentType.TexTVoice;
        req.faceIndex = index;
        req.fromSeatId = MJGameModel.Inst.mMySeatId;
        req.sex = MJGameModel.Inst.mRoomPlayers[myseatid].sex;
        mCtr.SendGameChat(req);
        mVoiceCDTime = 3;
        mVoiceCDTimeLabel.gameObject.SetActive(true);
    }

    /// <summary>
    /// 查看胡什么牌按钮点击
    /// </summary>
    public void OnChaHuClick(GameObject mBG)
    {

        if (mBtnPromptRoot.gameObject.activeSelf)
        {
            mBtnPromptRoot.gameObject.SetActive(false);
        }
        else
        {
            OptRequest req = new OptRequest();
            req.ins = eMJInstructionsType.CHATING;
            mContrl.SendOptRequest(req, null, false);
        }
    }

    public void SetChaHu(List<CanHuStruct> canHuList)
    {
        int index = MJGameModel.Inst.mnewSeatToIndex[MJGameModel.Inst.mMySeatId];
        mAllPlayer[index].SetHuPromptCard(canHuList, new Vector3(-190, 84, 0), true);
    }

    /// <summary>
    /// 分享点击
    /// </summary>
    /// <param name="idex"></param>
    public void OnShareSettlement(GameObject obj)
    {
        if (obj.name.Equals("1"))//好友
        {
            //Global.It.mMissionCtr.mShareMissionType = 2;
            //SdkManager.instance.ShareGame(cn.sharesdk.unity3d.PlatformType.WeChat);
        }
        else//朋友圈
        {
            //Global.It.mMissionCtr.mShareMissionType = 1;
            //SdkManager.instance.ShareGame(cn.sharesdk.unity3d.PlatformType.WeChatMoments);

        }
        mShareRoot.SetActive(false);
    }

    /// <summary>
    /// 分享遮罩点击
    /// </summary>
    public void OnShareMaskClick()
    {
        mShareRoot.SetActive(false);
    }

    /// <summary>
    /// 菜单按钮点击
    /// </summary>
    public void OnMenuClick()
    {
        mMenuBtn.GetComponent<UISprite>().spriteName = "btn_more2";
        mMenuBtn.normalSprite = "btn_more2";
        GameMoreFunctionWidget view = BaseView.GetWidget<GameMoreFunctionWidget>(AssetsPathDic.GameMoreFunctionWidget, mContent);
        view.SetBtnItems(eGameMore.Setting, eGameMore.Distance, eGameMore.Leave);
        view.SetBtnCallBack(OnSettingClick, OnShowDisClick, OnLeaveClick);
        view.SetBtnCloseCall(() =>
        {
            mMenuBtn.GetComponent<UISprite>().spriteName = "btn_more1";
            mMenuBtn.normalSprite = "btn_more1";
            mMenuBtn.GetComponent<UISprite>().MakePixelPerfect();
        });
    }

    /// <summary>
    /// 设置按钮点击
    /// </summary>
    public void OnSettingClick()
    {
        if (mSettingView == null)
            mSettingView = GetWidget<SettingWidget>("Windows/MainView/SettingWidget", mContent);
        mSettingView.gameObject.SetActive(true);
        mSettingView.SetData(false);
    }

    /// <summary>
    /// 距离按钮点击
    /// </summary>
    public void OnShowDisClick()
    {
        Global.Inst.GetController<MJGameController>().SendGetPlayerDistances();
    }

    /// <summary>
    /// 离开按钮点击
    /// </summary>
    public void OnLeaveClick()
    {
        Global.Inst.GetController<CommonTipsController>().ShowTips("您确定要离开吗？", "取消|确定", () =>
        {

        }, () =>
        {
            eRoomState state = MJGameModel.Inst.mStartGameData.roomInfo.roomState;
            if (state > eRoomState.READY && state < eRoomState.GAMEOVER)//在游戏中且没有胡牌不能退出
            {
                if (MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].huList == null || MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].huList.Count == 0)
                {
                    Global.Inst.GetController<CommonTipsController>().ShowTips("还未胡牌，无法离开");
                    return;
                }
            }
            OptRequest req = new OptRequest();
            req.ins = eMJInstructionsType.EXITROOM;
            Global.Inst.GetController<MJGameController>().SendInstructions(req, null);
        }, null, "提示");
    }

    /// <summary>
    /// 换桌按钮点击
    /// </summary>
    public void OnChangeDeskClick()
    {
        Global.Inst.GetController<MJGameController>().SendChangeDesk();
    }
    #endregion

    #region 玩家信息界面
    public void SetPlayerInfoShow(PlayerInfoStruct info, int seatid, bool isself)
    {
        //if (mPlayerInfoView == null)
        //    mPlayerInfoView = GetWidget<LookUserInfoWidget>("Windows/GameCommonView/LookUserInfoWidget", transform);
        //mPlayerInfoView.gameObject.SetActive(true);
        //mPlayerInfoView.UpdateUserInfo(info.nickName, info.uId, "", info.headUrl, info.seatId, mMySeatId);
    }
    #endregion

    #region 分享

    /// <summary>
    /// 打开分享界面
    /// </summary>
    public void OpenShareContent()
    {
        mShareRoot.SetActive(true);
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


    /// <summary>
    /// 显示剩余时间
    /// </summary>
    private void SetLeftTime()
    {
        if (mLeftTime >= 2f && mLeftTime - Time.deltaTime < 2 && mContrl.mModel.mCurInsSeatId == mContrl.mModel.mMySeatId)//时间刚好小于2秒并且只有自己操作的时候才有提示音
            SoundProcess.PlaySound("daojishi");
        mLeftTime -= Time.deltaTime;
        int num = (int)(mLeftTime + 1);
        mCountDown.text = num >= 10 ? num.ToString() : "0" + num;

        if (MJGameModel.Inst.TurnFixedTime != 0)
            SetOnePlayerCD(mCurOptPlayerIndex, mLeftTime / MJGameModel.Inst.TurnFixedTime);
    }

    public void SetLeftTime(float t, bool isshow)
    {
        mCountDown.gameObject.SetActive(isshow);
        if (isshow)
            mLeftTime = t;
    }
    #endregion


    #region 特殊操作处理
    /// <summary>
    /// 特殊操作处理
    /// </summary>
    /// <param name="ins"></param>
    /// <param name="player"></param>
    private void SpeciaeIns(eMJInstructionsType ins, MJPlayerBase player)
    {

        if (ins == eMJInstructionsType.HU)//胡牌的时候把听牌标记去掉
        {
            mSelfPlayer.SetTingTag(false);
            for (int i = 0; i < mOtherPlayer.Count; i++)
                mOtherPlayer[i].SetTingTag(false);
        }
        //else if (ins == eMJInstructionsType.baojiao)//听牌要显示听牌标记
        //    player.SetTingTag(true);
    }
    #endregion

    /// <summary>
    /// 设置换桌按钮是否显示
    /// </summary>
    /// <param name="state"></param>
    /// <param name="mode"></param>
    private void SetChangeDescBtnShow(eRoomState state, string mode, bool isHu = false)
    {
        if (mode != "m4")//不是平台房就隐藏换桌按钮
            mChangeDeskBtn.SetActive(false);
        else
        {
            if (state == eRoomState.READY || state == eRoomState.GAMEOVER || isHu)//在准备或者结束时显示换桌按钮
                mChangeDeskBtn.SetActive(true);
            else
                mChangeDeskBtn.SetActive(false);
        }
    }

    /// <summary>
    /// 设置准备按钮是否显示
    /// </summary>
    /// <param name="isshow"></param>
    public void SetSelfPreShow(bool isshow)
    {
        mReadyRoot.SetActive(isshow);
        mReadyBtn.gameObject.SetActive(isshow);
    }

    public void GetGameStartFormCtr()
    {
        InfoAllPlayerData(MJGameModel.Inst.mStartGameData);
    }

    /// <summary>
    /// 设置 可以胡的牌的 番数 和 张数
    /// </summary>
    public void SetHuPromptCard(List<CanHuStruct> canHuList, Vector3 vec3, UIGrid mGrid = null)
    {
        int index = MJGameModel.Inst.mnewSeatToIndex[MJGameModel.Inst.mMySeatId];
        mAllPlayer[index].SetHuPromptCard(canHuList, new Vector3(-190, 84, 0), true);
    }

    /// <summary>
    /// 设置局数
    /// </summary>
    /// <param name="num">当前局数</param>
    /// <param name="allnum">总局数</param>
    public void SetRoomPlayNum(int num, bool isshow)
    {
        mPlayNum.transform.parent.gameObject.SetActive(false);
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
    /// 玩家托管状态
    /// </summary>
    /// <param name="player"></param>
    /// <param name="state"></param>
    private void InitOnePlayerTrustState(MJPlayerBase player, int state)
    {
        player.SetTrustShipTagShow((eMJTrusteeshipType)state == eMJTrusteeshipType.trust);
    }

    /// <summary>
    /// 玩家托管状态
    /// </summary>
    /// <param name="player"></param>
    /// <param name="state"></param>
    private void InitOnePlayerZhuangState(MJPlayerBase player, bool show)
    {
        player.SetZhuangState(show);
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
                if (item.seatId == MJGameModel.Inst.mMySeatId)
                {
                    //是否有叫的按钮
                    SetChahuBtnShow(item.isHasJiao);
                    InitOnePlayerHandCards(mAllPlayer[index], item.handList, item.cardNum, item.currCard);
                }
                else
                {
                    int cur = item.isHasCurrCard ? 1 : 0;
                    InitOnePlayerHandCards(mAllPlayer[index], null, item.cardNum, cur);
                }

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
    /// 初始化 玩家数据
    /// </summary>
    /// <param name="info"></param>
    public void InfoAllPlayerData(StartGameRespone info)
    {

        if (info.roomInfo.roomState == eRoomState.READY || info.roomInfo.roomState == eRoomState.GAMEOVER)// 不显示剩余牌数量
        {
            SetLeftCardNum(info.startInfo.leaveCardNum, false);
            mCountDown.gameObject.SetActive(false);
            SetLeftTime(MJGameModel.Inst.TurnFixedTime, false);
            SetAllPlayerCD(1, 0);
        }
        else
        {
            SetLeftCardNum(info.startInfo.leaveCardNum, true);
            //剩余时间
            SetLeftTime(info.roomInfo.turnLeaveTime, true);
            SetAllPlayerCD(MJGameModel.Inst.mCurInsSeatId, info.roomInfo.turnLeaveTime / MJGameModel.Inst.TurnFixedTime);
        }

        InitMJScene();
        //MJPlayerInfo pinfp = new MJPlayerInfo();

        roomId = MJGameModel.Inst.mRoomId.ToString();
        mCurMode = info.roomInfo.mode;
        //pinfp.playerListInfo = info.playerList;
        //准备
        InitReady(info);
        //换桌按钮
        List<HuStruct> hulist = MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].huList;
        SetChangeDescBtnShow(info.roomInfo.roomState, info.roomInfo.mode, hulist != null && hulist.Count > 0);//胡了也可以换桌
        //头像
        foreach (var item in info.roomInfo.playerList)
        {
            int index = MJGameModel.Inst.mnewSeatToIndex[item.seatId];
            InitOnePlayerIcon(mAllPlayer[index], item);
            SetPlayerSeatId(item.seatId, mAllPlayer[index]);//座位号///**********************
            InitOnePlayerOnlineState(mAllPlayer[index], item);//在线状态
            InitOnePlayerTrustState(mAllPlayer[index], item.tgType);//托管状态
            SetPlayerGiveUp(mAllPlayer[index], item.isGiveUp);//是否弃牌
            InitOnePlayerZhuangState(mAllPlayer[index],MJGameModel.Inst.mZhuangSeatId == item.seatId);
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
        //换三张和定缺
        InitChangeCards(info);
        //定缺标记
        InitAllFixeType(MJGameModel.Inst.allPlayersCardsInfoStruct, info.roomInfo.roomState);
        //设置 打牌指针 轮到谁操作
        mMJSceneRoot.SetLight(MJGameModel.Inst.mCurInsSeatId);
        //关闭准备倒计时
        // SetReadyCountShow(false, 0);
        ServerReadyCountDown((int)(MJGameModel.Inst.ReadyCountDownTime - Time.realtimeSinceStartup));
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
        int _roomId = info.roomInfo.mode == "m4" ? 0 : int.Parse(roomId);//如果是平台房就不显示房间号
        if (info.roomInfo.mode == "m4")//平台房隐藏按钮
        {
            mInvitationBtn.gameObject.SetActive(false);
            mVoiceBtn.SetActive(false);
            mMicBtn.SetActive(false);
        }
        mMJSceneRoot.SetDescInfo(999, int.Parse(roomId), info.roomInfo.currGameCount, info.roomInfo.maxGameCount, 9999);//  番数       低分  没有
        //上一次打出牌特效
        if (info.roomInfo.roomState == eRoomState.START && info.roomInfo.lastHitSeatId != 0)
        {
            int index = mContrl.mModel.mnewSeatToIndex[info.roomInfo.lastHitSeatId];
            SetCurCardEffect(mAllPlayer[index], eMJInstructionsType.HIT, mCurCardOffset);//打出的牌特效

        }
        SetRoomPlayNum(MJGameModel.Inst.mStartGameData.roomInfo.currGameCount, true);//设局数显示
        //我的操作列表
        mSelfPlayer.ShowInstructions(info.roomInfo.optList);
        //MJGameModel.Inst.mIsTing = false;//听牌
        MJGameModel.Inst.isFirstGetStartGameData = false;
        if (MJGameModel.Inst.mSycOptListResponse != null)
            mContrl.ChoiseRoomState(MJGameModel.Inst.mSycOptListResponse);
        if (MJGameModel.Inst.msgQueue.Count>0)
        {
            SQDebug.Log("队列消息 执行!!!");
            mContrl.ChoiseIns(MJGameModel.Inst.msgQueue.Dequeue());
        }


    }
    /// <summary>
    /// 换三张和定缺状态
    /// </summary>
    /// <param name="info"></param>
    private void InitChangeCards(StartGameRespone info)
    {
        #region 换三张
        if (info.roomInfo.roomState == eRoomState.CHANGETHREE)//换三张
        {
            for (int i = 0; i < info.roomInfo.playerList.Count; i++)
            {
                CardsInfoStruct item = info.startInfo.cardsInfoList[i];
                if (item != null)
                {
                    if (item.seatId == MJGameModel.Inst.mMySeatId)//自己
                    {
                        if (!item.isChanged)//未换三张
                        {
                            mMJChangeThree.IntoThreeWidget(true);
                            InitChaneThreeHandCards(mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[MJGameModel.Inst.mMySeatId]], item.changeTipCards);
                        }
                        else
                        {
                            mMJChangeThree.IntoThreeWidget(false);
                            mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[MJGameModel.Inst.mMySeatId]].ChangeThreeObj(true);
                        }
                    }
                    else
                    {
                        mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[item.seatId]].SetChangeStateShow(true);
                    }
                }
            }
        }
        #endregion
        #region 定缺
        else if (info.roomInfo.roomState == eRoomState.FIXEDCOLOR)//定缺
        {
            mMJChangeThree.IntoThreeWidget(false);// 隐藏自己换三张牌,否则重连时显示换三张牌
            mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[MJGameModel.Inst.mMySeatId]].ChangeThreeObj(false);
            for (int i = 0; i < info.roomInfo.playerList.Count; i++)
            {
                CardsInfoStruct item = info.startInfo.cardsInfoList[i];
                if (item != null)
                {
                    int index = mContrl.mModel.mnewSeatToIndex[item.seatId];
                    eFixedType _type = (eFixedType)item.fixedType;
                    if (item.seatId == MJGameModel.Inst.mMySeatId)
                    {
                        if (!item.isFixedColor)//未定缺
                            mMJFixeDcolor.IntoView((eFixedType)item.fixedType);
                        else
                            mAllPlayer[index].UpdataFixe(_type, item.seatId == MJGameModel.Inst.mMySeatId);
                    }
                    else
                        mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[item.seatId]].SetDingqueShow(true, "定缺中");
                }
            }
        }
        #endregion
        else//隐藏自己换三张牌
        {
            mMJChangeThree.IntoThreeWidget(false);
            mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[MJGameModel.Inst.mMySeatId]].ChangeThreeObj(false);
        }
    }



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
                    mReadyRoot.SetActive(!item.isReady);
                    mReadyBtn.gameObject.SetActive(!item.isReady);
                }
                mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[item.seatId]].SetReady(item.isReady);
            }
        }
        else
        {
            //全部不显示准备
            mReadyRoot.SetActive(false);
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
                    
                    InitOnePlayerHandCards(mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[item.seatId]], MJGameModel.Inst.allPlayersCardsInfoStruct[item.seatId].handList,0, MJGameModel.Inst.allPlayersCardsInfoStruct[item.seatId].currCard);
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
    public void UpdataHandCardsAfterGetMsg(Action filish = null)
    {
        int mySeatid = MJGameModel.Inst.mnewSeatToIndex[MJGameModel.Inst.mMySeatId];
        InitOnePlayerHandCards(mAllPlayer[mySeatid], MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].handList, MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].handList.Count, MJGameModel.Inst.allPlayersCardsInfoStruct[MJGameModel.Inst.mMySeatId].currCard);
        if (filish != null)
        {
            filish();
        }
    }

    /// <summary>
    /// 换3张  确定后的操作
    /// </summary>
    /// <param name="data"></param>
    public void SureChangeThreeUI(MJoptInfoData data)
    {
        mMJChangeThree.IntoThreeWidget(false);
        UpdataHandCardsAfterGetMsg(() =>
        {
            mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[MJGameModel.Inst.mMySeatId]].ChangeThreeObj(true);
            //换3张 翻牌动画
        }
    );
    }
    /// <summary>
    /// 播放 换牌动画
    /// </summary>
    public void PlayChangeThreeAnim(MJoptInfoData data)
    {
        foreach (var item in MJGameModel.Inst.mRoomPlayers)
        {
            if (item != null)
            {
                if (item.seatId != MJGameModel.Inst.mMySeatId)
                {
                    mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[item.seatId]].SetChangeStateShow(false);
                }
            }
        }
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
        for (int i = 0; i < MJGameModel.Inst.mRoomPlayers.Length; i++)
        {
            if (MJGameModel.Inst.mRoomPlayers[i] == null)
                continue;
            seatid = MJGameModel.Inst.mRoomPlayers[i].seatId;
            index = MJGameModel.Inst.mnewSeatToIndex[seatid];
            if (mAllPlayer[index] != null)
            {
                mAllPlayer[index].ChangeThreeObj(true);
            }
        }
        RotaCards(data.type);
        DelayRun(2f, () =>
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
        mMJFixeDcolor.gameObject.SetActive(false);
        eFixedType etype = (eFixedType)data.type;//定缺类型
        mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[MJGameModel.Inst.mMySeatId]].SetDingqueAnim(true, etype);
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
            mAllPlayer[MJGameModel.Inst.mnewSeatToIndex[data.fixedList[i].seatId]].UpdataFixe(data.fixedList[i].type, isMy, !isMy);
        }
    }
    /// <summary>
    /// 查叫 显示
    /// </summary>
    /// <param name="flowSemList"></param>
    public void GetChaJiaoData(List<FlowSemStruct> flowSemList)
    {
        #region 测试
        /*flowSemList = new List<FlowSemStruct>();
        for (int i = 0; i < 2; i++)
        {
            FlowSemStruct fs = new FlowSemStruct();
            fs.seatId = i + 1;
            fs.effList = new List<effStruct>();
            for (int j = 0; j < 3; j++)
            {
                effStruct es = new effStruct();
                es.type = 2 + j;
                es.scoreList = new List<ScoreStruct>();
                for (int k = 0; k < 4; k++)
                {
                    ScoreStruct ss = new ScoreStruct();
                    ss.seatId = k + 1;
                    ss.score = k == i ? -19 + j : (k + 1) * 10 + j;
                    es.scoreList.Add(ss);
                }
                fs.effList.Add(es);
            }
            flowSemList.Add(fs);
        }
        */
        #endregion
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
    /// <summary>
    /// 设置聊天时间
    /// </summary>
    private void SetChatCD()
    {
        if (mVoiceCDTime > 0)
        {
            mVoiceCDTime -= Time.deltaTime;
            if (mVoiceCDTime <= 0)
                mVoiceCDTimeLabel.gameObject.SetActive(false);
            else
                mVoiceCDTimeLabel.text = ((int)(mVoiceCDTime + 1)).ToString();
        }
    }

    void OnApplicationPause(bool b)
    {
        if (b)//最小化游戏
        {
            SQDebug.Log("程序失去焦点");
            NetProcess.ReleaseAllConnect();
            //GameManager.Instance.ResetConnetTime();
        }
    }
}
