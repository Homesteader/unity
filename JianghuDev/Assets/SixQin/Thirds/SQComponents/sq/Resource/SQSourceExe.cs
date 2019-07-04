using UnityEngine;
using System.Collections;

public class SQSourceExe : MonoBehaviour {

    public static SQSourceExe It;
    public delegate void SQSourceLoad(Texture2D texture);

    //
    void Awake()
    {
        It = this;
    }

	// Use this for initialization
	void Start () {
	    
	}
	
    public void LoadSource(string url, SQSourceLoad loadCall)
    {
        StartCoroutine(loadImage(url, loadCall));
    }

    IEnumerator loadImage(string url, SQSourceLoad  loadCall)
    {
        if (url != null && url.Replace(" ", "") != "")
        {
            WWW www = new WWW(url);
            while (!www.isDone)
            {
                yield return new WaitForSeconds(0.01f);
            }
            yield return www;
            if (www.texture != null && string.IsNullOrEmpty(www.error))
            {
                if(loadCall != null)
                {
                    loadCall(www.texture);
                }
            }
        }
    }

    /// <summary>
    /// 创建资源加载器
    /// </summary>
    public static void Create()
    {
        GameObject Obj = new GameObject();
        Obj.name = "SQSourceLoad";
        Obj.AddComponent<SQSourceExe>();
        GameObject.DontDestroyOnLoad(Obj);
    }

}
