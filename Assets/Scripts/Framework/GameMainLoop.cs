using HalfStateFrame;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameMainLoop : MonoSingleton<GameMainLoop>
{
    private IState m_CurrentState;

    private Dictionary<Type, IState> m_States;
     
    public IState CurrentState => m_CurrentState;

    public override void OnAwake()
    { 
        isCanNotDestory = true; 
    }

  
    private void Start()
    {
        Debug.Log("Start");
        m_States = new Dictionary<Type, IState>();
        Init();
    }

    private void Init()
    {
        //添加场景
        MainMenuState mainMenu = new MainMenuState();
        m_States.Add(typeof(MainMenuState), mainMenu);
        StartState(m_States[typeof(MainMenuState)]);
    }

    public void StartState(IState state)
    {
        m_CurrentState = state;
        m_CurrentState.OnEnter(null);
    }

    public void ChangeState(IState state)
    {
        IModel message;
        m_CurrentState.OnExit(out message);
        m_CurrentState = state;
        m_CurrentState.OnEnter(message);
    }

    private void Update()
    {
        if (m_CurrentState == null) { return; }
        m_CurrentState.OnUpdate(Time.deltaTime);
    }
}