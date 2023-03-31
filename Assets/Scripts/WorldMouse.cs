using UnityEngine;

public class WorldMouse : MonoBehaviour
{
    private RaycastHit hit;

    public static WorldMouse Instance;

    [SerializeField]
    private LayerMask layerMask;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position = GetPosition();
    }

    public Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, float.MaxValue, layerMask);
        return hit.point;
    }
}