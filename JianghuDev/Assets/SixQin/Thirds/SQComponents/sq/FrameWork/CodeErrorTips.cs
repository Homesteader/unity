using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 服务器错误码返回提示
/// </summary>
public static class CodeErrorTips
{

    private static Dictionary<int, string> mDic = new Dictionary<int, string>();

    private static void AddCode()
    {
        mDic.Clear();
        List<ConfigDada> list = ConfigManager.GetConfigs<GameErrorCodeConfig>();
        GameErrorCodeConfig con;
        for (int i = 0; i < list.Count; i++)
        {
            con = list[i] as GameErrorCodeConfig;
            if (mDic.ContainsKey(con.code))
            {
                SQDebug.Log("GameErrorCodeConfig配置表包含相同的key：" + con.code);
            }
            else
                mDic.Add(con.code, con.message);
        }
    }

    /// <summary>
    /// 获取提示
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static string GetTips(int code, string re = "")
    {
        if (mDic.Count == 0)
            AddCode();
        string con;
        if (mDic.TryGetValue(code, out con))
        {
            return con;
        }
        else return re;
    }
}
