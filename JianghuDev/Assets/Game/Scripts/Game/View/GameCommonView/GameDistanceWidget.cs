using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDistanceWidget : BaseViewWidget
{
    public GameDistanceItem mItem;

    public UIGrid mGrid;


    private Dictionary<string, GameDistanceItem> mDisDic = new Dictionary<string, GameDistanceItem>();

    /// <summary>
    /// 添加一个相对距离显示
    /// </summary>
    /// <param name="leftHeadUrl"></param>
    /// <param name="leftname"></param>
    /// <param name="leftuid"></param>
    /// <param name="rightHeadUrl"></param>
    /// <param name="rightname"></param>
    /// <param name="rightUid"></param>
    /// <param name="dis">相对距离</param>
    public void AddDistancePaire(string leftHeadUrl, string leftname, string leftuid,
        string rightHeadUrl, string rightname, string rightUid, string dis)
    {

        GameDistanceItem item = null;

        if (mDisDic.TryGetValue(leftuid + "+" + rightUid, out item) || mDisDic.TryGetValue(rightUid + "+" + leftuid, out item)) {
            item.InitUI(leftHeadUrl, leftname, leftuid, rightHeadUrl, rightname, rightUid, dis);
        }
        else {
            GameObject go = Assets.InstantiateChild(mGrid.gameObject, mItem.gameObject);
            go.gameObject.SetActive(true);
            item = go.GetComponent<GameDistanceItem>();
            item.InitUI(leftHeadUrl, leftname, leftuid, rightHeadUrl, rightname, rightUid, dis);
            mDisDic.Add(leftuid + "+" + rightUid, item);
        }

        mGrid.Reposition();
    }


    public void OnCloseClick() {
        Close<GameDistanceWidget>();
    }
}
