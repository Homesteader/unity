using UnityEngine;
using System.Collections;
using System;

public class ClubController : BaseController
{
    public ClubView mView;
    public ClubController() : base("ClubView", "Windows/ClubView/ClubView")
    {
        SetModel<ClubModel>();
    }


    public override BaseView OpenWindow()
    {
        BaseView view = base.OpenWindow();
        if (mView == null)
            mView = view as ClubView;
        return view;
    }

    /// <summary>
    /// 获取俱乐部信息
    /// </summary>
    public void SendGetClubInfo()
    {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, ProtoIdMap.CMD_SendGetClubInfo, (msg) =>
        {
            ClubInfoAck ack = msg.Read<ClubInfoAck>();
            if (ack.code == 1 && ack.data.ClubInfo != null)
            {
                SendGetClubPlayerInfo();
                ClubModel.Inst.mClubId = ack.data.ClubInfo.clubId;
                ClubModel.Inst.mClubData = ack.data.ClubInfo;
            }
            else
            {
                if (ack.code == 15)
                {//需要创建一个俱乐部
                    BaseView.GetWidget<CreateClubWidget>(AssetsPathDic.CreateClubWidget, Global.Inst.GetController<MainController>().mView.transform);
                }
                else
                {
                    GameUtils.ShowErrorTips(ack.code);
                }
            }
        });
    }

    /// <summary>
    /// 创建俱乐部
    /// </summary>
    /// <param name="name"></param>
    public void SendCreateClub(string name) {
        SendCreateClubReq req = new SendCreateClubReq();
        req.name = name;
        NetProcess.SendRequest<SendCreateClubReq>(req, ProtoIdMap.CMD_SendCreateClub, (msg) => {
            SendCreateClubAck ack = msg.Read<SendCreateClubAck>();
            if (ack.code ==1) {
                Global.Inst.GetController<CommonTipsController>().ShowTips("创建成功!");
                BaseViewWidget.CloseWidget<CreateClubWidget>();
            }
            else {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 添加俱乐部玩家
    /// </summary>
    /// <param name="id"></param>
    public void SendAddUser(string id) {
        SendAddClubUser req = new SendAddClubUser();
        req.userId = id;
        NetProcess.SendRequest<SendAddClubUser>(req, ProtoIdMap.CMD_SendAddClubUser, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {
                Global.Inst.GetController<CommonTipsController>().ShowTips("申请已发送!");
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 获取俱乐部玩家列表
    /// </summary>
    /// <param name="call"></param>
    public void SendGetClubPlayerInfo()
    {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, ProtoIdMap.CMD_SendGetClubUsers, (msg) =>
        {
            ClubPlayerInfoAck data = msg.Read<ClubPlayerInfoAck>();
            if (data.code == 1)
            {
                if (data.data != null)
                    ClubModel.Inst.AllPlayerPeach = data.data.allPlayerPeach;
                if (data.data!=null && data.data.list!=null) {
                    ClubModel.Inst.ClubPlayerInfoData = data.data.list;
                }
                ClubView view = Global.Inst.GetController<ClubController>().OpenWindow() as ClubView;
                view.SetData(ClubModel.Inst.mClubData);
            }
            else
                GameUtils.ShowErrorTips(data.code);
        });
    }

    /// <summary>
    /// 获取俱乐部玩家信息并刷新
    /// </summary>
    /// <param name="callback"></param>
    public void SendGetClubPlayerInfoAndUpdate(CallBack<ClubInfo> callback)
    {
        NetProcess.SendRequest<CommonSendProto>(null, ProtoIdMap.CMD_SendGetClubUsers, (msg) =>
        {
            ClubPlayerInfoAck data = msg.Read<ClubPlayerInfoAck>();
            if (data.code == 1)
            {
                if (data.data != null)
                    ClubModel.Inst.AllPlayerPeach = data.data.allPlayerPeach;
                if (data.data != null && data.data.list != null)
                {
                    ClubModel.Inst.ClubPlayerInfoData = data.data.list;
                }
                if (callback != null)
                    callback(ClubModel.Inst.mClubData);
            }
            else
                GameUtils.ShowErrorTips(data.code);
        });
    }

    /// <summary>
    /// 添加联盟
    /// </summary>
    /// <param name="id"></param>
    public void SendAddClubConpany(string id) {
        SendAddClubConpany req = new global::SendAddClubConpany();
        req.clubId = id;
        NetProcess.SendRequest<SendAddClubConpany>(req, ProtoIdMap.CMD_SendAddClubConpany, (msg) => {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {
                Global.Inst.GetController<CommonTipsController>().ShowTips("申请已发送!");
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 获取俱乐部联盟列表
    /// </summary>
    /// <param name="clubid"></param>
    public void SendGetClubCompany(string clubid) {
        SendGetClubConpanyReq req = new SendGetClubConpanyReq();
        req.clubId = clubid;
        NetProcess.SendRequest<SendGetClubConpanyReq>(req, ProtoIdMap.CMD_SendGetClubCompanys, (msg) =>
        {
            SendGetClubConpanyAck ack = msg.Read<SendGetClubConpanyAck>();
            if (ack.code == 1)
            {
                ClubModel.Inst.ClubUnionInfoData = ack.data.list;
                ClubUnionWidget view = BaseView.GetWidget<ClubUnionWidget>("Windows/ClubView/ClubUnionWidget", mView.transform);
                view.InitUILabel(ack.data.num);
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 取消联盟
    /// </summary>
    /// <param name="clubid"></param>
    /// <param name="call"></param>
    public void SendCancelCompany(string clubid,Action call) {
        SendCancalCompanyReq req = new SendCancalCompanyReq();
        req.clubId = clubid;
        NetProcess.SendRequest<SendCancalCompanyReq>(req, ProtoIdMap.CMD_SendCancelCompany, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {
                if (call != null)
                {
                    call();
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 删除俱乐部成员
    /// </summary>
    /// <param name="clubId"></param>
    /// <param name="uid"></param>
    public void SendDelClubUser(string clubId,string uid,Action call) {
        SendDelClubUserReq req = new SendDelClubUserReq();
        req.clubId = clubId;
        req.uid = uid;
        NetProcess.SendRequest<SendDelClubUserReq>(req, ProtoIdMap.CMD_SendDelClubUser, (msg) =>
        {
            CommonRecieveProto ack = msg.Read<CommonRecieveProto>();
            if (ack.code == 1)
            {
                if (call!=null) {
                    call();
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }


    /// <summary>
    /// 核实玩家信息
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="call"></param>
    public void SendCheckUserInfo(string uid,CallBack<string> call) {
        SendCheckUserInfoReq req = new SendCheckUserInfoReq();
        req.uid = uid;
        NetProcess.SendRequest<SendCheckUserInfoReq>(req, ProtoIdMap.CMD_SendCheckUserInfo, (msg) =>
        {
            SendCheckUserInfoAck ack = msg.Read<SendCheckUserInfoAck>();
            if (ack.code == 1)
            {
                if (call!=null) {
                    call(ack.data.nickName);
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 借珍珠
    /// </summary>
    /// <param name="gold"></param>
    /// <param name="uid"></param>
    public void SendBorrowGold(int gold,string uid,CallBack<float> call) {
        SendBorrowReturnGold req = new SendBorrowReturnGold();
        req.glod = gold;
        req.uid = uid;
        NetProcess.SendRequest<SendBorrowReturnGold>(req, ProtoIdMap.CMD_SendBorrowGold, (msg) =>
        {
            SendBorrowReturnGoldAck ack = msg.Read<SendBorrowReturnGoldAck>();
            if (ack.code == 1)
            {
                //Global.Inst.GetController<CommonTipsController>().ShowTips("打赏成功!");
                PlayerModel.Inst.UserInfo.gold -= gold;
                GlobalEvent.dispatch(eEvent.UPDATE_PROP);
                if (call!=null) {
                    call(ack.data.ware);
                //}
                //if (mView!=null) {
                //    mView.UpdateByNet();
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 还珍珠
    /// </summary>
    /// <param name="gold"></param>
    /// <param name="uid"></param>
    public void SendReturnGold(int gold, string uid,Action call) {
        SendBorrowReturnGold req = new SendBorrowReturnGold();
        req.glod = gold;
        req.uid = uid;
        NetProcess.SendRequest<SendBorrowReturnGold>(req, ProtoIdMap.CMD_SendReturnGold, (msg) =>
        {
            SendBorrowReturnGoldAck ack = msg.Read<SendBorrowReturnGoldAck>();
            if (ack.code == 1)
            {
                if (call!=null) {
                    call();
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 提取盈利 1是俱乐部盈利 2是代理盈利
    /// </summary>
    public void SendGetOutWin(int type,CallBack<float> call) {
        SendGetOutWinGoldReq req = new SendGetOutWinGoldReq();
        req.type = type;
        NetProcess.SendRequest<SendGetOutWinGoldReq>(req, ProtoIdMap.CMD_SendGetOutWin, (msg) =>
        {
            SendGetOutWinGoldAck ack = msg.Read<SendGetOutWinGoldAck>();
            if (ack.code == 1)
            {
                if (call!=null) {
                    call(ack.data.gold);
                }   
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 请求获取代理收益
    /// </summary>
    public void SendGetAgentGain() {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, ProtoIdMap.CMD_SendGetAgentGain, (msg) =>
        {
            SendGetAgentWinAck ack = msg.Read<SendGetAgentWinAck>();
            if (ack.code == 1)
            {
                AgentGainWidget widget = Global.Inst.GetController<AgentGainController>().OpenWindow() as AgentGainWidget;
                if (ack.data!=null) {
                    mView.gameObject.SetActive(false);
                    float allpeach = PlayerModel.Inst.UserInfo.gold + PlayerModel.Inst.UserInfo.wareHouse;
                    widget.InitUI(ack.data.agentList, mView.gameObject, ack.data.agentNum, ClubModel.Inst.AllPlayerPeach, PlayerModel.Inst.UserInfo.wareHouse, ack.data.higherAgent);
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    //获取俱乐部和代理收益数量
    public void SetGetGainNumReq(Action<float,float> call) {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, ProtoIdMap.CMD_SendGetGain, (msg) =>
        {
            SendGetGainNumAck ack = msg.Read<SendGetGainNumAck>();
            if (ack.code == 1)
            {

                if (call!=null)
                {
                    call(ack.data.info.club, ack.data.info.agent);
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

    /// <summary>
    /// 请求局数排行版
    /// </summary>
    /// <param name="type"></param>
    /// <param name="call"></param>
    public void SendGetRankRound(string type,CallBack call) {
        SendGetRankForGameRoundReq req = new SendGetRankForGameRoundReq();
        req.type = type;
        NetProcess.SendRequest<SendGetRankForGameRoundReq>(req, ProtoIdMap.CMD_SendGetRankGameRound, (msg) =>
        {
            SendGetRankForGameRoundAck ack = msg.Read<SendGetRankForGameRoundAck>();
            if (ack.code == 1)
            {
                ClubModel.Inst.mRoundRank = ack.data.info == null ? new System.Collections.Generic.List<SendGetRankForGameRoundInfo>() : ack.data.info;
                if (call != null)
                {
                    call();
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

}
