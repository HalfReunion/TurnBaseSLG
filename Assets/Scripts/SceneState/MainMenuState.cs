using HalfStateFrame;

public class MainMenuState : StateBase
{ 
    public override void OnEnter(IModel message = null)
    {
        if (message != null)
        {
            RegisterModel(message);
        }
        isRenderInit = false;
        RegisterSystem(new TeamCustomSystem());
        RegisterSystem(new TeamMenuUISystem()).OpenUI<TeamCustomUI>();
        RegisterMono(new UIChar3DRender().GetAndIns());
    }

    public override void OnExit(out IModel message)
    {
        message = null;
    } 
   
}