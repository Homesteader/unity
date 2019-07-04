using UnityEngine;
using System.Collections;

//IP预警
public class MJIpWarningInfoView : BaseViewWidget
{

    public GameObject mItem;
    public UITable mTable;

    /// <summary>
    /// 设置Ip警告信息
    /// </summary>
    /// <param name="data"></param>
    public void SetIpWarningData(MJGameIpInfo[] data)
    {
        int count = data == null ? 0 : data.Length;
        int len = mTable.transform.childCount;
        GameObject obj;
        for (int i = 0; i < count; i++)
        {
            if (i < len)
                obj = mTable.transform.GetChild(i).gameObject;
            else
                obj = NGUITools.AddChild(mTable.gameObject, mItem);
            obj.SetActive(true);
            string name = GetPlayerNames(data[i]);
            obj.GetComponent<UILabel>().text = "玩家 " + @name + " 具有相同的IP地址：" + @data[i].ip;
        }
        for (int i = count; i < len; i++)
            mTable.transform.GetChild(i).gameObject.SetActive(false);
        mTable.Reposition();
    }

    /// <summary>
    /// 设置GPS警告信息
    /// </summary>
    /// <param name="s"></param>
    public void SetGPSWarningData(MJGameGpsWarningPlayerInfo[] info)
    {
        int count = info == null ? 0 : info.Length;
        int len = mTable.transform.childCount;
        GameObject obj;
        for (int i = 0; i < count; i++)
        {
            if (i < len)
                obj = mTable.transform.GetChild(i).gameObject;
            else
                obj = NGUITools.AddChild(mTable.gameObject, mItem);
            obj.SetActive(true);
            obj.GetComponent<UILabel>().text = Helper.Base64Decode(info[i].playerA) + " 与 " + Helper.Base64Decode(info[i].playerB) + " 距离小于200米";
        }
        for (int i = count; i < len; i++)
            mTable.transform.GetChild(i).gameObject.SetActive(false);
        mTable.Reposition();
    }


    private string GetPlayerNames(MJGameIpInfo info)
    {
        if (info == null || info.seatId == null)
            return "";
        string rolename = "";
        int len = info.seatId.Length;
        MJGameModel model = MJGameModel.Inst;
        for (int i = 0; i < len; i++)
        {
            if (model.mRoomPlayers[info.seatId[i]] != null)
            {
                string n = model.mRoomPlayers[info.seatId[i]].nickName.Replace("\u0000", "");
                if (i == 0)
                    rolename = n;
                else
                    rolename = rolename + "、" + n;
            }
        }
        return rolename;
    }

    /// <summary>
    /// 继续游戏按钮点击
    /// </summary>
    public void OnContinueClick()
    {
        Close<MJIpWarningInfoView>();
        gameObject.SetActive(false);
        //Global.Inst.GetController<GameController>().SendContinue();
    }


    /// <summary>
    /// 离开房间按钮点击
    /// </summary>
    public void OnCloseClick()
    {
        Global.Inst.GetController<MJGameController>().SendLeaveRoom(() =>
        {
            NetProcess.InitNetWork(GameManager.Instance.Ip, GameManager.Instance.port);
            Global.Inst.GetController<MJGameController>().ConnectedToHallServer(null);
        });
    }
}
