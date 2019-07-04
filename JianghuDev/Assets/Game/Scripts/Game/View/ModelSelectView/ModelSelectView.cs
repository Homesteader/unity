using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSelectView : BaseView {

    #region 按钮点击
    //快速匹配点击
    public void OnQuicklyClick()
    {
        Global.Inst.GetController<SelectRoomController>().SendGetGoldPeopleNum((data) =>
        {
            SelectRoomView v = Global.Inst.GetController<SelectRoomController>().OpenWindow() as SelectRoomView;
            v.SetData(data);
            Close();
        });
        
    }

    //创建房间点击
    public void OnCreateClick()
    {
        Global.Inst.GetController<CreateRoomController>().OpenWindow();
        Close();
    }

    //加入房间点击
    public void OnJoinClick()
    {
        Global.Inst.GetController<JoinRoomController>().OpenWindow();
        Close();
    }
    #endregion
}
