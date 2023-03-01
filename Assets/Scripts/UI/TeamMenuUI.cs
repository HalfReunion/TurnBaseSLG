using System.Collections.Generic;

public class TeamMenuUI : UISystem
{
    TeamCustomSystem teamCustom ;
    public override void Init()
    {
        //读取对应json，一次性加载所有和这个UI相关的资源 
        systemName = "TeamMenuUI";
        teamCustom = Current.GetSystem<TeamCustomSystem>();
        base.Init(); 
    }

    public TeamSelection GetSelectComponent() {
        TeamSelection selction = GetUI<TeamSelection>();
        return selction;
    }

    public List<PartyViewModel.CharacterInfoData> GetCustomTeamData()
    {
        return teamCustom.GetPartyViewData();
    }

    public List<PartyViewModel.CharacterInfoData> GetPartyData() {
        return teamCustom.GetPartyViewData();
    }
}
