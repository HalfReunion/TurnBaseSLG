using System;
using System.Collections.Generic;
using UnityEngine;

namespace HalfStateFrame
{
    public interface IGetState
    {
        IState CurrentState { get; }
    }

    public interface IGetSystem
    {
    }

    public interface IState
    {
        public void OnUpdate(float time);

        public void OnEnter(IModel message);

        public void OnExit(out IModel message);

        public TModel RegisterModel<TModel>(TModel model) where TModel : IModel;
        public void RegisterEvent(string ev, Action act);
        public void RegisterEvent<TParam>(string ev, Action<TParam> act);
         
        public void RegisterEvent<TP1, TP2>(string ev, Action<TP1,TP2> act);
        
        public TSystem RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem;

        public void EventTrigger<TParam>(string evn, TParam t) ;

        public void EventTrigger<TP1, TP2>(string evn, TP1 t1,TP2 t2);

        public TModel GetModel<TModel>() where TModel : IModel;

        public TSystem GetSystem<TSystem>() where TSystem : ISystem;

        public TMono GetMono<TMono>() where TMono : IMono;

        public TMono RegisterMono<TMono>(TMono mono) where TMono : IMono;
    }

    public abstract class StateBase : IState
    {
        private Dictionary<Type, ISystem> systems = new Dictionary<Type, ISystem>();
        private Dictionary<string, IEventItem> events = new Dictionary<string, IEventItem>();
        private Dictionary<Type, IModel> models = new Dictionary<Type, IModel>();
        private Dictionary<Type, IMono> monos = new Dictionary<Type, IMono>();

        protected LinkedList<ISystem> RenderInitSeq = new LinkedList<ISystem>();

        protected IState lastState;

        

        public abstract void OnUpdate(float time);

        public abstract void OnEnter(IModel message);

        public abstract void OnExit(out IModel message);

        public void SetLastState(IState lastState)
        {
            this.lastState = lastState;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TModel"> Model<信息类型> </typeparam>
        /// <param name="model"></param>
        public TModel RegisterModel<TModel>(TModel model) where TModel : IModel
        {
            Type type = model.GetType();
            if (!models.ContainsKey(type))
            {
                model.Init();
                models.Add(type, model);
                Debug.Log($"RegisterModel:{type.Name}");
            }
            return model;
        }

        public void RegisterEvent<TParam>(string evn, Action<TParam> act)
        { 
            if (events.TryGetValue(evn, out var val)) 
            {
                (val as EventItem<TParam>).RegisterEvent(act);
                Debug.Log($"RegisterAct");
                return;
            }
            EventItem<TParam> ev = new EventItem<TParam>();
            ev.RegisterEvent(act);
            events.Add(evn, ev); 
            Debug.Log($"RegisterEvent:{ev}");
        }

        public void RegisterEvent<TP1, TP2>(string evn, Action<TP1, TP2> act)
        {
            if (events.TryGetValue(evn, out var val))
            {
                (val as EventItem<TP1,TP2>).RegisterEvent(act);
                Debug.Log($"RegisterAct");
                return;
            }
            EventItem<TP1,TP2> ev = new EventItem<TP1, TP2>();
            ev.RegisterEvent(act);
            events.Add(evn, ev);
            Debug.Log($"RegisterEvent:{ev}");
        }

        public void EventTrigger<TParam>(string evn,TParam t) 
        {
            if (events.TryGetValue(evn, out var eventItem))
            {
                (eventItem as EventItem<TParam>).Trigger(t);
                return;
            }

        }
        public void EventTrigger<TP1, TP2>(string evn, TP1 t1, TP2 t2)
        {
            if (events.TryGetValue(evn, out var eventItem))
            {
                (eventItem as EventItem<TP1,TP2>).Trigger(t1,t2);
                return;
            }
        }
        public TSystem RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            Type type = system.GetType();
            if (!systems.ContainsKey(type))
            {
                systems.Add(type, system);
                system.Init();
                
                RenderInitSeq.AddLast(system);
                Debug.Log($"RegisterSystem:{type.Name}");
            }
            return system;
        }

        public TModel GetModel<TModel>() where TModel : IModel
        {
            Type type = typeof(TModel);
            if (models.TryGetValue(type, out var model))
            {
                return (TModel)model;
            }
            return default(TModel);
        }

        public TSystem GetSystem<TSystem>() where TSystem : ISystem
        {
            Type type = typeof(TSystem);
            if (systems.TryGetValue(type, out var system))
            {
                return (TSystem)system;
            }
            return default(TSystem);
        }

        public TMono GetMono<TMono>() where TMono : IMono
        {
            Type type = typeof(TMono);
            if (systems.TryGetValue(type, out var mono))
            {
                return (TMono)mono;
            }
            return default(TMono);
        }

        public TMono RegisterMono<TMono>(TMono mono) where TMono : IMono
        {
            Type type = mono.GetType();
            if (!monos.ContainsKey(type))
            {
                mono.Init();
                monos.Add(type, mono);
                Debug.Log($"RegisterMono:{type.Name}");
            }
            return mono;
        }

        public void RegisterEvent(string ev, Action act)
        {
            if (events.TryGetValue(ev, out var eventItem))
            {
                (eventItem as EventItem).Trigger();
                return;
            }
        }
    }
}