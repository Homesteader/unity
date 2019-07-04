using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJGameSettlementFinalView : BaseViewWidget
{
    public UILabel mBaseScore;//底分
    public UILabel mPlayCount;//局数
    public UITable mCardTable;//牌table
    public GameObject mCardNormalItem;//3张牌
    public GameObject mThreeLie;//3张平躺的牌
    public GameObject mCardOneItem;//1张牌
    public GameObject mCardGang;//四张牌
    public GameObject mOther;//空白
    public UIGrid mHandCard;//手牌
    public UILabel mName;//最佳牌型玩家名字
    public UILabel mRules;//本局玩法
    public MJGameSettlementFinalItemView[] mPlayers;//玩家
    public UIGrid mGrid;
    public UIButton mContinueBtn;//继续按钮
    public UIButton mShareBtn;//分享按钮
    public UILabel mHuType;//最佳牌型胡牌细节

    // Use this for initialization
    void Start()
    {
    }


    public void SetData(MJGameSettlementFinalInfo info)
    {
        //底分
        mBaseScore.text = MJGameModel.Inst.mBaseScore.ToString();
        //局数
        mPlayCount.text = MJGameModel.Inst.mPlayCount.ToString();
        //本期玩法
        //mRules.text = GetCurRules();
        //玩家信息
        if (info.totalContainr != null && info.totalContainr != null)
        {
            Dictionary<int, int> zuijiapaoshou = new Dictionary<int, int>();
            int score = -1;
            int diapao = 0;
            for (int i = 0; i < info.totalContainr.Count; i++)
            {
                diapao = info.totalContainr[i].dianPao;
                if (diapao > score && diapao > 0)
                {
                    score = diapao;
                    zuijiapaoshou.Clear();
                    zuijiapaoshou.Add(i, i);
                }
                else if (diapao == score)
                {
                    score = diapao;
                    if (!zuijiapaoshou.ContainsKey(i))
                        zuijiapaoshou.Add(i, i);
                }
            }
            int len = mPlayers.Length;
            for (int i = 0; i < len; i++)
            {
                if (i < info.totalContainr.Count)
                {
                    mPlayers[i].gameObject.SetActive(true);
                    mPlayers[i].SetData(info.totalContainr[i], zuijiapaoshou.ContainsKey(i));
                }
                else
                {
                    mPlayers[i].gameObject.SetActive(false);
                }
            }
            mGrid.Reposition();
        }
        //最佳牌型
        InitBestCards(info.bestHu);
        //最佳牌型胡牌细节
        //    mHuType.text = MJGameModel.Inst.GetHuType(info.bestHu);

        //ZhidianSDKManaerger.Inst.CaptureScreenshot(() =>
        //{
        //    mContinueBtn.gameObject.SetActive(true);
        //    mShareBtn.gameObject.SetActive(true);
        //});
    }

    #region 按钮点击
    /// <summary>
    /// 分享
    /// </summary>
    public void OnShareClick()
    {

    }


    /// <summary>
    /// 继续
    /// </summary>
    public void OnContinueClick()
    {
        MJGameModel.Inst.mFinalSettlementInfo = null;
        //Global.Inst.GetController<MJGameController>().ConnectedToHallServer(null);
        Global.Inst.GetController<MJGameController>().LoadStartScene(() =>
        {
            Close<MJGameSettlementFinalView>();
        });
        /*
        UnityEngine.SceneManagement.SceneManager.LoadScene("Start");
        Global.Inst.GetController<MJGameController>().CloseWindow();

        MJGamePlayerData data = MJGameModel.Inst.mPlayerdata[MJGameModel.Inst.mMySeatId - 1];
        LoginSR.RequestLogin req = new LoginSR.RequestLogin();
      
        req.Id = data.mPlayerInfo.userId;
        req.nickName = data.mPlayerInfo.nickName;
        req.sex = 1;
        req.headUrl = data.mPlayerInfo.headUrl;
        Global.Inst.GetController<LoginController>().SendEnterGameMSG(req);*/
    }
    #endregion

    #region 最佳牌型
    private void InitBestCards(MJGameSettlementPlayerInfo info)
    {
        if (info == null)
            return;
        mName.text = info.nickName;
        SetCard(info);
    }

    private void SetCard(MJGameSettlementPlayerInfo info)
    {
        GameUtils.ClearChildren(mCardTable.transform);
        //空白
        GameObject other = NGUITools.AddChild(mCardTable.gameObject, mOther);
        other.SetActive(true);
        //杠
        CreateCardList(info.gang, eMJInstructionsType.GANG);
        //吃
        //CreateCardList(info.chi, eMJInstructionsType.chi);
        //碰
        if (info.peng != null)
        {
            for (int i = 0; i < info.peng.Count; i++)
                InitPengItems(mThreeLie, info.peng[i].cards[0], info.peng[i].cards[0], info.peng[i].cards[0]);
        }
        //亮
        //CreateCardList(info.liang, eMJInstructionsType.liang);
        //手牌
        CreateHandCard(info.shoupai);
        //胡牌
        if (info.huPai.Count > 0)
        {
            for (int i = 0; i < info.huPai.Count; i++)
            {
                GameObject obj = NGUITools.AddChild(mCardTable.gameObject, mCardOneItem);
                obj.SetActive(true);
                obj.transform.GetChild(0).GetComponent<UISprite>().spriteName = "top_" + info.huPai[i].card;
            }
        }
        mCardTable.Reposition();
    }

    private void CreateCardList(List<GangStruct> list, eMJInstructionsType _type)
    {
        if (list == null)
            return;
        for (int i = 0; i < list.Count; i++)
            CreateItems(list[i], _type);
    }

    private void CreateItems(GangStruct card, eMJInstructionsType _type)
    {
        if (card == null || card.cards == null)
            return;
        if (_type == eMJInstructionsType.GANG)//杠
        {
            if (card.gangType == eGangType.ANGANG)//暗杠
                InitAngang(mCardGang, card.cards[0]);
            else
                InitPengItems(mCardGang, card.cards[0], card.cards[0], card.cards[0], card.cards[0]);
        }
        else
        {
            int len = card.cards.Count;
            if (len == 3)
                InitPengItems(mThreeLie, card.cards[0], card.cards[1], card.cards[2]);
            else if (len == 4)
                InitPengItems(mCardGang, card.cards[0], card.cards[1], card.cards[2], card.cards[3]);
        }
    }

    private void InitPengItems(GameObject item, int a, int b, int c, int d = 0)
    {
        Transform tran = NGUITools.AddChild(mCardTable.gameObject, item).transform;
        tran.gameObject.SetActive(true);
        tran.GetChild(0).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + a;
        tran.GetChild(1).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + b;
        tran.GetChild(2).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + c;
        if (d != 0)
            tran.GetChild(3).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + d;
    }

    private void InitAngang(GameObject item, int a)
    {
        Transform tran = NGUITools.AddChild(mCardTable.gameObject, item).transform;
        tran.gameObject.SetActive(true);
        tran.GetChild(0).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + a;
        tran.GetChild(1).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + a;
        tran.GetChild(2).GetChild(0).GetComponent<UISprite>().spriteName = "top_" + a;
        tran.GetChild(3).gameObject.SetActive(false);
        tran.GetChild(4).gameObject.SetActive(true);
    }

    private void CreateHandCard(List<int> card)
    {
        if (card == null)
            return;
        GameObject obj = NGUITools.AddChild(mCardTable.gameObject, mHandCard.gameObject);
        obj.SetActive(true);
        GameObject item;
        for (int i = 0; i < card.Count; i++)
        {
            item = NGUITools.AddChild(obj, mCardOneItem);
            item.SetActive(true);
            item.transform.GetChild(0).GetComponent<UISprite>().spriteName = "top_" + card[i];
        }
        obj.GetComponent<UIGrid>().Reposition();
    }
    #endregion

    #region 本期玩法
    private string GetCurRules()
    {
        string s = "";
        List<int> rule = MJGameModel.Inst.mCurRules;
        if (rule == null || rule.Count == 0)
            return s;
        s = "本局玩法：";
        MJGameRuleConfig.GameRuleDetail[] conarr = new MJGameRuleConfig.GameRuleDetail[2];//从配置表获取//ConfigManager.Create().mGameRuleConfig.mHaErBinList[0].gameRule;
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
                    s += "[" + r.name + "]";
                else
                    s += "、[" + r.name + "]";
                isfirst = false;
            }
        }
        return s;
    }
    #endregion
}
