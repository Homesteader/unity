using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BaseViewWidget : MonoBehaviour
{

    private const string mPathRoot = "Windows/";//路径
    private Stack<ViewListenerEventInfo> mListenerEventList;//通过AddEventListenerTarget添加的事件，
    private GameEvent mInternalEvent;
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

    #region 加载 关闭widget
    /// <summary>
    /// 加载UI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T GetWidget<T>(string path, Transform parent)
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
            return obj.GetComponent<T>();
        }
        return default(T);
    }

    /// <summary>
    /// 关闭UI
    /// </summary>
    public virtual void Close<T>()
    {
        if (BaseView.childrenWidgetDic.ContainsKey(typeof(T).Name))
        {
            GameObject go = BaseView.childrenWidgetDic[typeof(T).Name];
            BaseView.childrenWidgetDic.Remove(typeof(T).Name);
            if (go != null)
                Destroy(go);
        }
        StopAllCoroutines();
        MonoBehaviour.Destroy(this);
        MonoBehaviour.Destroy(gameObject);
    }

    public virtual void Close()
    {
        StopAllCoroutines();
        MonoBehaviour.Destroy(this);
        MonoBehaviour.Destroy(gameObject);
    }

    /// <summary>
    /// 关闭widget
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void CloseWidget<T>()
    {
        if (BaseView.childrenWidgetDic.ContainsKey(typeof(T).Name))
        {
            GameObject go = BaseView.childrenWidgetDic[typeof(T).Name];
            BaseView.childrenWidgetDic.Remove(typeof(T).Name);
            Destroy(go);
        }
    }


    /// <summary>
    /// 关闭所有的widget
    /// </summary>
    public static void CloseAllWidget()
    {
        foreach (var item in BaseView.childrenWidgetDic)
        {
            if (item.Value != null)
            {
                Destroy(item.Value);
            }
        }
        BaseView.childrenWidgetDic.Clear();
    }

    /// <summary>
    /// 关闭所有的widget，除了names里面的
    /// </summary>
    /// <param name="names">类名的集合</param>
    public static void CloseAllWidgetBut(List<string> names)
    {
        Dictionary<string, GameObject> temp = new Dictionary<string, GameObject>();
        foreach (var item in BaseView.childrenWidgetDic)
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
                    Destroy(item.Value);
                }

            }
        }
        BaseView.childrenWidgetDic.Clear();

        foreach (var item in temp)
        {
            BaseView.childrenWidgetDic.Add(item.Key, item.Value);
        }
        temp.Clear();
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
}
