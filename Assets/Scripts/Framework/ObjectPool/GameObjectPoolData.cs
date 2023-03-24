using System.Collections.Generic;
using UnityEngine;

public interface IGameObjectPoolData
{
    public T Get<T>();
    public GameObject Get();
    public void Push(GameObject obj);

    public void Init(Transform rootObj, GameObject m_prefab);

    public void Init<T>(Transform rootObj) where T : Component;

    public void Clear();
}

public class GameObjectPoolData : IGameObjectPoolData
{
    private Queue<GameObject> poolQueue;
  
    private Transform rootObj;
    private GameObject m_prefab;

    public GameObjectPoolData(int capcity = -1)
    {
        if (capcity != -1)
        {
            poolQueue = new Queue<GameObject>(capcity);
            return;
        }
        poolQueue = new Queue<GameObject>();
    }
     

    public T Get<T>()  
    { 
        if (!poolQueue.TryDequeue(out GameObject obj))
        {
            obj = GameObject.Instantiate(m_prefab); 
        }
        obj.name = m_prefab.name;
        obj.transform.SetParent(rootObj);
        obj.SetActive(true);
        return obj.GetComponent<T>();
    }

    public GameObject Get()
    {
        if (!poolQueue.TryDequeue(out GameObject obj))
        {
            obj = GameObject.Instantiate(m_prefab);
        }
        obj.name = m_prefab.name;
        obj.transform.SetParent(rootObj);
        obj.SetActive(true);
        return obj;
    }

    public void Init(Transform rootObj,GameObject m_prefab)
    {
        this.rootObj = rootObj;
        this.m_prefab = m_prefab;
    }

    public void Init<T>(Transform rootObj) where T:Component
    {
        this.rootObj = rootObj;
        GameObject game = new GameObject();
        game.AddComponent<T>();
        game.name = typeof(T).Name; 
        this.m_prefab = game;
        Push(game);
    }

    public void Push(GameObject obj)
    {
        obj.SetActive(false); 
        obj.transform.SetParent(rootObj);
        poolQueue.Enqueue(obj); 
    }

    public void Clear() {
        m_prefab = null;
        for (int i = 0; i < rootObj.childCount; i++)
        {
            GameObject childGameObject = rootObj.GetChild(i).gameObject;
            GameObject.Destroy(childGameObject);
        }
        
    }
}