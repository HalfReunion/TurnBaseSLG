using UnityEngine;

public class TestUI : UISystem
{
    public override void Init()
    {
        //读取对应json，一次性加载所有和这个UI相关的资源
        systemName = "TeamMenuUI";
        base.Init();
    }
}

public class TestSer : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    public void CreateUI()
    {
        GameMainLoop.Instance.CurrentState.GetSystem<TeamMenuUISystem>().OpenUI<TeamCustomUI>();
    }
}