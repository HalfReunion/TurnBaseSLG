using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridType { 
    Normal,       //���ڵ�,�ܹ�ȥ
    LowObstacle,  //���ڵ�,�ܿ�Խ
    HightObstacle //ȫ�ڵ�,����ȥ
}

public class GridSystemVisualItem : MonoBehaviour
{   
    [SerializeField] MeshRenderer meshRenderer;

    GridType gridType;

    private void Awake()
    {
        
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }

    public void Show(Material material)
    {
        meshRenderer.material = material;
        meshRenderer.enabled = true;
    }
    // Start is called before the first frame update

}
