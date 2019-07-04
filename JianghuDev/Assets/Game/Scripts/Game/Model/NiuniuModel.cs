using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NiuniuModel : BaseModel {


    public static NiuniuModel Inst;

    public override void SetController(BaseController c)
    {
        base.SetController(c);
        Inst = this;
    }

    /// <summary>
    /// 游戏类型
    /// </summary>
    public eGameType mGameId;

    /// <summary>
    /// 字游戏类型
    /// </summary>
    public int mSubGameId;

    /// <summary>
    /// 房间号
    /// </summary>
    public string mRoomId;

    /// <summary>
    /// 房间规则
    /// </summary>
    public SendCreateRoomReq mRoomRules;

    /// <summary>
    /// 自己座位号
    /// </summary>
    public int mMySeatId;

    /// <summary>
    /// 游戏状态
    /// </summary>
    public eNNGameState mGameState;

    /// <summary>
    /// 房间状态 0是还没开始过任何一小局 1是开始过了
    /// </summary>
    public int mRoomState;

    /// <summary>
    /// 庄家座位号
    /// </summary>
    public int mZhuangSeatId;

    /// <summary>
    /// 是否是金币场
    /// </summary>
    public bool mGoldPattern = false;

    /// <summary>
    /// 我是否参与了游戏
    /// </summary>
    public bool mGameed = false;

    /// <summary>
    /// 是否换桌
    /// </summary>
    public bool mChangeDesk = false;

    /// <summary>
    /// 是否看了牌
    /// </summary>
    public bool mLookCard = false;

    /// <summary>
    /// 座位号数组
    /// </summary>
    public List<int> mSeatList = new List<int>();

    /// <summary>
    /// 玩家信息
    /// </summary>
    public Dictionary<int, NNPlayerInfo> mPlayerInfoDic = new Dictionary<int, NNPlayerInfo>();

    /// <summary>
    /// 抢庄选择
    /// </summary>
    public List<int> mQzListValue = new List<int>();

    /// <summary>
    /// 下注选择
    /// </summary>
    public List<float> mXzListValue = new List<float>();

    /// <summary>
    /// 参与游戏的座位号
    /// </summary>
    public List<int> mGameedSeatIdList = new List<int>();


    /// <summary>
    /// 清理数据
    /// </summary>
    /// <param name="depth"></param>
    public void CleanModel(bool depth =false) {
        if (mQzListValue==null) {
            mQzListValue = new List<int>();
        }
        if (mXzListValue == null) {
            mXzListValue = new List<float>();
        }
        mQzListValue.Clear();
        mXzListValue.Clear();
        mGameedSeatIdList.Clear();
        mZhuangSeatId = 0;

        mGameed = false;
        mChangeDesk = false;
        mLookCard = false;

        mGameState = eNNGameState.Ready;
        if (depth) {
            mPlayerInfoDic.Clear();
            mSeatList.Clear();
        }
    }

}
