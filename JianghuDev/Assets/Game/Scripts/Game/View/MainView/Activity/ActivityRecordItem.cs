using UnityEngine;
using System.Collections;

public class ActivityRecordItem : MonoBehaviour
{
    public UILabel mTime;
    public UILabel mName;
    public UILabel mNum;
    public UILabel mOpt;


    public void SetData(RecordInfo data)
    {
        mTime.text = data.time;
        mName.text = data.name;
        mNum.text = data.num.ToString();
        mOpt.text = data.state == "1" ? "已处理" : "未处理";
    }
}
