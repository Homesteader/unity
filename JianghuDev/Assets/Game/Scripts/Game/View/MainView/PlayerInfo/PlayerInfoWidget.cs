using UnityEngine;
using System.Collections;

public class PlayerInfoWidget : BaseViewWidget
{
    public Transform mContent;
    public UITexture mIcon;//头像
    public UILabel mUserName;//玩家名字
    public UILabel mUserId;//玩家id
    public UILabel mCoinNum;//金币数量


    protected override void Awake()
    {
        base.Awake();
        InitData();
    }


    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="img">头像</param>
    /// <param name="name">名字</param>
    /// <param name="id">id</param>
    private void InitData()
    {
        if (PlayerModel.Inst.UserInfo != null)
        {
            //头像
            Assets.LoadIcon(PlayerModel.Inst.UserInfo.headUrl, (t) =>
            {
                mIcon.mainTexture = t;
            });
            //名字
            mUserName.text = PlayerModel.Inst.UserInfo.nickname;
            //id
            mUserId.text = PlayerModel.Inst.UserInfo.userId;
            //金币
            mCoinNum.text = PlayerModel.Inst.UserInfo.gold.ToString();
        }
        
    }


    /// <summary>
    /// 关闭点击
    /// </summary>
    public void OnCloseClick() {
        Close<PlayerInfoWidget>();
    }

    /// <summary>
    /// 修改点击
    /// </summary>
    public void OnFixClick() {
        GetWidget<FixPsdWidget>("MainView/PlayerInfoWidget/FixPsdWidget", transform);
    }
}
