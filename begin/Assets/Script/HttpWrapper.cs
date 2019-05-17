using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

public class HttpWrapper : MonoBehaviour
{

    public void GET(string url, Action<WWW> onSuccess, Action<WWW> onFail = null)
    {
        WWW www = new WWW(url);
        StartCoroutine(WaitForResponse(www, onSuccess, onFail));   
        
    }

    public void POST(string url, Dictionary<string, string> post,
        Action<WWW> onSuccess, Action<WWW> onFail = null)
    {
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<string, string> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(url, form);
        StartCoroutine(WaitForResponse(www, onSuccess, onFail));
    }

    IEnumerator WaitForResponse(WWW www, Action<WWW> onSuccess, Action<WWW> onFail = null)
    {
        yield return www;
        if (www.error == null)
        {
            onSuccess(www);
        }
        else
        {
            Debug.Log("response error: " + www.error);
            if (onFail != null)
            {
                onFail(www);
            }
            
        }
    }
    
    IEnumerator DownLoad(WWW www)
    {
        yield return www;
        if (www.error != null)
        {
            Debug.Log("error:" + www.error);            
        }
        else
        {
            Debug.Log("down load end");
            GetComponent<Renderer>().material.mainTexture = www.texture;
        }
    }
}
