
using System;
using System.Collections.Generic; 
using UnityEngine;
using HalfStateFrame; 
using Newtonsoft.Json;
using Unity.VisualScripting;
 
using Cysharp.Threading.Tasks;
using System.Linq;

using IState = HalfStateFrame.IState;
public abstract class UISystem: ISystem
{
    protected IState Current
    {
        get { return GameMainLoop.Instance.CurrentState; }
    }
    Dictionary<Type,GameObject> objDict = new Dictionary<Type, GameObject>();
    Dictionary<Type, UIBase> uiDict = new Dictionary<Type, UIBase>();

    private static TextAsset jsonTxt;

    protected string systemName;

    /// <summary>
    /// 为了让可以批量生成的UI可以生成
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public UIBase GetUI<T>() where T:IUIBase {
        
        if (uiDict.TryGetValue(typeof(T),out var uiBase)) {
            return uiBase;
        }
        uiBase.Init(this);
        return null;
    }

    public void Add<T>(UIBase uiBase,GameObject obj) where T: IUIBase
    {
        if (objDict.ContainsKey(typeof(T))) {
            throw (new InvalidOperationException("That UI has exists")); 
        }
        objDict.Add(typeof(T), obj); 
        uiDict.Add(typeof(T), uiBase);
    }

    public UIBase OpenUI<T>() {
        
        if (uiDict.TryGetValue(typeof(T), out var uiBase))
        {
            uiBase.Init(this);
            uiBase.Show();
        }
         
        return uiBase;
    }

    public void HideUI<T>()
    {
        if (uiDict.TryGetValue(typeof(T), out var uiBase))
        {
            uiBase.Hide();
        }
    }

    public void CloseUI<T>()
    {
        if (uiDict.TryGetValue(typeof(T), out var uiBase))
        {
            uiDict.Remove(typeof(T));
            uiBase.Close();
        }
    }

    public virtual void Init() {
        LoadByName(systemName);
    }

    private void LoadByName(string systemName) {

        //ABResLoader.Instance.LoadAsset<GameObject>("ui/teammenu", child.name)


        if (jsonTxt == null) { jsonTxt = ABResLoader.Instance.LoadAsset<TextAsset>("data/custom", "ui_prefab_path"); }

        List<Deserialize_Data_UI> list = SerializeTool.DeserializeToList<Deserialize_Data_UI>(jsonTxt);

        Deserialize_Data_UI.Child_Data_UI[] result = (from a in list
                                  where a.SystemName == systemName select a)
                                  .FirstOrDefault().ChildUIs;

        foreach (var ui in result) 
        { 
            Type type = Type.GetType(ui.Name);
            GameObject gbj = ABResLoader.Instance.LoadAsset<GameObject>("ui/teammenu", ui.Name);
            UIBase io = gbj.GetComponent(type) as UIBase;
            io.UILevel = (UI_Level)Enum.Parse(typeof(UI_Level),ui.Level) ; 
            objDict.Add(type, gbj);
            uiDict.Add(type, io);

            //临时
            break;
    }
}
}

