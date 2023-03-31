using System;
using TMPro;
using UnityEngine;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Unit unit;

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}