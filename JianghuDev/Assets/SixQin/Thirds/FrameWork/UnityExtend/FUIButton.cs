using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FUIButton : UIButton {
    public CallBack ClickCall;//按钮点击回调
    public CallBack<GameObject> ClickCallObj;//按钮回调
    protected override void OnClick()
    {
        base.OnClick();
        if (ClickCall != null)
            ClickCall();
        if (ClickCallObj != null)
            ClickCallObj(gameObject);
    }
}
