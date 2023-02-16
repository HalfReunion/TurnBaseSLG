using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T: MonoSingleton<T>
{

    public static T instance;
    public static T Instance { get { return instance; } }

    protected bool isCanNotDestory;

    public void Awake()
    {
        if (instance != null)
        {
           Destroy(gameObject);
        }
        instance = this as T;
        
        OnAwake();
        if (isCanNotDestory) {
            DontDestroyOnLoad(gameObject);
        }

        
        
    }

    public virtual void OnAwake() { 
    }
}

