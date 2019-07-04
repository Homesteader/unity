using System;
using System.Collections.Generic;
using System.Threading;


public interface IGameEvent
{
    void AddEvent(Enum type, GameEventHandler handler, bool isUseOnce = false, bool isFirst = false);
    void RemoveEvent(Enum type, GameEventHandler handler);
    void RemoveEvent(Enum type);
    void RemoveEvent();
    void DispatchEvent(Enum type, params object[] args);
    void DispatchAsyncEvent(Enum type, params object[] args);
    bool HasEvent(Enum type);
    void Dispose();
    bool IsDispose
    {
        get;
    }

}

public interface IGameEventArgs
{
    bool IsCancelDefaultAction
    {
        get;
        set;
    }


}


/// <summary>
/// Event handler.
/// external event function format
/// </summary>
public delegate void GameEventHandler(params object[] args);

/// <summary>
/// event execute
/// </summary>
public struct GameEventInfo
{
    public GameEventHandler eventHandler;
    public object[] args;
}

public class GameEvent : IGameEvent
{
    /// <summary>
    /// The same event id max number.
    /// </summary>
    public static ushort SameSyncEventMax = 10;


    /// <summary>
    /// event execute list
    /// </summary>
    private List<GameEventInfo> mAsyncEventList;


    /// <summary>
    /// The event list. 
    /// </summary>
    protected Dictionary<Enum, List<GameEventHandler>> mEventDic;

    /// <summary>
    /// just use once of event
    /// </summary>
    protected Dictionary<Enum, List<GameEventHandler>> mUseOnceEventDic;

    /// <summary>
    /// 
    /// </summary>
    public virtual bool IsDispose
    {
        get;
        set;
    }




    public GameEvent()
    {
        IsDispose = false;
        mAsyncEventList = new List<GameEventInfo>();
        mEventDic = new Dictionary<Enum, List<GameEventHandler>>();
        mUseOnceEventDic = new Dictionary<Enum, List<GameEventHandler>>();
    }


    /// <summary>
    /// on execute event error
    /// </summary>
    protected virtual void EventHandlerError(Exception error)
    {
        throw new Exception("No Catch Exception : ", error);
    }


    /// <summary>
    /// Adds the event.
    /// </summary>
    public virtual void AddEvent(Enum type, GameEventHandler handler, bool isUseOnce = false, bool isFirst = false)
    {
        List<GameEventHandler> handlerList = mEventDic.ContainsKey(type) ? mEventDic[type] : null;
        if (handlerList == null)
        {
            mEventDic[type] = new List<GameEventHandler>();
        }
        if (mEventDic[type].Contains(handler))
            return;

        if (isFirst)
            mEventDic[type].Insert(0, handler);
        else
            mEventDic[type].Add(handler);

        if (isUseOnce)
        {
            if (!mUseOnceEventDic.ContainsKey(type))
                mUseOnceEventDic.Add(type, new List<GameEventHandler>());
            mUseOnceEventDic[type].Add(handler);
        }
    }



    /// <summary>
    /// Removes the event.
    /// </summary>
    public virtual void RemoveEvent(Enum type, GameEventHandler handler)
    {
        List<GameEventHandler> handlerList = mEventDic.ContainsKey(type) ? mEventDic[type] : null;
        if (handlerList != null && handlerList.Contains(handler))
            handlerList.Remove(handler);

        if (mUseOnceEventDic.ContainsKey(type) && mUseOnceEventDic[type].Contains(handler))
            mUseOnceEventDic[type].Remove(handler);
    }


    /// <summary>
    /// Removes All this type of event.
    /// </summary>
    public virtual void RemoveEvent(Enum type)
    {
        if (mEventDic.ContainsKey(type))
            mEventDic.Remove(type);
        if (mUseOnceEventDic.ContainsKey(type))
            mUseOnceEventDic.Remove(type);
    }


    /// <summary>
    /// remove all event
    /// </summary>
    public virtual void RemoveEvent()
    {
        mEventDic.Clear();
        mUseOnceEventDic.Clear();
    }


