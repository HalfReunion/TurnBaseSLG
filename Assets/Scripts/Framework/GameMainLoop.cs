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
        SoundManager.Instance.Init();
        SettingInfo.Init();
        m_States = new Dictionary<Type, IState>();
        Init();
    }

    private void Init()
    {
        //添加场景
        MainMenuState mainMenu = new MainMenuState();
        FightSceneState fightScene = new FightSceneState();
        LoadingSceneState loadingScene = new LoadingSceneState();
        m_States.Add(typeof(MainMenuState), mainMenu);
        m_States.Add(typeof(FightSceneState), fightScene);
        m_States.Add(typeof(LoadingSceneState), loadingScene);
        StartState(m_States[typeof(MainMenuState)]);
    }

    public void StartState(IState state)
    {
        m_CurrentState = state;
        m_CurrentState.OnEnter(null);
    }

    public void ChangeState(Type state)
    {
        m_CurrentState.OnExit(out var message);
        m_CurrentState = m_States[state];
        m_CurrentState.OnEnter(message);
    }

    private void Update()
    {
        if (m_CurrentState == null) { return; }
        m_CurrentState.OnUpdate(Time.deltaTime);
    }
}