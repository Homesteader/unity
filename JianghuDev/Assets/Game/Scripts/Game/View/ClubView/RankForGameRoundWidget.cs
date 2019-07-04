using UnityEngine;
using System.Collections;

public class RankForGameRoundWidget : BaseViewWidget {

    public GameObject mToggleItem;
    public UIGrid mToggleGrid;

    public GameObject mMessageItem;
    public UIPanel mPanel;//panel
    public UIScrollView mScroll;
    public SingleScroll mSingleScroll;//SingleaSroll
    public UIGrid grid;

    private int LastAll = 0;

    private int mAllIndex;


    #region Unity
    #region Unity

    protected override void Awake()
    {
        base.Awake();

        mSingleScroll.AddEvent(eFuiScrollViewEvent.InitItem, OnItemInit);
        mSingleScroll.AddEvent(eFuiScrollViewEvent.UpdateItem, OnItemUpdate);
    }

    #endregion

    #endregion

    #region 初始化界面

    /// <summary>
    /// 初始化  NN ZJH MJ AGENT
    /// </summary>
    /// <param name="list"></param>
    public void InitUI(params string[] list) {
        GameObject go = null;
        int start = int.Parse(list[list.Length - 1]) -1;//初始按钮
        for (int i=0;i<list.Length - 1;i++) {
            go = NGUITools.AddChild(mToggleGrid.gameObject, mToggleItem.gameObject);
            go.name = list[i].ToLower();
            go.gameObject.SetActive(true);
            UIToggle toggle = go.GetComponentInChildren<UIToggle>();
            UILabel[] label = go.GetComponentsInChildren<UILabel>();
            string temp = "";
            switch (list[i])
            {
                case "MJ":
                    temp = "牛仔拼十";
                    break;
                case "ZJH":
                    temp = "拼三张";
                    break;
                case "NN":
                    temp = "跑得快";
                    break;
                case "AGENT":
                    temp = "代理";
                    break;
            }
            for (int k = 0; k < label.Length; k++)
            {
                label[k].text = temp;
            }

            if (i == start && toggle!=null) {
                toggle.value = true;
            }
        }

        mToggleGrid.Reposition();
    }

    #endregion


    #region 显示数据

    /// <summary>
    /// 显示朋友圈
    /// </summary>
    private void ShowItems()
    {
        mAllIndex = ClubModel.Inst.mRoundRank.Count;
        mSingleScroll.gameObject.SetActive(true);
        mSingleScroll.SetData(ClubModel.Inst.mRoundRank.Count);
    }


    #endregion

    #region 初始化

    private void OnItemInit(params object[] args)
    {
        int index = int.Parse(args[0].ToString());
       GameObject parent = args[1] as GameObject;
        GameObject obj = NGUITools.AddChild(parent, mMessageItem);
        obj.SetActive(true);
        obj.transform.localPosition = new Vector3(-398, 0, 0);
        obj.GetComponent<RankForGameRoundItem>().InitUI(index + 1,
            ClubModel.Inst.mRoundRank[index].headUrl,
            ClubModel.Inst.mRoundRank[index].nickName,
            ClubModel.Inst.mRoundRank[index].userId,
            ClubModel.Inst.mRoundRank[index].round);

    }


    private void OnItemUpdate(params object[] args)
    {
        int index = int.Parse(args[0].ToString());
        GameObject obj = args[1] as GameObject;
        obj.GetComponent<RankForGameRoundItem>().InitUI(index + 1,
            ClubModel.Inst.mRoundRank[index].headUrl,
            ClubModel.Inst.mRoundRank[index].nickName,
            ClubModel.Inst.mRoundRank[index].userId,
            ClubModel.Inst.mRoundRank[index].round);
    }

    public void AddLast()
    {
        mSingleScroll.AddItemByLast();
    }

    #endregion


    #region 按钮点击事件

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick() {
        ClubModel.Inst.mRoundRank.Clear();
        Close<RankForGameRoundWidget>();
    }

    /// <summary>
    /// toggle点击事件
    /// </summary>
    /// <param name="go"></param>
    public void OnTogggleClick(GameObject go) {
        if (UIToggle.current.value) {

            Global.Inst.GetController<ClubController>().SendGetRankRound(go.name, () =>
            {
                ShowItems();
            });
        }
    }

    #endregion


}
