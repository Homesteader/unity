using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigManager : MonoBehaviour
{

    private Dictionary<string, List<ConfigDada>> AllConfig = new Dictionary<string, List<ConfigDada>>();
    private Dictionary<string, Object> mConfigCach = new Dictionary<string, Object>();//还未解析的配置表，需要用到的时候才解析，以免托管堆内存占用过大

    private static ConfigManager Inst;

    public static ConfigManager Creat()
    {
        if (Inst != null)
            return Inst;
        GameObject obj = new GameObject("ConfigManager");
        Inst = obj.AddComponent<ConfigManager>();
        GameObject.DontDestroyOnLoad(obj);
        return Inst;
    }


    public static void LoadAllConfig(string strUrl, int cachid,CallBack onFinish=null)
    {
        string url = SQResourceLoader.LoadAssetBundle("AssetsResources/allgameconfigs");
        if (Application.platform != RuntimePlatform.WindowsEditor )
        {
            url = strUrl;
        }
        Assets.LoadConfig(url, (bundle) =>
         {
             if (bundle != null)
             {
                 Object[] alldata = bundle.LoadAllAssets();
                 for (int i = 0; i < alldata.Length; i++)
                 {
                     Inst.mConfigCach[alldata[i].name] = alldata[i];
                 }
                 bundle.Unload(false);
                 if (onFinish != null)
                     onFinish();
             }
             else
             {
                 Global.Inst.GetController<CommonTipsController>().ShowTips("资源下载失败,请检查网络后重新下载", "确定", () =>
                 {
                     LoadAllConfig(url, cachid, onFinish);
                 });
             }
         }, cachid);
    }

    private List<ConfigDada> LoadOneConfig<T>(string name) where T : ConfigDada
    {
        if (!mConfigCach.ContainsKey(name))
            return null;
        string s = mConfigCach[name].ToString();
        List<T> con = Json.Deserialize<List<T>>(s);
        List<ConfigDada> condata = new List<ConfigDada>();
        for (int i = 0; i < con.Count; i++)
            condata.Add(con[i]);
        AllConfig.Add(name, condata);
        mConfigCach.Remove(name);
        return condata;
    }

    /// <summary>
    /// 获取配置表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<ConfigDada> GetConfigs<T>() where T : ConfigDada
    {
        string name = typeof(T).Name;
        List<ConfigDada> list = null;
        if (Inst.AllConfig.TryGetValue(name, out list))
            return list;
        else
            return Inst.LoadOneConfig<T>(name);
    }
}
