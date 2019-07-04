using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJGameInfoView : BaseViewWidget
{
    public UILabel mRules;//当前玩法
    public UILabel mBaseScore;//当前底分
    public UILabel mPlayNum;//局数
    public UILabel mFanshu;//封顶番数
    public UITable mTable;//table

    /// <summary>
    /// 
    /// </summary>
    /// <param name="score">底分</param>
    /// <param name="num">当前局数</param>
    /// <param name="allnum">总局数</param>
    /// <param name="fanshu">封顶番数</param>
    public void SetData(int score, int num,int allnum, int fanshu)
    {
        mBaseScore.text = score.ToString();//底分
        mPlayNum.text = num + "/" + allnum;//局数
        mFanshu.text = fanshu == -1 ? "不封顶" : fanshu + "番封顶";
        mRules.text = GetCurRules();//规则
        mTable.Reposition();
    }


    private string GetCurRules()
    {
        string s = "";
        List<int> rule = MJGameModel.Inst.mCurRules;
        if (rule == null || rule.Count == 0)
            return s;
        MJGameRuleConfig.GameRuleDetail[] conarr = new MJGameRuleConfig.GameRuleDetail[1];//配比表获取 //ConfigManager.Create().mGameRuleConfig.mHaErBinList[0].gameRule;
        List<MJGameRuleConfig.GameRuleDetail> con = new List<MJGameRuleConfig.GameRuleDetail>(conarr);
        MJGameRuleConfig.GameRuleDetail r;
        //排除掉GPS和IP报警
        string exspect1 = "checkIP";
        string exspect2 = "checkGPS";
        bool isfirst = true;
        for (int i = 0; i < rule.Count; i++)
        {
            r = con.Find(o => o.ruleId == rule[i]);
            if (r != null && !r.enName.Equals(exspect1) && !r.enName.Equals(exspect2))
            {
                if (isfirst)
                    s += r.name;
                else
                    s += "、" + r.name;
                isfirst = false;
            }
        }
        return s;
    }

    public void OnCloseClick()
    {
        gameObject.SetActive(false);
    }

}
