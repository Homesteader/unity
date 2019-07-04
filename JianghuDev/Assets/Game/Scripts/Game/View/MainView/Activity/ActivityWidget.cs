using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActivityWidget : BaseViewWidget
{
    public Transform tran;
    public ActivityBigWheelItem item;
    public Transform itemObj;
    public UIButton mBeginBtn;   //开始按钮
    public GameObject mDish;  //转盘对象
    public UILabel ruleLabel;

    //中奖
    public GameObject mWin;
    public UISprite mWinGoods;
    public UILabel mNum;
    public UILabel mDes;
    public GameObject mLost;
    public UISprite mLostGoods;

    private GameObject lastluckyItem;//上一次点击的的底样
    private float mItemAgle = 45f;

    private GetPrizeConfigBackData prizeData;

    private PrizeInfo reward;

    public UILabel mCount;
    public UILabel mPlayCount;
    /// <summary>
    /// 每个区域角度区间
    /// </summary>
    private Dictionary<int, float[]> dicPrize = new Dictionary<int, float[]>();

    protected override void Start()
    {
        base.Start();
    }


    /// <summary>
    /// 初始化转盘显示
    /// </summary>
    public void InitDish(GetPrizeConfigBackData data)
    {
        ruleLabel.text = data.rule;

        prizeData = data;
        mCount.text = string.Format("{0}次", data.drawNum - data.drawTotal);
        mPlayCount.text = string.Format("[7F6E36]游戏局数:[-][FFA70B]{0}[-]  [7F6E36]已抽奖次数:[-][FFA70B]{1}[-]", data.games, data.drawTotal);
        List<ConfigDada> conList = ConfigManager.GetConfigs<ActivityBigWheelConfig>();//获取 配置显示对应的奖品
        if (conList == null) return;
        int cont = data.info.Count;
        if (cont % 2 != 0) cont++;
        float num = 360 / cont;
        mItemAgle = num;
        NGUITools.DestroyChildren(itemObj);
        for (int i = 0; i < cont; i++)
        {
            ActivityBigWheelItem ob;
            ActivityBigWheelConfig config = null;

            for (int m = 0; m < conList.Count; m++)
            {
                config = conList[m] as ActivityBigWheelConfig;

                if (config.name == data.info[i].type)
                {
                    if (i < 5)
                    {
                        ob = GameObject.Instantiate(item, itemObj) as ActivityBigWheelItem;
                        ob.SetItem(-(num / 2 + i * num), data.info[i], config);
                        ob.gameObject.SetActive(true);

                    }
                    else
                    {
                        ob = GameObject.Instantiate(item, itemObj) as ActivityBigWheelItem;
                        ob.SetItem(-(num / 2 + i * num), data.info[i], config);
                        ob.gameObject.SetActive(true);
                    }
                    break;
                }
            }
        }

    }


    /// <summary>
    /// 动画完成事件
    /// </summary>
    public void AnimFish()
    {
        SetBeginBtn(true);
        //抽奖动画完成!
        //显示当前奖励
        List<ConfigDada> conList = ConfigManager.GetConfigs<ActivityBigWheelConfig>();//获取 配置显示对应的奖品

        ActivityBigWheelConfig con;
        for (int i = 0; i < conList.Count; i++)
        {
            con = conList[i] as ActivityBigWheelConfig;
            if (reward.type == con.name)
            {
                if (!reward.type.Equals("nowinning"))
                {
                    mWin.SetActive(true);
                    mWinGoods.spriteName = con.iconUrl;
                    mWinGoods.MakePixelPerfect();
                    mNum.text = string.Format("x {0}", reward.num);
                    mDes.text = reward.note;
                }
                else
                {
                    mLost.SetActive(true);
                    mLostGoods.spriteName = con.iconUrl;
                    mLostGoods.MakePixelPerfect();
                }
                break;
            }
        }
    }

    /// <summary>
    /// 设置开始按钮动画
    /// </summary>
    /// <param name="enable"></param>
    private void SetBeginBtn(bool enable)
    {
        mBeginBtn.enabled = enable;
        TweenScale scal = mBeginBtn.GetComponent<TweenScale>();
        if (scal)
        {
            scal.enabled = enable;
        }
    }


    IEnumerator StartRun(Vector3 target)
    {
        SetBeginBtn(false);
        Hashtable table = new Hashtable();
        table.Add("rotation", -target);
        table.Add("time", UnityEngine.Random.Range(4.0f, 6.0f));
        table.Add("easeType", iTween.EaseType.easeInOutQuad);
        table.Add("oncomplete", (Action<object>)((complete) => { AnimFish(); }));
        iTween.RotateTo(mDish, table);

        yield break;
    }

    #region 按钮

    public void OnStartClick()
    {
        //
        SQDebug.Log("获取当前转盘奖励");

        if (prizeData.drawNum - prizeData.drawTotal <= 0)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("您没有抽奖次数");
            return;
        }

        Global.Inst.GetController<MainController>().SendPrizeBack((data) =>
        {
            prizeData.drawTotal++;

            mCount.text = string.Format("{0}次", prizeData.drawNum - prizeData.drawTotal);
            mPlayCount.text = string.Format("[7F6E36]游戏局数:[-][FFA70B]{0}[-]  [7F6E36]已抽奖次数:[-][FFA70B]{1}[-]",
                prizeData.games, prizeData.drawTotal);

            int index = prizeData.info.FindIndex((p) => p.prizeId == data.prizeInfo.prizeId);
            reward = data.prizeInfo;

            float angle = 18 + index * mItemAgle + UnityEngine.Random.Range(-mItemAgle * 0.35f, mItemAgle * 0.35f);

            angle += 360 * UnityEngine.Random.Range(6, 10);

            StartCoroutine(StartRun(Vector3.forward * angle));
        });
    }

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseBtnClick()
    {
        Close();
    }

    public void CheckGetRewardNote()
    {
        SQDebug.Log("打开中奖纪录面板");
        Global.Inst.GetController<MainController>().SendGetPrizeRecord((data) =>
        {
            ActivityRecordWidget wid = GetWidget<ActivityRecordWidget>("Windows/Mainview/ActivityRecordWidget", transform.parent);
            wid.ShowData(data);
        });
    }

    public void OnGetBtn_Click()
    {
        mWin.SetActive(false);

        if (reward.type.Equals("gold")) {
            PlayerModel.Inst.UserInfo.gold += reward.num;

            GlobalEvent.dispatch(eEvent.UPDATE_PROP, reward.num);
        }
    }

    public void OnSureBtn_Click()
    {
        mLost.SetActive(false);
    }

    #endregion

    protected override void OnDestroy()
    {
        if (!mBeginBtn.enabled)//正在旋转
        {
            //更新金币
            // PlayerModel.Inst.UpdateUserInfo(OperatingActivityModel.Inst.mLuckDishResultData.diamond,
            //OperatingActivityModel.Inst.mLuckDishResultData.gold,
            //OperatingActivityModel.Inst.mLuckDishResultData.ticket);
        }
        base.OnDestroy();
    }
}
