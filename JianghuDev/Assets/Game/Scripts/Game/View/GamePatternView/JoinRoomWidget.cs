using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JoinRoomWidget : BaseViewWidget {

    #region UI

    /// <summary>
    /// 房间号
    /// </summary>
    public UILabel[] mRoomIdLabel;

    #endregion

    #region 私有

    /// <summary>
    /// 现在输入的值
    /// </summary>
    private string mNowInputValue = "";

    /// <summary>
    /// 现在的数目
    /// </summary>
    private int mNowNum = 0;
    
    #endregion

    #region ui事件

    /// <summary>
    /// 键盘按钮点击事件
    /// </summary>
    /// <param name="go"></param>
    public void OnBtnItemClick(GameObject go)
    {
        int index = int.Parse(go.name);
        if (index > 10)
        {
            if (index == 11)
            {
                DeleteLastInput();
            }
            if (index == 12)
            {
                ClearInput();
            }
        }
        else
        {
            if (mNowInputValue.Length < 6)
            {
                mRoomIdLabel[mNowInputValue.Length].text = index + "";
                mNowInputValue += go.name;
                mNowNum = mNowInputValue.Length;
                if (mNowInputValue.Length >= 6)
                {
                    switch (GamePatternModel.Inst.mCurGameId)
                    {
                        case eGameType.MaJiang://麻将
                            Global.Inst.GetController<MJGameController>().SendJoinRoom(mNowInputValue);
                            break;
                        case eGameType.GoldFlower://金花
                            Global.Inst.GetController<XXGoldFlowerGameController>().SendJoinRoomReq(mNowInputValue);
                            break;
                        case eGameType.NiuNiu://牛牛
                            Global.Inst.GetController<NNGameController>().SendJoinRoomReq(mNowInputValue);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 背景点击
    /// </summary>
    public void OnBgClick()
    {
        Close<JoinRoomWidget>();
    }

    #endregion

    #region 内部

    /// <summary>
    /// 清空所有输入
    /// </summary>
    private void ClearInput()
    {
        mNowInputValue = "";
        foreach (var item in mRoomIdLabel)
        {
            item.text = "";
        }
        mNowNum = 0;
    }

    /// <summary>
    /// 删除最后一位输入
    /// </summary>
    private void DeleteLastInput()
    {
        if (mNowInputValue != "")
        {

            mNowInputValue = mNowInputValue.Remove(mNowInputValue.Length - 1, 1);
            mRoomIdLabel[mNowInputValue.Length].text = "";
            if (mNowInputValue.Length <= 0)
            {
                mNowInputValue = "";
            }

            mNowNum = mNowInputValue.Length;
        }
    }

    #endregion
}
