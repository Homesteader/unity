using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRoomController : BaseController {

	public SelectRoomController():base("SelectRoomView", "Windows/SelectRoomView/SelectRoomView")
    {

    }


    /// <summary>
    /// 获取平台场的人数
    /// </summary>
    /// <param name="call"></param>
    public void SendGetGoldPeopleNum(CallBack<List<int>> call)
    {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, ProtoIdMap.CMD_SendGetGoldPeopleNum, (msg) =>
        {
            GamePatternSendGetGoldPeopleNumAck ack = msg.Read<GamePatternSendGetGoldPeopleNumAck>();
            if (ack.code == 1)
            {
                if (call != null && ack.data != null)
                {
                    call(ack.data.nums);
                }
            }
        }, false);
    }
}
