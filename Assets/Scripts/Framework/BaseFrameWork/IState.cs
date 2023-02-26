﻿ 
using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HalfStateFrame
{
    public interface IGetState{
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
        public void RegisterEvent<TParam>(Action<TParam> ev);
        public TSystem RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem;

        public void EventTrigger<TEvent, TParam>(TParam t) where TEvent : EventItem<TParam>;

        public TModel GetModel<TModel>() where TModel : IModel;

        public TSystem GetSystem<TSystem>() where TSystem : ISystem;

        public TMono GetMono<TMono>() where TMono : IMono;

        public TMono RegisterMono<TMono>(TMono mono) where TMono : IMono;
    }


    public abstract class StateBase : IState
    {
        Dictionary<Type, ISystem> systems = new Dictionary<Type, ISystem>();
        Dictionary<Type, IEventItem> events = new Dictionary<Type, IEventItem>();
        Dictionary<Type, IModel> models = new Dictionary<Type, IModel>(); 
        Dictionary<Type, IMono> monos = new Dictionary<Type, IMono>();

        protected IState lastState;

        public void Init()
        {
            foreach (var item in systems)
            {
                item.Value.Init();
            }
        } 

        public abstract void OnUpdate(float time);
        public abstract void OnEnter(IModel message);
        public abstract void OnExit(out IModel message);

        public void SetLastState(IState lastState) {
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

        public void RegisterEvent<TParam>(Action<TParam> ev) 
        {   
            Type type = ev.GetType();
            if (!events.ContainsKey(type))
            {
                EventItem<TParam> item = new EventItem<TParam>();
                item.RegisterEvent(ev);
                Debug.Log($"RegisterEvent:{type.Name}");
            }
            
        }

        public void EventTrigger<TEvent, TParam>(TParam t) where TEvent : EventItem<TParam> 
        {
            Type type = typeof(TEvent);
            if (events.TryGetValue(type, out var eventItem))
            {
                ((TEvent)eventItem).Trigger(t);
            }
        }

        public TSystem RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            Type type = system.GetType();
            if (!systems.ContainsKey(type))
            {
                systems.Add(type, system);
                system.Init();
                Debug.Log($"RegisterSystem:{type.Name}");
            }
            return system;
        }

        public TModel GetModel<TModel>() where TModel:IModel
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
            if (systems.TryGetValue(type, out var system)) {
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

        public TMono RegisterMono<TMono>(TMono mono) where TMono :IMono
        {
            Type type = mono.GetType();
            if (!systems.ContainsKey(type))
            {
                monos.Add(type, mono);
                Debug.Log($"RegisterMono:{type.Name}");
            }
            return mono;
        }
    }

}