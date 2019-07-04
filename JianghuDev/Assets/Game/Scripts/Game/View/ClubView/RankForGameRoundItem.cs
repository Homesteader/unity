using UnityEngine;
using System.Collections;

public class RankForGameRoundItem : MonoBehaviour {

    public UILabel mNum;

    public UITexture mHead;

    public UILabel mNickName;

    public UILabel mUid;

    public UILabel mRound;



    public void InitUI(int num,string head,string uname,string uid,int round) {
        mNum.text = num.ToString();
        Assets.LoadIcon(head, (t) =>
        {
            mHead.mainTexture = t;
        });
        mNickName.text = uname;
        mUid.text = uid;
        mRound.text = round + "局";
    }

}
