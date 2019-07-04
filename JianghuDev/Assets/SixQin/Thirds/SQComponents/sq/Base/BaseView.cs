using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

internal struct ViewListenerEventInfo
{
    public IGameEvent target;
    public Enum type;
    public GameEventHandler handler;
}

public class BaseView : MonoBehaviour, IGameEvent
{
    public List<BaseController> mPreControllerList = new List<BaseController>();

    /// <summary>
    /// 全部显示的view
    /// </summary>
    public static Dictionary<string, BaseView> showViewDic = new Dictionary<string, BaseView>();

    private Stack<ViewListenerEventInfo> mListenerEventList;//通过AddEventListenerTarget添加的事件，
    private GameEvent mInternalEvent;
    protected string mName;  //view的名字
    private const string mPathRoot = "Windows/";//路径

    /// <summary>
    /// 所属的widget
    /// </summary>
    public static Dictionary<string, GameObject> childrenWidgetDic = new Dictionary<string, GameObject>();

    #region unity方法
    protected virtual void Awake()
    {
        mListenerEventList = new Stack<ViewListenerEventInfo>();
        mInternalEvent = new GameEvent();
    }

    protected virtual void OnEnable()
    {
    }


    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        mInternalEvent.UpdateEvent();
    }

    protected virtual void OnDisable()
    {
    }

    protected virtual void OnDestroy()
    {
        ClearEventListenerTarget();//删除监听的事件
        mInternalEvent.Dispose();
    }
    #endregion


    #region delay Run
    private IEnumerator DelayRunFunc(float delay, CallBack func)
    {
        if (delay != 0)
            yield return new WaitForSeconds(delay);
        else
            yield return new WaitForEndOfFrame();
        if (this.gameObject && null != func)
            func();
    }

    /// <summary>
    /// 延迟几秒后执行
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="func"></param>
    public virtual void DelayRun(float delay, CallBack func)
    {
        if (delay <= 0)
            func();
        else if (gameObject.activeInHierarchy)
            StartCoroutine(DelayRunFunc(delay, func));
    }

    /// <summary>
    /// 下一帧执行
    /// </summary>
    /// <param name="callback"></param>
    public virtual void DelayNextFrameRun(CallBack callback)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayNextFrameToRun(callback));
    }
    private IEnumerator DelayNextFrameToRun(CallBack callback)
    {
        yield return 1;
        callback();
    }

    public virtual void DelayRun(CallBack func)
    {
        if (this == null)
            return;

        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayRunFunc(0, func));
        else
            func();
    }


    #endregion

    #region event
    public void ClearEventListenerTarget()
    {
        while (mListenerEventList.Count > 0)
        {
            ViewListenerEventInfo info = mListenerEventList.Pop();
            if (info.target != null && !info.target.IsDispose && info.target.ToString() != "null")
                info.target.RemoveEvent(info.type, info.handler);
        }
    }
    public void AddEventListenerTarget(IGameEvent target, Enum type, GameEventHandler handler, bool isUseOnce = false, bool isFirst = false)
    {
        mListenerEventList.Push(new ViewListenerEventInfo()
        {
            handler = handler,
            type = type,
            target = target
        });
        target.AddEvent(type, handler, isUseOnce, isFirst);
    }

    public void AddEvent(Enum type, GameEventHandler handler, bool isUseOnce = false, bool isFirst = false)
    {
        mInternalEvent.AddEvent(type, handler, isUseOnce);
    }

    public void DispatchEvent(Enum type, params object[] args)
    {
        mInternalEvent.DispatchEvent(type, args);
    }

    public void DispatchAsyncEvent(Enum type, params object[] args)
    {
        mInternalEvent.DispatchAsyncEvent(type, args);
    }

    public bool HasEvent(Enum type)
    {
        return mInternalEvent.HasEvent(type);
    }

    public void RemoveEvent(Enum type, GameEventHandler handler)
    {
        mInternalEvent.RemoveEvent(type, handler);
    }

    public void RemoveEvent(Enum type)
    {
        mInternalEvent.RemoveEvent(type);
    }

    public void RemoveEvent()
    {
        mInternalEvent.RemoveEvent();
    }

    public virtual bool IsDispose
    {
        get
        {
            return mInternalEvent.IsDispose;
        }
    }
    #endregion


    #region 是否存在某个view
    /// <summary>
    /// 是否存在某个view
    /// </summary>
    /// <param name="name">view 名字</param>
    /// <returns></returns>
    public static bool ContainsView(string name)
    {
        if (showViewDic.ContainsKey(name) && showViewDic[name] != null)
            return true;
        return false;
    }

    /// <summary>
    /// 是否存在某个view
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool ContainsView<T>() where T : BaseView
    {
        string name = typeof(T).Name;
        return (ContainsView(name));
    }

    #endregion


    #region 创建和关闭view

    public void AddView(string name)
    {
        mName = name;
        if (!showViewDic.ContainsKey(name))
            showViewDic.Add(name, this);
        else
            showViewDic[name] = this;
    }

    /// <summary>
    /// 关闭所有界面
    /// </summary>
    public static void CloseAllView()
    {
        foreach (var item in showViewDic)
        {
            if (item.Value != null)
            {
                Destroy(item.Value);
            }
        }
        showViewDic.Clear();
    }

    /// <summary>
    /// 关闭所有的界面
    /// </summary>
    /// <param name="names">累名</param>
    public static void CloseAllViewBut(List<string> names)
    {
        Dictionary<string, BaseView> temp = new Dictionary<string, BaseView>();
        foreach (var item in showViewDic)
        {
            if (item.Value != null)
            {
                if (names.Contains(item.Key))
                {
                    temp.Add(item.Key, item.Value);
                    continue;
                }
                else
                {
                    Destroy(item.Value.gameObject);
                }
            }
        }
        showViewDic.Clear();
        foreach (var item in temp)
        {
            showViewDic.Add(item.Key, item.Value);
        }
        temp.Clear();
    }

    /// <summary>
    /// 关闭view
    /// </summary>
    public virtual void Close()
    {
        try
        {
            StopAllCoroutines();
            Dispose();
        }
        catch (Exception error)
        {
            SQDebug.Log(this.name + "  " + error);
        }
    }

    public virtual void Dispose()
    {
        if (!showViewDic.ContainsKey(name))
            MonoBehaviour.Destroy(this);
        MonoBehaviour.Destroy(gameObject);
    }
    #endregion

    #region 加载widget
    /// <summary>
    /// 加载UI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T GetWidget<T>(string path, Transform parent) where T : BaseViewWidget
    {
        path = mPathRoot + path;
        GameObject prefab = Assets.LoadPrefab(path);
        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab);
            if (parent != null)
            {
                obj.transform.parent = parent;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
            if (!childrenWidgetDic.ContainsKey(typeof(T).Name))
            {
                childrenWidgetDic.Add(typeof(T).Name, obj);
            }
            else
                childrenWidgetDic[typeof(T).Name] = obj;
            return obj.GetComponent<T>();
        }
        return default(T);
    }
    #endregion

    #region 上一页控制器相关
    /// <summary>
    /// 设置上一页的Controller
    /// </summary>
    public void SetPreController(BaseController pre)
    {
        if (mPreControllerList.Contains(pre))
        {
            return;
        }
        else
        {
            mPreControllerList.Add(pre);
        }
    }

    public void SetPreController(List<BaseController> list)
    {
        mPreControllerList = list;
    }

    public List<BaseController> GetPreController()
    {
        if (mPreControllerList != null)
        {
            return mPreControllerList;
        }
        else
        {
            return null;
        }
    }

    public void GotoPreView()
    {
        if (mPreControllerList != null)
        {
            for (int i = 0; i < mPreControllerList.Count; i++)
            {
                mPreControllerList[i].OpenWindow();
            }
        }
        mPreControllerList.Clear();
    }

    public void HideAllPreView()
    {
        if (mPreControllerList != null)
        {
            for (int i = 0; i < mPreControllerList.Count; i++)
            {
                mPreControllerList[i].HideWindow();
            }
        }
        mPreControllerList.Clear();
    }

    public void CloseAllPreView()
    {
        if (mPreControllerList != null)
        {
            for (int i = 0; i < mPreControllerList.Count; i++)
            {
                mPreControllerList[i].CloseWindow();
            }
        }
        mPreControllerList.Clear();
    }

    #endregion
}
