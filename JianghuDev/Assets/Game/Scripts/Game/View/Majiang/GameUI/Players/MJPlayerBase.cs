using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using YunvaIM;

public enum PlayerPosType
{
    Self,//自己
    Right,//右边玩家
    Top,//对家
    Left,//左边玩家
}

public class MJPlayerBase : BaseViewWidget
{
    public MjPlayerHead mHead;//头像信息
    public MjPlayerBase mPlayer;//玩家牌
    public Camera mCamera;
    public UISprite mOffLine;//离线
    public GameObject mTingTag;//听牌标记
    public GameObject mHuTag;//胡牌标记
    public UISprite mDingQue;//定缺的标记
    public TweenScale mDingqueAnim;//定缺动画
    public Transform mDingqueAnimRoot;//定缺动画root
    public UISprite mTips;//提示
    public GameObject mTrustShipTag;//托管标记
    public GameObject mIsGiveUpTag;//是否认输

    public GameObject mZhuangSp;//庄家
    public Texture2D mHuTexture;
    public Texture2D mZiMoTexture;

    public eChatTextDirectionType mDType;//文字聊天方向类型
    public Transform mHudongRoot;//互动表情root
    public Transform mFaceRoot;//表情root
    public Transform mTextRoot;//文字聊天root
    public Transform mVoiceRoot;//语音root
    public UISprite mChaJiao;//查叫    child 1  花猪  2 退税  3 没叫
    public UILabel mChajiaoScoreAdd;//查叫加分
    public UILabel mChajiaoScoreSubstract;//查叫减分
    public UISprite mZhuangSprite;//庄家图标
    

    private bool mIsPlayVoice;//是否正在播放语音


    public PlayerPosType mPlayerPosType;//玩家位置类型
    public int mMyseat;//我的座位号



    public virtual void ResetUI(bool depth = false)
    {
        SetTingTag(false);
        SetHuTag(false);
        SetDingqueShow(false);
        mZhuangSprite.gameObject.SetActive(false);
        SetTrustShipTagShow(false);
        SetIsGiveUp(false);
        SetChajiaoScores(0, eEff.NONE, 0, false);
        mHead.SetCD(0);
        mZhuangSp.SetActive(false);
        if(depth)
            mHead.gameObject.SetActive(!depth);
    }

    #region 手牌相关
    /// <summary>
    /// 初始化手牌
    /// </summary>
    /// <param name="cards">手牌</param>
    /// <param name="count">手牌数量（是自己可以任意填）</param>
    /// <param name="curcard">当前摸在手上的牌（>0表示有）</param>
    public virtual void InitHandCards(List<int> cards, int count, int curcard = 1)
    {
        mPlayer.InitHandCards(cards, count, curcard);
    }
    /// <summary>
    /// 结算前 刷新手牌
    /// </summary>
    /// <param name="cards"></param>
    public void GetPlayerHandCards(List<int> cards, bool isSelf)
    {
        mPlayer.GetAllPlayerHandCards(cards, isSelf);
    }

    /// <summary>
    /// 初始化手牌，除自己外的玩家
    /// </summary>
    /// <param name="count">手牌数量</param>
    /// <param name="isCurCard">是否有摸起来的牌</param>
    public virtual void InitHandCards(int count, bool isCurCard)
    {
        int card = isCurCard ? 1 : 0;
        InitHandCards(null, count, card);
    }


    protected virtual void AddOneHandCard(int num = -1)
    {
    }
    #endregion

    #region 打出的牌

    public virtual void InitCollectCard(List<int> cards)
    {
        mPlayer.InitCollectCard(cards);
    }

    /// <summary>
    /// 初始化胡牌
    /// </summary>
    /// <param name="card"></param>
    public void InitHuPai(int card)
    {
        if (card <= 0)
        {
            mPlayer.mHuCard.gameObject.SetActive(true);
            return;
        }
        mPlayer.mHuCard.gameObject.SetActive(true);
        mPlayer.mHuCard.SetNum(card, mPlayerPosType);
    }

