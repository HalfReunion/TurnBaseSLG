using Assets.Scripts.Model;
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
        Debug.Log("初始化TeamCustomSystem");
        PartyViewModel viewModel = new PartyViewModel(); 
        //PartyTeamCustomModel partyModel  = new PartyTeamCustomModel();
        UI3DCharacterData renderModel = new UI3DCharacterData();
        Current.RegisterModel(viewModel); 
        //Current.RegisterModel(partyModel);
        Current.RegisterModel(renderModel);
            
        Current.RegisterEvent<int>("OnTeamCustomChanged", OnTeamCustomToggleClick);
        Current.RegisterEvent<int>("OnTeamCustomChanged", SaveToTeamCustom);
    } 
    
    private void OnTeamCustomToggleClick(int idx) { 
        
    } 

    //写入文件
    private void SaveToTeamCustom(int idx) {
        PartyTeamCustomModel partyModel = Current.GetModel<PartyTeamCustomModel>();

    }

    public void TriggerToggleEvent(int idx) {
        Current.EventTrigger<int>("OnTeamCustomChanged", idx);
    }

    private void ConstructPartyTeamDict() { 
        
    }

    /// <summary>
    /// 返回
    /// </summary>
    public List<PartyViewModel.CharacterInfoData> GetPartyViewData() {
        return Current.GetModel<PartyViewModel>().GetValue;
    }

    public Dictionary<int,List<CharacterInfoData>> GetCustomTeamData()
    {
        return Current.GetModel<PartyTeamCustomModel>().GetValue;
    }
    public List<RenderTexture> GetRenderTextures()
    {
        return Current.GetModel<UI3DCharacterData>().GetValue;
    }

}
