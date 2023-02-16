using System;
using System.Collections.Generic;

namespace HalfStateFrame{ 
public interface IModule
{
    public T GetSystem<T>() where T : class, ISystem;

    public void RegisterSystem<T>(T system) where T : ISystem;

    public void SendCommand<T>(T command) where T : ICommand;

    public void RegisterEvent<T>(Action<T> act);

    public void SendEvent<T>(T t);
}

public abstract class ModuleBase<T> : IModule where T : ModuleBase<T>, new()
{
    private List<ISystem> systems = new List<ISystem>(); 

    private Dictionary<Type, object> entries;

    private Dictionary<Type, IEventItem> eventDic;

    public abstract void Init();

    private static T instance;

    public static T Instance => instance;

    static ModuleBase()
    {
        if (instance == null)
        {
            instance = new T();
            instance.entries = new Dictionary<Type, object>();
            foreach (var it in instance.systems)
            {
                it.Init();
            }
        }
    }

    public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
    {
        if (entries.TryGetValue(typeof(TSystem), out var io))
        {
            return io as TSystem;
        }
        return null;
    }

    public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
    {
        if (!entries.ContainsKey(typeof(TSystem)))
        {
            entries.Add(typeof(TSystem), system);
            systems.Add(system);
        }
        system.Init();
    }
  
    public void SendCommand<TCommand>(TCommand command) where TCommand : ICommand
    {
        command.SetModule(this);
        command.Execute();
    } 

    public void RegisterEvent<TParam>(Action<TParam> act)
    {
        //EventItem<TAction> eventItem = new EventItem<TAction>();
        if (eventDic.TryGetValue(typeof(TParam), out var acti)) {
            ((EventItem<TParam>)acti).RegisterEvent(act);
            return;
        }
        EventItem<TParam> eventItem = new EventItem<TParam>();
        eventDic.Add(typeof(TParam),eventItem);
        eventItem.RegisterEvent(act);  
    }

    public void SendEvent<TParam>(TParam t)
    {
        if (eventDic.TryGetValue(typeof(TParam), out var acti))
        {
            //acti.Trigger(t);
            return;
        }
    }
}
}