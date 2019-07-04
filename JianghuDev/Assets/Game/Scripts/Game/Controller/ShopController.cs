using UnityEngine;
using System.Collections;

public class ShopController : BaseController
{

    public ShopController() : base("ShopView", "Windows/ShopView/ShopView")
    {

    }

    public void SendBuyRoomCard(int id)
    {
        SendPayBuyRoomCardReq req = new SendPayBuyRoomCardReq();
        req.id = id;
        NetProcess.SendRequest<SendPayBuyRoomCardReq>(req, ProtoIdMap.CMD_SendBuyRoomCard, (msg) =>
        {
            SendPayBuyRoomCardAck ack = msg.Read<SendPayBuyRoomCardAck>();

            if (ack.code == 1)
            {
                Global.Inst.GetController<CommonTipsController>().ShowTips("购买成功");
                if (ack.data != null)
                {
                    PlayerModel.Inst.UserInfo.gold = ack.data.gold;
                    PlayerModel.Inst.UserInfo.roomCard = ack.data.roomCard;
                    PlayerModel.Inst.UpdateRoomCardGlods(ack.data.roomCard, ack.data.gold);
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }

  
}
