using UnityEngine;
using System.Collections;

public class YaoPai : MonoBehaviour
{

    public UIInput mInput;

    public void OnClickBtn()

    {
        OptRequest req = new OptRequest();
        req.ins = eMJInstructionsType.YAOPAI;
        req.cards = new System.Collections.Generic.List<int>();
        req.cards.Add(int.Parse(mInput.value));
        Global.Inst.GetController<MJGameController>().SendOptRequest(req, null, false);
    }

    public void ChagneHu()
    {

        MJGameModel.Inst.isHu = !MJGameModel.Inst.isHu;
        SQDebug.Log(MJGameModel.Inst.isHu);

    }

    public void GetSettle()
    {

        Global.Inst.GetController<MJGameController>().WaitSecond(MJGameModel.Inst.mSettlData);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetTouZiZero()
    {

        MJSenceRoot sRoot = Global.Inst.GetController<MJGameController>().mGameUI.mMJSceneRoot;
        for (int i = 0; i < sRoot.mTouzi.Length; i++)
        {


            Vector3 agule = sRoot.mTouzi[i].localRotation.eulerAngles;
            ;
            float a = 0f;
            Vector3 ver = new Vector3(1, 0, 0);
            if (agule.x != 0)
            {
                a = agule.x;
            }
            else if (agule.y != 0)
            {
                a = agule.y;
            }
            else if (agule.z != 0)
            {
                a = agule.z;
                ver = new Vector3(0, 0, 1);
            }
            sRoot.mTouzi[i].Rotate(ver, -a);
        }


    }

    public void SetTouZiView()
    {
        MJSenceRoot sRoot = Global.Inst.GetController<MJGameController>().mGameUI.mMJSceneRoot;
        sRoot.SetTouZiNum(MJGameModel.Inst.mStartGameData.startInfo.dices);
    }
}
