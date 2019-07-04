using UnityEngine;
using System.Collections;

public class RoomGameListWidget : BaseViewWidget
{
    public Color mLvColor;
    public Color mRedColor;
    public UILabel mLabel;   //游戏名字

    private CallBack<int> cb;
    private int id;  //游戏ID
    private UIToggle mToggle;

    protected override void Awake()
    {
        mToggle = GetComponent<UIToggle>();
    }

    /// <summary>
    /// 更新游戏名字
    /// </summary>
    /// <param name="name"></param>
    /// <param name="back"></param>
    public void UpdateName(string name, int id, CallBack<int> back)
    {
        mLabel.text = name;

        cb = back;
        this.id = id;
    }

    //事件响应
    public void OnGame_Click()
    {
        if (mToggle.value == true) {
            cb(this.id);
            mLabel.effectColor = mRedColor;
        }
        else {
            mLabel.effectColor = mLvColor;
        }
    }


}