    /// <summary>
    /// Dispatch the specified type, target and args. sync type. 
    /// </summary>
    public virtual void DispatchEvent(Enum type, params object[] args)
    {
        List<GameEventHandler> handlerList = mEventDic.ContainsKey(type) ? mEventDic[type] : null;
        if (handlerList != null && HasEvent(type))
        {
            GameEventHandler[] temp = new GameEventHandler[handlerList.Count];
            handlerList.CopyTo(temp);
            for (short i = 0; i < temp.Length; i++)
            {
                GameEventHandler handler = temp[i];
                if (mUseOnceEventDic.ContainsKey(type) && mUseOnceEventDic[type].Contains(handler))
                    RemoveEvent(type, handler);
                try
                {
                    handler(args);
                }
                catch (Exception err1)
                {
                    try
                    {
                        EventHandlerError(err1);
                    }
                    catch (Exception err2)
                    {
                        throw new Exception("Event Handler Error Exception : \n", err2);
                    }
                }

                if (args.Length > 0 && args[0] is IGameEventArgs && (args[0] as IGameEventArgs).IsCancelDefaultAction)
                    break;
            }
        }
    }

    /// <summary>
    /// Dispatch the specified type, target and args. async type, in idle frame execute function
    /// 抛出异步事件， 将事件加入异步事件列表，然后执行
    /// </summary>
    public virtual void DispatchAsyncEvent(Enum type, params object[] args)
    {
        List<GameEventHandler> handlerList = mEventDic.ContainsKey(type) ? mEventDic[type] : null;
        if (handlerList != null && HasEvent(type))
        {
            GameEventHandler[] temp = new GameEventHandler[handlerList.Count];
            handlerList.CopyTo(temp);
            for (short i = 0; i < temp.Length; i++)
            {
                mAsyncEventList.Add(new GameEventInfo()
                {
                    args = args,
                    eventHandler = temp[i]
                });
                if (mUseOnceEventDic.ContainsKey(type) && mUseOnceEventDic[type].Contains(temp[i]))
                {
                    RemoveEvent(type, temp[i]);
                }
            }
        }
    }

    /// <summary>
    /// Dispatch the specified type, target and args. in new child thread execute function
    /// 开辟子线程执行事件
    /// </summary>
    [System.Obsolete("Do not use temporary")]
    public virtual void DispatchThreadEvent(Enum type, object args)
    {
        ///////////////
        List<GameEventHandler> handlerList = mEventDic.ContainsKey(type) ? mEventDic[type] : null;
        if (handlerList != null && HasEvent(type))
        {
            GameEventHandler[] temp = new GameEventHandler[handlerList.Count];
            handlerList.CopyTo(temp);
            for (short i = 0; i < temp.Length; i++)
            {
                Thread thread = new Thread((object arg) => temp[i]());
                thread.Start(args);
                if (mUseOnceEventDic.ContainsKey(type) && mUseOnceEventDic[type].Contains(temp[i]))
                {
                    RemoveEvent(type, temp[i]);
                    i--;
                }
            }
        }
    }



    /// <summary>
    /// 是否有 type 类型的事件
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public virtual bool HasEvent(Enum type)
    {
        return mEventDic.ContainsKey(type);
    }


    /// <summary>
    /// 消息循环
    /// </summary>
    public virtual void UpdateEvent()
    {
        for (short i = 500; mAsyncEventList.Count > 0 && i > 0; i--)
        {
            int count = mAsyncEventList.Count;
            GameEventInfo taskEvent = mAsyncEventList[count - 1];
            mAsyncEventList.Remove(taskEvent);
            try
            {
                taskEvent.eventHandler(taskEvent.args);
            }
            catch (Exception error)
            {
                try
                {
                    EventHandlerError(error);
                }
                catch (Exception err)
                {
                    throw new Exception("Event Handler Error Exception : \n", err);
                }
            }
        }
    }


    /// <summary>
    /// 销毁，回收处理
    /// </summary>
    public virtual void Dispose()
    {
        IsDispose = true;
        mUseOnceEventDic.Clear();
        mEventDic.Clear();
        mAsyncEventList.Clear();
        mAsyncEventList = null;
        mEventDic = null;
        mUseOnceEventDic = null;
    }



}










