using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UIElements;

public class TeamCustom : UIBase
{
    private ScrollView partyScroll;
    private Button startBtn;
    protected override void OnInit(){
        partyScroll = transform.Find("Left/PartyView").GetComponent<ScrollView>();
        startBtn = transform.Find("Right/BottomPanel/StartBtn").GetComponent<Button>();
    }
}

