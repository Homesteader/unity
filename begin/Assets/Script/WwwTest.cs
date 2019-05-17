using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwwTest : MonoBehaviour
{

    public string url = "http://images0.cnblogs.com/blog2015/686199/201505/311920537358907.jpg";
    
   
    
    public Action<WWW> onSuccess;
    // Start is called before the first frame update
    void Start()
    {
       
        //StartCoroutine(DownLoad());
        this.onSuccess += this.ResponseSuccess;
        HttpWrapper httpWrapper = GetComponent<HttpWrapper>();
        httpWrapper.GET(url,this.onSuccess);

               
        int? a = null;
        object refa = a;
        Debug.Log("refa:" + refa);
        
        int? nintA = (int?) refa;
        Debug.Log("nintA: " + nintA);
        
        int valueA = (int) refa;
        Debug.Log("valueA: " + valueA);

    }

    private void ResponseSuccess(WWW www)
    {
        if (www == null)
        {
            return;
        }
        GetComponent<Renderer>().material.mainTexture = www.texture;
    }

    IEnumerator DownLoad()
    {
        WWW www = new WWW(url);
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
