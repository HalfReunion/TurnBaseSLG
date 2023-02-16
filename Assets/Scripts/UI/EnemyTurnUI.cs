using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class EnemyTurnUI : MonoBehaviour
{
    Text txt;
    private void Awake()
    {
        txt = GetComponent<Text>();
    }

    public void Show(string txt) {
        this.txt.text = txt;
    }

    public void Hide() { }
}
