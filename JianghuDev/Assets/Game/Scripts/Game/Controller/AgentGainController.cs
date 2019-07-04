using UnityEngine;
using System.Collections;

public class AgentGainController : BaseController
{
    public AgentGainController() :base("AgentGainWidget", "Windows/ClubView/AgentGainWidget")
    {

    }
    /// <summary>
    /// 获取总代信息
    /// </summary>
    /// <param name="callback"></param>
    public void GetGeneralData(CallBack<GeneralAgrentData> callback)
    {
        CommonSendProto req = new CommonSendProto();
        NetProcess.SendRequest<CommonSendProto>(req, ProtoIdMap.CMD_SendGetGeneralInfo, (msg) =>
        {
            GeneralAgrentRecieve data = msg.Read<GeneralAgrentRecieve>();
            if(data.code == 1)
            {
                if (callback != null)
                    callback(data.data);
            }
            else
            {
                GameUtils.ShowErrorTips(data.code);
            }
        });
    }

}
