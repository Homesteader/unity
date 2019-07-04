using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceController : BaseController {

	public ServiceController():base("ServiceView", "Windows/ServiceView/ServiceView")
    {

    }

    //获取客服信息
    public void GetServiceInfo(CallBack<string[]> call)
    {
        NetProcess.SendRequest<CommonSendProto>(null, ProtoIdMap.CMD_GetServiceInfo, (msg) =>
        {
            ReceiveServiceInfo ack = msg.Read<ReceiveServiceInfo>();
            if (ack.code == 1)
            {
                if (call != null)
                {
                    call(ack.data.wechatId);
                }
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }
}
