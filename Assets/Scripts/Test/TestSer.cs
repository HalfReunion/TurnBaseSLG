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
        //读取对应json，一次性加载所有和这个UI相关的资源 
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
