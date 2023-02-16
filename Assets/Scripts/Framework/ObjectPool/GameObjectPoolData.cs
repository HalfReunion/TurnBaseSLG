 using System.Collections.Generic;
using UnityEngine; 

public class GameObjectPoolData
{
    private Queue<GameObject> poolQueue;
    private int capacity = -1;
    private Transform rootObj;
    
    public  GameObjectPoolData(int capcity=-1)
    {
        if (capacity != -1) {
            poolQueue = new Queue<GameObject>(capacity);
            return;
        }
        poolQueue = new Queue<GameObject>();
    }

    public void Init(Transform rootObj) {
        this.rootObj = rootObj;
    }
}

