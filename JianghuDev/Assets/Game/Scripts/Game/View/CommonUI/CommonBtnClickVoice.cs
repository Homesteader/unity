using UnityEngine;
using System.Collections;

public class CommonBtnClickVoice : MonoBehaviour {

	void OnClick()
    {
        Debug.Log("click " + gameObject.name);
        SoundProcess.PlaySound("Other/BtnClick", 0.7f);
    }
}
