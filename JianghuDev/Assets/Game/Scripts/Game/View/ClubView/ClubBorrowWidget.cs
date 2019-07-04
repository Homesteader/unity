using UnityEngine;
using System.Collections;

public class ClubBorrowWidget : BaseViewWidget
{
    public UIInput mUserId;//玩家id

    public UIInput mInput;//输入框

    public UILabel mWare;

    private CallBack<float> mCallBack;

    protected override void Start()
    {
        base.Start();
        mWare.text = "(可打赏" + PlayerModel.Inst.UserInfo.wareHouse + "个)";
    }

    public void SetCallBack(CallBack<float> call)
    {
        mCallBack = call;
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="id"></param>
    public void SetData(string id)
    {
        mUserId.value = id;
    }


    #region 点击

    /// <summary>
    /// 删除成员
    /// </summary>
    public void OnDelUserClick() {
        Global.Inst.GetController<ClubController>().SendDelClubUser(ClubModel.Inst.mClubId, mUserId.value, () =>
        {
            ClubModel.Inst.RemovePlayer(mUserId.value);
        });
    }

    /// <summary>
    /// 关闭点击
    /// </summary>
    public void OnCloseClick()
    {
        Close();
    }

    /// <summary>
    /// 提交点击
    /// </summary>
    public void OnCommitClick()
    {
        if (string.IsNullOrEmpty(mInput.value))//输入为空
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("输入数量不能为空");
            return;
        }
        int num = 0;
        if (int.TryParse(mInput.value, out num))//输入的数字
        {
            if(num > 0)//输入的正整数且是100的倍数
            {
                Global.Inst.GetController<ClubController>().SendCheckUserInfo(mUserId.value, (nickName) =>
                 {
                     Global.Inst.GetController<CommonTipsController>().ShowTips("是否打赏玩家：" + nickName +" "+ int.Parse(mInput.value) + "金币？", "取消|确定",null, () =>
                     {
                         Global.Inst.GetController<ClubController>().SendBorrowGold(int.Parse(mInput.value), mUserId.value,(ware)=> {
                             mWare.text = "可打赏" + ware + "个";
                             PlayerModel.Inst.UserInfo.wareHouse = ware;
                             if (mCallBack != null)
                             {
                                 mCallBack(ware);
                             }
                             Close();
                         });
                     });
                 });
            }
            else
                Global.Inst.GetController<CommonTipsController>().ShowTips("请输入正整数");
        }
        else
            Global.Inst.GetController<CommonTipsController>().ShowTips("输入内容不合法");
    }
    #endregion

}
