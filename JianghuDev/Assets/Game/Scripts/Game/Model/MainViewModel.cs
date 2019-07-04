using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainViewModel : BaseModel
{

    public static MainViewModel Inst;

    public override void SetController(BaseController c)
    {
        base.SetController(c);
        Inst = this;
    }


    #region 当前连接的ip和端口

    /// <summary>
    /// 当前ip
    /// </summary>
    public string mNowIp;


    /// <summary>
    /// 当前端口
    /// </summary>
    public int mNowPort;

    #endregion

    /// <summary>
    /// 活动数据
    /// </summary>
    public List<SendGetActivityDataList> ActivityList;
    /// <summary>
    /// 广播信息
    /// </summary>
    public List<string> BroadMessage = new List<string>();//

    /// <summary>
    /// 积分榜
    /// </summary>
    public List<SendGetRankInfo> PointRankList = new List<SendGetRankInfo>();
    /// <summary>
    /// 当前查看盈利的页数
    /// </summary>
    public int mCurGainPage = 0;
    /// <summary>
    /// 当前盈利的类型
    /// </summary>
    public string mCurGainType = "";
    /// <summary>
    /// 当前查看的盈利数据
    /// </summary>
    public List<SendGetGainInfo> mCurGainList = new List<SendGetGainInfo>();

    /// <summary>
    /// 当前查看的代理打赏获赏记录的页数
    /// </summary>
    public int mCurAgentPage = 0;
    /// <summary>
    /// 当前查看的代理打赏获赏记录的类型
    /// </summary>
    public string mCurAgentType = "";
    /// <summary>
    /// 当前查看的代理打赏获赏记录
    /// </summary>
    public List<SendGetAgentBRInfo> mCurAgentList = new List<SendGetAgentBRInfo>();

    #region 玩家详细记录
    /// <summary>
    /// 当前查看的代理打赏获赏记录的页数
    /// </summary>
    public int mCurPlayerDetailPage = 0;
    /// <summary>
    /// 当前查看的代理打赏获赏记录
    /// </summary>
    public List<SendGetAgentBRInfo> mCurPlayerDetailList = new List<SendGetAgentBRInfo>();
    #endregion

    /// <summary>
    /// 当前查看的玩家打赏获赏记录的页数
    /// </summary>
    public int mCurUserPage = 0;
    /// <summary>
    /// 当前查看的玩家打赏获赏记录的类型
    /// </summary>
    public string mCurUserType = "";
    /// <summary>
    /// 当前查看的玩家打赏获赏记录
    /// </summary>
    public List<SendGetUserBRInfo> mCurUserList = new List<SendGetUserBRInfo>();

    public float mNextHudongFaceTime;//下一次互动表情时间

    /// <summary>
    /// 当前查看的玩家打赏获赏记录的页数
    /// </summary>
    public int mCurMessagePage = 0;
    /// <summary>
    /// 当前查看的玩家打赏获赏记录
    /// </summary>
    public List<SendGetMessageInfo> mCurMessageList = new List<SendGetMessageInfo>();

    /// <summary>
    /// 更新盈利数据
    /// </summary>
    /// <param name="newType"></param>
    /// <param name="newPage"></param>
    /// <param name="newList"></param>
    public void UpdateGainList(string newType, int newPage, List<SendGetGainInfo> newList)
    {

        if (!string.IsNullOrEmpty(mCurGainType) && newType != mCurGainType)
        {//切换类型
            mCurGainList.Clear();
        }
        if (newList != null)
        {
            if (newType == mCurGainType)//更新当前类型的数据
            {
                if (newPage == 1)
                {//插入到最开始
                    for (int i = newList.Count - 1; i >= 0; i--)
                    {
                        bool contain = false;
                        for (int j = 0; j < mCurGainList.Count; j++)
                        {
                            if (newList[i].messageId == mCurGainList[j].messageId)
                            {//如果老的数据里面有这个数据,就不用把这个数据加入model
                                contain = true;
                                continue;
                            }
                        }
                        if (!contain)
                        {//不包含
                            mCurGainList.Insert(0, newList[i]);
                        }
                    }
                }
                else//插入到尾部
                {
                    for (int i = 0; i < newList.Count; i++)
                    {
                        bool contain = false;
                        for (int j = 0; j < mCurGainList.Count; j++)
                        {
                            if (newList[i].messageId == mCurGainList[j].messageId)
                            {//如果老的数据里面有这个数据,就不用把这个数据加入model
                                contain = true;
                                continue;
                            }
                        }
                        if (!contain)
                        {//不包含
                            mCurGainList.Add(newList[i]);
                        }
                    }
                }
            }
            else
            {
                mCurGainPage = newPage;
                mCurGainType = newType;
                mCurGainList = newList;
            }
        }
    }


    /// <summary>
    /// 更新盈利数据
    /// </summary>
    /// <param name="newType"></param>
    /// <param name="newPage"></param>
    /// <param name="newList"></param>
    public void UpdateAgentList(string newType, int newPage, List<SendGetAgentBRInfo> newList)
    {

        if (!string.IsNullOrEmpty(mCurAgentType) && newType != mCurAgentType)
        {//切换类型
            mCurAgentList.Clear();
        }
        if (newList != null)
        {
            if (newType == mCurAgentType)//更新当前类型的数据
            {
                if (newPage == 1)
                {//插入到最开始
                    for (int i = newList.Count - 1; i >= 0; i--)
                    {
                        bool contain = false;
                        for (int j = 0; j < mCurAgentList.Count; j++)
                        {
                            if (newList[i].messageId == mCurAgentList[j].messageId)
                            {//如果老的数据里面有这个数据,就不用把这个数据加入model
                                contain = true;
                                continue;
                            }
                        }
                        if (!contain)
                        {//不包含
                            mCurAgentList.Insert(0, newList[i]);
                        }
                    }
                }
                else//插入到尾部
                {
                    for (int i = 0; i < newList.Count; i++)
                    {
                        bool contain = false;
                        for (int j = 0; j < mCurAgentList.Count; j++)
                        {
                            if (newList[i].messageId == mCurAgentList[j].messageId)
                            {//如果老的数据里面有这个数据,就不用把这个数据加入model
                                contain = true;
                                continue;
                            }
                        }
                        if (!contain)
                        {//不包含
                            mCurAgentList.Add(newList[i]);
                        }
                    }
                }
            }
            else
            {
                mCurAgentPage = newPage;
                mCurAgentType = newType;
                mCurAgentList = newList;
            }
        }
    }

    #region 玩家详细信息
    /// <summary>
    /// 玩家详细信息
    /// </summary>
    /// <param name="newType"></param>
    /// <param name="newPage"></param>
    /// <param name="newList"></param>
    public void UpdatePlayerRecordDetailList( int newPage, List<SendGetAgentBRInfo> newList)
    {

        if (newList != null)
        {
            if (newPage == 1)
            {//插入到最开始
                for (int i = newList.Count - 1; i >= 0; i--)
                {
                    bool contain = false;
                    for (int j = 0; j < mCurPlayerDetailList.Count; j++)
                    {
                        if (newList[i].messageId == mCurPlayerDetailList[j].messageId)
                        {//如果老的数据里面有这个数据,就不用把这个数据加入model
                            contain = true;
                            continue;
                        }
                    }
                    if (!contain)
                    {//不包含
                        mCurPlayerDetailList.Insert(0, newList[i]);
                    }
                }
            }
            else//插入到尾部
            {
                for (int i = 0; i < newList.Count; i++)
                {
                    bool contain = false;
                    for (int j = 0; j < mCurPlayerDetailList.Count; j++)
                    {
                        if (newList[i].messageId == mCurPlayerDetailList[j].messageId)
                        {//如果老的数据里面有这个数据,就不用把这个数据加入model
                            contain = true;
                            continue;
                        }
                    }
                    if (!contain)
                    {//不包含
                        mCurPlayerDetailList.Add(newList[i]);
                    }
                }
            }
            mCurPlayerDetailPage = newPage;
        }
    }
    #endregion

    /// <summary>
    /// 更新盈利数据
    /// </summary>
    /// <param name="newType"></param>
    /// <param name="newPage"></param>
    /// <param name="newList"></param>
    public void UpdateUserList(string newType, int newPage, List<SendGetUserBRInfo> newList)
    {

        if (!string.IsNullOrEmpty(mCurUserType) && newType != mCurUserType)
        {//切换类型
            mCurUserList.Clear();
        }
        if (newList != null)
        {
            if (newType == mCurUserType)//更新当前类型的数据
            {
                if (newPage == 1)
                {//插入到最开始
                    for (int i = newList.Count - 1; i >= 0; i--)
                    {
                        bool contain = false;
                        for (int j = 0; j < mCurUserList.Count; j++)
                        {
                            if (newList[i].messageId == mCurUserList[j].messageId)
                            {//如果老的数据里面有这个数据,就不用把这个数据加入model
                                contain = true;
                                continue;
                            }
                        }
                        if (!contain)
                        {//不包含
                            mCurUserList.Insert(0, newList[i]);
                        }
                    }
                }
                else//插入到尾部
                {
                    for (int i = 0; i < newList.Count; i++)
                    {
                        bool contain = false;
                        for (int j = 0; j < mCurUserList.Count; j++)
                        {
                            if (newList[i].messageId == mCurUserList[j].messageId)
                            {//如果老的数据里面有这个数据,就不用把这个数据加入model
                                contain = true;
                                continue;
                            }
                        }
                        if (!contain)
                        {//不包含
                            mCurUserList.Add(newList[i]);
                        }
                    }
                }
            }
            else
            {
                mCurUserPage = newPage;
                mCurUserType = newType;
                mCurUserList = newList;
            }
        }
    }


    /// <summary>
    /// 更新盈利数据
    /// </summary>
    /// <param name="newType"></param>
    /// <param name="newPage"></param>
    /// <param name="newList"></param>
    public void UpdateMessageList(int newPage, List<SendGetMessageInfo> newList)
    {
        if (newList != null)
        {
            if (newPage == 1)
            {//插入到最开始
                for (int i = newList.Count - 1; i >= 0; i--)
                {
                    bool contain = false;
                    for (int j = 0; j < mCurMessageList.Count; j++)
                    {
                        if (newList[i].messageId == mCurMessageList[j].messageId)
                        {//如果老的数据里面有这个数据,就不用把这个数据加入model
                            contain = true;
                            continue;
                        }
                    }
                    if (!contain)
                    {//不包含
                        mCurMessageList.Insert(0, newList[i]);
                    }
                }
            }
            else//插入到尾部
            {
                for (int i = 0; i < newList.Count; i++)
                {
                    bool contain = false;
                    for (int j = 0; j < mCurMessageList.Count; j++)
                    {
                        if (newList[i].messageId == mCurMessageList[j].messageId)
                        {//如果老的数据里面有这个数据,就不用把这个数据加入model
                            contain = true;
                            continue;
                        }
                    }
                    if (!contain)
                    {//不包含
                        mCurMessageList.Add(newList[i]);
                    }
                }
            }
        }
    }

}
