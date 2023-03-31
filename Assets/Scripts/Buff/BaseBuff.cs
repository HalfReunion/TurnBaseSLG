using CoreSystem;
using System;
using UnityEngine;

public enum BuffType
{
    Poison,
    Heal,
    Status
}

public enum UnitType
{
    Static,
    Percent
}

public abstract class BaseBuff
{
    protected BuffData buffData;
    protected int currentTurn;
    protected bool isActive;
    protected bool isExpire;

    public bool IsExpire => isExpire;

    public abstract void Execute(object obj, EventArgs e);

    public abstract void Restore(object obj, EventArgs e);

    public abstract void Refresh(object obj, EventArgs e);

    public BaseBuff()
    {
        isActive = false;
        isExpire = false;
    }

    public override string ToString()
    {
        return buffData.BuffName;
    }
}

public class Def_Down_Defbuff : BaseBuff
{
    public Def_Down_Defbuff() : base()
    {
        buffData = BuffManager.Instance.buffDatas.Find((t) => { return t.BuffName.Equals("Def_Down_Defbuff"); });
        currentTurn = buffData.lastTurn;
    }

    public override void Execute(object obj, EventArgs e)
    {
        HealthSystem healthSystem = obj as HealthSystem;
        if (!healthSystem) return;

        if (!isActive && !isExpire)
        {
            isActive = true;
            healthSystem.ReduceDef(buffData.value);
            return;
        }

        currentTurn--;

        Debug.Log("回合数为" + currentTurn);
        if (currentTurn <= 0)
        {
            Debug.Log("回合到了");
            isExpire = true;
        }
    }

    public override void Refresh(object obj, EventArgs e)
    {
        if (isActive)
        {
            Debug.Log("刷新时间");
            currentTurn = buffData.lastTurn;
        }
    }

    public override void Restore(object obj, EventArgs e)
    {
        if (isActive)
        {
            Debug.Log("Buff恢复");
            isActive = false;
            isExpire = true;
            HealthSystem healthSystem = obj as HealthSystem;
            healthSystem.RestoreDef();
        }
    }
}