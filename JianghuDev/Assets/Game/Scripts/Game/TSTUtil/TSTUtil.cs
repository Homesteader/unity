using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TSTUtil
{


    #region 单例

    private static TSTUtil _inst;

    public static TSTUtil Inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = new TSTUtil();
            }
            return _inst;
        }
    }

    #endregion

    #region 牌

    private static Dictionary<string, Color> ColorDic = new Dictionary<string, Color>();

    /// <summary>
    /// 获取牌颜色
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private static Color GetCardNumColor(string s)
    {
        if (ColorDic.Count == 0)
        {
            //类型1
            ColorDic.Add("a1", new Color(201 / 255f, 2 / 255f, 2 / 255f));//方块
            ColorDic.Add("b1", Color.black);//梅花
            ColorDic.Add("c1", ColorDic["a1"]);//红桃
            ColorDic.Add("d1", Color.black);//黑桃
            //类型2
            ColorDic.Add("a2", new Color(49 / 255f, 160 / 255f, 91 / 255f));//方块
            ColorDic.Add("b2", new Color(45 / 255f, 90 / 255f, 168 / 255f));//梅花
            ColorDic.Add("c2", new Color(216 / 255f, 86 / 255f, 4 / 255f));//红桃
            ColorDic.Add("d2", new Color(121 / 255f, 60 / 255f, 121 / 255f));//黑桃
        }
        Color c = Color.black;
        if (ColorDic.TryGetValue(s, out c))
            return c;
        return c;
    }
    /// <summary>
    /// 设置牌颜色和数字
    /// </summary>
    /// <param name="num">牌面的值</param>
    /// <param name="type">1是第2套牌，2是第2套牌</param>
    /// <param name="bg">牌的地板</param>
    /// <param name="numLabel">牌值的Label</param>
    public static void SetGameCardNum(string num, int type, UISprite bg, UILabel numLabel)
    {
        string s = num.Substring(0, 1);//花色 a:方块  b:梅花   c:红桃   d:黑桃
        string n = num.Substring(1, num.Length - 1);//值
        //背景
        bg.spriteName = s;
        //数字颜色
        numLabel.color = GetCardNumColor(s + type);
        //数字
        int m = int.Parse(n);
        if (m > 1 && m < 11)
            numLabel.text = n;
        else if (m == 11)
            numLabel.text = "J";
        else if (m == 12)
            numLabel.text = "Q";
        else if (m == 13)
            numLabel.text = "K";
        else if (m == 1)
            numLabel.text = "A";
        else if (m == 14)
            numLabel.text = "A";
    }

    /// <summary>
    /// 设置牌颜色和数字
    /// </summary>
    /// <param name="num">牌面的值</param>
    /// <param name="type"></param>
    /// <param name="bg"></param>
    /// <param name="numSpr">牌值的</param>
    public static void SetGameCardNum(string num, int type, UISprite bg, UISprite numSpr)
    {
        string spName = "";
        string s = num.Substring(0, 1);//花色 a:方块  b:梅花   c:红桃   d:黑桃
        string n = num.Substring(1, num.Length - 1);//值
        //背景
        bg.spriteName = s;
        //数字颜色
        string numType = "";
        if (s.Equals("a") || s.Equals("c"))
            numType = "red_";
        else
            numType = "black_";
        //数字
        int m = int.Parse(n);
        if (m == 14)
            m = 1;

        if (m == 50)
        {
            bg.spriteName = "xw";
            numSpr.spriteName = "";
        }
        else if (m == 51)
        {
            bg.spriteName = "dw";
            numSpr.spriteName = "";
        }
        else
        {
            numSpr.spriteName = numType + m;
        }
    }
    #endregion
}
