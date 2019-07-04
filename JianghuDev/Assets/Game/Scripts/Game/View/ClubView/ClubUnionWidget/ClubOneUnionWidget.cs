using UnityEngine;
using System.Collections;

public class ClubOneUnionWidget : BaseViewWidget
{
    public UILabel mId;//俱乐部id
    public UILabel mName;//俱乐部名字
    public UILabel mRichValue;//珍珠数量
    public UITexture mIcon;//俱乐部图标
    public UILabel mIndex;
    public GameObject mCancelUnionBtn;//取消联盟按钮

    private SendGetClubConpanyInfo mData;//数据
    private ClubUnionWidget mRootView;//父节点

    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="data"></param>
    public void SetData(int index,SendGetClubConpanyInfo data, ClubUnionWidget rootView)
    {
        mRootView = rootView;
        mIndex.text = (index + 1).ToString();
        mData = data;
        //id
        mId.text = data.cludId;
        //名字
        mName.text = data.clubName;
        //珍珠
        mRichValue.text = data.gold.ToString();
    }


    /// <summary>
    /// 取消联盟按钮点击
    /// </summary>
    public void OnCancelUnionClick()
    {
        Global.Inst.GetController<ClubController>().SendCancelCompany(mId.text,()=> {
            ClubModel.Inst.RemoveCompany(mId.text);
            Close<ClubOneUnionWidget>();
            GameObject go = null;
            if (BaseView.childrenWidgetDic.TryGetValue(typeof(ClubUnionWidget).Name, out go)) {
                go.GetComponent<ClubUnionWidget>().UpdateUI();
            }
        });
    }

    /// <summary>
    /// 点击自己
    /// </summary>
    public void OnItemClick()
    {
        ClubBorrowWidget view = BaseView.GetWidget<ClubBorrowWidget>("Windows/ClubView/ClubBorrowWidget", Global.Inst.GetController<ClubController>().mView.transform);
        view.SetData(mData.userId);
        view.SetCallBack((ware) =>
        {
            if (mRootView != null)
                mRootView.RefreshTiaopei();
        });
    }
}
