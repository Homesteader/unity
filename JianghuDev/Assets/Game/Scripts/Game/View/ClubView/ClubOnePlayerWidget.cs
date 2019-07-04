using UnityEngine;
using System.Collections;

public class ClubOnePlayerWidget : BaseViewWidget
{
    public UITexture mUserHeadIcon;//头像
    public UILabel mName;//名字
    public UILabel mId;//id
    public UILabel mPlayNum;//局数
    public UILabel mRichNum;//珍珠

    private Transform mParentRoot;//父节点root
    private ClubPlayerInfo mData;//数据

    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="data"></param>
    public void SetData(ClubPlayerInfo data, Transform parentRoot)
    {
        mData = data;
        mParentRoot = parentRoot;
        //头像
        Assets.LoadIcon(data.headUrl, (t) =>
        {
            mUserHeadIcon.mainTexture = t;
        });
        //名字
        mName.text = data.nickName;
        //id
        mId.text = "ID:"+data.userId;
        //局数
        //mPlayNum.text = data.playCount.ToString();
        //珍珠
        mRichNum.text = data.richNum.ToString();
    }


    /// <summary>
    /// 自己点击
    /// </summary>
    public void OnSelfClick()
    {
        ClubBorrowWidget view = GetWidget<ClubBorrowWidget>("Windows/ClubView/ClubBorrowWidget", mParentRoot);
        view.SetData(mData.userId);
        view.SetCallBack((t) =>
        {
            Global.Inst.GetController<ClubController>().SendGetClubPlayerInfo();
        });
        
    }
}
