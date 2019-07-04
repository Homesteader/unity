using UnityEngine;
using System.Collections;

public class ActivityBigWheelItem : MonoBehaviour
{

    public UILabel label_bet;
    public Transform objTran;
    public UISprite iconBG;
    public UILabel labelName;
    public int id;
    public void SetItem(float angel, PrizeInfo data, ActivityBigWheelConfig config)
    {
        id = data.prizeId;
        objTran.transform.localPosition = Vector3.zero;
        objTran.localRotation = Quaternion.Euler(0, 0, angel);
        if (!data.type.Equals("nowinning"))
        {
            label_bet.gameObject.SetActive(true);
            label_bet.text = string.Format("x {0}", data.num);
            iconBG.gameObject.SetActive(true);
            iconBG.spriteName = config.iconUrl;
            iconBG.MakePixelPerfect();
            labelName.text = string.Empty;
        }
        else
        {
            iconBG.gameObject.SetActive(false);
            label_bet.gameObject.SetActive(false);
            labelName.text = data.name;
        }

        label_bet.color = Color.red;
    }
}
