using UnityEngine;

public class TestUI : UISystem
{
    public override void Init()
    {
        //��ȡ��Ӧjson��һ���Լ������к����UI��ص���Դ
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