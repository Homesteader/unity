using UnityEngine;
using System.Collections;

public class RoomGameToggleWidget : BaseViewWidget
{
    public UIToggle mTog;   //单选按钮
    public UILabel mName;   //游戏名字

    private CallBack<int, int> cb;
    private int listid, id;     //大类游戏ID, 小类游戏ID

    public void UpdateName(string name, int listid, int id, CallBack<int, int> call)
    {
        mName.text = name;
        cb = call;
        this.id = id;
        this.listid = listid;
    }


    //事件响应
    #region
    /// <summary>
    /// 响应单选按钮
    /// </summary>
    public void OnToggle_Click()
    {
        if (mTog.value == true)
            cb(listid, id);
    }
    #endregion
}
