using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XXGoldFlowerGameModel : BaseModel {

    public static XXGoldFlowerGameModel Inst;

    public override void SetController(BaseController c)
    {
        base.SetController(c);
        Inst = this;
    }
    /// <summary>
    /// 房间状态
    /// </summary>
    public eGFGameState RoomState = eGFGameState.Ready;

    /// <summary>
    /// 房间号
    /// </summary>
    public string mRoomId;
    /// <summary>
    /// 房间规则
    /// </summary>
    public SendCreateRoomReq mRoomRules;

    /// <summary>
    /// 底池
    /// </summary>
    public float mDichi;

    /// <summary>
    /// 底分
    /// </summary>
    public float mDiFen;

    /// <summary>
    /// 创建放假时左侧的id
    /// </summary>
    public int mSubGameId;

    /// <summary>
    /// 第几套牌
    /// </summary>
    public int mCardType = 1;
    /// <summary>
    /// 座位号数组
    /// </summary>
    public List<int> mSeatIdList = new List<int>();

    /// <summary>
    /// 我的座位号
    /// </summary>
    public int mMySeatId;

    /// <summary>
    /// 轮到谁操作
    /// </summary>
    public int mTurnSeatId;

    /// <summary>
    /// 庄家座位号
    /// </summary>
    public int mZhuangSeatId;

    /// <summary>
    /// 当前第几轮
    /// </summary>
    public int mRound;

    /// <summary>
    /// 是否正在比牌
    /// </summary>
    public bool mComparingCard = false;

    /// <summary>
    /// 是否可以看牌
    /// </summary>
    public bool mCanLookCard = false;

    /// <summary>
    /// 游戏开始数据
    /// </summary>
    public OnGoldFlowerGameStart mStartInfo;

    /// <summary>
    /// 是否是金币场
    /// </summary>
    public bool mGoldPattern = false;

    /// <summary>
    /// 是否看牌了
    /// </summary>
    public bool mLookCard = false;

    /// <summary>
    /// 是否参与了游戏
    /// </summary>
    public bool mGameed = false;

    /// <summary>
    /// 是否换桌
    /// </summary>
    public bool mChangDesk = false;

    /// <summary>
    /// 是否正在搓牌中
    /// </summary>
    public bool mCuoCarding = false;

    /// <summary>
    /// 玩家信息数组
    /// </summary>
    public Dictionary<int, GoldFlowerPlayer> mPlayerInfoDic = new Dictionary<int, GoldFlowerPlayer>();

    /// <summary>
    /// 有手牌的座位号集合
    /// </summary>
    public List<int> mHasCardSeatList = new List<int>();

    /// <summary>
    /// 当前接受到的操作
    /// </summary>
    public GoldFlowerOpt mOpt;

    /// <summary>
    /// 自己看牌的数据
    /// </summary>
    public OnGoldFlowerPlayerOptResult mSelfLookCard;

    /// <summary>
    /// 是否自动更著
    /// </summary>
    public bool mAutoGen = false;

    /// <summary>
    /// 下注的金币的sp
    /// </summary>
    private List<string> mCoinName = new List<string> { "coin_01", "coin_02", "coin_03", "coin_04" };

    /// <summary>
    /// 清理数据
    /// </summary>
    public void CleanMode(bool depth = false) {
        mComparingCard = false;
        mCanLookCard = false;
        mLookCard = false;
        mGameed = false;
        mTurnSeatId = 0;
        mZhuangSeatId = 0;
        mOpt = null;
        mChangDesk = false;
        mAutoGen = false;
        mHasCardSeatList.Clear();
        mSelfLookCard = null;
        mCuoCarding = false;
        RoomState = eGFGameState.Ready;
        if (depth) {
            mPlayerInfoDic.Clear();
            mSeatIdList.Clear();
        }

        List<int> list = new List<int>();
        list.AddRange(mPlayerInfoDic.Keys);
        for (int i=0;i< list.Count;i++) {
            mPlayerInfoDic[list[i]].discard = false;
        }
    }

    /// <summary>
    /// 得到下注的金币的图片名字
    /// </summary>
    /// <param name="lookCard"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public string GetCoinSpriteName(bool lookCard, float value) {
        List<float> temp = new List<float>();
        if (lookCard)
        {
            for (int i = 1; i < mStartInfo.lookRate.Count;i++) {
                temp.Add(mStartInfo.lookRate[i]);
            }
        }
        else {
            for (int i = 1; i < mStartInfo.lookRate.Count; i++)
            {
                temp.Add(mStartInfo.menRate[i]);
            }
        }

        string name = "";

        for (int i=1;i<temp.Count-1;i++) {
            if (value >= temp[temp.Count - 1])
            {
                name = mCoinName[3];
            }
            else if (value < temp[1])
            {
                name = mCoinName[0];
            }
            else if (value < temp[i + 1] && value >= temp[i])
            {
                name = mCoinName[i];
            }
            else
                continue;
        }
        return name;
    }

}
