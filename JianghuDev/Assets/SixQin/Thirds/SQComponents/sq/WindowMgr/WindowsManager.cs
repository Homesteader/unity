using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindowsManager : MonoBehaviour {
    public GameObject mWindowRoot;
    public static WindowsManager Instance = null;

    public Dictionary<string, WndAttribute> mWindowDic = new Dictionary<string, WndAttribute>();

	// Use this for initialization
    void Awake()
    {
       
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public void Init()
    {
        Global.Inst.mUIRoot = mWindowRoot.transform;
        LoadingController mLoadCtr = Global.Inst.GetController<LoadingController>();
        mLoadCtr.OpenWindow();
    }
    void Start () {
        //设置为不可销毁对象
        GameObject.DontDestroyOnLoad(this);        
        //
    }

    #region 创建窗口管理器
    public static void Create()
    {
        UnityEngine.Object o = Resources.Load("Windows/WindowsRoot");
        GameObject go = GameObject.Instantiate(o) as GameObject;
        Global.Inst.mUIRoot = go.transform.GetChild(0).GetChild(0);
        go.name = "WindowManager";
    }
    #endregion

    #region 窗口打开和关闭操作

	private GameObject OpenWnd(string WndName, int type, out bool isNewWnd, bool isPreload=false)
    {
        WndAttribute OutWnd = null;
        if (!mWindowDic.TryGetValue(WndName, out OutWnd))
        {
            UnityEngine.Object o = Resources.Load("Windows/" + WndName);
            GameObject go = GameObject.Instantiate(o) as GameObject;
            go.name = WndName;
            go.transform.SetParent(mWindowRoot.transform);
			go.name = WndName;
            go.transform.transform.localPosition = new Vector3(0,0,go.transform.position.z);
            go.transform.transform.localScale = Vector3.one;
			go.SetActive(!isPreload);
            //加入窗口管理中
            OutWnd = WndAttribute.Create(WndName, type, go);
            mWindowDic.Add(WndName, OutWnd);
			isNewWnd = true;
            return go;
        }
        else
        {
            OutWnd.WndObj.gameObject.SetActive(true);
			isNewWnd = false;
        }

        return OutWnd.WndObj;
    }


    private bool CloseWnd(string WndName)
    {
        WndAttribute OutWnd = null;
        if (mWindowDic.TryGetValue(WndName, out OutWnd))
        {
            GameObject go = OutWnd.WndObj;
            if(go != null)
            {
                if(go.activeSelf){
                    go.gameObject.SetActive(false);
                }
                //删除窗口
                mWindowDic.Remove(WndName);
                GameObject.Destroy(go);
            }
        }

        return true;
    }

    /// <summary>
    /// 销毁同样类型的窗口
    /// </summary>
    /// <param name="wndType"></param>
    private void CleanWindow(int wndType)
    {
        List<WndAttribute> buffer = new List<WndAttribute>(mWindowDic.Values);
        for(int i = 0; i < buffer.Count; i++)
        {
            if (buffer[i].WndType == wndType)
            {
                CloseWnd(buffer[i].WndName);
            }
        }

    }


    private GameObject GetWindowObj(string wndName)
    {
        WndAttribute OutWnd = null;
        if (mWindowDic.TryGetValue(wndName, out OutWnd))
        {
            GameObject go = OutWnd.WndObj;
            return go;
        }

        return null;
    }

    #endregion

    #region 静态窗口接口
    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <param name="wndName"> 窗口定义名称 </param>
    /// <param name="wndType"> 窗口定义类型 </param>
    public static GameObject OpenWindow(string wndName, int wndType)
    {
		bool isnew = false;
        if(Instance != null)
        {
			return Instance.OpenWnd(wndName, (int)wndType, out isnew);
        }else
        {
            SQDebug.LogError("加载窗口出错了!");
        }

        return null;
    }


	public static GameObject OpenWindow(string wndName, int wndType, out bool isNew)
	{
		if(Instance != null)
		{
			return Instance.OpenWnd(wndName, wndType, out isNew);
		}else
		{
			SQDebug.LogError("加载窗口出错了!");
		}

		isNew = false;
		return null;
	}

    public static bool IsWindowOpen(string name)
    {
        GameObject obj = Instance.GetWindowObj(name);
        if (obj == null || !obj.activeInHierarchy)
            return false;
        return true;
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// <param name="wndName"></param>
    public static bool CloseWindow(string wndName)
    {
        if (Instance != null)
        {
            return Instance.CloseWnd(wndName.ToString());
        }
        else
        {
            SQDebug.LogError("关闭窗口出错了!实例为空NULL");
        }

        return false;
    }

    /// <summary>
    /// 清除某一类型的窗口
    /// </summary>
    /// <param name="type"></param>
    public static void CleanWindowByType(int wndType)
    {
        if (Instance != null)
        {
             Instance.CleanWindow(wndType);
        }
        else
        {
            SQDebug.LogError("清除窗口出错了!实例为空NULL");
        }
    }

    /// <summary>
    /// 不想使用单例的窗口，直接用这个
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="window"></param>
    /// <returns></returns>
    public static T GetWindow<T>(string wndName) where T : class
    {
        T t = null;
        if (Instance != null)
        {
             GameObject ob = Instance.GetWindowObj(wndName.ToString());
            if(ob != null)
            {
                t = ob.GetComponent<T>();
                return t;
            }
        }

        return t;
    }

    public static GameObject GetWindow(string wndName)
    {
        if (Instance != null)
        {
            GameObject ob = Instance.GetWindowObj(wndName.ToString());
            if (ob != null)
            {
                return ob;
            }
        }

        return null;
    }


    /// <summary>
    /// 指定窗口类型，打开窗口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="window"></param>
    /// <param name="wndType"></param>
    /// <returns></returns>
    public static T OpenWindow<T>(string window, int wndType) where T : class
    {
        T t = null;
        GameObject ob = WindowsManager.OpenWindow(window, wndType);
        if (ob != null)
        {
            t = ob.GetComponent<T>();
            return t;
        }
        return t;
    }

    public static T OpenWindow<T>(WndAttribute attr, out bool isNewWnd) where T : class
	{
		T t = null;
		GameObject ob = WindowsManager.OpenWindow(attr.WndName, attr.WndType, out isNewWnd);
		if (ob != null)
		{
			t = ob.GetComponent<T>();
			return t;
		}
		return t;
	}

    #endregion
}

#region 窗口属性 

public class WndAttribute
{
    public static WndAttribute Create(string wndName, int type, GameObject Wndobj)
    {
        return new WndAttribute(wndName, type, Wndobj);
    }
    //窗口名称
    public string WndName;
    //窗口类型
    public int WndType;
    //窗口实例
    public GameObject WndObj;


    public WndAttribute(string name, int type, GameObject obj)
    {
        WndName = name;
        WndType = type;
        WndObj = obj;
    }

}

#endregion
