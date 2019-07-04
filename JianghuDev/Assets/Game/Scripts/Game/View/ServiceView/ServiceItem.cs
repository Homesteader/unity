using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceItem : BaseViewWidget {

    public UILabel mTips;//提示内容

    private ServiceConfig mData;//数据
    private Transform mRoot;//父节点
    private string mWxId;//微信id

    //设置数据并显示
    public void SetData(Transform root,ServiceConfig con, string wxid)
    {
        mRoot = root;
        mData = con;
        mWxId = wxid;
        mTips.text = "[81452b]" + con.username + ":[-][603f30]24小时客服，请加微信[-]";
    }

    #region 按钮点击
    //复制微信
    public void OnCopyClick()
    {
       if(Application.platform != RuntimePlatform.WindowsEditor)
        {
            SixqinSDKManager.Inst.SendMsg(SixqinSDKManager.COPY_TEXT, mWxId);
        }
        Global.Inst.GetController<CommonTipsController>().ShowTips("复制成功");
    }

    //联系客服
    public void OnServiceClick()
    {
        string str = string.Format(mData.url, PlayerModel.Inst.UserInfo.userId, PlayerModel.Inst.UserInfo.userId);
        SQDebug.Log("客服:" + str);
        Application.OpenURL(str);
    }

    //赠送
    public void OnGiveClick()
    {
        ServiceGiveWidget view = GetWidget<ServiceGiveWidget>("ServiceView/ServiceGiveWidget", mRoot);
        view.SetData(mData);
    }
    #endregion
}
