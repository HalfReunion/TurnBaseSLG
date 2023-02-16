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
    /// ��������ÿ��Unit�ĸ���
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
    //Ŀǰ��ɫ���ŵĸ�����Ϣ
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
        //���ݽ�ɫλ�û�õ�ǰվ�ĸ���
        GridPos _gridPos = LevelGrid.Instance.GetGridPosition(transform.position);
        if (_gridPos != gridPos) { 
            //��ɫ�ߵ��˶�Ӧ������
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

    //�жϸõ�λ�Ƿ��ڿ��ƶ���Χ��
    public bool IsVaildGridPos(GridPos gridPos) { 
        return LevelGrid.Instance.IsVaildGridPos(gridPos);
    }
     

    public GridPos Vector2GridPos(Vector3 v) {
        return LevelGrid.Instance.GetGridPosition(v);
    }

  

    /// <summary>
    /// �ж��Ƿ��ж������ڸ�����
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

    #region �ж������

    /// <summary>
    /// �Ƿ��㹻����
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
    /// �����ж���
    /// </summary>
    /// <param name="amount"></param>
    private void SpendActionPoints(int amount)
    {
        if (this.currentActionPoints >= amount) { 
            this.currentActionPoints -= amount;

            //�㲥�����漰�ж���Ķ����¼�
            //OnAnyActionPointsChanged(this,EventArgs.Empty);

            Debug.Log($"�ѿ۳�{amount}��,ʣ��{currentActionPoints}��");
            return;
        }
        Debug.Log("��������");
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

    #region ��Ӧ����

   
    private void OnSelectedUnitChanged(object obj, EventArgs args)
    {
        MAction.GetVaildActionGridPositionList();
    }

    
    private void OnTurnChanged(object sender,EventArgs e) { 
        //��ʼ��
        if((unitCamp==Camp.Enemy&&TurnSystem.Instance.CurrentTurn==Camp.Enemy)
            ||
            (unitCamp == Camp.Player && TurnSystem.Instance.CurrentTurn == Camp.Player))
        currentActionPoints = actionPoints;
        OnAnyActionPointsChanged?.Invoke(this,EventArgs.Empty);
    }

    #endregion
}
