using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    void Start()
    {
        

    }

    public void CreateUI() {
        GameMainLoop.Instance.CurrentState.GetSystem<TeamMenuUI>().OpenUI<TeamCustom>();
    }
}
