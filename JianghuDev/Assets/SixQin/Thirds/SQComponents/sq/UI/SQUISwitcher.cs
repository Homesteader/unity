//----------------------------------------------
//            SQUIComponent: SQUIComponent Switcher
//            Copyright © 2017 by SixQin  
//----------------------------------------------
using UnityEngine;
using System.Collections;

public class SQUISwitcher : MonoBehaviour {
    public GameObject thumb;
    //默认是否选中    
    public bool IsSelect;
    //想要滑动的距离
    public float FixWidth = 0;
    //判断是否改变标志
    private bool jugeFlag;

	// Use this for initialization
	void Start () {
        this.jugeFlag = IsSelect;
	}
	
    void OnClick()
    {    
        IsSelect = !IsSelect;
        FixWidth = -FixWidth;
        transVal = -transVal;
    }

    private bool isFinished = true;
    private float finishVal = 0;
    private int transVal = -1;
	// Update is called once per frame
	void Update () {
        if (this.jugeFlag != IsSelect)
        {
            isFinished = false;
            this.jugeFlag = IsSelect;
        }

        if (!isFinished)
        {
            thumb.transform.Translate(new Vector3(transVal, 0,0) * Time.deltaTime);

            if(FixWidth > 0)
            {
                if (thumb.transform.localPosition.x > FixWidth)
                {
                    isFinished = true;
                }
            }
            else
            {
                if (thumb.transform.localPosition.x < FixWidth)
                {
                    isFinished = true;
                }
            }
         
        }
    
	}

}
