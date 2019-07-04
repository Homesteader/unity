using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//using YunvaIM;

public class NiuniuPlayerUI : BaseViewWidget
{

    #region UI

    public UIGrid mHandCardGid;//手牌的排列
    public UILabel mCathecticCoinLabel;//下注显示
    public UISprite mQZResultLabel;//抢庄结果
    public UISprite mCardTypeLabel;//牌型
    public UISprite mCardBeiLvLabel;//倍率
    public UILabel mWinLoseLabel;//输赢显示
    public GameObject mTxtChatSp;//文字聊天显示
    public SpriteAnimation mYYVoiceAnim;//语音动画
    public XXGFPlayerPos mUIPos;//所属ui位置
    public GameObject mHandCardItem;//手牌item
    public Transform mDealCardStartPos;//发牌起始点
    public GameObject mHeadInfoItem;//头像信息
    public GameObject mHeadParent;//头像父节点

    /// <summary>
    /// 赢了的图集
    /// </summary>
    public UIFont mWinAtlas;

    /// <summary>
    /// 输了的图集
    /// </summary>
    public UIFont mLoseAtlas;

    #endregion

    #region 私有属性

    protected int mSeatId;//座位号
    protected NiuniuBasePlayerInfo mBasePlayerInfo;//基础信息节点
    protected GameInteractionView mGameInteractionView;//互动表情处理
    protected List<string> mHandCard;//手牌
    private bool showCard = false;//是否亮牌
    private NNPlayerHandCardsType mCardsType;//牌型

    private Vector3 mWinScorePos;
    #endregion

    #region Unity 


    protected override void Start()
    {
        base.Start();
        mWinScorePos = mWinLoseLabel.transform.localPosition;
    }

    #endregion

    #region 字段

    /// <summary>
    /// 座位号属性
    /// </summary>
    public int SeatId
    {
        get
        {
            return mSeatId;
        }
        set
        {
            mSeatId = value;
        }
    }

    #endregion


    #region ui事件

    /// <summary>
    /// 头像点击事件
    /// </summary>
    public void OnHeadClick()
    {
        if (SeatId>0 && NiuniuModel.Inst.mGameedSeatIdList.Contains(SeatId)) {
            Global.Inst.GetController<NNGameController>().SendGetPlayerInfo(NiuniuModel.Inst.mPlayerInfoDic[mSeatId].userId, mSeatId);
        }        
    }


    #endregion

    #region 外部调用

    /// <summary>
    /// 清理玩家
    /// </summary>
    public void CleanPlayer()
    {
        CleanHandCards();
        if (mBasePlayerInfo != null)
        {
            NGUITools.DestroyImmediate(mBasePlayerInfo.gameObject);
            mBasePlayerInfo = null;
        }
        mCathecticCoinLabel.text = "";
        mCathecticCoinLabel.gameObject.SetActive(false);
        mTxtChatSp.gameObject.SetActive(false);
        mYYVoiceAnim.gameObject.SetActive(false);
        SeatId = 0;
        showCard = false;
        mCardsType = null;
        SetCardType(false, 0, 0);
        SetQiangZhuangResult(false,0);
    }


    /// <summary>
    /// 清除玩家手牌
    /// </summary>
    public void CleanHandCards()
    {
        if (mHandCard == null)
        {
            mHandCard = new List<string>();
        }
        mHandCard.Clear();
        NGUITools.DestroyChildren(mHandCardGid.transform);
        showCard = false;
        mCardsType = null;

    }

    /// <summary>
    /// 初始玩家
    /// </summary>
    /// <param name="head"></param>
    /// <param name="name"></param>
    /// <param name="uid"></param>
    /// <param name="score"></param>
    /// <param name="ready"></param>
    /// <param name="offline"></param>
    /// <param name="discard"></param>
    public void InitPlayer(string head, string name, string uid, float score, bool ready = false, bool offline = false)
    {
        if (mBasePlayerInfo == null)
        {
            //mBasePlayerInfo = Assets.InstantiateChild(mHeadParent, Global.Inst.GetController<NNGameController>().mView.mPlayerBaseItem.gameObject).GetComponent<NiuniuBasePlayerInfo>();
        }
        mBasePlayerInfo.transform.localPosition = Vector3.zero;

        mBasePlayerInfo.gameObject.SetActive(true);
        mBasePlayerInfo.InitUI(head, name, uid, score, ready, offline);
    }

