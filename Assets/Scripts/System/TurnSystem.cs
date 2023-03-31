using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public class TurnChangedMessage : EventArgs
    {
        public List<Unit> units;
    }

    private static TurnSystem instance;
    private int turnNumber = 1;

    public int TurnNumber => turnNumber;

    [SerializeField]
    private Camp currentTurn;

    public Camp CurrentTurn => currentTurn;
    private List<Camp> turnList;

    public static TurnSystem Instance => instance;

    public event EventHandler OnTurnChanged;

    public event EventHandler OnTurnOverChanged;

    public event EventHandler OnTurnStartChanged;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        turnList = new List<Camp>() { Camp.Player, Camp.Enemy };
        currentTurn = turnList[(turnNumber - 1) % turnList.Count];
    }

    public void NextTurn()
    {
        Debug.Log($"回合：{currentTurn.ToString()} 即将结束");

        List<Unit> units = UnitManager.Instance.GetUnitListByTurn(currentTurn);
        TurnChangedMessage message = new TurnChangedMessage() { units = units };

        OnTurnOverChanged?.Invoke(this, message);

        turnNumber++;
        currentTurn = turnList[(turnNumber - 1) % turnList.Count];
        Debug.Log($"进入 {currentTurn.ToString()} 回合");

        //OnTurnChanged?.Invoke(this,EventArgs.Empty);
        //OnTurnStartChanged?.Invoke(this, message);
        EventDispatcher.Instance.DispatchEvent(GameEventType.OnTurnChanged, this, message);
        EventDispatcher.Instance.DispatchEvent(GameEventType.OnTurnStartChanged, this, message);
    }

    public bool IsPlayerTurn()
    {
        if (currentTurn != Camp.Player) return false;
        return true;
    }
}