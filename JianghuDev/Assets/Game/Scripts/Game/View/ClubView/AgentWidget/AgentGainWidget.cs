using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentGainWidget : BaseView
{

    public UILabel mAgentNum;//代理总人数
    public UILabel mRichNum;//总桃数
    public UILabel mTPNum;//可调配数量
    public GameObject mItem;//代理item
    public FUIScrollView mScroll;//scroll
    public UILabel mHigherAgentName;//上级代理名字
    public UILabel mHigherAgentId;//上级代理id
    public UIGrid mHigherAgentGrid;//上级代理grid
    public GameObject mHigherAgentRoot;//上级代理root
    public GameObject mGeneralAgrentBtn;//总代按钮
    public GameObject mGeneralAgrentBg;//总代背景

    private List<SendGetAgentWinList> mData;//数据
    private GameObject mRootView;//前一个窗口
    private SendGetAgentWinList mHigherAgent;//上级代理

    protected override void Awake()
    {
        base.Awake();
        mScroll.AddEvent(eFuiScrollViewEvent.InitItem, OnInitItem);
        mScroll.AddEvent(eFuiScrollViewEvent.UpdateItem, OnUpdateItem);
    }
    

    public void InitUI(List<SendGetAgentWinList> agentList, GameObject rootview, int agentnum, float allpeach, float tpPeach, SendGetAgentWinList higherAgent)
    {
        mRootView = rootview;
        mData = agentList;
        //代理总人数
        mAgentNum.text = agentnum.ToString();
        //总金币数
        mRichNum.text = allpeach.ToString();
        //可调配数量
        mTPNum.text = tpPeach.ToString();
        //上级代理
        mHigherAgent = higherAgent;
        SetHigherAgrentData(mHigherAgent);
        //根据是否是总代显示总代按钮
        mGeneralAgrentBg.SetActive(PlayerModel.Inst.UserInfo.isGeneralAgent);
        mGeneralAgrentBtn.SetActive(PlayerModel.Inst.UserInfo.isGeneralAgent);
        int count = agentList == null ? 0 : agentList.Count;
        mScroll.SetData(count);
    }

    //设置上级代理数据
    private void SetHigherAgrentData(SendGetAgentWinList higherAgent)
    {
        if (higherAgent == null)//没有上级代理
            mHigherAgentRoot.SetActive(false);
        else//有上级代理
        {
            mHigherAgentRoot.SetActive(true);
            if (higherAgent.userId == "a888888")//管理员
                mHigherAgentId.gameObject.SetActive(false);
            else
            {
                mHigherAgentId.gameObject.SetActive(true);
                mHigherAgentId.text = "ID:" + higherAgent.userId;//id
            }
            mHigherAgentName.text = higherAgent.nickname;//名字
            mHigherAgentGrid.Reposition();
        }
    }


    #region scroll
    private void OnInitItem(params object[] args)
    {
        int index = (int)args[0];
        GameObject obj = NGUITools.AddChild(mScroll.gameObject, mItem);
        obj.SetActive(true);
        obj.GetComponent<AgentGainItem>().InitUI(mData[index], transform, this);
        mScroll.InitItem(index, obj);
    }


    private void OnUpdateItem(params object[] args)
    {
        int index = (int)args[0];
        GameObject obj = args[1] as GameObject;
        obj.SetActive(true);
        obj.GetComponent<AgentGainItem>().InitUI(mData[index], transform, this);
    }
    #endregion


    #region 按钮事件
    /// <summary>
    /// 调配按钮点击
    /// </summary>
    public void OnTiaopeiClick()
    {
        ClubBorrowWidget view = GetWidget<ClubBorrowWidget>("Windows/ClubView/ClubBorrowWidget", transform);
        view.SetCallBack((t) =>
        {
            //可调配数量
            mTPNum.text = PlayerModel.Inst.UserInfo.wareHouse.ToString();
        });
    }

    /// <summary>
    /// 记录按钮点击
    /// </summary>
    public void OnRecordClick()
    {
        AgentBRWidget widget = GetWidget<AgentBRWidget>(AssetsPathDic.AgentBRWidget, transform);
        widget.InitUI("JR", "JC");
    }

    /// <summary>
    /// 上级代理点击
    /// </summary>
    public void OnHigherAgrentClick()
    {
        if (mHigherAgent == null || mHigherAgent.userId == "a888888")//为空或者是管理员时不显示
            return;
        ClubBorrowWidget view = GetWidget<ClubBorrowWidget>("Windows/ClubView/ClubBorrowWidget", transform);
        view.SetData(mHigherAgent.userId);
        view.SetCallBack((t) =>
        {
            RefreshWare();
        });
    }

    //总代按钮点击
    public void OnGeneralAgentClick()
    {
        Global.Inst.GetController<AgentGainController>().GetGeneralData((data) =>
        {
            GeneralAgentWidget view = GetWidget<GeneralAgentWidget>("Windows/ClubView/GeneralAgentWidget", transform);
            view.SetData(data);
        });
    }


    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void CloseClick()
    {
        Close();
        if (mRootView != null)
            mRootView.SetActive(true);
    }
    #endregion

    public void RefreshWare()
    {
        //可调配数量
        mTPNum.text = PlayerModel.Inst.UserInfo.wareHouse.ToString();
    }
}
