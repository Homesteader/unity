using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


public class HttpProcess : MonoBehaviour {

    public bool   isShowLog = false;


    public delegate void HttpCallBack(string result);

    public static HttpProcess Instance = null;
    public  string url = null;

	void Start () {
        Instance = this;  
	}

    void OnDestroy()
    {
        Instance = null;  
    }

    public static void SendGet(string url,HttpCallBack callBack = null)
    {
        if (Instance == null || Instance.gameObject.activeSelf == false)
        {
            SQDebug.Log("Instance is null or active self == false,please send wait");
            return;
        }
        Instance.StartCoroutine(Instance.GetData(url, callBack));
    }


    public static void SendPost(string url, WWWForm form, HttpCallBack callBack = null)
    {
        if(Instance == null || Instance.gameObject.activeSelf == false)
        {
            SQDebug.Log("Instance is null or active self == false,please send wait" + url);
            return;
        }
        //
		Dictionary<string, string> header = new Dictionary<string, string>();
		//

        Instance.StartCoroutine(Instance.PostData(url, form, header, callBack));
    }

    IEnumerator PostData(string url, WWWForm form, Dictionary<string,string> header, HttpCallBack callBack = null)
    {
        
        if (isShowLog)
        {
            SQDebug.LogWarning(url);
        }
        WWW w = new WWW(url, form.data, header);
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            if (callBack == null)
            {
                SQDebug.Log("WWW  : "  + " erro info " + w.error);
                yield break;
            }
            ResultCallBack(callBack, w.error);
            yield break;
        }

        if (callBack != null)
        {
            ResultCallBack(callBack, w.text);
        }
    }

    IEnumerator GetData(string url, HttpCallBack callBack = null)
    {
        if (isShowLog)
        {
            SQDebug.LogWarning(url);
        }
        
        WWW w = new WWW(url);

       // w.he.Add("cookies", "openid=736455D011F599A8177806850F1839F5&&appid=1106110196&&openkey=3F6E4816A09D42953A5FF399A9C0A577");

        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            if(callBack == null)
            {
                SQDebug.Log("WWW error info " + w.error);
                yield break;
            }
            ResultCallBack(callBack,w.error);
            yield break;
        }

        if (callBack != null)
        {
            ResultCallBack(callBack, w.text);
        }
        
    }

    void ResultCallBack(HttpCallBack callBack,string result)
    {
        if (callBack.Method.IsStatic)
        {
            callBack(result);
            return;
        }
        if (callBack.Target.ToString() == "null")
        {
            SQDebug.LogWarning("Object was destroyed， but it always wanna to be handle");
            return;
        }
        callBack(result);
    }
}