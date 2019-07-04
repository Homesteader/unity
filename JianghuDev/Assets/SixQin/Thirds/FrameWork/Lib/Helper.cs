using UnityEngine;
using System.Collections;
using System.Text;
using System;

public class Helper  {

    #region Base64

    /// <summary>
    /// 编码base64
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Base64Encode(string str)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    }

    /// <summary>
    /// 解base64编码
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Base64Decode(string str)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(str));
    }

    public static int CardCompare(int c1, int c2) {
        if (c1 == c2) {
            return 0;
        }
        else if (c1 > c2) {
            return -1;
        }
        else {
            return 1;
        }
    }
    #endregion


}
