//------------------------------------------------------------------------
// |                                                                   |
// | by:Qcbf                                                           |
// |                                       |
// |                                                                   |
//-----------------------------------------------------------------------

using System;
using UnityEngine;
using System.Collections.Generic;


public class SetTimeout : MonoBehaviour
{

    public struct CallbackInfo
    {
        public CallBack call;
        public float dely;
        public float endTime;
        public int times;
        public bool ignoretimescale;
        public string flag;
    }
    private static List<CallbackInfo> mCallbackList = new List<CallbackInfo>();

    private static List<CallBack> m_CallBackUpdateList = new List<CallBack>();


    public static void add(float delay, CallBack handler, int times, bool ignoretimescale = true)
    {
        CallbackInfo temp = new CallbackInfo();
        temp.call = handler;
        temp.dely = delay;
        temp.times = times;
        temp.ignoretimescale = ignoretimescale;
        if (delay > 0 || times > 1)
            temp.endTime = (ignoretimescale ? Time.realtimeSinceStartup : Time.time) + delay;
        temp.flag = null;
        mCallbackList.Add(temp);
    }

    public static void add(float delay, CallBack handler, bool once = true, bool ignoretimescale = true)
    {
        add(null, delay, handler, once, ignoretimescale);
    }

    public static void add(string flag, float delay, CallBack handler, bool once = true, bool ignoretimescale = true)
    {
        CallbackInfo temp = new CallbackInfo();
        temp.call = handler;
        temp.dely = delay;
        temp.times = once ? 1 : int.MaxValue;
        temp.ignoretimescale = ignoretimescale;
        if (delay > 0 || !once)
            temp.endTime = (ignoretimescale ? Time.realtimeSinceStartup : Time.time) + delay;
        temp.flag = flag;
        mCallbackList.Add(temp);
    }

    public static void remove(CallBack handler, bool isAll = false)
    {
        for (int i = 0; i < mCallbackList.Count; i++)
        {
            if (mCallbackList[i].call == handler)
            {
                mCallbackList.RemoveAt(i);
                if (!isAll)
                    break;
            }
        }
    }

    public static void Clear()
    {
        mCallbackList.Clear();
    }


    public static void Clear(string flag)
    {
        mCallbackList.RemoveAll((target) =>
        {
            return target.flag == flag;
        });
    }

    public static bool contains(CallBack handler)
    {
        for (int i = 0; i < mCallbackList.Count; i++)
        {
            if (mCallbackList[i].call == handler)
            {
                return true;
            }
        }
        return false;
    }

    public static void AddUpdate(CallBack handler)
    {
        if (!m_CallBackUpdateList.Contains(handler))
            m_CallBackUpdateList.Add(handler);
    }
    public static void RemoveUpdate(CallBack handler)
    {
        m_CallBackUpdateList.Remove(handler);
    }



    private void Update()
    {
        for (int i = 0; i < mCallbackList.Count; i++)
        {
            float runTime = mCallbackList[i].ignoretimescale ? Time.realtimeSinceStartup : Time.time;
            if (mCallbackList[i].call == null)
            {
                mCallbackList.RemoveAt(i);
                i--;
            }
            else if (runTime >= mCallbackList[i].endTime)
            {
                CallbackInfo info = mCallbackList[i];
                if (mCallbackList[i].times > 1)
                {
                    info.endTime += info.dely;
                    info.times--;
                    mCallbackList[i] = info;
                }
                else
                {
                    mCallbackList.RemoveAt(i);
                    i--;
                }
                info.call();
            }
        }

        for (int i = 0; i < m_CallBackUpdateList.Count; ++i)
        {
            m_CallBackUpdateList[i]();
        }



    }

}



