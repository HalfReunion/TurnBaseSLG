using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using CoreSystem; 

public enum Camp { 
    Enemy,
    Player,
    Teammate
}

public class Unit : MonoBehaviour
{
    /// <summary>
    /// 最好是针对每个Unit的更新
    /// </summary>
    public static event EventHandler OnAnyActionPointsChanged; 
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead; 
    //public static event EventHandler BuffEvent;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private Camp unitCamp;

    [SerializeField]
    private HealthSystem healthSystem;


    public HealthSystem HealthSystem => healthSystem;

    public Camp UnitCamp => unitCamp;

    [SerializeField]
    private MoveAction moveAction;
     

    private BaseAction[] baseActionArray;
    //目前角色踩着的格子信息
    private GridPos gridPos;

    [SerializeField]
    private int actionPoints;

    [SerializeField]
    private int currentActionPoints;

    public int CurrentActionPoints => currentActionPoints;
    public int ActionPoints => actionPoints;
     

    public BaseAction[] BaseActionArray => baseActionArray;

    public MoveAction MAction
    {
        get
        {
            if (moveAction == null)
            {
                moveAction = gameObject.GetComponent<MoveAction>();
            }
            return moveAction;
        }
    }

   

    private void Awake()
    {
        //anim = GetComponentInChildren<Animator>();
        //targetPosition = transform.position;
        healthSystem = GetComponent<HealthSystem>();

        baseActionArray = GetComponents<BaseAction>(); 
    }

    private void Start()
    {
        gridPos = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitByGridPosition(gridPos,this);
        //UnitActionSystem.Instance.OnSelectedUnitChanged += OnSelectedUnitChanged;
         
        //TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
        EventDispatcher.Instance.SubScribe<object, EventArgs>(GameEventType.OnTurnChanged,
            OnTurnChanged);

        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this,EventArgs.Empty);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitByGridPosition(GetGridPosition(), this);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);

        Destroy(gameObject);
    }

    private void Update()
    {  
        //根据角色位置获得当前站的格子
        GridPos _gridPos = LevelGrid.Instance.GetGridPosition(transform.position);
        if (_gridPos != gridPos) { 
            //角色走到了对应格子上
            LevelGrid.Instance.UnitMoveToGridPosition(this, gridPos, _gridPos);
            gridPos = _gridPos;
        }
    }

   

    public GridPos GetGridPosition() { 
        return gridPos;
    }

    public Vector3 GetWorldPosition() {
        return transform.position;
    }

    //判断该点位是否在可移动范围内
    public bool IsVaildGridPos(GridPos gridPos) { 
        return LevelGrid.Instance.IsVaildGridPos(gridPos);
    }
     

    public GridPos Vector2GridPos(Vector3 v) {
        return LevelGrid.Instance.GetGridPosition(v);
    }

  

    /// <summary>
    /// 判断是否有东西踩在格子上
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    public bool IsUnitOnGridPos(GridPos gridPos)
    {
        return LevelGrid.Instance.IsUnitInGrid(gridPos);
    }


    public bool CanControl() {
        if (unitCamp != Camp.Player) return false; 
            return true;
    }

    public void Damage(int damageAmount) {
        healthSystem.Damage(damageAmount);
    }

    #region 行动点相关

    /// <summary>
    /// 是否足够点数
    /// </summary>
    /// <param name="baseAction"></param>
    /// <returns></returns>
    public bool IsEnoughActionPointsToTakeAction(BaseAction baseAction)
    {
        if (currentActionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 消耗行动点
    /// </summary>
    /// <param name="amount"></param>
    private void SpendActionPoints(int amount)
    {
        if (this.currentActionPoints >= amount) { 
            this.currentActionPoints -= amount;

            //广播所有涉及行动点改动的事件
            //OnAnyActionPointsChanged(this,EventArgs.Empty);

            Debug.Log($"已扣除{amount}点,剩余{currentActionPoints}点");
            return;
        }
        Debug.Log("点数不足");
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction,Action<int> uiChanged) {
        if (IsEnoughActionPointsToTakeAction(baseAction)){ 
            SpendActionPoints(baseAction.GetActionPointsCost());
            uiChanged(currentActionPoints);
            return true;
        }

        return false;
    }


    public void GetBuff(List<BaseBuff> buffs) {
        foreach (var item in buffs) { 
            BuffManager.Instance.AddBuff(this, item);
        }
    }

    #endregion

    #region 响应方法

   
    private void OnSelectedUnitChanged(object obj, EventArgs args)
    {
        MAction.GetVaildActionGridPositionList();
    }

    
    private void OnTurnChanged(object sender,EventArgs e) { 
        //初始化
        if((unitCamp==Camp.Enemy&&TurnSystem.Instance.CurrentTurn==Camp.Enemy)
            ||
            (unitCamp == Camp.Player && TurnSystem.Instance.CurrentTurn == Camp.Player))
        currentActionPoints = actionPoints;
        OnAnyActionPointsChanged?.Invoke(this,EventArgs.Empty);
    }

    #endregion
}
