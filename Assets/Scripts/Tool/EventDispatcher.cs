using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public delegate void Act<in T1, in T2>(T1 t1, T2 t2);
public delegate void Act<in T1>(T1 t1);
public delegate void Act();
public enum GameEventType {
    OnTurnChanged,
    OnTurnOverChanged,
    OnTurnStartChanged
}

public class EventDispatcher 
{
    private static EventDispatcher instance;
    public static EventDispatcher Instance
    {
        get { 
            if(instance == null) instance = new EventDispatcher();
            return instance;
        }
    }
    //待会使用列表来存储
    public Dictionary<int, Delegate> dispatchs = new Dictionary<int, Delegate>();

    //注册
    public void SubScribe<T1, T2>(GameEventType type, Act<T1, T2> @delegate)
    {
        Delegate del = null;
        if (dispatchs.TryGetValue((int)type, out del))
        {
            del = del != null ? Delegate.Combine(del, @delegate) : @delegate;
            dispatchs[(int)type] = del;
        }
        else
        {
            dispatchs.Add((int)type, @delegate);
        }
    }

    //解绑
    public void unSubScribe<T1, T2>(GameEventType type, Act<T1, T2> @delegate)
    {
        Delegate del = null;
        if (dispatchs.TryGetValue((int)type, out del))
        {
            del = Delegate.Remove(del, @delegate);
            dispatchs[(int)type] = del;
        }
    }

    //执行无参委托
    public void DispatchEvent(GameEventType type)
    {
        Delegate del = null;
        if (dispatchs.TryGetValue((int)type, out del) && del != null)
        {
            var temp = (Act)del;
            temp();
        }
    }

    //执行1个参数的委托
    public void DispatchEvent<T1>(GameEventType type, T1 t1)
    {
        Delegate del = null;
        if (dispatchs.TryGetValue((int)type, out del) && del != null)
        {
            var tmp = (Act<T1>)del;
            tmp(t1);
        }
    }

    //执行2个参数的委托
    public void DispatchEvent<T1, T2>(GameEventType type, T1 t1, T2 t2)
    {
        Delegate del = null;
        if (dispatchs.TryGetValue((int)type, out del) && del != null)
        {
            var tmp = (Act<T1, T2>)del;
            tmp(t1, t2);
        }
    }

    public void RemoveDelegate(GameEventType type)
    {
        Delegate del = null;
        if (dispatchs.ContainsKey((int)type))
            dispatchs[(int)type] = del;
    }

    public void DispatchClear()
    {
        dispatchs.Clear();
    }
}