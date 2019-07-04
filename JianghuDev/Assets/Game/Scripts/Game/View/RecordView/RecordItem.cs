using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordItem : BaseViewWidget {

    public UITexture mIcon;//头像
    public UILabel mUserName;//玩家名字
    public UILabel mPoint;//分数
    public UILabel mTime;//对战时间

    private RecordItemData mData;//数据
    private Transform mRoot;//root

    //设置数据并显示
    public void SetData(Transform parent, RecordItemData data)
    {
        mData = data;
        mRoot = parent;
        RecordPlayerData p = data.record.Find(o => o.userId == PlayerModel.Inst.UserInfo.userId);
        //头像
        Assets.LoadIcon(p.headUrl, (t) =>
        {
            mIcon.mainTexture = t;
        });
        //名字
        mUserName.text = p.username;
        //分数
        mPoint.text = p.point > 0 ? ("+" + p.point.ToString("f2")) : p.point.ToString("f2");
        //时间
        mTime.text = data.time;
    }


    //详情点击
    public void OnDetailClick()
    {
        RecordDetailWidget v = GetWidget<RecordDetailWidget>("RecordView/RecordDetailWidget", mRoot);
        v.SetData(mData);
    }
}
