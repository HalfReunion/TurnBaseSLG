using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{ 
    public static event EventHandler OnBeforeConfirm;

    public static event EventHandler OnAnyActionCompleted;

    public static event EventHandler<GridPos> OnConfirmAction;
    
    //该回合已经执行过命令
    protected bool hasTaken;

    public bool HasTaken => hasTaken;
    protected int actionPoint { get; set; }

    protected Animator anim;
    protected bool isActive = false;
    protected Action onActionComplete;
    protected Action onActionStart;
    protected Unit unit;

    public Unit BelongUnit => unit;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        unit = GetComponent<Unit>();
        hasTaken = false;
        actionPoint = 1;
    }

    protected virtual void Start()
    {
        //TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        EventDispatcher.Instance.SubScribe<object, EventArgs>(GameEventType.OnTurnChanged,
            TurnSystem_OnTurnChanged);
    }

    public abstract string GetActionName();


   

    public virtual void ConfirmAction(GridPos mouseGridPosition,Action onActionStart ,Action onActionComplete)
    { 
        this.onActionComplete = onActionComplete;
        this.onActionStart = onActionStart;
        this.onActionStart += CostAction; 
        OnBeforeConfirm?.Invoke(this, EventArgs.Empty); 
        OnConfirmAction?.Invoke(this, mouseGridPosition); 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    public virtual bool IsValidActionGridPos(GridPos gridPos)
    { 
        List<GridPos> moveGridPosList = GetVaildActionGridPositionList();
        if (moveGridPosList != null)
        {
            return moveGridPosList.Contains(gridPos);
        }
        return false;
    }

    public virtual int GetActionPointsCost() {
        return actionPoint;
    }

    public void CostAction() {
        hasTaken = true;
    }

    public void ResetAction()
    {
        hasTaken = false;
    }

    public void ResetActionPoint()
    {
        actionPoint = 1;
    }

    /// <summary>
    /// 动作开始时的初始化
    /// </summary>
    /// <param name="onActionComplete"></param>
    protected virtual void ActionStart() 
    {
        onActionStart();
        isActive = true; 
    }

    protected void ActionComplete() 
    {
        CostAction();
        
        isActive = false; 
        onActionComplete();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
        ResetActionPoint();
    }
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        ResetAction();
    } 

    public abstract void TakeAction(GridPos gridPos);
    public abstract List<GridPos> GetVaildActionGridPositionList();
}
