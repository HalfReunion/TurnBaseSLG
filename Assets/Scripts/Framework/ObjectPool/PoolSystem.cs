using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolSystem : Singleton<PoolSystem>
{
    private Dictionary<Type, IGameObjectPoolData> gbjPool = new Dictionary<Type, IGameObjectPoolData>();

    public void PushGameObject<T>(GameObject t)
    {
        if (gbjPool.TryGetValue(typeof(T), out var data))
        {
            data.Push(t);
        }
    }

    public T GetGameObject<T>() where T : Component
    {
        if (gbjPool.TryGetValue(typeof(T), out var data))
        {
            return data.Get<T>();
        }
        return null;
    }

    public void RegisterPoolGameObject<T>(Transform root, GameObject obj)
    {
        if (gbjPool.TryGetValue(typeof(T), out IGameObjectPoolData gobj))
        {
            gobj.Init(root, obj);
            return;
        }
        GameObjectPoolData data = new GameObjectPoolData();
        data.Init(root, obj);
        gbjPool.Add(typeof(T), data);
    }

    public void RegisterPoolGameObject<T>(Transform root) where T : Component
    {
        if (gbjPool.TryGetValue(typeof(T), out IGameObjectPoolData obj))
        {
            obj.Init<T>(root);
            return;
        }
        GameObjectPoolData data = new GameObjectPoolData();
        data.Init<T>(root);
        gbjPool.Add(typeof(T), data);
    }

    public void ClearGameObjectPool<T>()
    {
        if (gbjPool.ContainsKey(typeof(T)))
        {
            gbjPool[typeof(T)].Clear();
            gbjPool[typeof(T)] = null;
        }
    }

    public void ClearAllGameObjectPool()
    {
        if (gbjPool.Count <= 0) return;

        foreach (var i in gbjPool)
        {
            i.Value.Clear();
        }
        gbjPool.Clear();
    }
}