    public void InitHuPai(List<HuStruct> cards)
    {
        if (cards.Count <= 0)
        {
            mPlayer.mHuCard.gameObject.SetActive(true);
            return;
        }
        mPlayer.mHuCard.gameObject.SetActive(true);
        foreach (var item in cards)
        {
            mPlayer.mHuCard.SetNum(item.card, mPlayerPosType);
        }


    }
    /// <summary>
    /// 设置当前打出牌的特效
    /// </summary>
    /// <param name="spr"></param>
    public void SetCurOutCardEffect(bool isshow)
    {
        mPlayer.SetCurOutCardShow(isshow);
    }
    #endregion

    #region 碰或者杠
    public void InitPengCards(eMJInstructionsType instype, List<PengStruct> cards)
    {
        mPlayer.InitPengCards(instype, cards);
    }

    public void InitGangCards(eMJInstructionsType instype, List<GangStruct> cards)
    {
        mPlayer.InitGangCard(instype, cards);
    }
    public void InitHuCards(eMJInstructionsType instype, List<HuStruct> cards, int huType=-1)
    {
        if (cards != null && cards.Count > 0)
        {
            mHuTag.gameObject.SetActive(true);
            UITexture a= mHuTag.GetComponent<UITexture>();
            if (a!=null) {
                a.mainTexture = mHuTexture;
            }
            if (huType == (int)eHuType.ZIMO) {
                //显示成自摸
                if (a != null)
                {
                    a.mainTexture = mZiMoTexture;
                }
            }
            a.MakePixelPerfect();
        }
        mPlayer.InitHuCard(instype, cards);
    }

    #endregion

    /// <summary>
    /// 设置托管显示与否
    /// </summary>
    /// <param name="isshow"></param>
    public virtual void SetTrustShipTagShow(bool isshow)
    {
        mTrustShipTag.SetActive(isshow);
    }

    /// <summary>
    /// 设置庄家图标
    /// </summary>
    /// <param name="show"></param>
    public void SetZhuangState(bool show) {
        mZhuangSp.gameObject.SetActive(show);
    }

    /// <summary>
    /// 是否认输
    /// </summary>
    /// <param name="isshow"></param>
    public virtual void SetIsGiveUp(bool isshow)
    {
        mIsGiveUpTag.SetActive(isshow);
    }

    #region 准备
    /// <summary>
    /// 设置准备
    /// </summary>
    /// <param name="isready"></param>
    public void SetReady(bool isready)
    {
        mHead.SetReady(isready);
    }

    /// <summary>
    /// 开始时头像位置
    /// </summary>
    public void SetStart()
    {
        mHead.SetStart();
    }
    #endregion

    /// <summary>
    /// 定缺显示
    /// </summary>
    public void UpdataFixe(eFixedType type, bool isMy = false, bool isAnim = false)
    {
        if (type == eFixedType.NONE)
        {
            SetDingqueShow(false);
            return;
        }
        string str = "";
        switch (type)
        {
            case eFixedType.WAN:
                str = "万";
                if (isMy)
                    MJGameModel.Inst.eFixe = new int[] { 20, 30 };
                break;
            case eFixedType.TIAO:
                str = "条";
                if (isMy)
                    MJGameModel.Inst.eFixe = new int[] { 0, 10 };
                break;
            case eFixedType.TONG:
                str = "筒";
                if (isMy)
                    MJGameModel.Inst.eFixe = new int[] { 10, 20 };
                break;
        }
        if (isMy)
        {
            MJGameModel.Inst.eFixeType = type;
        }
        if (!isAnim)
            SetDingqueShow(true, str);
        else
            SetDingqueAnim(true, type);
    }

    #region 离开
    /// <summary>
    /// 离开房间
    /// </summary>
    public void GetOutRoom()
    {
        mHead.gameObject.SetActive(false);
        mHead.SetData(null, eMJRoomStatus.READY);
    }
    #endregion

