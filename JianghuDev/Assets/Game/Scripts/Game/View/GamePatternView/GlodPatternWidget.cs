using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlodPatternWidget : MonoBehaviour {

    public UIScrollView mScrollView;

    public GoldPatternItem mItem;

    public UIGrid mGrid;


    /// <summary>
    /// 通过配置表初始化ui
    /// </summary>
    public void InitByConfig() {

        GamePatternView view = Global.Inst.GetController<GamePatternController>().mView;

        List<ConfigDada> configs = ConfigManager.GetConfigs<GameGoldPatternConfig>();
        GameGoldPatternConfig con = null;
        for (int i=0;i< configs.Count;i++) {
            GameGoldPatternConfig temp = configs[i] as GameGoldPatternConfig;
            if (temp.gameId == (int)GamePatternModel.Inst.mCurGameId) {
                con = temp;
            }
        }

        if (GamePatternModel.Inst.mCurGameId == eGameType.MaJiang)//麻将特殊处理一下，向下滑动，一排四个
        {
            //mScrollView.movement = UIScrollView.Movement.Vertical;
            //mGrid.arrangement = UIGrid.Arrangement.Vertical;
            mGrid.transform.localScale = Vector3.one * 1f;
            //mGrid.cellWidth = 455;
            //mGrid.cellHeight = 550;
             mGrid.maxPerLine = 5;
            //mGrid.transform.localPosition = new Vector3(-4, -98, 0);
        }
        else if (GamePatternModel.Inst.mCurGameId == eGameType.GoldFlower)//金花
        {
        //   mScrollView.movement = UIScrollView.Movement.Vertical;
            mGrid.arrangement = UIGrid.Arrangement.Vertical;
            //mGrid.cellWidth = 455;
            //mGrid.cellHeight = 550;
             mGrid.maxPerLine = 2;
        }
        else
        {
            mGrid.maxPerLine = 3;
        }

        if (con!=null) {
            for (int i=0; i < con.config.Count;i++) {
                GameObject go = Assets.InstantiateChild(mGrid.gameObject, mItem.gameObject, mScrollView);
                go.gameObject.SetActive(true);
                GoldPatternItem item = go.GetComponent<GoldPatternItem>();
                if (item!=null) {
                    item.InitUIByConfig(con.config[i]);
                }
                view.AddGoldPatternItem(con.config[i].lvId, item);
            }
            mGrid.Reposition();
        }
    }
}
