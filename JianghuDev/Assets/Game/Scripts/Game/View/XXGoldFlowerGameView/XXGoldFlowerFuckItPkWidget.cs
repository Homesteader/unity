using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class XXGoldFlowerFuckItPkWidget : BaseViewWidget {
    
    public GameObject[] RightPlayers;
    

    public UITexture mLeftHead;

    public UILabel mLeftNickName;

    public UITexture mLeftWinLoseTex;
    
    

    public UITexture mRightWinLoseTex;
    


    /// <summary>
    /// 如果是左边玩家赢，为true
    /// </summary>
    private bool mLeftWin = false;
    /// <summary>
    /// 动画结束回调
    /// </summary>
    private Action mCallBack;


    /// <summary>
    /// 开始pk，如果是左边玩家赢，带入参数为true
    /// </summary>
    /// <param name="leftWin"></param>
    public void StartPk(bool leftWin, Action callBack)
    {
        mCallBack = callBack;
        mLeftWin = leftWin;
    }

    /// <summary>
    /// 初始化玩家信息
    /// </summary>
    /// <param name="rightSeatId"></param>
    /// <param name="leftSeatIds"></param>
    public void SetPlayers(int leftSeatId, List<int> rightSeatIds)
    {
        GoldFlowerPlayer player = null;
        for (int i = 0; i < rightSeatIds.Count;i++) {
            player = XXGoldFlowerGameModel.Inst.mPlayerInfoDic[rightSeatIds[i]];
            RightPlayers[i].gameObject.SetActive(true);
            Assets.LoadIcon(player.headUrl, (t) =>
            {
                RightPlayers[i].GetComponentInChildren<UITexture>().mainTexture = t;
            });
            RightPlayers[i].GetComponentInChildren<UILabel>().text = player.nickname;
        }
        for (int i = rightSeatIds.Count; i < RightPlayers.Length;i++) {
            RightPlayers[i].gameObject.SetActive(false);
        }
        player = XXGoldFlowerGameModel.Inst.mPlayerInfoDic[leftSeatId];
        Assets.LoadIcon(player.headUrl, (t) =>
        {
            mLeftHead.mainTexture = t;
        });
        mLeftNickName.text = player.nickname;
        StartCoroutine(PkLose());
    }



    public void OnCardMoveFinished()
    {
        StartCoroutine(PkLose());
    }


    private IEnumerator PkLose()
    {
        yield return new WaitForSeconds(0.5f);
        if (mLeftWin)
        {//左边玩家赢
            mRightWinLoseTex.color = Color.black;
            
        }
        else
        {
            mLeftWinLoseTex.color = Color.black;

        }

        yield return new WaitForSeconds(1f);
        if (mCallBack != null)
        {
            mCallBack();
        }
        Close<XXGoldFlowerFuckItPkWidget>();
    }


}
