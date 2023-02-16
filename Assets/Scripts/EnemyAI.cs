using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{ 
    private enum State { 
        WaitingForEnemyTurn,
        TakingTurn,
        Busy
    } 

    private float timer;
    private State state;

    private void Start()
    {
        //TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
        EventDispatcher.Instance.SubScribe<object, EventArgs>(GameEventType.OnTurnChanged, 
            OnTurnChanged);
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn()) {
            return;
        }

        switch (state) 
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {

                    TakeEnemyAIAction(SetStateTakingTurn);
                    state = State.Busy;
                    TurnSystem.Instance.NextTurn();
                }

                break;
            case State.Busy:
                break;
        } 
    }

    private void OnTurnChanged(object sender,EventArgs e) {
        if (!TurnSystem.Instance.IsPlayerTurn()) {
            state = State.TakingTurn;
        }
        timer = 2f;
    }

    private void TakeEnemyAIAction(Action onEnemyAIActionComplete) { 
        
    }

    private void SetStateTakingTurn() {
        timer = .5f;
        state = State.TakingTurn;
    }
}
