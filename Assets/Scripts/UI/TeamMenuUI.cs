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

    public void GetPartyData() {
        
    }
}
