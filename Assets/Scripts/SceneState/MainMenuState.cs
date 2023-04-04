using HalfStateFrame;

public class MainMenuState : StateBase
{
    public override void OnEnter(MessageHandlerBase message = null)
    {
        if (message != null)
        {
            foreach (var i in message.GetValue) { 
                RegisterModel(i);
            }
        }
        //isRenderInit = false;
        RegisterModel(new SceneInfoModel("MainMenu"));
        RegisterSystem(new TeamCustomSystem());
        RegisterSystem(new TeamMenuUISystem()).OpenUI<TeamCustomUI>();
        RegisterSystem(new MapSelectUISystem()).OpenUI<MapSelectUI>();
        RegisterMono(new UIChar3DRender().GetAndIns());
    }

    public override void OnExit(out MessageHandlerBase message)
    {
        OutMainMenuMessage outMainMenuMessage = new OutMainMenuMessage();
        outMainMenuMessage.Init(GetModel<TeamStageOutPutModel>()); 
        message = outMainMenuMessage;
        UnRegisterModel<SceneInfoModel>();
        UnRegisterSystem<TeamCustomSystem>();
        UnRegisterSystem<TeamMenuUISystem>();
        UnRegisterSystem<MapSelectUISystem>();
        UnRegisterMono<UIChar3DRender>(); 
    }
}