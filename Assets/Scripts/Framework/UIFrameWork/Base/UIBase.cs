using Cysharp.Threading.Tasks.Triggers;
using HalfStateFrame;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;

public enum UI_Status
{
    Close,
    Open,
    Hide
}
public enum UI_Level
{
    Main,
    Game,
    Popup
}


public interface IUIBase
{
    public string UIName { get; }

    public UI_Status UIState { get; }

    public UI_Level UILevel { get; }
    public UIBase Show();
    public void Hide();
    public void Close();
}



public abstract class UIBase : MonoBehaviour, IUIBase
{

    public virtual string UIName { get { return null; } }

    public UI_Status UIState => m_Status;

    public UI_Level UILevel { get => m_Level; set => m_Level = value; }

    protected UI_Status m_Status = UI_Status.Close;
    protected UI_Level m_Level;


    protected UISystem uiSystem;

    protected Transform root;

    public void Awake()
    {
        OnInit();
    }

    public void Init(UISystem uiSystem)
    {
        if (this.uiSystem != null) { return; }
        this.uiSystem = uiSystem;
    }

    protected virtual void OnInit() { }
    public virtual void Close()
    {
        m_Status = UI_Status.Close;
        Destroy(gameObject);
    }

    public virtual void Hide()
    {
        m_Status = UI_Status.Hide;
        gameObject.SetActive(false);
    }

    public virtual UIBase Show()
    {

        if (root == null)
        {
            root = GameObject.FindGameObjectWithTag("MainCanvas").transform;
        }
        if (m_Status == UI_Status.Close)
        {
            Transform rootTr = root.Find(m_Level.ToString());
            m_Status = UI_Status.Open;
            UIBase obj = Instantiate(this, rootTr);
            return obj;
        }
        //if (layer != -1) gameObject.transform.SetSiblingIndex(layer);
        if (gameObject.activeSelf == false) gameObject.SetActive(true);

        return this;
    }


}