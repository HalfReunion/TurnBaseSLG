using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TeamSelection : UIBase
{
    private Image charIcon;
    private TextMeshPro charName;
    protected override void OnInit() {
        charIcon = transform.Find("CharIcon").GetComponentInChildren<Image>();
        charName = transform.Find("CharName").GetComponentInChildren<TextMeshPro>();
    }


}
