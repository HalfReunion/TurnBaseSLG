using UnityEngine;

public enum GridType
{
    Normal,       //���ڵ�,�ܹ�ȥ
    LowObstacle,  //���ڵ�,�ܿ�Խ
    HightObstacle //ȫ�ڵ�,����ȥ
}

public class GridSystemVisualItem : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    private GridType gridType;

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