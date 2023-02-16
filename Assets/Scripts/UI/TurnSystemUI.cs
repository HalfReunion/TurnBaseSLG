using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] Button endBtn;
    [SerializeField] Text turnText;
    [SerializeField] GameObject enemyTurnUI;

    public void Start()
    {
        endBtn.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
            
        });

        //TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
        EventDispatcher.Instance.SubScribe<object, EventArgs>(GameEventType.OnTurnChanged,
            OnTurnChanged);

        //设定忙
        UnitActionSystem.Instance.OnBusyActionChanged += SetBtnDisabled;
        updateUITurnText();
        updateEnemyTurnUI();
    }

    public void SetBtnDisabled(bool isShow) {
        endBtn.gameObject.SetActive(!isShow);
    }
    
    /// <summary>
    /// 回合开始时必定要更新的东西
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTurnChanged(object sender, EventArgs e) {
        
        updateUITurnText();
        updateEnemyTurnUI();
        SetBtnDisabled(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void updateUITurnText() {
        turnText.text = $"回合：<color='Blue'>{TurnSystem.Instance.TurnNumber}</color>";
    }

    private void updateEnemyTurnUI() { 
        enemyTurnUI.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void OnDisable()
    {
        EventDispatcher.Instance.unSubScribe<object, EventArgs>(GameEventType.OnTurnChanged,
            OnTurnChanged);
        //TurnSystem.Instance.OnTurnChanged -= OnTurnChanged; 
    }
}
