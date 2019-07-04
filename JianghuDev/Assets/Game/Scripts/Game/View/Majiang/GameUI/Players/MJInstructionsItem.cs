using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJInstructionsItem : MonoBehaviour
{

    public eMJInstructionsType mOptIndex;//指令类型 
    public OptItemStruct mOpt;//指令
    private MJSelfPlayer mPlayer;

    public void OnItemClick()
    {
        //操作

        if (mOpt.ins == eMJInstructionsType.GANG)
        {
            if (mOpt.cards.Count > 1)
            {
                //mPlayer.ShowInstructions(null);//关闭选项界面
                mPlayer.SetSpeciaInstructions(mOpt);
                return;
            }
        }
        /*
        else if (mOpt.ins == eMJInstructionsType.chi)
        {
            List<MJliangcard> list = mPlayer.GetChiCards();
            if (list.Count > 1)
            {
                mPlayer.ShowInstructions(null);//关闭选项界面
                mPlayer.SetSpeciaInstructions(mOpt);
                return;
            }
        }
        */
        if (mPlayer != null)
            mPlayer.ShowInstructions(null);

        OptRequest req = new OptRequest();
        req.ins = mOptIndex;
        req.cards = new List<int>();
        if (mOpt.cards != null)
            req.cards.Add(mOpt.cards[0]);
        Global.Inst.GetController<MJGameController>().SendInstructions(req, () =>
        {
            if (mPlayer != null)
                mPlayer.ShowInstructions(null);
        });

    }

    /// <summary>
    /// 设置操作类型
    /// </summary>
    /// <param name="list"></param>
    public void SetOpt(OptItemStruct list, MJSelfPlayer player)
    {
        gameObject.SetActive(true);
        mOpt = list;
        mPlayer = player;
    }
}
