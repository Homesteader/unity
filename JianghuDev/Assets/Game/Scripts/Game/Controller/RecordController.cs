using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordController : BaseController {

	public RecordController() : base("RecordView", "Windows/RecordView/RecordView")
    {

    }

    //获取记录
    public void GetRecord(CallBack<List<RecordItemData>> call)
    {
        NetProcess.SendRequest<CommonSendProto>(null, ProtoIdMap.CMD_GetRecord, (msg) =>
        {
            RecordData data = msg.Read<RecordData>();
            if (data.code == 1)
            {
                if (call != null)
                    call(data.data);
            }
            else
            {
                GameUtils.ShowErrorTips(data.code);
            }
        });
    }
}
