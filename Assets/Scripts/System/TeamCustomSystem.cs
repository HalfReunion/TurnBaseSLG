using HalfStateFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static PartyViewModel;

public class TeamCustomSystem : SystemBase
{   
    
    protected override void OnInit()
    {
        Debug.Log("≥ı ºªØTeamCustomSystem");
        PartyViewModel viewModel = new PartyViewModel(); 
        PartyTeamCustomModel partyModel  = new PartyTeamCustomModel();
         
        Current.RegisterModel(viewModel); 
        Current.RegisterModel(partyModel);
        Current.RegisterEvent<int>("OnTeamCustomChanged", OnTeamCustomToggleClick);
        
    }

    private void OnTeamCustomToggleClick(int idx) { 
        
    }

    public void TriggerToggleEvent(int idx) {
        Current.EventTrigger<int>("OnTeamCustomChanged", idx);
    }

    private void ConstructPartyTeamDict() { 
        
    }

    /// <summary>
    /// ∑µªÿ
    /// </summary>
    public List<PartyViewModel.CharacterInfoData> GetPartyViewData() {
        return Current.GetModel<PartyViewModel>().GetValue;
    }

    public Dictionary<int,List<CharacterInfoData>> GetCustomTeamData()
    {
        return Current.GetModel<PartyTeamCustomModel>().GetValue;
    }


}
