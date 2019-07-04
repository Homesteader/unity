using UnityEngine;
using System.Collections;

public class PayModel : BaseModel
{
    public static PayModel Inst;

    public override void SetController(BaseController c)
    {
        base.SetController(c);
        Inst = this;
    }
}
