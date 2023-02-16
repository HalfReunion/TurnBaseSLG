using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusyActionUI : MonoBehaviour
{
    private void Start()
    {
        UnitActionSystem.Instance.OnBusyActionChanged += OnBusyChanged;
        Hide();
    }
    public void Show() 
    { 
        gameObject.SetActive(true);
    }

    public void Hide() 
    {
        gameObject.SetActive(false);
    }

    private void OnBusyChanged(bool isBusy) {
        if (isBusy) {
            Show();
            return;
        } 
        Hide();
    }
}
