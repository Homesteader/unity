using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Assets : MonoBehaviour
{

    private static Assets Inst;
    private Dictionary<string, Texture2D> CachTexture = new Dictionary<string, Texture2D>();//缓存的图片
    private List<string> CachTextureList = new List<string>();
    private const string mSendUrl = "http://h5future.com/depq/interface.php";//发送图片链接


    private void Awake()
    {
        Inst = this;
    }


    #region 加载图片
    /// <summary>
    /// 加载本地图片
    /// </summary>
    /// <param name="url"></param>
    /// <param name="call"></param>
    public static void LoadTexture(string url, CallBack<Texture2D> call)
    {
        Inst.StartCoroutine(Inst.LoadTexture(true, url, call));
    }

    /// <summary>
    /// 加载头像
    /// </summary>
    /// <param name="url"></param>
    /// <param name="call"></param>
    public static void LoadIcon(string url, CallBack<Texture2D> call,bool room = true)
    {
        Inst.StartCoroutine(Inst.LoadTexture(false, url, call,room));
    }

    IEnumerator LoadTexture(bool isRes, string url, CallBack<Texture2D> calback,bool room = true)
    {
        if (string.IsNullOrEmpty(url))
        {
            if (calback != null)
                calback(null);
        }
        else
        {
            if (room)
            {
                if (CachTexture.ContainsKey(url))
                {
                    calback(CachTexture[url]);
                }
                else
                {
                    Inst.StartCoroutine(LoadIcon(isRes,url,calback,room));
                }
            }
            else {
                Inst.StartCoroutine(LoadIcon(isRes, url, calback, room));
            }
            
        }
        yield return null;
    }


    private IEnumerator LoadIcon(bool isRes, string url, CallBack<Texture2D> calback, bool room = true) {
        Texture2D obj = null;
        if (isRes)//在resource里面加载
        {
            obj = Resources.Load(url) as Texture2D;
            if (null == obj)
                SQDebug.LogError("加载错误：" + url);
            if (null != calback)
            {
                AddTexture(url, obj);
                calback(obj);
            }
        }
        else//在网络地址中加载
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                if (null != calback)
                {
                    AddTexture(url, www.texture);
                    calback(www.texture);
                }
            }
            if (!string.IsNullOrEmpty(www.error))
                SQDebug.LogError("加载错误：" + url + "      " + www.error);
            www = null;
        }
    }

    private void AddTexture(string url, Texture2D obj)
    {
        if (CachTexture.ContainsKey(url))
            return;
        if (obj != null)
        {
            CachTexture[url] = obj;
            CachTextureList.Add(url);
            if (CachTextureList.Count > 20)
            {
                string t = CachTextureList[0];
                CachTexture.Remove(t);
                CachTextureList.RemoveAt(0);
            }
        }
    }

    /// <summary>
    /// 刷新缓存图片
    /// </summary>
    /// <param name="url"></param>
    /// <param name="obj"></param>
    public static void UpdateTexture(string url, Texture2D obj)
    {
        if (!string.IsNullOrEmpty(url) && obj == null && Inst.CachTexture.ContainsKey(url))
            Inst.CachTexture[url] = obj;

    }
    #endregion

    #region 加载配置表
    /// <summary>
    /// 添加配置表
    /// </summary>
    /// <param name="url"></param>
    /// <param name="call"></param>
    public static void LoadConfig(string url, CallBack<AssetBundle> call, int cachid)
    {
        Inst.StartCoroutine(Inst.LoadConfigIE(url, call, cachid));
    }


    IEnumerator LoadConfigIE(string url, CallBack<AssetBundle> call, int cachid)
    {
        WWW www = null;
        if (Application.platform == RuntimePlatform.WindowsEditor)
            www = new WWW(url);
        else
            www = WWW.LoadFromCacheOrDownload(url, cachid);

        yield return www;
        if (www.isDone && string.IsNullOrEmpty(www.error))
        {
            if (null != call)
                call(www.assetBundle);
        }
        if (!string.IsNullOrEmpty(www.error))
        {
            SQDebug.LogError("加载错误：" + url + "      " + www.error);
            if (null != call)
                call(null);
        }
    }

    #endregion


    #region 加载预制体
    /// <summary>
    /// 加载预制体
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static GameObject LoadPrefab(string path)
    {
        GameObject obj = Resources.Load(path) as GameObject;
        if (obj == null)
            SQDebug.LogWarning("加载路径：" + path + "  错误");
        return obj;
    }
    #endregion

    #region 上传图片
    /// <summary>
    /// 路径头
    /// </summary>
    public static string FinalPathHead
    {
        get
        {
            string url = "";
            if (Application.platform == RuntimePlatform.Android)
                url = "jar:file://";
            else
                url = "file:///";
            return url;
        }
    }

    /// <summary>
    /// 上传图片
    /// </summary>
    /// <param name="img">图片</param>
    /// <param name="call">回调方法，返回的链表长度为0表示上传失败</param>
    public static void SendPng(string[] img, CallBack<List<string>> call)
    {
        Inst.StartCoroutine(Inst.SendPngToServer(img, call));
    }
    /// <summary>
    /// 上传图片
    /// </summary>
    /// <param name="img"></param>
    /// <param name="call"></param>
    /// <returns></returns>
    IEnumerator SendPngToServer(string[] img, CallBack<List<string>> call)
    {
        List<string> imgurl = new List<string>();
        if (img != null && img.Length > 0)
        {
            Global.Inst.GetController<NetLoadingController>().ShowLoading(true,true);
            for (int i = 0; i < img.Length; i++)
            {
                WWWForm f = new WWWForm();
                f.AddField("act", "upPic");
                f.AddField("picBin", img[i]);
                
                WWW www = new WWW(mSendUrl, f);
                yield return www;
                if (www.isDone && string.IsNullOrEmpty(www.error))
                {
                    imgurl.Add(www.text);
                    SQDebug.Log("上传图片：" + www.text);
                }
                if (!string.IsNullOrEmpty(www.error))
                    SQDebug.Log("加载错误：第" + (i + 1) + "张      " + www.error);
                www = null;
            }
        }
        Global.Inst.GetController<NetLoadingController>().ShowLoading(false);
        if (call != null)
            call(imgurl);
        yield return 0;
    }
    #endregion

    #region 加载战绩文件

    /// <summary>
    /// 加载txt文件
    /// </summary>
    /// <param name="url"></param>
    public static void LoadTxtFileCallText(string url, CallBack<string> call, bool isshowLoading = true)
    {
        Inst.StartCoroutine(Inst.IELoadTxtFile(url, call, isshowLoading));
    }

    /// <summary>
    /// 加载txt文件
    /// </summary>
    /// <param name="url"></param>
    /// <param name="call"></param>
    /// <param name="isshowLoading"></param>
    public static void LoadTxtFileCallBytes(string url, CallBack<byte[]> call, bool isshowLoading = true)
    {
        Inst.StartCoroutine(Inst.IELoadTxtFile(url, call, isshowLoading));
    }

    /// <summary>
    /// 协议加载txt文件
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator IELoadTxtFile(string url, CallBack<string> call, bool isshowLoading = true)
    {
        Global.Inst.GetController<NetLoadingController>().ShowLoading(isshowLoading);

        yield return null;
        WWW www = new WWW(url);
        yield return www;
        if (www.isDone && www.error == null)
        {
            if (call != null)
            {
                SQDebug.Print("从连接地址:--->" + url + "   加载的txt文件内容为:--->" + www.text);
                call(www.text);
            }
            www.Dispose();
        }
        else
        {
            if (call != null)
            {
                SQDebug.Print("从连接地址：--->" + url + "   加载txt数据失败");
                call("");
            }
        }
        www = null;
        Global.Inst.GetController<NetLoadingController>().ShowLoading(false);
    }

    /// <summary>
    /// 协议加载txt文件
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator IELoadTxtFile(string url, CallBack<byte[]> call, bool isshowLoading = true)
    {
        Global.Inst.GetController<NetLoadingController>().ShowLoading(isshowLoading);

        yield return null;
        WWW www = new WWW(url);
        yield return www;
        if (www.isDone && www.error == null)
        {
            if (call != null)
            {
                SQDebug.Print("从连接地址:--->" + url + "   加载的txt文件内容为:--->" + www.text);
                call(www.bytes);
            }
            www.Dispose();
        }
        else
        {
            if (call != null)
            {
                SQDebug.Print("从连接地址：--->" + url + "   加载txt数据失败");
                call(null);
            }
        }
        www = null;
        Global.Inst.GetController<NetLoadingController>().ShowLoading(false);
    }

    #endregion

    #region 生成子对象

    /// <summary>
    /// 生成字节点
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="child"></param>
    /// <param name="sv"></param>
    /// <returns></returns>
    public static GameObject InstantiateChild(GameObject parent, GameObject child, UIScrollView sv = null) {
        GameObject go = NGUITools.AddChild(parent, child);
        if (sv!=null) {
            UIDragScrollView drag = go.GetComponentInChildren<UIDragScrollView>();
            if (drag!=null) {
                drag.scrollView = sv;
            }
        }
        return go;
    }

    #endregion
}

