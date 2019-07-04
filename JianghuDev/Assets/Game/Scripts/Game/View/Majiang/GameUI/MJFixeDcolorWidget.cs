using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJFixeDcolorWidget : BaseViewWidget
{

    public eFixedType CurFixedcolorType;//定缺牌的类型
    public List<UIButton> mToggleList;//万条筒 List
    public GameObject[] mCircle;//万条筒的圈
    public UIGrid mGreid;
    public UIButton button;

    /// <summary>
    /// 初始化
    /// </summary>
    public void IntoView(eFixedType fixType)
    {
        //button.gameObject.SetActive(true);
        for (int i = 0; i < mToggleList.Count; i++)
        {
            mToggleList[i].gameObject.SetActive(true);
            mCircle[i].SetActive(false);
        }
        CurFixedcolorType = fixType;
        if (CurFixedcolorType == eFixedType.NONE)
            CurFixedcolorType = eFixedType.WAN;
        mCircle[(int)CurFixedcolorType - 1].SetActive(true);
        gameObject.SetActive(true);
        //mGreid.Reposition();
    }
    /// <summary>
    /// 确认按钮事件
    /// </summary>
    public void OnSureButtonClick()
    {

        for (int i = 0; i < mToggleList.Count; i++)
        {
            if (int.Parse(mToggleList[i].name.Split('_')[1]) == (int)CurFixedcolorType)
            {
                mToggleList[i].gameObject.SetActive(true);
            }
            else
            {
                mToggleList[i].gameObject.SetActive(false);
            }
        }
        OptRequest req = new OptRequest();
        req.ins = eMJInstructionsType.FIXEDCOLOR;
        req.type = (int)CurFixedcolorType;
        Global.Inst.GetController<MJGameController>().SendOptRequest(req, () => { button.gameObject.SetActive(false); });
    }


    public void OnClickBtn(GameObject obj)
    {
        string name = obj.name; ;
        switch (int.Parse(name.Split('_')[1]))
        {
            case 1:
                CurFixedcolorType = eFixedType.WAN;
                break;
            case 2:
                CurFixedcolorType = eFixedType.TIAO;
                break;
            case 3:
                CurFixedcolorType = eFixedType.TONG;
                break;
        }
        for (int i = 0; i < mToggleList.Count; i++)
        {
            if (obj.name == mToggleList[i].name)
                mCircle[i].SetActive(true);
            else
                mCircle[i].SetActive(false);
        }
        OptRequest req = new OptRequest();
        req.ins = eMJInstructionsType.FIXEDCOLOR;
        req.type = (int)CurFixedcolorType;
        Global.Inst.GetController<MJGameController>().SendOptRequest(req, () => { button.gameObject.SetActive(false); });
    }
}
