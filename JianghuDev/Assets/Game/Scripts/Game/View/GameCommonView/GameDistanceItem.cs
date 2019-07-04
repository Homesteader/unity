using UnityEngine;
using System.Collections;

public class GameDistanceItem : MonoBehaviour {

    public UITexture mLeftHead;
    public UILabel mLeftName;
    public UILabel mLeftUID;

    public UILabel mDistance;

    public UITexture mRighttHead;
    public UILabel mRightName;
    public UILabel mRightUID;


    public void InitUI(string leftHeadUrl,string leftname,string leftuid,
        string rightHeadUrl,string rightname,string rightUid,string dis) {

        Assets.LoadIcon(leftHeadUrl, (t) => { mLeftHead.mainTexture = t; });
        mLeftName.text = leftname;
        mLeftUID.text = "UID：" + leftuid;

        Assets.LoadIcon(rightHeadUrl, (t) => { mRighttHead.mainTexture = t; });
        mRightName.text = rightname;
        mRightUID.text = "UID：" + rightUid;

        mDistance.text = dis;
    }




}
