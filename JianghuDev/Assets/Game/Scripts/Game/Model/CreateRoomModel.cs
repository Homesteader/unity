using UnityEngine;
using System.Collections.Generic;

public class CreateRoomModel : BaseModel
{

    public static CreateRoomModel Inst;


    public override void SetController(BaseController c)
    {
        base.SetController(c);

        Inst = this;
    }


    public int mSubGameId;
    /// <summary>
    /// 创建的规则item
    /// </summary>
    public List<CheckBoxItemWidget> RuleItems;

    /// <summary>
    /// 输入Item
    /// </summary>
    public List<RoomRuleInputItem> InputItems;



    public bool GetConnect(int index)
    {
        if (index == 0) return true;
        foreach (var item in RuleItems)
            if (item.GetIndex() == index) return true;
        return false;
    }
}