    #region 在线状态
    /// <summary>
    /// 设置在线状态
    /// </summary>
    /// <param name="isOffline"></param>
    public void SetOffLine(eMJOnlineState state)
    {
        if (state == eMJOnlineState.online || state == eMJOnlineState.giveUp)//在线和认输状态时就不显示
            mOffLine.gameObject.SetActive(false);
        else
            mOffLine.gameObject.SetActive(true);
        switch (state)
        {
            case eMJOnlineState.leave:
                mOffLine.spriteName = "exit";
                break;
            case eMJOnlineState.outline:
                mOffLine.spriteName = "offline";
                break;
            case eMJOnlineState.giveUp:
                SetIsGiveUp(true);
                break;
        }
    }
    #endregion

    #region 头像
    /// <summary>
    /// 设置头像
    /// </summary>
    /// <param name="info"></param>
    public void SetIcon(MJplayerInfo info, eMJRoomStatus state)
    {
        mHead.SetData(info, state);
    }
    #endregion

    #region   胡 听牌标记
    /// <summary>
    /// 设置听牌标记
    /// </summary>
    /// <param name="isshow"></param>
    public void SetTingTag(bool isshow)
    {
        mTingTag.SetActive(isshow);
    }
    public void SetHuTag(bool isshow)
    {
        mHuTag.SetActive(isshow);
        UITexture a = mHuTag.GetComponent<UITexture>();
        if (a!=null) {
            a.mainTexture = mHuTexture;
        }
        a.MakePixelPerfect();
    }

    public void SetZiMoHu() {
        mHuTag.gameObject.SetActive(true);
        //替换胡牌类型图片
        UITexture a = mHuTag.GetComponent<UITexture>();
        if (a != null)
        {
            a.mainTexture = mZiMoTexture;
        }
        a.MakePixelPerfect();
    }
    #endregion

    #region cd
    /// <summary>
    /// 设置cd
    /// </summary>
    /// <param name="t"></param>
    public void SetCD(float t)
    {
        mHead.SetCD(t);
    }
    #endregion

    #region 聊天
    /// <summary>
    /// 获取聊天位置
    /// </summary>
    /// <param name="chattype">聊天类型</param>
    /// <returns></returns>
    public Vector3 GetChatPos(eGameChatContentType chattype)
    {
        switch (chattype)
        {
            case eGameChatContentType.HDFace:
                return mHudongRoot.position;
            case eGameChatContentType.TexTVoice:
                return mTextRoot.position;
            case eGameChatContentType.Voice:
                return mVoiceRoot.position;
        }
        return transform.position;
    }

    /// <summary>
    /// 获取文字语音方向
    /// </summary>
    /// <returns></returns>
    public eChatTextDirectionType GetChatDirection()
    {
        return mDType;
    }
    #endregion

    /*******************************************************************************************************/
    public void SetIcon(PlayerInfoStruct info)
    {
        mHead.SetPlayerIcon(info);
    }

    #region 换三张
    /// <summary>
    /// 换3张处理手牌
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="count"></param>
    /// <param name="curcard"></param>
    public virtual void InitChaneThreeHandCards(List<int> data)
    {
        mPlayer.InitChaneThreeHandCards(data);
    }
    /// <summary>
    /// 换3张确定后的事件处理
    /// </summary>
    /// <param name="data"></param>
    public void InitSureChaneThree(MJoptInfoData data)
    {
        mPlayer.InitSureChaneThree(data);
    }

