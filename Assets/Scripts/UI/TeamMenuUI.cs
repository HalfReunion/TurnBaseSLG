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

    public void GetPartyData() {
        
    }
}
