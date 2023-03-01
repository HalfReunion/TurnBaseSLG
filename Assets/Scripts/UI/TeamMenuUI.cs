using System.Collections.Generic;

public class TeamMenuUI : UISystem
{
    TeamCustomSystem teamCustom ;
    public override void Init()
    {
        //��ȡ��Ӧjson��һ���Լ������к����UI��ص���Դ 
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
