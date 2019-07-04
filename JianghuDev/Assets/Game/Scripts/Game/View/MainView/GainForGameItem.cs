using UnityEngine;
using System.Collections;

public class GainForGameItem : MonoBehaviour {

    public UILabel mTimeLabel;

    public UILabel mContentLabel;

    public void SetGainData(SendGetGainInfo item) {
        mTimeLabel.text = item.time;
        mContentLabel.text = "[432115]收益[46A30E]" + item.gold + "[-]金币于[B95137]" + item.nickName+"[-]。[-]";
    }


    public void InitUI(string time,string nickName,float gold,string type,float roomCard=0.0f) {
        mTimeLabel.text = time;
        switch (type)
        {//jc jr gh rc
            case "jc":
                mContentLabel.text = "[432115]打赏[D53535]" + gold + "[-]金币给[B95137]" + nickName + "[-]。[-]";
                break;
            case "jr":
                mContentLabel.text = "[432115]从 [B95137]" + nickName + "[-]" + " 获赏[46A30E]" + gold + "[-]金币。[-]";
                break;
            case "gh":
                mContentLabel.text = "[432115][B95137]" + nickName + "[-]打赏[46A30E]" + gold + "[-]金币。[-]";
                break;
            case "rc":
                mContentLabel.text = "[432115]消耗[ff4455]" + gold + "[-]金币购买[46A30E]" + roomCard + "[-]房卡。[-]";
                break;
        }
    }


    public void InitUserUI(string time, string nickName, float gold, string type) {
        mTimeLabel.text = time;
        switch (type)
        {//jr gh
            case "jr":
                mContentLabel.text = "[f34efe]" + nickName + "[-]" + "获赏[ff4455]" + gold + "[-]金币。";
                break;
            case "gh":
                mContentLabel.text = "[f34efe]" + nickName + "[-]打赏[ff4455]" + gold + "[-]金币。";
                break;
        }
    }

}
