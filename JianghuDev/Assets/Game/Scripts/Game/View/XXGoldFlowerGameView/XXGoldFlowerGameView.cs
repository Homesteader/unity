using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XXGoldFlowerGameView : BaseView {

    /// <summary>
    /// 自己
    /// </summary>
    public XXGlodFlowerSelfPlayer mSelfPlayer;

    /// <summary>
    /// 玩家数组 顺时针，从我开始放入数组中，我的永远是在索引0，其他玩家座位号-我的座位号>0是他的索引，<0则需要+5才是他的索引
    /// </summary>
    public XXGlodFlowerPlayer[] mPlayerArray;
    /// <summary>
    /// 美女荷官的发牌位置
    /// </summary>
    public GameObject mMeiNvHeGuanHand;

    /// <summary>
    /// 发牌的那张Item
    /// </summary>
    public GameObject mCastCardItem;

    /// <summary>
    /// 玩家基本信息节点
    /// </summary>
    public XXGlodFlowerBasePlayerInfo mPlayerBaseItem;

    /// <summary>
    /// 飞的那个金币
    /// </summary>
    public GameObject mFlyCoin;

    /// <summary>
    /// 飞金币的父节点,也是中间堆叠金币的的位置
    /// </summary>
    public GameObject mFlyCoinParent;

    /// <summary>
    /// 房间号
    /// </summary>
    public UILabel mRoomIdLabel;

    /// <summary>
    /// 倒计时显示
    /// </summary>
    public GameObject mLastTimeGo;

    /// <summary>
    /// 当前跟注
    /// </summary>
    public UILabel mCurGenZhu;

    /// <summary>
    /// 总底池
    /// </summary>
    public UILabel mTotalDiChi;

    /// <summary>
    /// 轮数
    /// </summary>
    public UILabel mRoundLabel;

    /// <summary>
    /// 最后一站动画
    /// </summary>
    public GameObject mLastFightAnim;

    /// <summary>
    /// 最后一战的结果
    /// </summary>
    public GameObject mLastFightResult;
    /// <summary>
    /// 底注
    /// </summary>
    public UILabel mDizhu;//底注
    /// <summary>
    /// 我进入房间开始输赢的金币
    /// </summary>
    public UILabel mAllPeachOfThisMatch;
    /// <summary>
    /// 全压的动画
    /// </summary>
    public GameObject mAllInTween;
    /// <summary>
    /// 孤注一掷的动画
    /// </summary>
    public UISprite mFuckItTween;
    /// <summary>
    /// 时间显示
    /// </summary>
    public UILabel mTimeLabel;

    public UILabel mDesolveTips;//解散房间提示
    public GameObject mChatBtn;//聊天按钮
    /// <summary>
    /// 倒计时
    /// </summary>
    private int mLastTime;

    /// <summary>
    /// 倒计时显示文字
    /// </summary>
    private string mLastTimeContent;

    /// <summary>
    /// 比牌界面
    /// </summary>
    private XXGoldFlowerPkCardWidget mPkWidget;

    /// <summary>
    /// 搓牌界面
    /// </summary>
    private XXGoldFlowerCuoCardWidget mCuoCardWidget;

    /// <summary>
    /// 孤注一掷比跑
    /// </summary>
    private XXGoldFlowerFuckItPkWidget mFuckItWidget;

    /// <summary>
    /// 座位号->玩家UI
    /// </summary>
    private Dictionary<int, XXGlodFlowerPlayer> mPayerDic = new Dictionary<int, XXGlodFlowerPlayer>();

    /// <summary>
    /// 金币池
    /// </summary>
    private Queue<GameObject> mFlyCoinQueue = new Queue<GameObject>();

    /// <summary>
    /// 下注金币
    /// </summary>
    private Queue<GameObject> mCathecticCoinQueue = new Queue<GameObject>();

    /// <summary>
    /// 发牌是否结束
    /// </summary>
    private bool mCastCardDown = true;

    /// <summary>
    /// 比牌是否结束
    /// </summary>
    private bool mCompareCardDown = true;

    /// <summary>
    /// 弃牌是否结束
    /// </summary>
    private bool mDisCardDown = true;

    /// <summary>
    /// 最后一战是否结束
    /// </summary>
    private bool mLastFightDown = true;

    private GoldSettlementView mSettlementView;//小结算界面
    private float mDisolveTime;//解散时间
    private const string mDisolveText = "房间{0}秒内无玩家加入自动解散";

    #region Unity 

    protected override void Awake()
    {
        base.Awake();
        SoundProcess.PlayMusic("xyjZJHBG");
        mChatBtn.SetActive(PlayerModel.Inst.UserInfo.IsUseChat);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SoundProcess.PlayMusic("BGSOUND");
    }

    protected override void Update()
    {
        mTimeLabel.text = string.Format("{0}年{1}月{2}日{3:D2}:{4:D2}:{5:D2}", System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day,System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
        if(mDisolveTime > Time.realtimeSinceStartup)
        {
            if (!mDesolveTips.gameObject.activeInHierarchy)
                mDesolveTips.transform.parent.gameObject.SetActive(true);

            mDesolveTips.text = string.Format(mDisolveText, (int)(mDisolveTime - Time.realtimeSinceStartup));
        }
        else
        {
            if (mDesolveTips.gameObject.activeInHierarchy)
                mDesolveTips.transform.parent.gameObject.SetActive(false);
        }
    }

    #endregion

    #region 游戏逻辑


    /// <summary>
    /// 创建或者加入房间
    /// </summary>
    /// <param name="view"></param>
    public void ServerCreateJoinGame(GoldFlowerCreateRoomData data) {
        StopAllCoroutines();
        CleanAllDesk();
        if (data.roomInfo.goldPattern) {//金币场
            mRoomIdLabel.text = "房间号：" + data.roomInfo.roomId;
        }
        else {
            mRoomIdLabel.text = "房间号：" + data.roomInfo.roomId;
        }
        //底注
        mDizhu.text = XXGoldFlowerGameModel.Inst.mRoomRules.baseScore.ToString();
        //解散时间
        mDisolveTime = data.roomInfo.autoDestoryTime + Time.realtimeSinceStartup;
        for (int i = 0; i < data.roomInfo.playerList.Count; i++)
        {
            GoldFlowerPlayer player = data.roomInfo.playerList[i];
            PlayerSeatDown(player);


            if ((eGFGameState)data.roomInfo.gameState == eGFGameState.Ready || (eGFRoomState)data.roomInfo.roomState == eGFRoomState.NoStart)
            {
                UpdateCurGenZhu(0);
                XXGlodFlowerPlayer playerUI = null;
                if (TryGetPlayer(player.seatId, out playerUI))
                {
                    playerUI.UpdateCathecticCoin("");
                }
            }
            else
            {
                if (player.curGenZhu > 0)
                {
                    UpdateCurGenZhu(player.curGenZhu);
                }
            }
        }

        if ((eGFGameState)data.roomInfo.gameState == eGFGameState.Ready || (eGFRoomState)data.roomInfo.roomState == eGFRoomState.NoStart)
        {
            mSelfPlayer.HideSixBtnState();
            mSelfPlayer.SetAddBtnItemState(false);
            mSelfPlayer.SetShowCardBtnState(false);
            UpdateTotalDiChi(0);
            NetOnUpdateRound(0);
            if (data.roomInfo.goldPattern)//金币场
            {
                mSelfPlayer.SetInvateBtnState(false);
                //mSelfPlayer.SetChangDeskBtnState(true);
            }
            else
            {//好友长
                mSelfPlayer.SetInvateBtnState(true);
                //mSelfPlayer.SetChangDeskBtnState(false);
                //if(data.roomInfo.canLookCard)
                mSelfPlayer.SetStartGameBtn(true);
            }

            if (XXGoldFlowerGameModel.Inst.mPlayerInfoDic[data.roomInfo.mySeatId].isReady) {
                mSelfPlayer.SetReadybtnState(false);
            }
            else {
                mSelfPlayer.SetReadybtnState(true);
            }
        }
        else {
            mSelfPlayer.SetReadybtnState(false);
            mSelfPlayer.SetInvateBtnState(false);
            mSelfPlayer.SetChangDeskBtnState(false);
            mSelfPlayer.SetShowCardBtnState(false);
            mSelfPlayer.SetStartGameBtn(false);

            UpdateTotalDiChi(data.roomInfo.dichi);
            NetOnUpdateRound(data.roomInfo.round);
            if (data.roomInfo.handCardsList != null && data.roomInfo.handCardsList.otherCardsInfo != null)//其他玩家手牌
            {
                for (int i = 0; i < data.roomInfo.handCardsList.otherCardsInfo.Count; i++)
                {
                    GoldFlowerHandCardsInfo cards = data.roomInfo.handCardsList.otherCardsInfo[i];
                    XXGlodFlowerPlayer player = null;
                    if (TryGetPlayer(cards.seatId, out player) && cards.cards!=null)
                    {
                        if (!XXGoldFlowerGameModel.Inst.mHasCardSeatList.Contains(cards.seatId)) {
                            XXGoldFlowerGameModel.Inst.mHasCardSeatList.Add(cards.seatId);
                        }
                        InstantiateCards(player, cards.cards, (eGFCardType)cards.cardType);
                        if (cards.kpState == true)
                        {//看了牌
                            player.SetLookCarsState(true);
                        }
                        if (XXGoldFlowerGameModel.Inst.mPlayerInfoDic[cards.seatId].discard)
                        {//弃了牌的
                            UIGrid grid = player.GetHandCardParent();
                            List<Transform> list = grid.GetChildList();
                            for (int k=0;k<list.Count;k++) {
                                list[k].GetComponent<UIWidget>().alpha = 0.0f;
                            }
                            player.SetLookCarsState(false);
                            player.SetDisCardState(true);
                            XXGoldFlowerGameModel.Inst.mHasCardSeatList.Remove(cards.seatId);

                        }
                    }
                }
            }

            if (data.roomInfo.handCardsList != null && data.roomInfo.handCardsList.myCardsInfo != null && data.roomInfo.handCardsList.myCardsInfo.cards != null)
            {//自己有手牌
                XXGoldFlowerGameModel.Inst.mGameed = true;
                XXGlodFlowerPlayer player = null;
                if (TryGetPlayer(data.roomInfo.mySeatId, out player))
                {
                    if (!XXGoldFlowerGameModel.Inst.mHasCardSeatList.Contains(data.roomInfo.mySeatId)) {
                        XXGoldFlowerGameModel.Inst.mHasCardSeatList.Add(data.roomInfo.mySeatId);
                    }
                    InstantiateCards(player, data.roomInfo.handCardsList.myCardsInfo.cards, (eGFCardType)data.roomInfo.handCardsList.myCardsInfo.cardType);
                    if (data.roomInfo.handCardsList.myCardsInfo.kpState)
                    {
                        XXGoldFlowerGameModel.Inst.mLookCard = data.roomInfo.handCardsList.myCardsInfo.kpState;
                    }
                    if (XXGoldFlowerGameModel.Inst.mPlayerInfoDic[data.roomInfo.mySeatId].discard)
                    {//弃了牌的
                        UIGrid grid = player.GetHandCardParent();
                        List<Transform> list = grid.GetChildList();
                        for (int k = 0; k < list.Count; k++)
                        {
                            list[k].GetComponent<UIWidget>().alpha = 0.0f;
                        }
                        player.SetLookCarsState(false);
                        player.SetDisCardState(true);
                        XXGoldFlowerGameModel.Inst.mHasCardSeatList.Remove(data.roomInfo.mySeatId);
                    }
                }
                if (data.roomInfo.canLookCard)
                {
                    XXGoldFlowerGameModel.Inst.mCanLookCard = true;
                    mSelfPlayer.SetLookCardBtnState(true);
                    mSelfPlayer.SetCuoCardBtnState(true);
                }
                ManagerOptBtn(data.roomInfo.mySeatId);
            }

            if (data.roomInfo.opt != null && data.roomInfo.opt.optList != null && data.roomInfo.turnSeatId == data.roomInfo.mySeatId)//显示操作指令
            {
                NetOnSelfGameOpt(data.roomInfo.opt);
            }


        }
    }
    
    /// <summary>
    /// 有玩家离开房间
    /// </summary>
    /// <param name="seatId"></param>
    public void NetOnPlayerLeaveRoom(int seatId) {

        GameObject widget = null;
        if (BaseView.childrenWidgetDic.TryGetValue(typeof(GameUserInfoWidget).Name,out widget)) {
            if (widget!=null) {
                GameUserInfoWidget infoWidget = widget.GetComponent<GameUserInfoWidget>();
                if (infoWidget != null)
                {
                    if (infoWidget.GetSeatId() == seatId)
                    {
                        BaseViewWidget.CloseWidget<GameUserInfoWidget>();
                    }
                }
            }
        } 

        XXGlodFlowerPlayer player = null;
        if (TryGetPlayer(seatId, out player))
        {
            player.CleanHandCards();
            player.CleanPlayer();
            mPayerDic.Remove(seatId);
        }
    }

    /// <summary>
    /// 有玩家坐下
    /// </summary>
    /// <param name="player"></param>
    public void NetOnPlayerSeatDown(GoldFlowerPlayer player) {
        PlayerSeatDown(player);
    }

    /// <summary>
    /// 有玩家准备
    /// </summary>
    /// <param name="seatId"></param>
    public void NetOnPlayerReady(int seatId) {
        XXGlodFlowerPlayer player = null;
        if (TryGetPlayer(seatId, out player)) {
            player.SetReadyState(true);
        }
        if (seatId == XXGoldFlowerGameModel.Inst.mMySeatId)
        {
            mSelfPlayer.SetReadybtnState(false);
            if (mSettlementView != null)
                mSettlementView.Close<GoldSettlementView>();
        }
    }

    /// <summary>
    /// 游戏开始倒计时
    /// </summary>
    /// <param name="time"></param>
    public void NetOnGameStartLastTime(int time) {
        if (time < 0) {
            mLastTime = 0;
            mLastTimeGo.gameObject.SetActive(false);
            Global.Inst.GetController<CommonTipsController>().ShowTips("游戏未能开始");
        }
        else
        {
            ShowLastTime("游戏即将开始", time);
        }

    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    public void NetOnGameStart() {
        StopAllCoroutines();
        mSelfPlayer.SetInvateBtnState(false);
        mSelfPlayer.SetShowCardBtnState(false);
        mSelfPlayer.SetChangDeskBtnState(false);
        mSelfPlayer.SetReadybtnState(false);
        mSelfPlayer.SetAutoBtnState(false);
        mSelfPlayer.SetStartGameBtn(false);
        mDisolveTime = 0;
        CleanDesk();
        for (int i=0;i<XXGoldFlowerGameModel.Inst.mSeatIdList.Count;i++) {
            XXGlodFlowerPlayer player = null;
            if (TryGetPlayer(XXGoldFlowerGameModel.Inst.mSeatIdList[i],out player)) {
                player.CleanHandCards();
                player.SetDisCardState(false);
                player.SetLookCarsState(false);
                player.UpdateCathecticCoin("");
                player.SetZhuangState(false);
                if (XXGoldFlowerGameModel.Inst.mZhuangSeatId == XXGoldFlowerGameModel.Inst.mSeatIdList[i]) {
                    player.SetZhuangState(true);
                }
            }
        }
        NetOnUpdateRound(1);
        ShowLastTime("", 0);
    }

    /// <summary>
    /// 同步玩家操作结果
    /// </summary>
    /// <param name="data"></param>
    public void NetOnGameOptResult(OnGoldFlowerPlayerOptResult data) {
        if ((eGFOptIns)data.ins!= eGFOptIns.LookCard && (eGFOptIns)data.ins!= eGFOptIns.DisCard) {
            UpdateCurGenZhu(data.TotalXiaZhu);
            UpdateTotalDiChi(data.dichi);
        }

        if (XXGoldFlowerGameModel.Inst.mHasCardSeatList.Contains(XXGoldFlowerGameModel.Inst.mMySeatId) && 
            data.seatId == XXGoldFlowerGameModel.Inst.mMySeatId && 
            (eGFOptIns)data.ins!= eGFOptIns.LookCard) {

            XXGoldFlowerGameModel.Inst.mComparingCard = false;
            for (int i = 0; i < XXGoldFlowerGameModel.Inst.mSeatIdList.Count; i++)
            {
                XXGlodFlowerPlayer player = null;
                int seatId = XXGoldFlowerGameModel.Inst.mSeatIdList[i];
                if (TryGetPlayer(seatId, out player))
                {
                    player.SetCompareState(false);

                }
            }
            mSelfPlayer.SetAddBtnItemState(false);
            if ((eGFOptIns)data.ins != eGFOptIns.Compare) {
                ManagerOptBtn(data.seatId);
            }
        }

        if (data.seatId == XXGoldFlowerGameModel.Inst.mMySeatId) {
            //if ((eGFOptIns)data.ins == eGFOptIns.LookCard && XXGoldFlowerGameModel.Inst.mCuoCarding) {
            //    XXGoldFlowerGameModel.Inst.mSelfLookCard = data;
            //    //打开错牌界面
            //    if (mCuoCardWidget == null) {
            //        mCuoCardWidget = BaseView.GetWidget<XXGoldFlowerCuoCardWidget>(AssetsPathDic.XXGoldFlowerCuoCardWidget, transform);
            //    }
            //    mCuoCardWidget.ShowCuoCards(false, data.cards, () =>
            //    {
            //        OnPlayerLookCards(data);
            //        mSelfPlayer.SetCuoCardBtnState(false);
            //    });
            //    return;
            //}

            if ((eGFOptIns)data.ins == eGFOptIns.LookCard || (eGFOptIns)data.ins == eGFOptIns.DisCard ||
                (eGFOptIns)data.ins == eGFOptIns.Compare) {
                mSelfPlayer.SetCuoCardBtnState(false);
                BaseViewWidget.CloseWidget<XXGoldFlowerCuoCardWidget>();
                mCuoCardWidget = null;
            }
            if((eGFOptIns)data.ins != eGFOptIns.LookCard)//如果不是看牌就隐藏加倍
                mSelfPlayer.SetAddGridShow(false);
        }

        switch ((eGFOptIns)data.ins) {
            case eGFOptIns.DiZhu://下底注
                OnPlayerDiZhu(data);
                HideTurnSp(data.seatId);
                break;
            case eGFOptIns.AllIn://全压
                OnPlayerAddBet(data);
                UpdatePlayerScore(data.seatId, data.lastScore);
                HideTurnSp(data.seatId);
                ShowAllInTween();
                break;
            case eGFOptIns.Follow://跟注
                OnPlayerAddBet(data);
                UpdatePlayerScore(data.seatId, data.lastScore);
                HideTurnSp(data.seatId);
                break;
            case eGFOptIns.Add://加注
                OnPlayerAddBet(data);
                UpdatePlayerScore(data.seatId, data.lastScore);
                HideTurnSp(data.seatId);
                break;
            case eGFOptIns.DisCard://弃牌
                StartCoroutine(OnPlayerDisCard(data.seatId, data.disType == 0));
                HideTurnSp(data.seatId);
                break;
            case eGFOptIns.LookCard://看牌
                OnPlayerLookCards(data);
                break;
            case eGFOptIns.JustFuck://孤注一掷
                OnPlayerAddBet(data);
                OnPlayerFuckIt(data);
                UpdatePlayerScore(data.seatId, data.lastScore);
                HideTurnSp(data.seatId);
                ShowFuckItTween();
                break;
            case eGFOptIns.Compare://比牌
                OnPlayerAddBet(data);
                OnPlayerCompareCard(data);
                UpdatePlayerScore(data.seatId, data.lastScore);
                HideTurnSp(data.seatId);
                break;
        }
    }

    /// <summary>
    /// 自己可操作列表
    /// </summary>
    /// <param name="data"></param>
    public void NetOnSelfGameOpt(GoldFlowerOpt data) {
        if (XXGoldFlowerGameModel.Inst.mGameed == false)
        {
            return;
        }
        
        ManagerOptBtn(XXGoldFlowerGameModel.Inst.mMySeatId);
        if (data.optList!=null) {
            SQDebug.Log("自己的指令个数为:" + data.optList.Count);
            for (int i = 0; i < data.optList.Count; i++)
            {
                switch ((eGFOptIns)data.optList[i])
                {
                    case eGFOptIns.AllIn://全压
                        mSelfPlayer.SetAllInBtnState(true);
                        break;
                    case eGFOptIns.Follow://跟注
                        mSelfPlayer.SetFollowBtnState(true);
                        break;
                    case eGFOptIns.DisCard://弃牌
                        mSelfPlayer.SetDisCardBtnState(true);
                        break;
                    case eGFOptIns.LookCard://看牌
                        XXGoldFlowerGameModel.Inst.mCanLookCard = true;
                        mSelfPlayer.SetLookCardBtnState(true);
                        mSelfPlayer.SetCuoCardBtnState(true);
                        break;
                    case eGFOptIns.Add://加注
                        mSelfPlayer.SetJiazhuBtnState(true);
                        break;
                    case eGFOptIns.JustFuck://孤注一掷
                        mSelfPlayer.SetJustFuckBtnState(true);
                        break;
                    case eGFOptIns.Compare://比牌
                        mSelfPlayer.SetCompareCardBtnState(true);
                        break;
                }
            }
            mSelfPlayer.SetDisCardBtnState(true);
        }
    }

    /// <summary>
    /// 轮到谁操作
    /// </summary>
    /// <param name="seatId"></param>
    public void TurnOptSeat(int oldSeatId,int seatId,int time) {
        if (seatId <= 0)
        {
            return;
        }
        else {
            XXGlodFlowerPlayer oldPlayer = null;
            XXGlodFlowerPlayer newPlayer = null;

            if (oldSeatId == XXGoldFlowerGameModel.Inst.mMySeatId) {
                mSelfPlayer.SetLookCardBtnState(false);
                mSelfPlayer.SetCuoCardBtnState(false);
                mSelfPlayer.SetCompareCardBtnState(false);
                mSelfPlayer.SetShowCardBtnState(false);
                mSelfPlayer.SetDisCardBtnState(false);
            }


            if (TryGetPlayer(oldSeatId,out oldPlayer)) {
                oldPlayer.ShowLastTime(0);
            }
            if (TryGetPlayer(seatId, out newPlayer))
            {
                newPlayer.ShowLastTime(time);
            }

            SQDebug.Log("当前轮到座位号：" + seatId + "操作");
        }
    }

    /// <summary>
    /// 小结算
    /// </summary>
    /// <param name="data"></param>
    public void NetOnSmallSettle(GoldSettlementData data) {
        for (int i=0;i<XXGoldFlowerGameModel.Inst.mSeatIdList.Count;i++) {
            HideTurnSp(XXGoldFlowerGameModel.Inst.mSeatIdList[i]);
        }
        mSelfPlayer.HideSixBtnState();
        mSelfPlayer.SetAddBtnItemState(false);
        mSelfPlayer.SetCuoCardBtnState(false);
        StartCoroutine(OnSmallSettle(data));
    }

    /// <summary>
    /// 玩家上下线处理
    /// </summary>
    /// <param name="data"></param>
    public void NetOnOnOffLine(OnGoldFlowerOnOffLine data) {
        XXGlodFlowerPlayer player = null;
        if (TryGetPlayer(data.seatId,out player)) {
            player.SetOffLineState(data.state == 1 ? false : true);
        }
    }

    /// <summary>
    /// 有玩家亮牌
    /// </summary>
    /// <param name="data"></param>
    public void NetOnPlayerShowCards(OnGoldFlowerShowCard data) {
        XXGlodFlowerPlayer player = null;
        if (TryGetPlayer(data.seatId, out player)) {
            UIGrid grid = player.GetHandCardParent();
            List<Transform> list = grid.GetChildList();
            if (list.Count>0) {
                player.SetHandCard(data.cards);
                player.SetHandCardType((eGFCardType)data.cardType);
                ShowOnePlayerCard(data.seatId);
            }
            if (data.seatId == XXGoldFlowerGameModel.Inst.mMySeatId) {
                mSelfPlayer.SetShowCardBtnState(false);
            }
        }
    }

    /// <summary>
    /// 更新轮数
    /// </summary>
    /// <param name="round"></param>
    public void NetOnUpdateRound(int round) {
        mRoundLabel.text = "轮数："+round;
    }

    /// <summary>
    /// 游戏聊天
    /// </summary>
    /// <param name="chat"></param>
    public void NetOnGameTalk(SendReceiveGameChat chat) {
        XXGlodFlowerPlayer player = null;
        if (TryGetPlayer(chat.fromSeatId,out player)) {
            player.ServerGameChat(chat);
        }
    }

    /// <summary>
    /// 最后一站
    /// </summary>
    /// <param name="data"></param>
    public void NetOnLastFight(OnGetGoldFlowerLastFight data) {
        mLastFightDown = false;
        StartCoroutine(OnLastFight(data));
    }

    /// <summary>
    /// 自动换桌
    /// </summary>
    public void NetOnAutoChangDesk() {
        mSelfPlayer.SetChangDeskBtnState(false);
        mSelfPlayer.SetChanagDeskBtnState(false);
        mSelfPlayer.SetReadyBtnState(false);
    }


    /// <summary>
    /// 清理桌面
    /// </summary>
    public void CleanDesk() {
        //清除桌面上的金币
        int len = mFlyCoinParent.transform.childCount;
        for(int i = 0; i < len; i++)
        {
            mFlyCoinParent.transform.GetChild(i).gameObject.SetActive(false);
        }
        XXGoldFlowerGameModel.Inst.mComparingCard = false;
        mSelfPlayer.SetShowCardBtnState(false);
        mSelfPlayer.SetAutoBtnState(false);
        mSelfPlayer.SetCuoCardBtnState(false);
        XXGlodFlowerPlayer player = null;
        int seatId = 0;
        for (int i=0;i<XXGoldFlowerGameModel.Inst.mSeatIdList.Count;i++) {
            seatId = XXGoldFlowerGameModel.Inst.mSeatIdList[i];
            if (TryGetPlayer(seatId,out player)) {
                player.CleanHandCards();
                player.SetDisCardState(false);
                player.SetLookCarsState(false);
                player.UpdateCathecticCoin("");
                player.ShowLastTime(0);
                player.SetCompareState(false);
                player.SetZhuangState(false);
                player.SetInsTips(eGFOptIns.AllIn);
            }
        }
        NetOnUpdateRound(0);
        UpdateCurGenZhu(0.0f);
        UpdateTotalDiChi(0.0f);
        BaseViewWidget.CloseWidget<XXGoldFlowerCuoCardWidget>();
        mCuoCardWidget = null;

        BaseViewWidget.CloseWidget<XXGoldFlowerFuckItPkWidget>();
        mFuckItWidget = null;

        BaseViewWidget.CloseWidget<XXGoldFlowerPkCardWidget>();
        mPkWidget = null;

        mDisCardDown = true;
        mCompareCardDown = true;
        mCastCardDown = true;
        mLastFightDown = true;
    }

    /// <summary>
    /// 清除桌面所有数据
    /// </summary>
    public void CleanAllDesk() {
        CleanDesk();
        List<int> list = new List<int>();
        list.AddRange(mPayerDic.Keys);
        for (int i=0;i< list.Count;i++) {
            XXGlodFlowerPlayer player = null;
            if (TryGetPlayer(list[i],out player)) {
                player.CleanHandCards();
                player.CleanPlayer();
            }
        }

        for (int i = 0; i < mCathecticCoinQueue.Count;i++) {
            GameObject go = mCathecticCoinQueue.Dequeue();
            go.gameObject.SetActive(false);
            mFlyCoinQueue.Enqueue(go);
        }

        mPayerDic.Clear();
        ShowLastTime("", 0);
        mSelfPlayer.SetAddBtnItemState(false);
        mSelfPlayer.HideSixBtnState();
    }

    #endregion

    #region 同步玩家操作结果

    /// <summary>
    /// 同步玩家下底注
    /// </summary>
    /// <param name="data"></param>
    private void OnPlayerDiZhu(OnGoldFlowerPlayerOptResult data) {

        for (int i=0;i<data.lastScores.Count;i++) {
            if (!XXGoldFlowerGameModel.Inst.mHasCardSeatList.Contains(data.lastScores[i].seatId)) {
                XXGoldFlowerGameModel.Inst.mHasCardSeatList.Add(data.lastScores[i].seatId);
            }
        }

        if (XXGoldFlowerGameModel.Inst.mHasCardSeatList.Contains(XXGoldFlowerGameModel.Inst.mMySeatId))//自己有手牌
        {
            XXGoldFlowerGameModel.Inst.mComparingCard = false;
            XXGoldFlowerGameModel.Inst.mGameed = true;
            ManagerOptBtn(XXGoldFlowerGameModel.Inst.mMySeatId);
        }
        else {
            mSelfPlayer.HideSixBtnState();
        }

        CallBack call = new CallBack(() => {
            for (int i = 0; i < data.lastScores.Count; i++)
            {
                XXGlodFlowerPlayer player = null;
                int seatId = data.lastScores[i].seatId;
                if (seatId == XXGoldFlowerGameModel.Inst.mMySeatId) {
                    XXGoldFlowerGameModel.Inst.mGameed = true;
                }
                if (TryGetPlayer(seatId, out player))
                {
                    player.SetDisCardState(false);
                    player.SetLookCarsState(false);
                    player.SetReadyState(false);
                    player.UpdateCathecticCoin("");
                    player.SetUserScore(data.lastScores[i].lastScore);
                    player.UpdateCathecticCoin(data.TotalXiaZhu + "");
                    FlyCoin(seatId, 1, true, data.TotalXiaZhu);
                }
            }
        });

        StartCoroutine(IECastCard(call));
    }

    /// <summary>
    /// 同步玩家跟注 加注 全压 孤注一掷
    /// </summary>
    /// <param name="data"></param>
    private void OnPlayerAddBet(OnGoldFlowerPlayerOptResult data) {
        XXGlodFlowerPlayer player = null;
        int seatId = data.seatId;
        if (TryGetPlayer(seatId, out player))
        {
            SoundProcess.PlaySound("jh/" + GetInsSound(data));
            player.UpdateCathecticCoin(data.curXiaZhu + "");
            if (data.lastScores != null)
            {
                for (int i = 0; i < data.lastScores.Count; i++)
                {
                    if (data.lastScores[i].seatId == data.seatId)
                    {
                        player.SetUserScore(data.lastScores[i].lastScore);
                    }
                }
            }
            FlyCoin(seatId, 1, true, data.TotalXiaZhu);
            ManagerOptBtn(seatId);
            player.SetInsTips((eGFOptIns)data.ins);
        }
    }

    /// <summary>
    /// 同步玩家孤注一掷
    /// </summary>
    /// <param name="data"></param>
    private void OnPlayerFuckIt(OnGoldFlowerPlayerOptResult data) {
        mCompareCardDown = false;
        BaseViewWidget.CloseWidget<XXGoldFlowerCuoCardWidget>();
        mCuoCardWidget = null;

        if (mFuckItWidget==null) {
            mFuckItWidget = BaseView.GetWidget<XXGoldFlowerFuckItPkWidget>(AssetsPathDic.XXGoldFlowerFuckItPkWidget, transform);
        }
        mFuckItWidget.SetPlayers(data.seatId, data.pkSeatId);
        mFuckItWidget.StartPk(data.win, () =>
        {
            if (XXGoldFlowerGameModel.Inst.mSelfLookCard != null)
            {
                OnPlayerLookCards(XXGoldFlowerGameModel.Inst.mSelfLookCard);
                mSelfPlayer.SetCuoCardBtnState(false);
            }
            mCompareCardDown = true;
        });
    }

    /// <summary>
    /// 同步玩家看牌
    /// </summary>
    /// <param name="data"></param>
    private void OnPlayerLookCards(OnGoldFlowerPlayerOptResult data) {
        XXGlodFlowerPlayer player = null;

        if (TryGetPlayer(data.seatId, out player)) {
            SoundProcess.PlaySound("jh/" + GetInsSound(data));
            player.SetLookCarsState(true);
            if (data.seatId == XXGoldFlowerGameModel.Inst.mMySeatId)
            {
                player.SetHandCard(data.cards);
                player.SetHandCardType((eGFCardType)data.cardType);
                ShowOnePlayerCard(data.seatId);
                mSelfPlayer.SetLookCardBtnState(false);
            }
            player.SetInsTips((eGFOptIns)data.ins);
        }
        if (data.seatId == XXGoldFlowerGameModel.Inst.mMySeatId) {//看了牌之后更新加注选项
            XXGoldFlowerGameModel.Inst.mCanLookCard = false;
            XXGoldFlowerGameModel.Inst.mLookCard = true;
            if (XXGoldFlowerGameModel.Inst.mTurnSeatId == XXGoldFlowerGameModel.Inst.mMySeatId) {//当前轮到我操作
                if (XXGoldFlowerGameModel.Inst.mOpt!=null && XXGoldFlowerGameModel.Inst.mOpt.jiazhuList!=null) {
                    mSelfPlayer.SetAddBtnItemShowWhenActive();
                }
            }
        }
    }

    /// <summary>
    /// 同步玩家弃牌
    /// </summary>
    /// <param name="data"></param>
    private IEnumerator OnPlayerDisCard(int seatId, bool isGiveup = true) {


        GoldFlowerPlayer modePlayer = null;

        XXGoldFlowerGameModel.Inst.mHasCardSeatList.Remove(seatId);

        if (XXGoldFlowerGameModel.Inst.mPlayerInfoDic.TryGetValue(seatId, out modePlayer))
        {
            modePlayer.discard = true;
        }

        yield return new WaitUntil(() =>
        {
            return mCompareCardDown == true;
        });
        XXGlodFlowerPlayer player = null;
        if (TryGetPlayer(seatId, out player))
        {
            OnGoldFlowerPlayerOptResult  data= new OnGoldFlowerPlayerOptResult();
            data.seatId = seatId;
            data.ins = (int)eGFOptIns.DisCard;
            if(isGiveup)
                SoundProcess.PlaySound("jh/" + GetInsSound(data));

            player.SetDisCardState(true);
            player.SetLookCarsState(false);
            player.SetHandCardType(eGFCardType.Nil);
            UIGrid grid = player.GetHandCardParent();
            List<Transform> list = grid.GetChildList();
            IEDisCard(player, list, () =>
            {

            });

            if (seatId == XXGoldFlowerGameModel.Inst.mMySeatId)
            {
                mSelfPlayer.HideSixBtnState();
                mSelfPlayer.SetAddBtnItemState(false);
                mSelfPlayer.SetCuoCardBtnState(false);
            }
        }
    }

    /// <summary>
    /// 同步比牌结果
    /// </summary>
    private void OnPlayerCompareCard(OnGoldFlowerPlayerOptResult data) {
        if (data.seatId == XXGoldFlowerGameModel.Inst.mMySeatId || 
            data.otherSeatId == XXGoldFlowerGameModel.Inst.mMySeatId) {
            BaseViewWidget.CloseWidget<XXGoldFlowerCuoCardWidget>();
            mCuoCardWidget = null;
            mSelfPlayer.SetCuoCardBtnState(true);
        }

        for (int i = 0; i < XXGoldFlowerGameModel.Inst.mSeatIdList.Count; i++)
        {
            XXGlodFlowerPlayer player = null;
            if (TryGetPlayer(XXGoldFlowerGameModel.Inst.mSeatIdList[i], out player))
            {
                player.SetCompareState(false);
            }
        }
        if (mPkWidget == null)
        {
            mPkWidget = GetWidget<XXGoldFlowerPkCardWidget>("XXGoldFlowerGameView/XXGoldFlowerPkCardWidget", transform);
        }
        mCompareCardDown = false;

        GoldFlowerPlayer leftPlayer = XXGoldFlowerGameModel.Inst.mPlayerInfoDic[data.seatId];//第一个玩家
        GoldFlowerPlayer RightPlayer = XXGoldFlowerGameModel.Inst.mPlayerInfoDic[data.otherSeatId];//第二个玩家

        bool leftWin = data.seatId == data.winSeatId ? true : false;
        mPkWidget.SetPlayers(leftPlayer.headUrl, leftPlayer.nickname, leftPlayer.userId, RightPlayer.headUrl, RightPlayer.nickname, RightPlayer.userId);
        mPkWidget.StartPk(leftWin, () =>
        {
            mCompareCardDown = true;
            if (XXGoldFlowerGameModel.Inst.mSelfLookCard!=null) {
                OnPlayerLookCards(XXGoldFlowerGameModel.Inst.mSelfLookCard);
            }
        });
    }

    /// <summary>
    /// 最后一战
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private IEnumerator OnLastFight(OnGetGoldFlowerLastFight data) {
        if (PlayerModel.Inst.UserInfo.sex == 1)
        {
            SoundProcess.PlaySound("jh/bipai_boy");
        }
        else {
            SoundProcess.PlaySound("jh/bipai_girl");
        }

        mLastFightAnim.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        mLastFightAnim.gameObject.SetActive(false);
        for (int i=0;i<data.pkSeatList.Count;i++) {
            if (!data.winSeatList.Contains(data.pkSeatList[i])) {
                OnPlayerDisCard(data.pkSeatList[i]);
            }
        }
        yield return new WaitForSeconds(0.5f);//等待0.5f
        mLastFightResult.gameObject.SetActive(true);
        if (data.winSeatList.Contains(XXGoldFlowerGameModel.Inst.mMySeatId)) {//自己在赢的队列里面
            mLastFightResult.GetComponentInChildren<UILabel>().text = "You Win !";
        }
        else if (data.pkSeatList.Contains(XXGoldFlowerGameModel.Inst.mMySeatId))//自己在输的队列里面
        {
            mLastFightResult.GetComponentInChildren<UILabel>().text = "You Lose !";
        }
        else {//自己没参与比牌
            if (data.winSeatList.Count>1) {//多个玩家赢
                mLastFightResult.GetComponentInChildren<UILabel>().text = "平局!";
            }
            else if (data.winSeatList.Count==1)
            {
                mLastFightResult.GetComponentInChildren<UILabel>().text = XXGoldFlowerGameModel.Inst.mPlayerInfoDic[data.winSeatList[0]].nickname + " Win ！";
            }
           
        }
        yield return new WaitForSeconds(0.5f);
        mLastFightDown = true;
        mLastFightResult.gameObject.SetActive(false);
    }

    /// <summary>
    /// 小结算
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private IEnumerator OnSmallSettle(GoldSettlementData data) {

        mSelfPlayer.HideSixBtnState();
        mSelfPlayer.SetAddBtnItemState(false);

        yield return new WaitUntil(()=> {
            return mDisCardDown == true;//等待弃牌动画结束
        });
        yield return new WaitUntil(()=> {
            return mCompareCardDown == true;//等待比牌动画结束
        });
        yield return new WaitUntil(() => {
            return mLastFightDown == true;//最后一战结束
        });
        yield return new WaitForSeconds(1.0f);

        if (XXGoldFlowerGameModel.Inst.mGameed)
        {//参与了游戏
            mSelfPlayer.SetShowCardBtnState(true);
        }
        else
        {
            mSelfPlayer.SetShowCardBtnState(false);
        }
        mSelfPlayer.SetReadybtnState(true);//准备按钮

        if (XXGoldFlowerGameModel.Inst.mGoldPattern)
        {//金币场
            mSelfPlayer.SetChangDeskBtnState(true);
        }
        else
        {
            mSelfPlayer.SetChangDeskBtnState(false);
        }

        ShowLastTime("准备倒计时", 10);
        XXGlodFlowerPlayer player = null;

        //if (XXGoldFlowerGameModel.Inst.mGameed)
        //{
        //    if (TryGetPlayer(XXGoldFlowerGameModel.Inst.mMySeatId, out player))
        //    {
        //        player.SetHandCard(data.cards);
        //        player.SetHandCardType((eGFCardType)data.cardType);
        //        ShowOnePlayerCard(XXGoldFlowerGameModel.Inst.mMySeatId);
        //    }
        //}

        if (mSettlementView != null)
            mSettlementView.Close<GoldSettlementView>();
        mSettlementView = GetWidget<GoldSettlementView>("XXGoldFlowerGameView/GoldSettlementView", transform);
        mSettlementView.SetData(data.data);
        if (data.data != null)
        {
            for(int i = 0; i < data.data.Length; i++)
            {
                if(TryGetPlayer(data.data[i].seatId, out player)){
                    player.SetUserScore(data.data[i].gold);
                }
            }
        }
        //if (data.winList != null)
        //{
        //    int total = mCathecticCoinQueue.Count;//总数
        //    int num = total / data.winList.Count;//每个人有多少个金币
        //    for (int i = 0; i < data.winList.Count; i++)
        //    {
        //        if (i == data.winList.Count - 1)
        //        {//最后一个玩家得到最后剩下的全部金币
        //            FlyCoin(data.winList[i].seatId, mCathecticCoinQueue.Count, false);
        //        }
        //        else//其他玩家得到num个金币
        //        {
        //            FlyCoin(data.winList[i].seatId, num, false);
        //        }


        //        if (TryGetPlayer(data.winList[i].seatId, out player))
        //        {
        //            player.SetUserScore(data.winList[i].score);
        //            player.ShowWinScore(data.winList[i].winScore);
        //            player.SetWinState(true);
        //            if (data.winList[i].xiQian > 0 && data.winList[i].seatId == XXGoldFlowerGameModel.Inst.mMySeatId)
        //            {
        //                XXGoldFlowerXiQianWidget widget = GetWidget<XXGoldFlowerXiQianWidget>(AssetsPathDic.XXGoldFlowerXiQianWidget, transform);
        //                widget.Show(player,data.winList[i].xiQian);
        //            }
        //        }
        //    }
        //}


        //if (data.loseXiList!=null)
        //{
        //    for (int i=0;i<data.loseXiList.Count;i++) {
        //        if (TryGetPlayer(data.loseXiList[i].seatId,out player)) {
        //            if (data.loseXiList[i].seatId == XXGoldFlowerGameModel.Inst.mMySeatId) {
        //                XXGoldFlowerLoseXiQianWidget widget = GetWidget<XXGoldFlowerLoseXiQianWidget>(AssetsPathDic.XXGoldFlowerLoseXiQianWidget, transform);
        //                widget.Show(player, data.loseXiList[i].xiQian,data.loseXiList[i].cardsInfo);
        //            }
        //        }
        //    }
        //}

        //if (data.playerCardsList != null)
        //{
        //    for (int i = 0; i < data.playerCardsList.Count; i++)
        //    {
        //        if (TryGetPlayer(data.playerCardsList[i].seatId, out player))
        //        {
        //            player.SetHandCard(data.playerCardsList[i].card);
        //            player.SetHandCardType((eGFCardType)data.playerCardsList[i].cardType);
        //            ShowOnePlayerCard(data.playerCardsList[i].seatId);
        //        }
        //    }
        //}
        //UpdateAllPeachOfThisMatch(data.totalWin);
        XXGoldFlowerGameModel.Inst.CleanMode();
        DelayRun(3, ()=> {
            
            CleanDesk();
        } );
    }

    #endregion


    #region 辅助函数

    /// <summary>
    /// 通过座位号得到玩家ui
    /// </summary>
    /// <param name="seatId"></param>
    /// <returns></returns>
    public bool TryGetPlayer(int seatId, out XXGlodFlowerPlayer player) {
        return mPayerDic.TryGetValue(seatId, out player);
    }

    /// <summary>
    /// 玩家坐下
    /// </summary>
    /// <param name="player"></param>
    private void PlayerSeatDown(GoldFlowerPlayer player) {
        int index = 0;
        if (player.seatId == XXGoldFlowerGameModel.Inst.mMySeatId)
        {
            if (player.isReady)
            {
                mSelfPlayer.SetReadybtnState(false);
            }
            else {
                mSelfPlayer.SetReadybtnState(true);
            }
            index = 0;
            mSelfPlayer.SetAutoBtnState(player.autoGen);
            UpdateAllPeachOfThisMatch(player.totalWin);
        }
        else if (player.seatId - XXGoldFlowerGameModel.Inst.mMySeatId > 0)
        {
            index = player.seatId - XXGoldFlowerGameModel.Inst.mMySeatId;
        }
        else
        {
            index = player.seatId - XXGoldFlowerGameModel.Inst.mMySeatId + mPlayerArray.Length;
        }
        mPlayerArray[index].SeatId = player.seatId;
        mPlayerArray[index].InitPlayer(player.headUrl, player.nickname, player.userId,XXGoldFlowerGameModel.Inst.mSubGameId>0? player.score:player.gold,player.toalBet, player.isReady, player.onLineState == 1 ? false : true);
        if (mPayerDic.ContainsKey(player.seatId)) {
            mPayerDic[player.seatId] = mPlayerArray[index];
        }
        else {
            mPayerDic.Add(player.seatId,mPlayerArray[index]);
        }

        if (player.seatId == XXGoldFlowerGameModel.Inst.mZhuangSeatId) {
            mPlayerArray[index].SetZhuangState(true);
        }
        else {
            mPlayerArray[index].SetZhuangState(false);
        }

    }

    /// <summary>
    /// 显示倒计时
    /// </summary>
    private void ShowLastTime(string content,int lastTime) {

        mLastTime = lastTime;
        mLastTimeContent = content;
        mLastTimeGo.SetActive(true);
        mLastTimeGo.GetComponentInChildren<UILabel>().text = content+"(" + mLastTime + ")";
        if (IsInvoking("InvokeLastTime")) {
            CancelInvoke("InvokeLastTime");
        }
        if (lastTime > 0)
        {
            InvokeRepeating("InvokeLastTime", 0.01f, 1.0f);
        }
        else {
            mLastTimeGo.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 播放全压动画
    /// </summary>
    private void ShowAllInTween() {
        mAllInTween.gameObject.SetActive(true);
        TweenRotation s = mAllInTween.gameObject.GetComponentInChildren<TweenRotation>();
        StartCoroutine(IEFinished(mAllInTween.gameObject));
        s.PlayForward();
    }

    /// <summary>
    /// 播放孤注一掷动画
    /// </summary>
    private void ShowFuckItTween() {
        mFuckItTween.gameObject.SetActive(true);
        TweenScale s = mFuckItTween.gameObject.AddComponent<TweenScale>();
        s.from = new Vector3(3, 3, 3);
        s.to = new Vector3(2, 2, 2);
        s.duration = 0.25f;
        s.AddOnFinished(() =>
        {
            StartCoroutine(IEFinished(mFuckItTween.gameObject));
            Destroy(s);
        });
        s.PlayForward();
    }


    private IEnumerator IEFinished(GameObject go) {
        yield return new WaitForSeconds(2.0f);
        go.gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示倒计时
    /// </summary>
    private void InvokeLastTime() {
        mLastTimeGo.GetComponentInChildren<UILabel>().text = mLastTimeContent + "(" + mLastTime + ")";
        mLastTime -= 1;
        if (mLastTime<0) {
            mLastTimeGo.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 发牌
    /// </summary>
    /// <returns></returns>
    private IEnumerator IECastCard(CallBack call)
    {
        for (int i = 0; i < 3; i++)
        {
            mCastCardDown = false;
            CastOneCard();
            SoundProcess.PlaySound("jh/fapai");
            yield return new WaitForSeconds(0.2f);
        }
        if (call != null)
        {
            call();
        }
        mCastCardDown = true;
    }

    /// <summary>
    /// 弃牌动画
    /// </summary>
    /// <param name="call"></param>
    /// <returns></returns>
    private void IEDisCard(XXGlodFlowerPlayer player,List<Transform> cards,CallBack call) {
        for (int i = 0; i < cards.Count; i++)
        {
            mDisCardDown = false;
            if (cards[i]!=null && cards[i].gameObject!=null) {
                DisCardOnCard(cards[i].gameObject);
            }
        }
        if (call != null)
        {
            call();
        }
        mDisCardDown = true;
    }

    /// <summary>
    /// 只显示弃牌和看牌按钮
    /// </summary>
    private void ManagerOptBtn(int seatId) {
        SQDebug.Log("隐藏多余的操作按钮");
        if (seatId == XXGoldFlowerGameModel.Inst.mMySeatId)
        {
            mSelfPlayer.HideSixBtnState();
            //mSelfPlayer.SetDisCardBtnState(true);
            mSelfPlayer.SetAutoFollowState(true);
            //if (XXGoldFlowerGameModel.Inst.mCanLookCard) {
            //    mSelfPlayer.SetLookCardBtnState(true);
            //    mSelfPlayer.SetCuoCardBtnState(true);
            //}
        }
    }

    /// <summary>
    /// 直接生成三张牌
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="cards"></param>
    private void InstantiateCards(XXGlodFlowerPlayer Player, List<string> cards,eGFCardType type = eGFCardType.Nil) {
        Player.CleanHandCards();
        UIGrid grid = Player.GetHandCardParent();
        for (int i = 0; i < cards.Count; i++) {
            GameObject go = Assets.InstantiateChild(grid.gameObject, mCastCardItem.gameObject);
            if (Player.SeatId == XXGoldFlowerGameModel.Inst.mMySeatId) {
                go.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            go.gameObject.SetActive(true);
            if (cards[i].Contains("a") || cards[i].Contains("b") || cards[i].Contains("c") || cards[i].Contains("d"))
            {
                TurnCardAnim card = go.GetComponent<TurnCardAnim>();
                card.SetCard(cards[i]);
                card.ShowCardNum(cards[i]);
            }
        }
        if (type!=eGFCardType.Nil) {
            Player.SetHandCardType(type);
        }
        grid.Reposition();
    }

    /// <summary>
    /// 更新当前跟注
    /// </summary>
    /// <param name="value"></param>
    private void UpdateCurGenZhu(float value) {
        mCurGenZhu.text =  value.ToString("f2");
    }

    /// <summary>
    /// 更新总底池数量
    /// </summary>
    /// <param name="value"></param>
    private void UpdateTotalDiChi(float value) {
        mTotalDiChi.text =  value.ToString("f2");
    }

    /// <summary>
    /// 我进入房间开始输赢的金币
    /// </summary>
    private void UpdateAllPeachOfThisMatch(float value)
    {
        mAllPeachOfThisMatch.text = "" + value;
    }

    /// <summary>
    /// 隐藏轮到谁的提示
    /// </summary>
    /// <param name="seatId"></param>
    private void HideTurnSp(int seatId) {
        XXGlodFlowerPlayer player = null;

        if (TryGetPlayer(seatId, out player))
        {
            if (player != null)
            {
                player.ShowLastTime(0);
            }
        }
    }

    /// <summary>
    /// 更新玩家金币或者积分
    /// </summary>
    /// <param name="seat"></param>
    /// <param name="score"></param>
    private void UpdatePlayerScore(int seat,float score) {
        XXGlodFlowerPlayer player = null;
        if (TryGetPlayer(seat,out player)) {
            player.SetUserScore(score);
        }
    }

    /// <summary>
    /// 设置可以比牌的玩家的选择框
    /// </summary>
    /// <param name="show"></param>
    public void SetShowCompareSelectSp(bool show) {
        for (int i=0;i< XXGoldFlowerGameModel.Inst.mHasCardSeatList.Count;i++) {
            XXGlodFlowerPlayer player = null;
            if (TryGetPlayer(XXGoldFlowerGameModel.Inst.mHasCardSeatList[i], out player))
            {
                if (XXGoldFlowerGameModel.Inst.mHasCardSeatList[i]!=XXGoldFlowerGameModel.Inst.mMySeatId) {
                    player.SetCompareState(true);
                }
            }
        }
    }

    /// <summary>
    /// 得到同步指令的音效
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private string GetInsSound(OnGoldFlowerPlayerOptResult data) {
        List<ConfigDada> config = ConfigManager.GetConfigs<JinHuaInsSoundConfig>();
        JinHuaInsSoundConfig con = null;
        for (int i = 0; i < config.Count; i++)
        {
            JinHuaInsSoundConfig temp = config[i] as JinHuaInsSoundConfig;
            if (temp.ins == data.ins)
            {
                con = temp;
            }
        }
        List<string> sounds = new List<string>();
        if (XXGoldFlowerGameModel.Inst.mPlayerInfoDic[data.seatId].sex == 1)
        {
            sounds = con.Mansound;
        }
        else {
            sounds = con.WomanSound;
        }

        return sounds[Random.Range(0, sounds.Count)];
    }

    #endregion

    #region 发牌动画

    /// <summary>
    /// 发一张牌
    /// </summary>
    public void CastOneCard() {
        for (int i = 0; i < XXGoldFlowerGameModel.Inst.mHasCardSeatList.Count; i++)
        {
            XXGlodFlowerPlayer player = null;
            if (mPayerDic.TryGetValue(XXGoldFlowerGameModel.Inst.mHasCardSeatList[i], out player))
            {
                UIGrid playerHand = player.GetHandCardParent();
                GameObject cardItem = NGUITools.AddChild(playerHand.gameObject, mCastCardItem.gameObject);
                List<Transform> childs = playerHand.GetChildList();
                cardItem.transform.localPosition = new Vector3( playerHand.cellWidth * (childs.Count - 1), 0, 0);

                Vector3 topos = new Vector3(cardItem.transform.position.x, cardItem.transform.position.y, cardItem.transform.position.z);
                TurnCardAnim anim = cardItem.GetComponent<TurnCardAnim>();

                anim.SetCardBgDeepsByIndex(childs.Count);
                UIWidget wid = cardItem.GetComponentInChildren<UIWidget>();
                wid.depth = childs.Count;

                cardItem.gameObject.transform.position = mMeiNvHeGuanHand.transform.position;
                cardItem.gameObject.SetActive(true);
                Hashtable args = new Hashtable();

                List<object> finishargs = new List<object>();
                finishargs.Add(player);
                finishargs.Add(cardItem);
                args.Add("easeType", iTween.EaseType.linear);
                args.Add("time", 0.5f);
                args.Add("position", topos);
                iTween.MoveTo(cardItem, args);

                //if (XXGoldFlowerGameModel.Inst.mHasCardSeatList[i] == XXGoldFlowerGameModel.Inst.mMySeatId) {
                //    Hashtable selftweenargs = new Hashtable();
                //    List<object> finishArgs = new List<object>();
                //    finishArgs.Add(player);
                //    finishArgs.Add(cardItem);
                //    selftweenargs.Add("easeType", iTween.EaseType.linear);
                //    selftweenargs.Add("time", 0.2f);
                //    selftweenargs.Add("oncomplete", "OnSelfScaleFinished");
                //    selftweenargs.Add("oncompleteparams", finishargs);
                //    selftweenargs.Add("oncompletetarget", gameObject);
                //    //selftweenargs.Add("scale", new Vector3(2.0f, 2.0f, 2.0f));
                //    iTween.ScaleTo(cardItem, selftweenargs);
                //}
            }
        }
    }

    /// <summary>
    /// 发牌移动结束
    /// </summary>
    /// <param name="grid"></param>
    private void OnSelfScaleFinished(List<object> args)
    {
        XXGlodFlowerPlayer player = args[0] as XXGlodFlowerPlayer;
        GameObject cardItem = args[1] as GameObject;
        UIGrid playerHand = player.GetHandCardParent();
        playerHand.Reposition();
    }

    #endregion


    #region 弃牌动画

    /// <summary>
    /// 弃一张牌
    /// </summary>
    private void DisCardOnCard(GameObject card) {


        Hashtable moveArgs = new Hashtable();

        moveArgs.Add("easeType", iTween.EaseType.linear);
        moveArgs.Add("time", 0.3f);
        moveArgs.Add("position", mMeiNvHeGuanHand.transform.position);
        iTween.MoveTo(card, moveArgs);

        Hashtable aArgs = new Hashtable();

        aArgs.Add("easeType", iTween.EaseType.linear);
        aArgs.Add("time", 0.3f);
        aArgs.Add("scale", new Vector3(0.0f, 0.0f, 0.0f));
        iTween.ScaleTo(card, aArgs);

        TweenColor tween = card.AddComponent<TweenColor>();
        tween.to = new Color(255, 255, 255, 0);
        tween.duration = 0.3f;
        tween.AddOnFinished(()=> {
            Destroy(tween);
        });
        tween.PlayForward();
    }

    #endregion


    #region 飞金币动画


    /// <summary>
    /// 飞金币
    /// </summary>
    /// <param name="seatId">座位号</param>
    /// <param name="num">飞几个</param>
    /// <param name="forward">true是飞到桌子上，false是飞到自己头上</param>
    public void FlyCoin(int seatId, int num, bool forward,float value = 0.0f)
    {

        if (forward)
        {
            StartCoroutine(IEFlyCathecticCoin(seatId, num,value));
        }
        else {
            FlyGetCoin(seatId, num);
        }
    }

    /// <summary>
    /// 飞下注金币
    /// </summary>
    /// <param name="seatId"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    private IEnumerator IEFlyCathecticCoin(int seatId, int num,float value = 0.0f) {
        XXGlodFlowerPlayer player = null;
        if (mPayerDic.TryGetValue(seatId, out player))
        {
            for (int i = 0; i < num; i++)
            {
                GameObject coin = null;

                if (mFlyCoinQueue.Count > 0)
                {
                    coin = mFlyCoinQueue.Dequeue();
                }
                else
                {
                    coin = NGUITools.AddChild(mFlyCoinParent.gameObject, mFlyCoin.gameObject);
                }
                coin.gameObject.SetActive(true);
                mCathecticCoinQueue.Enqueue(coin);
                coin.GetComponentInChildren<UISprite>().spriteName = XXGoldFlowerGameModel.Inst.GetCoinSpriteName(player.GetLookCard(), value);
                UILabel label = coin.GetComponentInChildren<UILabel>();
                if (label!=null && value!=0.0F) {
                    label.text = value.ToString();
                    label.depth = 2 * mCathecticCoinQueue.Count + 1;
                }

                UISprite sprite = coin.GetComponentInChildren<UISprite>();
                if (sprite!=null) {
                    sprite.depth = 2 * mCathecticCoinQueue.Count;
                }


                coin.transform.position = player.GetBaseInfoPos();

                Hashtable args = new Hashtable();
                List<object> finishargs = new List<object>();
                args.Add("easeType", iTween.EaseType.linear);
                args.Add("time", 0.3f);
                args.Add("onstart", "OnFlyCoinCatheStart");
                args.Add("onstartparams", coin);
                args.Add("onstarttarget", gameObject);
                args.Add("position", new Vector3(mFlyCoinParent.transform.position.x + Random.Range(-.3f, 0.3f),
                    mFlyCoinParent.transform.position.y + Random.Range(-.1f, 0.1f), 0));
                iTween.MoveTo(coin, args);
                SoundProcess.PlaySound("jh/betCoin");
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    /// <summary>
    /// 飞玩家赢金币
    /// </summary>
    /// <param name="seatId"></param>
    /// <param name="num"></param>
    private void FlyGetCoin(int seatId, int num) {
        XXGlodFlowerPlayer player = null;
        SoundProcess.PlaySound("jh/getGlod");
        if (mPayerDic.TryGetValue(seatId, out player)) {
            int count = mCathecticCoinQueue.Count;
            for (int i = 0; i < num; i++)
            {
                GameObject coin = mCathecticCoinQueue.Dequeue();
                mFlyCoinQueue.Enqueue(coin);
                Hashtable args = new Hashtable();
                List<object> finishargs = new List<object>();
                args.Add("easeType", iTween.EaseType.linear);
                args.Add("time", 0.5f);
                args.Add("oncomplete", "OnGetFlyCoinFinished");
                args.Add("oncompleteparams", coin);
                args.Add("oncompletetarget", gameObject);
                args.Add("position", new Vector3(player.GetBaseInfoPos().x, player.GetBaseInfoPos().y, player.GetBaseInfoPos().z));
                iTween.MoveTo(coin, args);
            }
        }
    }

    /// <summary>
    /// 下注飞金币开始
    /// </summary>
    /// <param name="coin"></param>
    private void OnFlyCoinCatheStart(GameObject coin) {
        coin.gameObject.SetActive(true);
    }


    /// <summary>
    /// 得到金币结束
    /// </summary>
    /// <param name="coin"></param>
    private void OnGetFlyCoinFinished(GameObject coin)
    {
        coin.gameObject.SetActive(false);
    }


    #endregion


    #region 翻牌

    /// <summary>
    /// 显示所有玩家的手牌
    /// </summary>
    private void ShowAllPlayerCard() {
        for (int i=0;i<XXGoldFlowerGameModel.Inst.mSeatIdList.Count;i++) {
            ShowOnePlayerCard(XXGoldFlowerGameModel.Inst.mSeatIdList[i]);
        }
    }

    /// <summary>
    /// 显示一个玩家的手牌
    /// </summary>
    /// <param name="seatId"></param>
    private void ShowOnePlayerCard(int seatId) {
        XXGlodFlowerPlayer player = null;
        if (mPayerDic.TryGetValue(seatId,out player)) {
            UIGrid grid = player.GetHandCardParent();

            bool show = false;//是否翻过牌

            bool show1 = false;//是否牌被隐藏了

            List<Transform> list = grid.GetChildList();
            for (int i=0;i<list.Count;i++) {
                show = list[i].GetComponentInChildren<TurnCardAnim>().GetCardShowState();
                if (list[i].gameObject.GetComponent<UIWidget>().alpha<0.9f) {
                    show1 = true;
                }
                list[i].gameObject.GetComponent<UIWidget>().alpha = 1.0f;
                if (seatId == XXGoldFlowerGameModel.Inst.mMySeatId)
                {
                    list[i].gameObject.transform.localScale = Vector3.one ;
                }
                else {
                    list[i].gameObject.transform.localScale = Vector3.one;
                }
            }

            if (show && !show1) {
                return;
            }

            TurnCardAnim[] anims = grid.GetComponentsInChildren<TurnCardAnim>();
            for (int i=0;i<anims.Length;i++) {
                if (player.GetHandCard().Count>i) {
                    anims[i].SetCard(player.GetHandCard()[i]);
                }

                anims[i].TurnCard(0.3f, true, 1);
                int index = anims.Length - i;
                //if (player.GetPlayerUIPos() == XXGFPlayerPos.Left || player.GetPlayerUIPos() == XXGFPlayerPos.LeftTop)
                {
                    index = i + 1;
                }
                //else {
                //    index = anims.Length - i;
                //}
                anims[i].SetCardDeepsByIndex(index);
                UIWidget wid = anims[i].gameObject.GetComponentInChildren<UIWidget>();
                wid.depth = index;
            }
            grid.Reposition();
        }
    }

    #endregion

    //设置解散房间时间
    public void SetDisolveTime(float time)
    {
        mDisolveTime = time + Time.realtimeSinceStartup;
    }
    void OnApplicationPause(bool b)
    {
        if (b)//最小化游戏
        {
            SQDebug.Log("程序失去焦点");
            NetProcess.ReleaseAllConnect();
            GameManager.Instance.ResetConnetTime();
        }
    }

    [ContextMenu("Test")]
    public void Test()
    {
        Global.Inst.GetController<MainController>().BackToMain();
    }
}
