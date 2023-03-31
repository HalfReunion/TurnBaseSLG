using HalfStateFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IState = HalfStateFrame.IState;

public abstract class UISystem : ISystem
{
    protected IState Current
    {
        get { return GameMainLoop.Instance.CurrentState; }
    }

    private Dictionary<Type, GameObject> objDict = new Dictionary<Type, GameObject>();
    private Dictionary<Type, UIBase> uiDict = new Dictionary<Type, UIBase>();
    protected Transform root;

    private static TextAsset jsonTxt;

    protected string systemName;

    /// <summary>
    /// 为了让可以批量生成的UI可以生成
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetUI<T>() where T : class, IUIBase
    {
        if (uiDict.TryGetValue(typeof(T), out var uiBase))
        {
            uiBase.Init(this);
            return uiBase as T;
        }
        return null;
    }

    public void Add<T>(UIBase uiBase, GameObject obj) where T : IUIBase
    {
        if (objDict.ContainsKey(typeof(T)))
        {
            throw (new InvalidOperationException("That UI has exists"));
        }
        objDict.Add(typeof(T), obj);
        uiDict.Add(typeof(T), uiBase);
    }

    public UIBase OpenUI<T>(UI_Status uiStatus = UI_Status.Open)
    {
        if (root == null)
        {
            root = GameObject.FindGameObjectWithTag("MainCanvas").transform;
        }
        if (uiDict.TryGetValue(typeof(T), out var uiBase))
        {
            if (uiBase.UIState == UI_Status.Hide)
            {
                uiBase.Show();
                return uiBase;
            }
            Transform rootTr = root.Find(uiBase.UILevel.ToString());
            UIBase ui = GameObject.Instantiate(uiBase, rootTr);
            ui.Init(this);

            switch (uiStatus)
            {
                case (UI_Status.Hide):
                    ui.Hide();
                    break;

                case (UI_Status.Open):
                    ui.Show();
                    break;
            }

            objDict[typeof(T)] = ui.gameObject;
            uiDict[typeof(T)] = ui;
            return ui;
        }
        return null;
    }

    public void ShowUI<T>()
    {
        if (uiDict.TryGetValue(typeof(T), out var uiBase))
        {
            UIBase ui = uiBase.Show();
        }
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

    public virtual void Init()
    {
        LoadByName(systemName);
    }

    private void LoadByName(string systemName)
    {
        //ABResLoader.Instance.LoadAsset<GameObject>("ui/teammenu", child.name)

        if (jsonTxt == null) { jsonTxt = ABResLoader.Instance.LoadAsset<TextAsset>("data/custom", "ui_prefab_path"); }

        List<Deserialize_Data_UI> list = SerializeTool.DeserializeToList<Deserialize_Data_UI>(jsonTxt);

        Deserialize_Data_UI.Child_Data_UI[] result = (from a in list
                                                      where a.SystemName == systemName
                                                      select a)
                                  .FirstOrDefault().ChildUIs;

        foreach (var ui in result)
        {
            Type type = Type.GetType(ui.Name);
            GameObject gbj = ABResLoader.Instance.LoadAsset<GameObject>("ui/teammenu", ui.Name);
            UIBase io = gbj.GetComponent(type) as UIBase;
            io.UILevel = (UI_Level)Enum.Parse(typeof(UI_Level), ui.Level);
            objDict.Add(type, gbj);
            uiDict.Add(type, io);
        }
    }

    public virtual void RenderInit()
    { }
}