    /// <summary>
    /// 得到显示下注的位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetChePosition() {
        return mCathecticCoinLabel.gameObject.transform.position;
    }


    public Vector3 GetBaseInfoPos() {
        return mBasePlayerInfo.transform.position;
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
    /// 设置牌醒
    /// </summary>
    /// <param name="type"></param>
    public void SetCardsType(NNPlayerHandCardsType type) {
        mCardsType = type;
    }

    /// <summary>
    /// 得到牌型
    /// </summary>
    /// <returns></returns>
    public NNPlayerHandCardsType GetCardsType() {
        return mCardsType;
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

    /// <summary>
    /// 设置庄家状态
    /// </summary>
    /// <param name="show"></param>
    public void SetZhuangState(bool show)
    {
        mBasePlayerInfo.SetZhuangState(show);
    }

    /// <summary>
    /// 设置抢庄结果
    /// </summary>
    /// <param name="show"></param>
    /// <param name="result"></param>
    public void SetQiangZhuangResult(bool show, int result) {
        mQZResultLabel.gameObject.SetActive(show);
        mQZResultLabel.spriteName = "word_qiangx" + result;
        mQZResultLabel.MakePixelPerfect();
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
    /// 得到金币或者积分数量
    /// </summary>
    /// <returns></returns>
    public float GetUserScore()
    {
        return mBasePlayerInfo.GetUserScore();
    }

    /// <summary>
    /// 更新总的下注金币
    /// </summary>
    /// <param name="num"></param>
    public void UpdateCathecticCoin(string num)
    {
        mCathecticCoinLabel.text = num;
        if (string.IsNullOrEmpty(num))
        {
            mCathecticCoinLabel.gameObject.SetActive(false);
        }
        else {
            mCathecticCoinLabel.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 设置手牌
    /// </summary>
    /// <param name="cards"></param>
    public void SetHandCard(List<string> cards)
    {
        mHandCard = cards;
    }

    /// <summary>
    /// 返回手牌
    /// </summary>
    /// <returns></returns>
    public List<string> GetHandCard()
    {
        return mHandCard;
    }

    /// <summary>
    /// 设置牌型和倍率
    /// </summary>
    /// <param name="type"></param>
    /// <param name="beilv"></param>
    public void SetCardType(bool show,int type,int beilv) {
        mCardTypeLabel.gameObject.SetActive(show);
        mCardBeiLvLabel.gameObject.SetActive(show);
        mCardTypeLabel.spriteName = "niu_" + type;

        if (show) {
            int sex = NiuniuModel.Inst.mPlayerInfoDic[mSeatId].sex;
            string last = "boy";
            if (sex == 1)
            {
                last = "boy";
            }
            else {
                last = "girl";
            }
            SoundProcess.PlaySound("NN/niu_"+type+"_"+last);
        }

        if (beilv<=0) {
            mCardBeiLvLabel.gameObject.SetActive(false);
        }
        else {
            mCardBeiLvLabel.gameObject.SetActive(true);
            mCardBeiLvLabel.spriteName = "word_x" + beilv;
        }
        mCardBeiLvLabel.MakePixelPerfect();
        mCardBeiLvLabel.MakePixelPerfect();
    }

    /// <summary>
    /// 得到是否亮牌
    /// </summary>
    /// <returns></returns>
    public bool GetTurnState() {
        return showCard;
    }

    /// <summary>
    /// 设置输赢数量
    /// </summary>
    /// <param name="score"></param>
    public void SetWinLoseScore(float score) {
        mWinLoseLabel.gameObject.SetActive(true);
        UISprite sp = mWinLoseLabel.gameObject.GetComponentInChildren<UISprite>();
        if (sp != null)
        {
            sp.spriteName = score > 0 ? "win_bg" : "lose_bg";
        }
        mWinLoseLabel.bitmapFont = score > 0 ? mWinAtlas : mLoseAtlas;
        mWinLoseLabel.text = score > 0 ? "+" + score : score + "";
        TweenPosition p = mWinLoseLabel.gameObject.AddComponent<TweenPosition>();
        p.from = mWinScorePos;
        p.to = mWinScorePos + new Vector3(0, 110, 0);
        p.duration = 0.5f;
        p.AddOnFinished(() =>
        {
            Destroy(p);
            Invoke("DelayHideWinScore", 2.0f);
        });
    }


    /// <summary>
    /// 设置庄家动画
    /// </summary>
    /// <param name="show"></param>
    public void SetRandomZhuangAnimState(bool show)
    {
        mBasePlayerInfo.SetRandomZhuangAnimState(show);
    }

    /// <summary>
    /// 得到庄家图标位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetZhuangPosition()
    {
        return mBasePlayerInfo.GetZhuangPosition();
    }

    #endregion

    #region 发牌

    /// <summary>
    /// 发牌 有动画
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="type"></param>
    public void CastCardWithAnim(int seatId,List<string> cards,bool show, eNNCardsType type = eNNCardsType.Nil) {
        StartCoroutine(IECastCardWithAnim(seatId,cards,show,type));
    }

    /// <summary>
    /// 发牌
    /// </summary>
    /// <param name="seatId"></param>
    /// <param name="cards"></param>
    /// <param name="show"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private IEnumerator IECastCardWithAnim(int seatId, List<string> cards, bool show, eNNCardsType type = eNNCardsType.Nil) {
        if (cards != null)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                GameObject obj = NGUITools.AddChild(mHandCardGid.gameObject, mHandCardItem);
                obj.transform.localScale = Vector3.one;
                obj.SetActive(true);
                NiuniuHandCard card = obj.GetComponent<NiuniuHandCard>();

                if (show)
                {
                    card.ShowCardNum(cards[i]);//显示牌正面
                    card.SetCardDeepsByIndex(GetCardsIndex());////设置牌背层级
                }
                else
                {
                    card.ShowCardBg();//显示牌背
                    card.SetCardBgDeepsByIndex(GetCardsIndex());////设置牌背层级
                }

                obj.transform.position = mDealCardStartPos.position;
                Vector3 dpos = new Vector3(mHandCardGid.cellWidth * GetCardsIndex(), 0, 0);
                bool large = seatId == NiuniuModel.Inst.mMySeatId ? true : false;
                DealCardMove(card, dpos, 0.2f, large);
                mHandCard.Add(cards[i]);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    /// <summary>
    ///  直接生成牌，无动画
    /// </summary>
    /// <param name="seatId"></param>
    /// <param name="bgCards"></param>
    /// <param name="mingCards"></param>
    public void InstantiateCards(int seatId, List<string> bgCards, bool show = false, List<string> mingCards = null)
    {
        if (mingCards != null)
        {
            for (int i = 0; i < mingCards.Count; i++)
            {
                if (seatId == NiuniuModel.Inst.mMySeatId) {
                    DirectAddOneHandCard(seatId, GetCardsIndex(), mingCards[i], true);
                }
                else {
                    DirectAddOneHandCard(seatId, GetCardsIndex(), mingCards[i], false);
                }
            }
        }

        if (bgCards != null)
        {
            for (int i = 0; i < bgCards.Count; i++)
            {
                DirectAddOneHandCard(seatId, GetCardsIndex(), bgCards[i], show);
            }
        }
    }

    #endregion



    #region 辅助函数


    /// <summary>
    /// 发牌延迟移动
    /// </summary>
    /// <param name="card"></param>
    /// <param name="dpos"></param>
    /// <param name="time"></param>
    /// <param name="delayTime"></param>
    /// <param name="isEnlarge"></param>
    private void DealCardMove(NiuniuHandCard card, Vector3 dpos, float time, bool isEnlarge)
    {
        card.Move(dpos, time, isEnlarge);//移动到目标位置
    }


    /// <summary>
    /// 直接添加一张手牌
    /// </summary>
    /// <param name="value"></param>
    /// <param name="show"></param>
    private void DirectAddOneHandCard(int seatId, int index,string value , bool show = false)
    {
        GameObject obj = obj = NGUITools.AddChild(mHandCardGid.gameObject, mHandCardItem);
        NiuniuHandCard card = obj.GetComponent<NiuniuHandCard>();
        obj.gameObject.SetActive(true);

        if (seatId == NiuniuModel.Inst.mMySeatId)
        {
            obj.transform.localScale = new Vector3(2f, 2f, 2f);
        }

        if (value != "0" && !string.IsNullOrEmpty(value))
        {
            card.SetCard(value);
        }

        if (show)
        {
            card.ShowCardNum(value);
        }
        else
        {
            card.ShowCardBg();
        }
        mHandCard.Add(value);

        mHandCardGid.Reposition();
    }

    /// <summary>
    /// 得到这张牌是第几场牌
    /// </summary>
    /// <returns></returns>
    private int GetCardsIndex() {
        if (mHandCard==null) {
            mHandCard = new List<string>();
        }
        return mHandCard.Count;
    }

    /// <summary>
    /// 隐藏输赢接货
    /// </summary>
    private void HideWinLoseLabel() {
        mWinLoseLabel.gameObject.SetActive(false);
    }


    #endregion

    #region 翻牌和移动牌

    /// <summary>
    /// 翻牌
    /// </summary>
    /// <param name="cards"></param>
    public void TurnCards(List<string> cards) {
        showCard = true;
        if (cards.Count == 5) {
            mHandCard = cards;
        }
        List<Transform> list = mHandCardGid.GetChildList();
        for (int i =0;i<list.Count;i++) {
            NiuniuHandCard handCard = list[i].GetComponent<NiuniuHandCard>();
            if (handCard != null) {
                handCard.SetCard(cards[i]);
                if (mHandCardGid.cellWidth > 0)
                {
                    handCard.SetCardDeepsByIndex(i);
                }
                else {
                    handCard.SetCardDeepsByIndex(list.Count-i);
                }
                handCard.TurnCard(0.3f, true);
            }
        }
    }


    /// <summary>
    /// 翻牌后移动牌
    /// </summary>
    /// <param name="cards"></param>
    public void SeparateCards(List<int> three)
    {
        Vector3 finalPos = Vector3.zero;
        if (three == null || three.Count <= 0)
        {
            for (int i = 0; i < mHandCard.Count; i++)
            {
                if (SeatId == NiuniuModel.Inst.mMySeatId)
                {
                    finalPos = new Vector3(100 * i * (mHandCardGid.cellWidth / Mathf.Abs(mHandCardGid.cellWidth)), 0, 0);
                }
                else
                {
                    finalPos = new Vector3(50 * i * (mHandCardGid.cellWidth / Mathf.Abs(mHandCardGid.cellWidth)), 0, 0);
                }
                NiuniuHandCard card = mHandCardGid.transform.GetChild(i).GetComponent<NiuniuHandCard>();
                card.Move(finalPos, .5f);//移动到最终位置
            }
        }
        else {
            List<int> newThree = new List<int>();
            List<int> newTwo = new List<int>();
            List<int> result = new List<int>();


            for (int i=0;i<three.Count;i++) {
                newThree.Add(three[i]-1);
                if (SeatId == NiuniuModel.Inst.mMySeatId)
                {
                    finalPos = new Vector3(100 * i * (mHandCardGid.cellWidth / Mathf.Abs(mHandCardGid.cellWidth)), 0, 0);
                }
                else
                {
                    finalPos = new Vector3(50 * i * (mHandCardGid.cellWidth / Mathf.Abs(mHandCardGid.cellWidth)), 0, 0);
                }
                NiuniuHandCard card = mHandCardGid.transform.GetChild(newThree[i]).GetComponent<NiuniuHandCard>();
                card.Move(finalPos, .5f);//移动到最终位置
            }

            int temp = 0;

            for (int i = 0; i < mHandCard.Count;i++) {
                if (!newThree.Contains(i)) {
                    newTwo.Add(i);
                    if (SeatId == NiuniuModel.Inst.mMySeatId)
                    {
                        finalPos = new Vector3(100 * (newThree.Count + 1 + temp) * (mHandCardGid.cellWidth / Mathf.Abs(mHandCardGid.cellWidth)), 0, 0);
                    }
                    else
                    {
                        finalPos = new Vector3(50 * (newThree.Count + 1 + temp) * (mHandCardGid.cellWidth / Mathf.Abs(mHandCardGid.cellWidth)), 0, 0);
                    }
                    NiuniuHandCard card = mHandCardGid.transform.GetChild(newTwo[temp]).GetComponent<NiuniuHandCard>();
                    card.Move(finalPos, .5f);//移动到最终位置
                    temp++;
                }
            }
        }

    }

    #endregion

    #region 聊天
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
    protected void PlayVoiceChat(SendReceiveGameChat chat)
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
    protected void PlayYYVoiceAnimaion(SendReceiveGameChat chat)
    {
        mYYVoiceAnim.gameObject.SetActive(true);
        mYYVoiceAnim.SetBegin("yx_yy0", 1, 3, 50000, 0.25f);
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
    protected void PlayTxtVoiceChat(SendReceiveGameChat chat)
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
                NNPlayerInfo player = null;

                if (NiuniuModel.Inst.mPlayerInfoDic.TryGetValue(chat.fromSeatId,out player)) {
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
    protected void PlayHuDongFace(SendReceiveGameChat chat)
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

        NiuniuGameView view = Global.Inst.GetController<NNGameController>().mView;

        NiuniuPlayerUI fromPlayer = null;
        NiuniuPlayerUI toPlayer = null;

        if (view.TryGetPlayer(chat.fromSeatId, out fromPlayer) && view.TryGetPlayer(chat.toSeatId, out toPlayer))
        {
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
    private void DelayHideWinScore()
    {
        mWinLoseLabel.gameObject.SetActive(false);
        mWinLoseLabel.transform.localPosition = mWinScorePos;
    }

    #endregion
}
