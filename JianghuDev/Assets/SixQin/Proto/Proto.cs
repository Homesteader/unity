using UnityEngine;
using System.Collections;


public class MsgResponseBase
{
    public int code = 0;
    public string desc = "";

    public bool CheckCode()
    {
        if (code == 1)
        {
            return true;
        }
        return false;
    }
}

