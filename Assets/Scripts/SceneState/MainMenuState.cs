using HalfStateFrame;

public class MainMenuState : StateBase
{
    public override void OnEnter(IModel message = null)
    {
        if (message != null)
        {
            RegisterModel(message);
        }
        //isRenderInit = false;
        RegisterModel(new SceneInfoModel("MainMenu"));
        RegisterSystem(new TeamCustomSystem());
        RegisterSystem(new TeamMenuUISystem()).OpenUI<TeamCustomUI>();
        RegisterSystem(new MapSelectUISystem()).OpenUI<MapSelectUI>();
        RegisterMono(new UIChar3DRender().GetAndIns());
    }

    public override void OnExit(out IModel message)
    {
        message = null;
        UnRegisterModel<SceneInfoModel>();
        UnRegisterSystem<TeamCustomSystem>();
        UnRegisterSystem<TeamMenuUISystem>();
        UnRegisterSystem<MapSelectUISystem>();
        UnRegisterMono<UIChar3DRender>();
    }
}