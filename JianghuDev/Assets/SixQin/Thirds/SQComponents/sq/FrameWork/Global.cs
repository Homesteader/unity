using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public class Global : MonoBehaviour
{
    public Transform mUIRoot;//uiroot
    public static Global Inst;
    //当前网络ID
    private int mCurSessionId = 0;

    private Dictionary<string, BaseController> ControllerDic = new Dictionary<string, BaseController>();
    #region controller

    #endregion

    void Awake()
    {
        Inst = this;
        GameObject.DontDestroyOnLoad(this);
    }


    void Start()
    {

    }

    public void RegisterController<T>() where T : new()
    {
        T t = new T();
        string name = typeof(T).Name;
        if (!ControllerDic.ContainsKey(name))
            ControllerDic.Add(name, t as BaseController);
    }

    public T GetController<T>() where T : BaseController
    {
        string name = typeof(T).Name;
        BaseController bc = null;
        if (ControllerDic.TryGetValue(name, out bc))
            return bc as T;
        return null;

    }

    /// <summary>
    /// 连接游戏服务器的
    /// </summary>
    /// <param name="call">Call.</param>
	public void ConnectServer(Action<bool> call)
    {

        NetProcess.Connect(GameManager.Instance.Ip, GameManager.Instance.port, (isok, sessionId) =>
        {

            if (isok)
            {
                mCurSessionId = sessionId;
            }
            //
            if (call != null)
            {
                call(isok);
            }

        });


    }

    /// <summary>
    /// 限制 最小
    /// </summary>
    /// <param name="t">要限制的数</param>
    /// <param name="min"> 最小的值</param>
    /// <returns></returns>
    public int LimitMin(int t, int min)
    {
        if (t >= min)
        {
            return t;
        }
        else return min;
    }
}

