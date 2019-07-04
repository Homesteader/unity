using UnityEngine;
using System.Collections;

public class WareHouseWidget : BaseViewWidget {
    

    public GameObject[] mItemArray;


    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick() {
        Close<WareHouseWidget>();
    }

    /// <summary>
    /// toggle点击
    /// </summary>
    /// <param name="go"></param>
    public void OnToggleClick(GameObject go) {
        if (UIToggle.current.value == false) {
            return;
        }
        int index = int.Parse(go.name);
        for (int i=0;i< mItemArray.Length;i++) {
            mItemArray[i].gameObject.SetActive(false);
        }
        mItemArray[index].gameObject.SetActive(true);
        if (index != 3)
        {
            //Global.Inst.GetController<MainController>().SendGetWareInfo(() => { });
            
        }
    }
    

}
