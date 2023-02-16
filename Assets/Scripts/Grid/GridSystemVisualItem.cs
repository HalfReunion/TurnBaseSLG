using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridType { 
    Normal,       //无遮挡,能过去
    LowObstacle,  //半遮挡,能跨越
    HightObstacle //全遮挡,过不去
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
