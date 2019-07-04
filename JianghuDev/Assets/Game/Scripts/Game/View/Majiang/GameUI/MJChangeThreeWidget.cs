using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJChangeThreeWidget : MonoBehaviour
{



    public void IntoThreeWidget(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

    public void OnSureBtnClick()
    {
        if (MJGameModel.Inst.mCurSlectCardList.Count != 3)
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("选择的牌不符合要求");
            return;
        }
        SQDebug.Log("确定换3张-->发送消息");
        OptRequest req = new OptRequest();
        req.ins = eMJInstructionsType.CHANGETHREE;
        List<int> newIntList = new List<int>();
        foreach (var item in MJGameModel.Inst.mCurSlectCardList)
        {
            //Destroy(item.gameObject);
            item.gameObject.SetActive(false);
            newIntList.Add(item.mNum);
        }
        req.cards = newIntList;
        Global.Inst.GetController<MJGameController>().SendOptRequest(req);
    }
}
