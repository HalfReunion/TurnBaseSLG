
using CoreSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;
using HealthSystem = CoreSystem.HealthSystem;

public class BuffManager : MonoBehaviour
{
    #region 资源
    [ShowInInspector]
    public List<BuffData> buffDatas;
    #endregion


    public static BuffManager Instance => instance; 

    //广播buff的UI变化
    public event EventHandler<Dictionary<Unit, List<BaseBuff>>> BroadBuffChangedUI;

    [ShowInInspector,LabelText("Buff归属字典")]
    private Dictionary<Unit, List<BaseBuff>> buffUnitList;
     
    private static BuffManager instance;
    
    //通过UnitManager获得Buff栏 UI的位置
    private Transform BuffPanelTran; 

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        buffUnitList = new Dictionary<Unit, List<BaseBuff>>();
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnOverChanged += TurnSystem_TurnOverExecuteAllBuff;
    }

    public void AddBuff<T>(Unit unit, T buff) where T : BaseBuff
    {
        List<BaseBuff> list;
        //找不到角色
        if (!buffUnitList.TryGetValue(unit, out list))
        {
            list = new List<BaseBuff>();
            buffUnitList.Add(unit, list);
        }

        BaseBuff temp = list.Find((t) => { return t.ToString().Equals(buff.ToString()); });
        if (temp != null)
        {
            temp.Refresh(this, EventArgs.Empty);
            return;
        }

        list.Add(buff);
        buff.Execute(unit.HealthSystem, EventArgs.Empty); 
        BroadBuffChangedUI(this, buffUnitList);
    }

    /// <summary>
    /// 回合结束时结算
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="units"></param>
    public void TurnSystem_TurnOverExecuteAllBuff(object obj, EventArgs e)
    {
        Debug.Log("执行Buff");
        List<BaseBuff> buffs; 
        List<Unit> units =  (e as TurnSystem.TurnChangedMessage)?.units;

        if (units == null) return;

        for (int i = 0;i<units.Count;i++)
        { 
            Unit item = units[i];
            if (buffUnitList.TryGetValue(item, out buffs))
            {
                for (int j= 0;j<buffs.Count;j++)
                {
                    var n = buffs[j];
                    n.Execute(item.HealthSystem, EventArgs.Empty);
                    if (n.IsExpire)
                    {
                        Debug.Log($"删除Buff:{n.ToString()}");
                        n.Restore(item.HealthSystem, EventArgs.Empty);
                        buffUnitList[item].Remove(n);
                        continue;
                    }
                   
                }
            }
        }
        BroadBuffChangedUI(this, buffUnitList);
    }

    
}
