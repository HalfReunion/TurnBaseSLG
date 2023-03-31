using System;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;

    public event Action<bool> UnitSelected;

    private MeshRenderer meshRender;

    private void Awake()
    {
        meshRender = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        //Èç¹û
        if (UnitActionSystem.Instance.GetSelectedUnit() == unit)
        {
            UnitSelected(true);
            meshRender.enabled = true;
        }
        else
        {
            UnitSelected(false);
            meshRender.enabled = false;
        }
    }

    private void OnDestroy()
    {
    }
}