    /// <summary>
    /// 设置换三张状态是否显示
    /// </summary>
    /// <param name="isshow"></param>
    public void SetChangeStateShow(bool isshow)
    {
        mTips.gameObject.SetActive(isshow);
        if (isshow)
            mTips.spriteName = "label_change";
    }
    /// <summary>
    /// 设置换三张牌数值
    /// </summary>
    /// <param name="cards"></param>
    public void SetChangeThreeCardsNum(List<int> cards)
    {
        if (cards == null)
            return;
        mPlayer.SetChangeThreeCardsNum(cards);
    }
    #endregion
    #region 定缺
    /// <summary>
    /// 设置定缺显示
    /// </summary>
    /// <param name="isShow">是否显示定缺</param>
    /// <param name="text">定缺文字</param>
    public void SetDingqueShow(bool isShow, string text = "")
    {
        mDingQue.gameObject.SetActive(false);
        mTips.gameObject.SetActive(false);

        switch (text)
        {
            case "条":
                mDingQue.gameObject.SetActive(true);
                mDingQue.spriteName = "fix_tiao2";
                break;
            case "筒":
                mDingQue.gameObject.SetActive(true);
                mDingQue.spriteName = "fix_tong2";
                break;
            case "万":
                mDingQue.gameObject.SetActive(true);
                mDingQue.spriteName = "fix_wan2";
                break;
            case "定缺中":
                mTips.gameObject.SetActive(true);
                mTips.spriteName = "label_dingque";
                break;
        }
    }

    /// <summary>
    /// 显示定缺动画
    /// </summary>
    /// <param name="isshow"></param>
    /// <param name="text"></param>
    public void SetDingqueAnim(bool isshow, eFixedType ftype)
    {
        mDingQue.gameObject.SetActive(false);
        mTips.gameObject.SetActive(false);
        mDingqueAnim.gameObject.SetActive(isshow);
        string str = "";
        if(isshow)
        {
            string spname = "";
            switch (ftype)
            {
                case eFixedType.TIAO:
                    spname = "tiao_big";
                    str = "条";
                    break;
                case eFixedType.TONG:
                    spname = "tong_big";
                    str = "筒";
                    break;
                case eFixedType.WAN:
                    spname = "wan_big";
                    str = "万";
                    break;
            }
            mDingqueAnim.transform.position = mDingqueAnimRoot.position;
            mDingqueAnim.GetComponent<UISprite>().spriteName = spname;
            mDingqueAnim.ResetToBeginning();
            mDingqueAnim.PlayForward();
            iTween.MoveTo(mDingqueAnim.gameObject, iTween.Hash("position", mDingQue.transform.position, "time", 0.5f, "islocal", false, "easetype", iTween.EaseType.linear));
            DelayRun(0.51f, () =>
            {
                mDingqueAnim.gameObject.SetActive(false);
                SetDingqueShow(true, str);
            });
        }
    }

    #endregion
    public void ChangeThreeObj(bool isShow)
    {
        mPlayer.ChangeThreeObj(isShow);
    }
    public void RotaThreeObj(int t, float augle)
    {
        mPlayer.RotaThreeObj(t, augle);
    }
    /// <summary>
    ///获取查叫信息
    /// </summary>
    public void SetChajiaoScores(int seatid, eEff etype, float score, bool isshow = true)
    {
        mChaJiao.gameObject.SetActive(isshow);
        if (!isshow)
            return;
        float allscore = MJGameModel.Inst.mRoomPlayers[seatid].gold + score;
        MJGameModel.Inst.mRoomPlayers[seatid].gold = allscore;
        switch (etype)
        {
            case eEff.NONE://普通分数
                mChaJiao.spriteName = "";
                break;
            case eEff.HUAZHU://花猪
                mChaJiao.spriteName = "label_hz";
                break;
            case eEff.PEIYU://赔雨
                mChaJiao.spriteName = "label_ts";
                break;
            case eEff.CHAJIAO://查叫
                mChaJiao.spriteName = "label_mj";
                break;
            default:
                mChaJiao.spriteName = "";
                break;
        }
        if (score > 0)
        {
            mChajiaoScoreAdd.text = "+" + score;
            mChajiaoScoreSubstract.text = "";
        }
        else if (score < 0)
        {
            mChajiaoScoreAdd.text = "";
            mChajiaoScoreSubstract.text = score.ToString();
        }
        mHead.SetPoint(allscore);
    }

    #region 查看胡牌
    public virtual void SetHuPromptCard(List<CanHuStruct> canHuList, Vector3 vec3, bool isShow)
    {

    }
    #endregion
}
