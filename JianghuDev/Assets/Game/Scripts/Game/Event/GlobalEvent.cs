
using System;
using System.Collections.Generic;


public class GlobalEvent : GameEvent
{

    /// <summary>
    /// The _inst.
    /// </summary>
    private static GlobalEvent _inst = new GlobalEvent();
    public static GlobalEvent inst
    {
        get
        {
            return _inst;
        }
    }


    protected override void EventHandlerError(Exception error)
    {
        SQDebug.Print("EventHandlerError GlobalEvent\n" + error);
    }


    /// <summary>
    /// Adds the event.
    /// </summary>
    public static void add(Enum type, GameEventHandler handler, bool isUseOnce = false)
    {
        inst.AddEvent(type, handler, isUseOnce);
    }



    /// <summary>
    /// Removes the event.
    /// </summary>
    public static void remove(Enum type, GameEventHandler handler)
    {
        inst.RemoveEvent(type, handler);
    }


    /// <summary>
    /// Removes All this type of event.
    /// </summary>
    public static void remove(Enum type)
    {
        inst.RemoveEvent(type);
    }


    /// <summary>
    /// remove all event
    /// </summary>
    public static void remove()
    {
        inst.RemoveEvent();
    }


    /// <summary>
    /// Dispatch the specified type, target and args. sync type. 
    /// </summary>
    public static void dispatch(Enum type, params object[] args)
    {
        inst.DispatchEvent(type, args);
    }

    /// <summary>
    /// Dispatch the specified type, target and args. async type, in idle frame execute function
    /// </summary>
    public static void dispatchAsync(Enum type, params object[] args)
    {
        inst.DispatchAsyncEvent(type, args);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool hasEvent(Enum type)
    {
        return inst.HasEvent(type);
    }



    /// <summary>
    /// 消息循环
    /// </summary>
    public static void updateEvent()
    {
        inst.UpdateEvent();
    }







}

