using HalfStateFrame;

public class MainMenuState : StateBase
{
    private bool isRenderInit;

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

    public override void OnUpdate(float time)
    {
        //渲染初始化
        if (!isRenderInit)
        {
            foreach (var i in RenderInitSeq)
            {
                i.RenderInit();
                isRenderInit = true;
            }
        }
    }
}