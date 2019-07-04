using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

public static class GameUtils
{
    #region MD5加密
    // 使用MD5加密  32位
    public static string GetMd5(string str)
    {
        string password = "";
        MD5 md5 = new MD5CryptoServiceProvider();  //实例化一个md5对像
        byte[] bytes = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(str));//加密后是一个字节类型的数组
        password = System.BitConverter.ToString(bytes);
        password = password.Replace("-", "");
        password = password.ToLower();
        return password;
    }
    // MD5加密  16位
    public static string GetMd5Str(string ConvertString)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        string t2 = System.BitConverter.ToString(md5.ComputeHash(System.Text.UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
        t2 = t2.Replace("-", "");
        t2 = t2.ToLower();
        return t2;
    }
    #endregion

    /// <summary>
    /// 清除子节点
    /// </summary>
    /// <param name="tran"></param>
    public static void ClearChildren(Transform tran)
    {
        int len = tran.childCount;
        //List<GameObject> list = new List<GameObject>();
        GameObject obj;
        for (int i = 0; i < len; i++)
        {
            obj = tran.GetChild(i).gameObject;
            obj.SetActive(false);
            GameObject.Destroy(obj);
        }
    }

    /// <summary>
    /// 添加子对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static T AddChild<T>(T item, Transform parent) where T : Component
    {
        T newItem = GameObject.Instantiate<T>(item);
        Transform tran = newItem.transform;
        tran.parent = parent;
        tran.localPosition = Vector3.zero;
        tran.localRotation = item.transform.rotation;
        tran.localScale = item.transform.localScale;
        tran.gameObject.SetActive(true);
        return newItem;
    }

    /// <summary>
    /// 获取裁剪后的文字
    /// </summary>
    /// <param name="s">文字</param>
    /// <param name="label">label的长宽需要是固定的</param>
    /// <param name="changeLen">替换文字的长度</param>
    /// <param name="changeStr">替换文字</param>
    /// <returns></returns>
    public static string GetClampText(string s, UILabel label, int changeLen, string changeStr)
    {
        string strOut = string.Empty;
        if (!string.IsNullOrEmpty(s))
        {
            label.text = s;
            // 当前配置下的UILabel是否能够包围Text内容
            // Wrap是NGUI中自带的方法，其中strContent表示要在UILabel中显示的内容，strOur表示处理好后返回的字符串，uiLabel.height是字符串的高度 。
            bool bWarp = label.Wrap(s, out strOut, label.height);
            // 如果不能，就是说Text内容不能全部显示，这个时候，我们把最后一个字符去掉，换成省略号"..."
            if (!bWarp)
            {
                strOut = strOut.Substring(0, strOut.Length - changeLen);
                strOut += changeStr;
            }
        }
        return strOut;
    }


    /// <summary>
    /// 显示错误提示
    /// </summary>
    /// <param name="code"></param>
    public static void ShowErrorTips(int code)
    {
        Global.Inst.GetController<CommonTipsController>().ShowTips(CodeErrorTips.GetTips(code));
    }

    /// <summary>
    /// 获取图片转成base64
    /// </summary>
    /// <param name="pic"></param>
    /// <returns></returns>
    public static string GetTextureBase64(Texture pic)
    {
        Texture2D tex = GameObject.Instantiate(pic) as Texture2D;
        byte[] b = null;
        string f = tex.format.ToString();
        if (f.Contains("ARGB") || f.Contains("RGBA") || f.Contains("Alpha"))
            b = tex.EncodeToPNG();
        else
            b = tex.EncodeToJPG();
        GameObject.Destroy(tex);
        return System.Convert.ToBase64String(b);
    }


    /// <summary>
    /// 获取图片转成base64
    /// </summary>
    /// <param name="pic"></param>
    /// <returns></returns>
    public static string[] GetTextureBase64(List<Texture> pic)
    {
        if (pic == null)
            return null;
        string[] picStr = new string[pic.Count];
        for (int i = 0; i < pic.Count; i++)
        {
            picStr[i] = GetTextureBase64(pic[i]);
        }
        return picStr;
    }


    #region 通过int得到一个倒计时的string标识：hh:mm:ss

    /// <summary>
    /// 通过int得到一个倒计时的string标识：hh:mm:ss
    /// </summary>
    /// <param name="time">倒计时时间，int</param>
    /// <returns>倒计时格式------>hh:mm:ss</returns>
    public static string GetLastTime(int time)
    {
        int hh = (int)(time / 3600.0f);
        int mm = (int)(((time - hh * 3600) / 3600.0f) * 60);
        int ss = (int)(time - hh * 3600 - mm * 60);

        string shh;
        string smm;
        string sss;

        if (hh < 10)
        {
            shh = "0" + hh;
        }
        else
        {
            shh = hh + "";
        }

        if (mm < 10)
        {
            smm = "0" + mm;
        }
        else
        {
            smm = mm + "";
        }


        if (ss < 10)
        {
            sss = "0" + ss;
        }
        else
        {
            sss = ss + "";
        }

        return shh + ":" + smm + ":" + sss;
    }

    #endregion

    /// <summary>
    /// 将秒转化成hh:mm
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string GetTimeChangeHHMM(int time)
    {
        int h = time / 3600;//小时
        int m = (time - h * 3600) / 60;//分钟
        string t = (h < 10 ? ("0" + h) : h.ToString()) + ":" + (m < 10 ? ("0" + m) : m.ToString());
        return t;
    }

    /// <summary>
    /// 通过hh:mm的数据，得到int类型的时间
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int GetTimeIntByString(string str)
    {
        string[] temp = str.Split(':');
        int hh = int.Parse(temp[0]);//1小时是3600秒
        int mm = int.Parse(temp[1]);//1分钟是60秒
        return hh * 3600 + mm * 60;
    }

    public static string GetTransfParentListStr(Transform transf)
    {
        string str = "";
        Transform[] parentTransfs = transf.GetComponentsInParent<Transform>(true);
        for (int i = parentTransfs.Length - 1; i >= 0; i--)
        {
            str += parentTransfs[i].gameObject.name + "/";
        }
        if (parentTransfs.Length > 0)
            str = str.Substring(0, str.Length - 1);

        return str;
    }
}
