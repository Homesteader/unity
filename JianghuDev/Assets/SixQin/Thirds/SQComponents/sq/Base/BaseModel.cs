using UnityEngine;
using System.Collections;
using System;

public class BaseModel : IDisposable
{
    protected BaseController mController;


    public virtual void SetController(BaseController c)
    {
        mController = c;
    }

    public virtual void Dispose()
    {

    }
}
