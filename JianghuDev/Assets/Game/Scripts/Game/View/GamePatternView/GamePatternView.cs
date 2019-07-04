using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePatternView : BaseView
{

    public UIToggle mClubToggle;


    public UIToggle mGlodToggle;

    /// <summary>
    /// 俱乐部id'
    /// </summary>
    public UILabel mClubId;
    /// <summary>
    /// 当前人数
    /// </summary>
    public UILabel mCurPeopleNum;
    /// <summary>
    /// 在线人数
    /// </summary>
    public UILabel mOnLinePeopleNum;
    /// <summary>
    /// 抽成比例
    /// </summary>
    public UILabel mChouChengRateNum;
    /// <summary>
    /// 代理的设置按钮
    /// </summary>
    public GameObject mSettingBtn;
    /// <summary>
    /// 战绩按钮
    /// </summary>
    public GameObject mScoreBtn;
    /// <summary>
    /// 俱乐部/联盟房间
    /// </summary>
    public ClubPatternWidget mClubWidget;
    /// <summary>
    /// 平台对战
    /// </summary>
    public GlodPatternWidget mGoldWidget;
    public UILabel mClubName;//联盟名字

    /// <summary>
    /// 选择区间显示
    /// </summary>
    public UILabel mSelectLabel;

    /// <summary>
    /// 换三张的那个toggle
    /// </summary>
    public GameObject mChangThreeGo;
    /// <summary>
    /// 底分下拉菜单动画
    /// </summary>
    public TweenScale mSelectTween;

    public UILabel mGold;//金币
    public UILabel mRoomCards;//房卡
    public GameObject mRoomCardBG;//房卡
    public UILabel mPoint;//积分
  //  public GameObject mRoomCardBG;//房卡

    #region 私有属性

    /// <summary>
    /// 所有桌子
    /// </summary>
    private Dictionary<string, ClubPatternItem> mAllDeskDic = new Dictionary<string, ClubPatternItem>();
    /// <summary>
    /// 所有平台对战
    /// </summary>
    private Dictionary<int, GoldPatternItem> mAllLvDic = new Dictionary<int, GoldPatternItem>();

    /// <summary>
    /// 切换到房间列表的次数
    /// </summary>
    private int mNum = 0;

    /// <summary>
    /// 显示选择item吗
    /// </summary>
    private bool mShowSelect = false;

    #endregion


    #region Unity 函数

    protected override void Awake()
    {
        base.Awake();
        GamePatternModel.Inst.mCondition.Add(2);
    }


    protected override void Start()
    {
        base.Start();


        if (GamePatternModel.Inst.mCurGameId == eGameType.MaJiang)
        {
            mScoreBtn.gameObject.SetActive(true);
            mChangThreeGo.gameObject.SetActive(true);
        }
        else
        {
            mScoreBtn.gameObject.SetActive(false);
            mChangThreeGo.gameObject.SetActive(false);
        }
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        GamePatternModel.Inst.mCondition.Clear();
        GamePatternModel.Inst.mBaseScoreDown = -1;
        GamePatternModel.Inst.mBaseScoreTop = -1;
    }

    #endregion



    #region 更新界面
    /// <summary>
    /// 
    /// </summary>
    /// <param name="clubId">俱乐部id</param>
    /// <param name="totalPlayerNum">总人数</param>
    /// <param name="onlinePlayerNum">在线人数</param>
    public void SetData(string clubId, int totalPlayerNum, int onlinePlayerNum, float rate, string clubName)
    {
        //设置俱乐部id
        SetClubID(clubId);
        //俱乐部名字
        mClubName.text = clubName;
        //设置总人数
        SetTotalPeopleNum(totalPlayerNum);
        //设置在线人数
        SetOnLinePeopleNum(onlinePlayerNum);
        //刷新房间列表
        InitClubPatternWidget();
        //金币和房卡
        SetCardAndPoint();
        ///抽成比例
        mChouChengRateNum.text = "抽成比例：" + (int)(rate * 100) + "%";
    }

    /// <summary>
    /// 初始化界面
    /// </summary>
    public void InitClubPatternWidget()
    {
        mClubWidget.Init();
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="index"></param>
    public void UpdateClubPatternWidget(int index, SendGetRoomListInfo data)
    {
        mClubWidget.UpdateClubPatternWidget(index, data);
    }

    /// <summary>
    /// 设置总人数
    /// </summary>
    /// <param name="num"></param>
    private void SetTotalPeopleNum(int num)
    {
        mCurPeopleNum.text = num.ToString();
    }

    /// <summary>
    /// 设置在线人数
    /// </summary>
    /// <param name="num"></param>
    public void SetOnLinePeopleNum(int num)
    {
        mOnLinePeopleNum.text = num.ToString();
    }

    /// <summary>
    /// 设置俱乐部id
    /// </summary>
    /// <param name="num"></param>
    private void SetClubID(string num)
    {
        mClubId.text = num.ToString();
    }

    /// <summary>
    /// 设置金币和房卡
    /// </summary>
    private void SetCardAndPoint()
    {
        mGold.text = PlayerModel.Inst.UserInfo.gold.ToString();
        //mRoomCardBG.SetActive(PlayerModel.Inst.UserInfo.agent == 1);
        //mRoomCards.text = PlayerModel.Inst.UserInfo.roomCard.ToString();
        mPoint.text = PlayerModel.Inst.UserInfo.point.ToString();
    }
    #endregion


    #region UI事件

    /// <summary>
    /// 关闭按钮点击事件
    /// </summary>
    public void OnCloseBtnClick()
    {
        Global.Inst.GetController<GamePatternController>().SendCheckInGame();
    }

    /// <summary>
    /// 代理设置抽成按钮点击
    /// </summary>
    public void OnDaiLiSetingClick()
    {

        if (PlayerModel.Inst.UserInfo.agent == 1)
        {//是代理
            AgentSettingWidget widget = GetWidget<AgentSettingWidget>(AssetsPathDic.AgentSettingWidget, transform);
            widget.SetValueRate((int)GamePatternModel.Inst.mChouCheng * 100);
        }
        else
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("请先成为代理用户");
        }
    }


    /// <summary>
    /// 平台选择toggle
    /// </summary>
    /// <param name="go"></param>
    public void OnPatternToggleClick(GameObject go)
    {
        if (go.name == "1" && mClubToggle.value)//选择的是俱乐部
        {
            InitClubPatternWidget();

        }
        else if (go.name == "2" && mGlodToggle.value)//选择的是平台对战
        {
            if (mAllLvDic.Count <= 0)
            {
                mGoldWidget.InitByConfig();
            }

            Global.Inst.GetController<GamePatternController>().SendGetGoldPeopleNum((data) =>
            {
                if (data != null)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        SetGoldPatternPeoPleNum(i, data[i]);
                    }
                }
            });
        }
    }


    /// <summary>
    /// 战绩查询点击
    /// </summary>
    public void OnScoreClick()
    {
        Global.Inst.GetController<MJGameBackController>().GetAllDetailData(() =>
        {
            GetWidget<MJRecordWidget>("Windows/Majiang/MJRecordWidget", transform);
        });
    }

    /// <summary>
    /// 有空位联盟桌换三张
    /// </summary>
    public void OnConditionToggleClick(GameObject sp, GameObject go)
    {
        int index = int.Parse(go.name);
        if (GamePatternModel.Inst.mCondition.Contains(index) && !sp.activeSelf)
        {//包含了你还点
            return;
        }
        if (!sp.activeSelf)
        {//添加一个条件
            GamePatternModel.Inst.mCondition.Add(index);
        }
        else
        {
            GamePatternModel.Inst.mCondition.Remove(index);
        }
        InitClubPatternWidget();
        sp.gameObject.SetActive(!sp.activeSelf);
    }
    /// <summary>
    /// 加入房间点击
    /// </summary>
    public void OnJoinRoomClick()
    {
        GetWidget<JoinRoomWidget>(AssetsPathDic.JoinRoomWidget, transform);
    }

    /// <summary>
    /// 创建房间点击
    /// </summary>
    public void OnCreateRoomClick()
    {
        CreateRoomView rv = Global.Inst.GetController<CreateRoomController>().OpenWindow() as CreateRoomView;
    }

    /// <summary>
    /// 底分区间的item点击事件
    /// </summary>
    public void OnBaseSelectItemClick(GameObject go)
    {
        int index = int.Parse(go.name);
        switch (index)
        {
            case 1:
                GamePatternModel.Inst.mBaseScoreDown = 0.1f;
                GamePatternModel.Inst.mBaseScoreTop = 0.4f;
                mSelectLabel.text = "0.1 ~ 0.4";
                break;
            case 2:
                GamePatternModel.Inst.mBaseScoreDown = 0.5f;
                GamePatternModel.Inst.mBaseScoreTop = 0.9f;
                mSelectLabel.text = "0.5 ~ 0.9";

                break;
            case 3:
                GamePatternModel.Inst.mBaseScoreDown = 1.0f;
                GamePatternModel.Inst.mBaseScoreTop = 1.9f;
                mSelectLabel.text = "1.0 ~ 1.9";

                break;
            case 4:
                GamePatternModel.Inst.mBaseScoreDown = 2.0f;
                GamePatternModel.Inst.mBaseScoreTop = 4.9f;
                mSelectLabel.text = "2.0 ~ 4.9";

                break;
            case 5:
                GamePatternModel.Inst.mBaseScoreDown = 5.0f;
                GamePatternModel.Inst.mBaseScoreTop = -1.0f;
                mSelectLabel.text = "5.0及以上";

                break;
            case 6:
                GamePatternModel.Inst.mBaseScoreDown = -1.0f;
                GamePatternModel.Inst.mBaseScoreTop = -1.0f;
                mSelectLabel.text = "全  部";

                break;
        }
        InitClubPatternWidget();
        OnBaseSelectBtnClick();
    }

    /// <summary>
    /// 底分区间的选择点击
    /// </summary>
    public void OnBaseSelectBtnClick()
    {
        if (mShowSelect)
        {//显示了，被点击，应该要隐藏
            mSelectTween.PlayReverse();
            mShowSelect = false;
        }
        else
        {
            mSelectTween.PlayForward();
            mShowSelect = true;
        }
    }

    #endregion

    #region 外部调用

    public void BackToGlodWidget()
    {
        mGlodToggle.value = true;
        mClubToggle.value = false;
    }


    public void BackToFriendWidget()
    {
        mGlodToggle.value = false;
        mClubToggle.value = true;
    }

    /// <summary>
    /// 添加子物体
    /// </summary>
    /// <param name="roomId"></param>
    /// <param name="item"></param>
    public void AddClubPatternItem(string roomId, ClubPatternItem item)
    {
        if (mAllDeskDic.ContainsKey(roomId))
        {
            mAllDeskDic[roomId] = item;
        }
        else
        {
            mAllDeskDic.Add(roomId, item);
        }
    }


    public void AddGoldPatternItem(int lvId, GoldPatternItem item)
    {
        if (mAllLvDic.ContainsKey(lvId))
        {
            mAllLvDic[lvId] = item;
        }
        else
        {
            mAllLvDic.Add(lvId, item);
        }
    }


    public void SetGoldPatternPeoPleNum(int index, int num)
    {
        if (mAllLvDic.ContainsKey(index + 1))
        {
            mAllLvDic[index + 1].SetPeoPleNum(num);
        }
    }

    #endregion
}
