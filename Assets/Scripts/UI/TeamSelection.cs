using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TeamSelection : UIBase
{
    private Image charIcon;
    private TMP_Text charName;
    protected override void OnInit() {
         
        charIcon = transform.Find("CharIcon").GetComponent<Image>();
        charName = transform.Find("CharName").GetComponent<TMP_Text>();
    }

    public void SetData(PartyViewModel.CharacterInfoData ch) {
        charIcon.sprite = ch.Icon;
        charName.text = ch.Name;
    }

}
