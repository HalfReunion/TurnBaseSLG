
using Cysharp.Threading.Tasks.Triggers;
using HalfStateFrame;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public void Show();
    public void Hide();
    public void Close();
}



public abstract class UIBase : MonoBehaviour, IUIBase
{
    public virtual string UIName { get { return null; } }

    public  UI_Status UIState => m_Status;

    public UI_Level UILevel { get => m_Level; set => m_Level = value; }

    protected UI_Status m_Status = UI_Status.Close;
    protected UI_Level m_Level;

    
    protected Transform root;

    public void Awake()
    { 
        OnInit();
    }

    protected virtual void OnInit(){}
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

    public virtual void Show()
    {
        
        if (root == null) {
            root = GameObject.FindGameObjectWithTag("MainCanvas").transform;
        }
        
        if (m_Status == UI_Status.Close)
        {
            Transform temp = root.Find(m_Level.ToString());
            Instantiate(gameObject, temp);
        }
        m_Status = UI_Status.Open;
        if (gameObject.activeSelf != false) return;
        //if (layer != -1) gameObject.transform.SetSiblingIndex(layer);
        gameObject.SetActive(true); 
    }

    
}
 