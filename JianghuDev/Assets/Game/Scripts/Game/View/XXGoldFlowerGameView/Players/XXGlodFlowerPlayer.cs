using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//using YunvaIM;

public enum XXGFPlayerPos {
    RightTop = 1,
    LeftTop = 2,
    Right = 3,
    Left = 4,
    RightBottom = 5,
    Self = 5
}

public class XXGlodFlowerPlayer : MonoBehaviour {

    #region UI

    /// <summary>
    /// 手牌的排列
    /// </summary>
    public UIGrid mHandCardGid;

    /// <summary>
    /// 下注显示
    /// </summary>
    public UILabel mCathecticCoinLabel;

    /// <summary>
    /// 赢了多少钱的显示
    /// </summary>
    public UILabel mWinSocreShowLabel;

    /// <summary>
    /// 文字聊天显示
    /// </summary>
    public GameObject mTxtChatSp;

    /// <summary>
    /// 语音动画
    /// </summary>
    public SpriteAnimation mYYVoiceAnim;

    /// <summary>
    /// 赢了的图集
    /// </summary>
    public UIFont mWinAtlas;

    /// <summary>
    /// 输了的图集
    /// </summary>
    public UIFont mLoseAtlas;

    /// <summary>
    /// 基础信息的副几点
    /// </summary>
    public GameObject mHeadParentGo;
    /// <summary>
    /// 牌型
    /// </summary>
    public UISprite mCardType;

    /// <summary>
    /// 庄家图标
    /// </summary>
    public GameObject mZhuangSp;

    /// <summary>
    /// 所属ui位置
    /// </summary>
    public XXGFPlayerPos mUIPos;

    #endregion

    #region 私有属性

    /// <summary>
    /// 座位号
    /// </summary>
    private int mSeatId;

    /// <summary>
    /// 基础信息节点
    /// </summary>
    private XXGlodFlowerBasePlayerInfo mBasePlayerInfo;

    /// <summary>
    /// 互动表情处理
    /// </summary>
    private GameInteractionView mGameInteractionView;

    /// <summary>
    /// 手牌
    /// </summary>
    private List<string> mHandCard = new List<string>();

    /// <summary>
    /// 手牌类型
    /// </summary>
    private eGFCardType mHandCardType;

    /// <summary>
    /// 显示赢钱的初始位置
    /// </summary>
    private Vector3 mWinScorePos;

    #endregion

    #region Unity

    private void Start()
    {
        mWinScorePos = mWinSocreShowLabel.transform.localPosition;
    }

    #endregion

    #region 字段

    /// <summary>
    /// 座位号属性
    /// </summary>
    public int SeatId {
        get {
            return mSeatId;
        }
        set {
            mSeatId = value;
        }
    }

    #endregion


    #region ui事件

    /// <summary>
    /// 头像点击事件
    /// </summary>
    public void OnHeadClick() {
        if (SeatId > 0)
        {
            if (XXGoldFlowerGameModel.Inst.mComparingCard && SeatId != XXGoldFlowerGameModel.Inst.mMySeatId)
            {//正在比牌
                SendGoldFlowerOpt req = new SendGoldFlowerOpt();
                req.ins = (int)eGFOptIns.Compare;
                req.otherSeatId = SeatId;
                Global.Inst.GetController<XXGoldFlowerGameController>().SendOpt(req, () =>
                {
                    XXGoldFlowerGameModel.Inst.mComparingCard = false;
                });
            }
            else
            {//没有在比牌
                Global.Inst.GetController<XXGoldFlowerGameController>().SendGetPlayerInfo(XXGoldFlowerGameModel.Inst.mPlayerInfoDic[mSeatId].userId, mSeatId);
            }
        }

    }


    #endregion

    #region 外部调用

