using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ActionPoint : MonoBehaviour
{
    private Image actionPoints;

    public void Awake()
    {
        transform.TryGetComponent(out actionPoints);
    }

    public void Resume()
    {
        actionPoints.color = Color.blue;
    }

    public void Spent()
    {
        actionPoints.color = Color.gray;
    }
}