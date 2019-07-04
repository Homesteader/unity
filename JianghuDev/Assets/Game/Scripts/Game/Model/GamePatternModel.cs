using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePatternModel : BaseModel
{

    public static GamePatternModel Inst;

    public override void SetController(BaseController c)
    {
        base.SetController(c);
        Inst = this;
    }

    /// <summary>
    /// 俱乐部号
    /// </summary>
    public string mClubId;

    /// <summary>
    /// 抽成
    /// </summary>
    public float mChouCheng;

    /// <summary>
    /// 勾选条件
    /// </summary>
    public List<int> mCondition = new List<int>();

    /// <summary>
    /// 当前选择的是哪个游戏
    /// </summary>
    public eGameType mCurGameId = eGameType.MaJiang;

    /// <summary>
    /// 房间信息列表
    /// </summary>
    public List<SendGetRoomListInfo> mRoomList = new List<SendGetRoomListInfo>();

    /// <summary>
    /// 底分区间的上限
    /// </summary>
    public float mBaseScoreTop = -1;
    /// <summary>
    /// 底分区间的下限
    /// </summary>
    public float mBaseScoreDown = -1;

    /// <summary>
    /// 得到有空位的房间 1是有空位 2是联盟桌 3是联盟且有空位
    /// </summary>
    /// <param name="condition"></param>
    public List<SendGetRoomListInfo> GetConditionRoomList()
    {

        List<SendGetRoomListInfo> temp = new List<SendGetRoomListInfo>();

        if (GamePatternModel.Inst.mCondition.Count == 0)//全部没有勾选 默认俱乐部
        {
            for (int i = 0; i < mRoomList.Count; i++)
            {
                if (mRoomList[i].nil && mRoomList[i].model == "m1")
                {
                    temp.Add(mRoomList[i]);
                }
            }
            //temp = mRoomList;
        }
        else if (GamePatternModel.Inst.mCondition.Contains(1) &&
            GamePatternModel.Inst.mCondition.Contains(2) &&
            GamePatternModel.Inst.mCondition.Contains(3))
        {//全选
            if (mCurGameId == eGameType.MaJiang)
            {
                for (int i = 0; i < mRoomList.Count; i++)
                {
                    if (mRoomList[i].rule.ruleIndexs.Contains(6))
                    {
                        temp.Add(mRoomList[i]);
                    }
                }
            }
        }
        else if (GamePatternModel.Inst.mCondition.Contains(1) &&
            GamePatternModel.Inst.mCondition.Contains(2))//有空位而且是联盟桌
        {
            for (int i = 0; i < mRoomList.Count; i++)
            {
                if (mRoomList[i].nil)
                {
                    temp.Add(mRoomList[i]);
                }
            }
        }
        else if (GamePatternModel.Inst.mCondition.Contains(1) &&
            GamePatternModel.Inst.mCondition.Contains(3))//有空位而且是换三张
        {
            if (mCurGameId == eGameType.MaJiang)
            {
                for (int i = 0; i < mRoomList.Count; i++)
                {
                    if (mRoomList[i].nil && mRoomList[i].rule.ruleIndexs.Contains(6))
                    {
                        temp.Add(mRoomList[i]);
                    }
                }
            }
        }
        else if (GamePatternModel.Inst.mCondition.Contains(2) &&
            GamePatternModel.Inst.mCondition.Contains(3))//换三张而且是联盟桌
        {
            if (mCurGameId == eGameType.MaJiang)
            {
                for (int i = 0; i < mRoomList.Count; i++)
                {
                    if (mRoomList[i].rule.ruleIndexs.Contains(6))
                    {
                        temp.Add(mRoomList[i]);
                    }
                }
            }
        }
        else if (GamePatternModel.Inst.mCondition.Contains(1))
        {
            for (int i = 0; i < mRoomList.Count; i++)
            {
                if (mRoomList[i].nil)
                {
                    temp.Add(mRoomList[i]);
                }
            }
        }
        else if (GamePatternModel.Inst.mCondition.Contains(2))
        {
            for (int i = 0; i < mRoomList.Count; i++)
            {
                //if (mRoomList[i].model == "m2")
                {
                    temp.Add(mRoomList[i]);
                }
            }
        }
        else if (GamePatternModel.Inst.mCondition.Contains(3))
        {
            if (mCurGameId == eGameType.MaJiang)
            {
                for (int i = 0; i < mRoomList.Count; i++)
                {
                    if (mRoomList[i].rule.ruleIndexs.Contains(6))
                    {
                        temp.Add(mRoomList[i]);
                    }
                }
            }
        }

        return temp;
    }

    /// <summary>
    /// 解析房间规则
    /// </summary>
    /// <param name="roomInfo"></param>
    /// <param name="show"></param>
    /// <returns></returns>
    public string DeserializeRuleJosn(SendCreateRoomReq roomInfo, bool show, bool gold = false)
    {
        string result = "";

        List<ConfigDada> list = null;

        switch ((eGameType)roomInfo.gameId)
        {
            case eGameType.MaJiang:
                {
                    list = ConfigManager.GetConfigs<MJGameCreateConfig>();
                }
                break;
            case eGameType.NiuNiu:
                {
                    list = ConfigManager.GetConfigs<NiuNiuGameCreateConfig>();
                }
                break;
            case eGameType.GoldFlower:
                {
                    list = ConfigManager.GetConfigs<JinHuaGameCreateConfig>();
                }
                break;
            case eGameType.TenHalf:
                list = ConfigManager.GetConfigs<TenGameCreateConfig>();
                break;
        }

        CreateRoomRule config = null;
        for (int i = 0; i < list.Count; i++)
        {
            CreateRoomRule temp = list[i] as CreateRoomRule;
            if (temp.gameId == roomInfo.subType)
            {
                SQDebug.Log(temp.gameId + "::+++++");
                config = temp;
                break;
            }
        }

        if (!show)
        {
            result += config.name + ":";
        }

        for (int i = 0; i < config.rules.Length; i++)
        {
            if (show)
            {
                result += config.rules[i].name + "：";
            }
            bool contain = false;

            Debug.Log("iii=" + i + "  " + config.rules.Length);

            for (int j = 0; j < config.rules[i].content.Length; j++)
            {
                Debug.Log("jjj=" + j);
                if (CheckRule(roomInfo.ruleIndexs, config.rules[i].content[j].Index, config.rules[i].content[j].connect))
                {//包含这个规则
                    result += config.rules[i].content[j].name + "，";
                    contain = true;
                }
                if (config.rules[i].content[j].Index == 1)
                {//底分
                    result += "底分为" + roomInfo.baseScore + "，";
                    contain = true;
                }
                if (config.rules[i].content[j].Index == 2)
                {
                    result += "带入为" + roomInfo.into + "，";
                    contain = true;
                }
            }
            if (!contain && !config.rules[i].name.Contains("模式"))
            {
                result += "无，";
            }
            if (show)
            {
                if (gold && config.rules[i].name.Contains("模式"))
                {
                    result += "平台场，";
                }
                result = result.Remove(result.Length - 1, 1);//移除最后一位逗号
                result += "\n";
            }
        }
        if (!show)
        {
            result = result.Replace("无，", "");
            result = result.Remove(result.Length - 1, 1);//移除最后一位逗号
        }
        return result;
    }

    public bool GetConnect(List<int> rules, int index)
    {

        if (index == 0) return true;
        return rules.Contains(index);
    }

    public bool CheckRule(List<int> rules, int index, int connect)
    {
        if (rules.Contains(index) && GetConnect(rules, connect))
        {
            if (index != 26 && index != 27 && index != 3 && index != 4) return true;

            if ((index == 26 || index == 27) && !rules.Contains(3) && !rules.Contains(4)) return true;

            if ((index == 3 || index == 4) && !rules.Contains(27)) return true;
        }

        return false;
    }
}
