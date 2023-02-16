using DG.Tweening;
 
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ShootAction : BaseAction
{
    public class OnShootEventArgs : EventArgs {
        public Unit StartUnit;
        public Unit TargetUnit;
        public Action<int> OnDamage;
        public Action<List<BaseBuff>> MakeBuff;
        internal List<BaseBuff> BuffList;
        public int Damage;
        
         
    } 
    public EventHandler OnShootStart;
    public EventHandler<OnShootEventArgs> OnShooting; 
    public EventHandler OnShootEnd;


    private List<BaseBuff> canMakeBuffs;

    private enum State
    {
        Aiming,
        Shooting,
        Cooloff
    }
    protected override void Start()
    {
        base.Start();
        canMakeBuffs = new List<BaseBuff>();
        BaseBuff def_down = new Def_Down_Defbuff();
        canMakeBuffs.Add(def_down);
    }

    private Unit targetUnit;

    public Unit TargetUnit => targetUnit;

    [SerializeField]
    private int maxShootDistance = 4;
    

    [SerializeField]
    private float stateTimer; 
    private bool canShootBullet;

    private State state;

    [SerializeField]
    private BaseBuff baseBuff;

    private void Update()
    {
        if (!isActive) return;

        stateTimer -= Time.deltaTime;

        switch (state) {
            case State.Aiming: 
                OnShootStart(this,EventArgs.Empty); 
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f; 
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if (canShootBullet) {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
                OnShootEnd(this, EventArgs.Empty);
                
                break;
        }
       

        if (stateTimer <= 0f) NextState();

    }

    private void Shoot() {
        OnShooting(this, new OnShootEventArgs()
        {
            StartUnit = unit,
            TargetUnit = targetUnit,
            OnDamage = targetUnit.Damage,
            MakeBuff = targetUnit.GetBuff,
            Damage = 10,
            BuffList = canMakeBuffs
        }); 
    }
    
    
   

    private void NextState() {
        switch (state)
        {
            case State.Aiming: 
                state = State.Shooting;
                //动画的时长
                float shootingStateTime = 1f;
                stateTimer = shootingStateTime; 
                break;
            case State.Shooting: 
                state = State.Cooloff;
                //动画的时长
                float coolOffStateTime = 0.3f;
                stateTimer = coolOffStateTime;
                break;
            case State.Cooloff:  
                //state = State.Cooloff;
                //isActive = false;
                ActionComplete();
                break;
        }

    }
    public override List<GridPos> GetVaildActionGridPositionList()
    {
        List<GridPos> validGridPositionList = new List<GridPos>();

        GridPos unitGridPosition = unit.GetGridPosition();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++) {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++) {
                GridPos offsetGridPosition = new GridPos(x,z);
                GridPos testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsVaildGridPos(testGridPosition)) continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (testDistance > maxShootDistance) continue;
                 
                //没有目标在格子上就跳过
                if (!LevelGrid.Instance.IsUnitInGrid(testGridPosition)) continue;

                Unit target = LevelGrid.Instance.GetUnitByGridPosition(testGridPosition);

                if (target.UnitCamp == unit.UnitCamp) continue;
                 
                //添加到结果
               validGridPositionList.Add(testGridPosition);
            }
        }
       
        return validGridPositionList;
    }
    public Unit GetTargetUnit() {
        return targetUnit;
    }
    public override void TakeAction(GridPos gridPos)
    {
   
        ActionStart(); 
        state = State.Aiming;
        //瞄准动画的时长
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;
           
        canShootBullet = true; 
    }
    /// <summary>
    /// 确认界面
    /// </summary>
    /// <param name="mouseGridPosition"></param>
    /// <param name="onActionStart"></param>
    /// <param name="onActionComplete"></param>
    public override void ConfirmAction(GridPos mouseGridPosition, Action onActionStart, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitByGridPosition(mouseGridPosition);
        Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
         
        Quaternion quaternion = Quaternion.LookRotation(aimDir);
        transform.DORotateQuaternion(quaternion, 1f); 
        base.ConfirmAction(mouseGridPosition, onActionStart, onActionComplete);

    }

    public override string GetActionName()
    {
        return "Shoot";
    }
}