    /// <summary>
    /// 初始玩家
    /// </summary>
    /// <param name="head"></param>
    /// <param name="name"></param>
    /// <param name="uid"></param>
    /// <param name="score"></param>
    /// <param name="costCoin">已下注的金币数量</param>
    /// <param name="ready"></param>
    /// <param name="offline"></param>
    /// <param name="discard"></param>
    public void InitPlayer(string head, string name, string uid, float score, float costCoin, bool ready = false, bool offline = false, bool discard = false) {
        if (mBasePlayerInfo == null)
        {
            mBasePlayerInfo = Assets.InstantiateChild(mHeadParentGo, Global.Inst.GetController<XXGoldFlowerGameController>().mView.mPlayerBaseItem.gameObject).GetComponent<XXGlodFlowerBasePlayerInfo>();
        }
        mBasePlayerInfo.gameObject.SetActive(true);
        mBasePlayerInfo.transform.localPosition = Vector3.one;
        mBasePlayerInfo.InitUI(head, name, uid, score, ready, offline, discard);
        if (costCoin > 0)
        {
            mCathecticCoinLabel.gameObject.SetActive(true);
            mCathecticCoinLabel.text = costCoin.ToString();
        }
        else {
            mCathecticCoinLabel.text = "";
            mCathecticCoinLabel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 清理玩家
    /// </summary>
    public void CleanPlayer() {
        CleanHandCards();
        if (mBasePlayerInfo != null) {
            NGUITools.DestroyImmediate(mBasePlayerInfo.gameObject);
            mBasePlayerInfo = null;
        }
        mCathecticCoinLabel.text = "";
        mCardType.gameObject.SetActive(false);
        mCathecticCoinLabel.gameObject.SetActive(false);
        mTxtChatSp.gameObject.SetActive(false);
        mYYVoiceAnim.gameObject.SetActive(false);
        mZhuangSp.gameObject.SetActive(false);
        SeatId = 0;
    }


    public XXGFPlayerPos GetPlayerUIPos() {
        return mUIPos;
    }


    public Vector3 GetBaseInfoPos() {
        return mBasePlayerInfo.transform.position;
    }

    /// <summary>
    /// 设置玩家头像
    /// </summary>
    /// <param name="url"></param>
    public void SetPlayerHead(string url)
    {
        mBasePlayerInfo.SetPlayerHead(url);
    }

    /// <summary>
    /// 设置准备状态
    /// </summary>
    /// <param name="show"></param>
    public void SetReadyState(bool show)
    {
        mBasePlayerInfo.SetReadyState(show);
    }

    /// <summary>
    /// 设置离线状态
    /// </summary>
    /// <param name="show"></param>
    public void SetOffLineState(bool show)
    {
        mBasePlayerInfo.SetOffLineState(show);
    }

    //设置赢了
    public void SetWinState(bool show) {
        mBasePlayerInfo.SetWinState(show);
    }


    /// <summary>
    /// 设置庄家图标
    /// </summary>
    /// <param name="show"></param>
    public void SetZhuangState(bool show)
    {
        //mZhuangSp.gameObject.SetActive(show);
    }

    /// <summary>
    /// 设置弃牌状态
    /// </summary>
    /// <param name="show"></param>
    public void SetDisCardState(bool show)
    {
        mBasePlayerInfo.SetDisCardState(show);
    }

    /// <summary>
    /// 设置看牌状态
    /// </summary>
    /// <param name="show"></param>
    public void SetLookCarsState(bool show)
    {

        SQDebug.Log("look：" + show);

        mBasePlayerInfo.SetLookCarsState(show);
        if (show)
            mBasePlayerInfo.SetInsTips(eGFOptIns.LookCard);
    }

    //设置操作提示
    public void SetInsTips(eGFOptIns ins)
    {
        mBasePlayerInfo.SetInsTips(ins);
    }

    /// <summary>
    /// 设置玩家名字
    /// </summary>
    /// <param name="name"></param>
    public void SetUserName(string name)
    {
        mBasePlayerInfo.SetUserName(name);
    }

    /// <summary>
    /// 设置积分或者金币
    /// </summary>
    /// <param name="score"></param>
    public void SetUserScore(float score)
    {
        mBasePlayerInfo.SetUserScore(score);
    }

    /// <summary>
    /// 清除玩家手牌
    /// </summary>
    public void CleanHandCards() {
        if (mHandCard!=null) {
            mHandCard = new List<string>();
            mHandCard.Clear();
        }
        mCardType.gameObject.SetActive(false);
        mCathecticCoinLabel.gameObject.SetActive(false);
        SetHandCardType(eGFCardType.Nil);
        NGUITools.DestroyChildren(mHandCardGid.transform);
    }

    /// <summary>
    /// 得到手牌的父节点
    /// </summary>
    /// <returns></returns>
    public UIGrid GetHandCardParent()
    {
        return mHandCardGid;
    }


    /// <summary>
    /// 更新总的下注金币
    /// </summary>
    /// <param name="num"></param>
    public void UpdateCathecticCoin(string num) {
        if (string.IsNullOrEmpty(num)) {
            mCathecticCoinLabel.gameObject.SetActive(false);
        }
        else {
            mCathecticCoinLabel.gameObject.SetActive(true);
        }
        mCathecticCoinLabel.text = num;
    }

    /// <summary>
    /// 设置手牌
    /// </summary>
    /// <param name="cards"></param>
    public void SetHandCard(List<string> cards) {
        mHandCard = cards;
    }

    /// <summary>
    /// 设置手牌类型
    /// </summary>
    /// <param name="type"></param>
    public void SetHandCardType(eGFCardType type) {
        mHandCardType = type;
        if (type != eGFCardType.Nil)
        {
            mCardType.gameObject.SetActive(true);
            mCardType.spriteName = "jinhua_" + (int)type;
        }
        else {
            mCardType.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 返回手牌
    /// </summary>
    /// <returns></returns>
    public List<string> GetHandCard() {
        return mHandCard;
    }

    /// <summary>
    /// 返回手牌类型
    /// </summary>
    /// <returns></returns>
    public eGFCardType GetCardType() {
        return mHandCardType;
    }

    /// <summary>
    /// 返回是否看了牌
    /// </summary>
    /// <returns></returns>
    public bool GetLookCard() {
        return mBasePlayerInfo.mLookCardSp.gameObject.activeSelf;
    }

    /// <summary>
    /// 设置可比牌界面
    /// </summary>
    /// <param name="show"></param>
    public void SetCompareState(bool show)
    {
        //mBasePlayerInfo.transform.GetChild(0).gameObject.SetActive(show);
        mBasePlayerInfo.SetCompareState(show);
    }

    /// <summary>
    /// 显示赢了多少钱
    /// </summary>
    /// <param name="score"></param>
    public void ShowWinScore(float score) {
        mWinSocreShowLabel.gameObject.SetActive(true);
        UISprite sp = mWinSocreShowLabel.gameObject.GetComponentInChildren<UISprite>();
        if (sp!=null) {
            sp.spriteName = score > 0 ? "win_bg" : "lose_bg";
        }
        mWinSocreShowLabel.bitmapFont = score > 0 ? mWinAtlas : mLoseAtlas;
        mWinSocreShowLabel.text = score > 0 ? "+" + score : score+"";
        TweenPosition p = mWinSocreShowLabel.gameObject.AddComponent<TweenPosition>();
        p.from = mWinScorePos;
        p.to = mWinScorePos + new Vector3(0, 110, 0);
        p.duration = 0.5f;
        p.AddOnFinished(()=> {
            Destroy(p);
            Invoke("DelayHideWinScore", 2.0f);
        });
    }


    /// <summary>
    /// 显示倒计时
    /// </summary>
    /// <param name="time"></param>
    public void ShowLastTime(int time) {
        mBasePlayerInfo.SetLastTime(time*1.0f);
    }

    /// <summary>
    /// 游戏聊天
    /// </summary>
    /// <param name="chat"></param>
    public void ServerGameChat(SendReceiveGameChat chat)
    {
        switch ((eGameChatContentType)chat.chatType)
        {
            case eGameChatContentType.Face://表情

                break;
            case eGameChatContentType.Chat://普通输入的文字

                break;
            case eGameChatContentType.TexTVoice://快捷文字聊天
                PlayTxtVoiceChat(chat);
                break;
            case eGameChatContentType.HDFace://互动表情
                PlayHuDongFace(chat);
                break;
            case eGameChatContentType.Voice://语音
                PlayVoiceChat(chat);
                break;
        }
    }

    #region 语音聊天处理

    /// <summary>
    /// 播放语音
    /// </summary>
    /// <param name="chat"></param>
    private void PlayVoiceChat(SendReceiveGameChat chat)
    {
        bool ised = PlayerPrefs.GetInt("DDL_" + PlayerModel.Inst.UserInfo.userId) == 1 ? true : false;//等于1是勾选了，其他是未勾选
        if (!ised) {
            string ext = DateTime.Now.ToFileTime().ToString();
#if YYVOICE
            YunVaImSDK.instance.RecordStartPlayRequest("", chat.content, ext, (data2) =>
            {
                if (data2.result == 0)
                {
                    SQDebug.Log("播放成功");
                }
                else
                {
                    SQDebug.Log("播放失败");
                }
            });
#endif
        }
        PlayYYVoiceAnimaion(chat);
    }


    /// <summary>
    /// 播放语音动画
    /// </summary>
    private void PlayYYVoiceAnimaion(SendReceiveGameChat chat)
    {
        mYYVoiceAnim.gameObject.SetActive(true);
        mYYVoiceAnim.SetBegin("yx_yy0",1,3,50000,0.25f);
        float vTime = chat.voiceChatTime / 1000;
        StartCoroutine(DelaySetVoiceToBegin(vTime));
    }

    IEnumerator DelaySetVoiceToBegin(float vTime)
    {
        yield return new WaitForSeconds(vTime);
        mYYVoiceAnim.gameObject.SetActive(false);
    }

    #endregion

    #region 文字聊天处理

    /// <summary>
    /// 显示文字聊天
    /// </summary>
    /// <param name="chat"></param>
    private void PlayTxtVoiceChat(SendReceiveGameChat chat)
    {

        mTxtChatSp.gameObject.SetActive(true);

        List<ConfigDada> mRulelist = mRulelist = ConfigManager.GetConfigs<TSTGameTxtChatConfig>();

        for (int i = 0; i < mRulelist.Count; i++)
        {
            TSTGameTxtChatConfig config = mRulelist[i] as TSTGameTxtChatConfig;

            if (config.id == chat.faceIndex)
            {
                mTxtChatSp.GetComponentInChildren<UILabel>().text = config.name;
                mTxtChatSp.GetComponent<UISprite>().width = mTxtChatSp.GetComponentInChildren<UILabel>().width + 70;

                int sex = 1;

                GoldFlowerPlayer player = null;

                if (XXGoldFlowerGameModel.Inst.mPlayerInfoDic.TryGetValue(chat.fromSeatId,out player)) {
                    sex = player.sex;
                }

                if (sex == 1)
                {
                    SoundProcess.PlaySound("ChatSound/" + config.soundNameman);
                }
                else
                {
                    SoundProcess.PlaySound("ChatSound/" + config.soundNamewoman);
                }


                StopCoroutine("DelayHideTxtChat");
                StartCoroutine("DelayHideTxtChat");
            }
        }
    }


    IEnumerator DelayHideTxtChat()
    {
        yield return new WaitForSeconds(2.0f);
        mTxtChatSp.gameObject.SetActive(false);

    }

    #endregion


    #region 互动表情处理

    /// <summary>
    /// 显示互动表情
    /// </summary>
    /// <param name="chat"></param>
    private void PlayHuDongFace(SendReceiveGameChat chat)
    {

        if (mGameInteractionView == null)
            mGameInteractionView = Global.Inst.GetController<GameInteractionController>().OpenWindow() as GameInteractionView;

        List<ConfigDada> config = ConfigManager.GetConfigs<TSTHuDongFaceConfig>();
        TSTHuDongFaceConfig con = null;

        for (int i = 0; i < config.Count; i++)
        {
            TSTHuDongFaceConfig hdf = config[i] as TSTHuDongFaceConfig;
            if (hdf.id == chat.faceIndex)
            {
                con = hdf;
                break;
            }
        }
        //起始位置
        Vector3 from = Vector3.zero;
        //目标位置
        Vector3 to = Vector3.zero;

        XXGoldFlowerGameView view = Global.Inst.GetController<XXGoldFlowerGameController>().mView;

        XXGlodFlowerPlayer fromPlayer = null;
        XXGlodFlowerPlayer toPlayer = null;

        if (view.TryGetPlayer(chat.fromSeatId,out fromPlayer) && view.TryGetPlayer(chat.toSeatId,out toPlayer)) {
            from = fromPlayer.GetBaseInfoPos();
            to = toPlayer.GetBaseInfoPos();

            mGameInteractionView.AddOneInteractionFace(from, to, chat);
        }
    }

    #endregion

    #endregion


    #region 辅助函数

    /// <summary>
    /// 延迟隐藏赢了多少钱
    /// </summary>
    private void DelayHideWinScore() {
        mWinSocreShowLabel.gameObject.SetActive(false);
        mWinSocreShowLabel.transform.localPosition = mWinScorePos;
    }

    #endregion
}
